---
title: Use Entity Framework with .NET Aspire
description: Learn how to use the Entity Framework integrations to interact with a database from code in a .NET Aspire solution.
ms.date: 02/12/2025
uid: database/use-entity-framework
---

# Use Entity Framework with .NET Aspire

In a cloud-native solution, such as those .NET Aspire is built to create, microservices often need to store data in relational databases. .NET Aspire includes integrations that you can use to ease that task, some of which use the Entity Framework (EF) Object-Relational Mapping (ORM) framework to streamline to process further.

Developers use ORM frameworks to work with databases using code objects instead of SQL queries. EF automatically codes database interactions by generating SQL queries based on LINQ queries. EF supports various database providers, including SQL Server, PostgreSQL, and MySQL so it's easy to interact with relational databases while following object-oriented principles.

.NET Aspire includes the following client integrations, which implement EF for different database systems:

- SQL Server Entity Framework Core integration
- PostgreSQL Entity Framework Core integration
- MySQL Pomelo Entity Framework Core integration
- Oracle Entity Framework Core integration
- Cosmos DB Entity Framework Core integration

> [!NOTE]
> In .NET Aspire, EF is implemented by client integrations, not hosting integrations. So, for example, to use EF with a SQL Server database, you'd use the SQL Server hosting integration to create the SQL Server container and add a database to it. In the consuming microservices, when you want to use EF, choose the SQL Server Entity Framework Core integration instead of the SQL Server client integration.

## Stuff in the App Host

Is there stuff to do in the App Host that only applies to EF?

## Install the client integration

> Either repeat this here or link to the individual articles

## Use a .NET Aspire database context

In EF, a [database context](/ef/core/dbcontext-configuration/) is a class used to interact with the database. Database contexts inherit from the <xref:Microsoft.EntityFrameworkCore.DbContext> class. They provide access to the database through properties of type `DbSet<T>`, where each `DbSet` represents a table or collection of entities in the database. The context also manages database connections, tracks changes to entities, and handles operations like saving data and executing queries.

The .NET Aspire EF client integrations each include an extension method named `Add\<DatabaseSystem\>DbContext`, where **\<DatabaseSystem\>** is a string identifying the database product you are using. For example, for the SQL Server client extension, the method is named <xref:Microsoft.Extensions.Hosting.AspireSqlServerEFCoreSqlClientExtensions.AddSqlServerDbContext%2A> and for the PostgreSQL client extension, the method is named <xref:Microsoft.Extensions.Hosting.AspireEFPostgreSqlExtensions.AddNpgsqlDbContext%2A>.

These .NET Aspire add context methods:

- Check that a database context of the same type is not already registered in the Dependency Injection (DI) container.
- Use the connection name you pass to the method to get the connection string from the application builder. This connection name must match the name used when adding the corresponding resource to the app host project.
- Apply an EF settings object, if you passed one.
- Add a database context pool to the DI container.
- Configure instrumentation, such as tracing and health checks.

Use these .NET Aspire add context methods when you want a simple way to create the database context and don't need to implement advanced EF techniques.

> AJMTODO: Pivots?

```csharp
builder.AddSqlServerDbContext<ExampleDbContext>(connectionName: "database");
```

To retrieve the `ExampleDbContext` object from the DI container:

```csharp
public class ExampleService(ExampleDbContext context)
{
    // Use context...
}
```


> AJMTODO: Link to individual articles?

## Enrich an Entity Framework database context

Alternatively, you can create a database context and add it to the DI container using the standard entity framework <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContext*> method, as used in non-.NET Aspire projects:

> AJMTODO: pivots here?

```csharp
builder.Services.AddDbContext<ExampleDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("database")
        ?? throw new InvalidOperationException("Connection string 'database' not found.")));
```

You have more flexibility when you create the database context in this way, for example:

- You can reuse existing configuration code for the database context without rewriting it for .NET Aspire.
- You can choose not to use Entity Framework Core context pooling, which may perform better in some circumstances.
- You can use [Entity Framework Core interceptors](/ef/core/logging-events-diagnostics/interceptors) to modify database operations.

    ```csharp
    builder.Services.AddDbContext<ExampleDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("database"));
            options.AddInterceptors(new ExampleInterceptor());
		});
    ```

By default, a database context created this way doesn't include .NET Aspire features, such as telemetry and health checks. To add those features, each .NET Aspire EF client integration includes a method named `Enrich\<DatabaseSystem\>DbContext`. These enrich context methods:

- Apply an EF settings object, if you passed one.
- Configure connection retry settings.
- Configure instrumentation, such as tracing and health checks.

> [!NOTE]
> You must have already added a database context to the DI container before you call an enrich method.

> AJMTODO: pivots here?

```csharp
builder.EnrichSqlServerDbContext<ExampleDbContext>(
    configureSettings: settings =>
    {
        settings.DisableRetry = false;
        settings.CommandTimeout = 30 // seconds
    });
```

Retrieve the database context from the DI container using the same code as the previous example:

```csharp
public class ExampleService(ExampleDbContext context)
{
    // Use context...
}
```

## Use Entity Framework context factories in .NET Aspire

An Entity Framework database context is an object designed to be used for a single unit of work. For example, if you want to add a new customer to the database, you might need to add a row in the Customers table and a row in the Addresses table. You should create the EF context, add the new customer and address entities to it, call <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChanges*> or <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync*>, and then dispose the context.

In many types of web application, such as ASP.NET applications, each HTTP request closely corresponds to a single unit of work against the database. If your .NET Aspire microservice is an ASP.NET application or a similar web application, you can use the standard Entity Framework <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContext> method described above to create a context that is tied to the current HTTP request. Remember to call the .NET Aspire `Enrich\<DatabaseSystem\>DbContext` method to gain health checks, tracing, and other features. When you use this approach, the database context lifetime is tied to the web request. You don't have to call the `Dispose` method when the unit of work is complete. 

> AJMTODO: Review the method names and do xrefs for them.

Other types of application, such as ASP.NET Core Blazor, don't necessarily align each request with a unit of work, because they use dependency injection with a different service scope. In such apps, you may need to perform multiple units of work, each with a different database context, within a single HTTP request and response. To implement this approach, use a you can create a factory for database contexts, by calling <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContextFactory*>. This method partners well with the .NET Aspire `Enrich\<DatabaseSystem\>DbContext` methods:

> AJMTODO: Pivot?

```csharp
builder.Services.AddDbContextFactory<ExampleDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("database")
        ?? throw new InvalidOperationException("Connection string 'database' not found.")));

builder.EnrichSqlServerDbContext<ExampleDbContext>(
    configureSettings: settings =>
    {
        settings.DisableRetry = false;
        settings.CommandTimeout = 30 // seconds
    });
```

Notice that the above code adds and enriches a database context factory in the DI container. When you retreive this from the container, you must add a line of code to create a database context from it:

```csharp
public class ExampleService(IDbContextFactory<ExampleDbContext> contextFactory)
{
    using (var context = contextFactory.CreateDbContext())
    {
        // Use context...
    }
}
```

Database contexts created from factories in this way aren't disposed of automatically because they aren't tied to an HTTP request lifetime. You must make sure your code disposes of them. In this example, the `using` code block ensures the disposal. 

## Use Entity Framework context pooling in .NET Aspire

In Entity Framework a database context is relatively quick to create and dispose of so most applications can set them up as needed without impacting their performance. However, the overhead is not zero so, if your microservice intensively creates contexts, you may observe suboptimal performance. In such situations, consider using a database context pool.

Database context pooling is a feature of Entity Framework. Database contexts are created as normal but, when you dispose of one, it isn't destroyed but reset and stored in a pool. The next time your code creates a context, the stored one is returned to avoid the extra overhead of creating a new one.

In a .NET Aspire consuming project, there are three ways to use context pooling:

- If you use the .NET Aspire `Add\<DatabaseSystem\>DbContext` methods, a context pool is automatically used.
- Call the <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContextPool> method instead of the Entity Framework <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContext> method.

    ```csharp
    builder.Services.AddDbContextPool<ExampleDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("database")
            ?? throw new InvalidOperationException("Connection string 'database' not found.")));
    ```

- Call the <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbPooledContextFactory> method instead of the Entity Framework <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContextFactory> method.

    ```csharp
    builder.Services.AddDbPooledContextFactory<ExampleDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("database")
            ?? throw new InvalidOperationException("Connection string 'database' not found.")));
    ```

## Entity Framework approaches

Model first, database-first, code-first

Migrations are for the last one only.

## Use Entity Framework migrations

Intro - what is a migration? How does it work outside of Aspire

Link to EF documentation

### Create a migrations service

Is this the best way to do it? Refer to the migration tutorial

### Migrations and transactions

Should you use these or not (I think it depends on the version of Aspire and EF that you're using).

### Seed the database

Refer to the existing article

## See also

- [Entity Framework documentation hub](/ef)
- [Tutorial: Connect an ASP.NET Core app to SQL Server using .NET Aspire and Entity Framework Core](/dotnet/aspire/database/sql-server-integrations)
- [Apply Entity Framework Core migrations in .NET Aspire](/dotnet/aspire/database/ef-core-migrations)
- [Seed data in a database using .NET Aspire](/aspire/database/seed-database-data)
- [DbContext Lifetime, Configuration, and Initialization](/ef/core/dbcontext-configuration/)
- [Advanced Performance Topics](/ef/core/performance/advanced-performance-topics)
- [Entity Framework Interceptors](/ef/core/logging-events-diagnostics/interceptors)
