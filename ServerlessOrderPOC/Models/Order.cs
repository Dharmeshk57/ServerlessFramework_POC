using Azure;
using Azure.Data.Tables;

namespace ServerlessOrderPOC.Models
{
    public class Order : ITableEntity
    {
        public string OrderId { get; set; }
        public string customerName { get; set; }
        public int amount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        // ITableEntity required properties
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}