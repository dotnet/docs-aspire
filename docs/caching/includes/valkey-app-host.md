---
ms.topic: include
---

The Valkey hosting integration models a Valkey resource as the <xref:Aspire.Hosting.ApplicationModel.ValkeyResource> type. To access this type and APIs that allow you to add it to your [app model](xref:dotnet/aspire/app-host#define-the-app-model), install the [📦 Aspire.Hosting.Valkey](https://www.nuget.org/packages/Aspire.Hosting.Valkey) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Valkey
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Valkey"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add Valkey resource

In your app host project, call <xref:Aspire.Hosting.ValkeyBuilderExtensions.AddValkey*> on the `builder` instance to add a Valkey resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddValkey("cache");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(cache);

// After adding all resources, run the app...
```

When .NET Aspire adds a container image to the app host, as shown in the preceding example with the `docker.io/valkey/valkey` image, it creates a new Valkey instance on your local machine. A reference to your Valkey resource (the `cache` variable) is added to the `ExampleProject`.

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` named `"cache"`. For more information, see [Container resource lifecycle](../../fundamentals/app-host-overview.md#container-resource-lifecycle).

> [!TIP]
> If you'd rather connect to an existing Valkey instance, call <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> instead. For more information, see [Reference existing resources](../../fundamentals/app-host-overview.md#reference-existing-resources).

### Add Valkey resource with data volume

To add a data volume to the Valkey resource, call the <xref:Aspire.Hosting.ValkeyBuilderExtensions.AddValkey*> method on the Valkey resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddValkey("cache")
                   .WithDataVolume(isReadOnly: false);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(cache);

// After adding all resources, run the app...
```

The data volume is used to persist the Valkey data outside the lifecycle of its container. The data volume is mounted at the `/data` path in the Valkey container and when a `name` parameter isn't provided, the name is generated at random. For more information on data volumes and details on why they're preferred over [bind mounts](#add-valkey-resource-with-data-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

### Add Valkey resource with data bind mount

To add a data bind mount to the Valkey resource, call the <xref:Aspire.Hosting.ValkeyBuilderExtensions.WithDataBindMount*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddValkey("cache")
                   .WithDataBindMount(
                       source: @"C:\Valkey\Data",
                       isReadOnly: false);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(cache);

// After adding all resources, run the app...
```

[!INCLUDE [data-bind-mount-vs-volumes](../../includes/data-bind-mount-vs-volumes.md)]

Data bind mounts rely on the host machine's filesystem to persist the Valkey data across container restarts. The data bind mount is mounted at the `C:\Valkey\Data` on Windows (or `/Valkey/Data` on Unix) path on the host machine in the Valkey container. For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

### Add Valkey resource with persistence

To add persistence to the Valkey resource, call the <xref:Aspire.Hosting.ValkeyBuilderExtensions.WithPersistence*>
method with either the data volume or data bind mount:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddValkey("cache")
                   .WithDataVolume()
                   .WithPersistence(
                       interval: TimeSpan.FromMinutes(5),
                       keysChangedThreshold: 100);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(cache);

// After adding all resources, run the app...
```

The preceding code adds persistence to the Redis resource by taking snapshots of the Valkey data at a specified interval and threshold. The `interval` is time between snapshot exports and the `keysChangedThreshold` is the number of key change operations required to trigger a snapshot. For more information on persistence, see [Redis docs: Persistence](https://redis.io/topics/persistence).
