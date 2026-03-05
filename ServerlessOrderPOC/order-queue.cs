using Azure.Data.Tables;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using ServerlessOrderPOC.Models;
using System;
using System.Text.Json;

namespace ServerlessOrderPOC
{
    public class order_queue
    {
        private readonly ILogger<order_queue> _logger;

        public order_queue(ILogger<order_queue> logger)
        {
            _logger = logger;
        }

        [Function("ProcessOrderQueue")]
        public async Task Run(
            [QueueTrigger("order-queue")] string message)
        {
            var order = JsonSerializer.Deserialize<Order>(message);

            var tableClient = new TableClient(
                Environment.GetEnvironmentVariable("AzureWebJobsStorage"),
                "Orders");

            var entity = await tableClient.GetEntityAsync<Order>(
                "ORDER", order.OrderId);

            entity.Value.Status = "Processed";

            await tableClient.UpdateEntityAsync(
                entity.Value, entity.Value.ETag);

            _logger.LogInformation($"Order {order.OrderId} processed");
        }
    }
}
