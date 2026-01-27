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
    options.UseSqlServer(builder.Configuration.GetConnectionString("OrderDb")));

builder.Services.AddScoped<Order.Business.Kafka.IKafkaProducerService, Order.Business.Kafka.KafkaProducerService>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddHttpClient<IWarehouseClient, WarehouseClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7135");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
