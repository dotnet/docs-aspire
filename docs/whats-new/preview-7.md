---
title: .NET Aspire preview 7
description: .NET Aspire preview 7 is now available and includes breaking changes.
ms.date: 05/07/2024
---

# .NET Aspire preview 7

.NET Aspire preview 7 wasn't part of the original plan, but as a developer, you might appreciate being able to react quickly to changes in a quickly evolving development ecosystem. This preview has a lot of breaking API changes, partly due to the fact that once the product is released, we'll be committed to a stable API surface. Suffice it to say, the team was eager to ensure that they got these API changes in place before the final release.

The .NET Aspire preview 7 release version is **8.0.0-preview.7.24251.11**.

To update to the latest version, run the following commands:

```dotnetcli
dotnet workload update
dotnet workload install aspire
```

To validate that preview 7 is installed, run `dotnet workload list` to see the installed workloads:

```dotnetcli
dotnet workload list

Installed Workload Id    Manifest Version                    Installation Source
--------------------------------------------------------------------------------------------
aspire                   8.0.0-preview.7.24251.11/8.0.100    SDK 8.0.300, VS 17.10.34825.169
```

## Breaking changes

There were a number of breaking changes in this release. The following sections detail the changes that were made.

### Property renaming

All of the component settings classes have been updated to revise the naming of their shared properties. This change was made to ensure that the settings classes are more consistent and easier to use. The following lists all the settings classes impacted by this change:

- <xref:Aspire.Azure.AI.OpenAI.AzureOpenAISettings>
- <xref:Aspire.Azure.Data.Tables.AzureDataTablesSettings>
- `AzureMessagingEventHubsSettings` (formerly `AzureMessagingEventHubsBaseSettings`)
  - <xref:Aspire.Azure.Messaging.EventHubs.AzureMessagingEventHubsConsumerBaseSettings>
  - <xref:Aspire.Azure.Messaging.EventHubs.AzureMessagingEventHubsProducerSettings>
- <xref:Aspire.Azure.Search.Documents.AzureSearchSettings>
- <xref:Aspire.Azure.Security.KeyVault.AzureSecurityKeyVaultSettings>
- <xref:Aspire.Azure.Storage.Blobs.AzureStorageBlobsSettings>
- <xref:Aspire.Azure.Storage.Queues.AzureStorageQueuesSettings>
- <xref:Aspire.Confluent.Kafka.KafkaConsumerSettings>
- <xref:Aspire.Confluent.Kafka.KafkaProducerSettings>
- <xref:Aspire.Microsoft.Azure.Cosmos.AzureCosmosDBSettings>
- <xref:Aspire.Microsoft.Data.SqlClient.MicrosoftDataSqlClientSettings>
- <xref:Aspire.Microsoft.EntityFrameworkCore.Cosmos.EntityFrameworkCoreCosmosDBSettings>
- <xref:Aspire.Microsoft.EntityFrameworkCore.SqlServer.MicrosoftEntityFrameworkCoreSqlServerSettings>
- <xref:Aspire.MongoDB.Driver.MongoDBSettings>
- <xref:Aspire.MySqlConnector.MySqlConnectorSettings>
- <xref:Aspire.NATS.Net.NatsClientSettings>
- <xref:Aspire.Npgsql.NpgsqlSettings>
- <xref:Aspire.Npgsql.EntityFrameworkCore.PostgreSQL.NpgsqlEntityFrameworkCorePostgreSQLSettings>
- <xref:Aspire.Oracle.EntityFrameworkCore.OracleEntityFrameworkCoreSettings>
- <xref:Aspire.Pomelo.EntityFrameworkCore.MySql.PomeloEntityFrameworkCoreMySqlSettings>
- <xref:Aspire.RabbitMQ.Client.RabbitMQClientSettings>
- <xref:Aspire.Seq.SeqSettings>
- <xref:Aspire.StackExchange.Redis.StackExchangeRedisSettings>

The following table shows the changes that were made—not all properties exist in each settings class, but where they do, they were renamed as follows:

| Old property name | New property name     | Default value |
|-------------------|-----------------------|---------------|
| `HealthChecks`    | `DisableHealthChecks` | `false`       |
| `Metrics`         | `DisableMetrics`      | `false`       |
| `Retry`           | `DisableRetry`        | `false`       |
| `Tracing`         | `DisableTracing`      | `false`       |

> [!TIP]
> This is a negation change. If you were previously setting `HealthChecks` to `true`, and you upgrade to .NET Aspire preview 7, when the property name is updated you should now set `DisableHealthChecks` to `false`. If you never changed the default value, you don't need to do anything.

The Azure Service Bus component renamed its `Azure:Messaging:<INSTANCE_NAME>:Namespace` property to `Azure:Messaging:<INSTANCE_NAME>:FullyQualifiedNamespace`. This change was made to ensure that the property name aligns with what the property has always represented, the fully qualified namespace of the Azure Service Bus instance.

### Class renaming

The following classes were renamed:

- The `AzureMessagingEventHubsBaseSettings` abstract base class was renamed to `AzureMessagingEventHubsSettings` class.
- The `AzureCosmosDBSettings` class was renamed to `MicrosoftAzureCosmosDBSettings`.

### Method signature changes

When adding an Azure Cosmos DB client, the `AddAzureCosmosDBClient` method was renamed to `AddAzureCosmosClient`.

## Dashboard updates

The .NET Aspire dashboard continues to improve with each release. In this preview, the dashboard has updated several aesthetics and adds more localization.

## Container image default changes

.NET Aspire has always provided resources that express various container image dependencies in the [app host](../fundamentals/app-host-overview.md). All container images are now fully qualified with their registry and tag. The following table outlines the tag version changes for each container image and its corresponding resource:

| Container image | Resource type | Old tag | New tag |
|--|--|--|--|
| `docker.io/library/mysql` | `MySqlServerResource` | `8.3.0` | `8.3` |
| `docker.io/confluentinc/confluent-local` | `KafkaServerResource` | `7.6.0` | `7.6.1` |
| `docker.io/library/mongo` | `MongoDBServerResource` | `7.0.5` | `7.0` |
| `docker.io/library/mongo-express` | `MongoDBServerResource` | N/A | `1.0` |
| `docker.io/library/nats` | `MongoDBServerResource` | `7` | `2.10` |
| `docker.io/dpage/pgadmin4` | `PostgresServerResource` | N/A | `8.5` |
| `docker.io/qdrant/qdrant` | `PostgresServerResource` | `v1.8.3` | `v1.8.4` |
| `docker.io/library/rabbitmq` | `RabbitMQServerResource` | `3` | `3.13` |
| `docker.io/library/redis` | `RedisResource` | `7` | `7.2` |
| `docker.io/rediscommander/redis-commander` | `RedisResource` | N/A | `latest` |
| `docker.io/datalust/seq` | `SeqResource` | `2024.1` | `2024.2` |

> [!NOTE]
> All of these container image defaults can be overridden using the fluent API, chaining calls on the `IResourceBuilder<T>` interface, where `T` is the resource type you're configuring (and constrained to the `ContainerResource` type). The following APIs exist to support this customization:
>
> - <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithImage%2A>
> - <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithImageRegistry%2A>
> - <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithImageTag%2A>
