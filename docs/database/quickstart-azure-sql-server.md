---
title: Connect an ASP.NET Core app to .NET Aspire storage components
description: Learn how to connect an ASP.NET Core app to .NET Aspire storage components.
ms.date: 12/01/2023
ms.topic: tutorial
---

# Tutorial: Connect an ASP.NET Core app to .NET Aspire storage components

In this quickstart, you'll create an ASP.NET Core app that uses .NET Aspire components to connect to SQL Server to read and write support tickets. The app sends the tickets to a queue for processing and uploads an attachment to storage. You'll learn how to:

> [!div class="checklist"]
>
> - Create a basic .NET app that is set up to use .NET Aspire components
> - Add a .NET Aspire component to connect to SQL Server
> - Configure and use .NET Aspire Component features to read and write from the database

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

## Create the sample solution

1. At the top of Visual Studio, navigate to **File** > **New** > **Project**.
1. In the dialog window, search for *Blazor* and select **Blazor Web App**. Choose **Next**.
1. On the **Configure your new project** screen:
    - Enter a **Project Name** of **AspireSQL**.
    - Leave the rest of the values at their defaults and select **Next**.
1. On the **Additional information** screen:
    - Make sure **.NET 8.0** is selected.
    - Ensure the **Interactive render mode** is set to **None**.
    - Check the **Enlist in .NET Aspire orchestration** option and select **Create**.

Visual Studio creates a new ASP.NET Core solution that is structured to use .NET Aspire. The solution consists of the following projects:

- **AspireSQL**: A Blazor project that depends on service defaults.
- **AspireSQL.AppHost**: An orchestrator project designed to connect and configure the different projects and services of your app. The orchestrator should be set as the startup project.
- **AspireSQL.ServiceDefaults**: A shared class library to hold configurations that can be reused across the projects in your solution.

## Add the .NET Aspire components to the Blazor app

Add the [.NET Aspire SQL Server SqlClient library](azure-storage-blobs-component.md) package to your _AspireSQL_ project:

```dotnetcli
dotnet add package Aspire.Microsoft.Data.SqlClient --prerelease
```

Your **AspireSQL** project is now set up to use .NET Aspire components. Here's the updated _AspireSQL.csproj_ file:

:::code language="xml" source="snippets/tutorial/AspireSQL/AspireSQL/AspireSQL.csproj" highlight="10-13":::

The next step is to add the components to the app.

In the _Program.cs_ file of the _AspireSQL_ project, add calls to the <xref:Microsoft.Extensions.Hosting.AspireSqlServerEFCoreSqlClientExtensions.AddSqlServerDbContext%2A> extension method after the creation of the `builder` but before the call to `AddServiceDefaults`. For more information, see [.NET Aspire service defaults](../service-defaults.md). Provide the name of your connection string as a parameter.

:::code source="snippets/tutorial/AspireSQL/AspireSQL/Program.cs" range="1-26,40-58" highlight="2-3,7-8":::

With the additional `using` statements, these methods accomplish the following tasks:

- Register a `TicketDbContext` with the DI container for connecting to the containerized Azure SQL Database.
- Automatically enable corresponding health checks, logging, and telemetry.

## Create the form

The app requires a form for the user to be able to submit support ticket information and save it to the database.

Use the following Razor markup to create a basic form, replacing the contents of the _Home.razor_ file in the _AspireSQL/Components/Pages_ directory:

:::code language="razor" source="snippets/tutorial/AspireSQL/AspireSQL/Components/Pages/Home.razor":::

For more information about creating forms in Blazor, see [ASP.NET Core Blazor forms overview](/aspnet/core/blazor/forms).

## Update the AppHost

The _AspireSQL.AppHost_ project is the orchestrator for your app. It's responsible for connecting and configuring the different projects and services of your app. The orchestrator should be set as the startup project.

Replace the contents of the _Program.cs_ file in the _AspireSQL.AppHost_ project with the following code:

:::code source="snippets/tutorial/AspireSQL/AspireSQL.AppHost/Program.cs":::

The preceding code adds Azure storage, blobs, and queues, and when in development mode, it uses the emulator. Each project defines references for these resources that they depend on.

## Run and test the app locally

The sample app is now ready for testing. Verify that the submitted form data is sent to Azure Blob Storage and Azure Queue Storage by completing the following steps:

1. Press the run button at the top of Visual Studio to launch your .NET Aspire app dashboard in the browser.
1. On the projects page, in the **AspireSQL** row, click the link in the **Endpoints** column to open the UI of your app.

    :::image type="content" source="media/support-app.png" lightbox="media/support-app.png" alt-text="A screenshot showing the home page of the .NET Aspire support application.":::

1. Enter sample data into the `Title` and `Description` form fields and select a simple file to upload.
1. Select the **Submit** button, and the form submits the support ticket for processing — and clears the form.
1. In a separate browser tab, use the Azure portal to navigate to the **Storage browser** in your Azure Storage Account.
1. Select **Containers** and then navigate into the **Documents** container to see the uploaded file.
1. You can verify the message on the queue was processed by looking at the **Project logs** of the [.NET Aspire dashboard](../dashboard.md), and selecting the **AspireSQL.worker** from the dropdown.

    :::image type="content" source="media/queue-output.png" lightbox="media/queue-output.png"  alt-text="A screenshot showing the console output of the Worker app.":::

## Summary

The example app that you built demonstrates persisting blobs from an ASP.NET Core Blazor Web App and processing queues in a [.NET Worker Service](/dotnet/core/extensions/workers). Your app connects to Azure Storage using .NET Aspire components. The app sends the support tickets to a queue for processing and uploads an attachment to storage.

:::zone pivot="azurite"

Since you choose to use Azurite, there's no need to clean up these resources when you're done testing them, as you created them locally in the context of an emulator. The emulator enabled you to test your app locally without incurring any costs, as no Azure resources were provisioned or created.

:::zone-end
:::zone pivot="azure-portal,azure-cli"

Don't forget to clean up any Azure resources when you're done testing them.

:::zone-end
