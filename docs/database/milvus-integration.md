---
title: .NET Aspire Milvus database integration
description: Learn how to use the .NET Aspire Milvus database integration, which includes both hosting and client integrations.
ms.date: 02/14/2025
uid: database/milvus-integration
---

# .NET Aspire Milvus database integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[Milvus](https://milvus.io/) is an open-source vector database system that efficiently stores, indexes, and searches large-scale vector data. It's commonly used in machine learning, artificial intelligence, and data science applications.

Vector data encodes information as mathematical vectors, which are arrays of numbers or coordinates. Machine learning and AI systems often use vectors to represent unstructured objects like images, text, audio, or video. Each dimension in the vector describes a specific characteristic of the object. By comparing them, systems can classify, search, and identify clusters of objects.

In this article, you learn how to use the .NET Aspire Milvus database integration. The .NET Aspire Milvus database integration enables you to connect to existing Milvus databases or create new instances with the [`milvusdb/milvus` container image](https://hub.docker.com/r/milvusdb/milvus).

## Hosting integration

The Milvus database hosting integration models the server as the <xref:Aspire.Hosting.Milvus.MilvusServerResource> type and the database as the <xref:Aspire.Hosting.ApplicationModel.MilvusDatabaseResource> type. To access these types and APIs, add the [📦 Aspire.Hosting.Milvus](https://www.nuget.org/packages/Aspire.Hosting.Milvus) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Milvus
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Milvus"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add Milvus server and database resources

In your app host project, call <xref:Aspire.Hosting.MilvusBuilderExtensions.AddMilvus*> to add and return a Milvus resource builder. Chain a call to the returned resource builder to <xref:Aspire.Hosting.MilvusBuilderExtensions.AddDatabase*>, to add a Milvus database resource.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var milvus = builder.AddMilvus("milvus")
                    .WithLifetime(ContainerLifetime.Persistent);

var milvusdb = milvus.AddDatabase("milvusdb");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(milvusdb)
       .WaitFor(milvusdb);

// After adding all resources, run the app...
```

> [!NOTE]
> The Milvus container can be slow to start, so it's best to use a _persistent_ lifetime to avoid unnecessary restarts. For more information, see [Container resource lifetime](../fundamentals/app-host-overview.md#container-resource-lifetime).

When .NET Aspire adds a container image to the app host, as shown in the preceding example with the `milvusdb/milvus` image, it creates a new Milvus instance on your local machine. A reference to your Milvus resource builder (the `milvus` variable) is used to add a database. The database is named `milvusdb` and then added to the `ExampleProject`.

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` named `milvusdb`.

> [!TIP]
> If you'd rather connect to an existing Milvus server, call <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> instead. For more information, see [Reference existing resources](../fundamentals/app-host-overview.md#reference-existing-resources).

### Handling credentials and passing other parameters for the Milvus resource

The Milvus resource includes default credentials with a `username` of `root` and the password `Milvus`. Milvus supports configuration-based default passwords by using the environment variable `COMMON_SECURITY_DEFAULTROOTPASSWORD`. To change the default password in the container, pass an `apiKey` parameter when calling the `AddMilvus` hosting API:

```csharp
var apiKey = builder.AddParameter("apiKey", secret: true);

var milvus = builder.AddMilvus("milvus", apiKey);

var myService = builder.AddProject<Projects.ExampleProject>()
                       .WithReference(milvus);
```

The preceding code gets a parameter to pass to the `AddMilvus` API, and internally assigns the parameter to the `COMMON_SECURITY_DEFAULTROOTPASSWORD` environment variable of the Milvus container. The `apiKey` parameter is usually specified as a _user secret_:

```json
{
  "Parameters": {
    "apiKey": "Non-default-P@ssw0rd"
  }
}
```

For more information, see [External parameters](../fundamentals/external-parameters.md).

### Add a Milvus resource with a data volume

To add a data volume to the Milvus service resource, call the <xref:Aspire.Hosting.MilvusBuilderExtensions.WithDataVolume*> method on the Milvus resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var milvus = builder.AddMilvus("milvus")
                    .WithDataVolume();

var milvusdb = milvus.AddDatabase("milvusdb");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(milvusdb)
       .WaitFor(milvusdb);

// After adding all resources, run the app...
```

The data volume is used to persist the Milvus data outside the lifecycle of its container. The data volume is mounted at the `/var/lib/milvus` path in the SQL Server container and when a `name` parameter isn't provided, the name is generated at random. For more information on data volumes and details on why they're preferred over [bind mounts](#add-a-milvus-resource-with-a-data-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

### Add a Milvus resource with a data bind mount

To add a data bind mount to the Milvus resource, call the <xref:Aspire.Hosting.MilvusBuilderExtensions.WithDataBindMount*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var milvus = builder.AddMilvus("milvus")
                    .WithDataBindMount(source: @"C:\Milvus\Data");

var milvusdb = milvus.AddDatabase("milvusdb");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(milvusdb)
       .WaitFor(milvusdb);

// After adding all resources, run the app...
```

[!INCLUDE [data-bind-mount-vs-volumes](../includes/data-bind-mount-vs-volumes.md)]

Data bind mounts rely on the host machine's filesystem to persist the Milvus data across container restarts. The data bind mount is mounted at the `C:\Milvus\Data` on Windows (or `/Milvus/Data` on Unix) path on the host machine in the Milvus container. For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

### Create an Attu resource

[Attu](https://zilliz.com/attu) is a graphical user interface (GUI) and management tool designed to interact with Milvus and its databases. It includes rich visualization features that can help you investigate and understand your vector data.

If you want to use Attu to manage Milvus in your .NET Aspire solution, call the <xref:Aspire.Hosting.MilvusBuilderExtensions.WithAttu*> extension method on your Milvus resource. The method creates a container from the [`zilliz/attu` image](https://hub.docker.com/r/zilliz/attu):

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var milvus = builder.AddMilvus("milvus")
                    .WithAttu()
                    .WithLifetime(ContainerLifetime.Persistent);

var milvusdb = milvus.AddDatabase("milvusdb");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(milvusdb)
       .WaitFor(milvusdb);

// After adding all resources, run the app...
```

When you debug the .NET Aspire solution, you'll see an Attu container listed in the solution's resources. Select the resource's endpoint to open the GUI and start managing databases.

## Client integration (Preview)

To get started with the .NET Aspire Milvus client integration, install the [📦 Aspire.Milvus.Client](https://www.nuget.org/packages/Aspire.Milvus.Client) NuGet package in the client-consuming project, that is, the project for the application that uses the Milvus database client. The Milvus client integration registers a [Milvus.Client.MilvusClient](https://github.com/milvus-io/milvus-sdk-csharp) instance that you can use to interact with Milvus databases.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Milvus.Client
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Milvus.Client"
                  Version="*" />
```

---

### Add a Milvus client

In the _Program.cs_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireMilvusExtensions.AddMilvusClient*> extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register a `MilvusClient` for use through the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddMilvusClient("milvusdb");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the Milvus database resource in the app host project. In other words, when you call `AddDatabase` and provide a name of `milvusdb` that same name should be used when calling `AddMilvusClient`. For more information, see [Add a Milvus server resource and database resource](#add-milvus-server-and-database-resources).

You can then retrieve the `MilvusClient` instance using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(MilvusClient client)
{
    // Use the Milvus Client...
}
```

For more information on dependency injection, see [.NET dependency injection](/dotnet/core/extensions/dependency-injection).

### Add a keyed Milvus client

There might be situations where you want to register multiple `MilvusClient` instances with different connection names. To register keyed Milvus clients, call the <xref:Microsoft.Extensions.Hosting.AspireMilvusExtensions.AddKeyedMilvusClient*> method:

```csharp
builder.AddKeyedMilvusClient(name: "mainDb");
builder.AddKeyedMilvusClient(name: "loggingDb");
```

> [!IMPORTANT]
> When using keyed services, it's expected that your Milvus resource configured two named databases, one for the `mainDb` and one for the `loggingDb`.

Then you can retrieve the `MilvusClient` instances using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(
    [FromKeyedServices("mainDb")] MilvusClient mainDbClient,
    [FromKeyedServices("loggingDb")] MilvusClient loggingDbClient)
{
    // Use clients...
}
```

For more information on keyed services, see [.NET dependency injection: Keyed services](/dotnet/core/extensions/dependency-injection#keyed-services).

### Configuration

The .NET Aspire Milvus client integration provides multiple options to configure the connection to Milvus based on the requirements and conventions of your project.

> [!TIP]
> The default use is `root` and the default password is `Milvus`. To configure a different password in the Milvus container, see [Handling credentials and passing other parameters for the Milvus resource](#handling-credentials-and-passing-other-parameters-for-the-milvus-resource). Use the following techniques to configure consuming client apps in your .NET Aspire solution with the same password or other settings.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddMilvusClient()`:

```csharp
builder.AddMilvusClient("milvus");
```

And then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "milvus": "Endpoint=http://localhost:19530/;Key=root:Non-default-P@ssw0rd"
  }
}
```

By default the `MilvusClient` uses the gRPC API endpoint.

#### Use configuration providers

The .NET Aspire Milvus client integration supports <xref:Microsoft.Extensions.Configuration>. It loads the <xref:Aspire.Milvus.Client.MilvusClientSettings> from configuration by using the `Aspire:Milvus:Client` key. The following snippet is an example of a _:::no-loc text="appsettings.json":::_ that configures some of the options:

```json
{
  "Aspire": {
    "Milvus": {
      "Client": {
        "Endpoint": "http://localhost:19530/",
        "Database": "milvusdb",
        "Key": "root:Non-default-P@ssw0rd",
        "DisableHealthChecks": false
      }
    }
  }
}
```

For the complete Milvus client integration JSON schema, see [Aspire.Milvus.Client/ConfigurationSchema.json](https://github.com/dotnet/aspire/blob/v9.1.0/src/Components/Aspire.Milvus.Client/ConfigurationSchema.json).

#### Use inline delegates

Also you can pass the `Action<MilvusSettings> configureSettings` delegate to set up some or all the options inline, for example to set the API key from code:

```csharp
builder.AddMilvusClient(
    "milvus",
    static settings => settings.Key = "root:Non-default-P@ssw0rd");
```

### Client integration health checks

By default, .NET Aspire integrations enable [health checks](../fundamentals/health-checks.md) for all services. For more information, see [.NET Aspire integrations overview](../fundamentals/integrations-overview.md).

The .NET Aspire Milvus database integration:

- Adds the health check when <xref:Aspire.Milvus.Client.MilvusClientSettings.DisableHealthChecks?displayProperty=nameWithType> is `false`, which attempts to connect to the Milvus server.
- Uses the configured client to perform a `HealthAsync`. If the result _is healthy_, the health check is considered healthy, otherwise it's unhealthy. Likewise, if there's an exception, the health check is considered unhealthy with the error propagating through the health check failure.

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

#### Logging

The .NET Aspire Milvus database integration uses standard .NET logging, and you'll see log entries from the following category:

- `Milvus.Client`

#### Tracing

The .NET Aspire Milvus database integration doesn't currently emit tracing activities because they are not supported by the `Milvus.Client` library.

#### Metrics

The .NET Aspire Milvus database integration doesn't currently emit metrics because they are not supported by the `Milvus.Client` library.

## See also

- [Milvus](https://milvus.io/)
- [Milvus GitHub repo](https://github.com/milvus-io/milvus)
- [Milvus .NET SDK](https://github.com/milvus-io/milvus-sdk-csharp)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
