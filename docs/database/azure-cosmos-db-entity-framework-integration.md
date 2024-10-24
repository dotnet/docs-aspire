---
title: .NET Aspire Microsoft Entity Framework Core Cosmos DB integration
description: This article describes the .NET Aspire Microsoft Entity Framework Core Cosmos DB integration features and capabilities.
ms.topic: how-to
ms.date: 08/12/2024
---

# .NET Aspire Microsoft Entity Framework Core Cosmos DB integration

In this article, you learn how to use the .NET Aspire Microsoft Entity Framework Core Cosmos DB integration. The `Aspire.Microsoft.EntityFrameworkCore.Cosmos` library is used to register a <xref:System.Data.Entity.DbContext?displayProperty=fullName> as a singleton in the DI container for connecting to Azure Cosmos DB. It also enables corresponding health checks, logging and telemetry.

## Get started

To get started with the .NET Aspire Microsoft Entity Framework Core Cosmos DB integration, install the [Aspire.Microsoft.EntityFrameworkCore.Cosmos](https://www.nuget.org/packages/Aspire.Microsoft.EntityFrameworkCore.Cosmos) NuGet package in the client-consuming project, i.e., the project for the application that uses the Microsoft Entity Framework Core Cosmos DB client.

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

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireAzureEFCoreCosmosExtensions.AddCosmosDbContext%2A> extension to register a <xref:System.Data.Entity.DbContext?displayProperty=fullName> for use via the dependency injection container.

```csharp
builder.AddCosmosDbContext<MyDbContext>("cosmosdb");
```

You can then retrieve the <xref:Microsoft.EntityFrameworkCore.DbContext> instance using dependency injection. For example, to retrieve the client from a service:

```csharp
public class ExampleService(MyDbContext context)
{
    // Use context...
}
```

For more information on using Entity Framework Core with Azure Cosmos DB, see the [Examples for Azure Cosmos DB for NoSQL SDK for .NET](/ef/core/providers/cosmos/?tabs=dotnet-core-cli).

## App host usage

To add Azure Cosmos DB hosting support to your <xref:Aspire.Hosting.IDistributedApplicationBuilder>, install the [Aspire.Hosting.Azure.CosmosDB](https://www.nuget.org/packages/Aspire.Hosting.Azure.CosmosDB) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.CosmosDB
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.CosmosDB"
                  Version="*" />
```

---

In your app host project, register the .NET Aspire Microsoft Entity Framework Core Cosmos DB integration and consume the service using the following methods:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cosmos = builder.AddAzureCosmosDB("cosmos");
var cosmosdb = cosmos.AddDatabase("cosmosdb");

var exampleProject = builder.AddProject<Projects.ExampleProject>()
                            .WithReference(cosmosdb);
```

> [!TIP]
> To use the Azure Cosmos DB emulator, chain a call to the <xref:Aspire.Hosting.AzureCosmosExtensions.AddAzureCosmosDB%2A> method.
>
> ```csharp
> cosmosdb.RunAsEmulator();
> ```

## Configuration

The .NET Aspire Microsoft Entity Framework Core Cosmos DB integration provides multiple options to configure the Azure Cosmos DB connection based on the requirements and conventions of your project.

### Use a connection string

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

### Use configuration providers

The .NET Aspire Microsoft Entity Framework Core Cosmos DB integration supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the <xref:Aspire.Microsoft.EntityFrameworkCore.Cosmos.EntityFrameworkCoreCosmosSettings > from _:::no-loc text="appsettings.json":::_ or other configuration files using `Aspire:Microsoft:EntityFrameworkCore:Cosmos` key. Example _:::no-loc text="appsettings.json":::_ that configures some of the options:

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

### Use inline delegates

You can also pass the `Action<EntityFrameworkCoreCosmosSettings> configureSettings` delegate to set up some or all the <xref:Aspire.Microsoft.EntityFrameworkCore.Cosmos.EntityFrameworkCoreCosmosSettings> options inline, for example to disable tracing from code:

```csharp
builder.AddCosmosDbContext<MyDbContext>(
    "cosmosdb",
    settings => settings.DisableTracing = true);
```

[!INCLUDE [integration-health-checks](../includes/integration-health-checks.md)]

The .NET Aspire Microsoft Entity Framework Core Cosmos DB integration currently doesn't implement health checks, though this may change in future releases.

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

### Logging

The .NET Aspire Microsoft Entity Framework Core Cosmos DB integration uses the following log categories:

- Azure-Cosmos-Operation-Request-Diagnostics
- Microsoft.EntityFrameworkCore.ChangeTracking
- Microsoft.EntityFrameworkCore.Database.Command
- Microsoft.EntityFrameworkCore.Infrastructure
- Microsoft.EntityFrameworkCore.Query

### Tracing

The .NET Aspire Microsoft Entity Framework Core Cosmos DB integration will emit the following tracing activities using OpenTelemetry:

- Azure.Cosmos.Operation
- OpenTelemetry.Instrumentation.EntityFrameworkCore

### Metrics

The .NET Aspire Microsoft Entity Framework Core Cosmos DB integration currently supports the following metrics:

- Microsoft.EntityFrameworkCore"
  - ec_Microsoft_EntityFrameworkCore_active_db_contexts
  - ec_Microsoft_EntityFrameworkCore_total_queries
  - ec_Microsoft_EntityFrameworkCore_queries_per_second
  - ec_Microsoft_EntityFrameworkCore_total_save_changes
  - ec_Microsoft_EntityFrameworkCore_save_changes_per_second
  - ec_Microsoft_EntityFrameworkCore_compiled_query_cache_hit_rate
  - ec_Microsoft_Entity_total_execution_strategy_operation_failures
  - ec_Microsoft_E_execution_strategy_operation_failures_per_second
  - ec_Microsoft_EntityFramew_total_optimistic_concurrency_failures
  - ec_Microsoft_EntityF_optimistic_concurrency_failures_per_second

## See also

- [Azure Cosmos DB docs](/azure/cosmos-db/introduction)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
