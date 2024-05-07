---
title: .NET Aspire preview 7
description: .NET Aspire preview 7 is now available and includes breaking changes.
ms.date: 05/07/2024
---

# .NET Aspire preview 7

.NET Aspire preview 7 wasn't part of the original plan, but as a developer, you might appreciate being about to react quickly to changes in the ecosystem. This preview has a lot of breaking API changes, partly due to the fact that once the product is released, we'll be committed to a stable API surface. Suffice it to say, the team was eager to ensure that they got these changes in before the final release.

## Breaking changes

All of the component settings classes have been updated to revise the naming of their shared properties. This change was made to ensure that the settings classes are more consistent and easier to use. The following lists all the settings classes impacted by this change:

- <xref:Aspire.Azure.AI.OpenAI.AzureOpenAISettings>
- <xref:Aspire.Azure.Data.Tables.AzureDataTablesSettings>
- <xref:Aspire.Azure.Messaging.EventHubs.AzureMessagingEventHubsSettings> (formerly `AzureMessagingEventHubsBaseSettings`)
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
| `Tracing`         | `DisableTracing`      | `false`       |
| `Metrics`         | `DisableMetrics`      | `false`       |
| `Retry`           | `DisableRetry`        | `false`       |

> [!TIP]
> This is a negation change. If you were previously setting `HealthChecks` to `true`, and you upgrade to .NET Aspire preview 7, when the property name is updated you should now set `DisableHealthChecks` to `false`.

The Azure Service Bus component renamed its `Azure:Messaging:<INSTANCE_NAME>:Namespace` property to `Azure:Messaging:<INSTANCE_NAME>:FullyQualifiedNamespace`. This change was made to ensure that the property name aligns with what the property has always represented, the fully qualified namespace of the Azure Service Bus instance.

The following classes were renamed:

- The `AzureMessagingEventHubsBaseSettings` abstract base class was renamed to `AzureMessagingEventHubsSettings` class.
- The `AzureCosmosDBSettings` class was renamed to `MicrosoftAzureCosmosDBSettings`.
