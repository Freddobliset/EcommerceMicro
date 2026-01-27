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
            var config = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId = _groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(_topic);

            Console.WriteLine($"[KAFKA CONSUMER] In ascolto sul topic: {_topic}");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);
                    var jsonMessage = result.Message.Value;
                    Console.WriteLine($"[KAFKA CONSUMER] Ricevuto: {jsonMessage}");

                    var orderEvent = JsonConvert.DeserializeObject<OrderCreatedEvent>(jsonMessage);

                    if (orderEvent != null)
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

                            // Qui recuperiamo il prodotto e aggiorniamo la quantità
                            var product = await productService.GetProductByIdAsync(orderEvent.ProductId);
                            if (product != null)
                            {
                                // Simuliamo l'aggiornamento (per ora solo log, poi implementerai Update se vuoi)
                                product.StockQuantity -= orderEvent.QuantitySold;
                                Console.WriteLine($"[MAGAZZINO UPDATE] Prodotto {product.Id} aggiornato. Nuova Qtà: {product.StockQuantity}");
                                // TODO: Qui dovresti chiamare await productService.UpdateAsync(product);
                            }
                        }
                    }
                }
                catch (OperationCanceledException) { break; }
                catch (Exception e)
                {
                    Console.WriteLine($"[KAFKA ERROR] {e.Message}");
                }
            }
        }
    }
}