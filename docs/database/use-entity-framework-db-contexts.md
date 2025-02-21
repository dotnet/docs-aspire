---
title: Use Entity Framework database contexts with .NET Aspire
description: Learn how to optimize the performance of .NET Aspire Entity Framework integrations using their database context objects.
ms.date: 02/21/2025
uid: database/use-entity-framework-db-contexts
---

# Use Entity Framework database contexts with .NET Aspire

In a cloud-native solution, such as those .NET Aspire is built to create, microservices often need to store data in relational databases. .NET Aspire includes integrations that you can use to ease that task, some of which use the Entity Framework (EF) Object-Relational Mapping (ORM) framework to streamline the process further.

Developers use ORM frameworks to work with databases using code objects instead of SQL queries. EF automatically codes database interactions by generating SQL queries based on LINQ queries. EF supports various database providers, including SQL Server, PostgreSQL, and MySQL so it's easy to interact with relational databases while following object-oriented principles.

.NET Aspire includes the following client integrations, which implement EF for different database systems:

- SQL Server Entity Framework Core integration
- PostgreSQL Entity Framework Core integration
- MySQL Pomelo Entity Framework Core integration
- Oracle Entity Framework Core integration
- Cosmos DB Entity Framework Core integration

> [!NOTE]
> In .NET Aspire, EF is implemented by client integrations, not hosting integrations. So, for example, to use EF with a SQL Server database, you'd use the SQL Server hosting integration to create the SQL Server container and add a database to it. In the consuming microservices, when you want to use EF, choose the SQL Server Entity Framework Core integration instead of the SQL Server client integration.

## Use .NET Aspire to create a database context

In EF, a [**database context**](/ef/core/dbcontext-configuration/) is a class used to interact with the database. Database contexts inherit from the <xref:Microsoft.EntityFrameworkCore.DbContext> class. They provide access to the database through properties of type `DbSet<T>`, where each `DbSet` represents a table or collection of entities in the database. The context also manages database connections, tracks changes to entities, and handles operations like saving data and executing queries.

The .NET Aspire EF client integrations each include an extension method named `Add\<DatabaseSystem\>DbContext`, where **\<DatabaseSystem\>** is a string identifying the database product you are using. For example, for the SQL Server EF client integration, the method is named <xref:Microsoft.Extensions.Hosting.AspireSqlServerEFCoreSqlClientExtensions.AddSqlServerDbContext%2A> and for the PostgreSQL client integration, the method is named <xref:Microsoft.Extensions.Hosting.AspireEFPostgreSqlExtensions.AddNpgsqlDbContext%2A>.

These .NET Aspire add context methods:

- Check that a database context of the same type is not already registered in the Dependency Injection (DI) container.
- Use the connection name you pass to the method to get the connection string from the application builder. This connection name must match the name used when adding the corresponding resource to the app host project.
- Apply an EF settings object, if you passed one.
- Add a database context pool to the DI container.
- Configure instrumentation on the database context, such as tracing and health checks.

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

## Use EF to create a database context and then enrich it

Alternatively, you can create a database context and add it to the DI container using the standard EF <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContext*> method, as commonly used in non-.NET Aspire projects:

> AJMTODO: pivots here?

```csharp
builder.Services.AddDbContext<ExampleDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("database")
        ?? throw new InvalidOperationException("Connection string 'database' not found.")));
```

You have more flexibility when you create the database context in this way, for example:

- You can reuse existing configuration code for the database context without rewriting it for .NET Aspire.
- You can choose not to use EF Core context pooling, which may perform better in some circumstances. For more information, see [Use EF context pooling in .NET Aspire](#use-ef-context-pooling-in-net-aspire)
- You can use dynamic connection strings. For more information, see [Use EF with dynamic connection strings in .NET Aspire](#use-ef-with-dynamic-connection-strings-in-net-aspire)
- You can use [EF Core interceptors](/ef/core/logging-events-diagnostics/interceptors) to modify database operations. For more information, see [Use EF interceptors in .NET Aspire](#use-ef-inteceptors-in-net-aspire)

By default, a database context created this way doesn't include .NET Aspire features, such as telemetry and health checks. To add those features, each .NET Aspire EF client integration includes a method named `Enrich\<DatabaseSystem\>DbContext`. These enrich context methods:

- Apply an EF settings object, if you passed one.
- Configure connection retry settings.
- Configure instrumentation, such as tracing and health checks.

> [!NOTE]
> You must have added a database context to the DI container before you call an enrich method.

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

### Use EF with dynamic connection strings in .NET Aspire

   > AJMTODO: Can you? Add an example. Base it on this: https://gavilan.blog/2020/07/09/configuring-entity-framework-core-with-dynamic-connection-strings-asp-net-core/

### Use EF interceptors in .NET Aspire

> AJMTODO: intro them here

> AJMTODO: Pivots

    ```csharp
    builder.Services.AddDbContext<ExampleDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("database"));
            options.AddInterceptors(new ExampleInterceptor());
		});
    ```
	
> AJMTODO: enrich method

### Use EF Core context factories in .NET Aspire

An EF database context is an object designed to be used for a single unit of work. For example, if you want to add a new customer to the database, you might need to add a row in the **Customers** table and a row in the **Addresses** table. You should create the EF context, add the new customer and address entities to it, call <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChanges*> or <xref:Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync*>, and then dispose the context.

In many types of web application, such as ASP.NET applications, each HTTP request closely corresponds to a single unit of work against the database. If your .NET Aspire microservice is an ASP.NET application or a similar web application, you can use the standard EF <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContext> method described above to create a context that is tied to the current HTTP request. Remember to call the .NET Aspire `Enrich\<DatabaseSystem\>DbContext` method to gain health checks, tracing, and other features. When you use this approach, the database context lifetime is tied to the web request. You don't have to call the `Dispose` method when the unit of work is complete. 

> AJMTODO: Review the method names and do xrefs for them.

Other types of application, such as ASP.NET Core Blazor, don't necessarily align each request with a unit of work, because they use dependency injection with a different service scope. In such apps, you may need to perform multiple units of work, each with a different database context, within a single HTTP request and response. To implement this approach, use a you can create a factory for database contexts, by calling the EF <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContextFactory*> method. This method partners well with the .NET Aspire `Enrich\<DatabaseSystem\>DbContext` methods:

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

Notice that the above code adds and enriches a *database context factory* in the DI container. When you retreive this from the container, you must add a line of code to create a *database context* from it:

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

## Use EF context pooling in .NET Aspire

In EF Core a database context is relatively quick to create and dispose of so most applications can set them up as needed without impacting their performance. However, the overhead is not zero so, if your microservice intensively creates contexts, you may observe suboptimal performance. In such situations, consider using a database context pool.

Database context pooling is a feature of EF Core. Database contexts are created as normal but, when you dispose of one, it isn't destroyed but reset and stored in a pool. The next time your code creates a context, the stored one is returned to avoid the extra overhead of creating a new one.

In a .NET Aspire consuming project, there are three ways to use context pooling:

- Use the .NET Aspire `Add\<DatabaseSystem\>DbContext` methods to create the database context. These methods create a context pool automatically.
- Call the EF <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContextPool> method instead of the EF <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContext> method.

    ```csharp
    builder.Services.AddDbContextPool<ExampleDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("database")
            ?? throw new InvalidOperationException("Connection string 'database' not found.")));
    ```

- Call the EF <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbPooledContextFactory> method instead of the EF <xref:Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContextFactory> method.

    ```csharp
    builder.Services.AddDbPooledContextFactory<ExampleDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("database")
            ?? throw new InvalidOperationException("Connection string 'database' not found.")));
    ```

Remember to enrich the database context after using the last two methods, as described above.



> AJMTODO: The rest of this must be moved.

## EF approaches

Model first, database-first, code-first

Migrations are for the last one only.


## External databases

Databases that are not running in Aspire-managed containers.

## Manage schemas in .NET Aspire projects with EF migrations

In a cloud-native solution, such as those .NET Aspire is designed to create, each microservice can use a separate database. As you evolve your microservice and add features, you must make matching changes to the database schema. For example, in your **Customers** microservice, if you build a feature to enable users to store the credit card details, you'll need new columns in your database tables. In EF, instead of manually implementing schema changes with Data Definition Language (DDL) queries, you can just add properties to the `Customer` class. Create an EF **migration** to propagate those properties to the database. Migrations can add, modify, or delete tables, columns, or relationships in the database schema. Because each migration is a generated code file, it can be included seamlessly in version control with the rest of your code.

> [!NOTE]
> This article discusses how to use EF migrations in .NET Aspire. For more information about migrations, see [Migrations Overview](/ef/core/managing-schemas/migrations)

How to use 'em in Aspire?


### Create a migrations service

Is this the best way to do it? Refer to the migration tutorial

> AJMTODO: Use the DbContent.Database.Migrate() method? This article says don't: https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli


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
