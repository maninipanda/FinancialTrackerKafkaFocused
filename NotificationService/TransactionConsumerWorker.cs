using Confluent.Kafka;
using FinancialTracker.Application.Interfaces.Persistence;
using FinancialTracker.Domain.Entities;
using FinancialTracker.Domain.Events;
using FinancialTracker.Infrastructure.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Runtime;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationService
{
    public class TransactionConsumerWorker : BackgroundService
    {

        private readonly IServiceScopeFactory _scopeFactory;
      private readonly KafkaConsumerConfig _consumerConfig;
        private readonly IOrderRepository _orderRepository;
        private readonly IProcessedMessageRepository _processedRepo;
        private readonly ILogger<TransactionConsumerWorker> _logger;

        private readonly IConsumer<string, string> _consumer;
        public TransactionConsumerWorker(
            KafkaConsumerConfig _consumerConfig,
             
            
            IServiceScopeFactory scopeFactory,
           
            ILogger<TransactionConsumerWorker> logger)
        {
            
           
            _logger = logger;
            _scopeFactory = scopeFactory;

            
            // Properly build the consumer from configuration
            var consumerConfig = _consumerConfig.Create();
            _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();

            // Subscribe to the topic once
            _consumer.Subscribe("orders-created-topic");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Kafka consumer started for topic 'orders-created-topic'");

            while (!stoppingToken.IsCancellationRequested)
            {
                ConsumeResult<string, string> result = null;

                try
                {
                    // Consume message (blocks until a message is available or cancellation requested)
                    result = _consumer.Consume(stoppingToken);
                    if (result == null)
                        continue;

                    // Deserialize Kafka message
                    var evt = JsonSerializer.Deserialize<OrderCreatedEvent>(result.Message.Value);
                    if (evt == null)
                        continue;

                    // Map event to domain entity
                    var order = new SalesOrder(evt.OrderId, evt.Total, "UnknownCustomer", "Pending");

                    // Create processed message record for idempotency
                    var processedMessage = new ProcessedMessageM
                    {
                        MessageKey = evt.OrderId.ToString(),
                        Topic = result.Topic,
                        Offset = result.Offset.Value,
                        //Partition = result.Partition.Value,
                        ProcessedAt = DateTime.UtcNow
                    };
                    // Save domain entity
                    using var scope = _scopeFactory.CreateScope();
                    var handler = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

                    // Try atomic insert; returns true if first-time processing
                    bool shouldProcess = await _processedRepo.TryAddAsync(processedMessage);
                    if (!shouldProcess)
                    {
                        _logger.LogWarning("Duplicate message skipped {OrderId}", evt.OrderId);
                        continue;
                    }

                    
                    await handler.AddAsync(order);

                    _logger.LogInformation(
                        "Order {OrderId} processed successfully from Partition {Partition}, Offset {Offset}",
                        evt.OrderId,
                        result.Partition.Value,
                        result.Offset.Value
                    );

                    // Commit Kafka offset only after successful processing
                    _consumer.Commit(result);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Kafka consume error");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing Order {OrderId}", result?.Message?.Key);
                    // Optionally send to Dead Letter Topic or retry
                }
            }
        }

        public override void Dispose()
        {
            _consumer.Close(); // Graceful shutdown
            _consumer.Dispose();
            base.Dispose();
        }
    }
}
