using Azure.Data.Tables;
using Azure.Storage.Queues;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ServerlessOrderPOC.Models;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ServerlessOrderPOC
{
    public class CreateOrder
    {
        private readonly ILogger _logger;

        public CreateOrder(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CreateOrder>();
        }

        [Function("CreateOrder")]
        public async Task<HttpResponseData> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            
            _logger.LogInformation("CreateOrder function triggered");
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var order = JsonSerializer.Deserialize<Order>(body);

            if (order == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid request");
                return badResponse;
            }

            order.OrderId = Guid.NewGuid().ToString();
            order.Status = "Created";
            order.CreatedAt = DateTime.UtcNow;

            var tableClient = new TableClient(
                Environment.GetEnvironmentVariable("AzureWebJobsStorage"),
                "Orders");

            await tableClient.CreateIfNotExistsAsync();

            var entity = new Order
            {
                PartitionKey = "ORDER",
                RowKey = order.OrderId,
                OrderId = order.OrderId,
                customerName = order.customerName,
                amount = order.amount,
                Status = order.Status,
                CreatedAt = order.CreatedAt
            };

            await tableClient.AddEntityAsync(entity);

            var queueService = new QueueService("order-queue");
            var encoded = Convert.ToBase64String(
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(order)));

            await queueService.SendMessageAsync(encoded);

            var response = req.CreateResponse(HttpStatusCode.Accepted);
            await response.WriteAsJsonAsync(order);
            return response;
        }

    }
}