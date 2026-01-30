using Microsoft.EntityFrameworkCore;
using Warehouse.Repository;
using Warehouse.Business;
using Warehouse.Business.Interfaces;
using Warehouse.WebApi.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<WarehouseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
    {
        // Questo serve per le query, ma non basta per l'avvio
        sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(10), errorNumbersToAdd: null);
    }));

builder.Services.AddHostedService<KafkaConsumerService>();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// --- BLOCCO "ANTI-CRASH" PER IL DB ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<WarehouseDbContext>();

    // Proviamo a connetterci per 10 volte (circa 50 secondi totali)
    for (int i = 0; i < 10; i++)
    {
        try
        {
            Console.WriteLine($"[DB INIT] Tentativo connessione DB {i + 1}/10...");
            context.Database.EnsureCreated();
            Console.WriteLine("[DB INIT] SUCCESSO! Database connesso.");
            break; // Se ci riesce, esce dal ciclo
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DB INIT] Fallito: {ex.Message}. Riprovo tra 5 secondi...");
            System.Threading.Thread.Sleep(5000); // Aspetta 5 secondi
        }
    }
}
// -------------------------------------

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();
app.Run();