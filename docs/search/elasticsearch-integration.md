---
title: .NET Aspire Elasticsearch integration
description: This article describes the .NET Aspire Elasticsearch integration features and capabilities.
ms.topic: how-to
ms.date: 08/12/2024
---

# .NET Aspire Elasticsearch integration

In this article, you learn how to use the .NET Aspire Elasticsearch integration. The `Aspire.Elastic.Clients.Elasticsearch` library registers a [ElasticsearchClient](https://github.com/elastic/elasticsearch-net) in the DI container for connecting to a Elasticsearch. It enables corresponding health check and telemetry.

## Prerequisites

- Elasticsearch cluster.
- Endpoint URI string for accessing the Elasticsearch API endpoint or a CloudId and an ApiKey from [Elastic Cloud](https://www.elastic.co/cloud)

## Get started

To get started with the .NET Aspire Elasticsearch integration, install the [Aspire.Elastic.Clients.Elasticsearch](https://www.nuget.org/packages/Aspire.Elastic.Clients.Elasticsearch) NuGet package in the client-consuming project, i.e., the project for the application that uses the Elasticsearch client.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Elastic.Clients.Elasticsearch
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Elastic.Clients.Elasticsearch"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _Program.cs_ file of your project, call the `AddElasticsearchClient` extension method to register a `ElasticsearchClient` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddElasticsearchClient("elasticsearch");
```

## App host usage

To model the Elasticsearch resource in the app host, install the [Aspire.Hosting.Elasticsearch](https://www.nuget.org/packages/Aspire.Hosting.Elasticsearch) NuGet package in the [app host](xref:aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Elasticsearch
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Elasticsearch"
                  Version="[SelectVersion]" />
```

---

In the _Program.cs_ file of `AppHost`, register a Elasticsearch cluster and consume the connection using the following methods:

```csharp
var elasticsearch = builder.AddElasticsearch("elasticsearch");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(elasticsearch);
```

The `WithReference` method configures a connection in the `MyService` project named `elasticsearch`. In the _Program.cs_ file of `MyService`, the Elasticsearch connection can be consumed using:

```csharp
builder.AddElasticsearchClient("elasticsearch");
```

## Configuration

The .NET Aspire Elasticsearch client integration provides multiple options to configure the server connection based on the requirements and conventions of your project.

### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddElasticsearchClient()`:

```csharp
builder.AddElasticsearchClient("elasticsearch");
```

And then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "elasticsearch": "http://elastic:password@localhost:27011"
  }
}
```

### Use configuration providers

The .NET Aspire Elasticsearch Client integration supports [Microsoft.Extensions.Configuration](/dotnet/api/microsoft.extensions.configuration). It loads the `ElasticClientsElasticsearchSettings` from configuration by using the `Aspire:Elastic:Clients:Elasticsearch` key. Consider the following example _appsettings.json_ that configures some of the options:

```json
{
  "Aspire": {
    "Elastic": {
      "Clients": {
        "Elasticsearch": {
            "Endpoint": "http://elastic:password@localhost:27011"
        }
      }
    }
  }
}
```

### Use inline delegates

Also you can pass the `Action<ElasticClientsElasticsearchSettings> configureSettings` delegate to set up some or all the options inline, for example to set the API key from code:

```csharp
builder.AddElasticsearchClient(
    "elasticsearch",
    settings =>
        settings.Endpoint = new Uri("http://elastic:password@localhost:27011"));
```

### Use a `CloudId` and an `ApiKey` with configuration providers

When using [Elastic Cloud](https://www.elastic.co/cloud), you can provide the `CloudId` and `ApiKey` in `Aspire:Elastic:Clients:Elasticsearch` section when calling `builder.AddElasticsearchClient()`.

```csharp
builder.AddElasticsearchClient("elasticsearch");
```

Consider the following example _appsettings.json_ that configures the options:

```json
{
  "Aspire": {
    "Elastic": {
      "Clients": {
        "Elasticsearch": {
            "ApiKey": "<Valid ApiKey>",
            "CloudId": "<Valid CloudId>"
        }
      }
    }
  }
}
```

### Use a `CloudId` and an `ApiKey` with inline delegates

```csharp
builder.AddElasticsearchClient(
    "elasticsearch",
    settings =>
    {
        settings.ApiKey = "<Valid ApiKey>";
        settings.CloudId = "<Valid CloudId>";
    });
```

[!INCLUDE [integration-health-checks](../includes/integration-health-checks.md)]

The .NET Aspire Elasticsearch integration uses the configured client to perform a `PingAsync`. If the result is an HTTP 200 OK, the health check is considered healthy, otherwise it's unhealthy. Likewise, if there's an exception, the health check is considered unhealthy with the error propagating through the health check failure.

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

### Tracing

The .NET Aspire Elasticsearch integration will emit the following tracing activities using OpenTelemetry:

- `Elastic.Transport`

## See also

- [Elasticsearch .NET](https://github.com/elastic/elasticsearch-net)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)

<!--
https://github.com/dotnet/docs-aspire/issues/1059

In .NET Aspire 8.1 we are adding support to Elasticsearch hosting as a first-class API. See:

Add Elasticsearch Component aspire#4418
Add Elasticsearch Hosting aspire#4430

The article should cover:

AddElasticsearch
WithDataVolume/WithDataBindMount
for elastic hosting

also, its should cover client integration when its ready

-->