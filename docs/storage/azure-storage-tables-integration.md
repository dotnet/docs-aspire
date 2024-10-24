---
title: .NET Aspire Azure Data Tables integration
description: This article describes the .NET Aspire Azure Data Tables integration features and capabilities.
ms.date: 08/12/2024
ms.topic: how-to
---

# .NET Aspire Azure Data Tables integration

In this article, you learn how to use the .NET Aspire Azure Data Tables integration. The `Aspire.Azure.Data.Tables` library is used to:

- Registers a <xref:Azure.Data.Tables.TableServiceClient> as a singleton in the DI container for connecting to Azure Table storage.
- Enables corresponding health checks, logging and telemetry.

## Prerequisites

- Azure subscription - [create one for free](https://azure.microsoft.com/free/)
- An Azure storage account or Azure Cosmos DB database with Azure Table API specified. - [create a storage account](/azure/storage/common/storage-account-create)

## Get started

To get started with the .NET Aspire Azure Data Tables integration, install the [Aspire.Azure.Data.Tables](https://www.nuget.org/packages/Aspire.Azure.Data.Tables) NuGet package in the client-consuming project, i.e., the project for the application that uses the Azure Data Tables client.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Azure.Data.Tables
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Azure.Data.Tables"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _:::no-loc text="Program.cs":::_ file of your integration-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireTablesExtensions.AddAzureTableClient%2A> extension to register a `TableServiceClient` for use via the dependency injection container.

```csharp
builder.AddAzureTableClient("tables");
```

To retrieve the `TableServiceClient` instance using dependency injection, define it as a constructor parameter. Consider the following example service:

```csharp
public class ExampleService(TableServiceClient client)
{
    // Use client...
}
```

## App host usage

To add Azure Storage hosting support to your <xref:Aspire.Hosting.IDistributedApplicationBuilder>, install the [Aspire.Hosting.Azure.Storage](https://www.nuget.org/packages/Aspire.Hosting.Azure.Storage) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.Storage
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.Storage"
                  Version="*" />
```

---

In your app host project, register the Azure Table Storage integration and consume the service using the following methods:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var tables = builder.AddAzureStorage("storage")
                    .AddTables("tables");

Builder.AddProject<MyApp.ExampleProject>() 
       .WithReference(tables)
```

For more information, see <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A>.

## Configuration

The .NET Aspire Azure Table Storage integration provides multiple options to configure the `TableServiceClient` based on the requirements and conventions of your project.

### Use configuration providers

The .NET Aspire Azure Table Storage integration supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the <xref:Aspire.Azure.Data.Tables.AzureDataTablesSettings> from _:::no-loc text="appsettings.json":::_ or other configuration files using `Aspire:Azure:Data:Tables` key.

```json
{
  "Aspire":{
    "Azure": {
      "Data": {
        "Tables": {
          "ServiceUri": "YOUR_URI",
          "DisableHealthChecks": true,
          "DisableTracing": false,
          "ClientOptions": {
          "EnableTenantDiscovery": true
          }
        }
      }
    }
  }
}
```

If you have set up your configurations in the `Aspire:Azure:Data:Tables` section of your _:::no-loc text="appsettings.json":::_ file you can just call the method `AddAzureTableClient` without passing any parameters.

### Use inline delegates

You can also pass the `Action<AzureDataTablesSettings>` delegate to set up some or all the options inline, for example to set the `ServiceUri`:

```csharp
builder.AddAzureTableClient(
    "tables",
    static settings => settings.ServiceUri = new Uri("YOUR_SERVICEURI"));
```

You can also set up the <xref:Azure.Data.Tables.TableClientOptions> using `Action<IAzureClientBuilder<TableServiceClient, TableClientOptions>>` delegate, the second parameter of the <xref:Microsoft.Extensions.Hosting.AspireTablesExtensions.AddAzureTableClient%2A> method. For example to set the `TableServiceClient` ID to identify the client:

```csharp
builder.AddAzureTableClient(
    "tables",
    static clientBuilder =>
        clientBuilder.ConfigureOptions(
            static options => options.EnableTenantDiscovery = true));
```

### Configuration options

The following configurable options are exposed through the <xref:Aspire.Azure.Data.Tables.AzureDataTablesSettings> class:

| Name                  | Description                                                                               |
|-----------------------|-------------------------------------------------------------------------------------------|
| `ServiceUri`          | A "Uri" referencing the Table service.                                                    |
| `Credential`          | The credential used to authenticate to the Table Storage.                                 |
| `DisableHealthChecks` | A boolean value that indicates whether the Table Storage health check is disabled or not. |
| `DisableTracing`      | A boolean value that indicates whether the OpenTelemetry tracing is disabled or not.      |

[!INCLUDE [integration-health-checks](../includes/integration-health-checks.md)]

By default, The .NET Aspire Azure Data Tables integration handles the following:

- Adds the `AzureTableStorageHealthCheck` health check, which attempts to connect to and query table storage
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

### Logging

The .NET Aspire Azure Data Tables integration uses the following log categories:

- `Azure.Core`
- `Azure.Identity`

### Tracing

The .NET Aspire Azure Data Tables integration will emit the following tracing activities using OpenTelemetry:

- "Azure.Data.Tables.TableServiceClient"

### Metrics

The .NET Aspire Azure Data Tables integration currently does not support metrics by default due to limitations with the Azure SDK.

## See also

- [Azure Table Storage docs](/azure/storage/tables/)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
