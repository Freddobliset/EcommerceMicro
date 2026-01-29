using Confluent.Kafka;
using Newtonsoft.Json;
using Warehouse.Business.Interfaces;
using Warehouse.Shared.Events;

namespace Warehouse.WebApi.BackgroundServices
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly string _bootstrapServers = "localhost:9092";
        private readonly string _topic = "order-created-topic";
        private readonly string _groupId = "warehouse-group";
        private readonly IServiceScopeFactory _scopeFactory;

        public KafkaConsumerService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield(); 
            var config = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId = _groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                SocketTimeoutMs = 60000,
                SessionTimeoutMs = 30000,
            };

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();

                    consumer.Subscribe(_topic);

                    Console.WriteLine($"[KAFKA CONSUMER] Connesso! In ascolto su: {_topic}");

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            var result = consumer.Consume(stoppingToken);

                            if (result != null && result.Message != null)
                            {
                                var jsonMessage = result.Message.Value;
                                Console.WriteLine($"[KAFKA CONSUMER] Ricevuto: {jsonMessage}");
                                ProcessMessage(jsonMessage);
                            }
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"[KAFKA WARN] Errore durante il consumo: {e.Error.Reason}");
                            await Task.Delay(1000, stoppingToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[KAFKA ERROR] Kafka non raggiungibile. Riprovo tra 5 secondi... ({ex.Message})");

                    await Task.Delay(5000, stoppingToken);
                }
            }
        }

        private void ProcessMessage(string jsonMessage)
        {
            try
            {
                var orderEvent = JsonConvert.DeserializeObject<OrderCreatedEvent>(jsonMessage);
                if (orderEvent != null)
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

                        productService.UpdateStockAsync(orderEvent.ProductId, orderEvent.QuantitySold).Wait();

                        Console.WriteLine($"[MAGAZZINO] Comando di aggiornamento inviato al DB per Prodotto {orderEvent.ProductId} (-{orderEvent.QuantitySold})");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRORE PROCESSAMENTO] {ex.Message}");
            }
        }
    }
}