using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StraddleDisburseTransactionCore.Configurations.Azure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleDisburseTransactionCore.BackgroundServices
{
    public class AzureServiceBusBackgroundService : BackgroundService
    {
        private readonly ILogger<AzureServiceBusBackgroundService> _logger;

        private readonly IServiceScopeFactory _serviceScope;

        public AzureServiceBusBackgroundService(ILogger<AzureServiceBusBackgroundService> logger, IServiceScopeFactory serviceScope)
        {
            _logger = logger;
            _serviceScope = serviceScope;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await RunJob(stoppingToken);
                    await Task.Delay(5000, stoppingToken);
                }
                catch (OperationCanceledException ex)
                {
                    //catch the cancellation exception to stop execution
                    _logger.LogError($"Error:{ex.Message}");

                    return;
                }
            }
        }

        private async Task RunJob(CancellationToken token)
        {
            using (IServiceScope scope = _serviceScope.CreateScope())
            {
                IAzureServiceBusQueueConfiguration queue = scope.ServiceProvider.GetRequiredService<IAzureServiceBusQueueConfiguration>();
                await queue.ReceiveMessageAsync();
            }
        }
    }
}
