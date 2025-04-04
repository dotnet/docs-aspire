---
title: .NET Aspire Qdrant integration
description: Learn how to use the .NET Aspire Qdrant integration, which includes both hosting and client integrations.
ms.date: 01/13/2025
uid: database/qdrant-integration
---

# .NET Aspire Qdrant integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[Qdrant](https://qdrant.tech/) is an open-source vector similarity search engine that efficiently stores, indexes, and searches large-scale vector data. It's commonly used in machine learning, artificial intelligence, and data science applications.

Vector data encodes information as mathematical vectors, which are arrays of numbers or coordinates. Machine learning and AI systems often use vectors to represent unstructured objects like images, text, audio, or video. Each dimension in the vector describes a specific characteristic of the object. By comparing them, systems can classify, search, and identify clusters of objects.

In this article, you learn how to use the .NET Aspire Qdrant integration. The .NET Aspire Qdrant integration enables you to connect to existing Qdrant databases or create new instances with the [`qdrant/qdrant` container image](https://hub.docker.com/r/qdrant/qdrant).

## Hosting integration

The Qdrant hosting integration models the server as the <xref:Aspire.Hosting.ApplicationModel.QdrantServerResource> type. To access this type and APIs, add the [📦 Aspire.Hosting.Qdrant](https://www.nuget.org/packages/Aspire.Hosting.Qdrant) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Qdrant
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Qdrant"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add Qdrant resource

In your app host project, call <xref:Aspire.Hosting.QdrantBuilderExtensions.AddQdrant*> to add and return a Qdrant resource builder.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var qdrant = builder.AddQdrant("qdrant")
                    .WithLifetime(ContainerLifetime.Persistent);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(qdrant)
       .WaitFor(qdrant);

// After adding all resources, run the app...
```

> [!NOTE]
> The Qdrant container can be slow to start, so it's best to use a _persistent_ lifetime to avoid unnecessary restarts. For more information, see [Container resource lifetime](../fundamentals/orchestrate-resources.md#container-resource-lifetime).

When .NET Aspire adds a container image to the app host, as shown in the preceding example with the `qdrant/qdrant` image, it creates a new Qdrant instance on your local machine. The resource is named `qdrant` and then added to the `ExampleProject`.

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` named `qdrant`.

> [!TIP]
> If you'd rather connect to an existing Qdrant server, call <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> instead. For more information, see [Reference existing resources](../fundamentals/app-host-overview.md#reference-existing-resources).

> [!TIP]
> The `qdrant/qdrant` container image includes a web UI that you can use to explore your vectors and administer the database. To access this tool, start your .NET Aspire solution and then, in the .NET Aspire dashboard, select the endpoint for the Qdrant resource. In your browser's address bar, append **/dashboard** and press <kbd>Enter</kbd>.

### Handling API keys and passing other parameters for the Qdrant resource

To connect to Qdrant a client must pass the right API key. In the above code, when .NET Aspire adds a Qdrant resource to your solution, it sets the API key to a random string. If you want to use a specific API key instead, you can pass it as an `apiKey` parameter:

```csharp
var apiKey = builder.AddParameter("apiKey", secret: true);

var qdrant = builder.AddQdrant("qdrant", apiKey);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(qdrant);
```

Qdrant supports configuration-based default API keys by using the environment variable `QDRANT__SERVICE__API_KEY`.

The preceding code gets a parameter to pass to the `AddQdrant` API, and internally assigns the parameter to the `QDRANT__SERVICE__API_KEY` environment variable of the Qdrant container. The `apiKey` parameter is usually specified as a _user secret_:

```json
{
  "Parameters": {
    "apiKey": "Non-default-P@ssw0rd"
  }
}
```

For more information, see [External parameters](../fundamentals/external-parameters.md).

### Add Qdrant resource with data volume

To add a data volume to the Qdrant resource, call the <xref:Aspire.Hosting.QdrantBuilderExtensions.WithDataVolume*> extension method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var qdrant = builder.AddQdrant("qdrant")
                    .WithLifetime(ContainerLifetime.Persistent)
                    .WithDataVolume();

builder.AddProject<Projects.ExampleProject>()
       .WithReference(qdrant)
       .WaitFor(qdrant);

// After adding all resources, run the app...
```

The data volume is used to persist the Qdrant data outside the lifecycle of its container. The data volume is mounted at the `/qdrant/storage` path in the Qdrant container and when a `name` parameter isn't provided, the name is generated at random. For more information on data volumes and details on why they're preferred over [bind mounts](#add-qdrant-resource-with-data-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

### Add Qdrant resource with data bind mount

To add a data bind mount to the Qdrant resource, call the <xref:Aspire.Hosting.QdrantBuilderExtensions.WithDataBindMount*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var qdrant = builder.AddQdrant("qdrant")
                    .WithLifetime(ContainerLifetime.Persistent)
                    .WithDataBindMount(source: @"C:\Qdrant\Data");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(qdrant)
       .WaitFor(qdrant);

// After adding all resources, run the app...
```

[!INCLUDE [data-bind-mount-vs-volumes](../includes/data-bind-mount-vs-volumes.md)]

Data bind mounts rely on the host machine's filesystem to persist the Qdrant data across container restarts. The data bind mount is mounted at the `C:\Qdrant\Data` folder on Windows (or `/Qdrant/Data` on Unix) on the host machine in the Qdrant container. For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

### Hosting integration health checks

The Qdrant hosting integration automatically adds a health check for the Qdrant resource. The health check verifies that Qdrant is running and that a connection can be established to it.

## Client integration

To get started with the .NET Aspire Qdrant client integration, install the [📦 Aspire.Qdrant.Client](https://www.nuget.org/packages/Aspire.Qdrant.Client) NuGet package in the client-consuming project, that is, the project for the application that uses the Qdrant client. The Qdrant client integration registers a [Qdrant.Client.QdrantClient](https://github.com/qdrant/qdrant-dotnet/) instance that you can use to interact with Qdrant vector data.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Qdrant.Client
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Qdrant.Client"
                  Version="*" />
```

---

## Add a Qdrant client

In the _Program.cs_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireQdrantExtensions.AddQdrantClient*> extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register a `QdrantClient` for use through the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddQdrantClient("qdrant");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the Qdrant resource in the app host project. In other words, when you call `AddQdrant` and provide a name of `qdrant` that same name should be used when calling `AddQdrantClient`. For more information, see [Add Qdrant resource](#add-qdrant-resource).

You can then retrieve the `QdrantClient` instance using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(QdrantClient client)
{
    // Use client...
}
```

For more information on dependency injection, see [.NET dependency injection](/dotnet/core/extensions/dependency-injection).

## Add keyed Qdrant client

There might be situations where you want to register multiple `QdrantClient` instances with different connection names. To register keyed Qdrant clients, call the <xref:Microsoft.Extensions.Hosting.AspireQdrantExtensions.AddKeyedQdrantClient*> method:

```csharp
builder.AddKeyedQdrantClient(name: "mainQdrant");
builder.AddKeyedQdrantClient(name: "loggingQdrant");
```

Then you can retrieve the `QdrantClient` instances using dependency injection. For example, to retrieve the connections from an example service:

```csharp
public class ExampleService(
    [FromKeyedServices("mainQdrant")] QdrantClient mainQdrantClient,
    [FromKeyedServices("loggingQdrant")] QdrantClient loggingQdrantClient)
{
    // Use clients...
}
```

For more information on keyed services, see [.NET dependency injection: Keyed services](/dotnet/core/extensions/dependency-injection#keyed-services).

### Configuration

The .NET Aspire Qdrant client integration provides multiple options to configure the connection to Qdrant based on the requirements and conventions of your project.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddQdrantClient()`:

```csharp
builder.AddQdrantClient("qdrant");
```

Then .NET Aspire retrieves the connection string from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "qdrant": "Endpoint=http://localhost:6334;Key=123456!@#$%"
  }
}
```

By default the `QdrantClient` uses the gRPC API endpoint.

#### Use configuration providers

The .NET Aspire Qdrant client integration supports <xref:Microsoft.Extensions.Configuration>. It loads the <xref:Aspire.Qdrant.Client.QdrantClientSettings> from configuration by using the `Aspire:Qdrant:Client` key. The following is an example of an _:::no-loc text="appsettings.json":::_ that configures some of the options:

```json
{
  "Aspire": {
    "Qdrant": {
      "Client": {
        "Endpoint": "http://localhost:6334/",
        "Key": "123456!@#$%"
      }
    }
  }
}
```

For the complete Qdrant client integration JSON schema, see [Aspire.Qdrant.Client/ConfigurationSchema.json](https://github.com/dotnet/aspire/blob/v9.1.0/src/Components/Aspire.Qdrant.Client/ConfigurationSchema.json).

#### Use inline delegates

You can also pass the `Action<QdrantClientSettings> configureSettings` delegate to set up some or all the options inline, for example to set the API key from code:

```csharp
builder.AddQdrantClient(
    "qdrant", 
    settings => settings.Key = "12345!@#$%");
```

### Client integration health checks

By default, .NET Aspire integrations enable [health checks](../fundamentals/health-checks.md) for all services. For more information, see [.NET Aspire integrations overview](../fundamentals/integrations-overview.md).

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

#### Logging

The .NET Aspire Qdrant integration uses standard .NET logging, and you'll see log entries from the following category:

- `Qdrant.Client`

#### Tracing

The .NET Aspire Qdrant integration doesn't currently emit tracing activities because they are not supported by the `Qdrant.Client` library.

#### Metrics

The .NET Aspire Qdrant integration doesn't currently emit metrics because they are not supported by the `Qdrant.Client` library.

## See also

- [Qdrant](https://qdrant.tech/)
- [Qdrant documentation](https://qdrant.tech/documentation/quickstart/)
- [Qdrant GitHub repo](https://github.com/qdrant/qdrant)
- [Qdrant .NET SDK](https://github.com/qdrant/qdrant-dotnet)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
