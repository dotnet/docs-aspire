---
title: Seed data in a database using Aspire
description: Learn about how to seed database data in Aspire
ms.date: 05/27/2025
ms.topic: how-to
---

# Seed data in a database using Aspire

In this article, you learn how to configure Aspire projects to seed data in a database during app startup. Aspire enables you to seed data using database scripts or Entity Framework Core (EF Core) for common platforms such as SQL Server, PostgreSQL, and MySQL.

## When to seed data

Seeding data pre-populates database tables with rows of data so they're ready for testing your app. You may want to seed data for the following scenarios:

- You want to develop and test different features of your app manually against a meaningful set of data, such as a product catalog or a list of customers.
- You want to run test suites to verify that features behave correctly with a given set of data.

Manually seeding data is tedious and time consuming, so you should automate the process whenever possible. You can seed your database either by running database scripts for Aspire projects during startup or by using tools like EF Core, which handles many underlying concerns for you.

## Understand containerized databases

By default, Aspire database integrations rely on containerized databases, which create the following challenges when trying to seed data:

- By default, Aspire destroys and recreates containers every time the app restarts, which means you have to re-seed your database on each run.
- Depending on your selected database technology, the new container instance may or may not create a default database, which means you might also have to create the database itself.
- Even if a default database exists, it most likely won't have the desired name or schema for your specific app.

Aspire enables you to resolve these challenges using volumes, bind mounts, and a few configurations to seed data effectively.

> [!TIP]
> Container hosts like Docker and Podman support volumes and bind mounts, both of which provide locations for data that persist when a container restarts. Volumes are the recommended solution, because they offer better performance, portability, and security. The container host creates and remains in control of volumes. Each volume can store data for multiple containers. Bind mounts have relatively limited functionality in comparison but enable you to access the data from the host machine.

> [!NOTE]
> Visit the [Database Container Sample App](https://github.com/dotnet/aspire-samples/blob/main/samples/DatabaseContainers/DatabaseContainers.AppHost/AppHost.cs) to view the full project and file structure for each database option.

## Seed data using SQL scripts

The recommended method for executing database seeding scripts depends on the database server you use:

### [SQL Server](#tab/sql-server)

Starting with Aspire 9.2, you can use the <xref:Aspire.Hosting.SqlServerBuilderExtensions.WithCreationScript*> method to ensure a T-SQL script is run when the database is created. Add SQL code to this script that creates and populates the database, the necessary tables, and other database objects.

The following code is an example T-SQL script that creates and populates an address book database:

:::code source="~/aspire-samples/samples/DatabaseContainers/DatabaseContainers.ApiService/data/sqlserver/init.sql" :::

You must ensure that this script is copied to the AppHost's output directory, so that Aspire can execute it. Add the following XML to your *.csproj* file:

:::code language="xml" source="~/aspire-samples/samples/DatabaseContainers/DatabaseContainers.AppHost/DatabaseContainers.AppHost.csproj" range="13-17" :::

Adjust the `Include` parameter to match the path to your SQL script in the project.

Next, in the AppHost's *AppHost.cs* file, create the database and run the creation script:

:::code source="~/aspire-samples/samples/DatabaseContainers/DatabaseContainers.AppHost/AppHost.cs" range="40-49" :::

The preceding code:

- Create a SQL Server container by calling `builder.AddSqlServer()`.
- Ensures that data is persisted across debugging sessions by calling `WithDataVolume()` and `WithLifetime(ContainerLifetime.Persistent)`.
- Obtains the path to the T-SQL script in the output folder.
- Calls `WithCreationScript()` to create and seed the database.

### [PostgreSQL](#tab/postgresql)

Starting with Aspire 9.3, the `WithCreationScript()` method is supported for the PostgreSQL integration but, because there is no `USE DATABASE` in PostgreSQL, it only supports operations against the default database. For example, you can issue `CREATE DATABASE` statements to create other databases, but you can't populate them with tables and data. Instead, you must use a bind mount and deploy the setup SQL script to it, so that the data is seeded when the container initializes the database.

The following code is an example PostgreSQL script that creates and populates a to do list database:

:::code source="~/aspire-samples/samples/DatabaseContainers/DatabaseContainers.ApiService/data/postgres/init.sql" :::

In the AppHost's *AppHost.cs* file, create the database and mount the folder that contains the SQL script as a bind mount:

:::code source="~/aspire-samples/samples/DatabaseContainers/DatabaseContainers.AppHost/AppHost.cs" range="3-19" :::

### [MySQL](#tab/mysql)

Starting with Aspire 9.3, you can use the <xref:Aspire.Hosting.MySqlBuilderExtensions.WithCreationScript*> method to ensure a MySQL script is run when the database is created. Add SQL code to this script that creates and populates the database, the necessary tables, and other database objects.

In the following AppHost code, the script is created as a string and passed to the `WithCreationScript` method:

:::code source="snippets/mysql-seed-data/AppHost.cs" :::

The preceding code:

- Create a MySQL container by calling `builder.AddMySql()`.
- Uses the `MYSQL_DATABASE` environment variable to name the database `catalog`.
- Ensures that data is persisted across debugging sessions by calling `WithDataVolume()` and `WithLifetime(ContainerLifetime.Persistent)`.
- Create a second container that runs the PHP My Admin user interface for MySQL.
- Calls `WithCreationScript()` to create and seed the database.

If you run this code, you can use the PHP My Admin resource to check that a table called **catalog** has been created and populated with products.

---

## Seed data using EF Core

Starting with EF Core 9.0, you can use the <xref:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseSeeding*> and <xref:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseAsyncSeeding*> methods to configure database seeding directly during context configuration. This approach is cleaner than manually running migrations during startup and integrates better with EF Core's lifecycle management.

> [!IMPORTANT]
> These types of configurations should only be done during development, so make sure to add a conditional that checks your current environment context.

### Seed data with UseSeeding and UseAsyncSeeding methods

Add the following code to the _:::no-loc text="Program.cs":::_ file of your **API Service** project:

### [SQL Server](#tab/sql-server)

```csharp
builder.AddSqlServerDbContext<TicketContext>("TicketsDB", configureDbContextOptions: options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.UseSeeding((context, _) =>
        {
            var testTicket = context.Set<SupportTicket>().FirstOrDefault(t => t.Title == "Test Ticket 1");
            if (testTicket == null)
            {
                context.Set<SupportTicket>().Add(new SupportTicket { Title = "Test Ticket 1", Description = "This is a test ticket" });
                context.SaveChanges();
            }

        });
    
        options.UseAsyncSeeding(async (context, _, cancellationToken) =>
        {
            var testTicket = await context.Set<SupportTicket>().FirstOrDefaultAsync(t => t.Title == "Test Ticket 1", cancellationToken);
            if (testTicket == null)
            {
                context.Set<SupportTicket>().Add(new SupportTicket { Title = "Test Ticket 1", Description = "This is a test ticket" });
                await context.SaveChangesAsync(cancellationToken);
            }
        });
    }
});

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    // Ensure database is created and seeded
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<TicketContext>();
        await context.Database.EnsureCreatedAsync();
    }
}
```

### [PostgreSQL](#tab/postgresql)

```csharp
builder.AddNpgsqlDbContext<TicketContext>("TicketsDB", configureDbContextOptions: options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.UseSeeding((context, _) =>
        {
            var testTicket = context.Set<SupportTicket>().FirstOrDefault(t => t.Title == "Test Ticket 1");
            if (testTicket == null)
            {
                context.Set<SupportTicket>().Add(new SupportTicket { Title = "Test Ticket 1", Description = "This is a test ticket" });
                context.SaveChanges();
            }

        });
    
        options.UseAsyncSeeding(async (context, _, cancellationToken) =>
        {
            var testTicket = await context.Set<SupportTicket>().FirstOrDefaultAsync(t => t.Title == "Test Ticket 1", cancellationToken);
            if (testTicket == null)
            {
                context.Set<SupportTicket>().Add(new SupportTicket { Title = "Test Ticket 1", Description = "This is a test ticket" });
                await context.SaveChangesAsync(cancellationToken);
            }
        });
    }
});

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    // Ensure database is created and seeded
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<TicketContext>();
        await context.Database.EnsureCreatedAsync();
    }
}
```

### [MySQL](#tab/mysql)

```csharp
builder.AddMySqlDbContext<TicketContext>("TicketsDB", configureDbContextOptions: options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.UseSeeding((context, _) =>
        {
            var testTicket = context.Set<SupportTicket>().FirstOrDefault(t => t.Title == "Test Ticket 1");
            if (testTicket == null)
            {
                context.Set<SupportTicket>().Add(new SupportTicket { Title = "Test Ticket 1", Description = "This is a test ticket" });
                context.SaveChanges();
            }

        });
    
        options.UseAsyncSeeding(async (context, _, cancellationToken) =>
        {
            var testTicket = await context.Set<SupportTicket>().FirstOrDefaultAsync(t => t.Title == "Test Ticket 1", cancellationToken);
            if (testTicket == null)
            {
                context.Set<SupportTicket>().Add(new SupportTicket { Title = "Test Ticket 1", Description = "This is a test ticket" });
                await context.SaveChangesAsync(cancellationToken);
            }
        });
    }
});

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    // Ensure database is created and seeded
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<TicketContext>();
        await context.Database.EnsureCreatedAsync();
    }
}
```

---

> [!NOTE]
> `UseSeeding` is called from the `EnsureCreated` method, and `UseAsyncSeeding` is called from the `EnsureCreatedAsync` method. When using this feature, it's recommended to implement both `UseSeeding` and `UseAsyncSeeding` methods using similar logic, even if the code using EF is asynchronous. EF Core tooling currently relies on the synchronous version of the method and will not seed the database correctly if the `UseSeeding` method isn't implemented.

The `UseSeeding` and `UseAsyncSeeding` methods provide several advantages over manual seeding approaches:

- **Integrated lifecycle**: Seeding is automatically triggered when the database is created or when migrations are applied.
- **Conditional execution**: The seeding logic only runs when the database is first created, preventing duplicate data on subsequent runs.
- **Better performance**: The seeding methods are optimized for bulk operations and integrate with EF Core's change tracking.
- **Cleaner code**: Seeding configuration is co-located with the context configuration, making it easier to maintain.

### Seed data manually

You can also seed data in Aspire projects using EF Core by explicitly running migrations during startup. EF Core handles underlying database connections and schema creation for you, which eliminates the need to use volumes or run SQL scripts during container startup.

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

        // Manually seed data
        if (!context.Tickets.Any())
        {
            context.Tickets.AddRange(
                new Ticket { Title = "Example Ticket", Description = "This is a sample ticket for testing" }
            );
            context.SaveChanges();
        }
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

        // Manually seed data
        if (!context.Tickets.Any())
        {
            context.Tickets.AddRange(
                new Ticket { Title = "Example Ticket", Description = "This is a sample ticket for testing" }
            );
            context.SaveChanges();
        }
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

        // Manually seed data
        if (!context.Tickets.Any())
        {
            context.Tickets.AddRange(
                new Ticket { Title = "Example Ticket", Description = "This is a sample ticket for testing" }
            );
            context.SaveChanges();
        }
    }
}
```

---

## Next steps

Database seeding is useful in a variety of app development scenarios. Try combining these techniques with the resource implementations demonstrated in the following tutorials:

- [Tutorial: Connect an ASP.NET Core app to SQL Server using Aspire and Entity Framework Core](sql-server-integrations.md)
- [Tutorial: Apply Entity Framework Core migrations in Aspire](ef-core-migrations.md)
- [Tutorial: Connect an ASP.NET Core app to Aspire storage integrations](../storage/azure-storage-integrations.md)
- [Aspire orchestration overview](../fundamentals/app-host-overview.md)
