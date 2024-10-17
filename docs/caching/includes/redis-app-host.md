---
ms.topic: include
---

The Redis hosting integration models a Redis resource as the <xref:Aspire.Hosting.ApplicationModel.RedisResource> type. To access this type and APIs that allow you to add it to your [app model](xref:dotnet/aspire/app-host#define-the-app-model), install the [📦 Aspire.Hosting.Redis](https://www.nuget.org/packages/Aspire.Hosting.Redis) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Redis
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Redis"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add Redis resource

In your app host project, call <xref:Aspire.Hosting.RedisBuilderExtensions.AddRedis*> on the `builder` instance to add a Redis resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(cache);

// After adding all resources, run the app...
```

When .NET Aspire adds a container image to the app host, as shown in the preceding example with the `docker.io/Redis/Redis` image, it creates a new Redis instance on your local machine. A reference to your Redis resource (the `cache` variable) is added to the `ExampleProject`.

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` named `"cache"`. For more information, see [Container resource lifecycle](../../fundamentals/app-host-overview.md#container-resource-lifecycle).

> [!TIP]
> If you'd rather connect to an existing Redis instance, call <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> instead. For more information, see [Reference existing resources](../../fundamentals/app-host-overview.md#reference-existing-resources).

### Add Redis resource with Redis Insights

To add the [Redis Insights](https://redis.io/insight/) to the Redis resource, call the `WithRedisInsights` method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
                   .WithRedisInsights();

builder.AddProject<Projects.ExampleProject>()
       .WithReference(cache);

// After adding all resources, run the app...
```

Redis Insights is a free graphical interface for analyzing Redis data across all operating systems and Redis deployments with the help of our AI assistant, Redis Copilot. .NET Aspire adds another container image [`docker.io/redis/redisinsight`](https://hub.docker.com/r/redis/redisinsight) to the app host that runs the commander app.

> [!TIP]
> To configure the host port for the `RedisInsightResource` call `WithHostPort` and provide the desired port number.

### Add Redis resource with Redis Commander

To add the [Redis Commander](https://joeferner.github.io/redis-commander/) to the Redis resource, call the <xref:Aspire.Hosting.RedisBuilderExtensions.WithRedisCommander*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
                   .WithRedisCommander();

builder.AddProject<Projects.ExampleProject>()
       .WithReference(cache);

// After adding all resources, run the app...
```

Redis Commander is a Node.js web application used to view, edit, and manage a Redis Database. .NET Aspire adds another container image [`docker.io/rediscommander/redis-commander`](https://hub.docker.com/r/rediscommander/redis-commander) to the app host that runs the commander app.

> [!TIP]
> To configure the host port for the <xref:Aspire.Hosting.Redis.RedisCommanderResource> call <xref:Aspire.Hosting.RedisBuilderExtensions.WithHostPort*> and provide the desired port number.

### Add Redis resource with data volume

To add a data volume to the Redis resource, call the <xref:Aspire.Hosting.RedisBuilderExtensions.WithDataVolume*> method on the Redis resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
                   .WithDataVolume(isReadOnly: false);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(cache);

// After adding all resources, run the app...
```

The data volume is used to persist the Redis data outside the lifecycle of its container. The data volume is mounted at the `/data` path in the Redis container and when a `name` parameter isn't provided, the name is generated at random. For more information on data volumes and details on why they're preferred over [bind mounts](#add-redis-resource-with-data-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

### Add Redis resource with data bind mount

To add a data bind mount to the Redis resource, call the <xref:Aspire.Hosting.RedisBuilderExtensions.WithDataBindMount*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
                   .WithDataBindMount(
                       source: @"C:\Redis\Data",
                       isReadOnly: false);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(cache);

// After adding all resources, run the app...
```

[!INCLUDE [data-bind-mount-vs-volumes](../../includes/data-bind-mount-vs-volumes.md)]

Data bind mounts rely on the host machine's filesystem to persist the Redis data across container restarts. The data bind mount is mounted at the `C:\Redis\Data` on Windows (or `/Redis/Data` on Unix) path on the host machine in the Redis container. For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

### Add Redis resource with persistence

To add persistence to the Redis resource, call the <xref:Aspire.Hosting.RedisBuilderExtensions.WithPersistence*> method with either the data volume or data bind mount:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
                   .WithDataVolume()
                   .WithPersistence(
                       interval: TimeSpan.FromMinutes(5),
                       keysChangedThreshold: 100);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(cache);

// After adding all resources, run the app...
```

The preceding code adds persistence to the Redis resource by taking snapshots of the Redis data at a specified interval and threshold. The `interval` is time between snapshot exports and the `keysChangedThreshold` is the number of key change operations required to trigger a snapshot. For more information on persistence, see [Redis docs: Persistence](https://redis.io/topics/persistence).
