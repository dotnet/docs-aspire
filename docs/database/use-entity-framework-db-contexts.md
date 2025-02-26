---
title: Use Entity Framework Core with .NET Aspire
description: Learn how to optimize the performance of .NET Aspire Entity Framework integrations using their context objects.
ms.date: 02/21/2025
uid: database/use-entity-framework-db-contexts
zone_pivot_groups: entity-framework-client-integration
---

# Use Entity Framework Core with .NET Aspire

In a cloud-native solution, such as those .NET Aspire is built to create, microservices often need to store data in relational databases. .NET Aspire includes integrations that you can use to ease that task, some of which use the Entity Framework (EF) Object-Relational Mapping (ORM) framework to streamline the process further.

Developers use ORM frameworks to work with databases using code objects instead of SQL queries. EF automatically codes database interactions by generating SQL queries based on LINQ queries. EF supports various database providers, including SQL Server, PostgreSQL, and MySQL so it's easy to interact with relational databases while following object-oriented principles.

The most commonly used .NET Aspire EF client integrations are:

- SQL Server Entity Framework Core integration
- PostgreSQL Entity Framework Core integration
- MySQL Pomelo Entity Framework Core integration
- Oracle Entity Framework Core integration
- Cosmos DB Entity Framework Core integration

> [!NOTE]
> In .NET Aspire, EF is implemented by client integrations, not hosting integrations. So, for example, to use EF with a SQL Server database, you'd use the SQL Server hosting integration to create the SQL Server container and add a database to it. In the consuming microservices, when you want to use EF, choose the SQL Server Entity Framework Core integration instead of the SQL Server client integration.

## Use .NET Aspire to create a database context

In EF, a [**context**](/ef/core/dbcontext-configuration/) is a class used to interact with the database. Contexts inherit from the <xref:Microsoft.EntityFrameworkCore.DbContext> class. They provide access to the database through properties of type `DbSet<T>`, where each `DbSet` represents a table or collection of entities in the database. The context also manages database connections, tracks changes to entities, and handles operations like saving data and executing queries.

The .NET Aspire EF client integrations each include an extension method named `Add\<DatabaseSystem\>DbContext`, where **\<DatabaseSystem\>** is a string identifying the database product you are using. For example, for the SQL Server EF client integration, the method is named <xref:Microsoft.Extensions.Hosting.AspireSqlServerEFCoreSqlClientExtensions.AddSqlServerDbContext%2A> and for the PostgreSQL client integration, the method is named <xref:Microsoft.Extensions.Hosting.AspireEFPostgreSqlExtensions.AddNpgsqlDbContext%2A>.

These .NET Aspire add context methods:

- Check that a database context of the same type is not already registered in the Dependency Injection (DI) container.
- Use the connection name you pass to the method to get the connection string from the application builder. This connection name must match the name used when adding the corresponding resource to the app host project.
- Apply the `DbContext` options, if you passed them.
- Add the specified `DbContext` to the DI container with context pooling enabled.
- Apply the recommended defaults, unless you've disabled them through the Aspire EF settings:
  - Enable tracing.
  - Enable health checks.
  - Enable connection resiliency.

Use these .NET Aspire add context methods when you want a simple way to create the database context and don't yet need advanced EF customization.

:::zone pivot="sql-server-ef"

```csharp
builder.AddSqlServerDbContext<ExampleDbContext>(connectionName: "database");
```

> [!TIP]
> For full information about SQL Server hosting and client integrations, see [.NET Aspire SQL Server Entity Framework Core integration](/dotnet/aspire/database/sql-server-entity-framework-integration).

:::zone-end
:::zone pivot="postgresql-ef"

```csharp
builder.AddNpgsqlDbContext<ExampleDbContext>(connectionName: "database");
```

> [!TIP]
> For full information about PostgreSQL hosting and client integrations, see [.NET Aspire PostgreSQL Entity Framework Core integration](/dotnet/aspire/database/postgresql-entity-framework-integration).

:::zone-end
:::zone pivot="oracle-ef"

```csharp
builder.AddOracleDatabaseDbContext<ExampleDbContext>(connectionName: "database");
```

> [!TIP]
> For full information about Oracle Database hosting and client integrations, see [.NET Aspire Oracle Entity Framework Core integration](/dotnet/aspire/database/oracle-entity-framework-integration).

:::zone-end
:::zone pivot="mysql-ef"

```csharp
builder.AddMySqlDbContext<ExampleDbContext>(connectionName: "database");
```

> [!TIP]
> For full information about MySQL hosting and client integrations, see [.NET Aspire Pomelo MySQL Entity Framework Core integration](/dotnet/aspire/database/mysql-entity-framework-integration).

:::zone-end

Obtain the `ExampleDbContext` object from the DI container in the same way as for any other service:

```csharp
public class ExampleService(ExampleDbContext context)
{
    // Use context...
}
```

## Use EF to add a context and then enrich it

Alternatively, you can add a context to the DI container using the standard EF <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContextPool*> method, as commonly used in non-.NET Aspire projects:

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

You have more flexibility when you create the database context in this way, for example:

- You can reuse existing configuration code for the context without rewriting it for .NET Aspire.
- You can choose not to use EF Core context pooling, which may be necessary in some circumstances. For more information, see [Use EF context pooling in .NET Aspire](#use-ef-context-pooling-in-net-aspire)
- You can use EF Core context factories or change the lifetime for the EF services. For more information, see [Use EF Core context factories in .NET Aspire](#use-ef-core-context-factories-in-net-aspire)
- You can use dynamic connection strings. For more information, see [Use EF with dynamic connection strings in .NET Aspire](#use-ef-with-dynamic-connection-strings-in-net-aspire)
- You can use [EF Core interceptors](/ef/core/logging-events-diagnostics/interceptors) that depend on DI services to modify database operations. For more information, see [Use EF interceptors in .NET Aspire](#use-ef-interceptors-in-net-aspire)

By default, a context configured this way doesn't include .NET Aspire features, such as telemetry and health checks. To add those features, each .NET Aspire EF client integration includes a method named `Enrich\<DatabaseSystem\>DbContext`. These enrich context methods:

- Apply an EF settings object, if you passed one.
- Configure connection retry settings.
- Apply the recommended defaults, unless you've disabled them through the .NET Aspire EF settings:
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

Retrieve the database context from the DI container using the same code as the previous example:

```csharp
public class ExampleService(ExampleDbContext context)
{
    // Use context...
}
```

### Use EF interceptors with .NET Aspire

EF interceptors allow developers to hook into and modify database operations at various points during the execution of database queries and commands. You can use them to log, modify, or suppress operations with your own code. Your interceptor must implement one or more interface from the <xref:Microsoft.EntityFrameworkCore.Diagnostics.IInterceptor> interface.

Interceptors that depend on DI services are not supported by the .NET Aspire `Add\<DatabaseSystem\>DbContext` methods. Use the EF <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContextPool*> method and call the <xref:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.AddInterceptors*> method in the options builder:

:::zone-pivot="sql-server-ef"

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
:::zone-pivot="postgresql-ef"

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
:::zone-pivot="oracle-ef"

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
:::zone-pivot="mysql-ef"

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
> For more information about EF interceptors and their use, see [Interceptors](/ef/core/logging-events-diagnostics/interceptors).

### Use EF with dynamic connection strings in .NET Aspire

Most microservices always connect to the same database with the same credentials and other settings, so they always use the same connection string unless there's a major change in infrastructure. However, you may need to change the connection string for each request. For example:

- You might offer your service to multiple tenants and need to use a different database depending on which customer made the request.
- You might need to authenticate the request with a different database user account depending on which customer made the request.

For these requirements, you can use code to formulate a **dynamic connection string** and then use it to reach the database and run queries. However, this technique isn't supported by the .NET Aspire `Add\<DatabaseSystem\>DbContext` methods. Instead you must use the EF method to create the database context and then enrich it:

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

The above code replaces the place holder `{DatabaseName}` in the connection string with the string `ContosoDatabase`, at run time, before it creates the database context and enriches it.

### Use EF Core context factories in .NET Aspire

An EF context is an object designed to be used for a single unit of work. For example, if you want to add a new customer to the database, you might need to add a row in the **Customers** table and a row in the **Addresses** table. You should get the EF context, add the new customer and address entities to it, call <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync*>, and then dispose the context.

In many types of web application, such as ASP.NET applications, each HTTP request closely corresponds to a single unit of work against the database. If your .NET Aspire microservice is an ASP.NET application or a similar web application, you can use the standard EF <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContextPool*> method described above to register a context that is tied to the current HTTP request. Remember to call the .NET Aspire `Enrich\<DatabaseSystem\>DbContext` method to gain health checks, tracing, and other features. When you use this approach, the database context lifetime is tied to the web request. You don't have to call the <xref:Microsoft.EntityFrameworkCore.DbContext.Dispose*> method when the unit of work is complete.

Other application types, such as ASP.NET Core Blazor, don't necessarily align each request with a unit of work, because they use dependency injection with a different service scope. In such apps, you may need to perform multiple units of work, each with a different database context, within a single HTTP request and response. To implement this approach, you can register a context factory, by calling the EF <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddPooledDbContextFactory*> method. This method also partners well with the .NET Aspire `Enrich\<DatabaseSystem\>DbContext` methods:

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
builder.Services.AddPooledDbContextFactory<ExampleDbContext>(options =>
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
builder.Services.AddPooledDbContextFactory<ExampleDbContext>(options =>
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

## Use EF context pooling in .NET Aspire

In EF Core a database context is relatively quick to create and dispose of so most applications can set them up as needed without impacting their performance. However, the overhead is not zero so, if your microservice intensively creates contexts, you may observe suboptimal performance. In such situations, consider using a database context pool.

Context pooling is a feature of EF Core. Contexts are created as normal but, when you dispose of one, it isn't destroyed but reset and stored in a pool. The next time your code creates a context, the stored one is returned to avoid the extra overhead of creating a new one.

In a .NET Aspire consuming project, there are three ways to use context pooling:

- Use the .NET Aspire `Add\<DatabaseSystem\>DbContext` methods to create the database context. These methods create a context pool automatically.
- Call the EF <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContextPool*> method instead of the EF <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContext*> method.

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

- Call the EF <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddPooledDbContextFactory*> method instead of the EF <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContextFactory*> method.

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

Remember to enrich the database context after using the last two methods, as described above.

> [!IMPORTANT]
> Only the base context state is reset when it's returned to the pool. If you've manually changed the state of the `DbConnection` or another service, you must also manually reset it. Additionally, context pooling prevents you from using `OnConfiguring` to configure the context. See [DbContext pooling](/ef/core/performance/advanced-performance-topics#dbcontext-pooling) for more information.

## See also

- [Entity Framework documentation hub](/ef)
- [Tutorial: Connect an ASP.NET Core app to SQL Server using .NET Aspire and Entity Framework Core](/dotnet/aspire/database/sql-server-integrations)
- [Apply Entity Framework Core migrations in .NET Aspire](/dotnet/aspire/database/ef-core-migrations)
- [DbContext Lifetime, Configuration, and Initialization](/ef/core/dbcontext-configuration/)
- [Advanced Performance Topics](/ef/core/performance/advanced-performance-topics)
- [Entity Framework Interceptors](/ef/core/logging-events-diagnostics/interceptors)
