---
title: Seed data in a database using .NET Aspire
description: Learn about how to seed database data in .NET Aspire
ms.date: 05/14/2025
ms.topic: how-to
---

# Seed data in a database using .NET Aspire

In this article, you learn how to configure .NET Aspire projects to seed data in a database during app startup. .NET Aspire enables you to seed data using database scripts or Entity Framework Core (EF Core) for common platforms such as SQL Server, PostgreSQL, and MySQL.

## When to seed data

Seeding data pre-populates database tables with rows of data so they're ready for testing your app. You may want to seed data for the following scenarios:

- You want to develop and test different features of your app manually against a meaningful set of data, such as a product catalog or a list of customers.
- You want to run test suites to verify that features behave correctly with a given set of data.

Manually seeding data is tedious and time consuming, so you should automate the process whenever possible. You can seed your database either by running database scripts for .NET Aspire projects during startup or by using tools like EF Core, which handles many underlying concerns for you.

## Understand containerized databases

By default, .NET Aspire database integrations rely on containerized databases, which create the following challenges when trying to seed data:

- By default, .NET Aspire destroys and recreates containers every time the app restarts, which means you have to re-seed your database on each run.
- Depending on your selected database technology, the new container instance may or may not create a default database, which means you might also have to create the database itself.
- Even if a default database exists, it most likely won't have the desired name or schema for your specific app.

.NET Aspire enables you to resolve these challenges using volumes, bind mounts, and a few configurations to seed data effectively.

> [!TIP]
> Container hosts like Docker and Podman support volumes and bind mounts, both of which provide locations for data that persist when a container restarts. Volumes are the recommended solution, because they offer better performance, portability, and security. The container host creates and remains in control of volumes. Each volume can store data for multiple containers. Bind mounts have relatively limited functionality in comparison but enable you to access the data from the host machine.

> [!NOTE]
> Visit the [Database Container Sample App](https://github.com/dotnet/aspire-samples/blob/main/samples/DatabaseContainers/DatabaseContainers.AppHost/Program.cs) to view the full project and file structure for each database option.

## Seed data using SQL scripts

In .NET Aspire 9.2, the recommended method for executing database seeding scripts depends on the database server you use:

### [SQL Server](#tab/sql-server)

In .NET Aspire 9.2 and later versions, you can use the <xref:Aspire.Hosting.SqlServerBuilderExtensions.WithCreationScript*> method to ensure a T-SQL script is run when the database is created. Add SQL code to this script that creates and populates the database, the necessary tables, and other database objects.

The following code is an example T-SQL script that creates and populates an address book database:

:::code source="~/aspire-samples/samples/DatabaseContainers/DatabaseContainers.ApiService/data/sqlserver/init.sql" :::

You must ensure that this script is copied to the app host's output directory, so that .NET Aspire can execute it. Add the following XML to your *.csproj* file:

:::code language="xml" source="~/aspire-samples/samples/DatabaseContainers/DatabaseContainers.AppHost/DatabaseContainers.AppHost.csproj" range="13-17" :::

Adjust the `Include` parameter to match the path to your SQL script in the project.

Next, in the app host's *Program.cs* file, create the database and run the creation script:

:::code source="~/aspire-samples/samples/DatabaseContainers/DatabaseContainers.AppHost/Program.cs" range="40-49" :::

This code:

- Create a SQL Server container by calling `builder.AddSqlServer()`.
- Ensures that data is persisted across debugging sessions by calling `WithDataVolume()` and `WithLifetime(ContainerLifetime.Persistent)`.
- Obtains the path to the T-SQL script in the output folder.
- Calls `WithCreationScript()` to create and seed the database.

### [PostgreSQL](#tab/postgresql)

In .NET Aspire 9.2, the `WithCreationScript()` method isn't supported for the PostgreSQL integration. Instead, you must use a bind mount and deploy the setup SQL script to it, so that the data is seeded when the container initializes the database.

The following code is an example PostgreSQL script that creates and populates a to do list database:

:::code source="~/aspire-samples/samples/DatabaseContainers/DatabaseContainers.ApiService/data/postgres/init.sql" :::

In the app host's *Program.cs* file, create the database and mount the folder that contains the SQL script as a bind mount:

:::code source="~/aspire-samples/samples/DatabaseContainers/DatabaseContainers.AppHost/Program.cs" range="3-19" :::

### [MySQL](#tab/mysql)

In .NET Aspire 9.2, the `WithCreationScript()` method isn't supported for the MySQL integration. Instead, you must use a bind mount and deploy the setup SQL script to it, so that the data is seeded when the container initializes the database.

The following code is an example MySQL script that creates and populates a product catalog database:

:::code source="~/aspire-samples/samples/DatabaseContainers/DatabaseContainers.ApiService/data/mysql/init.sql" :::

In the app host's *Program.cs* file, create the database and mount the folder that contains the SQL script as a bind mount:

:::code source="~/aspire-samples/samples/DatabaseContainers/DatabaseContainers.AppHost/Program.cs" range="21-36" :::

---

## Seed data using EF Core

You can also seed data in .NET Aspire projects using EF Core by explicitly running migrations during startup. EF Core handles underlying database connections and schema creation for you, which eliminates the need to use volumes or run SQL scripts during container startup.

> [!IMPORTANT]
> These types of configurations should only be done during development, so make sure to add a conditional that checks your current environment context.

Add the following code to the _:::no-loc text="Program.cs":::_ file of your **API Service** project.

### [SQL Server](#tab/sql-server)

```csharp
// Register DbContext class
builder.AddSqlServerDbContext<TicketContext>("sqldata");

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    // Retrieve an instance of the DbContext class and manually run migrations during startup
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<TicketContext>();
        context.Database.Migrate();
    }
}
```

### [PostgreSQL](#tab/postgresql)

```csharp
// Register DbContext class
builder.AddNpgsqlDbContext<TicketContext>("sqldata");

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    // Retrieve an instance of the DbContext class and manually run migrations during startup
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<TicketContext>();
        context.Database.Migrate();
    }
}
```

### [MySQL](#tab/mysql)

```csharp
// Register DbContext class
builder.AddMySqlDataSource<TicketContext>("sqldata");

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    // Retrieve an instance of the DbContext class and manually run migrations during startup
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<TicketContext>();
        context.Database.Migrate();
    }
}
```

---

## Next steps

Database seeding is useful in a variety of app development scenarios. Try combining these techniques with the resource implementations demonstrated in the following tutorials:

- [Tutorial: Connect an ASP.NET Core app to SQL Server using .NET Aspire and Entity Framework Core](sql-server-integrations.md)
- [Tutorial: Apply Entity Framework Core migrations in .NET Aspire](ef-core-migrations.md)
- [Tutorial: Connect an ASP.NET Core app to .NET Aspire storage integrations](../storage/azure-storage-integrations.md)
- [.NET Aspire orchestration overview](../fundamentals/app-host-overview.md)
