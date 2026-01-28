using Confluent.Kafka;
using FinancialTracker.Application;
using FinancialTracker.Application.Interfaces.Messaging;
using FinancialTracker.Infrastructure;
using FinancialTracker.Infrastructure.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationService;

IHost host = Host.CreateDefaultBuilder(args)

     .ConfigureAppConfiguration((hostContext, config) =>
     {
         config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddEnvironmentVariables();
     })
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;
        // Bind Kafka settings
        services.Configure<KafkaSettings>(configuration.GetSection("KafkaConsumer"));
        services.Configure<KafkaSettings>(configuration.GetSection("feb-KafkaConsumer"));

        services.AddSingleton(sp => sp.GetRequiredService<IOptions<KafkaSettings>>().Value);

        // Kafka consumer config
        services.AddSingleton<KafkaConsumerConfig>(sp =>
        {
            var settings = sp.GetRequiredService<KafkaSettings>();
            return new KafkaConsumerConfig(settings);
        });

        ;



        // Register Application Layer
        services.AddApplication();

        // Register Infrastructure Layer
        services.AddInfrastructure(configuration);

       

        // Add Hosted Worker
        services.AddHostedService<TransactionConsumerWorker>();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

await host.RunAsync();
