---
title: Seed data in a database using .NET Aspire
description: Learn about how to seed database data in .NET Aspire
ms.date: 04/10/2024
ms.topic: how-to
---

# Seed data in a database using .NET Aspire

In this article, you learn how to configure .NET Aspire apps to seed data in a database during app startup. .NET Aspire enables you to seed data using database scripts or Entity Framework Core for common platforms such as SQL Server, PostgreSQL and MySQL.

## When to seed data

Seeding data pre-populates database tables with rows of data so they're ready for testing via your app. You may want to seed data for the following scenarios:

- Manually develop and test different features of your app against a meaningful set of data, such as a product catalog or list of customers.
- Run test suites to verify that features behave a specific way with a given set of data.

Manually seeding data is tedious and time consuming, so you should automate the process when possible. Use volumes to run database scripts for .NET Aspire apps during startup. You can also seed your database using tools like Entity Framework Core, which handles many underlying concerns for you.

## Understand containerized databases

By default, .NET Aspire database components rely on containerized databases, which create the following challenges when trying to seed data:

- .NET Aspire destroys and recreates containers every time the app restarts, which means by default you have to re-seed your database every time the app restarts.
- Depending on your selected database technology, the new container instance may or may not create a default database, which means you might also have to create the database itself.
- Even if a default database exists, it most likely will not have the desired name or schema for your specific app.

.NET Aspire enables you to resolve these challenges using volumes and a few configurations to seed data effectively.

## Seed data using volumes and SQL scripts

Volumes are the recommended way to automatically seed containerized databases when using SQL scripts. Volumes can store data for multiple containers at a time, offer high performance and are easy to back up or migrate. With .NET Aspire, you configure a volume for each resource container using the <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithBindMount%2A?displayProperty=nameWithType> method, which accepts three parameters:

- **Source**: The source path of the volume mount, which is the physical location on your host.
- **Target**: The target path in the container of the data you want to persist.

Consider the following volume configuration code from a _Program.cs_ file in a sample **AppHost** project:

```csharp
var todosDbName = "Todos";
var todosDb = builder.AddPostgres("postgres")
    .WithEnvironment("POSTGRES_DB", todosDbName)
    .WithBindMount(
        "../DatabaseContainers.ApiService/data/postgres",
        "/docker-entrypoint-initdb.d")
    .AddDatabase(todosDbName);
```

In this example, the `.WithBindMount` method parameters configure the following:

- `../DatabaseContainers.ApiService/data/postgres` sets a path to the SQL script in your local project that you want to run in the container to seed data.
- `/docker-entrypoint-initdb.d` sets the path to an entry point in the container so your script will be run during container startup.

The referenced SQL script located at `../DatabaseContainers.ApiService/data/postgres` creates and seeds a `Todos` table:

```sql
-- Postgres init script

-- Create the Todos table
CREATE TABLE IF NOT EXISTS Todos
(
    Id SERIAL PRIMARY KEY,
    Title text UNIQUE NOT NULL,
    IsComplete boolean NOT NULL DEFAULT false
);

-- Insert some sample data into the Todos table
INSERT INTO Todos (Title, IsComplete)
VALUES
    ('Give the dog a bath', false),
    ('Wash the dishes', false),
    ('Do the groceries', false)
ON CONFLICT DO NOTHING;
```

The script runs during startup every time a new container instance is created.

## Database seeding examples

The following examples demonstrate how to seed data using SQL scripts and configurations applied using the `.WithBindMount` method for different database technologies:

### [SQL Server](#tab/sql-server)

Configuration code in the **.AppHost** project:

:::code source="~/aspire-samples/samples/DatabaseContainers/DatabaseContainers.AppHost/Program.cs" range="25-35" :::

Corresponding SQL script included in the app:

:::code source="~/aspire-samples/samples/DatabaseContainers/DatabaseContainers.ApiService/data/sqlserver/init.sql" :::

### [PostgreSQL](#tab/postgresql)

Configuration code in the **.AppHost** project:

:::code source="~/aspire-samples/samples/DatabaseContainers/DatabaseContainers.AppHost/Program.cs" range="3-12" :::

Corresponding SQL script included in the app:

:::code source="~/aspire-samples/samples/DatabaseContainers/DatabaseContainers.ApiService/data/postgres/init.sql" :::

### [MySQL](#tab/mysql)

Configuration code in the **.AppHost** project:

:::code source="~/aspire-samples/samples/DatabaseContainers/DatabaseContainers.AppHost/Program.cs" range="14-23" :::

Corresponding SQL script included in the app:

:::code source="~/aspire-samples/samples/DatabaseContainers/DatabaseContainers.ApiService/data/mysql/init.sql" :::

---

## Seed data using Entity Framework Core

You can also seed data in .NET Aspire apps using Entity Framework Core by explicitly running migrations during startup. Entity Framework Core handles underlying database connections and schema creation for you, which eliminates the need to use volumes or run SQL scripts during container startup.

> [!IMPORTANT]
> These types of configurations should only be done during development, so make sure to add a conditional that checks your current environment context.

Add the following code to the _Program.cs_ file of your **.AppHost** project.

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
        context.Database.EnsureCreated();
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
        context.Database.EnsureCreated();
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
        context.Database.EnsureCreated();
    }
}
```

---

## Next steps

Database seeding is useful in a variety of app development scenarios. Try combining these techniques with the resource implementations demonstrated in the following tutorials:

- [Tutorial: Connect an ASP.NET Core app to SQL Server using .NET Aspire and Entity Framework Core](../database/sql-server-components.md)
- [Tutorial: Connect an ASP.NET Core app to .NET Aspire storage components](../storage/azure-storage-components.md)
- [.NET Aspire orchestration overview](../fundamentals/app-host-overview.md)
