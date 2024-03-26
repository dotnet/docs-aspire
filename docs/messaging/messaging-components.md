---
title: Use .NET Aspire messaging components in ASP.NET Core
description: Learn how to connect an ASP.NET Core app to messaging services using .NET Aspire components.
ms.date: 01/22/2024
ms.topic: tutorial
---

# Tutorial: Use .NET Aspire messaging components in ASP.NET Core

Cloud-native apps often require scalable messaging solutions that provide capabilities such as messaging queues and topics and subscriptions. .NET Aspire components simplify the process of connecting to various messaging providers, such as Azure Service Bus. In this tutorial, you'll create an ASP.NET Core app that uses .NET Aspire components to connect to Azure Service Bus to create a notification system. Submitted messages will be sent to a Service Bus topic for consumption by subscribers. You'll learn how to:

> [!div class="checklist"]
>
> - Create a basic .NET app that is set up to use .NET Aspire components
> - Add an .NET Aspire component to connect to Azure Service Bus
> - Configure and use .NET Aspire component features to send and receive data

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

## Set up the Azure Service Bus account

For this tutorial, you'll need access to an Azure Service Bus namespace with a topic and subscription configured. Use one of the following options to set up the require resources:

- **Azure portal**: [Create a service bus account with a topic and subscription](/azure/service-bus-messaging/service-bus-quickstart-topics-subscriptions-portal).

Alternatively:

- **Azure CLI**: Run the following commands in the Azure CLI or CloudShell to set up the required Azure Service Bus resources:

    ```azurecli-interactive
    az group create -n <your-resource-group-name> -location eastus
    az servicebus namespace create -g <your-resource-group-name> --name <your-namespace-name> --location eastus
    az servicebus topic create --g <your-resource-group-name> --namespace-name <your-namespace-name> --name notifications
    az servicebus topic subscription create --g <your-resource-group-name> --namespace-name <your-namespace-name> --topic-name notifications --name mobile
    ```

    > [!NOTE]
    > Replace the **your-resource-group-name** and **your-namespace-name** placeholders with your own values.
    > Service Bus namespace names must be globally unique across Azure.

### Azure authentication

This quickstart can be completed using either passwordless authentication or a connection string. Passwordless connections use Azure Active Directory and role-based access control (RBAC) to connect to a Service Bus namespace. You don't need to worry about having hard-coded connection string in your code, a configuration file, or in secure storage such as Azure Key Vault.

You can also use a connection string to connect to a Service Bus namespace, but the passwordless approach is recommended for real-world applications and production environments. For more information, read about [Authentication and authorization](/azure/service-bus-messaging/service-bus-authentication-and-authorization) or visit the passwordless [overview page](/dotnet/azure/sdk/authentication?tabs=command-line).

# [Passwordless (Recommended)](#tab/passwordless)

On your Service Bus namespace, assign the following role to the user account you logged into Visual Studio or the Azure CLI with:

- Service Bus Data Owner: [Assign an Azure RBAC role](/azure/storage/queues/assign-azure-role-data-access?tabs=portal)

# [Connection string](#tab/connection-string)

Retrieve the connection string for your Service Bus namespace from the **Shared access policies** menu in the Azure portal. Keep it somewhere safe for use during the quickstart.

:::image type="content" source="../media/aspire-service-bus.png" alt-text="Screenshot of Aspire service bus connection string menu.":::

---

## Create the sample solution

Create a .NET Aspire app using either Visual Studio or the .NET CLI.

## [Visual Studio](#tab/visual-studio)

Visual Studio provides app templates to get started with .NET Aspire that handle some of the initial setup configurations for you.

1. At the top of Visual Studio, navigate to **File** > **New** > **Project**.
1. In the dialog window, search for *ASP.NET Core* and select **ASP.NET Core Web API**. Choose **Next**.
1. On the **Configure your new project** screen:
    - Enter a **Project Name** of **AspireMessaging**.
    - Leave the rest of the values at their defaults and select **Next**.
1. On the **Additional information** screen:
    - Make sure **.NET 8.0** is selected.
    - Ensure that **Enlist in Aspire orchestration** is checked and select **Next**.

Visual Studio creates a new ASP.NET Core solution that is structured to use .NET Aspire. The solution consists of the following projects:

- **AspireMessaging** - An API project with default .NET Aspire service configurations.
- **AspireMessaging.AppHost** - An orchestrator project designed to connect and configure the different projects and services of your app. The orchestrator should be set as the startup project.
- **AspireMessaging.ServiceDefaults** - A shared class library to hold code that can be reused across the projects in your solution.

## [.NET CLI](#tab/dotnet-cli)

Use the [`dotnet new`](/dotnet/core/tools/dotnet-new) command to create a new .NET Aspire app:

```dotnetcli
dotnet new aspire-starter --name AspireMessaging
```

The solution consists of the following projects:

- **AspireMessaging** - An API project with default .NET Aspire service configurations.
- **AspireMessaging.AppHost** - An orchestrator project designed to connect and configure the different projects and services of your app. The orchestrator should be set as the startup project.
- **AspireMessaging.ServiceDefaults** - A shared class library to hold code that can be reused across the projects in your solution.

---

### Add the Worker Service project

Next, add a Worker Service project to the solution to retrieve and process messages to and from Azure Service Bus.

## [Visual Studio](#tab/visual-studio)

1. In the solution explorer, right click on the top level `AspireMessaging` solution node and select **Add** > **New project**.
1. Search for and select the **Worker Service** template and choose **Next**.
1. For the **Project name**, enter *AspireMessaging.WorkerService* and select **Next**.
1. On the **Additional information** screen:
    - Make sure **.NET 8.0** is selected.
    - Make sure **Enlist in .NET Aspire orchestration** is checked and select **Create**.

Visual Studio adds the project to your solution and updates the _Program.cs_ file of the `AspireMessaging.AppHost` project with a new line of code:

```csharp
builder.AddProject<Projects.AspireMessaging_WorkerService>("aspiremessaging-workerservice");
```

Visual Studio tooling added this line of code to register your new project with the <xref:Aspire.Hosting.IDistributedApplicationBuilder> object, which enables orchestration features you'll explore later.

## [.NET CLI](#tab/dotnet-cli)

1. In the root directory of the app, use the [`dotnet new`](/dotnet/core/tools/dotnet-new) command to create a new Worker Service app:

    ```dotnetcli
    dotnet new worker --name AspireMessaging.WorkerService
    ```

1. Use the [`dotnet sln`](/dotnet/core/tools/dotnet-sln) command to add the project to the solution:

    ```
    dotnet sln AspireMessaging.sln add AspireMessaging.WorkerService/AspireMessaging.WorkerService.csproj
    ```

1. Use the [`dotnet add`](/dotnet/core/tools/dotnet-add) command to add project reference between the **.AppHost** and **.WorkerService** project:

    ```dotnetcli
    dotnet add AspireMessaging.AppHost/AspireMessaging.AppHost.csproj reference AspireMessaging.WorkerService/AspireMessaging.WorkerService.csproj
    ```

1. Add the following line of code to the _Program.cs_ file in the **AspireMessaging.AppHost** project:

    ```csharp
    builder.AddProject<Projects.AspireMessaging_WorkerService>("aspiremessaging-workerservice");
    ```

---

The completed solution structure should resemble the following:

:::image type="content" source="../media/messaging-project.png" alt-text="A screenshot showing the completed sample .NET Aspire app structure.":::

## Add the .NET Aspire component to the API

Add the [.NET Aspire Azure Service Bus](azure-service-bus-component.md) component to your `AspireMessaging` app:

```dotnetcli
dotnet add package Aspire.Azure.Messaging.ServiceBus --prerelease
```

In the _Program.cs_ file of the `AspireMessaging` Razor Pages project, add a call to the `AddAzureServiceBus` extension methods:

```csharp
builder.AddAzureServiceBus("serviceBusConnection");
```

For more information, see <xref:Microsoft.Extensions.Hosting.AspireServiceBusExtensions.AddAzureServiceBus%2A>.

This method accomplishes the following tasks:

- Registers a <xref:Microsoft.Azure.Commands.ServiceBus.ServiceBusClient> with the DI container for connecting to Azure Service Bus.
- Automatically enables corresponding health checks, logging, and telemetry for the respective services.

In the _appsettings.json_ file of the `AspireMessaging` project, add the corresponding connection information:

# [Passwordless (Recommended)](#tab/passwordless)

```json
{
  "ConnectionStrings": {
    "serviceBusConnectionName": "{your_namespace}.servicebus.windows.net"
  }
}
```

# [Connection String](#tab/connection-string)

```json
{
  "ConnectionStrings": {
    "serviceBusConnection": "Endpoint=sb://{your_namespace}.servicebus.windows.net/;
          SharedAccessKeyName=accesskeyname;SharedAccessKey=accesskey"
  }
}
```

---

> [!NOTE]
> Make sure to replace **{your_namespace}** in the Service URIs with the name of your own Service Bus namespace.

## Create the API endpoint

The API must provide an endpoint to receive data and publish it to the Service Bus topic and broadcast to subscribers. Add the following endpoint to the `AspireMessaging` project to send a message to the topic:

```csharp
app.MapPost("/notify", static async (ServiceBusClient client, string message) =>
{
    var sender = client.CreateSender("notifications");

    // Create a batch
    using ServiceBusMessageBatch messageBatch =
        await sender.CreateMessageBatchAsync();

    if (messageBatch.TryAddMessage(
            new ServiceBusMessage($"Message {message}")) is false)
    {
        // If it's too large for the batch.
        throw new Exception(
            $"The message {message} is too large to fit in the batch.");
    }

    // Use the producer client to send the batch of
    // messages to the Service Bus topic.
    await sender.SendMessagesAsync(messageBatch);

    Console.WriteLine($"A message has been published to the topic.");
})
```

## Add the .NET Aspire component to the Worker Service

Add the [.NET Aspire Azure Service Bus](azure-service-bus-component.md) component to your `AspireMessaging.WorkerService` app:

```dotnetcli
dotnet add package Aspire.Azure.Messaging.ServiceBus --prerelease
```

In the _Program.cs_ file of the `AspireMessaging.WorkerService` Razor Pages project, add a call to the `AddAzureServiceBus` extension methods:

```csharp
builder.AddAzureServiceBus("serviceBusConnection");
```

This method accomplishes the following tasks:

- Registers a <xref:Microsoft.Azure.Commands.ServiceBus.ServiceBusClient> with the DI container for connecting to Azure Service Bus.
- Automatically enables corresponding health checks, logging, and telemetry for the respective services.

In the _appsettings.json_ file of the `AspireMessaging.WorkerService` project, add the corresponding connection information:

# [Passwordless (Recommended)](#tab/passwordless)

```json
{
  "ConnectionStrings": {
    "serviceBusConnectionName": "{your_namespace}.servicebus.windows.net"
  }
}
```

# [Connection String](#tab/connection-string)

```json
{
  "ConnectionStrings": {
    "serviceBusConnection": "Endpoint=sb://{your_namespace}.servicebus.windows.net/;
        SharedAccessKeyName=accesskeyname;SharedAccessKey=accesskey"
  }
}
```

---

> [!NOTE]
> Make sure to replace **{your_namespace}** in the Service URIs with the name of your own Service Bus namespace.

## Process the message from the subscriber

When a new message is placed on the `messages` queue, the worker service should retrieve, process, and delete the message. Update the _Worker.cs_ class to match the following code:

```csharp
public class Worker(
    ILogger<Worker> logger,
    ServiceBusClient client) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var processor = client.CreateProcessor(
                "notifications",
                "mobile",
                new ServiceBusProcessorOptions());

            // add handler to process messages
            processor.ProcessMessageAsync += MessageHandler;

            // add handler to process any errors
            processor.ProcessErrorAsync += ErrorHandler;

            // start processing
            await processor.StartProcessingAsync();

            logger.LogInformation(
                "Wait for a minute and then press any key to end the processing");
            Console.ReadKey();

            // stop processing
            logger.LogInformation("\nStopping the receiver...");
            await processor.StopProcessingAsync();
            logger.LogInformation("Stopped receiving messages");
        }
    }

    async Task MessageHandler(ProcessMessageEventArgs args)
    {
        string body = args.Message.Body.ToString();
        logger.LogInformation("Received: {Body} from subscription.", body);

        // complete the message. messages is deleted from the subscription.
        await args.CompleteMessageAsync(args.Message);
    }

    // handle any errors when receiving messages
    Task ErrorHandler(ProcessErrorEventArgs args)
    {
        logger.LogError(args.Exception, args.Exception.Message);
        return Task.CompletedTask;
    }
}
```

## Run and test the app locally

The sample app is now ready for testing. Verify that the data submitted to the API is sent to the Azure Service Bus topic and consumed by the subscriber worker service:

## [Visual Studio](#tab/visual-studio)

1. Press the run button at the top of Visual Studio to launch your Aspire app. The .NET Aspire dashboard app should open in the browser.
1. On the resources page, in the **aspireweb** row, click the link in the **Endpoints** column to open the Swagger UI page of your API.
1. On the .NET Aspire dashboard, navigate to the logs for the **AspireWorkerService** project.
1. Back on the Swagger UI page, expand the **/notify** endpoint and select **Try it out**.
1. Enter a test message in the **message** input box.
1. Select **Execute** to send a test request.
1. Switch back to the **AspireWorkerService** logs. You should see the test message printed in the output logs.

## [.NET CLI](#tab/dotnet-cli)

1. In a terminal window at the root of your project, use the `dotnet run` command to start the app:

    ```csharp
    dotnet run --project AspireMessaging.AppHost
    ```

1. On the resources page, in the **aspireweb** row, click the link in the **Endpoints** column to open the Swagger UI page of your API.
1. On the .NET Aspire dashboard, navigate to the logs for the **AspireWorkerService** project.
1. Back on the Swagger UI page, expand the **/notify** endpoint and select **Try it out**.
1. Enter a test message in the **message** input box.
1. Select **Execute** to send a test request.
1. Switch back to the **AspireWorkerService** logs. You should see the test message printed in the output logs.

---

Congratulations! You created and configured an ASP.NET Core API that connects to Azure Service Bus using Aspire components.

[!INCLUDE [clean-up-resources](../includes/clean-up-resources.md)]
