---
title: Connect an ASP.NET Core app to SQL Server using .NET Aspire and Entity Framework Core
description: Learn how to connect an ASP.NET Core app to to SQL Server using .NET Aspire and Entity Framework Core.
ms.date: 01/22/2024
ms.topic: tutorial
---

# Tutorial: Connect an ASP.NET Core app to SQL Server using .NET Aspire and Entity Framework Core

In this tutorial, you create an ASP.NET Core app that uses a .NET Aspire Entity Framework Core SQL Server component to connect to SQL Server to read and write support ticket data. [Entity Framework Core](/ef/core/) is a lightweight, extensible, open source object-relational mapper that enables .NET developers to work with databases using .NET objects. You'll learn how to:

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
    - Enter a **Project Name** of **AspireSQLEFCore**.
    - Leave the rest of the values at their defaults and select **Next**.
1. On the **Additional information** screen:
    - Make sure **.NET 8.0** is selected.
    - Ensure the **Interactive render mode** is set to **None**.
    - Check the **Enlist in .NET Aspire orchestration** option and select **Create**.

Visual Studio creates a new ASP.NET Core solution that is structured to use .NET Aspire. The solution consists of the following projects:

- **AspireSQLEFCore**: A Blazor project that depends on service defaults.
- **AspireSQLEFCore.AppHost**: An orchestrator project designed to connect and configure the different projects and services of your app. The orchestrator should be set as the startup project.
- **AspireSQLEFCore.ServiceDefaults**: A shared class library to hold configurations that can be reused across the projects in your solution.

## Create the database model and context classes

To represent a user submitted support request, add the following `SupportTicket` model class at the root of the _AspireSQLEFCore_ project.

:::code source="snippets/tutorial/AspireSQLEFCore/AspireSQLEFCore/SupportTicket.cs":::

Add the following `TicketDbContext` data context class at the root of the **AspireSQLEFCore** project. The class inherits <xref:System.Data.Entity.DbContext?displayProperty=fullName> to work with Entity Framework and represent your database.

:::code source="snippets/tutorial/AspireSQLEFCore/AspireSQLEFCore/TicketContext.cs":::

## Add the .NET Aspire component to the Blazor app

Add the [.NET Aspire Entity Framework Core Sql Server library](/dotnet/aspire/database/sql-server-entity-framework-component?tabs=dotnet-cli) package to your _AspireSQLEFCore_ project:

```dotnetcli
dotnet add package Aspire.Microsoft.EntityFrameworkCore.SqlServer --prerelease
```

Your _AspireSQLEFCore_ project is now set up to use .NET Aspire components. Here's the updated _AspireSQLEFCore.csproj_ file:

:::code language="csharp" source="snippets/tutorial/AspireSQLEFCore/AspireSQLEFCore/AspireSQLEFCore.csproj" highlight="10, 11":::

## Configure the .NET Aspire component

In the _Program.cs_ file of the _AspireSQLEFCore_ project, add a call to the <xref:Microsoft.Extensions.Hosting.AspireSqlServerEFCoreSqlClientExtensions.AddSqlServerDbContext%2A> extension method after the creation of the `builder` but before the call to `AddServiceDefaults`. For more information, see [.NET Aspire service defaults](../fundamentals/service-defaults.md). Provide the name of your connection string as a parameter.

:::code language="csharp" source="snippets/tutorial/AspireSQLEFCore/AspireSQLEFCore/Program.cs" range="1-14" highlight="5":::

This method accomplishes the following tasks:

- Registers a `TicketDbContext` with the DI container for connecting to the containerized Azure SQL Database.
- Automatically enable corresponding health checks, logging, and telemetry.

## Migrate and seed the database

While developing locally, you need to create a database inside the SQL Server container. Update the _Program.cs_ file with the following code to automatically run Entity Framework migrations during startup.

:::code language="csharp" source="snippets/tutorial/AspireSQLEFCore/AspireSQLEFCore/Program.cs" range="1-30" highlight="16-30":::

## Create the form

The app requires a form for the user to be able to submit support ticket information and save the entry to the database.

Use the following Razor markup to create a basic form, replacing the contents of the _Home.razor_ file in the _AspireSQLEFCore/Components/Pages_ directory:

:::code language="razor" source="snippets/tutorial/AspireSQLEFCore/AspireSQLEFCore/Components/Pages/Home.razor":::

For more information about creating forms in Blazor, see [ASP.NET Core Blazor forms overview](/aspnet/core/blazor/forms).

## Configure the AppHost

The _AspireSQLEFCore.AppHost_ project is the orchestrator for your app. It's responsible for connecting and configuring the different projects and services of your app. The orchestrator should be set as the startup project.

Add the [.NET Aspire Entity Framework Core Sql Server library](/dotnet/aspire/database/sql-server-entity-framework-component?tabs=dotnet-cli) package to your _AspireStorage.AppHost_ project:

```dotnetcli
dotnet add package Aspire.Microsoft.EntityFrameworkCore.SqlServer --prerelease
```

Replace the contents of the _Program.cs_ file in the _AspireSQLEFCore.AppHost_ project with the following code:

:::code language="csharp" source="snippets/tutorial/AspireSQLEFCore/AspireSQLEFCore.AppHost/Program.cs":::

The preceding code adds a SQL Server Container resource to your app and configures a connection to a database called `sqldata`. The Entity Framework classes you configured earlier will automatically use this connection when migrating and connecting to the database.

## Run and test the app locally

The sample app is now ready for testing. Verify that the submitted form data is persisted to the database by completing the following steps:

1. Select the run button at the top of Visual Studio (or <kbd>F5</kbd>) to launch your .NET Aspire app dashboard in the browser.
1. On the projects page, in the **AspireSQLEFCore** row, click the link in the **Endpoints** column to open the UI of your app.

    :::image type="content" source="media/app-home-screen.png" lightbox="media/app-home-screen.png" alt-text="A screenshot showing the home page of the .NET Aspire support application.":::

1. Enter sample data into the `Title` and `Description` form fields.
1. Select the **Submit** button, and the form submits the support ticket for processing — and clears the form.
1. The data you submitted displays in the table at the bottom of the page when the page reloads.

## See also

- [.NET Aspire with SQL Database deployment](sql-server-component-deployment.md)
- [.NET Aspire deployment via Azure Container Apps](../deployment/azure/aca-deployment.md)
- [Deploy a .NET Aspire app using GitHub Actions](../deployment/azure/aca-deployment-github-actions.md)
