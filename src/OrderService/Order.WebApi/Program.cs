using Microsoft.EntityFrameworkCore;
using Order.Business;
using Order.Business.Interfaces;
using Order.Repository;
using Warehouse.ClientHttp;
using Warehouse.ClientHttp.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(10), errorNumbersToAdd: null);
    }));

builder.Services.AddScoped<Order.Business.Kafka.IKafkaProducerService, Order.Business.Kafka.KafkaProducerService>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddHttpClient<IWarehouseClient, WarehouseClient>(client =>
{
    client.BaseAddress = new Uri("http://warehouse-service:8080");
});

var app = builder.Build();

// --- BLOCCO "ANTI-CRASH" PER IL DB ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<OrderDbContext>();

    for (int i = 0; i < 10; i++)
    {
        try
        {
            Console.WriteLine($"[DB INIT] Tentativo connessione DB {i + 1}/10...");
            context.Database.EnsureCreated();
            Console.WriteLine("[DB INIT] SUCCESSO! Database connesso.");
            break;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DB INIT] Fallito: {ex.Message}. Riprovo tra 5 secondi...");
            System.Threading.Thread.Sleep(5000);
        }
    }
}
// -------------------------------------

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();
app.Run();