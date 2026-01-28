using FinancialTracker.Application.Handlers;
using FinancialTracker.Application.Interfaces.Persistence;

using Microsoft.Extensions.DependencyInjection;





namespace FinancialTracker.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Command Handlers
            services.AddScoped<CreateOrderHandler>();


            return services;
        
        }
    }
}
