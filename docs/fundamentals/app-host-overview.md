---
title: .NET Aspire orchestration overview
description: Learn the fundamental concepts of .NET Aspire orchestration and explore the various APIs to express resource references.
ms.date: 02/15/2024
ms.topic: overview
---

# .NET Aspire orchestration overview

.NET Aspire provides APIs for expressing resources and dependencies within your distributed application. In addition to these APIs, [there's tooling](setup-tooling.md#install-net-aspire) that enables some compelling scenarios.

Before continuing, consider some common terminology used in .NET Aspire:

- **App model**: A collection of resources that make up your distributed application (<xref:Aspire.Hosting.DistributedApplication>). For a more formal definition, see [Define the app model](#define-the-app-model).
- **App host/Orchestrator project**: The .NET project that orchestrates the _app model_, named with the _*.AppHost_ suffix (by convention).
- **Resource**: A [resource](#built-in-resource-types) represents a part of an application whether it be a .NET project, container, or executable, or some other resource like a database, cache, or cloud service (such as a storage service).
- **Reference**: A reference defines a connection between resources, expressed as a dependency. For more information, see [Reference resources](#reference-resources).

## Define the app model

.NET Aspire empowers you to seamlessly build, provision, deploy, configure, test, run, and observe your cloud application. This is achieved through the utilization of an _app model_ that outlines the resources in your app and their relationships. These resources encompass projects, executables, containers, as well as external services and cloud resources that your app depends on. Within every .NET Aspire app, there is a designated [App host project](#app-host-project), where the app model is precisely defined using methods available on the <xref:Aspire.Hosting.IDistributedApplicationBuilder>. This builder is obtained by invoking <xref:Aspire.Hosting.DistributedApplication.CreateBuilder%2A?displayProperty=nameWithType>.

## App host project

The app host project handles running all of the projects that are part of the .NET Aspire application. In other words, it's responsible for orchestrating all apps within the app model. The following code describes an application with two projects and a Redis cache:

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

Each resource must be uniquely named. This diagram shows each resource and the relationships between them. The container resource is named "cache" and the project resources are named "apiservice" and "webfrontend". The web frontend project references the cache and API service projects. By expressing a reference in this way, the web frontend project is saying that it depends on these two resources.

## Built-in resource types

.NET Aspire apps are made up of a set of resources. There are three base types of compute resources:

| Method | Resource type | Description |
|--|--|--|
| <xref:Aspire.Hosting.ProjectResourceBuilderExtensions.AddProject%2A> | <xref:Aspire.Hosting.ApplicationModel.ProjectResource> | A .NET project, for example ASP.NET Core web apps. |
| <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.AddContainer%2A> | <xref:Aspire.Hosting.ApplicationModel.ContainerResource> | A container image, such as a Docker image. |
| <xref:Aspire.Hosting.ExecutableResourceBuilderExtensions.AddExecutable%2A> | <xref:Aspire.Hosting.ApplicationModel.ExecutableResource> | An executable file. |

Project resources are .NET projects that are part of the app model. When you add a project reference to the app host project, the app host generates a type in the `Projects` namespace for each referenced project.

To add a project to the app model, use the <xref:Aspire.Hosting.ProjectResourceBuilderExtensions.AddProject%2A> method:

```csharp
// Adds the project "apiservice" of type "Projects.AspireApp_ApiService".
var apiservice = builder.AddProject<Projects.AspireApp_ApiService>("apiservice");
```

## Reference resources

A reference represents a dependency between resources. Consider the following:

```csharp
var cache = builder.AddRedis("cache");

builder.AddProject<Projects.AspireApp_Web>("webfrontend")
       .WithReference(cache);
```

The "webfrontend" project resource uses <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> to add a dependency on the "cache" container resource. These dependencies can represent connection strings or service discovery information. In the preceding example, an environment variable is injected into the "webfronend" resource with the name `ConnectionStrings__cache`. This environment variable contains a connection string that the webfrontend can use to connect to redis via the .NET Aspire Redis component, for example, `ConnectionStrings__cache="localhost:6379"`.

### Connection string and endpoint references

It's also possible to have dependencies between project resources. Consider the following example code:

```csharp
var cache = builder.AddRedis("cache");

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

Adding a reference to the "apiservice" project results in service discovery environment variables being added to the front-end. This is because typically, project to project communication occurs over HTTP/gRPC. For more information, see [.NET Aspire service discovery](../service-discovery/overview.md).

It's possible to get specific endpoints from a container or executable using the <xref:Aspire.Hosting.ResourceBuilderExtensions.WithEndpoint%2A> and calling the <xref:Aspire.Hosting.ApplicationModel.IResourceWithEndpoints.GetEndpoint%2A>:

```csharp
var customContainer = builder.AddContainer("myapp", "mycustomcontainer")
                             .WithEndpoint(containerPort: 9043, name: "endpoint");

var endpoint = customContainer.GetEndpoint("endpoint");

var apiservice = builder.AddProject<Projects.AspireApp_ApiService>("apiservice")
                        .WithReference(endpoint);
```

| Method                    | Environment variable                             |
|---------------------------|--------------------------------------------------|
| `WithReference(endpoint)` | `services__myapp__0=http://_http.localhost:8034` |

### APIs for adding and expressing resources

Beyond the base resource types, <xref:Aspire.Hosting.ApplicationModel.ProjectResource>, <xref:Aspire.Hosting.ApplicationModel.ContainerResource>, and <xref:Aspire.Hosting.ApplicationModel.ExecutableResource>, .NET Aspire provides extension methods to add common resources to your app model. The following table lists the methods and their corresponding resource types:

**Cloud-agnostic resources available in the [📦 Aspire.Hosting](https://www.nuget.org/packages/Aspire.Hosting) NuGet package (available by default in .NET Aspire templates with the AppHost project):**

| Method | Resource type | Description |
|--|--|--|
| `AddMongoDB(...)` | `MongoDBServerResource` | Adds a MongoDB server resource. |
| `AddMongoDBContainer(...).AddDatabase` | `MongoDBDatabaseResource` | Adds a MongoDB database resource. |
| `AddMongoDBContainer(...)` | `MongoDBContainerResource` | Adds a MongoDB container resource. |
| `AddMySql(...)` | `MySqlServerResource` | Adds a MySql server resource. |
| `AddMySqlContainer(...).AddDatabase` | `MySqlDatabaseResource` | Adds a MySql database resource. |
| `AddMySqlContainer(...)` | `MySqlContainerResource` | Adds a MySql container resource. |
| `AddNodeApp(...)` | `NodeAppResource` | Adds a Node.js app resource. |
| `AddNpmApp(...)` | `NodeAppResource` | Adds a Node.js app resource that wraps an [NPM](https://www.npmjs.com/) package. |
| `AddPostgresContainer(...).`<xref:Aspire.Hosting.PostgresBuilderExtensions.AddDatabase%2A> | <xref:Aspire.Hosting.ApplicationModel.PostgresDatabaseResource> | Adds a Postgres database resource. |
| `AddSqlServerContainer(...).`<xref:Aspire.Hosting.SqlServerBuilderExtensions.AddDatabase%2A> | <xref:Aspire.Hosting.ApplicationModel.SqlServerDatabaseResource> | Adds a SQL Server database resource. |
| <xref:Aspire.Hosting.PostgresBuilderExtensions.AddPostgres%2A> | <xref:Aspire.Hosting.ApplicationModel.PostgresServerResource> | Adds a Postgres server resource. |
| <xref:Aspire.Hosting.PostgresBuilderExtensions.AddPostgresContainer%2A> | <xref:Aspire.Hosting.ApplicationModel.PostgresContainerResource> | Adds a Postgres container resource. |
| <xref:Aspire.Hosting.RabbitMQBuilderExtensions.AddRabbitMQ%2A> | <xref:Aspire.Hosting.ApplicationModel.RabbitMQServerResource> | Adds a RabbitMQ server resource. |
| <xref:Aspire.Hosting.RabbitMQBuilderExtensions.AddRabbitMQContainer%2A> | <xref:Aspire.Hosting.ApplicationModel.RabbitMQContainerResource> | Adds a RabbitMQ container resource. |
| <xref:Aspire.Hosting.RedisBuilderExtensions.AddRedisContainer%2A> | <xref:Aspire.Hosting.ApplicationModel.RedisContainerResource> | Adds a Redis container resource. |
| <xref:Aspire.Hosting.SqlServerBuilderExtensions.AddSqlServer%2A> | <xref:Aspire.Hosting.ApplicationModel.SqlServerServerResource> | Adds a SQL Server server resource. |
| <xref:Aspire.Hosting.SqlServerBuilderExtensions.AddSqlServerContainer%2A> | <xref:Aspire.Hosting.ApplicationModel.SqlServerContainerResource> | Adds a SQL Server container resource. |

**Azure specific resources available in the [📦 Aspire.Hosting.Azure](https://www.nuget.org/packages/Aspire.Hosting.Azure) NuGet package:**

| Method | Resource type | Description |
|--|--|--|
| <xref:Aspire.Hosting.AzureResourceExtensions.AddAzureStorage%2A> | <xref:Aspire.Hosting.ApplicationModel.AzureStorageResource> | Adds an Azure Storage resource. |
| `AddAzureStorage(...).`<xref:Aspire.Hosting.AzureResourceExtensions.AddBlobs%2A> | <xref:Aspire.Hosting.ApplicationModel.AzureBlobStorageResource> | Adds an Azure Blob Storage resource. |
| `AddAzureStorage(...).`<xref:Aspire.Hosting.AzureResourceExtensions.AddQueues%2A> | <xref:Aspire.Hosting.ApplicationModel.AzureQueueStorageResource> | Adds an Azure Queue Storage resource. |
| `AddAzureStorage(...).`<xref:Aspire.Hosting.AzureResourceExtensions.AddTables%2A> | <xref:Aspire.Hosting.ApplicationModel.AzureTableStorageResource> | Adds an Azure Table Storage resource. |
| <xref:Aspire.Hosting.AzureCosmosDBCloudApplicationBuilderExtensions.AddAzureCosmosDB%2A> | <xref:Aspire.Hosting.Azure.Data.Cosmos.AzureCosmosDBResource> | Adds an Azure Cosmos DB resource. |
| <xref:Aspire.Hosting.AzureResourceExtensions.AddAzureKeyVault%2A> | <xref:Aspire.Hosting.ApplicationModel.AzureKeyVaultResource> | Adds an Azure Key Vault resource. |
| <xref:Aspire.Hosting.AzureResourceExtensions.AddAzureRedis%2A> | <xref:Aspire.Hosting.ApplicationModel.AzureRedisResource> | Adds an Azure Redis resource. |
| <xref:Aspire.Hosting.AzureResourceExtensions.AddAzureServiceBus%2A> | <xref:Aspire.Hosting.ApplicationModel.AzureServiceBusResource> | Adds an Azure Service Bus resource. |

## See also

- [.NET Aspire components overview](components-overview.md)
- [Service discovery in .NET Aspire](../service-discovery/overview.md)
- [.NET Aspire service defaults](service-defaults.md)
