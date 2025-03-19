---
title: Entity Framework Core overview
description: Learn how to optimize the performance of .NET Aspire Entity Framework Core integrations using their context objects.
ms.date: 03/03/2025
uid: database/use-entity-framework-db-contexts
zone_pivot_groups: entity-framework-client-integration
---

# Entity Framework Core overview

In a cloud-native solution, such as those .NET Aspire is built to create, microservices often need to store data in relational databases. .NET Aspire includes integrations that you can use to ease that task, some of which use the Entity Framework Core (EF Core) object-relational mapper (O/RM) approach to streamline the process.

Developers use O/RMs to work with databases using code objects instead of SQL queries. EF Core automatically codes database interactions by generating SQL queries based on Language-Integrated Query (LINQ) queries. EF Core supports various database providers, including SQL Server, PostgreSQL, and MySQL, so it's easy to interact with relational databases while following object-oriented principles.

The most commonly used .NET Aspire EF Core client integrations are:

- [Cosmos DB Entity Framework Core integration](azure-cosmos-db-entity-framework-integration.md)
- [MySQL Pomelo Entity Framework Core integration](mysql-entity-framework-integration.md)
- [Oracle Entity Framework Core integration](oracle-entity-framework-integration.md)
- [PostgreSQL Entity Framework Core integration](postgresql-entity-framework-integration.md)
- [SQL Server Entity Framework Core integration](sql-server-entity-framework-integration.md)

## Overview of EF Core

O/RMs create a model that matches the schema and relationships defined in the database. Code against this model to query the data, create new records, or make other changes. In EF Core the model consists of:

- A set of entity classes, each of which represents a table in the database and its columns.
- A context class that represents the whole database.

An entity class might look like this:

```csharp
using System.ComponentModel.DataAnnotations;

namespace SupportDeskProject.Data;

public sealed class SupportTicket
{
    public int Id { get; set; }
    [Required]
    public string Title { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;
}
```

This entity class represents a database table with three columns: **Id**, **Title**, and **Description**.

A context class must inherit from <xref:System.Data.Entity.DbContext> and looks like this:

```csharp
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace SupportDeskProject.Data;

public class TicketContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<SupportTicket> Tickets => Set<SupportTicket>();
}
```

This context represents a database with a single table of support tickets. An instance of the context is usually created for each unit of work in the database. For example, a unit of work might be the creation of a new customer, and require changes in the Customers and Addresses tables. Once the unit of work is complete, you should dispose of the context.

> [!NOTE]
> For more information about creating models in EF Core, see [Creating and Configuring a Model](/ef/core/modeling/) in the EF Core documentation.

Once you've created a model, you can use LINQ to query it:

```csharp
using (var db = new TicketContext())
{
    var tickets = await db.Tickets
        .Where(t => t.Title = "Unable to log on")
        .OrderBy(t => t.Description)
        .ToListAsync();
}
```

> [!NOTE]
> EF Core also supports creating, modifying, and deleted records and complex queries. For more information, see [Querying Data](/ef/core/querying/) and [Saving Data](/ef/core/saving/)

## How .NET Aspire can help

.NET Aspire is designed to help build observable, production-ready, cloud-native solutions that consist of multiple microservices. It orchestrates multiple projects, each of which may be a microservice written by a dedicated team, and connects them to each other. It provides integrations that make it easy to connect to common services, such as databases.

If you want to use EF Core in any of your microservices, .NET Aspire can help by:

- Managing the connection to the database centrally in the App Host project and passing it to any project that uses it. There are two ways to approach this task:
    - By creating a new instance of the database in a virtualization container. You can configure this container either to be recreated every time you debug your solution or to persist data across debugging sessions.
    - By storing a connection string to an existing database.
- Providing EF Core-aware integrations that make it easy to create contexts in microservice projects. There are EF Core integrations for SQL Server, MySQL, PostgreSQL, Cosmos DB, and other popular database systems. In each microservice:
    - Define the EF Core model with entity classes and context classes.
    - Create an instance of the data context and add it to the Dependency Injection (DI) container.
    - When you want to interact with the database, obtain the context from DI and use it to execute LINQ queries against the database as normal for any EF Core code.

> [!NOTE]
> In .NET Aspire, EF Core is implemented by client integrations, not hosting integrations. So, for example, to use EF Core with a SQL Server database, you'd use the SQL Server hosting integration to create the SQL Server container and add a database to it. In the consuming microservices, when you want to use EF Core, choose the SQL Server Entity Framework Core integration instead of the SQL Server client integration.

> AJMTODO: Diagram



Note to explain that the rest of this article concerns DBContexts.


## Use .NET Aspire to create an EF Core context

In EF Core, a [**context**](/ef/core/dbcontext-configuration/) is a class used to interact with the database. Contexts inherit from the <xref:Microsoft.EntityFrameworkCore.DbContext> class. They provide access to the database through properties of type `DbSet<T>`, where each `DbSet` represents a table or collection of entities in the database. The context also manages database connections, tracks changes to entities, and handles operations like saving data and executing queries.

The .NET Aspire EF Core client integrations each include extension methods named `Add{DatabaseSystem}DbContext`, where **{DatabaseSystem}** is the name identifying the database product you're using. For example, consider the SQL Server EF Core client integration, the method is named <xref:Microsoft.Extensions.Hosting.AspireSqlServerEFCoreSqlClientExtensions.AddSqlServerDbContext%2A> and for the PostgreSQL client integration, the method is named <xref:Microsoft.Extensions.Hosting.AspireEFPostgreSqlExtensions.AddNpgsqlDbContext%2A>.

These .NET Aspire add context methods:

- Check that a context of the same type isn't already registered in the dependency injection (DI) container.
- Use the connection name you pass to the method to get the connection string from the application builder. This connection name must match the name used when adding the corresponding resource to the app host project.
- Apply the `DbContext` options, if you passed them.
- Add the specified `DbContext` to the DI container with context pooling enabled.
- Apply the recommended defaults, unless you've disabled them through the .NET Aspire EF Core settings:
  - Enable tracing.
  - Enable health checks.
  - Enable connection resiliency.

Use these .NET Aspire add context methods when you want a simple way to create a context and don't yet need advanced EF Core customization.

:::zone pivot="sql-server-ef"

```csharp
builder.AddSqlServerDbContext<ExampleDbContext>(connectionName: "database");
```

> [!TIP]
> For more information about SQL Server hosting and client integrations, see [.NET Aspire SQL Server Entity Framework Core integration](sql-server-entity-framework-integration.md).

:::zone-end
:::zone pivot="postgresql-ef"

```csharp
builder.AddNpgsqlDbContext<ExampleDbContext>(connectionName: "database");
```

> [!TIP]
> For more information about PostgreSQL hosting and client integrations, see [.NET Aspire PostgreSQL Entity Framework Core integration](postgresql-entity-framework-integration.md).

:::zone-end
:::zone pivot="oracle-ef"

```csharp
builder.AddOracleDatabaseDbContext<ExampleDbContext>(connectionName: "database");
```

> [!TIP]
> For more information about Oracle Database hosting and client integrations, see [.NET Aspire Oracle Entity Framework Core integration](oracle-entity-framework-integration.md).

:::zone-end
:::zone pivot="mysql-ef"

```csharp
builder.AddMySqlDbContext<ExampleDbContext>(connectionName: "database");
```

> [!TIP]
> For more information about MySQL hosting and client integrations, see [.NET Aspire Pomelo MySQL Entity Framework Core integration](mysql-entity-framework-integration.md).

:::zone-end

You obtain the `ExampleDbContext` object from the DI container in the same way as for any other service:

```csharp
public class ExampleService(ExampleDbContext context)
{
    // Use context...
}
```

## Use EF Core to add and enrich context

Alternatively, you can add a context to the DI container using the standard EF Core <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContextPool*> method, as commonly used in non-.NET Aspire projects:

:::zone pivot="sql-server-ef"

```csharp
builder.Services.AddDbContextPool<ExampleDbContext>(options =>
    var connectionString = builder.Configuration.GetConnectionString("database")
        ?? throw new InvalidOperationException("Connection string 'database' not found.");
    options.UseSqlServer(connectionString));
```

:::zone-end
:::zone pivot="postgresql-ef"

```csharp
builder.Services.AddDbContextPool<ExampleDbContext>(options =>
    var connectionString = builder.Configuration.GetConnectionString("database")
        ?? throw new InvalidOperationException("Connection string 'database' not found.");
    options.UseNpgsql(connectionString));
```

:::zone-end
:::zone pivot="oracle-ef"

```csharp
builder.Services.AddDbContextPool<ExampleDbContext>(options =>
    var connectionString = builder.Configuration.GetConnectionString("database")
        ?? throw new InvalidOperationException("Connection string 'database' not found.");
    options.UseOracle(connectionString));
```

:::zone-end
:::zone pivot="mysql-ef"

```csharp
builder.Services.AddDbContextPool<ExampleDbContext>(options =>
    var connectionString = builder.Configuration.GetConnectionString("database")
        ?? throw new InvalidOperationException("Connection string 'database' not found.");
    options.UseMySql(connectionString));
```

:::zone-end

You have more flexibility when you create the context in this way, for example:

- You can reuse existing configuration code for the context without rewriting it for .NET Aspire.
- You can choose not to use EF Core context pooling, which may be necessary in some circumstances. For more information, see [Use EF Core context pooling in .NET Aspire](#use-ef-core-context-pooling-in-net-aspire)
- You can use EF Core context factories or change the lifetime for the EF Core services. For more information, see [Use EF Core context factories in .NET Aspire](#use-ef-core-context-factories-in-net-aspire)
- You can use dynamic connection strings. For more information, see [Use EF Core with dynamic connection strings in .NET Aspire](#use-ef-core-with-dynamic-connection-strings-in-net-aspire)
- You can use [EF Core interceptors](/ef/core/logging-events-diagnostics/interceptors) that depend on DI services to modify database operations. For more information, see [Use EF Core interceptors in .NET Aspire](#use-ef-core-interceptors-with-net-aspire)

By default, a context configured this way doesn't include .NET Aspire features, such as telemetry and health checks. To add those features, each .NET Aspire EF Core client integration includes a method named `Enrich\<DatabaseSystem\>DbContext`. These enrich context methods:

- Apply an EF Core settings object, if you passed one.
- Configure connection retry settings.
- Apply the recommended defaults, unless you've disabled them through the .NET Aspire EF Core settings:
  - Enable tracing.
  - Enable health checks.
  - Enable connection resiliency.

> [!NOTE]
> You must add a context to the DI container before you call an enrich method.

:::zone pivot="sql-server-ef"

```csharp
builder.EnrichSqlServerDbContext<ExampleDbContext>(
    configureSettings: settings =>
    {
        settings.DisableRetry = false;
        settings.CommandTimeout = 30 // seconds
    });
```

:::zone-end
:::zone pivot="postgresql-ef"

```csharp
builder.EnrichNpgsqlDbContext<ExampleDbContext>(
    configureSettings: settings =>
    {
        settings.DisableRetry = false;
        settings.CommandTimeout = 30 // seconds
    });
```

:::zone-end
:::zone pivot="oracle-ef"

```csharp
builder.EnrichOracleDatabaseDbContext<ExampleDbContext>(
    configureSettings: settings =>
    {
        settings.DisableRetry = false;
        settings.CommandTimeout = 30 // seconds
    });
```

:::zone-end
:::zone pivot="mysql-ef"

```csharp
builder.EnrichMySqlDbContext<ExampleDbContext>(
    configureSettings: settings =>
    {
        settings.DisableRetry = false;
        settings.CommandTimeout = 30 // seconds
    });
```

:::zone-end

Obtain the context from the DI container using the same code as the previous example:

```csharp
public class ExampleService(ExampleDbContext context)
{
    // Use context...
}
```

### Use EF Core interceptors with .NET Aspire

EF Core interceptors allow developers to hook into and modify database operations at various points during the execution of database queries and commands. You can use them to log, modify, or suppress operations with your own code. Your interceptor must implement one or more interface from the <xref:Microsoft.EntityFrameworkCore.Diagnostics.IInterceptor> interface.

Interceptors that depend on DI services are not supported by the .NET Aspire `Add\<DatabaseSystem\>DbContext` methods. Use the EF Core <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContextPool*> method and call the <xref:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.AddInterceptors*> method in the options builder:

:::zone pivot="sql-server-ef"

```csharp
builder.Services.AddDbContextPool<ExampleDbContext>((serviceProvider, options) =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("database"));
        options.AddInterceptors(serviceProvider.GetRequiredService<ExampleInterceptor>());
    });

builder.EnrichSqlServerDbContext<ExampleDbContext>(
    configureSettings: settings =>
    {
        settings.DisableRetry = false;
        settings.CommandTimeout = 30 // seconds
    });
```

:::zone-end
:::zone pivot="postgresql-ef"

```csharp
builder.Services.AddDbContextPool<ExampleDbContext>((serviceProvider, options) =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("database"));
        options.AddInterceptors(serviceProvider.GetRequiredService<ExampleInterceptor>());
    });

builder.EnrichNpgsqlDbContext<ExampleDbContext>(
    configureSettings: settings =>
    {
        settings.DisableRetry = false;
        settings.CommandTimeout = 30 // seconds
    });
```

:::zone-end
:::zone pivot="oracle-ef"

```csharp
builder.Services.AddDbContextPool<ExampleDbContext>((serviceProvider, options) =>
    {
        options.UseOracle(builder.Configuration.GetConnectionString("database"));
        options.AddInterceptors(serviceProvider.GetRequiredService<ExampleInterceptor());
    });

builder.EnrichOracleDatabaseDbContext<ExampleDbContext>(
    configureSettings: settings =>
    {
        settings.DisableRetry = false;
        settings.CommandTimeout = 30 // seconds
    });
```

:::zone-end
:::zone pivot="mysql-ef"

```csharp
builder.Services.AddDbContextPool<ExampleDbContext>((serviceProvider, options) =>
    {
        options.UseMySql(builder.Configuration.GetConnectionString("database"));
        options.AddInterceptors(serviceProvider.GetRequiredService<ExampleInterceptor>());
    });

builder.EnrichMySqlDbContext<ExampleDbContext>(
    configureSettings: settings =>
    {
        settings.DisableRetry = false;
        settings.CommandTimeout = 30 // seconds
    });
```

:::zone-end

> [!NOTE]
> For more information about EF Core interceptors and their use, see [Interceptors](/ef/core/logging-events-diagnostics/interceptors).

### Use EF Core with dynamic connection strings in .NET Aspire

Most microservices always connect to the same database with the same credentials and other settings, so they always use the same connection string unless there's a major change in infrastructure. However, you may need to change the connection string for each request. For example:

- You might offer your service to multiple tenants and need to use a different database depending on which customer made the request.
- You might need to authenticate the request with a different database user account depending on which customer made the request.

For these requirements, you can use code to formulate a **dynamic connection string** and then use it to reach the database and run queries. However, this technique isn't supported by the .NET Aspire `Add\<DatabaseSystem\>DbContext` methods. Instead you must use the EF Core method to create the context and then enrich it:

:::zone pivot="sql-server-ef"

```csharp
var connectionStringWithPlaceHolder = builder.Configuration.GetConnectionString("database")
    ?? throw new InvalidOperationException("Connection string 'database' not found.");

connectionString = connectionStringWithPlaceHolder.Replace("{DatabaseName}", "ContosoDatabase");

builder.Services.AddDbContext<ExampleDbContext>(options =>
    options.UseSqlServer(connectionString
        ?? throw new InvalidOperationException("Connection string 'database' not found.")));

builder.EnrichSqlServerDbContext<ExampleDbContext>(
    configureSettings: settings =>
    {
        settings.DisableRetry = false;
        settings.CommandTimeout = 30 // seconds
    });
```

:::zone-end
:::zone pivot="postgresql-ef"

```csharp
var connectionStringWithPlaceHolder = builder.Configuration.GetConnectionString("database")
    throw new InvalidOperationException("Connection string 'database' not found.");

connectionString = connectionStringWithPlaceHolder.Replace("{DatabaseName}", "ContosoDatabase");

builder.Services.AddDbContext<ExampleDbContext>(options =>
    options.UseNpgsql(connectionString
        ?? throw new InvalidOperationException("Connection string 'database' not found.")));

builder.EnrichNpgsqlDbContext<ExampleDbContext>(
    configureSettings: settings =>
    {
        settings.DisableRetry = false;
        settings.CommandTimeout = 30 // seconds
    });
```

:::zone-end
:::zone pivot="oracle-ef"

```csharp
var connectionStringWithPlaceHolder = builder.Configuration.GetConnectionString("database")
    throw new InvalidOperationException("Connection string 'database' not found.");

connectionString = connectionStringWithPlaceHolder.Replace("{DatabaseName}", "ContosoDatabase");

builder.Services.AddDbContext<ExampleDbContext>(options =>
    options.UseOracle(connectionString
        ?? throw new InvalidOperationException("Connection string 'database' not found.")));

builder.EnrichOracleDatabaseDbContext<ExampleDbContext>(
    configureSettings: settings =>
    {
        settings.DisableRetry = false;
        settings.CommandTimeout = 30 // seconds
    });
```

:::zone-end
:::zone pivot="mysql-ef"

```csharp
var connectionStringWithPlaceHolder = builder.Configuration.GetConnectionString("database")
    throw new InvalidOperationException("Connection string 'database' not found.");

connectionString = connectionStringWithPlaceHolder.Replace("{DatabaseName}", "ContosoDatabase");

builder.Services.AddDbContext<ExampleDbContext>(options =>
    options.UseMySql(connectionString
        ?? throw new InvalidOperationException("Connection string 'database' not found.")));

builder.EnrichMySqlDbContext<ExampleDbContext>(
    configureSettings: settings =>
    {
        settings.DisableRetry = false;
        settings.CommandTimeout = 30 // seconds
    });
```

:::zone-end

The above code replaces the place holder `{DatabaseName}` in the connection string with the string `ContosoDatabase`, at run time, before it creates the context and enriches it.

### Use EF Core context factories in .NET Aspire

An EF Core context is an object designed to be used for a single unit of work. For example, if you want to add a new customer to the database, you might need to add a row in the **Customers** table and a row in the **Addresses** table. You should get the EF Core context, add the new customer and address entities to it, call <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync*>, and then dispose the context.

In many types of web application, such as ASP.NET applications, each HTTP request closely corresponds to a single unit of work against the database. If your .NET Aspire microservice is an ASP.NET application or a similar web application, you can use the standard EF Core <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContextPool*> method described above to register a context that is tied to the current HTTP request. Remember to call the .NET Aspire `Enrich\<DatabaseSystem\>DbContext` method to gain health checks, tracing, and other features. When you use this approach, the context lifetime is tied to the web request. You don't have to call the <xref:Microsoft.EntityFrameworkCore.DbContext.Dispose*> method when the unit of work is complete.

Other application types, such as ASP.NET Core Blazor, don't necessarily align each request with a unit of work, because they use dependency injection with a different service scope. In such apps, you may need to perform multiple units of work, each with a different context, within a single HTTP request and response. To implement this approach, you can register a context factory, by calling the EF Core <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddPooledDbContextFactory*> method. This method also partners well with the .NET Aspire `Enrich\<DatabaseSystem\>DbContext` methods:

:::zone pivot="sql-server-ef"

```csharp
builder.Services.AddPooledDbContextFactory<ExampleDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("database")
        ?? throw new InvalidOperationException("Connection string 'database' not found.")));

builder.EnrichSqlServerDbContext<ExampleDbContext>(
    configureSettings: settings =>
    {
        settings.DisableRetry = false;
        settings.CommandTimeout = 30 // seconds
    });
```

:::zone-end
:::zone pivot="postgresql-ef"

```csharp
builder.Services.AddPooledDbContextFactory<ExampleDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("database")
        ?? throw new InvalidOperationException("Connection string 'database' not found.")));

builder.EnrichNpgsqlDbContext<ExampleDbContext>(
    configureSettings: settings =>
    {
        settings.DisableRetry = false;
        settings.CommandTimeout = 30 // seconds
    });
```

:::zone-end
:::zone pivot="oracle-ef"

```csharp
builder.Services.AddPooledDbContextFactory<ExampleDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("database")
        ?? throw new InvalidOperationException("Connection string 'database' not found.")));

builder.EnrichOracleDatabaseDbContext<ExampleDbContext>(
    configureSettings: settings =>
    {
        settings.DisableRetry = false;
        settings.CommandTimeout = 30 // seconds
    });
```

:::zone-end
:::zone pivot="mysql-ef"

```csharp
builder.Services.AddPooledDbContextFactory<ExampleDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("database")
        ?? throw new InvalidOperationException("Connection string 'database' not found.")));

builder.EnrichMySqlDbContext<ExampleDbContext>(
    configureSettings: settings =>
    {
        settings.DisableRetry = false;
        settings.CommandTimeout = 30 // seconds
    });
```

:::zone-end

Notice that the above code adds and enriches a *context factory* in the DI container. When you retreive this from the container, you must add a line of code to create a *context* from it:

```csharp
public class ExampleService(IDbContextFactory<ExampleDbContext> contextFactory)
{
    using (var context = contextFactory.CreateDbContext())
    {
        // Use context...
    }
}
```

Contexts created from factories in this way aren't disposed of automatically because they aren't tied to an HTTP request lifetime. You must make sure your code disposes of them. In this example, the `using` code block ensures the disposal.

## Use EF Core context pooling in .NET Aspire

In EF Core a context is relatively quick to create and dispose of so most applications can set them up as needed without impacting their performance. However, the overhead is not zero so, if your microservice intensively creates contexts, you may observe suboptimal performance. In such situations, consider using a context pool.

Context pooling is a feature of EF Core. Contexts are created as normal but, when you dispose of one, it isn't destroyed but reset and stored in a pool. The next time your code creates a context, the stored one is returned to avoid the extra overhead of creating a new one.

In a .NET Aspire consuming project, there are three ways to use context pooling:

- Use the .NET Aspire `Add\<DatabaseSystem\>DbContext` methods to create the context. These methods create a context pool automatically.
- Call the EF Core <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContextPool*> method instead of the EF Core <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContext*> method.

    :::zone pivot="sql-server-ef"

    ```csharp
    builder.Services.AddDbContextPool<ExampleDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("database")
            ?? throw new InvalidOperationException("Connection string 'database' not found.")));
    ```

    :::zone-end
    :::zone pivot="postgresql-ef"

    ```csharp
    builder.Services.AddDbContextPool<ExampleDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("database")
            ?? throw new InvalidOperationException("Connection string 'database' not found.")));
    ```

    :::zone-end
    :::zone pivot="oracle-ef"

    ```csharp
    builder.Services.AddDbContextPool<ExampleDbContext>(options =>
        options.UseOracle(builder.Configuration.GetConnectionString("database")
            ?? throw new InvalidOperationException("Connection string 'database' not found.")));
    ```

    :::zone-end
    :::zone pivot="mysql-ef"

    ```csharp
    builder.Services.AddDbContextPool<ExampleDbContext>(options =>
        options.UseMySql(builder.Configuration.GetConnectionString("database")
            ?? throw new InvalidOperationException("Connection string 'database' not found.")));
    ```

    :::zone-end

- Call the EF Core <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddPooledDbContextFactory*> method instead of the EF Core <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContextFactory*> method.

    :::zone pivot="sql-server-ef"

    ```csharp
    builder.Services.AddPooledDbContextFactory<ExampleDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("database")
            ?? throw new InvalidOperationException("Connection string 'database' not found.")));
    ```

    :::zone-end
    :::zone pivot="postgresql-ef"

    ```csharp
    builder.Services.AddPooledDbContextFactory<ExampleDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("database")
            ?? throw new InvalidOperationException("Connection string 'database' not found.")));
    ```

    :::zone-end
    :::zone pivot="oracle-ef"

    ```csharp
    builder.Services.AddPooledDbContextFactory<ExampleDbContext>(options =>
        options.UseOracle(builder.Configuration.GetConnectionString("database")
            ?? throw new InvalidOperationException("Connection string 'database' not found.")));
    ```

    :::zone-end
    :::zone pivot="mysql-ef"

    ```csharp
    builder.Services.AddPooledDbContextFactory<ExampleDbContext>(options =>
        options.UseMySql(builder.Configuration.GetConnectionString("database")
            ?? throw new InvalidOperationException("Connection string 'database' not found.")));
    ```

    :::zone-end

Remember to enrich the context after using the last two methods, as described above.

> [!IMPORTANT]
> Only the base context state is reset when it's returned to the pool. If you've manually changed the state of the `DbConnection` or another service, you must also manually reset it. Additionally, context pooling prevents you from using `OnConfiguring` to configure the context. See [DbContext pooling](/ef/core/performance/advanced-performance-topics#dbcontext-pooling) for more information.

## See also

- [Entity Framework Core documentation hub](/ef/core)
- [Tutorial: Connect an ASP.NET Core app to SQL Server using .NET Aspire and Entity Framework Core](/dotnet/aspire/database/sql-server-integrations)
- [Apply Entity Framework Core migrations in .NET Aspire](/dotnet/aspire/database/ef-core-migrations)
- [DbContext Lifetime, Configuration, and Initialization](/ef/core/dbcontext-configuration/)
- [Advanced Performance Topics](/ef/core/performance/advanced-performance-topics)
- [Entity Framework Interceptors](/ef/core/logging-events-diagnostics/interceptors)
