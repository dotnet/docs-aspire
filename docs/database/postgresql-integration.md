---
title: .NET Aspire PostgreSQL integration
description: Learn how to integrate PostgreSQL with .NET Aspire applications, using both hosting and client integrations.
ms.date: 02/07/2025
uid: database/postgresql-integration
---

# .NET Aspire PostgreSQL integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[PostgreSQL](https://www.postgresql.org/) is a powerful, open source object-relational database system with many years of active development that has earned it a strong reputation for reliability, feature robustness, and performance. The .NET Aspire PostgreSQL integration provides a way to connect to existing PostgreSQL databases, or create new instances from .NET with the [`docker.io/library/postgres` container image](https://hub.docker.com/_/postgres).

## Hosting integration

[!INCLUDE [postgresql-app-host](includes/postgresql-app-host.md)]

### Hosting integration health checks

The PostgreSQL hosting integration automatically adds a health check for the PostgreSQL server resource. The health check verifies that the PostgreSQL server is running and that a connection can be established to it.

The hosting integration relies on the [📦 AspNetCore.HealthChecks.Npgsql](https://www.nuget.org/packages/AspNetCore.HealthChecks.Npgsql) NuGet package.

## Client integration

[!INCLUDE [postgresql-client](includes/postgresql-client.md)]

## See also

- [PostgreSQL docs](https://www.npgsql.org/doc/api/Npgsql.html)
- [.NET Aspire Azure PostgreSQL integration](azure-postgresql-integration.md)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
