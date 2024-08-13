---
title: .NET Aspire orchestration overview
description: Learn the fundamental concepts of .NET Aspire orchestration and explore the various APIs to express resource references.
ms.date: 08/12/2024
ms.topic: overview
uid: aspire/app-host
---

# .NET Aspire orchestration overview

.NET Aspire provides APIs for expressing resources and dependencies within your distributed application. In addition to these APIs, [there's tooling](setup-tooling.md#install-net-aspire) that enables some compelling scenarios. The orchestrator is intended for local development purposes.

Before continuing, consider some common terminology used in .NET Aspire:

- **App model**: A collection of resources that make up your distributed application (<xref:Aspire.Hosting.DistributedApplication>). For a more formal definition, see [Define the app model](#define-the-app-model).
- **App host/Orchestrator project**: The .NET project that orchestrates the _app model_, named with the _*.AppHost_ suffix (by convention).
- **Resource**: A [resource](#built-in-resource-types) represents a part of an application whether it be a .NET project, container, or executable, or some other resource like a database, cache, or cloud service (such as a storage service).
- **Integration**: An integration is a NuGet package for either the _app host_ that models a _resource_ or a package that configures a client for use in a consuming app.
- **Reference**: A reference defines a connection between resources, expressed as a dependency using the `WithReference` API. For more information, see [Reference resources](#reference-resources).

> [!NOTE]
> .NET Aspire's orchestration is designed to enhance your local development experience by simplifying the management of your cloud-native app's configuration and interconnections. While it's an invaluable tool for development, it's not intended to replace production environment systems like [Kubernetes](../deployment/overview.md#deploy-to-kubernetes), which are specifically designed to excel in that context.

## Define the app model

.NET Aspire empowers you to seamlessly build, provision, deploy, configure, test, run, and observe your cloud application. This is achieved through the utilization of an _app model_ that outlines the resources in your app and their relationships. These resources encompass projects, executables, containers, as well as external services and cloud resources that your app depends on. Within every .NET Aspire project, there is a designated [App host project](#app-host-project), where the app model is precisely defined using methods available on the <xref:Aspire.Hosting.IDistributedApplicationBuilder>. This builder is obtained by invoking <xref:Aspire.Hosting.DistributedApplication.CreateBuilder%2A?displayProperty=nameWithType>.

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

Each resource must be uniquely named. This diagram shows each resource and the relationships between them. The container resource is named "cache" and the project resources are named "apiservice" and "webfrontend". The web frontend project references the cache and API service projects. By expressing a reference in this way, the web frontend project is saying that it depends on these two resources.

## Built-in resource types

.NET Aspire projects are made up of a set of resources. There are three base types of compute resources from the primary [Aspire.Hosting.AppHost](https://www.nuget.org/packages/Aspire.Hosting.AppHost) Nuget package:

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

The "webfrontend" project resource uses <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> to add a dependency on the "cache" container resource. These dependencies can represent connection strings or [service discovery](../service-discovery/overview.md) information. In the preceding example, an environment variable is injected into the "webfronend" resource with the name `ConnectionStrings__cache`. This environment variable contains a connection string that the webfrontend can use to connect to redis via the .NET Aspire Redis integration, for example, `ConnectionStrings__cache="localhost:62354"`.

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
| `WithReference(cache)` | `ConnectionStrings__cache="localhost:62354"` |
| `WithReference(apiservice)` | `services__apiservice__http__0="http://localhost:5455"` <br /> `services__apiservice__https__0="https://localhost:7356"` |

Adding a reference to the "apiservice" project results in service discovery environment variables being added to the front-end. This is because typically, project to project communication occurs over HTTP/gRPC. For more information, see [.NET Aspire service discovery](../service-discovery/overview.md).

It's possible to get specific endpoints from a container or executable using the <xref:Aspire.Hosting.ResourceBuilderExtensions.WithEndpoint%2A> and calling the <xref:Aspire.Hosting.ResourceBuilderExtensions.GetEndpoint%2A>:

```csharp
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

In the preceding section, the `WithReference` method is used to express dependencies between resources. When service endpoints result in environment variables being injected into the dependent resource, the format may not be obvious. This section provides details on this format.

When one resource depends on another resource, the app host injects environment variables into the dependent resource. These environment variables configure the dependent resource to connect to the resource it depends on. The format of the environment variables is specific to .NET Aspire and expresses service endpoints in a way that is compatible with [Service Discovery](../service-discovery/overview.md).

Service endpoint environment variables start with `services` and are delimited by a double underscore `__`. They're prefixed with `services__`, followed by the service name, the service endpoint name or protocol, and the index. The index supports multiple endpoints for a single service, starting with `0` for the first endpoint and incrementing for each additional endpoint.

Consider the following environment variable examples:

```
services__apiservice__http__0
```

The preceding environment variable expresses the first HTTP endpoint for the `apiservice` service. The value of the environment variable is the URL of the service endpoint. A named endpoint might be expressed as follows:

```
services__apiservice__myendpoint__0
```

In the preceding example, the `apiservice` service has a named endpoint called `myendpoint`. The value of the environment variable is the URL of the service endpoint.

### APIs for adding and expressing resources

.NET Aspire hosting packages and [.NET Aspire integrations](integrations-overview.md) are both delivered as NuGet packages, but they serve different purposes. While integrations provide client library configuration for consuming apps outside the scope of the app host, hosting packages provide APIs for expressing resources and dependencies within the app host.

### Express container resources

To express a container resource, use the <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.AddContainer%2A> method:

## [Docker](#tab/docker)

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

## [Podman](#tab/podman)

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

Beyond the base resource types, <xref:Aspire.Hosting.ApplicationModel.ProjectResource>, <xref:Aspire.Hosting.ApplicationModel.ContainerResource>, and <xref:Aspire.Hosting.ApplicationModel.ExecutableResource>, .NET Aspire provides extension methods to add common resources to your app model. The following table lists the methods and their corresponding resource types:

**Cloud-agnostic resources are available in the following NuGet packages:**

- [📦 Aspire.Hosting.Dapr](https://www.nuget.org/packages/Aspire.Hosting.Dapr)
- [📦 Aspire.Hosting.Elasticsearch](https://www.nuget.org/packages/Aspire.Hosting.Elasticsearch)
- [📦 Aspire.Hosting.Kafka](https://www.nuget.org/packages/Aspire.Hosting.Kafka)
- [📦 Aspire.Hosting.Keycloak](https://www.nuget.org/packages/Aspire.Hosting.Keycloak)
- [📦 Aspire.Hosting.Milvus](https://www.nuget.org/packages/Aspire.Hosting.Milvus)
- [📦 Aspire.Hosting.MongoDB](https://www.nuget.org/packages/Aspire.Hosting.MongoDB)
- [📦 Aspire.Hosting.MySql](https://www.nuget.org/packages/Aspire.Hosting.MySql)
- [📦 Aspire.Hosting.Nats](https://www.nuget.org/packages/Aspire.Hosting.Nats)
- [📦 Aspire.Hosting.NodeJs](https://www.nuget.org/packages/Aspire.Hosting.NodeJs)
- [📦 Aspire.Hosting.Oracle](https://www.nuget.org/packages/Aspire.Hosting.Oracle)
- [📦 Aspire.Hosting.Orleans](https://www.nuget.org/packages/Aspire.Hosting.Orleans)
- [📦 Aspire.Hosting.PostgreSQL](https://www.nuget.org/packages/Aspire.Hosting.PostgreSQL)
- [📦 Aspire.Hosting.Python](https://www.nuget.org/packages/Aspire.Hosting.Python)
- [📦 Aspire.Hosting.Qdrant](https://www.nuget.org/packages/Aspire.Hosting.Qdrant)
- [📦 Aspire.Hosting.RabbitMQ](https://www.nuget.org/packages/Aspire.Hosting.RabbitMQ)
- [📦 Aspire.Hosting.Redis](https://www.nuget.org/packages/Aspire.Hosting.Redis)
- [📦 Aspire.Hosting.Seq](https://www.nuget.org/packages/Aspire.Hosting.Seq)
- [📦 Aspire.Hosting.SqlServer](https://www.nuget.org/packages/Aspire.Hosting.SqlServer)
- [📦 Aspire.Hosting.Testing](https://www.nuget.org/packages/Aspire.Hosting.Testing)

| Method | Resource type | Description |
|--|--|--|
| <xref:Aspire.Hosting.ElasticsearchBuilderExtensions.AddElasticsearch*> | <xref:Aspire.Hosting.ApplicationModel.ElasticsearchResource> | Adds an Elasticsearch container resource to the application model. |
| <xref:Aspire.Hosting.KeycloakResourceBuilderExtensions.AddKeycloak*> | <xref:Aspire.Hosting.ApplicationModel.KeycloakResource> | Adds a Keycloak container to the application model. |
| <xref:Aspire.Hosting.MilvusBuilderExtensions.AddMilvus*> | <xref:Aspire.Hosting.Milvus.MilvusServerResource> | Adds a Milvus resource to the application. |
| <xref:Aspire.Hosting.MongoDBBuilderExtensions.AddMongoDB*> | <xref:Aspire.Hosting.ApplicationModel.MongoDBServerResource> | Adds a MongoDB server resource. |
| <xref:Aspire.Hosting.MySqlBuilderExtensions.AddMySql*> | <xref:Aspire.Hosting.ApplicationModel.MySqlServerResource> | Adds a MySql server resource. |
| <xref:Aspire.Hosting.NodeAppHostingExtension.AddNodeApp*> | <xref:Aspire.Hosting.NodeAppResource> | Adds a Node.js app resource. |
| <xref:Aspire.Hosting.NodeAppHostingExtension.AddNpmApp*> | <xref:Aspire.Hosting.NodeAppResource> | Adds a Node.js app resource that wraps an [NPM](https://www.npmjs.com/) package. |
| <xref:Aspire.Hosting.PostgresBuilderExtensions.AddPostgres%2A> | <xref:Aspire.Hosting.ApplicationModel.PostgresServerResource> | Adds a Postgres server resource. |
| `AddPostgres(...).`<xref:Aspire.Hosting.PostgresBuilderExtensions.AddDatabase%2A> | <xref:Aspire.Hosting.ApplicationModel.PostgresDatabaseResource> | Adds a Postgres database resource. |
| <xref:Aspire.Hosting.PythonProjectResourceBuilderExtensions.AddPythonProject*> | <xref:Aspire.Hosting.Python.PythonProjectResource> | Adds a python application with a virtual environment to the application model. |
| <xref:Aspire.Hosting.QdrantBuilderExtensions.AddQdrant*> | <xref:Aspire.Hosting.ApplicationModel.QdrantServerResource> | Adds a Qdrant server resource. |
| <xref:Aspire.Hosting.RabbitMQBuilderExtensions.AddRabbitMQ%2A> | <xref:Aspire.Hosting.ApplicationModel.RabbitMQServerResource> | Adds a RabbitMQ server resource. |
| <xref:Aspire.Hosting.RedisBuilderExtensions.AddRedis%2A> | <xref:Aspire.Hosting.ApplicationModel.RedisResource> | Adds a Redis container resource. |
| <xref:Aspire.Hosting.SqlServerBuilderExtensions.AddSqlServer%2A> | <xref:Aspire.Hosting.ApplicationModel.SqlServerServerResource> | Adds a SQL Server server resource. |
| `AddSqlServer(...).`<xref:Aspire.Hosting.SqlServerBuilderExtensions.AddDatabase%2A> | <xref:Aspire.Hosting.ApplicationModel.SqlServerDatabaseResource> | Adds a SQL Server database resource. |

**Azure specific resources are available in the following NuGet packages:**

<span id="azure-hosting-libraries"></span>

- [📦 Aspire.Hosting.Azure.AppConfiguration](https://www.nuget.org/packages/Aspire.Hosting.Azure.AppConfiguration)
- [📦 Aspire.Hosting.Azure.ApplicationInsights](https://www.nuget.org/packages/Aspire.Hosting.Azure.ApplicationInsights)
- [📦 Aspire.Hosting.Azure.CognitiveServices](https://www.nuget.org/packages/Aspire.Hosting.Azure.CognitiveServices)
- [📦 Aspire.Hosting.Azure.CosmosDB](https://www.nuget.org/packages/Aspire.Hosting.Azure.CosmosDB)
- [📦 Aspire.Hosting.Azure.EventHubs](https://www.nuget.org/packages/Aspire.Hosting.Azure.EventHubs)
- [📦 Aspire.Hosting.Azure.KeyVault](https://www.nuget.org/packages/Aspire.Hosting.Azure.KeyVault)
- [📦 Aspire.Hosting.Azure.OperationalInsights](https://www.nuget.org/packages/Aspire.Hosting.Azure.OperationalInsights)
- [📦 Aspire.Hosting.Azure.PostgreSQL](https://www.nuget.org/packages/Aspire.Hosting.Azure.PostgreSQL)
- [📦 Aspire.Hosting.Azure.Redis](https://www.nuget.org/packages/Aspire.Hosting.Azure.Redis)
- [📦 Aspire.Hosting.Azure.Search](https://www.nuget.org/packages/Aspire.Hosting.Azure.Search)
- [📦 Aspire.Hosting.Azure.ServiceBus](https://www.nuget.org/packages/Aspire.Hosting.Azure.ServiceBus)
- [📦 Aspire.Hosting.Azure.SignalR](https://www.nuget.org/packages/Aspire.Hosting.Azure.SignalR)
- [📦 Aspire.Hosting.Azure.Sql](https://www.nuget.org/packages/Aspire.Hosting.Azure.Sql)
- [📦 Aspire.Hosting.Azure.Storage](https://www.nuget.org/packages/Aspire.Hosting.Azure.Storage)
- [📦 Aspire.Hosting.Azure.WebPubSub](https://www.nuget.org/packages/Aspire.Hosting.Azure.WebPubSub)

| Method | Resource type | Description |
|--|--|--|
| <xref:Aspire.Hosting.AzureStorageExtensions.AddAzureStorage%2A> | <xref:Aspire.Hosting.Azure.AzureStorageResource> | Adds an Azure Storage resource. |
| `AddAzureStorage(...).`<xref:Aspire.Hosting.AzureStorageExtensions.AddBlobs%2A> | <xref:Aspire.Hosting.Azure.AzureBlobStorageResource> | Adds an Azure Blob Storage resource. |
| `AddAzureStorage(...).`<xref:Aspire.Hosting.AzureStorageExtensions.AddQueues%2A> | <xref:Aspire.Hosting.Azure.AzureQueueStorageResource> | Adds an Azure Queue Storage resource. |
| `AddAzureStorage(...).`<xref:Aspire.Hosting.AzureStorageExtensions.AddTables%2A> | <xref:Aspire.Hosting.Azure.AzureTableStorageResource> | Adds an Azure Table Storage resource. |
| <xref:Aspire.Hosting.AzureCosmosExtensions.AddAzureCosmosDB%2A> | <xref:Aspire.Hosting.AzureCosmosDBResource> | Adds an Azure Cosmos DB resource. |
| <xref:Aspire.Hosting.AzureKeyVaultResourceExtensions.AddAzureKeyVault%2A> | <xref:Aspire.Hosting.Azure.AzureKeyVaultResource> | Adds an Azure Key Vault resource. |
| `AddRedis(...)`.<xref:Aspire.Hosting.AzureRedisExtensions.AsAzureRedis%2A> | <xref:Aspire.Hosting.Azure.AzureRedisResource> | Configures resource to use Azure for local development and when doing a deployment via the Azure Developer CLI. |
| `AddSqlServer(...)`.<xref:Aspire.Hosting.AzureSqlExtensions.AsAzureSqlDatabase%2A> | <xref:Aspire.Hosting.Azure.AzureSqlServerResource> | Configures SQL Server resource to be deployed as Azure SQL Database (server). |
| <xref:Aspire.Hosting.AzureServiceBusExtensions.AddAzureServiceBus%2A> | <xref:Aspire.Hosting.Azure.AzureServiceBusResource> | Adds an Azure Service Bus resource. |
| <xref:Aspire.Hosting.AzureWebPubSubExtensions.AddAzureWebPubSub%2A> | <xref:Aspire.Hosting.ApplicationModel.AzureWebPubSubResource> | Adds an Azure Web PubSub resources. |

> [!IMPORTANT]
> The .NET Aspire Azure hosting libraries rely on `Azure.Provisioning.*` libraries to provision Azure resources. For more information, [Azure provisioning libraries](../deployment/azure/local-provisioning.md#azure-provisioning-libraries).

**AWS specific resources are available in the following NuGet package:**

<span id="aws-hosting-libraries"></span>

- [📦 Aspire.Hosting.AWS](https://www.nuget.org/packages/Aspire.Hosting.AWS)

For more information, see [GitHub: Aspire.Hosting.AWS library](https://github.com/dotnet/aspire/tree/main/src/Aspire.Hosting.AWS).

## Execution context

The <xref:Aspire.Hosting.IDistributedApplicationBuilder> exposes an execution context (<xref:Aspire.Hosting.DistributedApplicationExecutionContext>), which provides information about the current execution of the app host. This context can be used to evaluate whether or not the app host is executing as "run" mode, or as part of a publish operation. Consider the following:

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

## See also

- [.NET Aspire integrations overview](integrations-overview.md)
- [Service discovery in .NET Aspire](../service-discovery/overview.md)
- [.NET Aspire service defaults](service-defaults.md)
