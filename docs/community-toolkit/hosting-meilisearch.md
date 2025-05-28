---
title: .NET Aspire Community Toolkit Meilisearch integration
description: Learn how to use the .NET Aspire Meilisearch hosting and client integration to run the Meilisearch container and accessing it via the Meilisearch client.
ms.date: 10/24/2024
---

# .NET Aspire Community Toolkit Meilisearch integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[!INCLUDE [banner](includes/banner.md)]

In this article, you learn how to use the .NET Aspire Meilisearch hosting integration to run [Meilisearch](https://meilisearch.com) container and accessing it via the [Meilisearch](https://github.com/meilisearch/meilisearch-dotnet) client.

## Hosting integration

To run the Meilisearch container, install the [📦 CommunityToolkit.Aspire.Hosting.Meilisearch](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.Meilisearch) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.Hosting.Meilisearch
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.Hosting.Meilisearch"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add Meilisearch resource

In the app host project, register and consume the Meilisearch integration using the `AddMeilisearch` extension method to add the Meilisearch container to the application builder.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var meilisearch = builder.AddMeilisearch("meilisearch");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(meilisearch);

// After adding all resources, run the app...
```

When .NET Aspire adds a container image to the app host, as shown in the preceding example with the `docker.io/getmeili/meilisearch` image, it creates a new Meilisearch instance on your local machine. A reference to your Meilisearch resource (the `meilisearch` variable) is added to the `ExampleProject`. The Meilisearch resource includes a randomly generated `master key` using the <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.CreateDefaultPasswordParameter*> method when a master key wasn't provided.

For more information, see [Container resource lifecycle](../fundamentals/orchestrate-resources.md#container-resource-lifecycle).

### Add Meilisearch resource with data volume

To add a data volume to the Meilisearch resource, call the `Aspire.Hosting.MeilisearchBuilderExtensions.WithDataVolume` method on the Meilisearch resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var meilisearch = builder.AddMeilisearch("meilisearch")
                         .WithDataVolume();

builder.AddProject<Projects.ExampleProject>()
       .WithReference(meilisearch);

// After adding all resources, run the app...
```

The data volume is used to persist the Meilisearch data outside the lifecycle of its container. The data volume is mounted at the `/meili_data` path in the Meilisearch container and when a `name` parameter isn't provided, the name is generated at random. For more information on data volumes and details on why they're preferred over [bind mounts](#add-meilisearch-resource-with-data-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

### Add Meilisearch resource with data bind mount

To add a data bind mount to the Meilisearch resource, call the `Aspire.Hosting.MeilisearchBuilderExtensions.WithDataBindMount` method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var meilisearch = builder.AddMeilisearch("meilisearch")
                         .WithDataBindMount(
                             source: @"C:\Meilisearch\Data");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(meilisearch);

// After adding all resources, run the app...
```

[!INCLUDE [data-bind-mount-vs-volumes](../includes/data-bind-mount-vs-volumes.md)]

Data bind mounts rely on the host machine's filesystem to persist the Meilisearch data across container restarts. The data bind mount is mounted at the `C:\Meilisearch\Data` on Windows (or `/Meilisearch/Data` on Unix) path on the host machine in the Meilisearch container. For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

### Add Meilisearch resource with master key parameter

When you want to explicitly provide the master key used by the container image, you can provide these credentials as parameters. Consider the following alternative example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var masterkey = builder.AddParameter("masterkey", secret: true);
var meilisearch = builder.AddMeilisearch("meilisearch", masterkey);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(meilisearch);

// After adding all resources, run the app...
```

For more information on providing parameters, see [External parameters](../fundamentals/external-parameters.md).

## Client integration

To get started with the .NET Aspire Meilisearch client integration, install the [📦 CommunityToolkit.Aspire.Meilisearch](https://nuget.org/packages/CommunityToolkit.Aspire.Meilisearch) NuGet package in the client-consuming project, that is, the project for the application that uses the Meilisearch client.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.Meilisearch
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.Meilisearch"
                  Version="*" />
```

---

### Add Meilisearch client

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the `Microsoft.Extensions.Hosting.AspireMeilisearchExtensions.AddMeilisearchClient` extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register an `MeilisearchClient` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddMeilisearchClient(connectionName: "meilisearch");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the Meilisearch resource in the app host project. For more information, see [Add Meilisearch resource](#add-meilisearch-resource).

You can then retrieve the `MeilisearchClient` instance using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(MeilisearchClient client)
{
    // Use client...
}
```

### Add keyed Meilisearch client

There might be situations where you want to register multiple `MeilisearchClient` instances with different connection names. To register keyed Meilisearch clients, call the `Microsoft.Extensions.Hosting.AspireMeilisearchExtensions.AddKeyedMeilisearchClient`

```csharp
builder.AddKeyedMeilisearchClient(name: "products");
builder.AddKeyedMeilisearchClient(name: "orders");
```

Then you can retrieve the `MeilisearchClient` instances using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(
    [FromKeyedServices("products")] MeilisearchClient productsClient,
    [FromKeyedServices("orders")] MeilisearchClient ordersClient)
{
    // Use clients...
}
```

For more information on keyed services, see [.NET dependency injection: Keyed services](/dotnet/core/extensions/dependency-injection#keyed-services).

### Configuration

The .NET Aspire Meilisearch client integration provides multiple options to configure the server connection based on the requirements and conventions of your project.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddMeilisearchClient`:

```csharp
builder.AddMeilisearchClient("meilisearch");
```

Then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "meilisearch": "Endpoint=http://localhost:19530/;MasterKey=123456!@#$%"
  }
}
```

#### Use configuration providers

The .NET Aspire Meilisearch Client integration supports <xref:Microsoft.Extensions.Configuration>. It loads the `CommunityToolkit.Aspire.Meilisearch.MeilisearchClientSettings` from configuration by using the `Aspire:Meilisearch:Client` key. Consider the following example _appsettings.json_ that configures some of the options:

```json
{
  "Aspire": {
    "Meilisearch": {
      "Client": {
        "Endpoint": "http://localhost:19530/",
        "MasterKey": "123456!@#$%"
      }
    }
  }
}
```

#### Use inline delegates

Also you can pass the `Action<MeilisearchClientSettings> configureSettings` delegate to set up some or all the options inline, for example to set the API key from code:

```csharp
builder.AddMeilisearchClient(
    "meilisearch",
    static settings => settings.MasterKey = "123456!@#$%");
```

#### Client integration health checks

The .NET Aspire Meilisearch integration uses the configured client to perform a `IsHealthyAsync`. If the result is `true`, the health check is considered healthy, otherwise it's unhealthy. Likewise, if there's an exception, the health check is considered unhealthy with the error propagating through the health check failure.

## See also

- [Meilisearch](https://meilisearch.com)
- [Meilisearch Client](https://github.com/meilisearch/meilisearch-dotnet)
- [.NET Aspire Community Toolkit GitHub repo](https://github.com/CommunityToolkit/Aspire)
