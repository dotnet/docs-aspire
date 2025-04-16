---
title: Orchestrate resources in .NET Aspire
description: Learn techniques to control the behavior of .NET Aspire resources such as project, containers, and executable resources.
ms.date: 04/16/2025
uid: dotnet/aspire/orchestrate-resources
---

# Orchestrate resources in .NET Aspire

In .NET Aspire, a **resource** is a dependant part of a cloud-native application. Resource types include:

- **.NET Project**: A custom microservice, responsible for specific functionality in your cloud-native application, and often built by a separate team of developers.
- **Executable**: If you need to build microservices with tools like Node.js or Orleans, they run as executable resources.
- **Container**: You can add Docker containers, based on specific images to your .NET Aspire solution.
- **Integration resources**: Integrations often add resources such as databases, caches, and messaging services to your application.

> [!NOTE]
> For the fundamentals of .NET Aspire orchestration and how it manages resources, see [.NET Aspire orchestration overview](app-host-overview.md). In this article, you'll learn how to customize the behavior of resources further by writing code in the app host project.

## Configure explicit resource start

Project, executable, and container resources are automatically started with your distributed application by default. A resource can be configured to wait for an explicit startup instruction with the <xref:Aspire.Hosting.ResourceBuilderExtensions.WithExplicitStart*> method. A resource configured with <xref:Aspire.Hosting.ResourceBuilderExtensions.WithExplicitStart*> is initialized with <xref:Aspire.Hosting.ApplicationModel.KnownResourceStates.NotStarted?displayProperty=nameWithType>.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var postgresdb = postgres.AddDatabase("postgresdb");

builder.AddProject<Projects.AspireApp_DbMigration>("dbmigration")
       .WithReference(postgresdb)
       .WithExplicitStart();
```

In the preceeding code the "dbmigration" resource is configured to not automatically start with the distributed application.

Resources with explicit start can be started from the .NET Aspire dashboard by clicking the "Start" command. For more information, see [.NET Aspire dashboard: Stop or Start a resource](dashboard/explore.md#stop-or-start-a-resource).

## Waiting for resources

In some cases, you might want to wait for a resource to be ready before starting another resource. For example, you might want to wait for a database to be ready before starting an API that depends on it. To express this dependency, use the <xref:Aspire.Hosting.ResourceBuilderExtensions.WaitFor*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var postgresdb = postgres.AddDatabase("postgresdb");

builder.AddProject<Projects.AspireApp_ApiService>("apiservice")
       .WithReference(postgresdb)
       .WaitFor(postgresdb);
```

In the preceding code, the "apiservice" project resource waits for the "postgresdb" database resource to enter the <xref:Aspire.Hosting.ApplicationModel.KnownResourceStates.Running?displayProperty=nameWithType> state. The example code shows the [.NET Aspire PostgreSQL integration](../database/postgresql-integration.md), but the same pattern can be applied to other resources.

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

In the preceding code, the "apiservice" project resource waits for the "migration" project resource to run to completion before starting. The "migration" project resource waits for the "postgresdb" database resource to enter the <xref:Aspire.Hosting.ApplicationModel.KnownResourceStates.Running?displayProperty=nameWithType> state. This can be useful in scenarios where you want to run a database migration before starting the API service, for example.

### Forcing resource start in the dashboard

Waiting for a resource can be bypassed using the "Start" command in the dashboard. Clicking "Start" on a waiting resource in the dashboard instructs it to start immediately without waiting for the resource to be healthy or completed. This can be useful when you want to test a resource immediately and don't want to wait for the app to be in the right state.

## APIs for adding and expressing resources

.NET Aspire [hosting integrations](integrations-overview.md#hosting-integrations) and [client integrations](integrations-overview.md#client-integrations) are both delivered as NuGet packages, but they serve different purposes. While _client integrations_ provide client library configuration for consuming apps outside the scope of the app host, _hosting integrations_ provide APIs for expressing resources and dependencies within the app host. For more information, see [.NET Aspire integrations overview: Integration responsibilities](integrations-overview.md#integration-responsibilities).

## Express container resources

To express a <xref:Aspire.Hosting.ApplicationModel.ContainerResource> you add it to an <xref:Aspire.Hosting.IDistributedApplicationBuilder> instance by calling the <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.AddContainer%2A> method:

### [Docker](#tab/docker)

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

### [Podman](#tab/podman)

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

### Customize container resources

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

### Container resource lifecycle

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

### Container resource lifetime

By default, container resources use the _session_ container lifetime. This means that every time the app host process is started, the container is created and started. When the app host stops, the container is stopped and removed. Container resources can opt-in to a _persistent_ lifetime to avoid unnecessary restarts and use persisted container state. To achieve this, chain a call the <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithLifetime*?displayProperty=nameWithType> API and pass <xref:Aspire.Hosting.ApplicationModel.ContainerLifetime.Persistent?displayProperty=nameWithType>:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var ollama = builder.AddContainer("ollama", "ollama/ollama")
    .WithLifetime(ContainerLifetime.Persistent);
```

The preceding code adds a container resource named "ollama" with the image "ollama/ollama" and a persistent lifetime.

## Resource relationships

Resource relationships link resources together. Relationships are informational and don't impact an app's runtime behavior. Instead, they're used when displaying details about resources in the dashboard. For example, relationships are visible in the [dashboard's resource details](./dashboard/explore.md#resource-details), and `Parent` relationships control resource nesting on the resources page.

Relationships are automatically created by some app model APIs. For example:

- <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference*> adds a relationship to the target resource with the type `Reference`.
- <xref:Aspire.Hosting.ResourceBuilderExtensions.WaitFor*> adds a relationship to the target resource with the type `WaitFor`.
- Adding a database to a DB container creates a relationship from the database to the container with the type `Parent`.

Relationships can also be explicitly added to the app model using <xref:Aspire.Hosting.ResourceBuilderExtensions.WithRelationship*> and <xref:Aspire.Hosting.ResourceBuilderExtensions.WithParentRelationship*>.

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

The preceding example uses <xref:Aspire.Hosting.ResourceBuilderExtensions.WithParentRelationship*> to configure `catalogdb` database as the `migration` project's parent. The `Parent` relationship is special because it controls resource nesting on the resource page. In this example, `migration` is nested under `catalogdb`.

> [!NOTE]
> There's validation for parent relationships to prevent a resource from having multiple parents or creating a circular reference. These configurations can't be rendered in the UI, and the app model will throw an error.

## See also

- [.NET Aspire orchestration overview](app-host-overview.md)
- [Eventing in .NET Aspire](../app-host/eventing.md)
