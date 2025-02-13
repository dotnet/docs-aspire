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

In EF, a [database context](/ef/core/dbcontext-configuration/) is a class used to interact with the database. Database contexts inherit from the `DbContext` class. They provide access to the database through properties of type `DbSet<T>`, where each `DbSet` represents a table or collection of entities in the database. The context also manages database connections, tracks changes to entities, and handles operations like saving data and executing queries.

The EF client integrations each include an extension method named `Add\<DatabaseSystem\>DbContext`, where **\<DatabaseSystem\>** is a string identifying the database product you are using. For example, for the SQL Server client extension, the method is named <xref:Microsoft.Extensions.Hosting.AspireSqlServerEFCoreSqlClientExtensions.AddSqlServerDbContext%2A> and for the PostgreSQL client extension, the method is named <xref:Microsoft.Extensions.Hosting.AspireEFPostgreSqlExtensions.AddNpgsqlDbContext%2A>.

These .NET Aspire add context methods:

- Check that a database context of the same type is not already registered in the Dependency Injection (DI) container.
- Use the connection name you pass to the method to get the connection string from the application builder. This connection name must match the name used when adding the corresponding resource to the app host project.
- Apply an EF settings object, if you passed one.
- Add a database context pool to the DI container.
- Configure instrumentation, such as tracing and health checks.

Use these .NET Aspire add context methods when you want a simple way to create the database context and don't need to implement advanced EF techniques.

> AJMTODO: Link to individual articles?

## Enrich an Entity Framework database context

Alternatively, you create a database context and add it to the DI container using the standard entity `AddDbContext` method, as used in non-.NET Aspire projects:

> AJMTODO: pivots here?

```csharp
builder.Services.AddDbContext<ExampleDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("database")
        ?? throw new InvalidOperationException("Connection string 'database' not found.")));
```

You have more flexibility when you create the database context in this way, for example:

- You can reuse existing configuration code for the database context without rewriting it for .NET Aspire.
- You can use Entity Framework Core interceptors to modify database operations.
- You can choose not to use Entity Framework Core context pooling, which may perform better in some circumstances.


By default, a database context create this way doesn't include .NET Aspire features,  such as telemetry and health checks. To add those features, each .NET Aspire EF client integration includes a method named `Enrich\<DatabaseSystem\>DbContext`. These enrich context methods:

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

> AJMTODO: Example with interceptors

## Use Entity Framework context factories in .NET Aspire

Is it possible?

Is it good?

Code

## Use Entity Framework context pooling in .NET Aspire

Should this section be part of the previous one?

Is it possible?

Is it good?

Code

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
