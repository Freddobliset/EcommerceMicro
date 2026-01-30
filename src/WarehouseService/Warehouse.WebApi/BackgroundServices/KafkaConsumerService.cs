using Confluent.Kafka;
using Newtonsoft.Json;
using Warehouse.Business.Interfaces;
using Warehouse.Shared.Events; 
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Warehouse.WebApi.BackgroundServices
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly string _bootstrapServers = "kafka:9092";
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
                EnableAutoCommit = true
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();

            try
            {
                consumer.Subscribe(_topic);
                Console.WriteLine($"[KAFKA CONSUMER] 🎧 In ascolto su {_topic} presso {_bootstrapServers}...");

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var result = consumer.Consume(stoppingToken);

                        if (result != null && !string.IsNullOrEmpty(result.Message.Value))
                        {
                            Console.WriteLine($"[KAFKA CONSUMER] 📩 Messaggio RICEVUTO: {result.Message.Value}");
                            await ProcessMessageAsync(result.Message.Value);
                        }
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"[KAFKA ERROR] Errore ricezione: {e.Error.Reason}");
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[KAFKA CRITICAL] Il consumer è morto: {ex.Message}");
            }
            finally
            {
                consumer.Close();
            }
        }

        private async Task ProcessMessageAsync(string message)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

                    var orderEvent = JsonConvert.DeserializeObject<OrderCreatedEvent>(message);

                    if (orderEvent != null)
                    {
                        Console.WriteLine($"[MAGAZZINO] 📉 Scalo {orderEvent.QuantitySold} pezzi dal prodotto {orderEvent.ProductId}...");

                        await productService.UpdateStockAsync(orderEvent.ProductId, orderEvent.QuantitySold);

                        Console.WriteLine("[MAGAZZINO] ✅ Stock aggiornato con successo!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MAGAZZINO ERROR] Impossibile elaborare l'ordine: {ex.Message}");
            }
        }
    }
}