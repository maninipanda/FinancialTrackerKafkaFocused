using Confluent.Kafka;
using FinancialTracker.Application.Interfaces.Messaging;
using FinancialTracker.Domain.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Runtime;
using System.Text.Json;
using static Confluent.Kafka.ConfigPropertyNames;

namespace FinancialTracker.Infrastructure.Messaging
{
    public sealed class KafkaProducer : IkafkaProducer, IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<KafkaProducer> _logger;
        private readonly ProducerConfig _pconfig;
        private readonly KafkaSettings _settings;

        public KafkaProducer(
            IOptions<KafkaSettings> options,
            ILogger<KafkaProducer> logger,
            ProducerConfig pconfig)
        {
            _logger = logger;
            var _settings = options.Value;

            _pconfig = pconfig;

            _producer = new ProducerBuilder<string, string>(pconfig).Build();
        }

        public async Task PublishAsync<T>(string topic, T message, CancellationToken ct = default)
        {
            var messagem = JsonSerializer.Serialize(message);

            var kafkaMessage = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = messagem
            };

            DeliveryResult<string, string> result;

            try
            {
                result = await _producer.ProduceAsync(topic, kafkaMessage, ct);
            }
            catch (ProduceException<string, string> ex)
            {
                
                _logger.LogError(ex, "Kafka produce failed: {Reason}", ex.Error.Reason);
                throw;
            }

            // ✅ THIS is how you verify ACKs
            if (result.Status != PersistenceStatus.Persisted)
            {
                throw new Exception(
                    $"Kafka message not persisted. Status={result.Status}");
            }

            _logger.LogInformation(
                "Kafka message published. Topic={Topic}, Partition={Partition}, Offset={Offset}",
                result.Topic,
                result.Partition.Value,
                result.Offset.Value
            );
        }

        public void Dispose()
        {
            _producer.Flush(TimeSpan.FromSeconds(10));
            _producer.Dispose();
        }
    }
}