---
title: .NET Aspire orchestration overview
description: Learn the fundamental concepts of .NET Aspire orchestration and explore the various APIs for adding resources and expressing dependencies.
ms.date: 02/19/2025
ms.topic: overview
uid: dotnet/aspire/app-host
---

# .NET Aspire orchestration overview

.NET Aspire provides APIs for expressing resources and dependencies within your distributed application. In addition to these APIs, [there's tooling](setup-tooling.md#install-net-aspire) that enables several compelling scenarios. The orchestrator is intended for _local development_ purposes and isn't supported in production environments.

<span id="terminology"></span>

Before continuing, consider some common terminology used in .NET Aspire:

- **App model**: A collection of resources that make up your distributed application (<xref:Aspire.Hosting.DistributedApplication>), defined within the <xref:Aspire.Hosting.ApplicationModel> namespace. For a more formal definition, see [Define the app model](#define-the-app-model).
- **App host/Orchestrator project**: The .NET project that orchestrates the _app model_, named with the _*.AppHost_ suffix (by convention).
- **Resource**: A [resource](#built-in-resource-types) is a dependent part of an application, such as a .NET project, container, executable, database, cache, or cloud service. It represents any part of the application that can be managed or referenced.
- **Integration**: An integration is a NuGet package for either the _app host_ that models a _resource_ or a package that configures a client for use in a consuming app. For more information, see [.NET Aspire integrations overview](integrations-overview.md).
- **Reference**: A reference defines a connection between resources, expressed as a dependency using the <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference*> API. For more information, see [Reference resources](#reference-resources) or [Reference existing resources](#reference-existing-resources).

> [!NOTE]
> .NET Aspire's orchestration is designed to enhance your _local development_ experience by simplifying the management of your cloud-native app's configuration and interconnections. While it's an invaluable tool for development, it's not intended to replace production environment systems like [Kubernetes](../deployment/overview.md#deploy-to-kubernetes), which are specifically designed to excel in that context.

## Define the app model

.NET Aspire empowers you to seamlessly build, provision, deploy, configure, test, run, and observe your distributed applications. All of these capabilities are achieved through the utilization of an _app model_ that outlines the resources in your .NET Aspire solution and their relationships. These resources encompass projects, executables, containers, and external services and cloud resources that your app depends on. Within every .NET Aspire solution, there's a designated [App host project](#app-host-project), where the app model is precisely defined using methods available on the <xref:Aspire.Hosting.IDistributedApplicationBuilder>. This builder is obtained by invoking <xref:Aspire.Hosting.DistributedApplication.CreateBuilder%2A?displayProperty=nameWithType>.

```csharp
// Create a new app model builder
var builder = DistributedApplication.CreateBuilder(args);

// TODO:
//   Add resources to the app model
//   Express dependencies between resources

builder.Build().Run();
```

## App host project

The app host project handles running all of the projects that are part of the .NET Aspire project. In other words, it's responsible for orchestrating all apps within the app model. The project itself is a .NET executable project that references the [📦 Aspire.Hosting.AppHost](https://www.nuget.org/packages/Aspire.Hosting.AppHost) NuGet package, sets the `IsAspireHost` property to `true`, and references the [.NET Aspire SDK](dotnet-aspire-sdk.md):

```xml
<Project Sdk="Microsoft.NET.Sdk">

    <Sdk Name="Aspire.AppHost.Sdk" Version="9.1.0" />
    
    <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>net9.0</TargetFramework>
        <IsAspireHost>true</IsAspireHost>
        <!-- Omitted for brevity -->
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="Aspire.Hosting.AppHost" Version="9.1.0" />
    </ItemGroup>

    <!-- Omitted for brevity -->

</Project>
```

The following code describes an app host `Program` with two project references and a Redis cache:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var apiservice = builder.AddProject<Projects.AspireApp_ApiService>("apiservice");

builder.AddProject<Projects.AspireApp_Web>("webfrontend")
       .WithExternalHttpEndpoints()
       .WithReference(cache)
       .WaitFor(cache)
       .WithReference(apiService)
       .WaitFor(apiService);

builder.Build().Run();
```

The preceding code:

- Creates a new app model builder using the <xref:Aspire.Hosting.DistributedApplication.CreateBuilder%2A> method.
- Adds a Redis `cache` resource named "cache" using the <xref:Aspire.Hosting.RedisBuilderExtensions.AddRedis*> method.
- Adds a project resource named "apiservice" using the <xref:Aspire.Hosting.ProjectResourceBuilderExtensions.AddProject%2A> method.
- Adds a project resource named "webfrontend" using the <xref:Aspire.Hosting.ProjectResourceBuilderExtensions.AddProject%2A> method.
  - Specifies that the project has external HTTP endpoints using the <xref:Aspire.Hosting.ResourceBuilderExtensions.WithExternalHttpEndpoints%2A> method.
  - Adds a reference to the `cache` resource and waits for it to be ready using the <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> and <xref:Aspire.Hosting.ResourceBuilderExtensions.WaitFor*> methods.
  - Adds a reference to the `apiservice` resource and waits for it to be ready using the <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> and <xref:Aspire.Hosting.ResourceBuilderExtensions.WaitFor*> methods.
- Builds and runs the app model using the <xref:Aspire.Hosting.DistributedApplicationBuilder.Build%2A> and <xref:Aspire.Hosting.DistributedApplication.Run%2A> methods.

The example code uses the [.NET Aspire Redis hosting integration](../caching/stackexchange-redis-integration.md#hosting-integration).

To help visualize the relationship between the app host project and the resources it describes, consider the following diagram:

:::image type="content" source="../media/app-host-resource-diagram.png" lightbox="../media/app-host-resource-diagram.png" alt-text="The relationship between the projects in the .NET Aspire Starter Application template.":::

Each resource must be uniquely named. This diagram shows each resource and the relationships between them. The container resource is named "cache" and the project resources are named "apiservice" and "webfrontend". The web frontend project references the cache and API service projects. When you're expressing references in this way, the web frontend project is saying that it depends on these two resources, the "cache" and "apiservice" respectively.

## Built-in resource types

.NET Aspire projects are made up of a set of resources. The primary base resource types in the [📦 Aspire.Hosting.AppHost](https://www.nuget.org/packages/Aspire.Hosting.AppHost) NuGet package are described in the following table:

| Method | Resource type | Description |
|--|--|--|
| <xref:Aspire.Hosting.ProjectResourceBuilderExtensions.AddProject%2A> | <xref:Aspire.Hosting.ApplicationModel.ProjectResource> | A .NET project, for example, an ASP.NET Core web app. |
| <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.AddContainer%2A> | <xref:Aspire.Hosting.ApplicationModel.ContainerResource> | A container image, such as a Docker image. |
| <xref:Aspire.Hosting.ExecutableResourceBuilderExtensions.AddExecutable%2A> | <xref:Aspire.Hosting.ApplicationModel.ExecutableResource> | An executable file, such as a [Node.js app](../get-started/build-aspire-apps-with-nodejs.md). |
| <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddParameter%2A> | <xref:Aspire.Hosting.ApplicationModel.ParameterResource> | A parameter resource that can be used to [express external parameters](external-parameters.md). |

Project resources represent .NET projects that are part of the app model. When you add a project reference to the app host project, the .NET Aspire SDK generates a type in the `Projects` namespace for each referenced project. For more information, see [.NET Aspire SDK: Project references](dotnet-aspire-sdk.md#project-references).

To add a project to the app model, use the <xref:Aspire.Hosting.ProjectResourceBuilderExtensions.AddProject%2A> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Adds the project "apiservice" of type "Projects.AspireApp_ApiService".
var apiservice = builder.AddProject<Projects.AspireApp_ApiService>("apiservice");
```

Projects can be replicated and scaled out by adding multiple instances of the same project to the app model. To configure replicas, use the <xref:Aspire.Hosting.ProjectResourceBuilderExtensions.WithReplicas*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Adds the project "apiservice" of type "Projects.AspireApp_ApiService".
var apiservice = builder.AddProject<Projects.AspireApp_ApiService>("apiservice")
                        .WithReplicas(3);
```

The preceding code adds three replicas of the "apiservice" project resource to the app model. For more information, see [.NET Aspire dashboard: Resource replicas](dashboard/explore.md#resource-replicas).

## Reference resources

A reference represents a dependency between resources. For example, you can probably imagine a scenario where you a web frontend depends on a Redis cache. Consider the following example app host `Program` C# code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

builder.AddProject<Projects.AspireApp_Web>("webfrontend")
       .WithReference(cache);
```

The "webfrontend" project resource uses <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> to add a dependency on the "cache" container resource. These dependencies can represent connection strings or [service discovery](../service-discovery/overview.md) information. In the preceding example, an environment variable is _injected_ into the "webfronend" resource with the name `ConnectionStrings__cache`. This environment variable contains a connection string that the `webfrontend` uses to connect to Redis via the [.NET Aspire Redis integration](../caching/stackexchange-redis-caching-overview.md), for example, `ConnectionStrings__cache="localhost:62354"`.

### Waiting for resources

In some cases, you might want to wait for a resource to be ready before starting another resource. For example, you might want to wait for a database to be ready before starting an API that depends on it. To express this dependency, use the <xref:Aspire.Hosting.ResourceBuilderExtensions.WaitFor*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var postgresdb = postgres.AddDatabase("postgresdb");

builder.AddProject<Projects.AspireApp_ApiService>("apiservice")
       .WithReference(postgresdb)
       .WaitFor(postgresdb);
```

In the preceding code, the "apiservice" project resource waits for the "postgresdb" database resource to enter the <xref:Aspire.Hosting.ApplicationModel.KnownResourceStates.Running?displayProperty=nameWithType>. The example code shows the [.NET Aspire PostgreSQL integration](../database/postgresql-integration.md), but the same pattern can be applied to other resources.

Other cases might warrant waiting for a resource to run to completion, either <xref:Aspire.Hosting.ApplicationModel.KnownResourceStates.Exited?displayProperty=nameWithType> or <xref:Aspire.Hosting.ApplicationModel.KnownResourceStates.Finished?displayProperty=nameWithType> before the dependent resource starts. To wait for a resource to run to completion, use the <xref:Aspire.Hosting.ResourceBuilderExtensions.WaitForCompletion*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var postgresdb = postgres.AddDatabase("postgresdb");

var migration = builder.AddProject<Projects.AspireApp_Migration>("migration")
                       .WithReference(postgresdb)
                       .WaitFor(postgresdb);

builder.AddProject<Projects.AspireApp_ApiService>("apiservice")
       .WithReference(postgresdb)
       .WaitForCompletion(migration);
```

In the preceding code, the "apiservice" project resource waits for the "migration" project resource to run to completion before starting. The "migration" project resource waits for the "postgresdb" database resource to enter the <xref:Aspire.Hosting.ApplicationModel.KnownResourceStates.Running?displayProperty=nameWithType>. This can be useful in scenarios where you want to run a database migration before starting the API service, for example.

### Configure explicit resource start

Project, executable and container resources are automatically started with your distributed application by default. A resource can be configured to wait for an explicit startup instruction with the `WithExplicitStart()` method. A resource configured with `WithExplicitStart()` is initialized with `KnownResourceStates.NotStarted`.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var postgresdb = postgres.AddDatabase("postgresdb");

builder.AddProject<Projects.AspireApp_DbMigration>("migration")
       .WithReference(postgresdb)
       .WithExplicitStart();
```

In the preceding code the `migration` resource is configured to not automatically start with the distributed application.

Resources with explicit start can be started from the .NET Aspire dashboard by clicking the "Start" command. For more information, see [.NET Aspire dashboard: Stop or Start a resource](dashboard/explore.md#stop-or-start-a-resource).

#### Forcing resource start in the dashboard

Waiting for a resource can be bypassed using the "Start" command in the dashboard. Clicking "Start" on a waiting resource in the dashboard instructs it to start immediately without waiting for the resource to be healthy or completed. This can be useful when you want to test a resource immediately and don't want to wait for the app to be in the right state.

### APIs for adding and expressing resources

.NET Aspire [hosting integrations](integrations-overview.md#hosting-integrations) and [client integrations](integrations-overview.md#client-integrations) are both delivered as NuGet packages, but they serve different purposes. While _client integrations_ provide client library configuration for consuming apps outside the scope of the app host, _hosting integrations_ provide APIs for expressing resources and dependencies within the app host. For more information, see [.NET Aspire integrations overview: Integration responsibilities](integrations-overview.md#integration-responsibilities).

### Express container resources

To express a <xref:Aspire.Hosting.ApplicationModel.ContainerResource> you add it to an <xref:Aspire.Hosting.IDistributedApplicationBuilder> instance by calling the <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.AddContainer%2A> method:

#### [Docker](#tab/docker)

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var ollama = builder.AddContainer("ollama", "ollama/ollama")
    .WithBindMount("ollama", "/root/.ollama")
    .WithBindMount("./ollamaconfig", "/usr/config")
    .WithHttpEndpoint(port: 11434, targetPort: 11434, name: "ollama")
    .WithEntrypoint("/usr/config/entrypoint.sh")
    .WithContainerRuntimeArgs("--gpus=all");
```

For more information, see [GPU support in Docker Desktop](https://docs.docker.com/desktop/gpu/).

#### [Podman](#tab/podman)

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var ollama = builder.AddContainer("ollama", "ollama/ollama")
    .WithBindMount("ollama", "/root/.ollama")
    .WithBindMount("./ollamaconfig", "/usr/config")
    .WithHttpEndpoint(port: 11434, targetPort: 11434, name: "ollama")
    .WithEntrypoint("/usr/config/entrypoint.sh")
    .WithContainerRuntimeArgs("--device", "nvidia.com/gpu=all");
```

For more information, see [GPU support in Podman](https://github.com/containers/podman/issues/19005).

---

The preceding code adds a container resource named "ollama" with the image `ollama/ollama`. The container resource is configured with multiple bind mounts, a named HTTP endpoint, an entrypoint that resolves to Unix shell script, and container run arguments with the <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithContainerRuntimeArgs%2A> method.

#### Customize container resources

All <xref:Aspire.Hosting.ApplicationModel.ContainerResource> subclasses can be customized to meet your specific requirements. This can be useful when using a [hosting integration](integrations-overview.md#hosting-integrations) that models a container resource, but requires modifications. When you have an `IResourceBuilder<ContainerResource>` you can chain calls to any of the available APIs to modify the container resource. .NET Aspire container resources typically point to pinned tags, but you might want to use the `latest` tag instead.

To help exemplify this, imagine a scenario where you're using the [.NET Aspire Redis integration](../caching/stackexchange-redis-integration.md). If the Redis integration relies on the `7.4` tag and you want to use the `latest` tag instead, you can chain a call to the <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithImageTag*> API:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
                   .WithImageTag("latest");

// Instead of using the "7.4" tag, the "cache" 
// container resource now uses the "latest" tag.
```

For more information and additional APIs available, see <xref:Aspire.Hosting.ContainerResourceBuilderExtensions#methods>.

#### Container resource lifecycle

When the app host is run, the <xref:Aspire.Hosting.ApplicationModel.ContainerResource> is used to determine what container image to create and start. Under the hood, .NET Aspire runs the container using the defined container image by delegating calls to the appropriate OCI-compliant container runtime, either Docker or Podman. The following commands are used:

#### [Docker](#tab/docker)

First, the container is created using the `docker container create` command. Then, the container is started using the `docker container start` command.

- [docker container create](https://docs.docker.com/reference/cli/docker/container/create/): Creates a new container from the specified image, without starting it.
- [docker container start](https://docs.docker.com/reference/cli/docker/container/start/): Start one or more stopped containers.

These commands are used instead of `docker run` to manage attached container networks, volumes, and ports. Calling these commands in this order allows any IP (network configuration) to already be present at initial startup.

#### [Podman](#tab/podman)

First, the container is created using the `podman container create` command. Then, the container is started using the `podman container start` command.

- [podman container create](https://docs.podman.io/en/latest/markdown/podman-create.1.html): Creates a writable container layer over the specified image and prepares it for running.
- [podman container start](https://docs.podman.io/en/latest/markdown/podman-start.1.html): Start one or more stopped containers.

These commands are used instead of `podman run` to manage attached container networks, volumes, and ports. Calling these commands in this order allows any IP (network configuration) to already be present at initial startup.

---

Beyond the base resource types, <xref:Aspire.Hosting.ApplicationModel.ProjectResource>, <xref:Aspire.Hosting.ApplicationModel.ContainerResource>, and <xref:Aspire.Hosting.ApplicationModel.ExecutableResource>, .NET Aspire provides extension methods to add common resources to your app model. For more information, see [Hosting integrations](integrations-overview.md#hosting-integrations).

#### Container resource lifetime

By default, container resources use the _session_ container lifetime. This means that every time the app host process is started, the container is created and started. When the app host stops, the container is stopped and removed. Container resources can opt-in to a _persistent_ lifetime to avoid unnecessary restarts and use persisted container state. To achieve this, chain a call the <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithLifetime*?displayProperty=nameWithType> API and pass <xref:Aspire.Hosting.ApplicationModel.ContainerLifetime.Persistent?displayProperty=nameWithType>:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var ollama = builder.AddContainer("ollama", "ollama/ollama")
    .WithLifetime(ContainerLifetime.Persistent);
```

The preceding code adds a container resource named "ollama" with the image "ollama/ollama" and a persistent lifetime.

### Connection string and endpoint references

It's common to express dependencies between project resources. Consider the following example code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var apiservice = builder.AddProject<Projects.AspireApp_ApiService>("apiservice");

builder.AddProject<Projects.AspireApp_Web>("webfrontend")
       .WithReference(cache)
       .WithReference(apiservice);
```

Project-to-project references are handled differently than resources that have well-defined connection strings. Instead of connection string being injected into the "webfrontend" resource, environment variables to support service discovery are injected.

| Method | Environment variable |
|--|--|
| `WithReference(cache)` | `ConnectionStrings__cache="localhost:62354"` |
| `WithReference(apiservice)` | `services__apiservice__http__0="http://localhost:5455"` <br /> `services__apiservice__https__0="https://localhost:7356"` |

Adding a reference to the "apiservice" project results in service discovery environment variables being added to the frontend. This is because typically, project-to-project communication occurs over HTTP/gRPC. For more information, see [.NET Aspire service discovery](../service-discovery/overview.md).

To get specific endpoints from a <xref:Aspire.Hosting.ApplicationModel.ContainerResource> or an <xref:Aspire.Hosting.ApplicationModel.ExecutableResource>, use one of the following endpoint APIs:

- <xref:Aspire.Hosting.ResourceBuilderExtensions.WithEndpoint*>
- <xref:Aspire.Hosting.ResourceBuilderExtensions.WithHttpEndpoint*>
- <xref:Aspire.Hosting.ResourceBuilderExtensions.WithHttpsEndpoint*>

Then call the <xref:Aspire.Hosting.ResourceBuilderExtensions.GetEndpoint*> API to get the endpoint which can be used to reference the endpoint in the `WithReference` method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var customContainer = builder.AddContainer("myapp", "mycustomcontainer")
                             .WithHttpEndpoint(port: 9043, name: "endpoint");

var endpoint = customContainer.GetEndpoint("endpoint");

var apiservice = builder.AddProject<Projects.AspireApp_ApiService>("apiservice")
                        .WithReference(endpoint);
```

| Method                    | Environment variable                                  |
|---------------------------|-------------------------------------------------------|
| `WithReference(endpoint)` | `services__myapp__endpoint__0=https://localhost:9043` |

The `port` parameter is the port that the container is listening on. For more information on container ports, see [Container ports](networking-overview.md#container-ports). For more information on service discovery, see [.NET Aspire service discovery](../service-discovery/overview.md).

### Service endpoint environment variable format

In the preceding section, the <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference*> method is used to express dependencies between resources. When service endpoints result in environment variables being injected into the dependent resource, the format might not be obvious. This section provides details on this format.

When one resource depends on another resource, the app host injects environment variables into the dependent resource. These environment variables configure the dependent resource to connect to the resource it depends on. The format of the environment variables is specific to .NET Aspire and expresses service endpoints in a way that is compatible with [Service Discovery](../service-discovery/overview.md).

Service endpoint environment variable names are prefixed with `services__` (double underscore), then the service name, the endpoint name, and finally the index. The index supports multiple endpoints for a single service, starting with `0` for the first endpoint and incrementing for each endpoint.

Consider the following environment variable examples:

```Environment
services__apiservice__http__0
```

The preceding environment variable expresses the first HTTP endpoint for the `apiservice` service. The value of the environment variable is the URL of the service endpoint. A named endpoint might be expressed as follows:

```Environment
services__apiservice__myendpoint__0
```

In the preceding example, the `apiservice` service has a named endpoint called `myendpoint`. The value of the environment variable is the URL of the service endpoint.

## Reference existing resources

Some situations warrant that you reference an existing resource, perhaps one that is deployed to a cloud provider. For example, you might want to reference an Azure database. In this case, you'd rely on the [Execution context](#execution-context) to dynamically determine whether the app host is running in "run" mode or "publish" mode. If you're running locally and want to rely on a cloud resource, you can use the `IsRunMode` property to conditionally add the reference. You might choose to instead create the resource in publish mode. Some [hosting integrations](integrations-overview.md#hosting-integrations) support providing a connection string directly, which can be used to reference an existing resource.

Likewise, there might be use cases where you want to integrate .NET Aspire into an existing solution. One common approach is to add the .NET Aspire app host project to an existing solution. Within your app host, you express dependencies by adding project references to the app host and [building out the app model](#define-the-app-model). For example, one project might depend on another. These dependencies are expressed using the <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method. For more information, see [Add .NET Aspire to an existing .NET app](../get-started/add-aspire-existing-app.md).

## App host life cycles

The .NET Aspire app host exposes several life cycles that you can hook into by implementing the <xref:Aspire.Hosting.Lifecycle.IDistributedApplicationLifecycleHook> interface. The following lifecycle methods are available:

| Order | Method | Description |
|--|--|--|
| **1** | <xref:Aspire.Hosting.Lifecycle.IDistributedApplicationLifecycleHook.BeforeStartAsync%2A> | Executes before the distributed application starts. |
| **2** | <xref:Aspire.Hosting.Lifecycle.IDistributedApplicationLifecycleHook.AfterEndpointsAllocatedAsync%2A> | Executes after the orchestrator allocates endpoints for resources in the application model. |
| **3** | <xref:Aspire.Hosting.Lifecycle.IDistributedApplicationLifecycleHook.AfterResourcesCreatedAsync%2A> | Executes after the resource was created by the orchestrator. |

While the app host provides life cycle hooks, you might want to register custom events. For more information, see [Eventing in .NET Aspire](../app-host/eventing.md).

### Register a life cycle hook

To register a life cycle hook, implement the <xref:Aspire.Hosting.Lifecycle.IDistributedApplicationLifecycleHook> interface and register the hook with the app host using the <xref:Aspire.Hosting.Lifecycle.LifecycleHookServiceCollectionExtensions.AddLifecycleHook*> API:

:::code source="snippets/lifecycles/AspireApp/AspireApp.AppHost/Program.cs":::

The preceding code:

- Implements the <xref:Aspire.Hosting.Lifecycle.IDistributedApplicationLifecycleHook> interface as a `LifecycleLogger`.
- Registers the life cycle hook with the app host using the <xref:Aspire.Hosting.Lifecycle.LifecycleHookServiceCollectionExtensions.AddLifecycleHook*> API.
- Logs a message for all the events.

When this app host is run, the life cycle hook is executed for each event. The following output is generated:

```Output
info: LifecycleLogger[0]
      BeforeStartAsync
info: Aspire.Hosting.DistributedApplication[0]
      Aspire version: 9.0.0
info: Aspire.Hosting.DistributedApplication[0]
      Distributed application starting.
info: Aspire.Hosting.DistributedApplication[0]
      Application host directory is: ..\AspireApp\AspireApp.AppHost
info: LifecycleLogger[0]
      AfterEndpointsAllocatedAsync
info: Aspire.Hosting.DistributedApplication[0]
      Now listening on: https://localhost:17043
info: Aspire.Hosting.DistributedApplication[0]
      Login to the dashboard at https://localhost:17043/login?t=d80f598bc8a64c7ee97328a1cbd55d72
info: LifecycleLogger[0]
      AfterResourcesCreatedAsync
info: Aspire.Hosting.DistributedApplication[0]
      Distributed application started. Press Ctrl+C to shut down.
```

The preferred way to hook into the app host life cycle is to use the eventing API. For more information, see [Eventing in .NET Aspire](../app-host/eventing.md).

## Execution context

The <xref:Aspire.Hosting.IDistributedApplicationBuilder> exposes an execution context (<xref:Aspire.Hosting.DistributedApplicationExecutionContext>), which provides information about the current execution of the app host. This context can be used to evaluate whether or not the app host is executing as "run" mode, or as part of a publish operation. Consider the following properties:

- <xref:Aspire.Hosting.DistributedApplicationExecutionContext.IsRunMode%2A>: Returns `true` if the current operation is running.
- <xref:Aspire.Hosting.DistributedApplicationExecutionContext.IsPublishMode%2A>: Returns `true` if the current operation is publishing.

This information can be useful when you want to conditionally execute code based on the current operation. Consider the following example that demonstrates using the `IsRunMode` property. In this case, an extension method is used to generate a stable node name for RabbitMQ for local development runs.

```csharp
private static IResourceBuilder<RabbitMQServerResource> RunWithStableNodeName(
    this IResourceBuilder<RabbitMQServerResource> builder)
{
    if (builder.ApplicationBuilder.ExecutionContext.IsRunMode)
    {
        builder.WithEnvironment(context =>
        {
            // Set a stable node name so queue storage is consistent between sessions
            var nodeName = $"{builder.Resource.Name}@localhost";
            context.EnvironmentVariables["RABBITMQ_NODENAME"] = nodeName;
        });
    }

    return builder;
}
```

The execution context is often used to conditionally add resources or connection strings that point to existing resources. Consider the following example that demonstrates conditionally adding Redis or a connection string based on the execution context:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.ExecutionContext.IsRunMode
    ? builder.AddRedis("redis")
    : builder.AddConnectionString("redis");

builder.AddProject<Projects.WebApplication>("api")
       .WithReference(redis);

builder.Build().Run();
```

In the preceding code:

- If the app host is running in "run" mode, a Redis container resource is added.
- If the app host is running in "publish" mode, a connection string is added.

This logic can easily be inverted to connect to an existing Redis resource when you're running locally, and create a new Redis resource when you're publishing.

> [!IMPORTANT]
> .NET Aspire provides common APIs to control the modality of resource builders, allowing resources to behave differently based on the execution mode. The fluent APIs are prefixed with `RunAs*` and `PublishAs*`. The `RunAs*` APIs influence the local development (or run mode) behavior, whereas the `PublishAs*` APIs influence the publishing of the resource. For more information on how Azure resources use these APIs, see [App host run modes](../azure/integrations-overview.md#app-host-run-modes).

## Resource relationships  

Resource relationships link resources together. Relationships are informational and don't impact an app's runtime behavior. Instead, they're used when displaying details about resources in the dashboard. For example, relationships are visible in the [dashboard's resource details](./dashboard/explore.md#resource-details), and `Parent` relationships control resource nesting on the resources page.

Relationships are automatically created by some app model APIs. For example:

- <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference*> adds a relationship to the target resource with the type `Reference`.
- <xref:Aspire.Hosting.ResourceBuilderExtensions.WaitFor*> adds a relationship to the target resource with the type `WaitFor`.
- Adding a database to a DB container creates a relationship from the database to the container with the type `Parent`.

Relationships can also be explicitly added to the app model using `WithRelationship` and `WithParentRelationship`.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var catalogDb = builder.AddPostgres("postgres")
                       .WithDataVolume()
                       .AddDatabase("catalogdb");

builder.AddProject<Projects.AspireApp_CatalogDbMigration>("migration")
       .WithReference(catalogDb)
       .WithParentRelationship(catalogDb);

builder.Build().Run();
```

The preceding example uses `WithParentRelationship` to configure `catalogdb` database as the `migration` project's parent. The `Parent` relationship is special because it controls resource nesting on the resource page. In this example, `migration` is nested under `catalogdb`.

> [!NOTE]
> There's validation for parent relationships to prevent a resource from having multiple parents or creating a circular reference. These configurations can't be rendered in the UI, and the app model will throw an error.

## See also

- [.NET Aspire integrations overview](integrations-overview.md)
- [.NET Aspire SDK](dotnet-aspire-sdk.md)
- [Eventing in .NET Aspire](../app-host/eventing.md)
- [Service discovery in .NET Aspire](../service-discovery/overview.md)
- [.NET Aspire service defaults](service-defaults.md)
- [Expressing external parameters](external-parameters.md)
- [.NET Aspire inner-loop networking overview](networking-overview.md)
