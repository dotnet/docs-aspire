---
title: .NET Aspire integrations overview
description: Explore the fundamental concepts of .NET Aspire integrations and learn how to integrate them into your apps.
ms.date: 09/13/2024
ms.topic: conceptual
---

# .NET Aspire integrations overview

.NET Aspire integrations are a curated suite of NuGet packages selected to facilitate the integration of cloud-native applications with prominent services and platforms, such as Redis and PostgreSQL. Each integration furnishes essential cloud-native functionalities through either automatic provisioning or standardized configuration patterns.

> [!TIP]
> Always strive to use the latest version of .NET Aspire integrations to take advantage of the latest features, improvements, and security updates.

## Integration responsibilities

There are two types of integrations in .NET Aspire, each with a different responsibility. One type represents resources within the app host project—these are known as [hosting integrations](#hosting-integrations). The other type of integration represents client libraries that connect to the aforementioned resources, and they're known as [client integrations](#client-integrations).

### Hosting integrations

Hosting integrations configure applications by provisioning resources (like containers or cloud resources) or pointing to existing instances (such as a local SQL server). These packages model various services, platforms, or capabilities, including caches, databases, logging, storage, and messaging systems.

Hosting integrations extend the <xref:Aspire.Hosting.IDistributedApplicationBuilder> interface, enabling the _app host_ project to express resources within its _app model_. The official [hosting integration NuGet packages](https://www.nuget.org/packages?q=owner%3A+aspire+tags%3A+aspire+hosting+integration&includeComputedFrameworks=true&prerel=true&sortby=relevance) are tagged with `aspire`, `integration`, and `hosting`.

For information on creating a custom hosting integration, see [Create custom .NET Aspire hosting integration](../extensibility/custom-hosting-integration.md).

### Client integrations

Client integrations define configuration schema, wire up client libraries to dependency injection (DI), and add health checks, resiliency, and telemetry where applicable. These packages configure existing client libraries to connect to hosting integrations. They extend the <xref:Microsoft.Extensions.DependencyInjection.IServiceCollection> interface allowing client-consuming projects, such as your web app or API, to use the connected resource. The official [client integration NuGet packages](https://www.nuget.org/packages?q=owner%3A+aspire+tags%3A+aspire+client+integration&includeComputedFrameworks=true&prerel=true&sortby=relevance) are tagged with `aspire`, `integration`, and `client`.

For more information on creating a custom client integration, see [Create custom .NET Aspire client integrations](../extensibility/custom-client-integration.md).

### Relationship between hosting and client integrations

Hosting and client integrations are best when used together, but are **not** coupled and may be used separately. Some hosting integrations may not have a corresponding client integration. Configuration is what makes the hosting integration work with the client integration.

For example, if you want to use [Redis for caching](../caching/stackexchange-redis-caching-overview.md) in your .NET Aspire solution, you would use the `Aspire.Hosting.Redis` package to model the Redis resource. Then, you can connect to the Redis resource using the `Aspire.StackExchange.Redis` client integration.

## Available integrations

The following section details the available .NET Aspire integrations, links to their respective NuGet packages, and provides a brief description of each integration.

<!-- markdownlint-disable MD033 MD045 -->
| Integration | NuGet packages | Description |
|--|--|--|
| [Apache Kafka](../messaging/kafka-integration.md) <br/> <img src="media/icons/Aspire-logo-256.png" alt=".NET Aspire logo." role="presentation" width="64" data-linktype="relative-path"> | - **Hosting**: [Aspire.Hosting.Kafka](https://www.nuget.org/packages/Aspire.Hosting.Kafka)<br>- **Client**: [Aspire.Confluent.Kafka](https://www.nuget.org/packages/Aspire.Confluent.Kafka) | A library for producing and consuming messages from an [Apache Kafka](https://kafka.apache.org/) broker. |
| [Azure AI OpenAI](../azureai/azureai-openai-integration.md) <br/> <img src="media/icons/AzureOpenAI_256x.png" alt="Azure OpenAI logo." role="presentation" width="64" data-linktype="relative-path"> | - **Hosting**: [Aspire.Hosting.Azure.CognitiveServices](https://www.nuget.org/packages/Aspire.Hosting.Azure.CognitiveServices)<br>- **Client**: [Aspire.Azure.AI.OpenAI](https://www.nuget.org/packages/Aspire.Azure.AI.OpenAI) | A library for accessing [Azure AI OpenAI](/azure/ai-services/openai/overview) or OpenAI functionality. |
| [Azure AI Search](../azureai/azureai-search-document-integration.md) <br/> <img src="media/icons/AzureSearch_256x.png" alt="Azure AI Search Documents logo." role="presentation" width="64" data-linktype="relative-path"> | - **Hosting**: [Aspire.Hosting.Azure.Search](https://www.nuget.org/packages/Aspire.Hosting.Azure.Search)<br>- **Client**: [Aspire.Azure.Search.Documents](https://www.nuget.org/packages/Aspire.Azure.Search.Documents) | A library for accessing [Azure AI Search](/azure/search/search-what-is-azure-search) functionality. |
| [Azure Blob Storage](../storage/azure-storage-blobs-integration.md) <br/> <img src="media/icons/AzureStorageContainer_256x.png" alt="Azure Blog Storage logo."  role="presentation" width="64" data-linktype="relative-path"> | - **Hosting**: [Aspire.Hosting.Azure.Storage](https://www.nuget.org/packages/Aspire.Hosting.Azure.Storage)<br>- **Client**: [Aspire.Azure.Storage.Blobs](https://www.nuget.org/packages/Aspire.Azure.Storage.Blobs) | A library for accessing [Azure Blob Storage](/azure/storage/blobs/storage-blobs-introduction). |
| [Azure Cosmos DB Entity Framework Core](../database/azure-cosmos-db-entity-framework-integration.md) <br/> <img src="media/icons/AzureCosmosDB_256x.png" alt="Azure Cosmos DB EF logo." role="presentation" width="64" data-linktype="relative-path"> | - **Hosting**: [Aspire.Hosting.Azure.CosmosDB](https://www.nuget.org/packages/Aspire.Hosting.Azure.CosmosDB)<br>- **Client**: [Aspire.Microsoft.EntityFrameworkCore.Cosmos](https://www.nuget.org/packages/Aspire.Microsoft.EntityFrameworkCore.Cosmos) | A library for accessing Azure Cosmos DB databases with [Entity Framework Core](/ef/core/providers/cosmos/). |
| [Azure Cosmos DB](../database/azure-cosmos-db-integration.md) <br/> <img src="media/icons/AzureCosmosDB_256x.png" alt="Azure Cosmos DB logo." role="presentation" width="64" data-linktype="relative-path"> | - **Hosting**: [Aspire.Hosting.Azure.CosmosDB](https://www.nuget.org/packages/Aspire.Hosting.Azure.CosmosDB)<br>- **Client**: [Aspire.Microsoft.Azure.Cosmos](https://www.nuget.org/packages/Aspire.Microsoft.Azure.Cosmos) | A library for accessing [Azure Cosmos DB](/azure/cosmos-db/introduction) databases. |
| [Azure Event Hubs](../messaging/azure-event-hubs-integration.md) <br/> <img src="media/icons/AzureEventHubs_256x.png" alt="Azure Event Hubs logo." role="presentation" width="64" data-linktype="relative-path"> | - **Hosting**: [Aspire.Hosting.Azure.EventHubs](https://www.nuget.org/packages/Aspire.Hosting.Azure.EventHubs)<br>- **Client**: [Aspire.Azure.Messaging.EventHubs](https://www.nuget.org/packages/Aspire.Azure.Messaging.EventHubs) | A library for accessing [Azure Event Hubs](/azure/event-hubs/event-hubs-about). |
| [Azure Key Vault](../security/azure-security-key-vault-integration.md) <br/> <img src="media/icons/AzureKeyVault_256x.png" alt="Azure Key Vault logo." role="presentation" width="64" data-linktype="relative-path"> | - **Hosting**: [Aspire.Hosting.Azure.KeyVault](https://www.nuget.org/packages/Aspire.Hosting.Azure.KeyVault)<br>- **Client**: [Aspire.Azure.Security.KeyVault](https://www.nuget.org/packages/Aspire.Azure.Security.KeyVault) | A library for accessing [Azure Key Vault](/azure/key-vault/general/overview). |
| [Azure SignalR Service](../real-time/azure-signalr-scenario.md) <br/> <img src="media/icons/AzureSignalR_256x.png" alt="Azure Service Bus logo." role="presentation" width="64" data-linktype="relative-path"> | - **Hosting**: [Aspire.Hosting.Azure.SignalR](https://www.nuget.org/packages/Aspire.Hosting.Azure.SignalR)<br>- **Client**: [Microsoft.Azure.SignalR](https://www.nuget.org/packages/Microsoft.Azure.SignalR) | A library for accessing [Azure SignalR Service](/azure/azure-signalr/signalr-overview). |
| [Azure Service Bus](../messaging/azure-service-bus-integration.md) <br/> <img src="media/icons/AzureServiceBus_256x.png" alt="Azure Service Bus logo." role="presentation" width="64" data-linktype="relative-path"> | - **Hosting**: [Aspire.Hosting.Azure.ServiceBus](https://www.nuget.org/packages/Aspire.Hosting.Azure.ServiceBus)<br>- **Client**: [Aspire.Azure.Messaging.ServiceBus](https://www.nuget.org/packages/Aspire.Azure.Messaging.ServiceBus) | A library for accessing [Azure Service Bus](/azure/service-bus-messaging/service-bus-messaging-overview). |
| [Azure Storage Queues](../storage/azure-storage-queues-integration.md) <br/> <img src="media/icons/AzureStorageQueue_256x.png" alt="Azure Storage Queues logo." role="presentation" width="64" data-linktype="relative-path"> | - **Hosting**: [Aspire.Hosting.Azure.Storage](https://www.nuget.org/packages/Aspire.Hosting.Azure.Storage)<br>- **Client**: [Aspire.Azure.Storage.Queues](https://www.nuget.org/packages/Aspire.Azure.Storage.Queues) | A library for accessing [Azure Storage Queues](/azure/storage/queues/storage-queues-introduction). |
| [Azure Table Storage](../storage/azure-storage-tables-integration.md) <br/> <img src="media/icons/AzureTable_256x.png" alt="Azure Table Storage logo." role="presentation" width="64" data-linktype="relative-path"> | - **Hosting**: [Aspire.Hosting.Azure.Storage](https://www.nuget.org/packages/Aspire.Hosting.Azure.Storage)<br>- **Client**: [Aspire.Azure.Data.Tables](https://www.nuget.org/packages/Aspire.Azure.Data.Tables) | A library for accessing the [Azure Table](/azure/storage/tables/table-storage-overview) service. |
| [Azure Web PubSub](../messaging/azure-web-pubsub-integration.md) <br/> <img src="media/icons/AzureWebPubSub_256x.png" alt="Azure Web PubSub logo." role="presentation" width="64" data-linktype="relative-path"> | - **Hosting**: [Aspire.Hosting.Azure.WebPubSub](https://www.nuget.org/packages/Aspire.Hosting.Azure.WebPubSub)<br>- **Client**: [Aspire.Azure.Messaging.WebPubSub](https://www.nuget.org/packages/Aspire.Azure.Messaging.WebPubSub) | A library for accessing the [Azure Web PubSub](/azure/azure-web-pubsub/) service. |
| [Elasticsearch](../search/elasticsearch-integration.md) <br/> <img src="media/icons/Elastic_logo_256x.png" alt="Elasticsearch logo." role="presentation" width="64" data-linktype="relative-path"> | - **Hosting**: [Aspire.Hosting.Elasticsearch](https://www.nuget.org/packages/Aspire.Hosting.Elasticsearch)<br>- **Client**: [Aspire.Elastic.Clients.Elasticsearch](https://www.nuget.org/packages/Aspire.Elastic.Clients.Elasticsearch) | A library for accessing [Elasticsearch](https://www.elastic.co/guide/en/elasticsearch/client/index.html) databases. |
| [Keycloak](../authentication/keycloak-integration.md) <br/> <img src="media/icons/Aspire-logo-256.png" alt=".NET Aspire logo." role="presentation" width="64" data-linktype="relative-path"> | - **Hosting**: [Aspire.Hosting.Keycloak](https://www.nuget.org/packages/Aspire.Hosting.Keycloak)<br>- **Client**: [Aspire.Keycloak.Authentication](https://www.nuget.org/packages/Aspire.Keycloak.Authentication) | A library for accessing [Keycloak](https://www.keycloak.org/docs/latest/server_admin/index.html) authentication. |
| [Milvus](../database/milvus-integration.md) <br/> <img src="media/icons/Milvus_256x.png" alt="Milvus logo." role="presentation" width="64" data-linktype="relative-path"> | - **Hosting**: [Aspire.Hosting.Milvus](https://www.nuget.org/packages/Aspire.Hosting.Milvus)<br>- **Client**: [Aspire.Milvus.Client](https://www.nuget.org/packages/Aspire.Milvus.Client) | A library for accessing [Milvus](https://milvus.io/) databases. |
| [MongoDB Driver](../database/mongodb-integration.md) <br/> <img src="media/icons/MongoDB_256px.png" alt="MongoDB logo." role="presentation" width="64" data-linktype="relative-path"> | - **Hosting**: [Aspire.Hosting.MongoDB](https://www.nuget.org/packages/Aspire.Hosting.MongoDB)<br>- **Client**: [Aspire.MongoDB.Driver](https://www.nuget.org/packages/Aspire.MongoDB.Driver) | A library for accessing [MongoDB](https://www.mongodb.com/docs) databases. |
| [MySqlConnector](../database/mysql-integration.md) <br/> <img src="media/icons/mysqlconnector_logo.png" alt="MySqlConnector logo." role="presentation" width="64" data-linktype="relative-path"> | - **Hosting**: [Aspire.Hosting.MySql](https://www.nuget.org/packages/Aspire.Hosting.MySql)<br>- **Client**: [Aspire.MySqlConnector](https://www.nuget.org/packages/Aspire.MySqlConnector) | A library for accessing [MySqlConnector](https://mysqlconnector.net/) databases. |
| [NATS](../messaging/nats-integration.md) <br/> <img src="media/icons/nats-icon.png" alt="NATS logo." role="presentation" width="64" data-linktype="relative-path"> | - **Hosting**: [Aspire.Hosting.Nats](https://www.nuget.org/packages/Aspire.Hosting.Nats)<br>- **Client**: [Aspire.NATS.Net](https://www.nuget.org/packages/Aspire.NATS.Net) | A library for accessing [NATS](https://nats.io/) messaging. |
| [Oracle Entity Framework Core](../database/oracle-entity-framework-integration.md) <br/> <img src="media/icons/Aspire-logo-256.png" alt=".NET Aspire logo." role="presentation" width="64" data-linktype="relative-path"> | - **Hosting**: [Aspire.Hosting.Oracle](https://www.nuget.org/packages/Aspire.Hosting.Oracle)<br>- **Client**: [Aspire.Oracle.EntityFrameworkCore](https://www.nuget.org/packages/Aspire.Oracle.EntityFrameworkCore) | A library for accessing Oracle databases with [Entity Framework Core](/ef/core). |
| [Pomelo MySQL Entity Framework Core](../database/mysql-entity-framework-integration.md) <br/> <img src="media/icons/Aspire-logo-256.png" alt=".NET Aspire logo." role="presentation" width="64" data-linktype="relative-path"> | - **Hosting**: [Aspire.Hosting.MySql](https://www.nuget.org/packages/Aspire.Hosting.MySql)<br>- **Client**: [Aspire.Pomelo.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Aspire.Pomelo.EntityFrameworkCore.MySql) | A library for accessing MySql databases with [Entity Framework Core](/ef/core). |
| [PostgreSQL Entity Framework Core](../database/postgresql-entity-framework-integration.md) <br/> <img src="media/icons/PostgreSQL_logo.3colors.256x.png" alt="PostgreSQL logo." role="presentation" width="64" data-linktype="relative-path"> | - **Hosting**: [Aspire.Hosting.PostgreSQL](https://www.nuget.org/packages/Aspire.Hosting.PostgreSQL)<br>- **Client**: [Aspire.Npgsql.EntityFrameworkCore.PostgreSQL](https://www.nuget.org/packages/Aspire.Npgsql.EntityFrameworkCore.PostgreSQL) | A library for accessing PostgreSQL databases using [Entity Framework Core](https://www.npgsql.org/efcore/index.html). |
| [PostgreSQL](../database/postgresql-integration.md) <br/> <img src="media/icons/PostgreSQL_logo.3colors.256x.png" alt="PostgreSQL logo." role="presentation" width="64" data-linktype="relative-path"> | - **Hosting**: [Aspire.Hosting.PostgreSQL](https://www.nuget.org/packages/Aspire.Hosting.PostgreSQL)<br>- **Client**: [Aspire.Npgsql](https://www.nuget.org/packages/Aspire.Npgsql) | A library for accessing [PostgreSQL](https://www.npgsql.org/doc/index.html) databases. |
| [Qdrant](../database/qdrant-integration.md) <br/> <img src="media/icons/QdrantLogo_256x.png" alt="Qdrant logo." role="presentation" width="64" data-linktype="relative-path"> |  - **Hosting**: [Aspire.Hosting.Qdrant](https://www.nuget.org/packages/Aspire.Hosting.Qdrant)<br>- **Client**: [Aspire.Qdrant.Client](https://www.nuget.org/packages/Aspire.Qdrant.Client) | A library for accessing [Qdrant](https://qdrant.tech/) databases. |
| [RabbitMQ](../messaging/rabbitmq-client-integration.md) <br/> <img src="media/icons/Aspire-logo-256.png" alt=".NET Aspire logo." role="presentation" width="64" data-linktype="relative-path"> |  - **Hosting**: [Aspire.Hosting.RabbitMQ](https://www.nuget.org/packages/Aspire.Hosting.RabbitMQ)<br>- **Client**: [Aspire.RabbitMQ.Client](https://www.nuget.org/packages/Aspire.RabbitMQ.Client) | A library for accessing [RabbitMQ](https://www.rabbitmq.com/dotnet.html). |
| [Redis Distributed Caching](../caching/stackexchange-redis-distributed-caching-integration.md) <br/> <img src="media/icons/redis-cube-red_white-rgb.png" alt="Redis logo." role="presentation" width="64" data-linktype="relative-path"> | - **Hosting**: [Aspire.Hosting.Redis](https://www.nuget.org/packages/Aspire.Hosting.Redis), [Aspire.Hosting.Garnet](https://www.nuget.org/packages/Aspire.Hosting.Garnet), or [Aspire.Hosting.Valkey](https://www.nuget.org/packages/Aspire.Hosting.RValkeyedis)<br>- **Client**: [Aspire.StackExchange.Redis.DistributedCaching](https://www.nuget.org/packages/Aspire.StackExchange.Redis.DistributedCaching) | A library for accessing [Redis](https://stackexchange.github.io/StackExchange.Redis/) caches for [distributed caching](/aspnet/core/performance/caching/distributed). |
| [Redis Output Caching](../caching/stackexchange-redis-output-caching-integration.md) <br/> <img src="media/icons/redis-cube-red_white-rgb.png" alt="Redis logo." role="presentation" width="64" data-linktype="relative-path"> |  - **Hosting**: [Aspire.Hosting.Redis](https://www.nuget.org/packages/Aspire.Hosting.Redis), [Aspire.Hosting.Garnet](https://www.nuget.org/packages/Aspire.Hosting.Garnet), or [Aspire.Hosting.Valkey](https://www.nuget.org/packages/Aspire.Hosting.RValkeyedis)<br>- **Client**: [Aspire.StackExchange.Redis.OutputCaching](https://www.nuget.org/packages/Aspire.StackExchange.Redis.OutputCaching) | A library for accessing [Redis](https://stackexchange.github.io/StackExchange.Redis/) caches for [output caching](/aspnet/core/performance/caching/output). |
| [Redis](../caching/stackexchange-redis-integration.md) <br/> <img src="media/icons/redis-cube-red_white-rgb.png" alt="Redis logo." role="presentation" width="64" data-linktype="relative-path"> | - **Hosting**: [Aspire.Hosting.Redis](https://www.nuget.org/packages/Aspire.Hosting.Redis), [Aspire.Hosting.Garnet](https://www.nuget.org/packages/Aspire.Hosting.Garnet), or [Aspire.Hosting.Valkey](https://www.nuget.org/packages/Aspire.Hosting.RValkeyedis)<br>- **Client**: [Aspire.StackExchange.Redis](https://www.nuget.org/packages/Aspire.StackExchange.Redis) | A library for accessing [Redis](https://stackexchange.github.io/StackExchange.Redis/) caches. |
| [Seq](../logging/seq-integration.md) <br/> <img src="media/icons/Seq_logo.256x.png" alt="Seq logo." role="presentation" width="64" data-linktype="relative-path"> | - **Hosting**: [Aspire.Hosting.Seq](https://www.nuget.org/packages/Aspire.Hosting.Seq)<br>- **Client**: [Aspire.Seq](https://www.nuget.org/packages/Aspire.Seq) | A library for logging to [Seq](https://datalust.co/seq). |
| [SQL Server Entity Framework Core](../database/sql-server-entity-framework-integration.md) <br/> <img src="media/icons/SQL_256x.png" alt="SQL logo." role="presentation" width="64" data-linktype="relative-path"> | - **Hosting**: [Aspire.Hosting.SqlServer](https://www.nuget.org/packages/Aspire.Hosting.SqlServer)<br>- **Client**: [Aspire.Microsoft.EntityFrameworkCore.SqlServer](https://www.nuget.org/packages/Aspire.Microsoft.EntityFrameworkCore.SqlServer) | A library for accessing [SQL Server databases using Entity Framework Core](/ef/core/providers/sql-server/). |
| [SQL Server](../database/sql-server-integration.md) <br/> <img src="media/icons/SQL_256x.png" alt="SQL logo." role="presentation" width="64" data-linktype="relative-path"> | - **Hosting**: [Aspire.Hosting.SqlServer](https://www.nuget.org/packages/Aspire.Hosting.SqlServer)<br>- **Client**: [Aspire.Microsoft.Data.SqlClient](https://www.nuget.org/packages/Aspire.Microsoft.Data.SqlClient) | A library for accessing [SQL Server](/sql/sql-server/) databases. |
<!-- markdownlint-enable MD033 MD045 -->

For more information on working with .NET Aspire integrations in Visual Studio, see [Visual Studio tooling](setup-tooling.md#visual-studio-tooling).

**Azure specific resources are available in the following NuGet packages:**

<span id="azure-hosting-libraries"></span>

- [📦 Aspire.Hosting.Azure.AppConfiguration](https://www.nuget.org/packages/Aspire.Hosting.Azure.AppConfiguration)
- [📦 Aspire.Hosting.Azure.ApplicationInsights](https://www.nuget.org/packages/Aspire.Hosting.Azure.ApplicationInsights)
- [📦 Aspire.Hosting.Azure.CognitiveServices](https://www.nuget.org/packages/Aspire.Hosting.Azure.CognitiveServices)
- [📦 Aspire.Hosting.Azure.CosmosDB](https://www.nuget.org/packages/Aspire.Hosting.Azure.CosmosDB)
- [📦 Aspire.Hosting.Azure.EventHubs](https://www.nuget.org/packages/Aspire.Hosting.Azure.EventHubs)
- [📦 Aspire.Hosting.Azure.KeyVault](https://www.nuget.org/packages/Aspire.Hosting.Azure.KeyVault)
- [📦 Aspire.Hosting.Azure.OperationalInsights](https://www.nuget.org/packages/Aspire.Hosting.Azure.OperationalInsights)
- [📦 Aspire.Hosting.Azure.PostgreSQL](https://www.nuget.org/packages/Aspire.Hosting.Azure.PostgreSQL)
- [📦 Aspire.Hosting.Azure.Redis](https://www.nuget.org/packages/Aspire.Hosting.Azure.Redis)
- [📦 Aspire.Hosting.Azure.Search](https://www.nuget.org/packages/Aspire.Hosting.Azure.Search)
- [📦 Aspire.Hosting.Azure.ServiceBus](https://www.nuget.org/packages/Aspire.Hosting.Azure.ServiceBus)
- [📦 Aspire.Hosting.Azure.SignalR](https://www.nuget.org/packages/Aspire.Hosting.Azure.SignalR)
- [📦 Aspire.Hosting.Azure.Sql](https://www.nuget.org/packages/Aspire.Hosting.Azure.Sql)
- [📦 Aspire.Hosting.Azure.Storage](https://www.nuget.org/packages/Aspire.Hosting.Azure.Storage)
- [📦 Aspire.Hosting.Azure.WebPubSub](https://www.nuget.org/packages/Aspire.Hosting.Azure.WebPubSub)

| Method | Resource type | Description |
|--|--|--|
| <xref:Aspire.Hosting.AzureStorageExtensions.AddAzureStorage%2A> | <xref:Aspire.Hosting.Azure.AzureStorageResource> | Adds an Azure Storage resource. |
| `AddAzureStorage(...).`<xref:Aspire.Hosting.AzureStorageExtensions.AddBlobs%2A> | <xref:Aspire.Hosting.Azure.AzureBlobStorageResource> | Adds an Azure Blob Storage resource. |
| `AddAzureStorage(...).`<xref:Aspire.Hosting.AzureStorageExtensions.AddQueues%2A> | <xref:Aspire.Hosting.Azure.AzureQueueStorageResource> | Adds an Azure Queue Storage resource. |
| `AddAzureStorage(...).`<xref:Aspire.Hosting.AzureStorageExtensions.AddTables%2A> | <xref:Aspire.Hosting.Azure.AzureTableStorageResource> | Adds an Azure Table Storage resource. |
| <xref:Aspire.Hosting.AzureCosmosExtensions.AddAzureCosmosDB%2A> | <xref:Aspire.Hosting.AzureCosmosDBResource> | Adds an Azure Cosmos DB resource. |
| <xref:Aspire.Hosting.AzureKeyVaultResourceExtensions.AddAzureKeyVault%2A> | <xref:Aspire.Hosting.Azure.AzureKeyVaultResource> | Adds an Azure Key Vault resource. |
| `AddRedis(...)`.<xref:Aspire.Hosting.AzureRedisExtensions.AsAzureRedis%2A> | <xref:Aspire.Hosting.Azure.AzureRedisResource> | Configures resource to use Azure for local development and when doing a deployment via the Azure Developer CLI. |
| `AddSqlServer(...)`.<xref:Aspire.Hosting.AzureSqlExtensions.AsAzureSqlDatabase%2A> | <xref:Aspire.Hosting.Azure.AzureSqlServerResource> | Configures SQL Server resource to be deployed as Azure SQL Database (server). |
| <xref:Aspire.Hosting.AzureServiceBusExtensions.AddAzureServiceBus%2A> | <xref:Aspire.Hosting.Azure.AzureServiceBusResource> | Adds an Azure Service Bus resource. |
| <xref:Aspire.Hosting.AzureWebPubSubExtensions.AddAzureWebPubSub%2A> | <xref:Aspire.Hosting.ApplicationModel.AzureWebPubSubResource> | Adds an Azure Web PubSub resources. |

> [!IMPORTANT]
> The .NET Aspire Azure hosting libraries rely on `Azure.Provisioning.*` libraries to provision Azure resources. For more information, [Azure provisioning libraries](../deployment/azure/local-provisioning.md#azure-provisioning-libraries).

**AWS specific resources are available in the following NuGet package:**

<span id="aws-hosting-libraries"></span>

- [📦 Aspire.Hosting.AWS](https://www.nuget.org/packages/Aspire.Hosting.AWS)

For more information, see [GitHub: Aspire.Hosting.AWS library](https://github.com/dotnet/aspire/tree/main/src/Aspire.Hosting.AWS).

## Cloud-native features

Cloud-native applications surface many unique requirements and concerns. The core features of .NET Aspire orchestration and integrations are designed to handle many cloud-native concerns for you with minimal configurations. Some of the key features include:

- [Orchestration](app-host-overview.md): A lightweight, extensible, and cross-platform app host for .NET Aspire projects. The app host provides a consistent configuration and dependency injection experience for .NET Aspire integrations.
- [Service discovery](../service-discovery/overview.md): A technique for locating services within a distributed application. Service discovery is a key integration of microservice architectures.
- [Service defaults](service-defaults.md): A set of default configurations intended for sharing among resources within .NET Aspire projects. These defaults are designed to work well in most scenarios and can be customized as needed.

Some .NET Aspire integrations also include more capabilities for specific services or platforms, which can be found in the integration specific reference docs.

### Observability and telemetry

.NET Aspire integrations automatically set up Logging, Tracing, and Metrics configurations, which are sometimes known as _the pillars of observability_.

- **[Logging](/dotnet/core/diagnostics/logging-tracing)**: A technique where code is instrumented to produce logs of interesting events that occurred while the program was running. A baseline set of log events is enabled for .NET Aspire integrations by default and more extensive logging can be enabled on-demand to diagnose particular problems.

- **[Tracing](/dotnet/core/diagnostics/distributed-tracing)**: A specialized form of logging that helps you localize failures and performance issues within applications distributed across multiple machines or processes. This technique tracks requests through an application to correlate work done by different application integrations and separate it from other work the application may be doing for concurrent requests.

- **[Metrics](/dotnet/core/diagnostics/metrics)**: Numerical measurements recorded over time to monitor application performance and health. Metrics are often used to generate alerts when potential problems are detected. Metrics have low performance overhead and many services configure them as always-on telemetry.

Together, these types of telemetry allow you to gain insights into your application's behavior and performance using various monitoring and analysis tools. Depending on the backing service, some integrations might only support some of these features. For example, some integrations support logging and tracing, but not metrics. Telemetry features can also be disabled. For more information, see [.NET Aspire service defaults](service-defaults.md).

### Health checks

.NET Aspire integrations enable health checks for services by default. Health checks are HTTP endpoints exposed by an app to provide basic availability and state information. These endpoints can be configured to report information used for various scenarios:

- Influence decisions made by container orchestrators, load balancers, API gateways, and other management services. For instance, if the health check for a containerized app fails, it might be skipped by a load balancer routing traffic.
- Verify that underlying dependencies are available, such as a database or cache, and return an appropriate status message.
- Trigger alerts or notifications when an app isn't responding as expected.

For example, the .NET Aspire PostgreSQL integration automatically adds a health check at the `/health` URL path to verify the following:

- A database connection could be established
- A database query could be executed successfully

If either of these operations fail, the health check also fails. For more information, see [Health checks in .NET Aspire](health-checks.md).

### Resiliency

.NET Aspire integrations enable resiliency configurations automatically where appropriate. Resiliency is the ability of your system to react to failure and still remain functional. Resiliency extends beyond preventing failures to include recovering and reconstructing your cloud-native environment back to a healthy state. Examples of resiliency configurations include:

- **Connection retries**: You can configure some .NET Aspire integrations to retry requests that initially fail. For example, failed database queries can be retried multiple times if the first request fails. This creates tolerance in environments where service dependencies may be briefly unresponsive or unavailable when the system state changes.

- **Timeouts**: You can configure how long an .NET Aspire integration waits for a request to finish before it times out. Timeout configurations can be useful for handling dependencies with variable response times.

For more information, see [Build resilient HTTP apps](/dotnet/core/resilience/http-resilience).
