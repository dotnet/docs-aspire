---
title: Use .NET Aspire messaging components in ASP.NET Core
description: Learn how to connect an ASP.NET Core app to messaging services using .NET Aspire components.
ms.date: 06/05/2024
ms.topic: tutorial
zone_pivot_groups: dev-environment
---

# Tutorial: Use .NET Aspire messaging components in ASP.NET Core

Cloud-native apps often require scalable messaging solutions that provide capabilities such as messaging queues and topics and subscriptions. .NET Aspire components simplify the process of connecting to various messaging providers, such as Azure Service Bus. In this tutorial, you'll create an ASP.NET Core app that uses .NET Aspire components to connect to Azure Service Bus to create a notification system. Submitted messages will be sent to a Service Bus topic for consumption by subscribers. You'll learn how to:

> [!div class="checklist"]
>
> - Create a basic .NET app that is set up to use .NET Aspire components
> - Add an .NET Aspire component to connect to Azure Service Bus
> - Configure and use .NET Aspire component features to send and receive data

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

In addition to the preceding prerequisites, you'll also need to install the Azure CLI. To install the Azure CLI, follow the instructions in the [Azure CLI installation guide](/cli/azure/install-azure-cli).

## Set up the Azure Service Bus account

For this tutorial, you'll need access to an Azure Service Bus namespace with a topic and subscription configured. Use one of the following options to set up the require resources:

- **Azure portal**: [Create a service bus account with a topic and subscription](/azure/service-bus-messaging/service-bus-quickstart-topics-subscriptions-portal).

Alternatively:

- **Azure CLI**: Run the following commands in the Azure CLI or CloudShell to set up the required Azure Service Bus resources:

    ```azurecli-interactive
    az group create -n <your-resource-group-name> --location eastus
    az servicebus namespace create -g <your-resource-group-name> --name <your-namespace-name> --location eastus
    az servicebus topic create -g <your-resource-group-name> --namespace-name <your-namespace-name> --name notifications
    az servicebus topic subscription create -g <your-resource-group-name> --namespace-name <your-namespace-name> --topic-name notifications --name mobile
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

To create a new .NET Aspire Starter Application, you can use either Visual Studio, Visual Studio Code, or the .NET CLI.

:::zone pivot="visual-studio"

[!INCLUDE [visual-studio-file-new](../includes/visual-studio-file-new.md)]

:::zone-end
:::zone pivot="vscode"

[!INCLUDE [vscode-file-new](../includes/vscode-file-new.md)]

:::zone-end
:::zone pivot="dotnet-cli"

[!INCLUDE [dotnet-cli-file-new](../includes/dotnet-cli-file-new.md)]

:::zone-end

### Add the Worker Service project

Next, add a Worker Service project to the solution to retrieve and process messages to and from Azure Service Bus.

:::zone pivot="visual-studio"

1. In the solution explorer, right click on the top level `AspireSample` solution node and select **Add** > **New project**.
1. Search for and select the **Worker Service** template and choose **Next**.
1. For the **Project name**, enter *AspireSample.WorkerService* and select **Next**.
1. On the **Additional information** screen:
    - Make sure **.NET 8.0** is selected.
    - Make sure **Enlist in .NET Aspire orchestration** is checked and select **Create**.

Visual Studio adds the project to your solution and updates the _:::no-loc text="Program.cs":::_ file of the `AspireSample.AppHost` project with a new line of code:

```csharp
builder.AddProject<Projects.AspireSample_WorkerService>(
    "aspiresample-workerservice");
```

Visual Studio tooling added this line of code to register your new project with the <xref:Aspire.Hosting.IDistributedApplicationBuilder> object, which enables orchestration features you'll explore later.

:::zone-end
:::zone pivot="vscode"

1. From the **Solution Explorer** in Visual Studio Code, select the **+** button next to the solution name to add a new project to the solution:

    :::image type="content" source="media/vscode-add-project.png" lightbox="media/vscode-add-project.png" alt-text="Visual Studio Code: Add new project from C# DevKit Solution Explorer.":::

1. To filter the project templates, enter **Worker** in the search box and select the **Worker Service** template that's found:

    :::image type="content" source="media/vscode-create-worker.png" lightbox="media/vscode-create-worker.png" alt-text="Visual Studio Code: Filter to Worker Service project template from Add project.":::

1. Choose the **Worker Service** template and enter the project name as **AspireSample.WorkerService**.
1. Select **Default directory** to create the project in the same directory as the solution.
1. Select **Create project** to add the project to the solution.
1. Right-click on the **AspireSample.AppHost** project in the **Solution Explorer** and select **Add Project Reference**:

    :::image type="content" source="media/vscode-add-project-reference.png" lightbox="media/vscode-add-project-reference.png" alt-text="Visual Studio Code: Add project reference from AspireSample.AppHost to AspireSample.WorkerService.":::

1. Add the following line of code to the _:::no-loc text="Program.cs":::_ file in the **AspireSample.AppHost** project before the call to `builder.Build().Run();`:

    ```csharp
    builder.AddProject<Projects.AspireSample_WorkerService>(
        "aspiresample-workerservice");
    ```

:::zone-end
:::zone pivot="dotnet-cli"

1. In the root directory of the app, use the [dotnet new](/dotnet/core/tools/dotnet-new) command to create a new Worker Service app:

    ```dotnetcli
    dotnet new worker --name AspireSample.WorkerService
    ```

1. Use the [`dotnet sln`](/dotnet/core/tools/dotnet-sln) command to add the project to the solution:

    ```dotnetcli
    dotnet sln AspireSample.sln add AspireSample.WorkerService/AspireSample.WorkerService.csproj
    ```

1. Use the [`dotnet add`](/dotnet/core/tools/dotnet-add-reference) command to add a project reference between the **.AppHost** and **.WorkerService** project:

    ```dotnetcli
    dotnet add AspireSample.AppHost/AspireSample.AppHost.csproj reference AspireSample.WorkerService/AspireSample.WorkerService.csproj
    ```

1. Add the following line of code to the _:::no-loc text="Program.cs":::_ file in the **AspireSample.AppHost** project before the call to `builder.Build().Run();`:

    ```csharp
    builder.AddProject<Projects.AspireSample_WorkerService>(
        "aspiresample-workerservice");
    ```

:::zone-end

The completed solution structure should resemble the following, assuming the top-level directory is named _aspire-messaging_:

```Directory
└───📂 aspire-messaging
     ├───📂 AspireSample.WorkerService
     │    ├───📂 Properties
     │    │    └─── launchSettings.json
     │    ├─── appsettings.Development.json
     │    ├─── appsettings.json
     │    ├─── AspireSample.WorkerService.csproj
     │    ├─── Program.cs
     │    └─── Worker.cs
     ├───📂 AspireSample.ApiService
     │    ├───📂 Properties
     │    │    └─── launchSettings.json
     │    ├─── appsettings.Development.json
     │    ├─── appsettings.json
     │    ├─── AspireSample.ApiService.csproj
     │    └─── Program.cs
     ├───📂 AspireSample.AppHost
     │    ├───📂 Properties
     │    │    └─── launchSettings.json
     │    ├─── appsettings.Development.json
     │    ├─── appsettings.json
     │    ├─── AspireSample.AppHost.csproj
     │    └─── Program.cs
     ├───📂 AspireSample.ServiceDefaults
     │    ├─── AspireSample.ServiceDefaults.csproj
     │    └─── Extensions.cs
     ├───📂 AspireSample.Web
     │    ├───📂 Components
     │    │    ├───📂 Layout
     │    │    │    ├─── MainLayout.razor
     │    │    │    ├─── MainLayout.razor.css
     │    │    │    ├─── NavMenu.razor
     │    │    │    └─── NavMenu.razor.css
     │    │    ├───📂 Pages
     │    │    │    ├─── Counter.razor
     │    │    │    ├─── Error.razor
     │    │    │    ├─── Home.razor
     │    │    │    └─── Weather.razor
     │    │    ├─── _Imports.razor
     │    │    ├─── App.razor
     │    │    └─── Routes.razor
     │    ├───📂 Properties
     │    │    └─── launchSettings.json
     │    ├───📂 wwwroot
     │    │    ├───📂 bootstrap
     │    │    │    ├─── bootstrap.min.css
     │    │    │    └─── bootstrap.min.css.map
     │    │    ├─── app.css
     │    │    └─── favicon.png
     │    ├─── appsettings.Development.json
     │    ├─── appsettings.json
     │    ├─── AspireSample.Web.csproj
     │    ├─── Program.cs
     │    └─── WeatherApiClient.cs
     └─── AspireSample.sln
```

## Add the .NET Aspire component to the API

Add the [.NET Aspire Azure Service Bus](azure-service-bus-component.md) component to your `AspireSample.ApiService` app:

```dotnetcli
dotnet add package Aspire.Azure.Messaging.ServiceBus
```

In the _:::no-loc text="Program.cs":::_ file of the `AspireSample` Razor Pages project, add a call to the `AddAzureServiceBus` extension methods:

```csharp
builder.AddAzureServiceBusClient("serviceBusConnection");
```

For more information, see <xref:Microsoft.Extensions.Hosting.AspireServiceBusExtensions.AddAzureServiceBusClient%2A>.

This method accomplishes the following tasks:

- Registers a <xref:Microsoft.Azure.Commands.ServiceBus.ServiceBusClient> with the DI container for connecting to Azure Service Bus.
- Automatically enables corresponding health checks, logging, and telemetry for the respective services.

In the _:::no-loc text="appsettings.json":::_ file of the `AspireSample` project, add the corresponding connection information:

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

The API must provide an endpoint to receive data and publish it to the Service Bus topic and broadcast to subscribers. Add the following endpoint to the `AspireSample.ApiService` project to send a message to the Service Bus topic. Place this code in _Program.cs_ before the `app.MapDefaultEndpoints()` call:

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
});
```

## Add the .NET Aspire component to the Worker Service

Add the [.NET Aspire Azure Service Bus](azure-service-bus-component.md) component to your `AspireSample.WorkerService` app:

```dotnetcli
dotnet add package Aspire.Azure.Messaging.ServiceBus
```

In the _:::no-loc text="Program.cs":::_ file of the `AspireSample.WorkerService` Worker Service project, add a call to the `AddAzureServiceBus` extension methods:

```csharp
builder.AddAzureServiceBusClient("serviceBusConnection");
```

This method accomplishes the following tasks:

- Registers a <xref:Microsoft.Azure.Commands.ServiceBus.ServiceBusClient> with the DI container for connecting to Azure Service Bus.
- Automatically enables corresponding health checks, logging, and telemetry for the respective services.

In the _:::no-loc text="appsettings.json":::_ file of the `AspireSample.WorkerService` project, add the corresponding connection information:

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

1. Launch the Aspire app by selecting the run button (Visual Studio) or running `dotnet run --project AspireSample.AppHost`. The .NET Aspire dashboard app should open in the browser.
1. On the resources page, in the **apiservice** row, find the link in the **Endpoints** that opens the `weatherforecast` endpoint. Note the HTTPS port number.
1. On the .NET Aspire dashboard, navigate to the logs for the **aspiresample-workerservice** project.
1. In a terminal window, use the `curl` command to send a test message to the API:

    ```bash
    curl -X POST -H "Content-Type: application/json" https://localhost:{port}/notify?message=hello%20aspire  
    ```

    Be sure to replace **{port}** with the port number from earlier.
1. Switch back to the **aspiresample-workerservice** logs. You should see the test message printed in the output logs.

Congratulations! You created and configured an ASP.NET Core API that connects to Azure Service Bus using Aspire components.

[!INCLUDE [clean-up-resources](../includes/clean-up-resources.md)]
