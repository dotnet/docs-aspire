---
title: .NET Aspire orchestration overview
description: Learn the fundamental concepts of .NET Aspire orchestration and explore the various APIs to express resource references.
ms.date: 11/13/2023
ms.topic: overview
ms.prod: dotnet
---

# .NET Aspire orchestration overview

.NET Aspire provides APIs for expressing resources and dependencies within your distributed application. In addition to these APIs, [there's tooling](setup-tooling.md#install-net-aspire) that enables some compelling scenarios.

Before continuing, consider some common terminology used in .NET Aspire:

- **App model**: A collection of resources that make up your distributed application (`DistributedApplication`). For a more formal definition, see [App model](#app-model).
- **App host/Orchestrator project**: The .NET project that orchestrates the _app model_, named with the _*.AppHost_ suffix (by convention).
- **Resource**: A [resource](#built-in-resource-types) represents a part of an application whether it be a .NET project, container, or executable, or some other resource like a database, cache, or cloud service (such as a storage service).
- **Reference**: A reference defines a connection between resources, expressed as a dependency. For more information, see [Reference resources](#reference-resources).

## App model

.NET Aspire empowers you to seamlessly build, provision, deploy, configure, test, run, and observe your cloud application. This is achieved through the utilization of an _app model_ that outlines the resources in your app and their relationships. These resources encompass projects, executables, containers, as well as external services and cloud resources that your app depends on. Within every .NET Aspire app, there is a designated [AppHost project](#orchestration-project), where the app model is precisely defined using methods available on the `IDistributedApplicationBuilder`. This builder is obtained by invoking `DistributedApplication.CreateBuilder(args)`.

## App host project

The app host project handles running all of the projects that are part of the .NET Aspire application. In other words, it's responsible for orchestrating all apps within the app model. The following code describes an application with two projects and a Redis cache:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedisContainer("cache");

var apiservice = builder.AddProject<Projects.AspireApp_ApiService>("apiservice");

builder.AddProject<Projects.AspireApp_Web>("webfrontend")
    .WithReference(cache)
    .WithReference(apiservice);

builder.Build().Run();
```

The help visualize the relationship between the app host project and the resources it describes, consider the following diagram:

:::image type="content" source="media/app-host-resource-diagram.png" lightbox="media/app-host-resource-diagram.png" alt-text="The relationship between the projects in the .NET Aspire Starter Application template.":::

Each resource must be uniquely named. This diagram shows each resource and the relationships between them. The container resource is named "cache" and the project resources are named "apiservice" and "webfrontend". The web frontend project references the cache and API service projects. By expressing a reference in this way, the web frontend project is saying that it depends on these two resources.

## Built-in resource types

.NET Aspire apps are made up of a set of resources. There are three base types of compute resources:

| Method | Resource type | Description |
|--|--|--|
| `AddProject` | `ProjectResource` | A .NET project, for example ASP.NET Core web apps. |
| `AddContainer` | `ContainerResource` | A container image, such as a Docker image. |
| `AddExecutable` | `ExecutableResource` | An executable file. |

Project resources are .NET projects that are part of the app model. When you add a project reference to the app host project, the app host generates a type in the `Projects` namespace for each referenced project.

To add a project to the app model, use the `AddProject` method:

```csharp
// Adds the project "apiservice" of type "Projects.AspireApp_ApiService".
var apiservice = builder.AddProject<Projects.AspireApp_ApiService>("apiservice");
```

## Reference resources

A reference represents a dependency between resources. Consider the following:

```csharp
var cache = builder.AddRedisContainer("cache");

builder.AddProject<Projects.AspireApp_Web>("webfrontend")
    .WithReference(cache);
```

The "webfrontend" project resource uses `WithReference` to add a dependency on the "cache" container resource. These dependencies can represent connection strings or service discovery information. In the preceding example, an environment variable is injected into the "webfronend" resource with the name `ConnectionStrings__cache`. This environment variable contains a connection string that the webfrontend can use to connect to redis via the .NET Aspire Redis component, for example, `ConnectionStrings__cache="localhost:6379"`.

### Connection string and endpoint references

It's also possible to have dependencies between project resources. Consider the following example code:

```csharp
var cache = builder.AddRedisContainer("cache");

var apiservice = builder.AddProject<Projects.AspireApp_ApiService>("apiservice");

builder.AddProject<Projects.AspireApp_Web>("webfrontend")
    .WithReference(cache)
    .WithReference(apiservice);
```

Project-to-project references are handled differently than resources that have well defined connection strings. Instead of connection string being injected into the "webfrontend" resource, environment variables to support service discovery are injected.

| Method | Environment variable |
|--|--|
| `WithReference(cache)` | `ConnectionStrings__cache="localhost:6379"` |
| `WithReference(apiservice)` | `services__apiservice__0="http://_http.localhost:8034"` <br /> `services__apiservice__1="http://localhost:8034"` |

Adding a reference to the "apiservice" project results in service discovery environment variables being added to the front-end. This is because typically, project to project communication occurs over HTTP/gRPC. For more information, see [.NET Aspire service discovery](service-discovery/overview.md).

It's possible to get specific endpoints from a container or executable using the `GetEndpoint` method:

```csharp
var customContainer = builder.AddContainer("myapp", "mycustomcontainer")
                             .WithServiceBinding(containerPort: 9043, name: "endpoint");

var endpoint = customContainer.GetEndpoint("endpoint");

var apiservice = builder.AddProject<Projects.AspireApp_ApiService>("apiservice")
                        .WithReference(endpoint);
```

| Method | Environment variable |
|--|--|
| `WithReference(endpoint)` | `services__myapp__0=http://_http.localhost:8034` |

### APIs for adding and expressing resources

Beyond the base resource types, `ProjectResource`, `ContainerResource`, and `ExecutableResource`, .NET Aspire provides extension methods to add common resources to your app model. The following table lists the methods and their corresponding resource types:

**Cloud-agnostic resources available in the [📦 Aspire.Hosting](https://www.nuget.org/packages/Aspire.Hosting) NuGet package (available by default in .NET Aspire templates with the AppHost project):**

| Method | Resource type | Description |
|--|--|--|
| `AddPostgresConnection` | `PostgresConnectionResource` | Adds a Postgres connection resource. |
| `AddPostgresContainer` | `PostgresContainerResource` | Adds a Postgres container resource. |
| `AddPostgresContainer(...).AddDatabase` | `PostgresDatabaseResource` | Adds a Postgres database resource. |
| `AddRabbitMQConnection` | `RabbitMQConnectionResource` | Adds a RabbitMQ connection resource. |
| `AddRabbitMQContainer` | `RabbitMQContainerResource` | Adds a RabbitMQ container resource. |
| `AddRedisContainer` | `RedisContainerResource` | Adds a Redis container resource. |
| `AddSqlServerConnection` | `SqlServerConnectionResource` | Adds a SQL Server connection resource. |
| `AddSqlServerContainer` | `SqlServerContainerResource` | Adds a SQL Server container resource. |
| `AddSqlServerContainer(...).AddDatabase` | `SqlServerDatabaseResource` | Adds a SQL Server database resource. |

**Azure specific resources available in the [📦 Aspire.Hosting.Azure](https://www.nuget.org/packages/Aspire.Hosting.Azure) NuGet package:**

| Method | Resource type | Description |
|--|--|--|
| `AddAzureStorage` | `AzureStorageResource` | Adds an Azure Storage resource. |
| `AddAzureStorage(...).AddBlobs` | `AzureBlobStorageResource` | Adds an Azure Blob Storage resource. |
| `AddAzureStorage(...).AddQueues` | `AzureQueueStorageResource` | Adds an Azure Queue Storage resource. |
| `AddAzureStorage(...).AddTables` | `AzureTableStorageResource` | Adds an Azure Table Storage resource. |
| `AddAzureCosmosDB` | `AzureCosmosDBResource` | Adds an Azure Cosmos DB resource. |
| `AddAzureKeyVault` | `AzureKeyVaultResource` | Adds an Azure Key Vault resource. |
| `AddAzureRedisResource` | `AzureRedisResource` | Adds an Azure Redis resource. |
| `AddAzureServiceBus` | `AzureServiceBusResource` | Adds an Azure Service Bus resource. |

## See also

- [.NET Aspire components overview](components-overview.md)
- [Service discovery in .NET Aspire](service-discovery/overview.md)
- [.NET Aspire service defaults](service-defaults.md)
