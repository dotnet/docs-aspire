---
title: Tutorial - PostgreSQL migrations with Entity Framework Core in .NET Aspire
description: Learn how to create, apply, and manage PostgreSQL migrations using Entity Framework Core in .NET Aspire applications.
ms.date: 02/07/2025
ms.topic: tutorial
---

# Tutorial: PostgreSQL migrations with Entity Framework Core in .NET Aspire

In this comprehensive tutorial, you'll learn how to create a .NET Aspire application that uses PostgreSQL with Entity Framework Core migrations. You'll learn how to:

> [!div class="checklist"]
>
> - Set up a .NET Aspire project with PostgreSQL and Entity Framework Core
> - Create and configure Entity Framework Core models and contexts
> - Generate PostgreSQL database migrations
> - Create a migration service to apply migrations automatically
> - Handle common migration scenarios and troubleshooting

Entity Framework Core [migrations](/ef/core/managing-schemas/migrations) are essential for managing database schema changes over time. Unlike SQL Server examples found elsewhere, this tutorial focuses specifically on PostgreSQL integration patterns and common challenges developers face when working with PostgreSQL in containerized .NET Aspire applications.

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

## Create the sample solution

1. At the top of Visual Studio, navigate to **File** > **New** > **Project**.
1. In the dialog window, search for *Blazor* and select **Blazor Web App**. Choose **Next**.
1. On the **Configure your new project** screen:
    - Enter a **Project Name** of **AspirePostgreSQLEFCore**.
    - Leave the rest of the values at their defaults and select **Next**.
1. On the **Additional information** screen:
    - Make sure **.NET 9.0** is selected.
    - Ensure the **Interactive render mode** is set to **None**.
    - Check the **Enlist in .NET Aspire orchestration** option and select **Create**.

Visual Studio creates a new ASP.NET Core solution that is structured to use .NET Aspire. The solution consists of the following projects:

- **AspirePostgreSQLEFCore**: A Blazor project that depends on service defaults.
- **AspirePostgreSQLEFCore.AppHost**: An orchestrator project designed to connect and configure the different projects and services of your app.
- **AspirePostgreSQLEFCore.ServiceDefaults**: A shared class library to hold configurations that can be reused across the projects in your solution.

## Create the data project

To organize your Entity Framework Core models and migrations, create a separate data project:

1. Right-click the solution in Solution Explorer and select **Add** > **New Project**.
1. Search for *Class Library* and select **Class Library**. Choose **Next**.
1. On the **Configure your new project** screen:
    - Enter a **Project Name** of **AspirePostgreSQLEFCore.Data**.
    - Select **Next**.
1. On the **Additional information** screen:
    - Make sure **.NET 9.0** is selected and select **Create**.

Add the Entity Framework Core PostgreSQL packages to the data project:

```dotnetcli
dotnet add AspirePostgreSQLEFCore.Data package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add AspirePostgreSQLEFCore.Data package Microsoft.EntityFrameworkCore.Tools
```

## Create the database model and context

Create the models directory and add a `SupportTicket` model class to represent support tickets:

:::code source="snippets/postgresql-ef-core-tutorial/AspirePostgreSQLEFCore.Data/Models/SupportTicket.cs":::

Create the data context class that inherits from <xref:Microsoft.EntityFrameworkCore.DbContext>:

:::code source="snippets/postgresql-ef-core-tutorial/AspirePostgreSQLEFCore.Data/TicketContext.cs":::

## Add Entity Framework Core to the web application

Add the [.NET Aspire PostgreSQL Entity Framework Core integration](postgresql-entity-framework-integration.md) to your **AspirePostgreSQLEFCore** project:

```dotnetcli
dotnet add AspirePostgreSQLEFCore package Aspire.Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add AspirePostgreSQLEFCore reference AspirePostgreSQLEFCore.Data
```

In the **Program.cs** file of the **AspirePostgreSQLEFCore** project, add a call to the <xref:Microsoft.Extensions.Hosting.AspireEFPostgreSqlExtensions.AddNpgsqlDbContext%2A> extension method:

:::code source="snippets/postgresql-ef-core-tutorial/AspirePostgreSQLEFCore/Program.cs" range="1-10" highlight="5":::

This method accomplishes the following tasks:

- Registers a `TicketContext` with the DI container for connecting to the PostgreSQL database.
- Automatically enables corresponding health checks, logging, and telemetry.

## Configure the app host

The **AspirePostgreSQLEFCore.AppHost** project orchestrates your application. Add the [.NET Aspire PostgreSQL hosting integration](postgresql-integration.md#hosting-integration) to your app host project:

```dotnetcli
dotnet add AspirePostgreSQLEFCore.AppHost package Aspire.Hosting.PostgreSQL
```

Update the **Program.cs** file in the **AspirePostgreSQLEFCore.AppHost** project:

:::code source="snippets/postgresql-ef-core-tutorial/AspirePostgreSQLEFCore.AppHost/Program.cs":::

The preceding code:

- Adds a PostgreSQL server resource with persistent data storage
- Creates a database named `ticketdb`
- Configures the web application to reference the database

## Create Entity Framework Core migrations

Now you'll create migrations to define your database schema. This is where many developers encounter issues with PostgreSQL and .NET Aspire.

### Configure temporary connection string

Because .NET Aspire uses service discovery that's only available at runtime, you need a temporary connection string for the EF Core tools to work. In the **AspirePostgreSQLEFCore** project, create an `appsettings.json` file (if it doesn't exist) with the following content:

:::code source="snippets/postgresql-ef-core-tutorial/AspirePostgreSQLEFCore/appsettings.json":::

> [!IMPORTANT]
> This connection string is only used by the EF Core command-line tools. The actual runtime connection string will be provided by .NET Aspire service discovery.

### Create the initial migration

From the **AspirePostgreSQLEFCore** project directory, run the following command to create your initial migration:

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet ef migrations add InitialCreate --project ..\AspirePostgreSQLEFCore.Data\AspirePostgreSQLEFCore.Data.csproj
```

### [Package Manager Console](#tab/package-manager-console)

If using Visual Studio's Package Manager Console:

1. Open **Tools** > **NuGet Package Manager** > **Package Manager Console**.
1. Set the **Default project** to **AspirePostgreSQLEFCore.Data**.
1. Set the **Startup project** to **AspirePostgreSQLEFCore**.
1. Run the migration command:

```powershell
Add-Migration InitialCreate
```

---

This creates a migration in the **AspirePostgreSQLEFCore.Data/Migrations** folder that defines the initial database schema.

### Add additional model properties

Let's add more fields to demonstrate additional migrations. Update the `SupportTicket` model:

:::code source="snippets/postgresql-ef-core-tutorial/AspirePostgreSQLEFCore.Data/Models/SupportTicket.cs" range="1-25" highlight="20-23":::

Create another migration to capture these changes:

### [.NET CLI](#tab/dotnet-cli-2)

```dotnetcli
dotnet ef migrations add AddTicketStatusAndDates --project ..\AspirePostgreSQLEFCore.Data\AspirePostgreSQLEFCore.Data.csproj
```

### [Package Manager Console](#tab/package-manager-console-2)

```powershell
Add-Migration AddTicketStatusAndDates
```

---

### Clean up temporary configuration

Remove the temporary connection string from `appsettings.json` since .NET Aspire will provide the connection string at runtime:

:::code source="snippets/postgresql-ef-core-tutorial/AspirePostgreSQLEFCore/appsettings-clean.json":::

## Create the migration service

To apply migrations automatically when the application starts, create a dedicated migration service:

1. Add a new Worker Service project to the solution:

```dotnetcli
dotnet new worker -n AspirePostgreSQLEFCore.MigrationService -f "net9.0"
dotnet sln add AspirePostgreSQLEFCore.MigrationService
```

2. Add the necessary project references and packages:

```dotnetcli
dotnet add AspirePostgreSQLEFCore.MigrationService reference AspirePostgreSQLEFCore.Data
dotnet add AspirePostgreSQLEFCore.MigrationService reference AspirePostgreSQLEFCore.ServiceDefaults
dotnet add AspirePostgreSQLEFCore.MigrationService package Aspire.Npgsql.EntityFrameworkCore.PostgreSQL
```

3. Update the migration service **Program.cs**:

:::code source="snippets/postgresql-ef-core-tutorial/AspirePostgreSQLEFCore.MigrationService/Program.cs":::

4. Replace the **Worker.cs** file content:

:::code source="snippets/postgresql-ef-core-tutorial/AspirePostgreSQLEFCore.MigrationService/Worker.cs":::

## Update the app host to use the migration service

Update the app host **Program.cs** to include the migration service and ensure proper startup order:

:::code source="snippets/postgresql-ef-core-tutorial/AspirePostgreSQLEFCore.AppHost/Program.cs" highlight="8-11,13":::

Add the migration service project reference to the app host:

```dotnetcli
dotnet add AspirePostgreSQLEFCore.AppHost reference AspirePostgreSQLEFCore.MigrationService
```

## Create the user interface

Create a simple form to test the database integration. Replace the contents of **Components/Pages/Home.razor**:

:::code source="snippets/postgresql-ef-core-tutorial/AspirePostgreSQLEFCore/Components/Pages/Home.razor":::

## Test the application

1. Run the application by pressing **F5** in Visual Studio or using:

```dotnetcli
dotnet run --project AspirePostgreSQLEFCore.AppHost
```

2. In the .NET Aspire dashboard, wait for all services to start:
   - The **postgres** container should show as **Running**
   - The **migration** service should show as **Finished** 
   - The web application should show as **Running**

3. Click the web application endpoint to open the application.

4. Fill out the support ticket form and submit it.

5. Verify that the data appears in the table below the form.

## Verify data persistence

Stop and restart the application to verify that data persists between runs:

1. Stop debugging (**Shift + F5**).
1. Start debugging again (**F5**).
1. Navigate to the web application.
1. Verify that previously submitted tickets are still displayed.

This works because the app host uses `WithDataVolume()` on the PostgreSQL resource, which persists data between container restarts.

## Troubleshooting common issues

### "No database provider has been configured" error

If you encounter this error when creating migrations, ensure you have a temporary connection string in `appsettings.json` as shown in the [Create Entity Framework Core migrations](#create-entity-framework-core-migrations) section.

### Migrations not applying

If migrations aren't applying, check the following:

1. Verify the migration service is referenced in the app host
2. Check that the `WaitFor(migrations)` dependency is properly configured
3. Review the migration service logs in the .NET Aspire dashboard

### PostgreSQL connection issues

If you can't connect to PostgreSQL:

1. Ensure the PostgreSQL container is running in the dashboard
2. Verify the connection string name matches between the app host and client configuration
3. Check that the database name is consistent across your configuration

### Different behavior than SQL Server

PostgreSQL has some differences from SQL Server that may affect your migrations:

- Case sensitivity: PostgreSQL is case-sensitive by default
- Data types: PostgreSQL has different data type mappings
- Naming conventions: PostgreSQL typically uses snake_case for table and column names

To handle these differences, you can configure EF Core conventions in your `TicketContext`:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Configure PostgreSQL naming conventions
    foreach (var entity in modelBuilder.Model.GetEntityTypes())
    {
        entity.SetTableName(entity.GetTableName()?.ToSnakeCase());
        
        foreach (var property in entity.GetProperties())
        {
            property.SetColumnName(property.GetColumnName()?.ToSnakeCase());
        }
    }
}
```

## Next steps

Now that you have a working PostgreSQL + Entity Framework Core + Migrations setup, you can:

- Add more complex entity relationships
- Implement data seeding in your migration service
- Add migration rollback capabilities
- Set up automated migration deployment pipelines
- Explore PostgreSQL-specific features like JSON columns

## See also

- [.NET Aspire PostgreSQL integration](postgresql-integration.md)
- [.NET Aspire PostgreSQL Entity Framework Core integration](postgresql-entity-framework-integration.md)
- [Apply Entity Framework Core migrations in .NET Aspire](ef-core-migrations.md)
- [Entity Framework Core documentation](/ef/core/)
- [PostgreSQL documentation](https://www.postgresql.org/docs/)