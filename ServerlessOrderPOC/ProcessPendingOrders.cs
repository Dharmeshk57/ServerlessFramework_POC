using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using ServerlessOrderPOC.Models;

public class ProcessPendingOrders
{
    private readonly ILogger _logger;

    public ProcessPendingOrders(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<ProcessPendingOrders>();
    }

    [Function("ProcessPendingOrders")]
    public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo timer)
    {
        _logger.LogInformation($"Timer trigger executed at: {DateTime.UtcNow}");

        var tableClient = new TableClient(
            Environment.GetEnvironmentVariable("AzureWebJobsStorage"),
            "Orders");

        var orders = tableClient.QueryAsync<Order>(o => o.Status == "Created");

        await foreach (var order in orders)
        {
            order.Status = "Processed";

            await tableClient.UpdateEntityAsync(
                order,
                order.ETag,
                TableUpdateMode.Replace);

            _logger.LogInformation($"Processed Order: {order.OrderId}");
        }
    }
}