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
        sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(10), errorNumbersToAdd: null);
    }));

builder.Services.AddHostedService<KafkaConsumerService>();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<WarehouseDbContext>();

    for (int i = 0; i < 10; i++)
    {
        try
        {
            Console.WriteLine($"[DB INIT] Tentativo connessione e migrazione DB {i + 1}/10...");

            context.Database.Migrate();

            if (!context.Products.Any())
            {
                Console.WriteLine("[DB SEED] Inserimento prodotto di test...");
                context.Products.Add(new Warehouse.Repository.Entities.Product
                {
                    Name = "Prodotto Test Automatico",
                    Price = 100,
                    StockQuantity = 50,
                    SupplierName = "Sistema Automatico"
                });
                context.SaveChanges();
                Console.WriteLine("[DB SEED] Prodotto inserito.");
            }

            Console.WriteLine("[DB INIT] SUCCESSO! Database migrato e connesso.");
            break;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DB INIT] Fallito: {ex.Message}. Riprovo tra 5 secondi...");
            System.Threading.Thread.Sleep(5000);
        }
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();
app.Run();