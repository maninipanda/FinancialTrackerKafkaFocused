

namespace FinancialTracker.Infrastructure.Messaging
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; } 
        public string GroupId { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public string ClientId { get; set; }

        public int MessageTimeoutMs { get; set; } = 30000;
    }
    }
