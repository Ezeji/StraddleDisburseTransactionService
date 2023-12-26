using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StraddleDisburseTransactionCore.Configurations.Azure.Interfaces;
using StraddleDisburseTransactionCore.Models.DTO.Shared;
using StraddleDisburseTransactionCore.Services.Wallets.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleDisburseTransactionCore.Configurations.Azure
{
    public class AzureServiceBusQueueConfiguration : IAzureServiceBusQueueConfiguration
    {
        private readonly IConfiguration _configuration;
        private readonly IQueueClient _queueClient;

        private readonly ILogger<AzureServiceBusQueueConfiguration> _logger;
        private readonly IServiceScopeFactory _serviceScope;

        public AzureServiceBusQueueConfiguration(IConfiguration configuration, ILogger<AzureServiceBusQueueConfiguration> logger,
            IServiceScopeFactory serviceScope)
        {
            _configuration = configuration;
            _logger = logger;
            _serviceScope = serviceScope;
            _queueClient = new QueueClient(_configuration["AzureServiceBusConfig:ConnectionString"], _configuration["AzureServiceBusConfig:QueueName"]);
        }

        public async Task<Task> SendMessageAsync(string message)
        {
            Message? encodedMessage = new Message(Encoding.UTF8.GetBytes(message));
            await _queueClient.SendAsync(encodedMessage);

            return Task.CompletedTask;
        }

        public async Task ReceiveMessageAsync()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            _queueClient.RegisterMessageHandler(ProcessMessageAsync, messageHandlerOptions);
            await Task.CompletedTask; // Let the method run continuously
        }

        private async Task ProcessMessageAsync(Message message, CancellationToken token)
        {
            string? messageBody = Encoding.UTF8.GetString(message.Body);

            using (IServiceScope scope = _serviceScope.CreateScope())
            {
                DisburseTransactionDTO? disburseTransactionDTO = JsonConvert.DeserializeObject<DisburseTransactionDTO?>(messageBody);

                IDisburseTransactionService service = scope.ServiceProvider.GetRequiredService<IDisburseTransactionService>();
                
                await service.InitiateDisburseTransactionAsync(disburseTransactionDTO);
            }

            await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            _logger.LogInformation($"Message handler encountered an exception: {exceptionReceivedEventArgs.Exception}");
            return Task.CompletedTask;
        }
    }
}
