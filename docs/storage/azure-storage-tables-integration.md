---
title: Aspire Azure Data Tables integration
description: Learn how to use the Aspire Azure Data Tables integration, which includes both hosting and client integrations.
ms.date: 04/08/2025
uid: storage/azure-data-tables-integration
---

# Aspire Azure Data Tables integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[Azure Table Storage](https://azure.microsoft.com/services/storage/tables/) is a service for storing structured NoSQL data. The Aspire Azure Data Tables integration enables you to connect to existing Azure Table Storage instances or create new instances from .NET applications.

## Hosting integration

[!INCLUDE [storage-app-host](includes/storage-app-host.md)]

### Add Azure Table Storage resource

In your AppHost project, register the Azure Table Storage integration by chaining a call to <xref:Aspire.Hosting.AzureStorageExtensions.AddTables*> on the `IResourceBuilder<IAzureStorageResource>` instance returned by <xref:Aspire.Hosting.AzureStorageExtensions.AddAzureStorage*>. The following example demonstrates how to add an Azure Table Storage resource named `storage` and a table resource named `tables`:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var tables = builder.AddAzureStorage("storage")
                    .AddTables("tables");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(tables)
       .WaitFor(tables);

// After adding all resources, run the app...
```

The preceding code:

- Adds an Azure Storage resource named `storage`.
- Adds a table storage resource named `tables` to the storage resource.
- Adds the `storage` resource to the `ExampleProject` and waits for it to be ready before starting the project.

[!INCLUDE [storage-bicep](includes/storage-bicep.md)]

[!INCLUDE [storage-emulator](includes/storage-emulator.md)]

[!INCLUDE [storage-connections](includes/storage-connections.md)]

[!INCLUDE [storage-hosting-health-checks](includes/storage-hosting-health-checks.md)]

## Client integration

To get started with the Aspire Azure Data Tables client integration, install the [📦 Aspire.Azure.Data.Tables](https://www.nuget.org/packages/Aspire.Azure.Data.Tables) NuGet package in the client-consuming project, that is, the project for the application that uses the Azure Data Tables client. The Azure Data Tables client integration registers a <xref:Azure.Data.Tables.TableServiceClient> instance that you can use to interact with Azure Table Storage.

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

### Add Azure Table Storage client

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireTablesExtensions.AddAzureTableClient%2A> extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register a `TableServiceClient` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddAzureTableClient("tables");
```

You can then retrieve the `TableServiceClient` instance using dependency injection. For example, to retrieve the client from a service:

```csharp
public class ExampleService(TableServiceClient client)
{
    // Use client...
}
```

### Configuration

The Aspire Azure Table Storage integration provides multiple options to configure the `TableServiceClient` based on the requirements and conventions of your project.

#### Use configuration providers

The Aspire Azure Table Storage integration supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the <xref:Aspire.Azure.Data.Tables.AzureDataTablesSettings> and <xref:Azure.Data.Tables.TableClientOptions> from configuration by using the `Aspire:Azure:Data:Tables` key. The following snippet is an example of a _:::no-loc text="appsettings.json":::_ file that configures some of the options:

```json
{
  "Aspire": {
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

For the complete Azure Data Tables client integration JSON schema, see [Aspire.Azure.Data.Tables/ConfigurationSchema.json](https://github.com/dotnet/aspire/blob/v9.1.0/src/Components/Aspire.Azure.Data.Tables/ConfigurationSchema.json).

#### Use named configuration

The Aspire Azure Table Storage integration supports named configuration, which allows you to configure multiple instances of the same resource type with different settings. The named configuration uses the connection name as a key under the main configuration section.

```json
{
  "Aspire": {
    "Azure": {
      "Data": {
        "Tables": {
          "tables1": {
            "ServiceUri": "https://myaccount1.table.core.windows.net/",
            "DisableHealthChecks": true,
            "ClientOptions": {
              "EnableTenantDiscovery": true
            }
          },
          "tables2": {
            "ServiceUri": "https://myaccount2.table.core.windows.net/",
            "DisableTracing": true,
            "ClientOptions": {
              "EnableTenantDiscovery": false
            }
          }
        }
      }
    }
  }
}
```

In this example, the `tables1` and `tables2` connection names can be used when calling `AddAzureTableClient`:

```csharp
builder.AddAzureTableClient("tables1");
builder.AddAzureTableClient("tables2");
```

Named configuration takes precedence over the top-level configuration. If both are provided, the settings from the named configuration override the top-level settings.

#### Use inline delegates

You can also pass the `Action<AzureDataTablesSettings> configureSettings` delegate to set up some or all the options inline, for example to configure the `ServiceUri`:

```csharp
builder.AddAzureTableClient(
    "tables",
    settings => settings.DisableHealthChecks = true);
```

You can also set up the <xref:Azure.Data.Tables.TableClientOptions> using `Action<IAzureClientBuilder<TableServiceClient, TableClientOptions>> configureClientBuilder` delegate, the second parameter of the `AddAzureTableClient` method. For example, to set the `TableServiceClient` ID to identify the client:

```csharp
builder.AddAzureTableClient(
    "tables",
    configureClientBuilder: clientBuilder =>
        clientBuilder.ConfigureOptions(
            options => options.EnableTenantDiscovery = true));
```

### Client integration health checks

By default, Aspire integrations enable [health checks](../fundamentals/health-checks.md) for all services. For more information, see [Aspire integrations overview](../fundamentals/integrations-overview.md).

The Aspire Azure Data Tables integration:

- Adds the health check when <xref:Aspire.Azure.Data.Tables.AzureDataTablesSettings.DisableHealthChecks?displayProperty=nameWithType> is `false`, which attempts to connect to the Azure Table Storage.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic.

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

### Logging

The Aspire Azure Data Tables integration uses the following log categories:

- `Azure.Core`
- `Azure.Identity`

### Tracing

The Aspire Azure Data Tables integration emits the following tracing activities using OpenTelemetry:

- `Azure.Data.Tables.TableServiceClient`

### Metrics

The Aspire Azure Data Tables integration currently doesn't support metrics by default due to limitations with the Azure SDK.

## See also

- [Azure Table Storage docs](/azure/storage/tables/)
- [Aspire integrations](../fundamentals/integrations-overview.md)
- [Aspire GitHub repo](https://github.com/dotnet/aspire)
