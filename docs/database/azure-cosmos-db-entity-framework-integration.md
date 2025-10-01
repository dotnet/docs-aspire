---
title: Aspire Cosmos DB Entity Framework Core integration
description: Learn how to install and configure the Aspire Cosmos DB Entity Framework Core integration to connect to existing Cosmos DB instances or create new instances from .NET with the Azure Cosmos DB emulator.
ms.date: 04/01/2025
uid: dotnet/aspire/azure-cosmos-db-entity-framework-integration
ms.custom: sfi-ropc-nochange
---

# Aspire Cosmos DB Entity Framework Core integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[Azure Cosmos DB](https://azure.microsoft.com/services/cosmos-db/) is a fully managed NoSQL database service for modern app development. The Aspire Cosmos DB Entity Framework Core integration enables you to connect to existing Cosmos DB instances or create new instances from .NET with the Azure Cosmos DB emulator.

## Hosting integration

[!INCLUDE [cosmos-app-host](includes/cosmos-app-host.md)]

### Hosting integration health checks

The Azure Cosmos DB hosting integration automatically adds a health check for the Cosmos DB resource. The health check verifies that the Cosmos DB is running and that a connection can be established to it.

The hosting integration relies on the [📦 AspNetCore.HealthChecks.CosmosDb](https://www.nuget.org/packages/AspNetCore.HealthChecks.CosmosDb) NuGet package.

## Client integration

To get started with the Aspire Microsoft Entity Framework Core Cosmos DB integration, install the [📦 Aspire.Microsoft.EntityFrameworkCore.Cosmos](https://www.nuget.org/packages/Aspire.Microsoft.EntityFrameworkCore.Cosmos) NuGet package in the client-consuming project, i.e., the project for the application that uses the Microsoft Entity Framework Core Cosmos DB client.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Microsoft.EntityFrameworkCore.Cosmos
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Microsoft.EntityFrameworkCore.Cosmos"
                  Version="*" />
```

---

### Add Cosmos DB context

In the :::no-loc text="Program.cs"::: file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireAzureEFCoreCosmosExtensions.AddCosmosDbContext%2A> extension method to register a <xref:System.Data.Entity.DbContext?displayProperty=fullName> for use via the dependency injection container. The method takes a connection name parameter and a database name parameter.

```csharp
builder.AddCosmosDbContext<MyDbContext>("cosmosdb", "databaseName");
```

Alternatively, the database name can be inferred from the connection when there's a single database in the connection string. In this case, you can omit the database name parameter:

```csharp
builder.AddCosmosDbContext<MyDbContext>("cosmosdb");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the Cosmos DB resource in the AppHost project. In other words, when you call `AddAzureCosmosDB` and provide a name of `cosmosdb` that same name should be used when calling `AddCosmosDbContext`. For more information, see [Add Azure Cosmos DB resource](#add-azure-cosmos-db-resource).

You can then retrieve the <xref:Microsoft.EntityFrameworkCore.DbContext> instance using dependency injection. For example, to retrieve the client from a service:

```csharp
public class ExampleService(MyDbContext context)
{
    // Use context...
}
```

For more information on using Entity Framework Core with Azure Cosmos DB, see the [Examples for Azure Cosmos DB for NoSQL SDK for .NET](/ef/core/providers/cosmos/?tabs=dotnet-core-cli).

### Configuration

The Aspire Microsoft Entity Framework Core Cosmos DB integration provides multiple options to configure the Azure Cosmos DB connection based on the requirements and conventions of your project.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddCosmosDbContext`:

```csharp
builder.AddCosmosDbContext<MyDbContext>("CosmosConnection");
```

And then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "CosmosConnection": "AccountEndpoint=https://{account_name}.documents.azure.com:443/;AccountKey={account_key};"
  }
}
```

For more information, see the [ConnectionString documentation](/azure/cosmos-db/nosql/how-to-dotnet-get-started#connect-with-a-connection-string).

#### Use configuration providers

The Aspire Microsoft Entity Framework Core Cosmos DB integration supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the <xref:Aspire.Microsoft.EntityFrameworkCore.Cosmos.EntityFrameworkCoreCosmosSettings> from configuration files such as _:::no-loc text="appsettings.json":::_. Example _:::no-loc text="appsettings.json"::: that configures some of the options:

```json
{
  "Aspire": {
    "Microsoft": {
      "EntityFrameworkCore": {
        "Cosmos": {
          "DisableTracing": true
        }
      }
    }
  }
}
```

For the complete Cosmos DB client integration JSON schema, see [Aspire.Microsoft.EntityFrameworkCore.Cosmos/ConfigurationSchema.json](https://github.com/dotnet/aspire/blob/v9.1.0/src/Components/Aspire.Microsoft.EntityFrameworkCore.Cosmos/ConfigurationSchema.json).

#### Use inline delegates

You can also pass the `Action<EntityFrameworkCoreCosmosSettings> configureSettings` delegate to set up some or all the <xref:Aspire.Microsoft.EntityFrameworkCore.Cosmos.EntityFrameworkCoreCosmosSettings> options inline, for example to disable tracing from code:

```csharp
builder.AddCosmosDbContext<MyDbContext>(
    "cosmosdb",
    settings => settings.DisableTracing = true);
```

### Client integration health checks

The Aspire Microsoft Entity Framework Core Cosmos DB integration currently doesn't implement health checks, though this may change in future releases.

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

#### Logging

The Aspire Microsoft Entity Framework Core Cosmos DB integration uses the following log categories:

- `Azure-Cosmos-Operation-Request-Diagnostics`
- `Microsoft.EntityFrameworkCore.ChangeTracking`
- `Microsoft.EntityFrameworkCore.Database.Command`
- `Microsoft.EntityFrameworkCore.Infrastructure`
- `Microsoft.EntityFrameworkCore.Query`

#### Tracing

The Aspire Microsoft Entity Framework Core Cosmos DB integration will emit the following tracing activities using OpenTelemetry:

- `Azure.Cosmos.Operation`
- `OpenTelemetry.Instrumentation.EntityFrameworkCore`

#### Metrics

The Aspire Microsoft Entity Framework Core Cosmos DB integration currently supports the following metrics:

- `Microsoft.EntityFrameworkCore`
  - `ec_Microsoft_EntityFrameworkCore_active_db_contexts`
  - `ec_Microsoft_EntityFrameworkCore_total_queries`
  - `ec_Microsoft_EntityFrameworkCore_queries_per_second`
  - `ec_Microsoft_EntityFrameworkCore_total_save_changes`
  - `ec_Microsoft_EntityFrameworkCore_save_changes_per_second`
  - `ec_Microsoft_EntityFrameworkCore_compiled_query_cache_hit_rate`
  - `ec_Microsoft_Entity_total_execution_strategy_operation_failures`
  - `ec_Microsoft_E_execution_strategy_operation_failures_per_second`
  - `ec_Microsoft_EntityFramew_total_optimistic_concurrency_failures`
  - `ec_Microsoft_EntityF_optimistic_concurrency_failures_per_second`

## See also

- [Azure Cosmos DB docs](/azure/cosmos-db/introduction)
- [Aspire integrations](../fundamentals/integrations-overview.md)
- [Aspire GitHub repo](https://github.com/dotnet/aspire)
