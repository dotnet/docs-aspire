---
title: ".NET Aspire docs: What's new for June 2024"
description: "What's new in the .NET Aspire docs for June 2024."
ms.custom: June-2024
ms.date: 07/01/2024
---

# .NET Aspire docs: What's new for June 2024

Welcome to what's new in the .NET Aspire docs for June 2024. This article lists some of the major changes to docs during this period.

## Get started

### Updated articles

- [.NET Aspire overview](../get-started/aspire-overview.md) - purge app except app host
- [Add Node.js apps to a .NET Aspire project](../get-started/build-aspire-apps-with-nodejs.md)
  - purge app except app host
  - Update toc.yml - node.js app
- [Quickstart: Build your first .NET Aspire project](../get-started/build-your-first-aspire-app.md)
  - Update build-your-first-aspire-app.md - app -> project
  - Add details about trusting dev-cert
- [Tutorial: Add .NET Aspire to an existing .NET app](../get-started/add-aspire-existing-app.md)
  - purge app except app host
  - Remove .NET CLI command that was mistakenly included.
  - Add external HTTP and update corresponding content.

## Fundamentals

### Updated articles

- [.NET Aspire and launch profiles](../fundamentals/launch-profiles.md) - purge app except app host
- [.NET Aspire integrations overview](../fundamentals/integrations-overview.md) - purge app except app host
- [.NET Aspire dashboard overview](../fundamentals/dashboard/overview.md) - purge app except app host
- [.NET Aspire orchestration overview](../fundamentals/app-host-overview.md)
  - purge app except app host
  - Add missing code, and add flexible PostgreSQL
  - Add details about container runtime args.
- [.NET Aspire setup and tooling](../fundamentals/setup-tooling.md) - purge app except app host
- [.NET Aspire telemetry](../fundamentals/telemetry.md) - purge app except app host
- [Explore the .NET Aspire dashboard](../fundamentals/dashboard/explore.md)
  - purge app except app host
  - Update cookie auth details.
- [Health checks in .NET Aspire](../fundamentals/health-checks.md)
  - Fix OutputCache typos
  - Add info about required middleware to health checks.
- [Persist .NET Aspire project data using volumes](../fundamentals/persist-data-volumes.md) - purge app except app host
- [Security considerations for running the .NET Aspire dashboard](../fundamentals/dashboard/security-considerations.md) - Update cookie auth details.
- [Testing .NET Aspire projects](../fundamentals/testing.md) - purge app except app host

## Storage

### Updated articles

- [.NET Aspire Azure Blob Storage integration](../storage/azure-storage-blobs-integration.md) - Add missing code, and add flexible PostgreSQL
- [.NET Aspire Azure Data Tables integration](../storage/azure-storage-tables-integration.md) - Add missing code, and add flexible PostgreSQL
- [.NET Aspire Azure Queue Storage integration](../storage/azure-storage-queues-integration.md) - Add missing code, and add flexible PostgreSQL
- [Tutorial: Connect an ASP.NET Core app to .NET Aspire storage integrations](../storage/azure-storage-integrations.md)
  - purge app except app host
  - Minor clean up

## Database

### Updated articles

- [.NET Aspire Azure Cosmos DB integration](../database/azure-cosmos-db-integration.md) - Add missing code, and add flexible PostgreSQL
- [.NET Aspire Microsoft Entity Framework Core Cosmos DB integration](../database/azure-cosmos-db-entity-framework-integration.md) - Add missing code, and add flexible PostgreSQL
- [.NET Aspire MongoDB database integration](../database/mongodb-integration.md) - Add missing code, and add flexible PostgreSQL
- [.NET Aspire MySQL database integration](../database/mysql-integration.md)
  - Add missing code, and add flexible PostgreSQL
  - Adds parameters to app host usage examples, where applicable
- [.NET Aspire Oracle Entity Framework Component](../database/oracle-entity-framework-integration.md)
  - Add missing code, and add flexible PostgreSQL
  - Adds parameters to app host usage examples, where applicable
- [.NET Aspire Pomelo MySQL Entity Framework Component](../database/mysql-entity-framework-integration.md) - Add missing code, and add flexible PostgreSQL
- [.NET Aspire PostgreSQL integration](../database/postgresql-integration.md)
  - Add missing code, and add flexible PostgreSQL
  - Adds parameters to app host usage examples, where applicable
- [.NET Aspire PostgreSQL Entity Framework Core integration](../database/postgresql-entity-framework-integration.md)
  - Add missing code, and add flexible PostgreSQL
  - Adds parameters to app host usage examples, where applicable
- [.NET Aspire Qdrant integration](../database/qdrant-integration.md)
  - Add missing code, and add flexible PostgreSQL
  - Adds parameters to app host usage examples, where applicable
- [.NET Aspire SQL Server integration](../database/sql-server-integration.md)
  - Add missing code, and add flexible PostgreSQL
  - Adds parameters to app host usage examples, where applicable
- [.NET Aspire SqlServer Entity Framework Core integration](../database/sql-server-entity-framework-integration.md) - Add missing code, and add flexible PostgreSQL
- [Apply Entity Framework Core migrations in .NET Aspire](../database/ef-core-migrations.md) - purge app except app host
- [Seed data in a database using .NET Aspire](../database/seed-database-data.md) - purge app except app host
- [Tutorial: Connect an ASP.NET Core app to SQL Server using .NET Aspire and Entity Framework Core](../database/sql-server-integrations.md)
  - Fix code and content.
  - purge app except app host
- [Tutorial: Deploy a .NET Aspire project with a SQL Server Database to Azure](../database/sql-server-integration-deployment.md) - purge app except app host

## Messaging

### Updated articles

- [.NET Aspire Apache Kafka integration](../messaging/kafka-integration.md) - Add missing code, and add flexible PostgreSQL
- [.NET Aspire Azure Event Hubs integration](../messaging/azure-event-hubs-integration.md) - Add missing code, and add flexible PostgreSQL
- [.NET Aspire Azure Service Bus integration](../messaging/azure-service-bus-integration.md)
  - Add missing code, and add flexible PostgreSQL
  - Correct Service Bus tracing details.
- [.NET Aspire NATS integration](../messaging/nats-integration.md) - Add missing code, and add flexible PostgreSQL
- [.NET Aspire RabbitMQ integration](../messaging/rabbitmq-client-integration.md)
  - Fix RabbitMQ docs usage of AddRabbitMQ for connection string based connection
  - Add missing code, and add flexible PostgreSQL
  - Adds parameters to app host usage examples, where applicable
- [Tutorial: Use .NET Aspire messaging integrations in ASP.NET Core](../messaging/messaging-integrations.md)
  - purge app except app host
  - Add VS Code bits and fix errors

## Caching

### New articles

- [Stack Exchange Redis caching overview](../caching/stackexchange-redis-caching-overview.md)

### Updated articles

- [.NET Aspire Stack Exchange Redis integration](../caching/stackexchange-redis-integration.md)
  - Add an overview of Stack Exchange Redis caching
  - Add missing code, and add flexible PostgreSQL
- [.NET Aspire Stack Exchange Redis distributed caching integration](../caching/stackexchange-redis-distributed-caching-integration.md)
  - Matched the App host Redis reference to the consuming project
  - Add an overview of Stack Exchange Redis caching
  - Add missing code, and add flexible PostgreSQL
- [.NET Aspire Stack Exchange Redis output caching integration](../caching/stackexchange-redis-output-caching-integration.md)
  - Add an overview of Stack Exchange Redis caching
  - Add missing code, and add flexible PostgreSQL
- [Stack Exchange Redis caching overview](../caching/stackexchange-redis-caching-overview.md)
  - purge app except app host
  - Add an overview of Stack Exchange Redis caching
- [Tutorial: Deploy a .NET Aspire project with a Redis Cache to Azure](../caching/caching-integrations-deployment.md)
  - purge app except app host
  - Add an overview of Stack Exchange Redis caching
- [Tutorial: Implement caching with .NET Aspire integrations](../caching/caching-integrations.md)
  - Add an overview of Stack Exchange Redis caching
  - Update and correct hosting bits.

## Security

### Updated articles

- [.NET Aspire Azure Key Vault integration](../security/azure-security-key-vault-integration.md)
  - Add missing code, and add flexible PostgreSQL
  - Update KeyVault usage documentation

## Deployment

### New articles

- [Use custom Bicep templates](../deployment/azure/custom-bicep-templates.md)

### Updated articles

- [.NET Aspire deployments](../deployment/overview.md) - purge app except app host
- [.NET Aspire manifest format for deployment tool builders](../deployment/manifest-format.md) - purge app except app host
- [Deploy a .NET Aspire project to Azure Container Apps](../deployment/azure/aca-deployment.md)
  - purge app except app host
  - Add an include about resource naming.
- [Deploy a .NET Aspire project to Azure Container Apps using the Azure Developer CLI (in-depth guide)](../deployment/azure/aca-deployment-azd-in-depth.md)
  - purge app except app host
  - Add an include about resource naming.
- [Deploy a .NET Aspire project to Azure Container Apps using Visual Studio](../deployment/azure/aca-deployment-visual-studio.md)
  - purge app except app host
  - Add an include about resource naming.
- [Local Azure provisioning](../deployment/azure/local-provisioning.md) - Remove invalid trailing comma in config example
- [Tutorial: Deploy a .NET Aspire project using the Azure Developer CLI and GitHub Actions](../deployment/azure/aca-deployment-github-actions.md)
  - purge app except app host
  - Add an include about resource naming.
- [Use Application Insights for .NET Aspire telemetry](../deployment/azure/application-insights.md) - purge app except app host
- [Use custom Bicep templates](../deployment/azure/custom-bicep-templates.md)
  - purge app except app host
  - Add new article: Use custom Bicep templates

## Reference

### Updated articles

- [Frequently asked questions about .NET Aspire](../reference/aspire-faq.yml)
  - purge app except app host
  - update GA on aspire :)

## Community contributors

The following people contributed to the .NET Aspire docs during this period. Thank you! Learn how to contribute by following the links under "Get involved" in the [what's new landing page](index.yml).

- [BaileyHewitt](https://github.com/BaileyHewitt) -  ![1 pull requests.](https://img.shields.io/badge/Merged%20Pull%20Requests-1-green)
- [willibrandon](https://github.com/willibrandon) - Brandon Williams ![1 pull requests.](https://img.shields.io/badge/Merged%20Pull%20Requests-1-green)
