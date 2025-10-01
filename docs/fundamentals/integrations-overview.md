---
title: Aspire integrations overview
description: Explore the fundamental concepts of Aspire integrations and learn how to integrate them into your apps.
ms.date: 09/23/2025
ms.topic: conceptual
uid: dotnet/aspire/integrations
---

# Aspire integrations overview

Aspire integrations are a curated suite of NuGet packages selected to facilitate the integration of cloud-native applications with prominent services and platforms, such as Redis and PostgreSQL. Each integration furnishes essential cloud-native functionalities through either automatic provisioning or standardized configuration patterns.

> [!TIP]
> Always strive to use the latest version of Aspire integrations to take advantage of the latest features, improvements, and security updates.

> [!WARNING]
> Integrations execute code in your development environment. Ensure that third-party integrations are trusted before use. For more information, see [Best practices for a secure software supply chain](/nuget/concepts/security-best-practices).

## Integration responsibilities

Most Aspire integrations are made up of two separate libraries, each with a different responsibility. One type represents resources within the [_AppHost_](app-host-overview.md) project—known as [hosting integrations](#hosting-integrations). The other type of integration represents client libraries that connect to the resources modeled by hosting integrations, and they're known as [client integrations](#client-integrations).

### Hosting integrations

Hosting integrations configure applications by provisioning resources (like containers or cloud resources) or pointing to existing instances (such as a local SQL server). These packages model various services, platforms, or capabilities, including caches, databases, logging, storage, and messaging systems.

Hosting integrations extend the <xref:Aspire.Hosting.IDistributedApplicationBuilder> interface, enabling the _AppHost_ project to express resources within its [_app model_](app-host-overview.md#terminology). **Hosting integrations work with any type of application**, not just .NET applications. They provide infrastructure and inject configuration details (such as connection strings, endpoints, and credentials) as environment variables into any project, executable, or container that references them.

The official [hosting integration NuGet packages](https://www.nuget.org/packages?q=owner%3A+aspire+tags%3A+aspire+hosting+integration&includeComputedFrameworks=true&prerel=true&sortby=relevance) are tagged with `aspire`, `integration`, and `hosting`. In addition to the official hosting integrations, the [community has created hosting integrations](../community-toolkit/overview.md) for various services and platforms as part of the Community Toolkit.

For information on creating a custom _hosting integration_, see [Create custom Aspire hosting integration](../extensibility/custom-hosting-integration.md).

### Client integrations

Client integrations wire up client libraries to [dependency injection (DI)](/dotnet/core/extensions/dependency-injection), define configuration schema, and add [health checks](health-checks.md), [resiliency](/dotnet/core/resilience), and [telemetry](telemetry.md) where applicable. Aspire client integration libraries are prefixed with `Aspire.` and then include the full package name that they integrate with, such as `Aspire.StackExchange.Redis`.

These packages configure existing client libraries to connect to hosting integrations. They extend the <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> interface allowing client-consuming projects, such as your web app or API, to use the connected resource. The official [client integration NuGet packages](https://www.nuget.org/packages?q=owner%3A+aspire+tags%3A+aspire+client+integration&includeComputedFrameworks=true&prerel=true&sortby=relevance) are tagged with `aspire`, `integration`, and `client`. In addition to the official client integrations, the [community has created client integrations](../community-toolkit/overview.md) for various services and platforms as part of the Community Toolkit.

> [!IMPORTANT]
> Aspire integrations require <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> and are **not compatible** with `HostingStartup` implementations, which only provide access to <xref:Microsoft.AspNetCore.Hosting.IWebHostBuilder>. If you're using `HostingStartup` for modular configuration, see [HostingStartup is not supported with Aspire integrations](../troubleshooting/hosting-startup-not-supported.md) for migration guidance.

For more information on creating a custom client integration, see [Create custom Aspire client integrations](../extensibility/custom-client-integration.md).

### Relationship between hosting and client integrations

Hosting and client integrations are best when used together, but are **not** coupled and can be used separately. Some hosting integrations don't have a corresponding client integration. Configuration is what makes the hosting integration work with the client integration.

**Client integrations are .NET-specific** and provide convenient, opinionated ways to configure .NET applications. However, **you can use hosting integrations without client integrations** for non-.NET applications or when you prefer to configure connections manually. In these scenarios, the hosting integration still provides the infrastructure and exposes connection information through environment variables that any application technology can consume.

Consider the following diagram that depicts the relationship between hosting and client integrations:

:::image type="content" source="media/integrations-thumb.png" lightbox="media/integrations.png" alt-text="A diagram ":::

The AppHost project is where hosting integrations are used. Configuration, specifically environment variables, is injected into projects, executables, and containers, allowing client integrations to connect to the hosting integrations.

## Integration features

When you add a client integration to a project within your Aspire solution, [service defaults](service-defaults.md) are automatically applied to that project; meaning the Service Defaults project is referenced and the `AddServiceDefaults` extension method is called. These defaults are designed to work well in most scenarios and can be customized as needed. The following service defaults are applied:

- **Observability and telemetry**: Automatically sets up logging, tracing, and metrics configurations:

  - **[Logging](/dotnet/core/diagnostics/logging-tracing)**: A technique where code is instrumented to produce logs of interesting events that occurred while the program was running.
  - **[Tracing](/dotnet/core/diagnostics/distributed-tracing)**: A specialized form of logging that helps you localize failures and performance issues within applications distributed across multiple machines or processes.
  - **[Metrics](/dotnet/core/diagnostics/metrics)**: Numerical measurements recorded over time to monitor application performance and health. Metrics are often used to generate alerts when potential problems are detected.

- **[Health checks](health-checks.md)**: Exposes HTTP endpoints to provide basic availability and state information about an app. Health checks are used to influence decisions made by container orchestrators, load balancers, API gateways, and other management services.
- **[Resiliency](/dotnet/core/resilience/http-resilience)**: The ability of your system to react to failure and still remain functional. Resiliency extends beyond preventing failures to include recovering and reconstructing your cloud-native environment back to a healthy state.

## Understand host integration extension methods

Aspire hosting integrations provide extension methods that start with either `Add` or `With`. These methods conform to the following pattern:

- **`Add*` methods**: `Add*` methods create and register new resources within the AppHost and return an `IResourceBuilder<TResource>` where `TResource` is the concrete resource type added. This lets you continue fluent configuration on the returned builder. For example, calling <xref:Aspire.Hosting.AzureServiceBusExtensions.AddAzureServiceBus*> returns an `IResourceBuilder<AzureServiceBusResource>`, and then calling <xref:Aspire.Hosting.AzureServiceBusExtensions.AddServiceBusQueue*> on that namespace builder returns an `IResourceBuilder<AzureServiceBusQueueResource>`. This pattern models parent-child relationships (for example, a Service Bus namespace and its queues or topics) while preserving a consistent fluent builder API.
- **`With*` methods**: Use `With*` methods to configure or enhance an existing resource. These methods typically return the same object type as the parent, allowing you to chain additional configuration calls.

> [!IMPORTANT]
> When using `Add` methods, make sure to pass the correct resource object to your client integration. Passing the wrong object can result in misconfigured connections or runtime errors.

Consider this code:

```csharp
var serviceBus = builder.AddAzureServiceBus(name: "serviceBus")
    .AddServiceBusTopic(name: "messagetopic");

var apiService = builder.AddProject<Projects.servicebusexp_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(serviceBus);

// The serviceBus is an IResourceBuilder<AzureServiceBusTopicResource> type
```

You may expect `serviceBus` to represent the Azure Service Bus resource but in fact, because you called <xref:Aspire.Hosting.AzureServiceBusExtensions.AddServiceBusTopic*> on the same line, `serviceBus` is an Azure Service Bus topic resource. To avoid this result, call `AddServiceBusTopic` on a separate line:

```csharp
var serviceBus = builder.AddAzureServiceBus(name: "serviceBus");
var topic = serviceBus.AddServiceBusTopic(name: "messagetopic");

var apiService = builder.AddProject<Projects.servicebusexp_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(serviceBus);

// The serviceBus is an IResourceBuilder<AzureServiceBusResource> type
```

Now, you can choose to pass the resource that consuming project needs. Either, as in the example, the Service Bus resource or the topic resource.

This distinction helps you model your application's infrastructure accurately and ensures that client integrations receive the correct connection information.

## Versioning considerations

Hosting and client integrations are updated each release to target the latest stable versions of dependent resources. When container images are updated with new image versions, the hosting integrations update to these new versions. Similarly, when a new NuGet version is available for a dependent client library, the corresponding client integration updates to the new version. This ensures the latest features and security updates are available to applications. The Aspire update type (major, minor, patch) doesn't necessarily indicate the type of update in dependent resources. For example, a new major version of a dependent resource may be updated in a Aspire patch release, if necessary.

When major breaking changes happen in dependent resources, integrations may temporarily split into version-dependent packages to ease updating across the breaking change. For more information, see the [first example of such a breaking change](https://github.com/dotnet/aspire/issues/3956).

## Official integrations

Aspire provides many integrations to help you build cloud-native applications. These integrations are designed to work seamlessly with the Aspire AppHost and client libraries. The following sections detail cloud-agnostic, Azure-specific, Amazon Web Services (AWS), and Community Toolkit integrations.

### Cloud-agnostic integrations

The following section details cloud-agnostic Aspire integrations with links to their respective docs and NuGet packages, and provides a brief description of each integration.

<!-- markdownlint-disable MD033 MD045 -->
| Integration docs and NuGet packages | Description |
|--|--|
| - **Learn more**: [📄 Apache Kafka](../messaging/kafka-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Kafka](https://www.nuget.org/packages/Aspire.Hosting.Kafka)<br>- **Client**: [📦 Aspire.Confluent.Kafka](https://www.nuget.org/packages/Aspire.Confluent.Kafka) | A library for producing and consuming messages from an [Apache Kafka](https://kafka.apache.org/) broker. |
| - **Learn more**: [📄 Dapr](../frameworks/dapr.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Dapr](https://www.nuget.org/packages/Aspire.Hosting.Dapr)<br>- **Client**: N/A | A library for modeling [Dapr](https://dapr.io/) as a Aspire resource. |
| - **Learn more**: [📄 Docker](../deployment/docker-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Docker](https://www.nuget.org/packages/Aspire.Hosting.Docker)<br>- **Client**: N/A | A library for deploying Aspire applications using Docker Compose. |
| - **Learn more**: [📄 Elasticsearch](../search/elasticsearch-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Elasticsearch](https://www.nuget.org/packages/Aspire.Hosting.Elasticsearch)<br>- **Client**: [📦 Aspire.Elastic.Clients.Elasticsearch](https://www.nuget.org/packages/Aspire.Elastic.Clients.Elasticsearch) | A library for accessing [Elasticsearch](https://www.elastic.co/guide/en/elasticsearch/client/index.html) databases. |
| - **Learn more**: [📄 Keycloak](../authentication/keycloak-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Keycloak](https://www.nuget.org/packages/Aspire.Hosting.Keycloak)<br>- **Client**: [📦 Aspire.Keycloak.Authentication](https://www.nuget.org/packages/Aspire.Keycloak.Authentication) | A library for accessing [Keycloak](https://www.keycloak.org/docs/latest/server_admin/index.html) authentication. |
| - **Learn more**: [📄 Milvus](../database/milvus-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Milvus](https://www.nuget.org/packages/Aspire.Hosting.Milvus)<br>- **Client**: [📦 Aspire.Milvus.Client](https://www.nuget.org/packages/Aspire.Milvus.Client) | A library for accessing [Milvus](https://milvus.io/) databases. |
| - **Learn more**: [📄 MongoDB Driver](../database/mongodb-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.MongoDB](https://www.nuget.org/packages/Aspire.Hosting.MongoDB)<br>- **Client**: [📦 Aspire.MongoDB.Driver](https://www.nuget.org/packages/Aspire.MongoDB.Driver) | A library for accessing [MongoDB](https://www.mongodb.com/docs) databases. |
| - **Learn more**: [📄 MySqlConnector](../database/mysql-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.MySql](https://www.nuget.org/packages/Aspire.Hosting.MySql)<br>- **Client**: [📦 Aspire.MySqlConnector](https://www.nuget.org/packages/Aspire.MySqlConnector) | A library for accessing [MySqlConnector](https://mysqlconnector.net/) databases. |
| - **Learn more**: [📄 NATS](../messaging/nats-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Nats](https://www.nuget.org/packages/Aspire.Hosting.Nats)<br>- **Client**: [📦 Aspire.NATS.Net](https://www.nuget.org/packages/Aspire.NATS.Net) | A library for accessing [NATS](https://nats.io/) messaging. |
| - **Learn more**: [📄 Oracle - EF Core](../database/oracle-entity-framework-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Oracle](https://www.nuget.org/packages/Aspire.Hosting.Oracle)<br>- **Client**: [📦 Aspire.Oracle.EntityFrameworkCore](https://www.nuget.org/packages/Aspire.Oracle.EntityFrameworkCore) | A library for accessing Oracle databases with [Entity Framework Core](/ef/core). |
| - **Learn more**: [📄 OpenAI](../openai/openai-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.OpenAI](https://www.nuget.org/packages/Aspire.Hosting.OpenAI)<br>- **Client**: [📦 Aspire.OpenAI](https://www.nuget.org/packages/Aspire.OpenAI) | A library for accessing [OpenAI](https://platform.openai.com/docs) APIs. |
| - **Learn more**: [📄 Orleans](../frameworks/Orleans.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Orleans](https://www.nuget.org/packages/Aspire.Hosting.Orleans)<br>- **Client**: N/A | A library for modeling [Orleans](/dotnet/Orleans) as a Aspire resource. |
| - **Learn more**: [📄 Pomelo MySQL - EF Core](../database/mysql-entity-framework-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.MySql](https://www.nuget.org/packages/Aspire.Hosting.MySql)<br>- **Client**: [📦 Aspire.Pomelo.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Aspire.Pomelo.EntityFrameworkCore.MySql) | A library for accessing MySql databases with [Entity Framework Core](/ef/core). |
| - **Learn more**: [📄 PostgreSQL - EF Core](../database/postgresql-entity-framework-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.PostgreSQL](https://www.nuget.org/packages/Aspire.Hosting.PostgreSQL)<br>- **Client**: [📦 Aspire.Npgsql.EntityFrameworkCore.PostgreSQL](https://www.nuget.org/packages/Aspire.Npgsql.EntityFrameworkCore.PostgreSQL) | A library for accessing PostgreSQL databases using [Entity Framework Core](https://www.npgsql.org/efcore/index.html). |
| - **Learn more**: [📄 PostgreSQL](../database/postgresql-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.PostgreSQL](https://www.nuget.org/packages/Aspire.Hosting.PostgreSQL)<br>- **Client**: [📦 Aspire.Npgsql](https://www.nuget.org/packages/Aspire.Npgsql) | A library for accessing [PostgreSQL](https://www.npgsql.org/doc/index.html) databases. |
| - **Learn more**: [📄 Qdrant](../database/qdrant-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Qdrant](https://www.nuget.org/packages/Aspire.Hosting.Qdrant)<br>- **Client**: [📦 Aspire.Qdrant.Client](https://www.nuget.org/packages/Aspire.Qdrant.Client) | A library for accessing [Qdrant](https://qdrant.tech/) databases. |
|  - **Learn more**: [📄 RabbitMQ](../messaging/rabbitmq-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.RabbitMQ](https://www.nuget.org/packages/Aspire.Hosting.RabbitMQ)<br>- **Client**: [📦 Aspire.RabbitMQ.Client](https://www.nuget.org/packages/Aspire.RabbitMQ.Client) | A library for accessing [RabbitMQ](https://www.rabbitmq.com/dotnet.html). |
| - **Learn more**: [📄 Redis Distributed Caching](../caching/stackexchange-redis-distributed-caching-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Redis](https://www.nuget.org/packages/Aspire.Hosting.Redis), [📦 Aspire.Hosting.Garnet](https://www.nuget.org/packages/Aspire.Hosting.Garnet), or [📦 Aspire.Hosting.Valkey](https://www.nuget.org/packages/Aspire.Hosting.Valkey)<br>- **Client**: [📦 Aspire.StackExchange.Redis.DistributedCaching](https://www.nuget.org/packages/Aspire.StackExchange.Redis.DistributedCaching) | A library for accessing [Redis](https://stackexchange.github.io/StackExchange.Redis/) caches for [distributed caching](/aspnet/core/performance/caching/distributed). |
| - **Learn more**: [📄 Redis Output Caching](../caching/stackexchange-redis-output-caching-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Redis](https://www.nuget.org/packages/Aspire.Hosting.Redis), [📦 Aspire.Hosting.Garnet](https://www.nuget.org/packages/Aspire.Hosting.Garnet), or [📦 Aspire.Hosting.Valkey](https://www.nuget.org/packages/Aspire.Hosting.Valkey)<br>- **Client**: [📦 Aspire.StackExchange.Redis.OutputCaching](https://www.nuget.org/packages/Aspire.StackExchange.Redis.OutputCaching) | A library for accessing [Redis](https://stackexchange.github.io/StackExchange.Redis/) caches for [output caching](/aspnet/core/performance/caching/output). |
| - **Learn more**: [📄 Redis](../caching/stackexchange-redis-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Redis](https://www.nuget.org/packages/Aspire.Hosting.Redis), [📦 Aspire.Hosting.Garnet](https://www.nuget.org/packages/Aspire.Hosting.Garnet), or [📦 Aspire.Hosting.Valkey](https://www.nuget.org/packages/Aspire.Hosting.Valkey)<br>- **Client**: [📦 Aspire.StackExchange.Redis](https://www.nuget.org/packages/Aspire.StackExchange.Redis) | A library for accessing [Redis](https://stackexchange.github.io/StackExchange.Redis/) caches. |
| - **Learn more**: [📄 Seq](../logging/seq-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Seq](https://www.nuget.org/packages/Aspire.Hosting.Seq)<br>- **Client**: [📦 Aspire.Seq](https://www.nuget.org/packages/Aspire.Seq) | A library for logging to [Seq](https://datalust.co/seq). |
| - **Learn more**: [📄 SQL Server - EF Core](../database/sql-server-entity-framework-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.SqlServer](https://www.nuget.org/packages/Aspire.Hosting.SqlServer)<br>- **Client**: [📦 Aspire.Microsoft.EntityFrameworkCore.SqlServer](https://www.nuget.org/packages/Aspire.Microsoft.EntityFrameworkCore.SqlServer) | A library for accessing [SQL Server databases using EF Core](/ef/core/providers/sql-server/). |
| - **Learn more**: [📄 SQL Server](../database/sql-server-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.SqlServer](https://www.nuget.org/packages/Aspire.Hosting.SqlServer)<br>- **Client**: [📦 Aspire.Microsoft.Data.SqlClient](https://www.nuget.org/packages/Aspire.Microsoft.Data.SqlClient) | A library for accessing [SQL Server](/sql/sql-server/) databases. |
<!-- markdownlint-enable MD033 MD045 -->

For more information on working with Aspire integrations in Visual Studio, see [Visual Studio tooling](setup-tooling.md#visual-studio-tooling).

### Azure integrations

Azure integrations configure applications to use Azure resources. These hosting integrations are available in the `Aspire.Hosting.Azure.*` NuGet packages, while their client integrations are available in the `Aspire.*` NuGet packages:

<!-- markdownlint-disable MD033 MD045 -->
| Integration | Docs and NuGet packages | Description |
|--|--|--|
| <img src="media/icons/AzureAppConfig_256x.png" alt="Azure App Configuration logo." role="presentation" width="78" data-linktype="relative-path"> | - **Learn more**: [📄 Azure App Configuration](https://github.com/dotnet/aspire/blob/main/src/Aspire.Hosting.Azure.AppConfiguration/README.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Azure.AppConfiguration](https://www.nuget.org/packages/Aspire.Hosting.Azure.AppConfiguration)<br>- **Client**: N/A | A library for interacting with [Azure App Configuration](/azure/azure-app-configuration/). |
| <img src="media/icons/AzureAppInsights_256x.png" alt="Azure Application Insights logo." role="presentation" width="78" data-linktype="relative-path"> | - **Learn more**: [📄 Azure Application Insights](https://github.com/dotnet/aspire/blob/main/src/Aspire.Hosting.Azure.ApplicationInsights/README.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Azure.ApplicationInsights](https://www.nuget.org/packages/Aspire.Hosting.Azure.ApplicationInsights)<br>- **Client**: N/A | A library for interacting with [Azure Application Insights](/azure/azure-monitor/app/app-insights-overview). |
| <img src="media/icons/AzureCacheRedis_256x.png" alt="Azure Cache for Redis logo" role="presentation" width="78" data-linktype="relative-path"> | - **Learn more**: [📄 Azure Cache for Redis](../caching/azure-cache-for-redis-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Azure.Redis](https://www.nuget.org/packages/Aspire.Hosting.Azure.Redis)<br>- **Client**: [📦 Aspire.StackExchange.Redis](https://www.nuget.org/packages/Aspire.StackExchange.Redis) or [📦 Aspire.StackExchange.Redis.DistributedCaching](https://www.nuget.org/packages/Aspire.StackExchange.Redis.DistributedCaching) or [📦 Aspire.StackExchange.Redis.OutputCaching](https://www.nuget.org/packages/Aspire.StackExchange.Redis.OutputCaching) | A library for accessing [Azure Cache for Redis](/azure/azure-cache-for-redis/). |
| <img src="media/icons/AzureCosmosDB_256x.png" alt="Azure Cosmos DB EF logo." role="presentation" width="78" data-linktype="relative-path"> | - **Learn more**: [📄 Azure Cosmos DB - EF Core](../database/azure-cosmos-db-entity-framework-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Azure.CosmosDB](https://www.nuget.org/packages/Aspire.Hosting.Azure.CosmosDB)<br>- **Client**: [📦 Aspire.Microsoft.EntityFrameworkCore.Cosmos](https://www.nuget.org/packages/Aspire.Microsoft.EntityFrameworkCore.Cosmos) | A library for accessing Azure Cosmos DB databases with [Entity Framework Core](/ef/core/providers/cosmos/). |
| <img src="media/icons/AzureCosmosDB_256x.png" alt="Azure Cosmos DB logo." role="presentation" width="78" data-linktype="relative-path">| - **Learn more**: [📄 Azure Cosmos DB](../database/azure-cosmos-db-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Azure.CosmosDB](https://www.nuget.org/packages/Aspire.Hosting.Azure.CosmosDB)<br>- **Client**: [📦 Aspire.Microsoft.Azure.Cosmos](https://www.nuget.org/packages/Aspire.Microsoft.Azure.Cosmos) | A library for accessing [Azure Cosmos DB](/azure/cosmos-db/introduction) databases. |
| <img src="media/icons/AzureEventHubs_256x.png" alt="Azure Event Hubs logo." role="presentation" width="78" data-linktype="relative-path"> | - **Learn more**: [📄 Azure Event Hubs](../messaging/azure-event-hubs-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Azure.EventHubs](https://www.nuget.org/packages/Aspire.Hosting.Azure.EventHubs)<br>- **Client**: [📦 Aspire.Azure.Messaging.EventHubs](https://www.nuget.org/packages/Aspire.Azure.Messaging.EventHubs) | A library for accessing [Azure Event Hubs](/azure/event-hubs/event-hubs-about). |
| <img src="media/icons/AzureFunctionApps_256x.png" alt="Azure Functions logo." role="presentation" width="78" data-linktype="relative-path"> | - **Learn more**: [📄 Azure Functions](../serverless/functions.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Azure.Functions](https://www.nuget.org/packages/Aspire.Hosting.Azure.Functions)<br>- **Client**: N/A | A library for integrating with [Azure Functions](/azure/azure-functions/). |
| <img src="media/icons/AzureKeyVault_256x.png" alt="Azure Key Vault logo." role="presentation" width="78" data-linktype="relative-path"> | - **Learn more**: [📄 Azure Key Vault](../security/azure-security-key-vault-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Azure.KeyVault](https://www.nuget.org/packages/Aspire.Hosting.Azure.KeyVault)<br>- **Client**: [📦 Aspire.Azure.Security.KeyVault](https://www.nuget.org/packages/Aspire.Azure.Security.KeyVault) | A library for accessing [Azure Key Vault](/azure/key-vault/general/overview). |
| <img src="media/icons/AzureLogAnalytics_256x.png" alt="Azure Operational Insights logo." role="presentation" width="78" data-linktype="relative-path"> | - **Learn more**: [📄 Azure Operational Insights](https://github.com/dotnet/aspire/blob/main/src/Aspire.Hosting.Azure.OperationalInsights/README.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Azure.OperationalInsights](https://www.nuget.org/packages/Aspire.Hosting.Azure.OperationalInsights)<br>- **Client**: N/A | A library for interacting with [Azure Operational Insights](/azure/azure-monitor/logs/log-analytics-workspace-overview). |
| <img src="media/icons/AzureOpenAI_256x.png" alt="Azure OpenAI logo." role="presentation" width="78" data-linktype="relative-path"> | - **Learn more**: [📄 Azure AI OpenAI](../azureai/azureai-openai-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Azure.CognitiveServices](https://www.nuget.org/packages/Aspire.Hosting.Azure.CognitiveServices)<br>- **Client**: [📦 Aspire.Azure.AI.OpenAI](https://www.nuget.org/packages/Aspire.Azure.AI.OpenAI) | A library for accessing [Azure AI OpenAI](/azure/ai-services/openai/overview) or OpenAI functionality. |
| <img src="media/icons/AzurePostgreSQL_256x.png" alt="Azure PostgreSQL logo." role="presentation" width="78" data-linktype="relative-path"> | - **Learn more**: [📄 Azure PostgreSQL](https://github.com/dotnet/aspire/blob/main/src/Aspire.Hosting.Azure.PostgreSQL/README.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Azure.PostgreSQL](https://www.nuget.org/packages/Aspire.Hosting.Azure.PostgreSQL)<br>- **Client**: N/A | A library for interacting with [Azure Database for PostgreSQL](/azure/postgresql/). |
| <img src="media/icons/AzureSearch_256x.png" alt="Azure AI Search logo." role="presentation" width="78" data-linktype="relative-path"> | - **Learn more**: [📄 Azure AI Search](../azureai/azureai-search-document-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Azure.Search](https://www.nuget.org/packages/Aspire.Hosting.Azure.Search)<br>- **Client**: [📦 Aspire.Azure.Search.Documents](https://www.nuget.org/packages/Aspire.Azure.Search.Documents) | A library for accessing [Azure AI Search](/azure/search/search-what-is-azure-search) functionality. |
| <img src="media/icons/AzureServiceBus_256x.png" alt="Azure Service Bus logo." role="presentation" width="78" data-linktype="relative-path"> | - **Learn more**: [📄 Azure Service Bus](../messaging/azure-service-bus-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Azure.ServiceBus](https://www.nuget.org/packages/Aspire.Hosting.Azure.ServiceBus)<br>- **Client**: [📦 Aspire.Azure.Messaging.ServiceBus](https://www.nuget.org/packages/Aspire.Azure.Messaging.ServiceBus) | A library for accessing [Azure Service Bus](/azure/service-bus-messaging/service-bus-messaging-overview). |
| <img src="media/icons/AzureSignalR_256x.png" alt="Azure SignalR Service logo." role="presentation" width="78" data-linktype="relative-path"> | - **Learn more**: [📄 Azure SignalR Service](../real-time/azure-signalr-scenario.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Azure.SignalR](https://www.nuget.org/packages/Aspire.Hosting.Azure.SignalR)<br>- **Client**: [Microsoft.Azure.SignalR](https://www.nuget.org/packages/Microsoft.Azure.SignalR) | A library for accessing [Azure SignalR Service](/azure/azure-signalr/signalr-overview). |
| <img src="media/icons/AzureBlobPageStorage_256x.png" alt="Azure Blob Storage logo." role="presentation" width="78" data-linktype="relative-path"> | - **Learn more**: [📄 Azure Blob Storage](../storage/azure-storage-blobs-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Azure.Storage](https://www.nuget.org/packages/Aspire.Hosting.Azure.Storage)<br>- **Client**: [📦 Aspire.Azure.Storage.Blobs](https://www.nuget.org/packages/Aspire.Azure.Storage.Blobs) | A library for accessing [Azure Blob Storage](/azure/storage/blobs/storage-blobs-introduction). |
| <img src="media/icons/AzureStorageQueue_256x.png" alt="Azure Storage Queues logo." role="presentation" width="78" data-linktype="relative-path"> | - **Learn more**: [📄 Azure Storage Queues](../storage/azure-storage-queues-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Azure.Storage](https://www.nuget.org/packages/Aspire.Hosting.Azure.Storage)<br>- **Client**: [📦 Aspire.Azure.Storage.Queues](https://www.nuget.org/packages/Aspire.Azure.Storage.Queues) | A library for accessing [Azure Storage Queues](/azure/storage/queues/storage-queues-introduction). |
| <img src="media/icons/AzureTable_256x.png" alt="Azure Table Storage logo." role="presentation" width="78" data-linktype="relative-path"> | - **Learn more**: [📄 Azure Table Storage](../storage/azure-storage-tables-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Azure.Storage](https://www.nuget.org/packages/Aspire.Hosting.Azure.Storage)<br>- **Client**: [📦 Aspire.Azure.Data.Tables](https://www.nuget.org/packages/Aspire.Azure.Data.Tables) | A library for accessing the [Azure Table](/azure/storage/tables/table-storage-overview) service. |
| <img src="media/icons/AzureWebPubSub_256x.png" alt="Azure Web PubSub logo." role="presentation" width="78" data-linktype="relative-path"> | - **Learn more**: [📄 Azure Web PubSub](../messaging/azure-web-pubsub-integration.md) <br/> - **Hosting**: [📦 Aspire.Hosting.Azure.WebPubSub](https://www.nuget.org/packages/Aspire.Hosting.Azure.WebPubSub)<br>- **Client**: [📦 Aspire.Azure.Messaging.WebPubSub](https://www.nuget.org/packages/Aspire.Azure.Messaging.WebPubSub) | A library for accessing the [Azure Web PubSub](/azure/azure-web-pubsub/) service. |
<!-- markdownlint-enable MD033 MD045 -->

### Amazon Web Services (AWS) hosting integrations

<!-- markdownlint-disable MD033 MD045 -->
| Integration docs and NuGet packages | Description |
|--|--|
| - **Learn more**: [📄 AWS Hosting](https://github.com/aws/integrations-on-dotnet-aspire-for-aws/blob/main/src/Aspire.Hosting.AWS/README.md) <br/> - **Hosting**: [📦 Aspire.Hosting.AWS](https://www.nuget.org/packages/Aspire.Hosting.AWS)<br>- **Client**: N/A | A library for modeling [AWS resources](https://aws.amazon.com/cdk/). |
<!-- markdownlint-enable MD033 MD045 -->

For more information, see [GitHub: Aspire.Hosting.AWS library](https://github.com/aws/integrations-on-dotnet-aspire-for-aws/tree/main/src/Aspire.Hosting.AWS).

### Community Toolkit integrations

> [!NOTE]
> The Community Toolkit integrations are community-driven and maintained by the Aspire community. These integrations are not officially supported by the Aspire team.

<!-- markdownlint-disable MD033 MD045 -->
| Integration docs and NuGet packages | Description |
|--|--|
| - **Learn More**: [📄 Bun hosting](../community-toolkit/hosting-bun.md) <br /> - **Hosting**: [📦 CommunityToolkit.Aspire.Hosting.Bun](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.Bun) <br /> - **Client**: N/A | A hosting integration for Bun apps. |
| - **Learn More**: [📄 Deno hosting](../community-toolkit/hosting-deno.md) <br /> - **Hosting**: [📦 CommunityToolkit.Aspire.Hosting.Deno](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.Deno) <br /> - **Client**: N/A | A hosting integration for Deno apps. |
| - **Learn More**: [📄 Go hosting](../community-toolkit/hosting-golang.md) <br /> - **Hosting**: [📦 CommunityToolkit.Aspire.Hosting.Golang](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.Golang) <br /> - **Client**: N/A | A hosting integration for Go apps. |
| - **Learn More**: [📄 Java/Spring hosting](../community-toolkit/hosting-java.md) <br /> - **Hosting**: [📦 CommunityToolkit.Aspire.Hosting.Java](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.Java) <br /> - **Client**: N/A | A integration for running Java code in Aspire either using the local JDK or using a container. |
| - **Learn More**: [📄 Node.js hosting extensions](../community-toolkit/hosting-nodejs-extensions.md) <br /> - **Hosting**: [📦 CommunityToolkit.Aspire.Hosting.NodeJs.Extensions](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.NodeJS.Extensions) <br /> - **Client**: N/A  | An integration that contains some additional extensions for running Node.js applications |
| - **Learn More**: [📄 Ollama](../community-toolkit/ollama.md) <br /> - **Hosting**: [📦 CommunityToolkit.Aspire.Hosting.Ollama](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.Ollama) <br /> - **Client**: [📦 Aspire.CommunitToolkit.OllamaSharp](https://nuget.org/packages/CommunityToolkit.Aspire.OllamaSharp) | An Aspire component leveraging the [Ollama](https://ollama.com) container with support for downloading a model on startup. |
| - **Learn More**: [📄 Meilisearch hosting](../community-toolkit/hosting-meilisearch.md) <br /> - **Hosting**: [📦 CommunityToolkit.Aspire.Hosting.Meilisearch](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.Meilisearch) <br /> - **Client**: [📦 Aspire.CommunitToolkit.Meilisearch](https://nuget.org/packages/CommunityToolkit.Aspire.Meilisearch) | An Aspire component leveraging the [Meilisearch](https://meilisearch.com) container. |
| - **Learn More**: [📄 Rust hosting](../community-toolkit/hosting-rust.md) <br /> - **Hosting**: [📦 CommunityToolkit.Aspire.Hosting.Rust](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.Rust) <br /> - **Client**: N/A | A hosting integration for Rust apps. |
| - **Learn More**: [📄 SQL Database projects hosting](../community-toolkit/hosting-sql-database-projects.md) <br /> - **Hosting**: [📦 CommunityToolkit.Aspire.Hosting.SqlDatabaseProjects](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.SqlDatabaseProjects) <br /> - **Client**: N/A | An Aspire hosting integration for SQL Database Projects. |
| - **Learn More**: [📄 RavenDB](../community-toolkit/ravendb.md) <br /> - **Hosting**: [📦 CommunityToolkit.Aspire.Hosting.RavenDB](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.RavenDB) <br /> - **Client**: [📦 Aspire.CommunityToolkit.RavenDB.Client](https://nuget.org/packages/CommunityToolkit.Aspire.RavenDB.Client) | An Aspire component leveraging the [RavenDB](https://ravendb.net/) container. |
<!-- markdownlint-enable MD033 MD045 -->

For more information, see [Aspire Community Toolkit](../community-toolkit/overview.md).
