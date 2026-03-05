using Azure.Storage.Queues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessOrderPOC
{
    public class QueueService
    {
        private readonly QueueClient _queueClient;

        public QueueService(string queueName)
        {
            _queueClient = new QueueClient(
                Environment.GetEnvironmentVariable("AzureWebJobsStorage"),
                queueName);

            _queueClient.CreateIfNotExists();
        }

        public async Task SendMessageAsync(string message)
        {
            var encodedMessage = Convert.ToBase64String(Encoding.UTF8.GetBytes(message));
            await _queueClient.SendMessageAsync(encodedMessage);
        }
    }
}
