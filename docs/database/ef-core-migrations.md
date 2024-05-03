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

1. Open a terminal (<kbd>Ctrl</kbd>+<kbd>\`</kbd> in Visual Studio) and set *SupportTicketApi\SupportTicketApi.Api* as the current directory. 
1. Use the [`dotnet ef` command-line tool](https://learn.microsoft.com/ef/core/managing-schemas/migrations/#install-the-tools) to create a new migration to capture the initial state of the database schema:

    ```dotnetcli
    dotnet ef migrations add InitialCreate --project ..\SupportTicketApi.Data\SupportTicketApi.Data.csproj
    ```

    The proceeding command:

      - Runs EF Core migration command-line tool in the *SupportTicketApi.Api* directory. `dotnet ef` is run in this location because the API service is where the DB context is used.
      - Creates a migration named *InitialCreate*.
      - Creates the migration in the in the *Migrations* folder in the *SupportTicketApi.Data* project.

1. Modify the model so that it includes a new property. Open *SupportTicketApi.Data\Models\SupportTicket.cs* and add a new property to the `SupportTicket` class:

    :::code source="snippets/tutorial/SupportTicketApi/SupportTicketApi.Data/Models/SupportTicket.cs" range="5-13" highlight="12" 

1. Create another migration to apply the changes:

    ```dotnetcli
    dotnet ef migrations add AddClosedStatus --project ..\SupportTicketApi.Data\SupportTicketApi.Data.csproj
    ```

## Apply migrations during app startup

Now that you have migrations to apply, you can configure the app to run them during startup. 

1. Add a new Worker Service project to the solution named *SupportTicketApi.MigrationService*. This project will contain a background service that runs migrations during app startup.

    :::image type="content" source="media/ef-core-migrations/new-worker-service.png" lightbox="media/ef-core-migrations/new-worker-service.png" alt-text="A screenshot of the new project dialog in Visual Studio for a new Worker Service project.":::

The `DatabaseMigrations.ApiModel` project contains the EF Core model and migrations. The [`dotnet ef` command-line tool](https://learn.microsoft.com/ef/core/managing-schemas/migrations/#install-the-tools) can be used to create new migrations:

1. Update the `Entry` entity in database context in `MyDb1Context.cs`. Add a `Name` property:

    ```cs
    public class Entry
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Name { get; set; }
    }
    ```

2. Open a command prompt in the `DatabaseMigrations.ApiService` directory and run the EF Core migration tool to create a migration named **MyNewMigration**.

    ```bash
    dotnet ef migrations add MyNewMigration --project ..\DatabaseMigrations.ApiModel\DatabaseMigrations.ApiModel.csproj
    ```

    The proceeding command:

      * Runs EF Core migration command-line tool in the `DatabaseMigrations.ApiService` directory. `dotnet ef` is run in this location because the API service is where the DB context is used.
      * Creates a migration named `MyNewMigration`.
      * Creates the migration in the `DatabaseMigrations.ApiModel` project.

4. View the new migration files in the `DatabaseMigrations.ApiModel` project.

## Run the app

If using Visual Studio, open the solution file `DatabaseMigrations.sln` and launch/debug the `DatabaseMigrations.AppHost` project.

If using the .NET CLI, run `dotnet run` from the `DatabaseContainers.AppHost` directory.

When the app starts up, the `DatabaseMigrations.MigrationService` background worker runs migrations on the SQL Server container. The migration service:

* Creates a database in the SQL Server container.
* Creates the database schema.
* Stops itself once the migration is complete.



## Next steps

Database seeding is useful in a variety of app development scenarios. Try combining these techniques with the resource implementations demonstrated in the following tutorials:

- [Tutorial: Connect an ASP.NET Core app to SQL Server using .NET Aspire and Entity Framework Core](../database/sql-server-components.md)
- [Tutorial: Connect an ASP.NET Core app to .NET Aspire storage components](../storage/azure-storage-components.md)
- [.NET Aspire orchestration overview](../fundamentals/app-host-overview.md)
