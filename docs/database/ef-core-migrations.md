---
title: Apply EF Core migrations in .NET Aspire
description: Learn about how to to apply Entity Framework Core migrations in .NET Aspire
ms.date: 07/31/2024
ms.topic: how-to
uid: database/ef-core-migrations
ms.custom: sfi-image-nochange
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
- **SupportTicketApi.AppHost**: Contains the .NET Aspire AppHost and configuration.
- **SupportTicketApi.Data**: Contains the EF Core contexts and models.
- **SupportTicketApi.ServiceDefaults**: Contains the default service configurations.

Run the app to ensure it works as expected. In the .NET Aspire dashboard, wait until all resources are running and healthy. Then select the **https** Swagger endpoint and test the API's **GET /api/SupportTickets** endpoint by expanding the operation and selecting **Try it out**. Select **Execute** to send the request and view the response:

```json
[
  {
    "id": 1,
    "title": "Initial Ticket",
    "description": "Test ticket, please ignore."
  }
]
```

Close the browser tabs that display the Swagger endpoint and the .NET Aspire dashboard and then stop debugging.

## Create migrations

Start by creating some migrations to apply.

### [.NET CLI](#tab/dotnet-cli)

1. Open a terminal (<kbd>Ctrl</kbd>+<kbd>\`</kbd> in Visual Studio).
1. Set *:::no-loc text="SupportTicketApi\\SupportTicketApi.Api":::* as the current directory.
1. Use the [`dotnet ef` command-line tool](/ef/core/managing-schemas/migrations/#install-the-tools) to create a new migration to capture the initial state of the database schema:

    ```dotnetcli
    dotnet ef migrations add InitialCreate --project ..\SupportTicketApi.Data\SupportTicketApi.Data.csproj
    ```

    The proceeding command:

      - Runs EF Core migration command-line tool in the *SupportTicketApi.Api* directory. `dotnet ef` is run in this location because the API service is where the DB context is used.
      - Creates a migration named *InitialCreate*.
      - Creates the migration in the in the *Migrations* folder in the *SupportTicketApi.Data* project.

### [Package Manager Console](#tab/package-manager-console)

If you prefer using Visual Studio's Package Manager Console instead of the command line:

1. Open the **Package Manager Console** in Visual Studio by selecting **Tools** > **NuGet Package Manager** > **Package Manager Console**.
1. Set the **Default project** dropdown to *SupportTicketApi.Data*.
1. Set the **Startup project** to *SupportTicketApi.Api* using the dropdown in the toolbar or by right-clicking the project in Solution Explorer and selecting **Set as Startup Project**.
1. Run the migration command:

    ```powershell
    Add-Migration InitialCreate
    ```

    > [!IMPORTANT]
    > When using Package Manager Console, ensure the startup project is set to the project that contains the DbContext registration (usually your API or web project), and the default project is set to where you want the migrations to be created (usually your data project). Remember to change the startup project back to your AppHost project when you're done, or the .NET Aspire dashboard won't start when you press <kbd>F5</kbd>.

---

1. Modify the model so that it includes a new property. Open *:::no-loc text="SupportTicketApi.Data\\Models\\SupportTicket.cs":::* and add a new property to the `SupportTicket` class:

    :::code source="~/aspire-docs-samples-solution/SupportTicketApi/SupportTicketApi.Data/Models/SupportTicket.cs" range="4-15" highlight="10" :::

1. Create another new migration to capture the changes to the model:

    ### [.NET CLI](#tab/dotnet-cli-2)

    ```dotnetcli
    dotnet ef migrations add AddCompleted --project ..\SupportTicketApi.Data\SupportTicketApi.Data.csproj
    ```

    ### [Package Manager Console](#tab/package-manager-console-2)

    ```powershell
    Add-Migration AddCompleted
    ```

    ---

Now you've got some migrations to apply. Next, you'll create a migration service that applies these migrations during app startup.

## Troubleshoot migration issues

When working with EF Core migrations in .NET Aspire projects, you might encounter some common issues. Here are solutions to the most frequent problems:

### "No database provider has been configured" error

If you get an error like "No database provider has been configured for this DbContext" when running migration commands, it's because the EF tools can't find a connection string or database provider configuration. This happens because .NET Aspire projects use service discovery and orchestration that's only available at runtime.

**Solution**: Temporarily add a connection string to your project's `appsettings.json` file:

1. In your API project (where the DbContext is registered), open or create an `appsettings.json` file.
1. Add a connection string with the same name used in your .NET Aspire AppHost:

    ```json
    {
      "ConnectionStrings": {
        "ticketdb": "Server=(localdb)\\mssqllocaldb;Database=TicketDb;Trusted_Connection=true"
      }
    }
    ```

1. Run your migration commands as normal.
1. Remove the connection string from `appsettings.json` when you're done, as .NET Aspire will provide it at runtime.

> [!TIP]
> The connection string name must match what you use in your AppHost. For example, if you use `builder.AddProject<Projects.SupportTicketApi_Api>().WithReference(sqlServer.AddDatabase("ticketdb"))`, then use "ticketdb" as the connection string name.

### Multiple databases in one solution

When your .NET Aspire solution has multiple services with different databases, create migrations for each database separately:

1. Navigate to each service project directory that has a DbContext.
1. Run migration commands with the appropriate project reference:

    ```dotnetcli
    # For the first service/database
    dotnet ef migrations add InitialCreate --project ..\FirstService.Data\FirstService.Data.csproj

    # For the second service/database  
    dotnet ef migrations add InitialCreate --project ..\SecondService.Data\SecondService.Data.csproj
    ```

1. Create separate migration services for each database, or handle multiple DbContexts in a single migration service.

### Startup project configuration

Ensure you're running migration commands from the correct project:

- **CLI**: Navigate to the project directory that contains the DbContext registration (usually your API project)
- **Package Manager Console**: Set the startup project to the one that configures the DbContext, and the default project to where migrations should be created

## Create the migration service

To execute migrations, call the EF Core <xref:Microsoft.EntityFrameworkCore.Migrations.IMigrator.Migrate*> method or the <xref:Microsoft.EntityFrameworkCore.Migrations.IMigrator.MigrateAsync*> method. In this tutorial, you'll create a separate worker service to apply migrations. This approach separates migration concerns into a dedicated project, which is easier to maintain and allows migrations to run before other services start.

> [!NOTE]
> **Where to create migrations**: Migrations should be created in the project that contains your Entity Framework DbContext and model classes (in this example, *SupportTicketApi.Data*). The migration service project references this data project to apply the migrations at startup.

To create a service that applies the migrations:

1. Add a new Worker Service project to the solution. If using Visual Studio, right-click the solution in Solution Explorer and select **:::no-loc text="Add":::** > **:::no-loc text="New Project":::**. Select **:::no-loc text="Worker Service":::**, name the project *:::no-loc text="SupportTicketApi.MigrationService":::* and target **.NET 8.0**. If using the command line, use the following commands from the solution directory:

    ```dotnetcli
    dotnet new worker -n SupportTicketApi.MigrationService -f "net8.0"
    dotnet sln add SupportTicketApi.MigrationService
    ```

1. Add the *:::no-loc text="SupportTicketApi.Data":::* and *:::no-loc text="SupportTicketApi.ServiceDefaults":::* project references to the *:::no-loc text="SupportTicketApi.MigrationService":::* project using Visual Studio or the command line:

    ```dotnetcli
    dotnet add SupportTicketApi.MigrationService reference SupportTicketApi.Data
    dotnet add SupportTicketApi.MigrationService reference SupportTicketApi.ServiceDefaults
    ```

1. Add the [📦 Aspire.Microsoft.EntityFrameworkCore.SqlServer](https://www.nuget.org/packages/Aspire.Microsoft.EntityFrameworkCore.SqlServer) NuGet package reference to the *:::no-loc text="SupportTicketApi.MigrationService":::* project using Visual Studio or the command line:

    ```dotnetcli
    cd SupportTicketApi.MigrationService
    dotnet add package Aspire.Microsoft.EntityFrameworkCore.SqlServer -v "9.4.0"
    ```

    > [!TIP]
    > In some cases, you might also need to add the [📦 Microsoft.EntityFrameworkCore.Tools](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Tools) package to prevent EF Core from failing silently without applying migrations. This is particularly relevant when using databases other than SQL Server, such as PostgreSQL. For more information, see [dotnet/efcore#27215](https://github.com/dotnet/efcore/issues/27215#issuecomment-2045767772).

1. Add the highlighted lines to the *:::no-loc text="Program.cs":::* file in the *:::no-loc text="SupportTicketApi.MigrationService":::* project:

    :::code source="~/aspire-docs-samples-solution/SupportTicketApi/SupportTicketApi.MigrationService/Program.cs" highlight="1,7,9-11" :::

    In the preceding code:

    - The `AddServiceDefaults` extension method [adds service defaults functionality](/dotnet/aspire/fundamentals/service-defaults#add-service-defaults-functionality).
    - The `AddOpenTelemetry` extension method [configures OpenTelemetry functionality](/dotnet/aspire/fundamentals/telemetry#net-aspire-opentelemetry-integration).
    - The `AddSqlServerDbContext` extension method adds the `TicketContext` service to the service collection. This service is used to run migrations and seed the database.

1. Replace the contents of the *:::no-loc text="Worker.cs":::* file in the *:::no-loc text="SupportTicketApi.MigrationService":::* project with the following code:

    :::code source="~/aspire-docs-samples-solution/SupportTicketApi/SupportTicketApi.MigrationService/Worker.cs" :::

    In the preceding code:

    - The `ExecuteAsync` method is called when the worker starts. It in turn performs the following steps:
      1. Gets a reference to the `TicketContext` service from the service provider.
      1. Calls `RunMigrationAsync` to apply any pending migrations.
      1. Calls `SeedDataAsync` to seed the database with initial data.
      1. Stops the worker with `StopApplication`.
    - The `RunMigrationAsync` and `SeedDataAsync` methods both encapsulate their respective database operations using execution strategies to handle transient errors that may occur when interacting with the database. To learn more about execution strategies, see [Connection Resiliency](/ef/core/miscellaneous/connection-resiliency).

## Add the migration service to the orchestrator

The migration service is created, but it needs to be added to the .NET Aspire AppHost so that it runs when the app starts.

1. In the *:::no-loc text="SupportTicketApi.AppHost":::* project, open the *:::no-loc text="Program.cs":::* file.
1. Add the following highlighted code:

    :::code source="~/aspire-docs-samples-solution/SupportTicketApi/SupportTicketApi.AppHost/Program.cs" highlight="7-9, 13-14" :::

    This code enlists the *:::no-loc text="SupportTicketApi.MigrationService":::* project as a service in the .NET Aspire AppHost. It also ensures that the API resource doesn't run until the migrations are complete.

    > [!NOTE]
    > In the preceding code, the call to <xref:Aspire.Hosting.SqlServerBuilderExtensions.AddDatabase*> adds a representation of a SQL Server database to the .NET Aspire application model with a connection string. It *doesn't* create a database in the SQL Server container. To ensure that the database is created, the sample project calls the EF Core <xref:Microsoft.EntityFrameworkCore.Storage.IDatabaseCreator.EnsureCreated*> method from the support ticket API's *:::no-loc text="Program.cs":::* file.

    > [!TIP]
    > The code creates the SQL Server container each time it runs and applies migrations to it. Data doesn't persist across debugging sessions and any new database rows you create during testing will not survive an app restart. If you would prefer to persist this data, add a data volume to your container. For more information, see [Add SQL Server resource with data volume](sql-server-entity-framework-integration.md#add-sql-server-resource-with-data-volume).

1. If the code cannot resolve the migration service project, add a reference to the migration service project in the AppHost project:

    ```dotnetcli
    dotnet add SupportTicketApi.AppHost reference SupportTicketApi.MigrationService
    ```

    > [!IMPORTANT]
    > If you are using Visual Studio, and you selected the **:::no-loc text="Enlist in Aspire orchestration":::** option when creating the Worker Service project, similar code is added automatically with the service name `supportticketapi-migrationservice`. Replace that code with the preceding code.

## Multiple databases scenario

If your .NET Aspire solution uses multiple databases, you have two options for managing migrations:

### Option 1: Separate migration services (Recommended)

Create a dedicated migration service for each database. This approach provides better isolation and makes it easier to manage different database schemas independently.

1. Create separate migration service projects for each database:

    ```dotnetcli
    dotnet new worker -n FirstService.MigrationService -f "net8.0"
    dotnet new worker -n SecondService.MigrationService -f "net8.0"
    ```

1. Configure each migration service to handle its specific database context.
1. Add both migration services to your AppHost:

    ```csharp
    var firstDb = sqlServer.AddDatabase("firstdb");
    var secondDb = postgres.AddDatabase("seconddb");

    var firstMigrations = builder.AddProject<Projects.FirstService_MigrationService>()
        .WithReference(firstDb);

    var secondMigrations = builder.AddProject<Projects.SecondService_MigrationService>()
        .WithReference(secondDb);

    // Ensure services wait for their respective migrations
    builder.AddProject<Projects.FirstService_Api>()
        .WithReference(firstDb)
        .WaitFor(firstMigrations);

    builder.AddProject<Projects.SecondService_Api>()
        .WithReference(secondDb)
        .WaitFor(secondMigrations);
    ```

### Option 2: Single migration service with multiple contexts

Alternatively, you can create one migration service that handles multiple database contexts:

1. Add references to all data projects in the migration service.
1. Register all DbContexts in the migration service's `Program.cs`.
1. Modify the `Worker.cs` to apply migrations for each context:

    ```csharp
    public async Task<bool> RunMigrationAsync(IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        
        var firstContext = scope.ServiceProvider.GetRequiredService<FirstDbContext>();
        var secondContext = scope.ServiceProvider.GetRequiredService<SecondDbContext>();
        
        await firstContext.Database.MigrateAsync();
        await secondContext.Database.MigrateAsync();
        
        return true;
    }
    ```

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

1. Select the **:::no-loc text="Console logs":::** icon on the migration service to investigate the logs showing the SQL commands that were executed.

## Get the code

You can find the [completed sample app on GitHub](https://github.com/MicrosoftDocs/aspire-docs-samples/tree/solution/SupportTicketApi).

## More sample code

The [Aspire Shop](/samples/dotnet/aspire-samples/aspire-shop/) sample app uses this approach to apply migrations. See the `AspireShop.CatalogDbManager` project for the migration service implementation.
