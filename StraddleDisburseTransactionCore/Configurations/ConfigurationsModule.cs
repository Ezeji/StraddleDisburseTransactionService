using Microsoft.Extensions.DependencyInjection;
using StraddleDisburseTransactionCore.Configurations.Azure;
using StraddleDisburseTransactionCore.Configurations.Azure.Interfaces;

namespace StraddleDisburseTransactionCore.Configurations
{
    public static class ConfigurationsModule
    {
        public static void AddConfigurations(this IServiceCollection services)
        {
            //Azure
            services.AddScoped<IAzureServiceBusQueueConfiguration, AzureServiceBusQueueConfiguration>();
        }
    }
}
