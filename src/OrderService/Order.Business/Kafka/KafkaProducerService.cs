using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Newtonsoft.Json;
using Warehouse.Shared.Events;

namespace Order.Business.Kafka
{
    public interface IKafkaProducerService
    {
        Task SendOrderCreatedMessage(OrderCreatedEvent message);
    }

    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly string _bootstrapServers = "kafka:9092";
        private readonly string _topic = "order-created-topic";

        public async Task SendOrderCreatedMessage(OrderCreatedEvent message)
        {
            var config = new ProducerConfig { BootstrapServers = _bootstrapServers };

            using var producer = new ProducerBuilder<Null, string>(config).Build();

            var jsonMessage = JsonConvert.SerializeObject(message);

            try
            {
                await producer.ProduceAsync(_topic, new Message<Null, string> { Value = jsonMessage });
                Console.WriteLine($"[KAFKA PRODUCER] Messaggio inviato: {jsonMessage}");
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"[KAFKA ERROR] Invio fallito: {e.Error.Reason}");
            }
        }
    }
}