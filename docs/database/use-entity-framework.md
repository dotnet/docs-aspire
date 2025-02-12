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

## Install the client integration

> Either repeat this here or link to the individual articles

## Use a .NET Aspire database context

In EF, a [database context](/ef/core/dbcontext-configuration/) is a class used to interact with the database. Database contexts inherit from the `DbContext` class. They provide access to the database through properties of type `DbSet<T>`, where each `DbSet` represents a table or collection of entities in the database. The context also manages database connections, tracks changes to entities, and handles operations like saving data and executing queries.


What does the aspire AddXDbContext method do?

Why is this good?

Link to individual articles

## Enrich an Entity Framework database context

Use the standard EF way to create a context and add it to the DI container

Use the Enrich method to add Aspire stuff to it

Why do it this way?

Example with interceptors

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
