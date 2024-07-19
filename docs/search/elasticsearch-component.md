---
title: .NET Aspire Elasticsearch component
description: This article describes the .NET Aspire Elasticsearch component features and capabilities.
ms.topic: how-to
ms.date: 07/19/2024
---

# .NET Aspire Elasticsearch component

In this article, you learn how to use the .NET Aspire Elasticsearch component. The `Aspire.Elastic.Clients.Elasticsearch` library registers a [ElasticsearchClient](https://github.com/elastic/elasticsearch-net) in the DI container for connecting to a Elasticsearch. It enables corresponding health check, logging and telemetry.

## Get started

To get started with the .NET Aspire Elasticsearch component, install the [Aspire.Elastic.Clients.Elasticsearch](https://www.nuget.org/packages/Aspire.Elastic.Clients.Elasticsearch) NuGet package in the consuming client project.

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

To model the RabbitMQ resource in the app host, install the [Aspire.Hosting.Elasticsearch](https://www.nuget.org/packages/Aspire.Hosting.Elasticsearch) NuGet package in the [app host](xref:aspire/app-host) project.

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

The .NET Aspire Elasticsearch client component provides multiple options to configure the server connection based on the requirements and conventions of your project.

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

The .NET Aspire Elasticsearch Client component supports [Microsoft.Extensions.Configuration](/dotnet/api/microsoft.extensions.configuration). It loads the  `ElasticClientsElasticsearchSettings` from configuration by using the `Aspire:Elastic:Clients:Elasticsearch` key. Example _appsettings.json_ that configures some of the options:

```json
{
  "Aspire": {
    "Elastic": {
      "Clients": {
        "Elasticsearch": {
            "ConnectionString": "http://elastic:password@localhost:27011"
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
        settings.ConnectionString = "http://elastic:password@localhost:27011");
```

### Use a `CloudId` and an `ApiKey` with configuration providers

When using [Elastic Cloud](https://www.elastic.co/cloud), you can provide the `CloudId` and `ApiKey` in `Aspire:Elastic:Clients:Elasticsearch:Cloud` section and set `Aspire:Elastic:Clients:Elasticsearch:UseCloud` key to `true` when calling `builder.AddElasticsearchClient()`.

```csharp
builder.AddElasticsearchClient("elasticsearch");
```

Consider the following example _appsettings.json_ that configures the options:

```json
{
  "Aspire": {
    "Elastic": {
      "Clients": {
        "UseCloud" : true,
        "Cloud": {
            "ApiKey": "Valid ApiKey",
            "CloudId": "Valid CloudId"
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
        settings.UseCloud = true;
        settings.Cloud.CloudId = "<Valid CloudId>";
        settings.Cloud.ApiKey = "<Valid ApiKey>";
    });
```

## See also

- [Elasticsearch .NET](https://github.com/elastic/elasticsearch-net)
- [.NET Aspire components](../fundamentals/components-overview.md)
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

also, its should cover client component when its ready

-->