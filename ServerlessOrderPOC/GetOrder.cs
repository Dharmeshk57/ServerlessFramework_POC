using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using ServerlessOrderPOC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessOrderPOC
{
    public class GetOrder
    {
        [Function("GetOrder")]
        public async Task<HttpResponseData> Run(
                    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "order/{orderId}")]
    HttpRequestData req,
                    string orderId) 
        {
            var tableClient = new TableClient(
                Environment.GetEnvironmentVariable("AzureWebJobsStorage"),
                "Orders");

            try
            {
                var entity = await tableClient.GetEntityAsync<Order>(
                    "ORDER", orderId);

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(entity.Value);
                return response;
            }
            catch
            {
                var response = req.CreateResponse(HttpStatusCode.NotFound);
                await response.WriteStringAsync("Order not found");
                return response;
            }
        }
    }
}
