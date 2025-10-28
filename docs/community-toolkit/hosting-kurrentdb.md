---
title: Aspire Community Toolkit KurrentDB integration
description: Learn how to use the Aspire KurrentDB hosting and client integration to run the KurrentDB container and accessing it via the KurrentDB client.
ms.date: 10/28/2025
ms.custom: sfi-ropc-nochange
---

# Aspire Community Toolkit KurrentDB integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[!INCLUDE [banner](includes/banner.md)]

In this article, you learn how to use the Aspire KurrentDB hosting integration to run [KurrentDB](https://www.kurrent.io) container and accessing it via the [KurrentDB](https://github.com/kurrent-io/KurrentDB-Client-Dotnet) client.

## Hosting integration

To run the KurrentDB container, install the [📦 CommunityToolkit.Aspire.Hosting.KurrentDB][hosting-nuget-link] NuGet package in the [AppHost](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.Hosting.KurrentDB
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.Hosting.KurrentDB"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add KurrentDB resource

In the AppHost project, register and consume the x integration using the `AddKurrentDB` extension method to add the KurrentDB container to the application builder.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var kurrentDb = builder.AddKurrentDB("kurrentdb");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(kurrentDb);

// After adding all resources, run the app...
```

When Aspire adds a container image to the AppHost, as shown in the preceding example with the `docker.io/kurrentplatform/kurrentdb` image, it creates a new KurrentDB instance on your local machine. A reference to your KurrentDB resource (the `kurrentDb` variable) is added to the `ExampleProject`.

For more information, see [Container resource lifecycle](../fundamentals/orchestrate-resources.md#container-resource-lifecycle).

### Add KurrentDB resource with data volume

To add a data volume to the KurrentDB resource, call the `Aspire.Hosting.KurrentDBBuilderExtensions.WithDataVolume` method on the KurrentDB resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var kurrentDb = builder.AddKurrentDB("kurrentdb")
                        .WithDataVolume();

builder.AddProject<Projects.ExampleProject>()
       .WithReference(kurrentDb);

// After adding all resources, run the app...
```

The data volume is used to persist the KurrentDB data outside the lifecycle of its container. The data volume is mounted at the `/var/lib/kurrentdb` path in the KurrentDB container and when a `name` parameter isn't provided, the name is generated at random. For more information on data volumes and details on why they're preferred over [bind mounts](#add-kurrentdb-resource-with-data-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

### Add KurrentDB resource with data bind mount

To add a data bind mount to the KurrentDB resource, call the `Aspire.Hosting.KurrentDBBuilderExtensions.WithDataBindMount` method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var kurrentDb = builder.AddKurrentDB("kurrentdb")
                        .WithDataBindMount(source: @"C:\KurrentDB\Data");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(kurrentDb);

// After adding all resources, run the app...
```

[!INCLUDE [data-bind-mount-vs-volumes](../includes/data-bind-mount-vs-volumes.md)]

Data bind mounts rely on the host machine's filesystem to persist the KurrentDB data across container restarts. The data bind mount is mounted at the `C:\KurrentDB\Data` on Windows (or `/KurrentDB/Data` on Unix) path on the host machine in the KurrentDB container. For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

### Add KurrentDB resource with log volume

To add a log volume to the KurrentDB resource, call the <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithVolume*> extension method on the KurrentDB resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var kurrentDb = builder.AddKurrentDB("kurrentdb")
                        .WithVolume(name: "kurrentdb_logs", target: "/var/log/kurrentdb");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(kurrentDb);

// After adding all resources, run the app...
```

The data volume is used to persist the KurrentDB logs outside the lifecycle of its container. The data volume must be mounted at the `/var/log/kurrentdb` target path in the KurrentDB container and when a `name` parameter isn't provided, the name is generated at random. For more information on data volumes and details on why they're preferred over [bind mounts](#add-kurrentdb-resource-with-log-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

For more information about KurrentDB logs location, see [KurrentDB Resources: Logs](https://docs.kurrent.io/server/v25.1/diagnostics/logs.html#logs-location).

### Add KurrentDB resource with log bind mount

To add a log bind mount to the KurrentDB resource, call the <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithBindMount*> extension method on the KurrentDB resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var kurrentDb = builder.AddKurrentDB("kurrentdb")
                        .WithBindMount(@"C:\KurrentDB\Logs", "/var/log/kurrentdb");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(kurrentDb);

// After adding all resources, run the app...
```

[!INCLUDE [data-bind-mount-vs-volumes](../includes/data-bind-mount-vs-volumes.md)]

Data bind mounts rely on the host machine's filesystem to persist the KurrentDB logs across container restarts. The data bind mount is mounted at the `C:\KurrentDB\Logs` on Windows (or `/KurrentDB/Logs` on Unix) path on the host machine in the KurrentDB container. The target path must be set to the log folder used by the KurrentDB container (`/var/log/kurrentdb`).

For more information about KurrentDB logs location, see [KurrentDB Resources: Logs](https://docs.kurrent.io/server/v25.1/diagnostics/logs.html#logs-location).

For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

## Client integration

To get started with the Aspire KurrentDB client integration, install the [📦 CommunityToolkit.Aspire.KurrentDB][client-nuget-link] NuGet package in the client-consuming project, that is, the project for the application that uses the KurrentDB client.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.KurrentDB
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.KurrentDB"
                  Version="*" />
```

---

### Add KurrentDB client

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the `Microsoft.Extensions.Hosting.AspireKurrentDBExtensions.AddKurrentDBClient` extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register a `KurrentDBClient` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddKurrentDBClient(connectionName: "kurrentdb");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the KurrentDB resource in the AppHost project. For more information, see [Add KurrentDB resource](#add-kurrentdb-resource).

You can then retrieve the `KurrentDBClient` instance using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(KurrentDBClient client)
{
    // Use client...
}
```

### Add keyed KurrentDB client

There might be situations where you want to register multiple `KurrentDBClient` instances with different connection names. To register keyed KurrentDB clients, call the `Microsoft.Extensions.Hosting.AspireKurrentDBExtensions.AddKeyedKurrentDBClient`

```csharp
builder.AddKeyedKurrentDBClient(name: "accounts");
builder.AddKeyedKurrentDBClient(name: "orders");
```

Then you can retrieve the `KurrentDBClient` instances using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(
    [FromKeyedServices("accounts")] KurrentDBClient accountsClient,
    [FromKeyedServices("orders")] KurrentDBClient ordersClient)
{
    // Use clients...
}
```

For more information on keyed services, see [.NET dependency injection: Keyed services](/dotnet/core/extensions/dependency-injection#keyed-services).

### Configuration

The Aspire KurrentDB client integration provides multiple options to configure the server connection based on the requirements and conventions of your project.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddKurrentDBClient`:

```csharp
builder.AddKurrentDBClient("kurrentdb");
```

Then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "kurrentdb": "esdb://localhost:22113?tls=false"
  }
}
```

#### Use configuration providers

The Aspire KurrentDB Client integration supports <xref:Microsoft.Extensions.Configuration>. It loads the `CommunityToolkit.Aspire.KurrentDB.KurrentDBSettings` from configuration by using the `Aspire:KurrentDB:Client` key. Consider the following example _appsettings.json_ that configures some of the options:

```json
{
  "Aspire": {
    "KurrentDB": {
      "Client": {
        "ConnectionString": "esdb://localhost:22113?tls=false",
        "DisableHealthChecks": true
      }
    }
  }
}
```

#### Use inline delegates

Also you can pass the `Action<KurrentDBSettings> configureSettings` delegate to set up some or all the options inline, for example to set the API key from code:

```csharp
builder.AddKurrentDBClient(
    "kurrentdb",
    static settings => settings.DisableHealthChecks = true);
```

#### Client integration health checks

The Aspire KurrentDB integration uses the configured client to perform a `IsHealthyAsync`. If the result is `true`, the health check is considered healthy, otherwise it's unhealthy. Likewise, if there's an exception, the health check is considered unhealthy with the error propagating through the health check failure.

## See also

- [KurrentDB](https://www.kurrent.io)
- [KurrentDB Client](https://github.com/kurrent-io/KurrentDB-Client-Dotnet)
- [KurrentDB Docs](https://docs.kurrent.io/getting-started/introduction.html)
- [Aspire Community Toolkit GitHub repo](https://github.com/CommunityToolkit/Aspire)

[hosting-nuget-link]: https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.KurrentDB
[client-nuget-link]: https://nuget.org/packages/CommunityToolkit.Aspire.KurrentDB
