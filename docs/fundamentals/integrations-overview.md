---
title: .NET Aspire integrations overview
description: Explore the fundamental concepts of .NET Aspire integrations and learn how to integrate them into your apps.
ms.date: 08/12/2024
ms.topic: conceptual
---

# .NET Aspire integrations overview

.NET Aspire integrations are a curated suite of NuGet packages specifically selected to facilitate the integration of cloud-native applications with prominent services and platforms, including but not limited to Redis and PostgreSQL. Each integration furnishes essential cloud-native functionalities through either automatic provisioning or standardized configuration patterns. .NET Aspire integrations can be used without an app host (orchestrator) project, but they're designed to work best with the [.NET Aspire app host](app-host-overview.md).

.NET Aspire integrations should not be confused with .NET Aspire hosting packages, as they serve different purposes. Hosting packages are used to model and configure various resources in a .NET Aspire project, while integrations are used to map configuration to various client libraries.

> [!TIP]
> Always strive to use the latest version of .NET Aspire integrations to take advantage of the latest features, improvements, and security updates.

## Available integrations

The following table lists the .NET Aspire integrations currently available for use:

<!-- markdownlint-disable MD033 MD045 -->
| Component | NuGet | Description |
|--|--|--|
| [Apache Kafka](../messaging/kafka-integration.md) <br/> <img src="media/icons/Aspire-logo-256.png" alt=".NET Aspire logo." role="presentation" width="64" data-linktype="relative-path"> | [Aspire.Confluent.Kafka](https://www.nuget.org/packages/Aspire.Confluent.Kafka) | A library for producing and consuming messages from an [Apache Kafka](https://kafka.apache.org/) broker. |
| [Azure AI OpenAI](../azureai/azureai-openai-integration.md) <br/> <img src="media/icons/AzureOpenAI_256x.png" alt="Azire OpenAI logo." role="presentation" width="64" data-linktype="relative-path"> | [Aspire.Azure.AI.OpenAI](https://www.nuget.org/packages/Aspire.Azure.AI.OpenAI) | A library for accessing [Azure AI OpenAI](/azure/ai-services/openai/overview) or OpenAI functionality. |
| [Azure Blob Storage](../storage/azure-storage-blobs-integration.md) <br/> <img src="media/icons/AzureStorageContainer_256x.png" alt="Azure Blog Storage logo."  role="presentation" width="64" data-linktype="relative-path"> | [Aspire.Azure.Storage.Blobs](https://www.nuget.org/packages/Aspire.Azure.Storage.Blobs) | A library for accessing [Azure Blob Storage](/azure/storage/blobs/storage-blobs-introduction). |
| [Azure Cosmos DB Entity Framework Core](../database/azure-cosmos-db-entity-framework-integration.md) <br/> <img src="media/icons/AzureCosmosDB_256x.png" alt="Azure Cosmos DB EF logo." role="presentation" width="64" data-linktype="relative-path"> | [Aspire.Microsoft.EntityFrameworkCore.Cosmos](https://www.nuget.org/packages/Aspire.Microsoft.EntityFrameworkCore.Cosmos) | A library for accessing Azure Cosmos DB databases with [Entity Framework Core](/ef/core/providers/cosmos/). |
| [Azure Cosmos DB](../database/azure-cosmos-db-integration.md) <br/> <img src="media/icons/AzureCosmosDB_256x.png" alt="Azure Cosmos DB logo." role="presentation" width="64" data-linktype="relative-path"> | [Aspire.Microsoft.Azure.Cosmos](https://www.nuget.org/packages/Aspire.Microsoft.Azure.Cosmos) | A library for accessing [Azure Cosmos DB](/azure/cosmos-db/introduction) databases. |
| [Azure Event Hubs](../messaging/azure-event-hubs-integration.md) <br/> <img src="media/icons/AzureEventHubs_256x.png" alt="Azure Event Hubs logo." role="presentation" width="64" data-linktype="relative-path"> | [Aspire.Azure.Messaging.EventHubs](https://www.nuget.org/packages/Aspire.Azure.Messaging.EventHubs) | A library for accessing [Azure Event Hubs](/azure/event-hubs/event-hubs-about). |
| [Azure Key Vault](../security/azure-security-key-vault-integration.md) <br/> <img src="media/icons/AzureKeyVault_256x.png" alt="Azure Key Vault logo." role="presentation" width="64" data-linktype="relative-path"> | [Aspire.Azure.Security.KeyVault](https://www.nuget.org/packages/Aspire.Azure.Security.KeyVault) | A library for accessing [Azure Key Vault](/azure/key-vault/general/overview). |
| [Azure Search Documents](../azureai/azureai-search-document-integration.md) <br/> <img src="media/icons/AzureSearch_256x.png" alt="Azure Search Documents logo." role="presentation" width="64" data-linktype="relative-path"> | [Aspire.Azure.Search.Documents](https://www.nuget.org/packages/Aspire.Azure.Search.Documents) | A library for accessing [Azure AI Search](/azure/search/search-what-is-azure-search). |
| [Azure Service Bus](../messaging/azure-service-bus-integration.md) <br/> <img src="media/icons/AzureServiceBus_256x.png" alt="Azure Service Bus logo." role="presentation" width="64" data-linktype="relative-path"> | [Aspire.Azure.Messaging.ServiceBus](https://www.nuget.org/packages/Aspire.Azure.Messaging.ServiceBus) | A library for accessing [Azure Service Bus](/azure/service-bus-messaging/service-bus-messaging-overview). |
| [Azure Storage Queues](../storage/azure-storage-queues-integration.md) <br/> <img src="media/icons/AzureStorageQueue_256x.png" alt="Azure Storage Queues logo." role="presentation" width="64" data-linktype="relative-path"> | [Aspire.Azure.Storage.Queues](https://www.nuget.org/packages/Aspire.Azure.Storage.Queues) | A library for accessing [Azure Storage Queues](/azure/storage/queues/storage-queues-introduction). |
| [Azure Table Storage](../storage/azure-storage-tables-integration.md) <br/> <img src="media/icons/AzureTable_256x.png" alt="Azure Table Storage logo." role="presentation" width="64" data-linktype="relative-path"> | [Aspire.Azure.Data.Tables](https://www.nuget.org/packages/Aspire.Azure.Data.Tables) | A library for accessing the [Azure Table](/azure/storage/tables/table-storage-overview) service. |
| [Azure Web PubSub](../messaging/azure-web-pubsub-integration.md) <br/> <img src="media/icons/AzureWebPubSub_256x.png" alt="Azure Web PubSub logo." role="presentation" width="64" data-linktype="relative-path"> | [Aspire.Azure.Messaging.WebPubSub](https://www.nuget.org/packages/Aspire.Azure.Messaging.WebPubSub) | A library for accessing the [Azure Web PubSub](/azure/azure-web-pubsub/) service. |
| [Elasticsearch](../search/elasticsearch-integration.md) <br/> <img src="media/icons/Elastic_logo_256x.png" alt="Elasticsearch logo." role="presentation" width="64" data-linktype="relative-path"> | [Aspire.Elastic.Clients.Elasticsearch](https://www.nuget.org/packages/Aspire.Elastic.Clients.Elasticsearch) | A library for accessing [Elasticsearch](https://www.elastic.co/guide/en/elasticsearch/client/index.html) databases. |
| [Keycloak](../authentication/keycloak-integration.md) <br/> <img src="media/icons/Aspire-logo-256.png" alt=".NET Aspire logo." role="presentation" width="64" data-linktype="relative-path"> | [Aspire.Keycloak.Authentication](https://www.nuget.org/packages/Aspire.Keycloak.Authentication) | A library for accessing [Keycloak](https://www.keycloak.org/docs/latest/server_admin/index.html) authentication. |
| [Milvus](../database/milvus-integration.md) <br/> <img src="media/icons/Milvus_256x.png" alt="Milvus logo." role="presentation" width="64" data-linktype="relative-path"> | [Aspire.Milvus.Client](https://www.nuget.org/packages/Aspire.Milvus.Client) | A library for accessing [Milvus](https://milvus.io/) databases. |
| [MongoDB Driver](../database/mongodb-integration.md) <br/> <img src="media/icons/MongoDB_256px.png" alt="MongoDB logo." role="presentation" width="64" data-linktype="relative-path"> | [Aspire.MongoDB.Driver](https://www.nuget.org/packages/Aspire.MongoDB.Driver) | A library for accessing [MongoDB](https://www.mongodb.com/docs) databases. |
| [MySqlConnector](../database/mysql-integration.md) <br/> <img src="media/icons/mysqlconnector_logo.png" alt="MySqlConnector logo." role="presentation" width="64" data-linktype="relative-path"> | [Aspire.MySqlConnector](https://www.nuget.org/packages/Aspire.MySqlConnector) | A library for accessing [MySqlConnector](https://mysqlconnector.net/) databases. |
| [NATS](../messaging/nats-integration.md) <br/> <img src="media/icons/nats-icon.png" alt="NATS logo." role="presentation" width="64" data-linktype="relative-path"> | [Aspire.NATS.Net](https://www.nuget.org/packages/Aspire.NATS.Net) | A library for accessing [NATS](https://nats.io/) messaging. |
| [Oracle Entity Framework Core](../database/oracle-entity-framework-integration.md) <br/> <img src="media/icons/Aspire-logo-256.png" alt=".NET Aspire logo." role="presentation" width="64" data-linktype="relative-path"> | [Aspire.Oracle.EntityFrameworkCore](https://www.nuget.org/packages/Aspire.Oracle.EntityFrameworkCore) | A library for accessing Oracle databases with [Entity Framework Core](/ef/core). |
| [Pomelo MySQL Entity Framework Core](../database/mysql-entity-framework-integration.md) <br/> <img src="media/icons/Aspire-logo-256.png" alt=".NET Aspire logo." role="presentation" width="64" data-linktype="relative-path"> | [Aspire.Pomelo.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Aspire.Pomelo.EntityFrameworkCore.MySql) | A library for accessing MySql databases with [Entity Framework Core](/ef/core). |
| [PostgreSQL Entity Framework Core](../database/postgresql-entity-framework-integration.md) <br/> <img src="media/icons/PostgreSQL_logo.3colors.256x.png" alt="PostgreSQL logo." role="presentation" width="64" data-linktype="relative-path"> | [Aspire.Npgsql.EntityFrameworkCore.PostgreSQL](https://www.nuget.org/packages/Aspire.Npgsql.EntityFrameworkCore.PostgreSQL) | A library for accessing PostgreSQL databases using [Entity Framework Core](https://www.npgsql.org/efcore/index.html). |
| [PostgreSQL](../database/postgresql-integration.md) <br/> <img src="media/icons/PostgreSQL_logo.3colors.256x.png" alt="PostgreSQL logo." role="presentation" width="64" data-linktype="relative-path"> | [Aspire.Npgsql](https://www.nuget.org/packages/Aspire.Npgsql) | A library for accessing [PostgreSQL](https://www.npgsql.org/doc/index.html) databases. |
| [Qdrant](../database/qdrant-integration.md) <br/> <img src="media/icons/QdrantLogo_256x.png" alt="Qdrant logo." role="presentation" width="64" data-linktype="relative-path"> | [Aspire.Qdrant.Client](https://www.nuget.org/packages/Aspire.Qdrant.Client) | A library for accessing [Qdrant](https://qdrant.tech/) databases. |
| [RabbitMQ](../messaging/rabbitmq-client-integration.md) <br/> <img src="media/icons/Aspire-logo-256.png" alt=".NET Aspire logo." role="presentation" width="64" data-linktype="relative-path"> | [Aspire.RabbitMQ.Client](https://www.nuget.org/packages/Aspire.RabbitMQ.Client) | A library for accessing [RabbitMQ](https://www.rabbitmq.com/dotnet.html). |
| [Redis Distributed Caching](../caching/stackexchange-redis-distributed-caching-integration.md) <br/> <img src="media/icons/redis-cube-red_white-rgb.png" alt="Redis logo." role="presentation" width="64" data-linktype="relative-path"> | [Aspire.StackExchange.Redis.DistributedCaching](https://www.nuget.org/packages/Aspire.StackExchange.Redis.DistributedCaching) | A library for accessing [Redis](https://stackexchange.github.io/StackExchange.Redis/) caches for [distributed caching](/aspnet/core/performance/caching/distributed). |
| [Redis Output Caching](../caching/stackexchange-redis-output-caching-integration.md) <br/> <img src="media/icons/redis-cube-red_white-rgb.png" alt="Redis logo." role="presentation" width="64" data-linktype="relative-path"> | [Aspire.StackExchange.Redis.OutputCaching](https://www.nuget.org/packages/Aspire.StackExchange.Redis.OutputCaching) | A library for accessing [Redis](https://stackexchange.github.io/StackExchange.Redis/) caches for [output caching](/aspnet/core/performance/caching/output). |
| [Redis](../caching/stackexchange-redis-integration.md) <br/> <img src="media/icons/redis-cube-red_white-rgb.png" alt="Redis logo." role="presentation" width="64" data-linktype="relative-path"> | [Aspire.StackExchange.Redis](https://www.nuget.org/packages/Aspire.StackExchange.Redis) | A library for accessing [Redis](https://stackexchange.github.io/StackExchange.Redis/) caches. |
| [Seq](../logging/seq-integration.md) <br/> <img src="media/icons/Seq_logo.256x.png" alt="Seq logo." role="presentation" width="64" data-linktype="relative-path"> | [Aspire.Seq](https://www.nuget.org/packages/Aspire.Seq) | A library for logging to [Seq](https://datalust.co/seq). |
| [SQL Server Entity Framework Core](../database/sql-server-entity-framework-integration.md) <br/> <img src="media/icons/SQL_256x.png" alt="SQL logo." role="presentation" width="64" data-linktype="relative-path"> | [Aspire.Microsoft.EntityFrameworkCore.SqlServer](https://www.nuget.org/packages/Aspire.Microsoft.EntityFrameworkCore.SqlServer) | A library for accessing [SQL Server databases using Entity Framework Core](/ef/core/providers/sql-server/). |
| [SQL Server](../database/sql-server-integration.md) <br/> <img src="media/icons/SQL_256x.png" alt="SQL logo." role="presentation" width="64" data-linktype="relative-path"> | [Aspire.Microsoft.Data.SqlClient](https://www.nuget.org/packages/Aspire.Microsoft.Data.SqlClient) | A library for accessing [SQL Server](/sql/sql-server/) databases. |
<!-- markdownlint-enable MD033 MD045 -->

For more information on working with .NET Aspire integrations in Visual Studio, see [Visual Studio tooling](setup-tooling.md#visual-studio-tooling).

## Explore a sample integration workflow

.NET Aspire integrations streamline the process of consuming popular services and platforms. For example, consider the **.NET Aspire project** template. With this template, you get the [AppHost](app-host-overview.md) and [ServiceDefaults](service-defaults.md) projects. Imagine that you have a need for a worker service to perform some database processing. You could use the [.NET Aspire PostgreSQL integration](../database/postgresql-integration.md) to connect to and utilize a PostgreSQL database. The database could be hosted on-prem or in a cloud service such as Azure, AWS, or GCP. The following steps demonstrate how to integrate this integration into your app:

1. In the integration consuming (worker service) project, install the [Aspire.Npgsql](https://www.nuget.org/packages/Aspire.Npgsql) NuGet package.

    # [.NET CLI](#tab/dotnet-cli)

    ```dotnetcli
    dotnet add package Aspire.Npgsql
    ```

    # [PackageReference](#tab/package-reference)

    ```xml
    <PackageReference Include="Aspire.Npgsql" Version="[SelectVersion]" />
    ```

    ---

    For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

1. In the _:::no-loc text="Program.cs":::_ file of your worker service project, call the <xref:Microsoft.Extensions.Hosting.AspirePostgreSqlNpgsqlExtensions.AddNpgsqlDataSource%2A> extension method to register `NpgsqlDataSource` as a service.

    :::code source="snippets/integrations/AspireApp/WorkerService/Program.cs" highlight="5":::

    The preceding code adds the `NpgsqlDataSource` to the dependency injection container with the connection name of `"customers"`. The connection name is later used by the orchestrator project, when expressing resource dependencies.

    > [!TIP]
    > Components that are designed to connect to Azure services also support passwordless authentication and authorization using [Azure RBAC](/azure/role-based-access-control/overview), which is the recommended approach for production apps.

1. In your app host project (the project with the _*.AppHost_ suffix), add a reference to the worker service project. If you're using Visual Studio, you can use the [**Add .NET Aspire Orchestrator Support**](setup-tooling.md#add-orchestration-projects) project context menu item to add the reference automatically. The following code snippet shows the project reference of the _AspireApp.AppHost.csproj_:

    :::code language="xml" source="snippets/integrations/AspireApp/AspireApp.AppHost/AspireApp.AppHost.csproj" highlight="17":::

    After the worker service is referenced by the orchestrator project, the worker service project has its _:::no-loc text="Program.cs":::_ file updated to call the `AddServiceDefaults` method. For more information on service defaults, see [Service defaults](service-defaults.md).

1. In the orchestrator project, update the _:::no-loc text="Program.cs":::_ file with the following code:

    :::code source="snippets/integrations/AspireApp/AspireApp.AppHost/Program.cs" highlight="3-4,6-8":::

    The preceding code:

    - Calls <xref:Aspire.Hosting.PostgresBuilderExtensions.AddPostgres%2A> and chains a call to <xref:Aspire.Hosting.PostgresBuilderExtensions.AddDatabase%2A>, adding a PostgreSQL database container to the app model with a database named `"customers"`.
    - Chains calls on the result of the <xref:Aspire.Hosting.ProjectResourceBuilderExtensions.AddProject%2A> from the worker service project:
      - Calls <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> to add a reference to the `database`.
      - Calls <xref:Aspire.Hosting.ProjectResourceBuilderExtensions.WithReplicas%2A> to set the number of replicas to `3`.

1. Inject the `NpgsqlDataSource` object into the `Worker` to run commands against the database:

    :::code source="snippets/integrations/AspireApp/WorkerService/Worker.cs" highlight="7,13":::

You now have a fully configured PostgreSQL database integration and corresponding container with connection integrated into your app! This integration also configured health checks, logging, metrics, retries, and other useful capabilities for you behind the scenes. .NET Aspire integrations provide various options to configure each of these features.

## Configure .NET Aspire integrations

.NET Aspire integrations implement a consistent configuration experience via <xref:Microsoft.Extensions.Configuration.IConfiguration> and <xref:Microsoft.Extensions.Options.IOptions%601>. Configuration is schematized and part of a integration's contract, ensuring backward compatibility across versions of the integration. You can set up every .NET Aspire integration through either JSON configuration files or directly through code using delegates. JSON files must follow a standardized naming convention based on the Component name.

For example, add the following code to the _:::no-loc text="appsettings.json":::_ file to configure the PostgreSQL integration:

```json
{
  "Aspire": {
    "Npgsql": {
      "DisableHealthChecks": true,
      "DisableTracing": true
    }
  }
}
```

Alternatively, you can configure the integration directly in your code using a delegate:

```csharp
builder.AddNpgsqlDataSource(
    "PostgreSqlConnection",
    static settings => settings.DisableHealthChecks  = true);
```

## Dependency injection

.NET Aspire integrations automatically register essential services with the .NET dependency container using the proper scope. This allows key integration classes and services to be injected throughout your code. For example, the .NET Aspire PostgreSQL integration makes available the `NpgsqlDataSource` to inject into your application layers and run commands against a database:

```csharp
public class ExampleService(NpgsqlDataSource dataSource)
{
}
```

For more information, see [.NET dependency injection](/dotnet/core/extensions/dependency-injection).

### Keyed services

.NET Aspire integrations also support keyed dependency injection. In this scenario, the service name for keyed dependency injection will be the same as the connection name:

```csharp
builder.AddKeyedNpgsqlDataSource(
    "PostgreSqlConnection",
    static settings => settings.DisableHealthChecks  = true);
```

You can then retrieve the registered service using the <xref:Microsoft.Extensions.DependencyInjection.FromKeyedServicesAttribute>:

```csharp
public class ExampleService(
    [FromKeyedServices("PostgreSqlConnection")] NpgsqlDataSource npgContext)
{
}
```

For more information, see [Dependency injection in .NET: Keyed services](/dotnet/core/extensions/dependency-injection#keyed-services).

## Cloud-native features

Cloud-native applications surface many unique requirements and concerns. The core features of .NET Aspire orchestration and integrations are designed to handle many cloud-native concerns for you with minimal configurations. Some of the key features include:

- [Orchestration](app-host-overview.md): A lightweight, extensible, and cross-platform app host for .NET Aspire projects. The app host provides a consistent configuration and dependency injection experience for .NET Aspire integrations.
- [Service discovery](../service-discovery/overview.md): A technique for locating services within a distributed application. Service discovery is a key integration of microservice architectures.
- [Service defaults](service-defaults.md): A set of default configurations intended for sharing amongst resources within .NET Aspire projects. These defaults are designed to work well in most scenarios and can be customized as needed.

Some .NET Aspire integrations also include more capabilities for specific services or platforms, which can be found in the integration specific reference docs.

### Observability and telemetry

.NET Aspire integrations automatically set up Logging, Tracing, and Metrics configurations, which are sometimes known as _the pillars of observability_.

- **[Logging](/dotnet/core/diagnostics/logging-tracing)**: A technique where code is instrumented to produce logs of interesting events that occurred while the program was running. A baseline set of log events are enabled for .NET Aspire integrations by default and more extensive logging can be enabled on-demand to diagnose particular problems.

- **[Tracing](/dotnet/core/diagnostics/distributed-tracing)**: A specialized form of logging that helps you localize failures and performance issues within applications distributed across multiple machines or processes. This technique tracks requests through an application to correlate work done by different application integrations and separate it from other work the application may be doing for concurrent requests.

- **[Metrics](/dotnet/core/diagnostics/metrics)**: Numerical measurements recorded over time to monitor application performance and health. Metrics are often used to generate alerts when potential problems are detected. Metrics have low performance overhead and many services configure them as always-on telemetry.

Together, these types of telemetry allow you to gain insights into your application's behavior and performance using various monitoring and analysis tools. Depending on the backing service, some integrations may only support some of these features. For example, some integrations support logging and tracing, but not metrics. Telemetry features can also be disabled. For more information, see [.NET Aspire service defaults](service-defaults.md).

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