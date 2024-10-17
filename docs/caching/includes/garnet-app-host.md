---
ms.topic: include
---

The Garnet hosting integration models a Garnet resource as the <xref:Aspire.Hosting.ApplicationModel.GarnetResource> type. To access this type and APIs that allow you to add it to your [app model](xref:dotnet/aspire/app-host#define-the-app-model), install the [📦 Aspire.Hosting.Garnet](https://www.nuget.org/packages/Aspire.Hosting.Garnet) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Garnet
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Garnet"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add Garnet resource

In your app host project, call <xref:Aspire.Hosting.GarnetBuilderExtensions.AddGarnet*> on the `builder` instance to add a Garnet resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddGarnet("cache");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(cache);

// After adding all resources, run the app...
```

When .NET Aspire adds a container image to the app host, as shown in the preceding example with the `ghcr.io/microsoft/garnet` image, it creates a new Garnet instance on your local machine. A reference to your Garnet resource (the `cache` variable) is added to the `ExampleProject`.

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` named `"cache"`. For more information, see [Container resource lifecycle](../../fundamentals/app-host-overview.md#container-resource-lifecycle).

> [!TIP]
> If you'd rather connect to an existing Garnet instance, call <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> instead. For more information, see [Reference existing resources](../../fundamentals/app-host-overview.md#reference-existing-resources).

### Add Garnet resource with data volume

To add a data volume to the Garnet resource, call the <xref:Aspire.Hosting.GarnetBuilderExtensions.AddGarnet*> method on the Garnet resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddGarnet("cache")
                   .WithDataVolume(isReadOnly: false);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(cache);

// After adding all resources, run the app...
```

The data volume is used to persist the Garnet data outside the lifecycle of its container. The data volume is mounted at the `/data` path in the Garnet container and when a `name` parameter isn't provided, the name is generated at random. For more information on data volumes and details on why they're preferred over [bind mounts](#add-garnet-resource-with-data-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

### Add Garnet resource with data bind mount

To add a data bind mount to the Garnet resource, call the <xref:Aspire.Hosting.GarnetBuilderExtensions.WithDataBindMount*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddGarnet("cache")
                   .WithDataBindMount(
                       source: @"C:\Garnet\Data",
                       isReadOnly: false);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(cache);

// After adding all resources, run the app...
```

[!INCLUDE [data-bind-mount-vs-volumes](../../includes/data-bind-mount-vs-volumes.md)]

Data bind mounts rely on the host machine's filesystem to persist the Garnet data across container restarts. The data bind mount is mounted at the `C:\Garnet\Data` on Windows (or `/Garnet/Data` on Unix) path on the host machine in the Garnet container. For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

### Add Garnet resource with persistence

To add persistence to the Garnet resource, call the <xref:Aspire.Hosting.GarnetBuilderExtensions.WithPersistence*> method with either the data volume or data bind mount:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddGarnet("cache")
                   .WithDataVolume()
                   .WithPersistence(
                       interval: TimeSpan.FromMinutes(5),
                       keysChangedThreshold: 100);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(cache);

// After adding all resources, run the app...
```

The preceding code adds persistence to the Redis resource by taking snapshots of the Garnet data at a specified interval and threshold. The `interval` is time between snapshot exports and the `keysChangedThreshold` is the number of key change operations required to trigger a snapshot. For more information on persistence, see [Redis docs: Persistence](https://redis.io/topics/persistence).
