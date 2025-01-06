---
title: .NET Aspire Milvus database integration
description: Learn how to use the .NET Aspire Milvus database integration, which includes both hosting and client integrations.
ms.date: 08/22/2024
uid: database/milvus-integration
---

# .NET Aspire Milvus database integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[Milvus](https://milvus.io/) is an open-source vector database system that efficiently stores, indexes, and searches large-scale vector data. It's commonly used in machine learning, artificial intelligence, and data science applications.

Vector data encodes information as mathematical vectors, which are arrays of numbers or coordinates. Machine learning and AI systems often use vectors to represent unstructured objects like images, text, audio, or video. Each dimension in the vector describes a specific characteristic of the object. By comparing them, systems can classify, search, and identify clusters of objects.

In this article, you learn how to use the .NET Aspire Milvus database integration. The `Aspire.Milvus.Client` library registers a [MilvusClient](https://github.com/milvus-io/milvus-sdk-csharp) in the DI container for connecting to a Milvus server.

The .NET Aspire Milvus database integration enables you to connect to existing Milvus databases or create new instances with the [`milvusdb/milvus` container image](https://hub.docker.com/r/milvusdb/milvus).

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

### Add a Milvus server resource and database resource

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

### Handling credentials for the Milvus resource

The Milvus resource includes default credentials with a `username` of `root` and the password `Milvus`. Milvus supports configuration-based default passwords by using the environment variable `COMMON_SECURITY_DEFAULTROOTPASSWORD`. To change the default password in the container, pass an `apiKey` parameter when calling the `AddMilvus` hosting API:

```csharp
var apiKey = builder.AddParameter("apiKey");

var milvus = builder.AddMilvus("milvus", apiKey);

var myService = builder.AddProject<Projects.ExampleProject>()
                       .WithReference(milvus);
```

The preceding code gets a parameter to pass to the `AddMilvus` API, and internally assigns the parameter to the `COMMON_SECURITY_DEFAULTROOTPASSWORD` environment variable of the Milvus container. The `apiKey` parameter is usually specified as a _user secret_:

```json
{
  "Parameters": {
    "apiKey": "Non-default P@ssw0rd"
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









> AJMTODO: Original from here.

## Prerequisites

- Milvus server and connection string for accessing the server API endpoint.

## Get started

To get started with the .NET Aspire Milvus database integration, install the [📦 Aspire.Milvus.Client](https://www.nuget.org/packages/Aspire.Milvus.Client) NuGet package in the client-consuming project, i.e., the project for the application that uses the Milvus database client.

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

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _Program.cs_ file of your project, call the `AddMilvusClient` extension method to register a `MilvusClient` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddMilvusClient("milvus");
```

## App host usage

To model the Milvus resource in the app host, install the [📦 Aspire.Hosting.Milvus](https://www.nuget.org/packages/Aspire.Hosting.Milvus) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

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

In the _Program.cs_ file of `AppHost`, register a Milvus server and consume the connection using the following methods:

```csharp
var milvus = builder.AddMilvus("milvus");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(milvus);
```

The `WithReference` method configures a connection in the `MyService` project named `milvus`. In the _Program.cs_ file of `MyService`, the Milvus connection can be consumed using:

```csharp
builder.AddMilvusClient("milvus");
```

Milvus supports configuration-based (environment variable `COMMON_SECURITY_DEFAULTROOTPASSWORD`) default passwords. The default user is `root` and the default password is `Milvus`. To change the default password in the container, pass an `apiKey` parameter when calling the `AddMilvus` hosting API:

```csharp
var apiKey = builder.AddParameter("apiKey");

var milvus = builder.AddMilvus("milvus", apiKey);

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(milvus);
```

The preceding code gets a parameter to pass to the `AddMilvus` API, and internally assigns the parameter to the `COMMON_SECURITY_DEFAULTROOTPASSWORD` environment variable of the Milvus container. The `apiKey` parameter is usually specified as a _user secret_:

```json
{
  "Parameters": {
    "apiKey": "Non-default P@ssw0rd"
  }
}
```

For more information, see [External parameters](../fundamentals/external-parameters.md).

## Configuration

The .NET Aspire Milvus Client integration provides multiple options to configure the server connection based on the requirements and conventions of your project.

> [!TIP]
> The default use is `root` and the default password is `Milvus`. Currently, Milvus doesn't support changing the superuser password at startup. It needs to be manually changed with the client.

### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddMilvusClient()`:

```csharp
builder.AddMilvusClient("milvus");
```

And then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "milvus": "Endpoint=http://localhost:19530/;Key=root:123456!@#$%"
  }
}
```

By default the `MilvusClient` uses the gRPC API endpoint.

### Use configuration providers

The .NET Aspire Milvus Client integration supports [Microsoft.Extensions.Configuration](/dotnet/api/microsoft.extensions.configuration). It loads the `MilvusSettings` from configuration by using the `Aspire:Milvus:Client` key. Consider the following example _appsettings.json_ that configures some of the options:

```json
{
  "Aspire": {
    "Milvus": {
      "Client": {
        "Key": "root:123456!@#$%"
      }
    }
  }
}
```

### Use inline delegates

Also you can pass the `Action<MilvusSettings> configureSettings` delegate to set up some or all the options inline, for example to set the API key from code:

```csharp
builder.AddMilvusClient(
    "milvus",
    settings => settings.Key = "root:12345!@#$%");
```

[!INCLUDE [integration-health-checks](../includes/integration-health-checks.md)]

The .NET Aspire Milvus database integration uses the configured client to perform a `HealthAsync`. If the result _is healthy_, the health check is considered healthy, otherwise it's unhealthy. Likewise, if there's an exception, the health check is considered unhealthy with the error propagating through the health check failure.

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

### Logging

The .NET Aspire Milvus database integration uses standard .NET logging, and you'll see log entries from the following category:

- `Milvus.Client`

## See also

- [Milvus .NET SDK](https://github.com/milvus-io/milvus-sdk-csharp)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)

<!--
https://github.com/dotnet/docs-aspire/issues/1039

We added a new Aspire.Milvus.Client integration and Aspire.Hosting.Milvus hosting library in main. See:

Add Milvus Aspire Component aspire#796
Adds Milvus to the Aspire hosting/integration packages aspire#4179
https://github.com/dotnet/aspire/tree/main/src/Components/Aspire.Milvus.Client

Include links to:
- https://milvus.io/
- https://github.com/milvus-io/milvus
-->
