---
title: .NET Aspire Elasticsearch integration
description: Learn how to use the .NET Aspire Elasticsearch integration, which includes both hosting and client integrations.
ms.date: 10/11/2024
uid: search/elasticsearch-integration
---

# .NET Aspire Elasticsearch integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[Elasticsearch](https://www.elastic.co/elasticsearch) is a distributed, RESTful search and analytics engine, scalable data store, and vector database capable of addressing a growing number of use cases. The .NET Aspire Elasticsearch integration enables you to connect to existing Elasticsearch instances, or create new instances from .NET with the [`docker.io/library/elasticsearch` container image](https://hub.docker.com/_/elasticsearch).

## Hosting integration

The Elasticsearch hosting integration models an Elasticsearch instance as the <xref:Aspire.Hosting.ApplicationModel.ElasticsearchResource> type. To access this type and APIs that allow you to add it to your [app model](xref:dotnet/aspire/app-host#define-the-app-model), install the [📦 Aspire.Hosting.Elasticsearch](https://www.nuget.org/packages/Aspire.Hosting.Elasticsearch) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Elasticsearch
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Elasticsearch"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add Elasticsearch resource

In your app host project, call <xref:Aspire.Hosting.ElasticsearchBuilderExtensions.AddElasticsearch*> on the `builder` instance to add an Elasticsearch resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var elasticsearch = builder.AddElasticsearch("elasticsearch");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(elasticsearch);

// After adding all resources, run the app...
```

When .NET Aspire adds a container image to the app host, as shown in the preceding example with the `docker.io/library/elasticsearch` image, it creates a new Elasticsearch instance on your local machine. A reference to your Elasticsearch resource (the `elasticsearch` variable) is added to the `ExampleProject`. The Elasticsearch resource includes default credentials with a `username` of `"elastic"` and randomly generated `password` using the <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.CreateDefaultPasswordParameter*> method when a password wasn't provided.

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` named `"elasticsearch"`. For more information, see [Container resource lifecycle](../fundamentals/app-host-overview.md#container-resource-lifecycle).

> [!TIP]
> If you'd rather connect to an existing Elasticsearch instance, call <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> instead. For more information, see [Reference existing resources](../fundamentals/app-host-overview.md#reference-existing-resources).

### Add Elasticsearch resource with data volume

To add a data volume to the Elasticsearch resource, call the <xref:Aspire.Hosting.ElasticsearchBuilderExtensions.WithDataVolume*> method on the Elasticsearch resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var elasticsearch = builder.AddElasticsearch("elasticsearch")
                           .WithDataVolume(isReadOnly: false);

builder.AddProject<Projects.ExampleProject>()
        .WithReference(elasticsearch);

// After adding all resources, run the app...
```

The data volume is used to persist the Elasticsearch data outside the lifecycle of its container. The data volume is mounted at the `/usr/share/elasticsearch/data` path in the Elasticsearch container and when a `name` parameter isn't provided, the name is generated at random. For more information on data volumes and details on why they're preferred over [bind mounts](#add-elasticsearch-resource-with-data-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

### Add Elasticsearch resource with data bind mount

To add a data bind mount to the Elasticsearch resource, call the <xref:Aspire.Hosting.ElasticsearchBuilderExtensions.WithDataBindMount*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var elasticsearch = builder.AddElasticsearch("elasticsearch")
                           .WithDataBindMount(
                               source: @"C:\Elasticsearch\Data",
                               isReadOnly: false);

builder.AddProject<Projects.ExampleProject>()
        .WithReference(elasticsearch);

// After adding all resources, run the app...
```

[!INCLUDE [data-bind-mount-vs-volumes](../includes/data-bind-mount-vs-volumes.md)]

Data bind mounts rely on the host machine's filesystem to persist the Elasticsearch data across container restarts. The data bind mount is mounted at the `C:\Elasticsearch\Data` on Windows (or `/Elasticsearch/Data` on Unix) path on the host machine in the Elasticsearch container. For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

### Add Elasticsearch resource with password parameter

When you want to explicitly provide the password used by the container image, you can provide these credentials as parameters. Consider the following alternative example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("password", secret: true);
var elasticsearch = builder.AddElasticsearch("elasticsearch", password);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(elasticsearch);

// After adding all resources, run the app...
```

For more information on providing parameters, see [External parameters](../fundamentals/external-parameters.md).

### Hosting integration health checks

The Elasticsearch hosting integration automatically adds a health check for the Elasticsearch resource. The health check verifies that the Elasticsearch instance is running and that a connection can be established to it.

The hosting integration relies on the [📦 AspNetCore.HealthChecks.Elasticsearch](https://www.nuget.org/packages/AspNetCore.HealthChecks.Elasticsearch) NuGet package.

## Client integration

To get started with the .NET Aspire Elasticsearch client integration, install the [📦 Aspire.Elastic.Clients.Elasticsearch](https://www.nuget.org/packages/Aspire.Elastic.Clients.Elasticsearch) NuGet package in the client-consuming project, that is, the project for the application that uses the Elasticsearch client. The Elasticsearch client integration registers an [ElasticsearchClient](https://github.com/elastic/elasticsearch-net) instance that you can use to interact with Elasticsearch.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Elastic.Clients.Elasticsearch
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Elastic.Clients.Elasticsearch"
                  Version="*" />
```

---

### Add Elasticsearch client

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireElasticClientsElasticsearchExtensions.AddElasticsearchClient*> extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register an `ElasticsearchClient` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddElasticsearchClient(connectionName: "elasticsearch");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the Elasticsearch resource in the app host project. For more information, see [Add Elasticsearch resource](#add-elasticsearch-resource).

You can then retrieve the `ElasticsearchClient` instance using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(ElasticsearchClient client)
{
    // Use client...
}
```

### Add keyed Elasticsearch client

There might be situations where you want to register multiple `ElasticsearchClient` instances with different connection names. To register keyed Elasticsearch clients, call the <xref:Microsoft.Extensions.Hosting.AspireElasticClientsElasticsearchExtensions.AddKeyedElasticsearchClient*>:

```csharp
builder.AddKeyedElasticsearchClient(name: "products");
builder.AddKeyedElasticsearchClient(name: "orders");
```

Then you can retrieve the `ElasticsearchClient` instances using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(
    [FromKeyedServices("products")] ElasticsearchClient productsClient,
    [FromKeyedServices("orders")] ElasticsearchClient ordersClient)
{
    // Use clients...
}
```

For more information on keyed services, see [.NET dependency injection: Keyed services](/dotnet/core/extensions/dependency-injection#keyed-services).

### Configuration

The .NET Aspire Elasticsearch client integration provides multiple options to configure the server connection based on the requirements and conventions of your project.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddElasticsearchClient`:

```csharp
builder.AddElasticsearchClient("elasticsearch");
```

Then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "elasticsearch": "http://elastic:password@localhost:27011"
  }
}
```

#### Use configuration providers

The .NET Aspire Elasticsearch Client integration supports <xref:Microsoft.Extensions.Configuration>. It loads the <xref:Aspire.Elastic.Clients.Elasticsearch.ElasticClientsElasticsearchSettings> from configuration by using the `Aspire:Elastic:Clients:Elasticsearch` key. Consider the following example _appsettings.json_ that configures some of the options:

```json
{
  "Aspire": {
    "Elastic": {
      "Clients": {
        "Elasticsearch": {
            "DisableHealthChecks": false,
            "DisableTracing": false,
            "HealthCheckTimeout": "00:00:03",  
            "ApiKey": "<Valid ApiKey>",
            "Endpoint": "http://elastic:password@localhost:27011",
            "CloudId": "<Valid CloudId>"
        }
      }
    }
  }
}
```

For the complete Elasticsearch client integration JSON schema, see [Aspire.Elastic.Clients.Elasticsearch/ConfigurationSchema.json](https://github.com/dotnet/aspire/blob/v8.2.1/src/Components/Aspire.Elastic.Clients.Elasticsearch/ConfigurationSchema.json).

#### Use inline delegates

Also you can pass the `Action<ElasticClientsElasticsearchSettings> configureSettings` delegate to set up some or all the options inline, for example to set the API key from code:

```csharp
builder.AddElasticsearchClient(
    "elasticsearch",
    static settings =>
        settings.Endpoint = new Uri("http://elastic:password@localhost:27011"));
```

#### Use a `CloudId` and an `ApiKey` with configuration providers

When using [Elastic Cloud](https://www.elastic.co/cloud), you can provide the `CloudId` and `ApiKey` in `Aspire:Elastic:Clients:Elasticsearch` section when calling `builder.AddElasticsearchClient`.

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

#### Use a `CloudId` and an `ApiKey` with inline delegates

```csharp
builder.AddElasticsearchClient(
    "elasticsearch",
    static settings =>
    {
        settings.ApiKey = "<Valid ApiKey>";
        settings.CloudId = "<Valid CloudId>";
    });
```

#### Client integration health checks

By default, .NET Aspire integrations enable [health checks](../fundamentals/health-checks.md) for all services. For more information, see [.NET Aspire integrations overview](../fundamentals/integrations-overview.md).

The .NET Aspire Elasticsearch integration uses the configured client to perform a `PingAsync`. If the result is an HTTP 200 OK, the health check is considered healthy, otherwise it's unhealthy. Likewise, if there's an exception, the health check is considered unhealthy with the error propagating through the health check failure.

### Observability and telemetry

.NET Aspire integrations automatically set up Logging, Tracing, and Metrics configurations, which are sometimes known as *the pillars of observability*. For more information about integration observability and telemetry, see [.NET Aspire integrations overview](../fundamentals/integrations-overview.md). Depending on the backing service, some integrations may only support some of these features. For example, some integrations support logging and tracing, but not metrics. Telemetry features can also be disabled using the techniques presented in the [Configuration](#configuration) section.

#### Tracing

The .NET Aspire Elasticsearch integration will emit the following tracing activities using OpenTelemetry:

- `Elastic.Transport`

## See also

- [Elasticsearch .NET](https://github.com/elastic/elasticsearch-net)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
