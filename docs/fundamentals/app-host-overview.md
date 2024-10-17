---
title: .NET Aspire orchestration overview
description: Learn the fundamental concepts of .NET Aspire orchestration and explore the various APIs for adding resources and expressing dependencies.
ms.date: 10/17/2024
ms.topic: overview
uid: aspire/app-host
---

# .NET Aspire orchestration overview

.NET Aspire provides APIs for expressing resources and dependencies within your distributed application. In addition to these APIs, [there's tooling](setup-tooling.md#install-net-aspire) that enables several compelling scenarios. The orchestrator is intended for _local development_ purposes and isn't supported in production environments.

<span id="terminology"></span>

Before continuing, consider some common terminology used in .NET Aspire:

- **App model**: A collection of resources that make up your distributed application (<xref:Aspire.Hosting.DistributedApplication>). For a more formal definition, see [Define the app model](#define-the-app-model).
- **App host/Orchestrator project**: The .NET project that orchestrates the _app model_, named with the _*.AppHost_ suffix (by convention).
- **Resource**: A [resource](#built-in-resource-types) is a dependent part of an application, such as a .NET project, container, executable, database, cache, or cloud service. It represents any part of the application that can be managed or referenced.
- **Integration**: An integration is a NuGet package for either the _app host_ that models a _resource_ or a package that configures a client for use in a consuming app. For more information, see [.NET Aspire integrations overview](integrations-overview.md).
- **Reference**: A reference defines a connection between resources, expressed as a dependency using the <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference*> API. For more information, see [Reference resources](#reference-resources).

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

The app host project handles running all of the projects that are part of the .NET Aspire project. In other words, it's responsible for orchestrating all apps within the app model. The following code describes an application with two projects and a Redis cache:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var apiservice = builder.AddProject<Projects.AspireApp_ApiService>("apiservice");

builder.AddProject<Projects.AspireApp_Web>("webfrontend")
       .WithReference(cache)
       .WithReference(apiservice);

builder.Build().Run();
```

To help visualize the relationship between the app host project and the resources it describes, consider the following diagram:

:::image type="content" source="../media/app-host-resource-diagram.png" lightbox="../media/app-host-resource-diagram.png" alt-text="The relationship between the projects in the .NET Aspire Starter Application template.":::

Each resource must be uniquely named. This diagram shows each resource and the relationships between them. The container resource is named "cache" and the project resources are named "apiservice" and "webfrontend". The web frontend project references the cache and API service projects. When you're expressing references in this way, the web frontend project is saying that it depends on these two resources, the "cache" and "apiservice" respectively.

## Built-in resource types

.NET Aspire projects are made up of a set of resources. The primary base resource types in the [Aspire.Hosting.AppHost](https://www.nuget.org/packages/Aspire.Hosting.AppHost) NuGet package are described in the following table:

| Method | Resource type | Description |
|--|--|--|
| <xref:Aspire.Hosting.ProjectResourceBuilderExtensions.AddProject%2A> | <xref:Aspire.Hosting.ApplicationModel.ProjectResource> | A .NET project, for example ASP.NET Core web apps. |
| <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.AddContainer%2A> | <xref:Aspire.Hosting.ApplicationModel.ContainerResource> | A container image, such as a Docker image. |
| <xref:Aspire.Hosting.ExecutableResourceBuilderExtensions.AddExecutable%2A> | <xref:Aspire.Hosting.ApplicationModel.ExecutableResource> | An executable file, such as a [Node.js app](../get-started/build-aspire-apps-with-nodejs.md). |
| <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddParameter%2A> | <xref:Aspire.Hosting.ApplicationModel.ParameterResource> | A parameter resource that can be used to [express external parameters](external-parameters.md). |

Project resources represent .NET projects that are part of the app model. When you add a project reference to the app host project, the .NET Aspire SDK generates a type in the `Projects` namespace for each referenced project.

To add a project to the app model, use the <xref:Aspire.Hosting.ProjectResourceBuilderExtensions.AddProject%2A> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Adds the project "apiservice" of type "Projects.AspireApp_ApiService".
var apiservice = builder.AddProject<Projects.AspireApp_ApiService>("apiservice");
```

## Reference resources

A reference represents a dependency between resources. Consider the following example app host `Program` C# code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

builder.AddProject<Projects.AspireApp_Web>("webfrontend")
       .WithReference(cache);
```

The "webfrontend" project resource uses <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> to add a dependency on the "cache" container resource. These dependencies can represent connection strings or [service discovery](../service-discovery/overview.md) information. In the preceding example, an environment variable is _injected_ into the "webfronend" resource with the name `ConnectionStrings__cache`. This environment variable contains a connection string that the `webfrontend` uses to connect to Redis via the [.NET Aspire Redis integration](../caching/stackexchange-redis-caching-overview.md), for example, `ConnectionStrings__cache="localhost:62354"`.

### APIs for adding and expressing resources

.NET Aspire [hosting integrations](integrations-overview.md#hosting-integrations) and [client integrations](integrations-overview.md#client-integrations) are both delivered as NuGet packages, but they serve different purposes. While _client integrations_ provide client library configuration for consuming apps outside the scope of the app host, _hosting integrations_ provide APIs for expressing resources and dependencies within the app host. For more information, see [Integration responsibilities](integrations-overview.md#integration-responsibilities).

### Express container resources

To express a <xref:Aspire.Hosting.ApplicationModel.ContainerResource>, use the <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.AddContainer%2A> method:

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

The preceding code adds a container resource named "ollama" with the image "ollama/ollama". The container resource is configured with multiple bind mounts, a named HTTP endpoint, an entrypoint that resolves to Unix shell script, and container run arguments with the <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithContainerRuntimeArgs%2A> method.

#### Container resource lifecycle

When the app host is run, the container resource is used to determine what container image to create and start. Under the hood, .NET Aspire runs the container using the defined container image by delegating calls to the appropriate OCI-compliant container runtime, either Docker or Podman. The following commands are used:

- `docker/podman container create`: Create the container.
- `docker/podman container start`: Start the container.

These commands are used instead of `docker/podman run` to manage attached container networks, volumes, and ports. Calling these commands in this order allows any IP (network configuration) to already be present at initial startup.

Beyond the base resource types, <xref:Aspire.Hosting.ApplicationModel.ProjectResource>, <xref:Aspire.Hosting.ApplicationModel.ContainerResource>, and <xref:Aspire.Hosting.ApplicationModel.ExecutableResource>, .NET Aspire provides extension methods to add common resources to your app model. For more information, see [Hosting integrations](integrations-overview.md#hosting-integrations).

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

It's possible to get specific endpoints from a container or executable using the <xref:Aspire.Hosting.ResourceBuilderExtensions.WithEndpoint%2A> and calling the <xref:Aspire.Hosting.ResourceBuilderExtensions.GetEndpoint%2A>:

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

In the preceding section, the `WithReference` method is used to express dependencies between resources. When service endpoints result in environment variables being injected into the dependent resource, the format might not be obvious. This section provides details on this format.

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

## See also

- [.NET Aspire integrations overview](integrations-overview.md)
- [.NET Aspire SDK](dotnet-aspire-sdk.md)
- [Service discovery in .NET Aspire](../service-discovery/overview.md)
- [.NET Aspire service defaults](service-defaults.md)
- [Expressing external parameters](external-parameters.md)
- [.NET Aspire inner-loop networking overview](networking-overview.md)
