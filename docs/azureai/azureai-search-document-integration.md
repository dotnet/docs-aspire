---
title: .NET Aspire Azure AI Search integration
description: Learn how to integrate Azure AI Search with .NET Aspire.
ms.date: 07/22/2025
ms.custom: sfi-ropc-nochange
---

# .NET Aspire Azure AI Search integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

The .NET Aspire Azure AI Search Documents integration enables you to connect to [Azure AI Search](/azure/search/search-what-is-azure-search) (formerly Azure Cognitive Search) services from your .NET applications. Azure AI Search is an enterprise-ready information retrieval system for your heterogeneous content that you ingest into a search index, and surface to users through queries and apps. It comes with a comprehensive set of advanced search technologies, built for high-performance applications at any scale.

## Hosting integration

The .NET Aspire Azure AI Search hosting integration models the Azure AI Search resource as the <xref:Aspire.Hosting.Azure.AzureSearchResource> type. To access this type and APIs for expressing them within your [AppHost](xref:dotnet/aspire/app-host) project, install the [📦 Aspire.Hosting.Azure.Search](https://www.nuget.org/packages/Aspire.Hosting.Azure.Search) NuGet package:

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.Search
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.Search"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add an Azure AI Search resource

To add an <xref:Aspire.Hosting.Azure.AzureSearchResource> to your AppHost project, call the <xref:Aspire.Hosting.AzureSearchExtensions.AddAzureSearch*> method providing a name:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var search = builder.AddAzureSearch("search");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(search);

// After adding all resources, run the app...
```

The preceding code adds an Azure AI Search resource named `search` to the AppHost project. The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference*> method passes the connection information to the `ExampleProject` project.

> [!IMPORTANT]
> When you call <xref:Aspire.Hosting.AzureSearchExtensions.AddAzureSearch*>, it implicitly calls <xref:Aspire.Hosting.AzureProvisionerExtensions.AddAzureProvisioning(Aspire.Hosting.IDistributedApplicationBuilder)>—which adds support for generating Azure resources dynamically during app startup. The app must configure the appropriate subscription and location. For more information, see Local provisioning: Configuration

#### Provisioning-generated Bicep

If you're new to [Bicep](/azure/azure-resource-manager/bicep/overview), it's a domain-specific language for defining Azure resources. With .NET Aspire, you don't need to write Bicep by hand; instead, the provisioning APIs generate Bicep for you. When you publish your app, the generated Bicep is output alongside the manifest file. When you add an Azure AI Search resource, Bicep is generated to provision the search service with appropriate defaults.

:::code language="bicep" source="../snippets/azure/AppHost/search/search.bicep":::

The preceding Bicep is a module that provisions an Azure AI Search service resource. Additionally, role assignments are created for the Azure resource in a separate module:

:::code language="bicep" source="../snippets/azure/AppHost/search-roles/search-roles.bicep":::

The generated Bicep is a starting point and is influenced by changes to the provisioning infrastructure in C#. Customizations to the Bicep file directly will be overwritten, so make changes through the C# provisioning APIs to ensure they are reflected in the generated files.

#### Customize provisioning infrastructure

All .NET Aspire Azure resources are subclasses of the <xref:Aspire.Hosting.Azure.AzureProvisioningResource> type. This type enables the customization of the generated Bicep by providing a fluent API to configure the Azure resources—using the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},System.Action{Aspire.Hosting.Azure.AzureResourceInfrastructure})> API. For example, you can configure the search service partitions, replicas, and more:

:::code language="csharp" source="../snippets/azure/AppHost/Program.ConfigureSearchInfra.cs" id="configure":::

The preceding code:

- Chains a call to the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure*> API:
  - The `infra` parameter is an instance of the <xref:Aspire.Hosting.Azure.AzureResourceInfrastructure> type.
  - The provisionable resources are retrieved by calling the <xref:Azure.Provisioning.Infrastructure.GetProvisionableResources> method.
  - The single <xref:Azure.Provisioning.Search.SearchService> resource is retrieved.
    - The <xref:Azure.Provisioning.Search.SearchService.PartitionCount?displayProperty=nameWithType> is set to `6`.
    - The <xref:Azure.Provisioning.Search.SearchService.ReplicaCount?displayProperty=nameWithType> is set to `3`.
    - The <xref:Azure.Provisioning.Search.SearchService.SearchSkuName?displayProperty=nameWithType> is set to <xref:Azure.Provisioning.Search.SearchServiceSkuName.Standard3?displayProperty=nameWithType>.
    - A tag is added to the Cognitive Services resource with a key of `ExampleKey` and a value of `Example value`.

There are many more configuration options available to customize the Azure AI Search resource. For more information, see [`Azure.Provisioning` customization](../azure/customize-azure-resources.md#azureprovisioning-customization).

### Connect to an existing Azure AI Search service

You might have an existing Azure AI Search service that you want to connect to. You can chain a call to annotate that your <xref:Aspire.Hosting.Azure.AzureSearchResource> is an existing resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var existingSearchName = builder.AddParameter("existingSearchName");
var existingSearchResourceGroup = builder.AddParameter("existingSearchResourceGroup");

var search = builder.AddAzureSearch("search")
                    .AsExisting(existingSearchName, existingSearchResourceGroup);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(search);

// After adding all resources, run the app...
```

[!INCLUDE [azure-configuration](../azure/includes/azure-configuration.md)]

For more information on treating Azure AI Search resources as existing resources, see [Use existing Azure resources](../azure/integrations-overview.md#use-existing-azure-resources).

> [!NOTE]
> Alternatively, instead of representing an Azure AI Search resource, you can add a connection string to the AppHost. This approach is weakly-typed, and doesn't work with role assignments or infrastructure customizations. For more information, see [Add existing Azure resources with connection strings](../azure/integrations-overview.md#add-existing-azure-resources-with-connection-strings).

### Hosting integration health checks

The Azure AI Search hosting integration doesn't currently implement any health checks. This limitation is subject to change in future releases. As always, feel free to [open an issue](https://github.com/dotnet/aspire/issues) if you have any suggestions or feedback.

## Client integration

To get started with the .NET Aspire Azure AI Search Documents client integration, install the [📦 Aspire.Azure.Search.Documents](https://www.nuget.org/packages/Aspire.Azure.Search.Documents) NuGet package in the client-consuming project, that is, the project for the application that uses the Azure AI Search Documents client.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Azure.Search.Documents
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Azure.Search.Documents"
                  Version="*" />
```

---

### Add an Azure AI Search index client

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireAzureSearchExtensions.AddAzureSearchClient*> extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register a [SearchIndexClient class](/dotnet/api/microsoft.azure.search.searchindexclient) for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddAzureSearchClient(connectionName: "search");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the Azure AI Search resource in the AppHost project. For more information, see [Add an Azure AI Search resource](#add-an-azure-ai-search-resource).

After adding the `SearchIndexClient`, you can retrieve the client instance using dependency injection. For example, to retrieve the client instance from an example service:

```csharp
public class ExampleService(SearchIndexClient indexClient)
{
    // Use indexClient
}
```

You can also retrieve a `SearchClient` which can be used for querying, by calling the <xref:Azure.Search.Documents.Indexes.SearchIndexClient.GetSearchClient(System.String)> method:

```csharp
public class ExampleService(SearchIndexClient indexClient)
{
    public async Task<long> GetDocumentCountAsync(
        string indexName,
        CancellationToken cancellationToken)
    {
        var searchClient = indexClient.GetSearchClient(indexName);

        var documentCountResponse = await searchClient.GetDocumentCountAsync(
            cancellationToken);

        return documentCountResponse.Value;
    }
}
```

For more information, see:

- Azure AI Search client library for .NET [samples using the `SearchIndexClient`](/azure/search/samples-dotnet).
- [Dependency injection in .NET](/dotnet/core/extensions/dependency-injection) for details on dependency injection.

### Add keyed Azure AI Search index client

There might be situations where you want to register multiple `SearchIndexClient` instances with different connection names. To register keyed Azure AI Search clients, call the <xref:Microsoft.Extensions.Hosting.AspireAzureSearchExtensions.AddKeyedAzureSearchClient*> method:

```csharp
builder.AddKeyedAzureSearchClient(name: "images");
builder.AddKeyedAzureSearchClient(name: "documents");
```

> [!IMPORTANT]
> When using keyed services, it's expected that your Azure AI Search resource configured two named connections, one for the `images` and one for the `documents`.

Then you can retrieve the client instances using dependency injection. For example, to retrieve the clients from a service:

```csharp
public class ExampleService(
    [KeyedService("images")] SearchIndexClient imagesClient,
    [KeyedService("documents")] SearchIndexClient documentsClient)
{
    // Use clients...
}
```

For more information, see [Keyed services in .NET](/dotnet/core/extensions/dependency-injection#keyed-services).

### Configuration

The .NET Aspire Azure AI Search Documents library provides multiple options to configure the Azure AI Search connection based on the requirements and conventions of your project. Either an `Endpoint` or a `ConnectionString` is required to be supplied.

#### Use a connection string

A connection can be constructed from the **Keys and Endpoint** tab with the format `Endpoint={endpoint};Key={key};`. You can provide the name of the connection string when calling `builder.AddAzureSearchClient()`:

```csharp
builder.AddAzureSearchClient("searchConnectionName");
```

The connection string is retrieved from the `ConnectionStrings` configuration section. Two connection formats are supported:

##### Account endpoint

The recommended approach is to use an `Endpoint`, which works with the `AzureSearchSettings.Credential` property to establish a connection. If no credential is configured, the <xref:Azure.Identity.DefaultAzureCredential> is used.

```json
{
  "ConnectionStrings": {
    "search": "https://{search_service}.search.windows.net/"
  }
}
```

##### Connection string

Alternatively, a connection string with key can be used, however; it's not the recommended approach:

```json
{
  "ConnectionStrings": {
    "search": "Endpoint=https://{search_service}.search.windows.net/;Key={account_key};"
  }
}
```

### Use configuration providers

The .NET Aspire Azure AI Search library supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the `AzureSearchSettings` and `SearchClientOptions` from configuration by using the `Aspire:Azure:Search:Documents` key. Example _:::no-loc text="appsettings.json":::_ that configures some of the options:

```json
{
  "Aspire": {
    "Azure": {
      "Search": {
        "Documents": {
          "DisableTracing": false
        }
      }
    }
  }
}
```

For the complete Azure AI Search Documents client integration JSON schema, see [Aspire.Azure.Search.Documents/ConfigurationSchema.json](https://github.com/dotnet/aspire/blob/v9.1.0/src/Components/Aspire.Azure.Search.Documents/ConfigurationSchema.json).

### Use named configuration

The .NET Aspire Azure AI Search library supports named configuration, which allows you to configure multiple instances of the same resource type with different settings. The named configuration uses the connection name as a key under the main configuration section.

```json
{
  "Aspire": {
    "Azure": {
      "Search": {
        "Documents": {
          "search1": {
            "Endpoint": "https://mysearch1.search.windows.net/",
            "DisableTracing": false
          },
          "search2": {
            "Endpoint": "https://mysearch2.search.windows.net/",
            "DisableTracing": true
          }
        }
      }
    }
  }
}
```

In this example, the `search1` and `search2` connection names can be used when calling `AddAzureSearchClient`:

```csharp
builder.AddAzureSearchClient("search1");
builder.AddAzureSearchClient("search2");
```

Named configuration takes precedence over the top-level configuration. If both are provided, the settings from the named configuration override the top-level settings.

### Use inline delegates

You can also pass the `Action<AzureSearchSettings> configureSettings` delegate to set up some or all the options inline, for example to disable tracing from code:

```csharp
builder.AddAzureSearchClient(
    "searchConnectionName",
    static settings => settings.DisableTracing = true);
```

You can also set up the <xref:Azure.Search.Documents.SearchClientOptions> using the optional `Action<IAzureClientBuilder<SearchIndexClient, SearchClientOptions>> configureClientBuilder` parameter of the `AddAzureSearchClient` method. For example, to set the client ID for this client:

```csharp
builder.AddAzureSearchClient(
    "searchConnectionName",
    configureClientBuilder: builder => builder.ConfigureOptions(
        static options => options.Diagnostics.ApplicationId = "CLIENT_ID"));
```

[!INCLUDE [client-integration-health-checks](../includes/client-integration-health-checks.md)]

The .NET Aspire Azure AI Search Documents integration implements a single health check that calls the <xref:Azure.Search.Documents.Indexes.SearchIndexClient.GetServiceStatisticsAsync%2A> method on the `SearchIndexClient` to verify that the service is available.

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

### Logging

The .NET Aspire Azure AI Search Documents integration uses the following log categories:

- `Azure`
- `Azure.Core`
- `Azure.Identity`

### Tracing

The .NET Aspire Azure AI Search Documents integration emits tracing activities using OpenTelemetry when interacting with the search service.

## See also

- [Azure AI Search](https://azure.microsoft.com/products/ai-services/ai-search)
- [.NET Aspire integrations overview](../fundamentals/integrations-overview.md)
- [.NET Aspire Azure integrations overview](../azure/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
