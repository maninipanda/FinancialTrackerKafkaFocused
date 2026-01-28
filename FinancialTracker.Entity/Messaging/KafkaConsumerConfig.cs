using Confluent.Kafka;
using Microsoft.Extensions.Options;


namespace FinancialTracker.Infrastructure.Messaging
{
    public class KafkaConsumerConfig
    {
        private readonly KafkaSettings _settings;

        //public KafkaConsumerConfig(IOptions<KafkaSettings> options)
        //{

        //    _settings = options.Value;
        //}
        // Inject KafkaSettings directly
        public KafkaConsumerConfig(KafkaSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public ConsumerConfig Create()
        {
            return new ConsumerConfig
            {

                BootstrapServers = _settings?.BootstrapServers ?? "kafka:9092",
                GroupId = "order-consumer-group",
                Acks = Acks.Leader,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true,          // manual commit for idempotency
                AllowAutoCreateTopics = true,
                EnablePartitionEof = true,          // optional, useful for debugging
                SessionTimeoutMs = 30000,           // 30s session timeout
                MaxPollIntervalMs = 30000,         // 5 min max poll interval
                EnableAutoOffsetStore = false       // store offset only after processing
            };


        }

    }
}

