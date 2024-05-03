---
title: .NET Aspire Azure Cosmos DB component
description: This article describes the .NET Aspire Azure Cosmos DB component features and capabilities.
ms.topic: how-to
ms.date: 04/24/2024
---

# .NET Aspire Azure Cosmos DB component

In this article, you learn how to use the .NET Aspire Azure Cosmos DB component. The `Aspire.Microsoft.Azure.Cosmos` library is used to register a <xref:Microsoft.Azure.Cosmos.CosmosClient> as a singleton in the DI container for connecting to Azure Cosmos DB. It also enables corresponding health checks, logging and telemetry.

## Get started

To get started with the .NET Aspire Azure Cosmos DB component, install the [Aspire.Microsoft.Azure.Cosmos](https://www.nuget.org/packages/Aspire.Microsoft.Azure.Cosmos) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Microsoft.Azure.Cosmos --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Microsoft.Azure.Cosmos"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _Program.cs_ file of your component-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireAzureCosmosDBExtensions.AddAzureCosmosDBClient%2A> extension to register a <xref:Microsoft.Azure.Cosmos.CosmosClient?displayProperty=fullName> for use via the dependency injection container.

```csharp
builder.AddAzureCosmosDBClient("cosmosdb");
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

To add Azure Cosmos DB hosting support to your <xref:Aspire.Hosting.IDistributedApplicationBuilder>, install the [Aspire.Hosting.Azure.CosmosDB](https://www.nuget.org/packages/Aspire.Hosting.Azure.CosmosDB) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.CosmosDB --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.CosmosDB"
                  Version="[SelectVersion]" />
```

---

In your app host project, register the .NET Aspire Azure Cosmos DB component and consume the service using the following methods:

```csharp
// Service registration
var cosmos = builder.AddAzureCosmosDB("cosmos");
var cosmosdb = cosmos.AddDatabase("cosmosdb");

// Service consumption
var exampleProject = builder.AddProject<Projects.ExampleProject>()
                            .WithReference(cosmosdb);
```

## Configuration

The .NET Aspire Azure Cosmos DB library provides multiple options to configure the Azure Cosmos DB connection based on the requirements and conventions of your project.

### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddAzureCosmosDB`:

```csharp
builder.AddAzureCosmosDB("cosmosConnectionName");
```

And then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "cosmosConnectionName": "https://{account_name}.documents.azure.com:443/"
  }
}
```

The recommended connection approach is to use an account endpoint, which works with the <xref:Aspire.Microsoft.Azure.Cosmos.AzureCosmosDBSettings.Credential?displayProperty=nameWithType> property to establish a connection. If no credential is configured, the <xref:Azure.Identity.DefaultAzureCredential> is used:

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

The .NET Aspire Azure Cosmos DB component supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the <xref:Aspire.Microsoft.Azure.Cosmos.AzureCosmosDBSettings> from _appsettings.json_ or other configuration files using `Aspire:Microsoft:Azure:Cosmos` key. Example `appsettings.json` that configures some of the options:

```json
{
  "Aspire": {
    "Microsoft": {
      "Azure": {
        "Cosmos": {
          "Tracing": true,
        }
      }
    }
  }
}
```

### Use inline delegates

You can also pass the `Action<AzureCosmosDBSettings>` delegate to set up some or all the options inline, for example to disable tracing from code:

```csharp
builder.AddAzureCosmosDB(
    "cosmosConnectionName",
    static settings => settings.Tracing = false);
```

You can also set up the <xref:Microsoft.Azure.Cosmos.CosmosClientOptions?displayProperty=fullName> using the optional `Action<CosmosClientOptions> configureClientOptions` parameter of the `AddAzureCosmosDB` method. For example to set the <xref:Microsoft.Azure.Cosmos.CosmosClientOptions.ApplicationName?displayProperty=nameWithType> user-agent header suffix for all requests issues by this client:

```csharp
builder.AddAzureCosmosDB(
    "cosmosConnectionName",
    configureClientOptions:
        clientOptions => clientOptions.ApplicationName = "myapp");
```

[!INCLUDE [component-health-checks](../includes/component-health-checks.md)]

The .NET Aspire Azure Cosmos DB component currently doesn't implement health checks, though this may change in future releases.

[!INCLUDE [component-observability-and-telemetry](../includes/component-observability-and-telemetry.md)]

### Logging

The .NET Aspire Azure Cosmos DB component uses the following log categories:

- Azure-Cosmos-Operation-Request-Diagnostics

### Tracing

The .NET Aspire Azure Cosmos DB component will emit the following tracing activities using OpenTelemetry:

- Azure.Cosmos.Operation

### Metrics

The .NET Aspire Azure Cosmos DB component currently doesn't support metrics by default due to limitations with the Azure SDK.

## See also

- [Azure Cosmos DB docs](/azure/cosmos-db/introduction)
- [.NET Aspire components](../fundamentals/components-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
