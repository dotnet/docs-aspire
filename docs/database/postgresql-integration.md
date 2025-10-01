---
title: Aspire PostgreSQL integration
description: Learn how to integrate PostgreSQL with Aspire applications, using both hosting and client integrations.
ms.date: 07/22/2025
uid: database/postgresql-integration
ms.custom: sfi-ropc-nochange
---

# Aspire PostgreSQL integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[PostgreSQL](https://www.postgresql.org/) is a powerful, open source object-relational database system with many years of active development that has earned it a strong reputation for reliability, feature robustness, and performance. The Aspire PostgreSQL integration provides a way to connect to existing PostgreSQL databases, or create new instances from .NET with the [`docker.io/library/postgres` container image](https://hub.docker.com/_/postgres).

## Hosting integration

[!INCLUDE [postgresql-app-host](includes/postgresql-app-host.md)]

### Hosting integration health checks

The PostgreSQL hosting integration automatically adds a health check for the PostgreSQL server resource. The health check verifies that the PostgreSQL server is running and that a connection can be established to it.

The hosting integration relies on the [📦 AspNetCore.HealthChecks.Npgsql](https://www.nuget.org/packages/AspNetCore.HealthChecks.Npgsql) NuGet package.

### Using with non-.NET applications

The PostgreSQL hosting integration can be used with any application technology, not just .NET applications. When you use <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> to reference a PostgreSQL resource, connection information is automatically injected as environment variables into the referencing application.

For applications that don't use the [client integration](#client-integration), you can access the connection information through environment variables. Here's an example of how to configure environment variables for a non-.NET application:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent);

var database = postgres.AddDatabase("myDatabase");

// Example: Configure a non-.NET application with PostgreSQL access
var app = builder.AddExecutable("my-app", "node", "app.js", ".")
    .WithReference(database) // Provides ConnectionStrings__myDatabase
    .WithEnvironment(context =>
    {
        // Additional individual connection details as environment variables
        context.EnvironmentVariables["POSTGRES_HOST"] = postgres.Resource.PrimaryEndpoint.Property(EndpointProperty.Host);
        context.EnvironmentVariables["POSTGRES_PORT"] = postgres.Resource.PrimaryEndpoint.Property(EndpointProperty.Port);
        context.EnvironmentVariables["POSTGRES_USER"] = postgres.Resource.UserNameParameter;
        context.EnvironmentVariables["POSTGRES_PASSWORD"] = postgres.Resource.PasswordParameter;
        context.EnvironmentVariables["POSTGRES_DATABASE"] = database.Resource.DatabaseName;
    });

builder.Build().Run();
```

This configuration provides the non-.NET application with several environment variables:

- `ConnectionStrings__myDatabase`: The complete PostgreSQL connection string
- `POSTGRES_HOST`: The hostname/IP address of the PostgreSQL server
- `POSTGRES_PORT`: The port number the PostgreSQL server is listening on  
- `POSTGRES_USER`: The database username
- `POSTGRES_PASSWORD`: The dynamically generated password
- `POSTGRES_DATABASE`: The name of the database

Your non-.NET application can then read these environment variables to connect to the PostgreSQL database using the appropriate database driver for that technology (for example, `psycopg2` for Python, `pg` for Node.js, or `pq` for Go).

## Client integration

[!INCLUDE [postgresql-client](includes/postgresql-client.md)]

## See also

- [PostgreSQL docs](https://www.npgsql.org/doc/api/Npgsql.html)
- [Aspire Azure PostgreSQL integration](azure-postgresql-integration.md)
- [Aspire integrations](../fundamentals/integrations-overview.md)
- [Aspire GitHub repo](https://github.com/dotnet/aspire)
