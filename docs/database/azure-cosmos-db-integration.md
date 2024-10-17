---
title: .NET Aspire Azure Cosmos DB integration
description: This article describes the .NET Aspire Azure Cosmos DB integration features and capabilities.
ms.topic: how-to
ms.date: 08/12/2024
---

# .NET Aspire Azure Cosmos DB integration

In this article, you learn how to use the .NET Aspire Azure Cosmos DB integration. The `Aspire.Microsoft.Azure.Cosmos` library is used to register a <xref:Microsoft.Azure.Cosmos.CosmosClient> as a singleton in the DI container for connecting to Azure Cosmos DB. It also enables corresponding health checks, logging and telemetry.

## Get started

To get started with the .NET Aspire Azure Cosmos DB integration, install the [Aspire.Microsoft.Azure.Cosmos](https://www.nuget.org/packages/Aspire.Microsoft.Azure.Cosmos) NuGet package in the client-consuming project, i.e., the project for the application that uses the Azure Cosmos DB client.

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

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireMicrosoftAzureCosmosExtensions.AddAzureCosmosClient%2A> extension to register a <xref:Microsoft.Azure.Cosmos.CosmosClient?displayProperty=fullName> for use via the dependency injection container.

```csharp
builder.AddAzureCosmosClient("cosmosConnectionName");
```

You can then retrieve the `CosmosClient` instance using dependency injection. For example, to retrieve the client from a service:

```csharp
public class ExampleService(CosmosClient client)
{
    // Use client...
}
```

For more information on using the <xref:Microsoft.Azure.Cosmos.CosmosClient>, see the [Examples for Azure Cosmos DB for NoSQL SDK for .NET](/azure/cosmos-db/nosql/samples-dotnet).

## App host usage

To add Azure Cosmos DB hosting support to your <xref:Aspire.Hosting.IDistributedApplicationBuilder>, install the [Aspire.Hosting.Azure.CosmosDB](https://www.nuget.org/packages/Aspire.Hosting.Azure.CosmosDB) NuGet package in the [app host](xref:dotnet/aspire/app-host) project. This is helpful if you want Aspire to provision a new Azure Cosmos DB account for you, or if you want to use the Azure Cosmos DB emulator. If you want to use an Azure Cosmos DB account that is already provisioned, there's no need to add it to the app host project.

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

In your app host project, register the .NET Aspire Azure Cosmos DB integration and consume the service using the following methods:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cosmos = builder.AddAzureCosmosDB("myNewCosmosAccountName");
var cosmosdb = cosmos.AddDatabase("myCosmosDatabaseName");

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

The .NET Aspire Azure Cosmos DB library provides multiple options to configure the `CosmosClient` connection based on the requirements and conventions of your project.

### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddAzureCosmosClient`:

```csharp
builder.AddAzureCosmosClient("cosmosConnectionName");
```

And then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "cosmosConnectionName": "https://{account_name}.documents.azure.com:443/"
  }
}
```

The recommended connection approach is to use an account endpoint, which works with the <xref:Aspire.Microsoft.Azure.Cosmos.MicrosoftAzureCosmosSettings.Credential?displayProperty=nameWithType> property to establish a connection. If no credential is configured, the <xref:Azure.Identity.DefaultAzureCredential> is used:

```json
{
    "ConnectionStrings": {
      "cosmosConnectionName": "https://{account_name}.documents.azure.com:443/"
    }
}
```

Alternatively, an Azure Cosmos DB connection string can be used:

```json
{
    "ConnectionStrings": {
    "cosmosConnectionName": "AccountEndpoint=https://{account_name}.documents.azure.com:443/;AccountKey={account_key};"
    }
}
```

### Use configuration providers

The .NET Aspire Azure Cosmos DB integration supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the <xref:Aspire.Microsoft.Azure.Cosmos.MicrosoftAzureCosmosSettings> from _:::no-loc text="appsettings.json":::_ or other configuration files using `Aspire:Microsoft:Azure:Cosmos` key. Example _:::no-loc text="appsettings.json":::_ that configures some of the options:

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

### Use inline delegates

You can also pass the `Action<MicrosoftAzureCosmosSettings >` delegate to set up some or all the options inline, for example to disable tracing from code:

```csharp
builder.AddAzureCosmosClient(
    "cosmosConnectionName",
    static settings => settings.DisableTracing = true);
```

You can also set up the <xref:Microsoft.Azure.Cosmos.CosmosClientOptions?displayProperty=fullName> using the optional `Action<CosmosClientOptions> configureClientOptions` parameter of the `AddAzureCosmosClient` method. For example to set the <xref:Microsoft.Azure.Cosmos.CosmosClientOptions.ApplicationName?displayProperty=nameWithType> user-agent header suffix for all requests issues by this client:

```csharp
builder.AddAzureCosmosClient(
    "cosmosConnectionName",
    configureClientOptions:
        clientOptions => clientOptions.ApplicationName = "myapp");
```

[!INCLUDE [integration-health-checks](../includes/integration-health-checks.md)]

The .NET Aspire Azure Cosmos DB integration currently doesn't implement health checks, though this may change in future releases.

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

### Logging

The .NET Aspire Azure Cosmos DB integration uses the following log categories:

- Azure-Cosmos-Operation-Request-Diagnostics

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

### Tracing

The .NET Aspire Azure Cosmos DB integration will emit the following tracing activities using OpenTelemetry:

- Azure.Cosmos.Operation

Azure Cosmos DB tracing is currently in preview, so you must set the experimental switch to ensure traces are emitted. [Learn more about tracing in Azure Cosmos DB.](/azure/cosmos-db/nosql/sdk-observability#trace-attributes)

```csharp
AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);
```

### Metrics

The .NET Aspire Azure Cosmos DB integration currently doesn't support metrics by default due to limitations with the Azure SDK.

## See also

- [Azure Cosmos DB docs](/azure/cosmos-db/introduction)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
