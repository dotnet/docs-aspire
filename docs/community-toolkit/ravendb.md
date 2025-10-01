---
title: Aspire Community Toolkit RavenDB integration
author: shiranshalom
description: Learn how to use the Aspire RavenDB hosting and client integrations to run a RavenDB container and access it via the RavenDB client.
ms.date: 5/27/2025
---

# Aspire Community Toolkit RavenDB integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[!INCLUDE [banner](includes/banner.md)]

[RavenDB](https://ravendb.net/) is a high-performance, open-source NoSQL database designed for fast, efficient, and scalable data storage. It supports advanced features like ACID transactions, distributed data replication, and time-series data management, making it an excellent choice for modern application development. The Aspire RavenDB integration enables you to connect to existing RavenDB instances or create new instances from .NET using the [docker.io/library/ravendb container image](https://hub.docker.com/r/ravendb/ravendb).

## Hosting integration

The RavenDB hosting integration models the server as the `RavenDBServerResource` type and the database as the `RavenDBDatabaseResource` type. To access these types and APIs, add the [📦 CommunityToolkit.Aspire.Hosting.RavenDB](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.RavenDB) NuGet package in the [AppHost](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.Hosting.RavenDB
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.Hosting.RavenDB"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add RavenDB server resource and database resource

To set up RavenDB in your AppHost project, call one of the `AddRavenDB` extension methods on the `builder` instance to add a RavenDB server resource, then call `AddDatabase` on the server resource to add a database. Here’s an example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var ravenServer = builder.AddRavenDB("ravenServer");
var ravendb = ravenServer.AddDatabase("ravendb");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(ravendb)
       .WaitFor(ravendb);

// After adding all resources, build and run the app...
```

> [!IMPORTANT]
> A valid RavenDB license is required.  If you don’t have one yet, you can request a free Community license [here](https://ravendb.net/license/request/community).

When Aspire adds a container image to the AppHost, as shown in the preceding example with the `docker.io/ravendb/ravendb` image, it creates a new RavenDB instance on your local machine. A reference to your RavenDB database resource (the `ravendb` variable) is added to the `ExampleProject`.

For more information, see [Container resource lifecycle](../fundamentals/orchestrate-resources.md#container-resource-lifecycle).

### Add RavenDB server resource with data volume

To add a data volume to the RavenDB server resource, call the `Aspire.Hosting.RavenDBBuilderExtensions.WithDataVolume` method on the RavenDB server resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var ravenServer = builder.AddRavenDB("ravenServer")
                         .WithDataVolume();

builder.AddProject<Projects.ExampleProject>()
       .WithReference(ravenServer)
       .WaitFor(ravenServer);
```

The data volume remains available after the container's lifecycle ends, preserving RavenDB data. The data volume is mounted at the `/var/lib/ravendb/data` path in the RavenDB container and when a `name` parameter isn't provided, the name is generated at random. For more information on data volumes and details on why they're preferred over [bind mounts](#add-ravendb-server-resource-with-data-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

### Add RavenDB server resource with data bind mount

To add a data bind mount to the RavenDB server resource, call the `Aspire.Hosting.RavenDBBuilderExtensions.WithDataBindMount` method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var ravenServer = builder.AddRavenDB("ravenServer")
                         .WithDataBindMount(source: @"C:\RavenDb\Data");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(ravenServer)
       .WaitFor(ravenServer);
```

[!INCLUDE [data-bind-mount-vs-volumes](../includes/data-bind-mount-vs-volumes.md)]

Data bind mounts rely on the host machine's filesystem to persist the RavenDB data across container restarts. The data bind mount is mounted at the `C:\RavenDb\Data` on Windows (or `/RavenDB/Data` on Unix) path on the host machine in the RavenDB container. For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

### Add secured RavenDB server resource  

To create a new secured RavenDB instance using settings from a pre-configured _settings.json_ file or a self-signed certificate, use the `RavenDBServerSettings.Secured` method or `RavenDBServerSettings.SecuredWithLetsEncrypt` for Let’s Encrypt configurations. These methods allow you to specify the domain URL, certificate details, and additional server settings.
Here’s an example of how to add a secured RavenDB server resource using Let's Encrypt:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var serverSettings = RavenDBServerSettings.SecuredWithLetsEncrypt(
    domainUrl: "https://mycontainer.development.run",
    certificatePath: "/etc/ravendb/security/cluster.server.certificate.mycontainer.pfx");

var ravendb = builder.AddRavenDB("ravenSecuredServer", serverSettings)
    .WithBindMount("C:/RavenDB/Server/Security", "/etc/ravendb/security", false)
    .AddDatabase("ravendbSecured");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(ravendb)
       .WaitFor(ravendb);
```

> [!IMPORTANT]
> Ensure the certificate path is accessible to the container by bind-mounting it to `/etc/ravendb/security`.

### Hosting integration health checks

The RavenDB hosting integration automatically adds a health check for the RavenDB server resource, verifying that the server is running and reachable.

The hosting integration relies on the [📦 AspNetCore.HealthChecks.RavenDB](https://www.nuget.org/packages/AspNetCore.HealthChecks.RavenDB) NuGet package.

## Client integration

To get started with the Aspire RavenDB client integration, install the [📦 CommunityToolkit.Aspire.RavenDB.Client](https://nuget.org/packages/CommunityToolkit.Aspire.RavenDB.Client) NuGet package in the client-consuming project, that is, the project for the application that uses the RavenDB client. The RavenDB client integration registers an [IDocumentStore](https://ravendb.net/docs/article-page/6.2/csharp/client-api/what-is-a-document-store) instance, which serves as the entry point for interacting with the RavenDB server resource or an existing RavenDB instance. If your AppHost includes RavenDB database resources, the associated [IDocumentSession](https://ravendb.net/docs/article-page/6.2/csharp/client-api/session/what-is-a-session-and-how-does-it-work) and [IAsyncDocumentSession](https://ravendb.net/docs/article-page/6.2/csharp/client-api/session/what-is-a-session-and-how-does-it-work) instances are also registered for dependency injection.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.RavenDB.Client
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.RavenDB.Client"
                  Version="*" />
```

---

### Add RavenDB client

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the `Microsoft.Extensions.Hosting.RavenDBClientExtension.AddRavenDBClient` extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register a `IDocumentStore` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddRavenDBClient(connectionName: "ravendb");
```

> [!TIP]  
> The `connectionName` parameter must match the name used when adding the RavenDB server resource (or the database resource, if provided) in the AppHost project. In other words, when you call `AddDatabase` and provide a name of `ravendb`, that same name should be used when calling `AddRavenDBClient`. For more information, see [Add RavenDB server resource and database resource](#add-ravendb-server-resource-and-database-resource).

You can then retrieve the `IDocumentStore` instance using dependency injection. For example, to retrieve the client from an example service:

```csharp
public class ExampleService(IDocumentStore client)
{
    // Use client...
}
```

#### Add RavenDB client using `RavenDBClientSettings`

The `AddRavenDBClient` method provides overloads that accept a `RavenDBClientSettings` object. This allows you to use the client integration independently of the hosting integration.
The `RavenDBClientSettings` class contains the parameters needed to establish a connection. More details about the available configuration options can be found in the [Configuration options](#configuration-options) section below.

Here’s an example:

```csharp
var settings = new RavenDBClientSettings
{
    Urls = new[] { serverUrl },
    DatabaseName =  myDatabaseName,
    Certificate = myCertificate
};

builder.AddRavenDBClient(settings: settings);
```

> [!Note]  
> These methods are ideal for connecting to an existing RavenDB instance without relying on the hosting integration. This is particularly useful if you already have a standalone instance running (e.g., in the cloud) and want to connect to it using its specific details.

After registration, you can retrieve the `IDocumentStore` instance and its associated `IDocumentSession` and `IAsyncDocumentSession` instances as follows:

```csharp
var documentStore = host.Services.GetRequiredService<IDocumentStore>();
var session = host.Services.GetRequiredService<IDocumentSession>();
var asyncSession = host.Services.GetRequiredService<IAsyncDocumentSession>();
```

### Add keyed RavenDB client

If your application requires multiple `IDocumentStore` instances with different connection configurations, you can register keyed RavenDB clients using the `Microsoft.Extensions.Hosting.RavenDBClientExtension.AddKeyedRavenDBClient` extension method:

```csharp
builder.AddKeyedRavenDBClient(serviceKey: "production", connectionName: "production");
builder.AddKeyedRavenDBClient(serviceKey: "testing", connectionName: "testing");
```

Then you can retrieve the `IDocumentStore` instances using dependency injection. For example, to retrieve a connection from an example service:

```csharp
public class ExampleService(
    [FromKeyedServices("production")] IDocumentStore production,
    [FromKeyedServices("testing")] IDocumentStore testing)
{
    // Use databases...
}
```

For more information on keyed services, see [.NET dependency injection: Keyed services](/dotnet/core/extensions/dependency-injection#keyed-services).

### Configuration

The Aspire RavenDB Client integration provides multiple configuration approaches and options to meet the requirements and conventions of your project.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, provide the name of the connection string when calling `builder.AddRavenDBClient`:

```csharp
builder.AddRavenDBClient("ravendb");
```

The connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "ravendb": "Url=http://localhost:8080/;Database=ravendb"
  }
}
```

#### Use configuration providers

The Aspire RavenDB integration supports <xref:Microsoft.Extensions.Configuration>. It loads the `RavenDBClientSettings` from the configuration using the `Aspire:RavenDB:Client` key. Consider the following example _appsettings.json_ that configures some of the options:

```json
{
  "Aspire": {
    "RavenDB": {
      "Client": {
        "ConnectionString": "URL=http://localhost:8080;Database=ravendb",
        "DisableHealthChecks": false,
        "HealthCheckTimeout": 10000,
        "DisableTracing": false
      }
    }
  }
}
```

#### Use inline configurations

You can also pass the `Action<RavenDBClientSettings>` delegate to set up some or all the options inline:

```csharp
builder.AddRavenDBClient(connectionName: "ravendb", configureSettings: 
    settings => 
    {
        settings.CreateDatabase = true;
        settings.Certificate = myCertificate;
        settings.DisableTracing = true;
    }
```

### Configuration options

The Aspire RavenDB client integration provides flexible configuration options through the `RavenDBClientSettings` class, enabling you to tailor the connection to your project's requirements. Here are the key properties:

| Name                  | Description                                                                           |
|-----------------------|---------------------------------------------------------------------------------------|
| `Urls`    | The connection URLs (`string[]`) of the RavenDB cluster.                 |
| `DatabaseName`    | Optional. The name of the RavenDB database to create or connect to. |
| `CertificatePath`    | Optional. The path to the certificate for secured RavenDB instances. |
| `CertificatePassword`    | Optional. The password for the certificate, if required. |
| `Certificate`    | Optional. An `X509Certificate2` instance for secured RavenDB instances. |
| `CreateDatabase` | A boolean value that indicates whether a new database should be created if it does not already exist.  |
| `ModifyDocumentStore` | Optional. An `Action` to modify the `IDocumentStore` instance. |
| `DisableHealthChecks` | A boolean value that indicates whether the database health check is disabled or not.  |
| `HealthCheckTimeout`  | An `int?` value that indicates the RavenDB health check timeout in milliseconds.      |
| `DisableTracing`      | A boolean value that indicates whether the OpenTelemetry tracing is disabled or not.  |

#### Client integration health checks

The Aspire RavenDB client integration uses the configured client to perform a `IsHealthyAsync`. If the result is `true`, the health check is considered healthy, otherwise it's unhealthy. Likewise, if there's an exception, the health check is considered unhealthy with the error propagating through the health check failure.

## See also

- [RavenDB](https://ravendb.net/)
- [Running in a Docker Container](https://ravendb.net/docs/article-page/6.2/csharp/start/installation/running-in-docker-container)
- [RavenDB Code](https://github.com/ravendb/ravendb)
- [Aspire Community Toolkit GitHub repo](https://github.com/CommunityToolkit/Aspire)
