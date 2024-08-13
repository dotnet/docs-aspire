---
title: The connection string is missing
description: Learn how to troubleshoot the error "ConnectionString is missing" during execution of your app.
ms.date: 07/03/2024
---

# Connection string is missing

In .NET Aspire, code identifies resources with an arbitrary string, such as "database". Code that is consuming the resource elsewhere must use the same string or it will fail to correctly configure their relationships.

## Symptoms

When your app accesses a service that needs one of the integrations in your app, it may fail with an exception similar to the following:

> "InvalidOperationException: ConnectionString is missing."

## Possible solutions

Verify that the name of the resource, for instance a database resource, is the same in the AppHost and the Service that fails.

For example, if the AppHost defines a PostgreSQL resource with the name `db1` like this:

```csharp
var db1 = builder.AddPostgres("pg1").AddDatabase("db1");
```

Then the service needs to resolve the resource with the same name `db1`.

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddNpgsqlDbContext<MyDb1Context>("db1");
```

Any other value than the one provided in the AppHost will result in the exception message described above.
