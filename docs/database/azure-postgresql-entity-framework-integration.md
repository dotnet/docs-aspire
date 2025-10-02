---
title: Aspire Azure PostgreSQL Entity Framework Core integration
description: Learn how to integrate Azure PostgreSQL with Aspire applications, using both hosting and Entity Framework Core client integrations.
ms.date: 03/31/2025
uid: dotnet/aspire/azure-postgresql-entity-framework-integration
---

# Aspire Azure PostgreSQL Entity Framework Core integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[Azure Database for PostgreSQL](/azure/postgresql/)—Flexible Server is a relational database service based on the open-source Postgres database engine. It's a fully managed database-as-a-service that can handle mission-critical workloads with predictable performance, security, high availability, and dynamic scalability. The Aspire Azure PostgreSQL integration provides a way to connect to existing Azure PostgreSQL databases, or create new instances from .NET with the [`docker.io/library/postgres` container image](https://hub.docker.com/_/postgres).

## Hosting integration

[!INCLUDE [postgresql-flexible-server](includes/postgresql-flexible-server.md)]

## Client integration

[!INCLUDE [azure-postgresql-ef-client](includes/azure-postgresql-ef-client.md)]

## See also

- [PostgreSQL docs](https://www.npgsql.org/doc/api/Npgsql.html)
- [Azure Database for PostgreSQL](/azure/postgresql/)
- [Aspire PostgreSQL Entity Framework Core integration](postgresql-entity-framework-integration.md)
- [Aspire integrations](../fundamentals/integrations-overview.md)
- [Aspire GitHub repo](https://github.com/dotnet/aspire)
