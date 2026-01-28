using Confluent.Kafka;
using FinancialTracker.Application;
using FinancialTracker.Application.Interfaces.Messaging;

using FinancialTracker.Infrastructure;
using FinancialTracker.Infrastructure.Messaging;
using FinancialTracker.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Runtime;



var builder = WebApplication.CreateBuilder(args);


bool isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";


builder.WebHost.ConfigureKestrel(options =>
{
   // options.ListenAnyIP(5146); // optional HTTP
    if (isDocker)
    {
        options.ListenAnyIP(7238, listenOptions =>
        {
            listenOptions.UseHttps("/https/aspnet.pfx", "devpassword");
        });
    }
});

// Bind Kafka settings
builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("KafkaProducer"));
     //builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<KafkaSettings>>().Value);

     builder.Services.AddApplication();


     // 3️⃣ Register Infrastructure Layer
     builder.Services.AddInfrastructure(builder.Configuration);



     builder.Services.AddControllers();
     // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
     // swagger services
     builder.Services.AddEndpointsApiExplorer();
     builder.Services.AddSwaggerGen(); // This now works with the correct using directives

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy => policy
            .WithOrigins("https://localhost:54368")
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});





var app = builder.Build();


//swagger middlewear
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

// Configure the HTTP request pipeline.


app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowAngular");

app.MapControllers();
app.MapGet("/", () => "Hello HTTPS Docker!");

app.Run();



//dotnet dev-certs https --trust
