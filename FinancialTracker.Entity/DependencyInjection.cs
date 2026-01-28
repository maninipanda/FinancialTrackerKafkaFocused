using Confluent.Kafka;
using FinancialTracker.Application.Interfaces.Messaging;
using FinancialTracker.Application.Interfaces.Persistence;
using FinancialTracker.Domain.Events;
using FinancialTracker.Infrastructure.Messaging;
using FinancialTracker.Infrastructure.Persistence.Data;
using FinancialTracker.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FinancialTracker.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<MyDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sql =>
                    {
                        sql.EnableRetryOnFailure(
                            maxRetryCount: 10,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null);
                    });
            });



            // Optional: also register KafkaSettings as singleton for direct injection
            //services.AddSingleton(sp => sp.GetRequiredService<IOptions<KafkaSettings>>().Value);

            // Bind Kafka config from appsettings.json

            services.AddSingleton(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<KafkaSettings>>().Value;
                return new ProducerConfig
                {
                    BootstrapServers = settings.BootstrapServers,
                    MessageTimeoutMs = 60000, // 10 seconds
                    ClientId = settings.ClientId,
                    Acks = Acks.Leader,
                    //EnableIdempotence = true,
                };
            });


            services.AddScoped<IOrderRepository, OrderRepository>();
           services.AddScoped<IProcessedMessageRepository, ProcessedMessageRepository>();
            services.AddSingleton<IkafkaProducer, KafkaProducer>();

            //services.Configure<ProducerConfig>(configuration.GetSection("Kafka"));
            // kafka
            //KafkaProducer will automatically receive the bound ProducerConfig via DI


            return services;
        }
    }
}
