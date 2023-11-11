---
title: Connect an ASP.NET Core app to .NET Aspire storage components
description: Learn how to connect an ASP.NET Core app to .NET Aspire storage components.
ms.date: 11/10/2023
ms.topic: quickstart
ms.prod: dotnet
---

# Tutorial: Connect an ASP.NET Core app to .NET Aspire storage components

Cloud-native apps often require scalable storage solutions that provide capabilities like blob storage, queues, or semi-structured NoSQL databases. .NET Aspire components simplify connections to various storage services, such as Azure Blob Storage. In this quickstart, you'll create an ASP.NET Core app that uses .NET Aspire components to connect to Azure Blob Storage and Azure Queue Storage to submit support tickets. The app sends the tickets to a queue for processing and uploads an attachment to storage. You'll learn how to:

> [!div class="checklist"]
>
> - Create a basic .NET app that is setup to use .NET Aspire Components
> - Add .NET Aspire Components to connect to multiple storage services
> - Configure and use .NET Aspire Component features to send and receive data

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

## Set up the Azure Storage Account

For this quickstart, you'll need data contributor access to an Azure Storage Account with a blob container and storage queue. Ensure you have the following resources and configurations available:

# [Portal](#tab/portal)

1. An Azure Storage account - [Create a storage account](/azure/storage/common/storage-account-create?tabs=azure-portal).
1. A Blob Storage container named **fileuploads** - [Create a blob storage container](/azure/storage/blobs/blob-containers-portal).
1. A Storage Queue named **tickets** - [Create a storage queue](/azure/storage/queues/storage-quickstart-queues-portal).

# [Azure CLI script](#tab/cli)

Run the following commands in the Azure CLI or CloudShell to setup the required Azure Storage resources:

```azurecli-interactive
az group create --name aspirestorage --location eastus2
az storage account create -n aspirestorage -g aspirestorage -l eastus2
az storage container create -n fileuploads --account-name aspirestorage
az storage queue create -n tickets --account-name aspirestorage
```

---

You also need to assign the following roles to the user account you are logged into Visual Studio with:

- Storage Blob Data Contributor - [Assign an Azure RBAC role](/azure/storage/queues/assign-azure-role-data-access?tabs=portal)
- Storage Queue Data Contributor - [Assign an Azure RBAC role](/azure/storage/queues/assign-azure-role-data-access?tabs=portal)

## Create the sample solution

1. At the top of Visual Studio, navigate to **File** > **New** > **Project**.
1. In the dialog window, search for *Blazor* and select **Blazor Web App**. Choose **Next**.
1. On the **Configure your new project** screen:
    - Enter a **Project Name** of **AspireStorage**.
    - Leave the rest of the values at their defaults and select **Next**.
1. On the **Additional information** screen:
    - Make sure **.NET 8.0** is selected.
    - Make sure **Enlist in .NET Aspire orchestration** is checked and select **Create**.

Visual Studio creates a new ASP.NET Core solution that is structured to use .NET Aspire. The solution consists of the following projects:

- **AspireStorage** - A Blazor project with default .NET Aspire service configurations.
- **AspireStorage.AppHost** - An orchestrator project designed to connect and configure the different projects and services of your app. The orchestrator should be set as the startup project.
- **AspireStorage.Shared** - A shared class library to hold configurations that can be reused across the projects in your solution.

### Add the Worker Service project

Next, add a Worker Service project to the solution to retrieve and process messages as they are added to the Azure Storage queue.

1. In the solution explorer, right click on the top level `AspireStorage` solution node and select **Add** > **New project**.
1. Search for and select the **Worker Service** template and choose **Next**.
1. For the **Project name**, enter *AspireStorage.Worker* and select **Next**.
1. On the **Additional information** screen:
    - Make sure **.NET 8.0** is selected.
    - Make sure **Enlist in .NET Aspire orchestration** is checked and select **Create**.

Visual Studio adds the project to your solution and updates the _Program.cs_ file of the `AspireStorage.AppHost` project with a new line of code:

```csharp
builder.AddProject<AspireStorage.AppHost.Projects.AspireStorageWorker>();
```

Visual Studio tooling added this line of code to register your new project with the `CloudApplicationBuilder` object, which enables orchestration features you'll explore later.

The completed solution structure should resemble the following:

:::image type="content" source="../media/storage-project.png" alt-text="A screenshot showing the structure of the .NET Aspire storage sample project.":::

## Add the .NET Aspire components to the Blazor app

Add the [Aspire Azure Blob Storage](/aspire/storage/aspire-azure-storage-blobs) and [Aspire Azure Queue Storage](/aspire/storage/aspire-azure-storage-queues) component packages to your `AspireStorage` app:

```dotnetcli
dotnet add package Aspire.Azure.Storage.Blobs --prerelease
dotnet add package Aspire.Azure.Storage.Queues --prerelease
```

In the _Program.cs_ file of the `AspireStorage` project, add calls to the `AddAzureBlobService` and `AddAzureQueueService` extension methods. Provide the name of your connection string as a parameter.

```csharp
builder.AddAzureBlobService("blobConnection");
builder.AddAzureQueueService("queueConnection");
```

These methods accomplish the following tasks:

- Register a <xref:Azure.Storage.Blobs.BlobServiceClient?displayProperty=fullName> and a <xref:Azure.Storage.Queues.QueueServiceClient?displayProperty=fullName> with the DI container for connecting to Azure Storage.
- Automatically enable corresponding health checks, logging, and telemetry for the respective services.

In the _appsettings.json file of the `AspireStorage` project, add the corresponding connection information:

```json
"ConnectionStrings": {
  "blobConnection": "https://{account_name}.blob.core.windows.net/",
  "queueConnection": "https://{account_name}.queue.core.windows.net/"
}
```

## Add the .NET Aspire component to the Worker Service

The worker service handles pulling messages off of the Azure Storage queue for processing. Add the [Aspire Azure Queue Storage](/aspire/storage/aspire-azure-storage-queues) component package to your `AspireStorage.Worker` app:

```dotnetcli
dotnet add package Aspire.Azure.Storage.Queues --prerelease
```

In the _Program.cs_ file of the `AspireStorage.Worker` project, add a call to the `AddAzureQueueService` extension methods:

```csharp
builder.AddAzureQueueService("queueConnection");
```

This method handles the following tasks:

- Register a `QueueServiceClient` with the DI container for connecting to Azure Storage Queues.
- Automatically enable corresponding health checks, logging, and telemetry for the respective services.

In the _appsettings.json file of the `AspireStorage.Worker` project, add the corresponding connection string information:

```json
"ConnectionStrings": {
  "queueConnection": "https://{account_name}.queue.core.windows.net/"
}
```

## Create the form

The application requires a form for the user to be able to submit support ticket information and upload an attachment. The app uploads the attached file on the `Document` property to Azure Blob Storage using the injected <xref:Azure.Storage.Blobs.BlobServiceClient?displayProperty=fullName>. The <xref:Azure.Storage.Queues.QueueServiceClient?displayProperty=fullName> sends a message composed of the `Title` and `Description` to the Azure Storage Queue.

Use following Razor and bootstrap markup to create a basic form:

```razor
@page "/"
@using Azure.Storage.Blobs
@using Azure.Storage.Queues
@inject BlobServiceClient blobClient
@inject QueueServiceClient queueServiceClient

<PageTitle>Home</PageTitle>

<div class="text-center">
    <h1 class="display-4">Request Support</h1>
</div>

<EditForm Model="@Ticket" FormName="Tickets" method="post" 
    OnValidSubmit="@HandleValidSubmit" enctype="multipart/form-data">
    <div class="mb-4">
        <label>Issue Title</label>
        <InputText class="form-control" @bind-Value="@Ticket.Title" />
    </div>
    <div class="mb-4">
        <label>Issue Description</label>
        <InputText class="form-control" @bind-Value="@Ticket.Description" />
    </div>
    <div class="mb-4">
        <label>Attachment</label>
        <InputFile class="form-control" name="Ticket.Document" />
    </div>
    <button class="btn btn-primary" type="submit">Submit</button>
</EditForm>

@code {
    [SupplyParameterFromForm(FormName = "Tickets")]
    private SupportTicket Ticket { get; set; } = new SupportTicket();

    private async void HandleValidSubmit()
    {
        var docsContainer = blobClient.GetBlobContainerClient("fileuploads");

        // Upload file to blob storage
        await docsContainer.UploadBlobAsync(
            Ticket.Document.FileName,
            Ticket.Document.OpenReadStream());

        // Send message to queue
        var queueClient = queueServiceClient.GetQueueClient("tickets");
        await queueClient.SendMessageAsync(
             $"{Ticket.Title} - {Ticket.Description}");
    }

    private class SupportTicket()
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile Document { get; set; }
    }
}
```

## Process the items in the queue

When a new message is placed on the `tickets` queue, the worker service should retrieve, process, and delete the message. Update the `Worker.cs` class to match the following code:

```csharp
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace AspireProcessor;

public sealed class Worker(
    QueueServiceClient client,
    ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var queueClient = client.GetQueueClient("tickets");
            QueueMessage[] messages =
                await queueClient.ReceiveMessagesAsync(maxMessages: 25);

            foreach(var message in messages)
            {
                logger.LogInformation(
                    "Message from queue: {Message}", message.MessageText);

                await queueClient.DeleteMessageAsync(
                    message.MessageId, message.PopReceipt);
            }
        }
    }
}
```

## Run and test the app locally

The sample app is now ready for testing. Verify that the submitted form data is sent to Azure Blob Storage and Azure Queue Storage by completing the following steps:

1. Press the run button at the top of Visual Studio to launch your .NET Aspire app dashboard in the browser.
1. On the projects page, in the **asireweb** row, click the link in the **Endpoints** column to open the UI of your app.

    :::image type="content" source="../media/support-app.png" alt-text="A screenshot showing the home page of the .NET Aspire support application.":::

1. Enter sample data into the `Title` and `Description` form fields and select a simple file to upload.
1. Press submit, and the page should reload.
1. In a separate browser tab, use the Azure portal to navigate to the **Storage browser** in your Azure Storage Account.
1. Select **Containers** and then navigate into the **Documents** container to see the uploaded file.
1. You can verify the message on the queue was processed by searching for the **Message Processed** log from the the worker service in the Visual Studio output window.

    :::image type="content" source="../media/queue-output.png" alt-text="A screenshot showing the console output of the Worker app.":::

Congratulations! You created and configured an ASP.NET Core app app that connects to Azure Storage using .NET Aspire components.
