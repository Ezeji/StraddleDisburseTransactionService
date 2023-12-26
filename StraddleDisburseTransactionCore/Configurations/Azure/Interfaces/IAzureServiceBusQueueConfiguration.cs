using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleDisburseTransactionCore.Configurations.Azure.Interfaces
{
    public interface IAzureServiceBusQueueConfiguration
    {
        Task<Task> SendMessageAsync(string message);

        Task ReceiveMessageAsync();
    }
}
