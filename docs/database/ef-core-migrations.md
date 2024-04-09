---
title: Apply EF Core migrations in .NET Aspire
description: Learn about how to to apply Entity Framework Core migrations in .NET Aspire
ms.date: 3/20/2024
ms.topic: how-to
---

# Apply EF Core migrations in .NET Aspire

In this article, you learn how to configure .NET Aspire apps to seed data in a database during app startup. .NET Aspire enables you to seed data using database scripts or Entity Framework Core for common platforms such as SQL Server, PostgreSQL and MySQL.

## When to run migrations

You should run migrations whenever you make changes to your database schema. This includes creating new tables, modifying existing tables, or adding/removing columns. By running migrations, you ensure that your database schema is in sync with your application code.

# Database migrations with Entity Framework Core sample app

This sample demonstrates how to use Entity Framework Core's [migrations feature](https://learn.microsoft.com/ef/core/managing-schemas/migrations) with Aspire.

The sample has three important projects:

- `DatabaseMigrations.ApiService` - A web app that calls the database.
- `DatabaseMigrations.MigrationService` - A background worker app that applies migrations when it starts up.
- `DatabaseMigrations.ApiModel` - The EF Core context, entities, and migrations. This project is used by the API and migration service.

`DatabaseMigrations.ApiService` and `DatabaseMigrations.MigrationService` reference a SQL Server resource. During local development the SQL Server resource is launched in a container.

## Demonstrates

- How to create migrations in an Aspire solution
- How to apply migrations in an Aspire solution

## Sample prerequisites

This sample is written in C# and targets .NET 8.0. It requires the [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later.

## Create migration

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
