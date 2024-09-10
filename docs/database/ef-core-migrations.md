---
title: Apply EF Core migrations in .NET Aspire
description: Learn about how to to apply Entity Framework Core migrations in .NET Aspire
ms.date: 07/31/2024
ms.topic: how-to
---

# Apply Entity Framework Core migrations in .NET Aspire

Since .NET Aspire projects use a containerized architecture, databases are ephemeral and can be recreated at any time. Entity Framework Core (EF Core) uses a feature called [migrations](/ef/core/managing-schemas/migrations) to create and update database schemas. Since databases are recreated when the app starts, you need to apply migrations to initialize the database schema each time your app starts. This is accomplished by registering a migration service project in your app that runs migrations during startup.

In this tutorial, you learn how to configure .NET Aspire projects to run EF Core migrations during app startup.

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

## Obtain the starter app

This tutorial uses a sample app that demonstrates how to apply EF Core migrations in .NET Aspire. Use Visual Studio to clone [the sample app from GitHub](https://github.com/MicrosoftDocs/aspire-docs-samples/) or use the following command:

```bash
git clone https://github.com/MicrosoftDocs/aspire-docs-samples/
```

The sample app is in the *SupportTicketApi* folder. Open the solution in Visual Studio or VS Code and take a moment to review the sample app and make sure it runs before proceeding. The sample app is a rudimentary support ticket API, and it contains the following projects:

- **SupportTicketApi.Api**: The ASP.NET Core project that hosts the API.
- **SupportTicketApi.Data**: Contains the EF Core contexts and models.
- **SupportTicketApi.AppHost**: Contains the .NET Aspire app host and configuration.
- **SupportTicketApi.ServiceDefaults**: Contains the default service configurations.

Run the app to ensure it works as expected. From the .NET Aspire dashboard, select the **https** Swagger endpoint and test the API's **GET /api/SupportTickets** endpoint by expanding the operation and selecting **Try it out**. Select **Execute** to send the request and view the response:

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
1. Set *:::no-loc text="SupportTicketApi\SupportTicketApi.Api":::* as the current directory.
1. Use the [`dotnet ef` command-line tool](/ef/core/managing-schemas/migrations/#install-the-tools) to create a new migration to capture the initial state of the database schema:

    ```dotnetcli
    dotnet ef migrations add InitialCreate --project ..\SupportTicketApi.Data\SupportTicketApi.Data.csproj
    ```

    The proceeding command:

      - Runs EF Core migration command-line tool in the *SupportTicketApi.Api* directory. `dotnet ef` is run in this location because the API service is where the DB context is used.
      - Creates a migration named *InitialCreate*.
      - Creates the migration in the in the *Migrations* folder in the *SupportTicketApi.Data* project.

1. Modify the model so that it includes a new property. Open *:::no-loc text="SupportTicketApi.Data\Models\SupportTicket.cs":::* and add a new property to the `SupportTicket` class:

    :::code source="~/aspire-docs-samples-solution/SupportTicketApi/SupportTicketApi.Data/Models/SupportTicket.cs" range="5-13" highlight="8" :::

1. Create another new migration to capture the changes to the model:

    ```dotnetcli
    dotnet ef migrations add AddCompleted --project ..\SupportTicketApi.Data\SupportTicketApi.Data.csproj
    ```

Now you've got some migrations to apply. Next, you'll create a migration service that applies these migrations during app startup.

## Create the migration service

To run the migrations at startup, you need to create a service that applies the migrations.

1. Add a new Worker Service project to the solution. If using Visual Studio, right-click the solution in Solution Explorer and select **:::no-loc text="Add":::** > **:::no-loc text="New Project":::**. Select **:::no-loc text="Worker Service":::** and name the project *:::no-loc text="SupportTicketApi.MigrationService":::*. If using the command line, use the following commands from the solution directory:

    ```dotnetcli
    dotnet new worker -n SupportTicketApi.MigrationService
    dotnet sln add SupportTicketApi.MigrationService
    ```

1. Add the *:::no-loc text="SupportTicketApi.Data":::* and *:::no-loc text="SupportTicketApi.ServiceDefaults":::* project references to the *:::no-loc text="SupportTicketApi.MigrationService":::* project using Visual Studio or the command line:

    ```dotnetcli
    dotnet add SupportTicketApi.MigrationService reference SupportTicketApi.Data
    dotnet add SupportTicketApi.MigrationService reference SupportTicketApi.ServiceDefaults
    ```

1. Add the [Aspire.Microsoft.EntityFrameworkCore.SqlServer](https://www.nuget.org/packages/Aspire.Microsoft.EntityFrameworkCore.SqlServer) NuGet package reference to the *:::no-loc text="SupportTicketApi.MigrationService":::* project using Visual Studio or the command line:

    ```dotnetcli
    dotnet add package Aspire.Microsoft.EntityFrameworkCore.SqlServer
    ```

1. Add the highlighted lines to the *:::no-loc text="Program.cs":::* file in the *:::no-loc text="SupportTicketApi.MigrationService":::* project:

    :::code source="~/aspire-docs-samples-solution/SupportTicketApi/SupportTicketApi.MigrationService/Program.cs" highlight="1,6,9-12" :::

    In the preceding code:

    - The `AddServiceDefaults` extension method [adds service defaults functionality](/dotnet/aspire/fundamentals/service-defaults#add-service-defaults-functionality).
    - The `AddOpenTelemetry` extension method [configures OpenTelemetry functionality](/dotnet/aspire/fundamentals/telemetry#net-aspire-opentelemetry-integration).
    - The `AddSqlServerDbContext` extension method adds the `TicketContext` service to the service collection. This service is used to run migrations and seed the database.

1. Replace the contents of the *:::no-loc text="Worker.cs":::* file in the *:::no-loc text="SupportTicketApi.MigrationService":::* project with the following code:

    :::code source="~/aspire-docs-samples-solution/SupportTicketApi/SupportTicketApi.MigrationService/Worker.cs" :::

    In the preceding code:

    - The `ExecuteAsync` method is called when the worker starts. It in turn performs the following steps:
      1. Gets a reference to the `TicketContext` service from the service provider.
      1. Calls `EnsureDatabaseAsync` to create the database if it doesn't exist.
      1. Calls `RunMigrationAsync` to apply any pending migrations.
      1. Calls `SeedDataAsync` to seed the database with initial data.
      1. Stops the worker with `StopApplication`.
    - The `EnsureDatabaseAsync`, `RunMigrationAsync`, and `SeedDataAsync` methods all encapsulate their respective database operations using execution strategies to handle transient errors that may occur when interacting with the database. To learn more about execution strategies, see [Connection Resiliency](/ef/core/miscellaneous/connection-resiliency).

## Add the migration service to the orchestrator

The migration service is created, but it needs to be added to the .NET Aspire app host so that it runs when the app starts.

1. In the *:::no-loc text="SupportTicketApi.AppHost":::* project, open the *:::no-loc text="Program.cs":::* file.
1. Add the following highlighted code to the `ConfigureServices` method:

    :::code source="~/aspire-docs-samples-solution/SupportTicketApi/SupportTicketApi.AppHost/Program.cs" highlight="9-10" :::

    This enlists the *:::no-loc text="SupportTicketApi.MigrationService":::* project as a service in the .NET Aspire app host.

    > [!IMPORTANT]
    > If you are using Visual Studio, and you selected the **:::no-loc text="Enlist in Aspire orchestration":::** option when creating the Worker Service project, similar code is added automatically with the service name `supportticketapi-migrationservice`. Replace that code with the preceding code.

## Remove existing seeding code

Since the migration service seeds the database, you should remove the existing data seeding code from the API project.

1. In the *:::no-loc text="SupportTicketApi.Api":::* project, open the *:::no-loc text="Program.cs":::* file.
1. Delete the highlighted lines.

    :::code source="~/aspire-docs-samples-main/SupportTicketApi/SupportTicketApi.Api/Program.cs" range="20-36" highlight="6-16" :::

## Test the migration service

Now that the migration service is configured, run the app to test the migrations.

1. Run the app and observe the :::no-loc text="SupportTicketApi"::: dashboard.
1. After a short wait, the `migrations` service state will display **Finished**.

    :::image type="content" source="media/ef-core-migrations/dashboard-post-migration.png" lightbox="media/ef-core-migrations/dashboard-post-migration.png" alt-text="A screenshot of the .NET Aspire dashboard with the migration service in a Finished state." :::

1. Select the **:::no-loc text="View":::** link on the migration service to investigate the logs showing the SQL commands that were executed.

## Get the code

You can find the [completed sample app on GitHub](https://github.com/MicrosoftDocs/aspire-docs-samples/tree/solution/SupportTicketApi).

## More sample code

The [Aspire Shop](/samples/dotnet/aspire-samples/aspire-shop/) sample app uses this approach to apply migrations. See the `AspireShop.CatalogDbManager` project for the migration service implementation.
