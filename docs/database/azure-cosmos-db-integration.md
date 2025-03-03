---
title: .NET Aspire Azure Cosmos DB integration
description: Learn how to install and configure the .NET Aspire Azure Cosmos DB integration to connect to existing Cosmos DB instances or create new instances from .NET with the Azure Cosmos DB emulator.
ms.date: 02/26/2025
uid: dotnet/aspire/azure-cosmos-db-integration
---

# .NET Aspire Azure Cosmos DB integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[Azure Cosmos DB](https://azure.microsoft.com/services/cosmos-db/) is a fully managed NoSQL database service for modern app development. The .NET Aspire Azure Cosmos DB integration enables you to connect to existing Cosmos DB instances or create new instances from .NET with the Azure Cosmos DB emulator.

## Hosting integration

[!INCLUDE [cosmos-app-host](includes/cosmos-app-host.md)]

### Hosting integration health checks

The Azure Cosmos DB hosting integration automatically adds a health check for the Cosmos DB resource. The health check verifies that the Cosmos DB is running and that a connection can be established to it.

The hosting integration relies on the [📦 AspNetCore.HealthChecks.CosmosDb](https://www.nuget.org/packages/AspNetCore.HealthChecks.CosmosDb) NuGet package.

## Client integration

To get started with the .NET Aspire Azure Cosmos DB client integration, install the [📦 Aspire.Microsoft.Azure.Cosmos](https://www.nuget.org/packages/Aspire.Microsoft.Azure.Cosmos) NuGet package in the client-consuming project, that is, the project for the application that uses the Cosmos DB client. The Cosmos DB client integration registers a <xref:Microsoft.Azure.Cosmos.CosmosClient> instance that you can use to interact with Cosmos DB.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Microsoft.Azure.Cosmos
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Microsoft.Azure.Cosmos"
                  Version="*" />
```

---

### Add Cosmos DB client

In the :::no-loc text="Program.cs"::: file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireMicrosoftAzureCosmosExtensions.AddAzureCosmosClient*> extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register a <xref:Azure.Cosmos.CosmosClient> for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddAzureCosmosClient(connectionName: "cosmos-db");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the Cosmos DB resource in the app host project. In other words, when you call `AddAzureCosmosDB` and provide a name of `cosmos-db` that same name should be used when calling `AddAzureCosmosClient`. For more information, see [Add Azure Cosmos DB resource](#add-azure-cosmos-db-resource).

You can then retrieve the <xref:Azure.Cosmos.CosmosClient> instance using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(CosmosClient client)
{
    // Use client...
}
```

For more information on dependency injection, see [.NET dependency injection](/dotnet/core/extensions/dependency-injection).

### Add keyed Cosmos DB client

There might be situations where you want to register multiple `CosmosClient` instances with different connection names. To register keyed Cosmos DB clients, call the <xref:Microsoft.Extensions.Hosting.AspireMicrosoftAzureCosmosExtensions.AddKeyedAzureCosmosClient*> method:

```csharp
builder.AddKeyedAzureCosmosClient(name: "mainDb");
builder.AddKeyedAzureCosmosClient(name: "loggingDb");
```

> [!IMPORTANT]
> When using keyed services, it's expected that your Cosmos DB resource configured two named databases, one for the `mainDb` and one for the `loggingDb`.

Then you can retrieve the `CosmosClient` instances using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(
    [FromKeyedServices("mainDb")] CosmosClient mainDbClient,
    [FromKeyedServices("loggingDb")] CosmosClient loggingDbClient)
{
    // Use clients...
}
```

For more information on keyed services, see [.NET dependency injection: Keyed services](/dotnet/core/extensions/dependency-injection#keyed-services).

### Configuration

The .NET Aspire Azure Cosmos DB integration provides multiple options to configure the connection based on the requirements and conventions of your project.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling the <xref:Microsoft.Extensions.Hosting.AspireMicrosoftAzureCosmosExtensions.AddAzureCosmosClient*> method:

```csharp
builder.AddAzureCosmosClient("cosmos-db");
```

Then the connection string is retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "cosmos-db": "AccountEndpoint=https://{account_name}.documents.azure.com:443/;AccountKey={account_key};"
  }
}
```

For more information on how to format this connection string, see the ConnectionString documentation.

#### Use configuration providers

The .NET Aspire Azure Cosmos DB integration supports <xref:Microsoft.Extensions.Configuration>. It loads the <xref:Aspire.Microsoft.Azure.Cosmos.MicrosoftAzureCosmosSettings> from configuration by using the `Aspire:Microsoft:Azure:Cosmos` key. The following snippet is an example of a :::no-loc text="appsettings.json"::: file that configures some of the options:

```json
{
  "Aspire": {
    "Microsoft": {
      "Azure": {
        "Cosmos": {
          "DisableTracing": false,
        }
      }
    }
  }
}
```

For the complete Cosmos DB client integration JSON schema, see [Aspire.Microsoft.Azure.Cosmos/ConfigurationSchema.json](https://github.com/dotnet/aspire/blob/v9.0.0/src/Components/Aspire.Microsoft.Azure.Cosmos/ConfigurationSchema.json).

#### Use inline delegates

Also you can pass the `Action<MicrosoftAzureCosmosSettings> configureSettings` delegate to set up some or all the options inline, for example to disable tracing from code:

```csharp
builder.AddAzureCosmosClient(
    "cosmos-db",
    static settings => settings.DisableTracing = true);
```

You can also set up the <xref:Microsoft.Azure.Cosmos.CosmosClientOptions?displayProperty=fullName> using the optional `Action<CosmosClientOptions> configureClientOptions` parameter of the `AddAzureCosmosClient` method. For example to set the <xref:Microsoft.Azure.Cosmos.CosmosClientOptions.ApplicationName?displayProperty=nameWithType> user-agent header suffix for all requests issues by this client:

```csharp
builder.AddAzureCosmosClient(
    "cosmosConnectionName",
    configureClientOptions:
        clientOptions => clientOptions.ApplicationName = "myapp");
```

### Client integration health checks

The .NET Aspire Cosmos DB client integration currently doesn't implement health checks, though this may change in future releases.

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

#### Logging

The .NET Aspire Azure Cosmos DB integration uses the following log categories:

- `Azure-Cosmos-Operation-Request-Diagnostics`

In addition to getting Azure Cosmos DB request diagnostics for failed requests, you can configure latency thresholds to determine which successful Azure Cosmos DB request diagnostics will be logged. The default values are 100 ms for point operations and 500 ms for non point operations.

```csharp
builder.AddAzureCosmosClient(
    "cosmosConnectionName",
    configureClientOptions:
        clientOptions => {
            clientOptions.CosmosClientTelemetryOptions = new()
            {
                CosmosThresholdOptions = new()
                {
                    PointOperationLatencyThreshold = TimeSpan.FromMilliseconds(50),
                    NonPointOperationLatencyThreshold = TimeSpan.FromMilliseconds(300)
                }
            };
        });
```

#### Tracing

The .NET Aspire Azure Cosmos DB integration will emit the following tracing activities using OpenTelemetry:

- `Azure.Cosmos.Operation`

Azure Cosmos DB tracing is currently in preview, so you must set the experimental switch to ensure traces are emitted.

```csharp
AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);
```

For more information, see [Azure Cosmos DB SDK observability: Trace attributes](/azure/cosmos-db/nosql/sdk-observability?tabs=dotnet#trace-attributes).

#### Metrics

The .NET Aspire Azure Cosmos DB integration currently doesn't support metrics by default due to limitations with the Azure SDK.

## See also

- [Azure Cosmos DB](https://azure.microsoft.com/services/cosmos-db)
- [.NET Aspire Cosmos DB Entity Framework Core integration](azure-cosmos-db-entity-framework-integration.md)
- [.NET Aspire integrations overview](../fundamentals/integrations-overview.md)
- [.NET Aspire Azure integrations overview](../azure/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
