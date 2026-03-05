# Azure Functions Order Processing POC

## Overview

This project demonstrates a **serverless order processing workflow**
using **Azure Functions** with the following components:

-   HTTP Trigger -- Create a new order
-   Azure Table Storage -- Persist order data
-   Queue Trigger -- Process orders asynchronously
-   Timer Trigger -- Background job to monitor orders

The solution runs **locally using Azurite** for storage emulation.

------------------------------------------------------------------------

## Architecture

Client → HTTP Trigger → Azure Table Storage\
                      ↓\
                   Queue Message\
                      ↓\
                Queue Trigger Function\
                      ↓\
                 Update Order Status\
                      ↓\
                 Timer Trigger (Background Monitoring)

------------------------------------------------------------------------

## Project Structure

    OrderFunctionApp
    │
    ├── CreateOrderFunction.cs      # HTTP Trigger to create order
    ├── ProcessOrderQueue.cs        # Queue Trigger to process order
    ├── OrderMonitorTimer.cs        # Timer Trigger background job
    ├── Models
    │   └── Order.cs                # Order entity model
    └── host.json

------------------------------------------------------------------------

## Prerequisites

Install the following tools:

-   .NET 8 SDK
-   Azure Functions Core Tools
-   Azurite (Local Azure Storage Emulator)
-   Azure Storage Explorer (optional)

------------------------------------------------------------------------

## Start Azurite

Run Azurite locally:

``` bash
azurite
```

Default connection string used in **local.settings.json**:

    UseDevelopmentStorage=true

------------------------------------------------------------------------

## Run the Project

Start the Azure Function locally:

``` bash
func start
```

You should see endpoints like:

    http://localhost:7071/api/CreateOrder

------------------------------------------------------------------------

## Create Order (HTTP Trigger)

Send request:

    POST /api/CreateOrder

Example JSON:

``` json
{
  "orderId": "12345",
  "customerName": "Dharmesh",
  "amount": 500
}
```

What happens:

1.  Order is saved in **Azure Table Storage**
2.  Message pushed to **Azure Queue**
3.  Queue trigger starts background processing

------------------------------------------------------------------------

## Queue Processing

The **Queue Trigger**:

-   Reads order message
-   Fetches order from Table Storage
-   Updates order status to **Processed**

------------------------------------------------------------------------

## Timer Trigger (Background Job)

Runs automatically every minute.

Responsibilities:

-   Scan orders table
-   Find pending orders
-   Log or take action if needed

Example CRON expression:

    0 */1 * * * *

Meaning:

**Runs every 1 minute.**

------------------------------------------------------------------------

## View Data

You can view stored orders using:

**Azure Storage Explorer**

Connect using:

    UseDevelopmentStorage=true

Then navigate to:

    Tables → Orders

------------------------------------------------------------------------

## Sample Stored Order

    PartitionKey : ORDER
    RowKey       : OrderId
    Status       : Created / Processed
    CustomerName : Dharmesh
    Amount       : 500

------------------------------------------------------------------------

## Learning Goals

This POC demonstrates:

-   Serverless architecture
-   Event-driven processing
-   Azure Table Storage usage
-   Queue-based asynchronous workflows
-   Background jobs with Timer Triggers

------------------------------------------------------------------------

## Author

Dharmesh
