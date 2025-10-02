---
title: Aspire Azure PostgreSQL integration
description: Learn how to integrate Azure PostgreSQL with Aspire applications, using both hosting and client integrations.
ms.date: 03/31/2025
uid: dotnet/aspire/azure-postgresql-integration
---

# Aspire Azure PostgreSQL integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[Azure Database for PostgreSQL](/azure/postgresql/)—Flexible Server is a relational database service based on the open-source Postgres database engine. It's a fully managed database-as-a-service that can handle mission-critical workloads with predictable performance, security, high availability, and dynamic scalability. The Aspire Azure PostgreSQL integration provides a way to connect to existing Azure PostgreSQL databases, or create new instances from .NET with the [`docker.io/library/postgres` container image](https://hub.docker.com/_/postgres).

## Hosting integration

[!INCLUDE [postgresql-flexible-server](includes/postgresql-flexible-server.md)]

## Client integration

[!INCLUDE [azure-postgresql-client](includes/azure-postgresql-client.md)]

## See also

- [PostgreSQL docs](https://www.npgsql.org/doc/api/Npgsql.html)
- [Azure Database for PostgreSQL](/azure/postgresql/)
- [Aspire Azure PostgreSQL Entity Framework Core integration](azure-postgresql-entity-framework-integration.md)
- [Aspire PostgreSQL integration](postgresql-integration.md)
- [Aspire integrations](../fundamentals/integrations-overview.md)
- [Aspire GitHub repo](https://github.com/dotnet/aspire)
