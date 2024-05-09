---
title: Connect an ASP.NET Core app to .NET Aspire storage components
description: Learn how to connect an ASP.NET Core app to .NET Aspire storage components.
ms.date: 04/24/2024
ms.topic: tutorial
zone_pivot_groups: azure-storage-mechanism
ms.custom: devx-track-extended-azdevcli
---

# Tutorial: Connect an ASP.NET Core app to .NET Aspire storage components

Cloud-native apps often require scalable storage solutions that provide capabilities like blob storage, queues, or semi-structured NoSQL databases. .NET Aspire components simplify connections to various storage services, such as Azure Blob Storage. In this tutorial, you'll create an ASP.NET Core app that uses .NET Aspire components to connect to Azure Blob Storage and Azure Queue Storage to submit support tickets. The app sends the tickets to a queue for processing and uploads an attachment to storage. You'll learn how to:

> [!div class="checklist"]
>
> - Create a basic .NET app that is set up to use .NET Aspire components
> - Add .NET Aspire components to connect to multiple storage services
> - Configure and use .NET Aspire Component features to send and receive data

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

## Explore the completed sample app

A completed version of the sample app from this tutorial is available on GitHub. The project is also structured as a template for the [Azure Developer CLI](/azure/developer/azure-developer-cli/overview), meaning you can use the `azd up` command to automate Azure resource provisioning if you have the tool [installed](/azure/developer/azure-developer-cli/install-azd).

```bash
git clone https://github.com/Azure-Samples/dotnet-aspire-connect-storage.git
```

## Set up the Azure Storage resources

:::zone pivot="azure-portal,azure-cli"

For this article, you'll need data contributor access to an Azure Storage account with a blob container and storage queue. Ensure you have the following resources and configurations available:

:::zone-end

:::zone pivot="azurite"

For this article, you'll need to create a blob container and storage queue resource in your local development environment using an emulator. To do so, use Azurite. Azurite is a free, open source, cross-platform Azure Storage API compatible server (emulator) that runs in a Docker container.

To use the emulator you need to [install Azurite](/azure/storage/common/storage-use-azurite#install-azurite).

:::zone-end
:::zone pivot="azure-portal"

1. An Azure Storage account - [Create a storage account](/azure/storage/common/storage-account-create?tabs=azure-portal).
1. A Blob Storage container named **fileuploads** - [Create a blob storage container](/azure/storage/blobs/blob-containers-portal).
1. A Storage Queue named **tickets** - [Create a storage queue](/azure/storage/queues/storage-quickstart-queues-portal).

:::zone-end
:::zone pivot="azure-cli"

Run the following commands in the Azure CLI or CloudShell to set up the required Azure Storage resources:

```azurecli-interactive
az group create --name aspirestorage --location eastus2
az storage account create -n aspirestorage -g aspirestorage -l eastus2
az storage container create -n fileuploads --account-name aspirestorage
az storage queue create -n tickets --account-name aspirestorage
```

:::zone-end

:::zone pivot="azure-portal,azure-cli"

You also need to assign the following roles to the user account you are logged into Visual Studio with:

- Storage Blob Data Contributor - [Assign an Azure RBAC role](/azure/storage/queues/assign-azure-role-data-access?tabs=portal)
- Storage Queue Data Contributor - [Assign an Azure RBAC role](/azure/storage/queues/assign-azure-role-data-access?tabs=portal)

:::zone-end

:::zone pivot="azure-developer-cli"

The [Azure Developer CLI](/azure/developer/azure-developer-cli/overview) enables you to provision and deploy Azure resources using a template system. This tutorial provides a [complete template](https://github.com/Azure-Samples/dotnet-aspire-connect-storage.git) that provisions the required Azure resources and includes the completed sample application code. Run the following commands to initialize and run the template:

1. Run [`azd auth login`](/azure/developer/azure-developer-cli/reference#azd-auth) to sign-in to Azure:

    ```azurecli
    azd auth login
    ```

1. Run [`azd init`](/azure/developer/azure-developer-cli/reference#azd-init) to clone and initialize the sample template:

    ```azurecli
    azd init --template dotnet-aspire-connect-storage
    ```

1. Run [`azd up`](/azure/developer/azure-developer-cli/reference#azd-up) to provision the Azure resources:

    ```azurecli
    azd up
    ```

1. When prompted, select the subscription and Azure region for the provisioned resources. The template runs and completes the following tasks for you:

    - Creates an Azure Storage Account with blob and queue services enabled
    - Creates a blob storage container named `fileUploads`
    - Creates a queue named `tickets`
    - Assigns the following roles to the user account that ran the template.
      - Storage Blob Data Contributor
      - Storage Queue Data Contributor

After the operation completes successfully, you have two options moving forward:

- Option 1: Run the .NET sample app in the template `src` directory to experiment with the completed app.
- Option 2: Build the sample app step by step using the sections ahead and connect it to the Azure resources provisioned by `azd`.

:::zone-end

## Create the sample solution

Create a .NET Aspire app using either Visual Studio or the .NET CLI.

## [Visual Studio](#tab/visual-studio)

1. At the top of Visual Studio, navigate to **File** > **New** > **Project**.
1. In the dialog window, search for *Blazor* and select **Blazor Web App**. Choose **Next**.
1. On the **Configure your new project** screen:
    - Enter a **Project Name** of **AspireStorage**.
    - Leave the rest of the values at their defaults and select **Next**.
1. On the **Additional information** screen:
    - Make sure **.NET 8.0** is selected.
    - Ensure the **Interactive render mode** is set to **None**.
    - Check the **Enlist in .NET Aspire orchestration** option and select **Create**.

Visual Studio creates a new ASP.NET Core solution that is structured to use .NET Aspire. The solution consists of the following projects:

- **AspireStorage**: A Blazor project that depends on service defaults.
- **AspireStorage.AppHost**: An orchestrator project designed to connect and configure the different projects and services of your app. The orchestrator should be set as the startup project.
- **AspireStorage.ServiceDefaults**: A shared class library to hold configurations that can be reused across the projects in your solution.

## [.NET CLI](#tab/net-cli)

Use the [`dotnet new`](/dotnet/core/tools/dotnet-new) command to create a new .NET Aspire app:

```dotnetcli
dotnet new aspire-starter --name AspireStorage
```

The solution consists of the following projects:

- **AspireStorage** - An API project with default .NET Aspire service configurations.
- **AspireStorage.AppHost** - An orchestrator project designed to connect and configure the different projects and services of your app. The orchestrator should be set as the startup project.
- **AspireStorage.ServiceDefaults** - A shared class library to hold code that can be reused across the projects in your solution.

---

### Add the Worker Service project

Next, add a Worker Service project to the solution to retrieve and process messages as they are added to the Azure Storage queue.

## [Visual Studio](#tab/visual-studio)

1. In the solution explorer, right click on the top level _AspireStorage_ solution node and select **Add** > **New project**.
1. Search for and select the **Worker Service** template and choose **Next**.
1. For the **Project name**, enter _AspireStorage.Worker_ and select **Next**.
1. On the **Additional information** screen:
    - Make sure **.NET 8.0** is selected.
    - Make sure **Enlist in .NET Aspire orchestration** is checked and select **Create**.

Visual Studio adds the project to your solution and updates the _Program.cs_ file of the _AspireStorage.AppHost_ project with a new line of code:

```csharp
builder.AddProject<Projects.AspireStorage_Worker>();
```

Visual Studio tooling added this line of code to register your new project with the <xref:Aspire.Hosting.IDistributedApplicationBuilder> object, which enables orchestration features. For more information, see [.NET Aspire orchestration overview](../fundamentals/app-host-overview.md).

## [.NET CLI](#tab/net-cli)

1. In the root directory of the app, use the [dotnet new](/dotnet/core/tools/dotnet-new) command to create a new Worker Service app:

    ```dotnetcli
    dotnet new worker --name AspireStorage.WorkerService
    ```

1. Use the [dotnet sln](/dotnet/core/tools/dotnet-sln) command to add the project to the solution:

    ```
    dotnet sln AspireStorage.sln add AspireStorage.WorkerService/AspireStorage.WorkerService.csproj
    ```

1. Use the [dotnet add reference](/dotnet/core/tools/dotnet-add-reference) command to add project reference between the **.AppHost** and **.WorkerService** project:

    ```dotnetcli
    dotnet add AspireStorage.AppHost/AspireStorage.AppHost.csproj reference AspireStorage.WorkerService/AspireStorage.WorkerService.csproj
    ```

1. Add the following line of code to the _Program.cs_ file in the **AspireStorage.AppHost** project:

    ```csharp
    builder.AddProject<Projects.AspireStorage_WorkerService>("aspirestorage-workerservice");
    ```

---

The completed solution structure should resemble the following:

:::image type="content" source="media/storage-project.png" alt-text="A screenshot showing the structure of the .NET Aspire storage sample solution.":::

## Add the .NET Aspire components to the Blazor app

Add the [.NET Aspire Azure Blob Storage component](azure-storage-blobs-component.md) and [.NET Aspire Azure Queue Storage component](azure-storage-queues-component.md) packages to your _AspireStorage_ project:

```dotnetcli
dotnet add package Aspire.Azure.Storage.Blobs --prerelease
dotnet add package Aspire.Azure.Storage.Queues --prerelease
```

Your **AspireStorage** project is now set up to use .NET Aspire components. Here's the updated _AspireStorage.csproj_ file:

:::code language="xml" source="snippets/tutorial/AspireStorage/AspireStorage/AspireStorage.csproj" highlight="10-13":::

The next step is to add the components to the app.

In the _Program.cs_ file of the _AspireStorage_ project, add calls to the <xref:Microsoft.Extensions.Hosting.AspireBlobStorageExtensions.AddAzureBlobClient%2A> and <xref:Microsoft.Extensions.Hosting.AspireQueueStorageExtensions.AddAzureQueueClient%2A> extension methods after the creation of the `builder` but before the call to `AddServiceDefaults`. For more information, see [.NET Aspire service defaults](../fundamentals/service-defaults.md). Provide the name of your connection string as a parameter.

:::zone pivot="azurite"

:::code source="snippets/tutorial/AspireStorage/AspireStorage/Program.cs" highlight="2-3,7-8,29-38":::

:::zone-end
:::zone pivot="azure-portal,azure-cli"

:::code source="snippets/tutorial/AspireStorage/AspireStorage/Program.cs" range="1-26,40-58" highlight="2-3,7-8":::

:::zone-end

With the additional `using` statements, these methods accomplish the following tasks:

- Register a <xref:Azure.Storage.Blobs.BlobServiceClient?displayProperty=fullName> and a <xref:Azure.Storage.Queues.QueueServiceClient?displayProperty=fullName> with the DI container for connecting to Azure Storage.
- Automatically enable corresponding health checks, logging, and telemetry for the respective services.

:::zone pivot="azurite"

When the _AspireStorage_ project starts, it will create a `fileuploads` container in Azurite Blob Storage and a `tickets` queue in Azurite Queue Storage. This is conditional when the app is running in a development environment. When the app is running in a production environment, the container and queue are assumed to have already been created.

:::zone-end

## Add the .NET Aspire component to the Worker Service

The worker service handles pulling messages off of the Azure Storage queue for processing. Add the [.NET Aspire Azure Queue Storage component](azure-storage-queues-component.md) component package to your _AspireStorage.Worker_ app:

```dotnetcli
dotnet add package Aspire.Azure.Storage.Queues --prerelease
```

In the _Program.cs_ file of the _AspireStorage.Worker_ project, add a call to the <xref:Microsoft.Extensions.Hosting.AspireQueueStorageExtensions.AddAzureQueueClient%2A> extension method after the creation of the `builder` but before the call to `AddServiceDefaults`:

:::code source="snippets/tutorial/AspireStorage/AspireStorage.Worker/Program.cs" highlight="5":::

This method handles the following tasks:

- Register a <xref:Azure.Storage.Queues.QueueServiceClient> with the DI container for connecting to Azure Storage Queues.
- Automatically enable corresponding health checks, logging, and telemetry for the respective services.

## Create the form

The app requires a form for the user to be able to submit support ticket information and upload an attachment. The app uploads the attached file on the `Document` (<xref:Microsoft.AspNetCore.Http.IFormFile>) property to Azure Blob Storage using the injected <xref:Azure.Storage.Blobs.BlobServiceClient>. The <xref:Azure.Storage.Queues.QueueServiceClient> sends a message composed of the `Title` and `Description` to the Azure Storage Queue.

Use the following Razor markup to create a basic form, replacing the contents of the _Home.razor_ file in the _AspireStorage/Components/Pages_ directory:

:::code language="razor" source="snippets/tutorial/AspireStorage/AspireStorage/Components/Pages/Home.razor":::

For more information about creating forms in Blazor, see [ASP.NET Core Blazor forms overview](/aspnet/core/blazor/forms).

## Update the AppHost

The _AspireStorage.AppHost_ project is the orchestrator for your app. It's responsible for connecting and configuring the different projects and services of your app. The orchestrator should be set as the startup project.

To add Azure Storage hosting support to your <xref:Aspire.Hosting.IDistributedApplicationBuilder>, install the [Aspire.Hosting.Azure.Storage](https://www.nuget.org/packages/Aspire.Hosting.Azure.Storage) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.Storage --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.Storage"
                  Version="[SelectVersion]" />
```

---

Replace the contents of the _Program.cs_ file in the _AspireStorage.AppHost_ project with the following code:

:::zone pivot="azurite"

:::code source="snippets/tutorial/AspireStorage/AspireStorage.AppHost/Program.cs":::

The preceding code adds Azure storage, blobs, and queues, and when in development mode, it uses the emulator. Each project defines references for these resources that they depend on.

:::zone-end
:::zone pivot="azure-portal,azure-cli"

:::code source="snippets/tutorial/AspireStorage/AspireStorage.AppHost/Program.cs" range="1-5,11-23":::

The preceding code adds Azure storage, blobs, and queues, and defines references for these resources within each project that depend on them.

:::zone-end

## Process the items in the queue

When a new message is placed on the `tickets` queue, the worker service should retrieve, process, and delete the message. Update the _Worker.cs_ class, replacing the contents with the following code:

:::zone pivot="azurite"

:::code source="snippets/tutorial/AspireStorage/AspireStorage.Worker/Worker.cs":::

Before the worker service can process messages, it needs to be able to connect to the Azure Storage queue. With Azurite, you need to ensure that the queue is available before the worker service starts executing message queue processing.

:::zone-end
:::zone pivot="azure-portal,azure-cli"

:::code source="snippets/tutorial/AspireStorage/AspireStorage.Worker/Worker.cs" range="1-12,15-37":::

The worker service processes messages by connecting to the Azure Storage queue, and pulling messages off the queue.

:::zone-end

The worker service processes message in the queue and deletes them when they've been processed.

:::zone pivot="azure-portal,azure-cli,azure-developer-cli"

## Configure the connection strings

The **AspireStorage** and **AspireStorage.Worker** projects must be configured to connect to the correct Azure Storage Account you created earlier. You can specify the endpoints for the blob and queue services in the storage account using the `appsettings.json` file in each project.

1. In the **AspireStorage** project, add the following configuration to the `appsettings.Development.json` file:

    ```json
      "ConnectionStrings": {
        "BlobConnection": "https://<your-storage-account-name>.blob.core.windows.net/",
        "QueueConnection": "https://<your-storage-account-name>.queue.core.windows.net/"
      }
    ```

1. In the **AspireStorage.Worker** project, add the following configuration to the `appsettings.Development.json` file:

    ```json
      "ConnectionStrings": {
        "QueueConnection": "https://<your-storage-account-name>.queue.core.windows.net/"
      }
    ```

:::zone-end

## Run and test the app locally

The sample app is now ready for testing. Verify that the submitted form data is sent to Azure Blob Storage and Azure Queue Storage by completing the following steps:

## [Visual Studio](#tab/visual-studio)

1. Press the run button at the top of Visual Studio to launch your .NET Aspire app dashboard in the browser.
1. On the resources page, in the **aspirestorage** row, click the link in the **Endpoints** column to open the UI of your app.

    :::image type="content" source="media/support-app.png" lightbox="media/support-app.png" alt-text="A screenshot showing the home page of the .NET Aspire support application.":::

1. Enter sample data into the `Title` and `Description` form fields and select a simple file to upload.
1. Select the **Submit** button, and the form submits the support ticket for processing — and clears the form.
1. In a separate browser tab, use the Azure portal to navigate to the **Storage browser** in your Azure Storage Account.
1. Select **Containers** and then navigate into the **Documents** container to see the uploaded file.
1. You can verify the message on the queue was processed by looking at the **Project logs** of the [.NET Aspire dashboard](../fundamentals/dashboard/overview.md), and selecting the **aspirestorage.worker** from the dropdown.

    :::image type="content" source="media/queue-output.png" lightbox="media/queue-output.png"  alt-text="A screenshot showing the console output of the Worker app.":::

## [.NET CLI](#tab/net-cli)

1. In a terminal window at the root of your project, use the `dotnet run` command to start the app:

    ```csharp
    dotnet run --project AspireStorage.AppHost
    ```

1. On the resources page, in the **aspirestorage** row, click the link in the **Endpoints** column to open the UI of your app.

    :::image type="content" source="media/support-app.png" lightbox="media/support-app.png" alt-text="A screenshot showing the home page of the .NET Aspire support application.":::

1. Enter sample data into the `Title` and `Description` form fields and select a simple file to upload.
1. Select the **Submit** button, and the form submits the support ticket for processing — and clears the form.
1. In a separate browser tab, use the Azure portal to navigate to the **Storage browser** in your Azure Storage Account.
1. Select **Containers** and then navigate into the **Documents** container to see the uploaded file.
1. You can verify the message on the queue was processed by looking at the **Project logs** of the [.NET Aspire dashboard](../fundamentals/dashboard/overview.md), and selecting the **aspirestorage.worker** from the dropdown.

    :::image type="content" source="media/queue-output.png" lightbox="media/queue-output.png"  alt-text="A screenshot showing the console output of the Worker app.":::

---

## Summary

The example app that you built demonstrates persisting blobs from an ASP.NET Core Blazor Web App and processing queues in a [.NET Worker Service](/dotnet/core/extensions/workers). Your app connects to Azure Storage using .NET Aspire components. The app sends the support tickets to a queue for processing and uploads an attachment to storage.

:::zone pivot="azurite"

Since you choose to use Azurite, there's no need to clean up these resources when you're done testing them, as you created them locally in the context of an emulator. The emulator enabled you to test your app locally without incurring any costs, as no Azure resources were provisioned or created.

:::zone-end
:::zone pivot="azure-portal,azure-cli"

[!INCLUDE [clean-up-resources](../includes/clean-up-resources.md)]

:::zone-end
