---
title: .NET Aspire Azure AI Search Documents component
description: Learn how to use the .NET Aspire Azure AI Search Documents component.
ms.topic: how-to
ms.date: 04/18/2024
---

# .NET Aspire Azure AI Search Documents component

In this article, you learn how to use the .NET Aspire Azure AI Search Documents client. The `Aspire.Azure.Search.Documents` library is used to register an <xref:Azure.Search.Documents.Indexes.SearchIndexClient> in the dependency injection (DI) container for connecting to Azure Search. It enables corresponding health checks and logging.

For more information on using the `SearchIndexClient`, see [How to use Azure.Search.Documents in a C# .NET Application](/azure/search/search-howto-dotnet-sdk).

## Get started

- Azure subscription: [create one for free](https://azure.microsoft.com/free/).
- Azure Search service: [create an Azure OpenAI Service resource](/azure/search/search-create-service-portal).

To get started with the .NET Aspire Azure AI Search Documents component, install the [Aspire.Azure.Search.Documents](https://www.nuget.org/packages/Aspire.Azure.Search.Documents) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Azure.Search.Documents --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Azure.Search.Documents"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _Program.cs_ file of your component-consuming project, call the extension method to register an `SearchIndexClient` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddAzureSearch("searchConnectionName");
```

You can then retrieve the `SearchIndexClient` instance using dependency injection. For example, to retrieve the client from an example service:

```csharp
public class ExampleService(SearchIndexClient indexClient)
{
    // Use indexClient
}
```

You can also retrieve a `SearchClient` which can be used for querying, by calling the <xref:Azure.Search.Documents.Indexes.SearchIndexClient.GetSearchClient%2A?displayProperty=nameWithType> method as follows:

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

For more information, see the [Azure AI Search client library for .NET](/dotnet/api/overview/azure/search.documents-readme?view=azure-dotnet&preserve-view=true) for examples on using the `SearchIndexClient`.

## App host usage

To add Azure AI hosting support to your <xref:Aspire.Hosting.IDistributedApplicationBuilder>, install the [Aspire.Hosting.Azure.CognitiveServices](https://www.nuget.org/packages/Aspire.Hosting.Azure.CognitiveServices) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.CognitiveServices --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.CognitiveServices"
                  Version="[SelectVersion]" />
```

---

In the _Program.cs_ file of `AppHost`, add an Azure Search service and consume the connection using the following methods:

```csharp
var search = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureSearch("search")
    : builder.AddConnectionString("search");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(search);
```

The `AddAzureSearch` method will read connection information from the AppHost's configuration (for example, from "user secrets") under the `ConnectionStrings:search` config key. The `WithReference` method passes that connection information into a connection string named `search` in the `MyService` project. In the _Program.cs_ file of `MyService`, the connection can be consumed using:

```csharp
builder.AddAzureSearch("search");
```

## Configuration

The .NET Aspire Azure Azure Search library provides multiple options to configure the Azure Search Service based on the requirements and conventions of your project. Note that either an `Endpoint` or a `ConnectionString` is required to be supplied.

### Use a connection string

A connection can be constructed from the **Keys and Endpoint** tab with the format `Endpoint={endpoint};Key={key};`. You can provide the name of the connection string when calling `builder.AddAzureSearch()`:

```csharp
builder.AddAzureSearch("searchConnectionName");
```

And then the connection string will be retrieved from the `ConnectionStrings` configuration section. Two connection formats are supported:

#### Account endpoint

The recommended approach is to use an `Endpoint`, which works with the `AzureSearchSettings.Credential` property to establish a connection. If no credential is configured, the <xref:Azure.Identity.DefaultAzureCredential> is used.

```json
{
  "ConnectionStrings": {
    "searchConnectionName": "https://{search_service}.search.windows.net/"
  }
}
```

#### Connection string

Alternatively, a custom connection string can be used.

```json
{
  "ConnectionStrings": {
    "searchConnectionName": "Endpoint=https://{search_service}.search.windows.net/;Key={account_key};"
  }
}
```

### Use configuration providers

The .NET Aspire Azure AI Search library supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the `AzureSearchSettings` and `SearchClientOptions` from configuration by using the `Aspire:Azure:Search:Documents` key. Example `appsettings.json` that configures some of the options:

```json
{
  "Aspire": {
    "Azure": {
      "Search": {
        "Documents": {
          "Tracing": true,
        }
      }
    }
  }
}
```

### Use inline delegates

You can also pass the `Action<AzureSearchSettings> configureSettings` delegate to set up some or all the options inline, for example to disable tracing from code:

```csharp
builder.AddAzureSearch(
    "searchConnectionName",
    static settings => settings.Tracing = false);
```

You can also setup the <xref:Azure.Search.Documents.SearchClientOptions> using the optional `Action<IAzureClientBuilder<SearchIndexClient, SearchClientOptions>> configureClientBuilder` parameter of the `AddAzureSearch` method. For example, to set the client ID for this client:

```csharp
builder.AddAzureSearch(
    "searchConnectionName",
    configureClientBuilder: builder => builder.ConfigureOptions(
        static options => options.Diagnostics.ApplicationId = "CLIENT_ID"));
```

[!INCLUDE [component-health-checks](../includes/component-health-checks.md)]

The .NET Aspire Azure AI Search Documents component implements a single health check, that calls the <xref:Azure.Search.Documents.Indexes.SearchIndexClient.GetServiceStatisticsAsync%2A> method on the `SearchIndexClient` to verify that the service is available.

[!INCLUDE [component-observability-and-telemetry](../includes/component-observability-and-telemetry.md)]

### Logging

The .NET Aspire Azure AI Search Documents component uses the following log categories:

- `Azure`
- `Azure.Core`
- `Azure.Identity`

## See also

- [Azure AI OpenAI docs](/azure/ai-services/openai/overview)
- [.NET Aspire components](../fundamentals/components-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
