---
title: Apply EF Core migrations in .NET Aspire
description: Learn about how to to apply Entity Framework Core migrations in .NET Aspire
ms.date: 3/20/2024
ms.topic: how-to
---

# Apply Entity Framework Core migrations in .NET Aspire

Since .NET Aspire apps use a containerized architecture, databases are ephemeral and can be recreated at any time. Entity Framework Core (EF Core) uses a feature called [migrations](/ef/core/managing-schemas/migrations) to create and update database schemas. Since databases are recreated when the app starts, you need to apply migrations to initialize the database schema each time your app starts. This is accomplished by registering a migration service project in your app that runs migrations during startup.

In this tutorial, you learn how to configure .NET Aspire apps to run EF Core migrations during app startup.

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

## Obtain the starter app

This tutorial uses a sample app that demonstrates how to apply EF Core migrations in .NET Aspire. Use Visual Studio to clone [the sample app from GitHub](https://github.com/MicrosoftDocs/mslearn-aspire-starter) or use the following command:

```bash
git clone https://github.com/MicrosoftDocs/mslearn-aspire-starter
```

The sample app is in the *SupportTicketApi* folder. Open the solution in Visual Studio or VS Code and take a moment to review the sample app and make sure it runs before proceeding. The sample app is a rudimentary support ticket API, and it contains the following projects:

- **SupporTicketApi.Api**: The ASP.NET Core project that hosts the API.
- **SupportTicketApi.Data**: Contains the EF Core contexts and models.
- **SupportTicketApi.AppHost**: Contains the Aspire app host and configuration.
- **SupportTicketApi.ServiceDefaults**: Contains the default service configurations.

Run the app to ensure it works as expected. From the Aspire dashboard, select the **https** Swagger endpoint and test the API's **GET /api/SupportTickets** endpoint by expanding the operation and selecting **Try it out**. Select **Execute** to send the request and view the response:

```json
[
  {
    "id": 1,
    "title": "Initial Ticket",
    "description": "Test ticket, please ignore."
  }
]
```

## Create migrations

Start by creating some migrations to apply.

1. Open a terminal (<kbd>Ctrl</kbd>+<kbd>\`</kbd> in Visual Studio).
1. Set *SupportTicketApi\SupportTicketApi.Api* as the current directory.
1. Use the [`dotnet ef` command-line tool](https://learn.microsoft.com/ef/core/managing-schemas/migrations/#install-the-tools) to create a new migration to capture the initial state of the database schema:

    ```dotnetcli
    dotnet ef migrations add InitialCreate --project ..\SupportTicketApi.Data\SupportTicketApi.Data.csproj
    ```

    The proceeding command:

      - Runs EF Core migration command-line tool in the *SupportTicketApi.Api* directory. `dotnet ef` is run in this location because the API service is where the DB context is used.
      - Creates a migration named *InitialCreate*.
      - Creates the migration in the in the *Migrations* folder in the *SupportTicketApi.Data* project.

1. Modify the model so that it includes a new property. Open *SupportTicketApi.Data\Models\SupportTicket.cs* and add a new property to the `SupportTicket` class:

    :::code source="~/aspire-docs-samples-solution/SupportTicketApi/SupportTicketApi.Data/Models/SupportTicket.cs" range="5-13" highlight="8" :::

1. Create a new migration to capture the changes to the model:

    ```dotnetcli
    dotnet ef migrations add AddCompleted --project ..\SupportTicketApi.Data\SupportTicketApi.Data.csproj
    ```

Now you've got some migrations to apply. Next, you'll create a migration service that applies these migrations during app startup.

## Create the migration service

To run the migrations at startup, you need to create a service that applies the migrations.

1. Add a new Worker Service project to the solution. If using Visual Studio, right-click the solution in Solution Explorer and select **Add** > **New Project**. Select **Worker Service** and name the project *SupportTicketApi.Migrations*. If using the command line, use the following commands from the solution directory:

    ```dotnetcli
    dotnet new worker -n SupportTicketApi.Migrations
    dotnet sln add SupportTicketApi.Migrations
    ```

1. Add the *SupportTicketApi.Data* and *SupportTicketApi.ServiceDefaults* project references to the *SupportTicketApi.Migrations* project using Visual Studio or the command line:

    ```dotnetcli
    dotnet add SupportTicketApi.Migrations reference SupportTicketApi.Data
    dotnet add SupportTicketApi.Migrations reference SupportTicketApi.ServiceDefaults
    ```

1. Add the *Aspire.Microsoft.EntityFrameworkCore.SqlServer* NuGet package reference to the *SupportTicketApi.Migrations* project using Visual Studio or the command line:

    ```dotnetcli
    dotnet add package Aspire.Microsoft.EntityFrameworkCore.SqlServer
    ```

1. Replace the contents of the *Worker.cs* file in the *SupportTicketApi.Migrations* project with the following code:

    :::code source="~/aspire-docs-samples-solution/SupportTicketApi/SupportTicketApi.Migrations/Worker.cs" :::

    In the preceding code:

    - The `ExecuteAsync` method is called when the worker starts. It in turn performs the following steps:
      1. Gets a reference to the `TicketContext` service from the service provider.
      1. Calls `EnsureDatabaseAsync` to create the database if it doesn't exist.
      1. Calls `RunMigrationAsync` to apply any pending migrations.
      1. Calls `SeedDataAsync` to seed the database with initial data.
      1. Stops the worker with `StopApplication`.
    - `EnsureDatabaseAsync`, `RunMigrationAsync`, and `SeedDataAsync` all encapsulate their database operations using execution strategies to handle transient errors that may occur when interacting with the database. To learn more about execution strategies, see [Connection Resiliency](/ef/core/miscellaneous/connection-resiliency).

Remaining steps are WIP:

1. Add the migration service to the app host.
1. Remove the seed logic in the API
