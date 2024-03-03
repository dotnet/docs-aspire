---
title: .NET Aspire preview 4
description: .NET Aspire preview 4 is now available and includes many improvements and new capabilities
ms.date: 02/27/2024
---

# .NET Aspire preview 4

.NET Aspire preview 4 introduces lots of new improvements to various parts of the stack including addressing some of the most requested features from the community.

## Podman support

.NET Aspire preview 4 introduces support for running applications using Podman. Podman is a daemonless container engine for developing, managing, and running OCI Containers on your Linux System. It's a great alternative to Docker for Linux users who want to run containers without a daemon.

Docker or Podman will be auto-detected; if both are present, Docker is preferred. Podman can be explicitly enabled/forced via an environment variable `DOTNET_ASPIRE_CONTAINER_RUNTIME=podman`.

## Dashboard

### Dashboard face lift

:::image type="content" source="media/preview-4/dashboard-face-lift.png" lightbox="media/preview-4/dashboard-face-lift.png" alt-text=".NET Aspire Dashboard: Updates showing landing page.":::

The dashboard has been updated with a new look and feel. The new dashboard is designed to reduce the space used by the navigation tabs and to make it easier to navigate between logs, metrics, and traces.

### Running the Aspire dashboard standalone

The Aspire dashboard can now be run as a standalone container image. This makes it easier to use the dashboard to manage applications that are running on a different machine or in a different environment. The dashboard can be used as an [OTLP](https://opentelemetry.io/docs/specs/otlp/) collector and viewer for applications that want to send and visualize telemetry data.

Here's the command you can run to start the dashboard:

```bash
docker run --rm -it -p 18888:18888 -p 4317:18889 -d --name aspire-dashboard mcr.microsoft.com/dotnet/nightly/aspire-dashboard:8.0.0-preview.4
```

:::image type="content" source="media/preview-4/dashboard-as-container.png" lightbox="media/preview-4/dashboard-as-container.png" alt-text=".NET Aspire Dashboard: running in Docker Desktop.":::

There are 2 ports that are exposed:

1. The port that serves the dashboard UI (18888)
2. The port that serves the OTLP grpc endpoint (18889)

That will bring up a dashboard that you can use view logs, metrics, and traces from your applications. Here's a [sample application](https://learn.microsoft.com/en-us/samples/dotnet/aspire-samples/aspire-standalone-dashboard/) that sends telemetry data to the dashboard.

### Dashboard shortcuts

The Aspire Dashboard now supports keyboard navigation via keyboard shortcuts. Click <kbd>Shift</kbd>+<kbd>?</kbd> to display the list of available shortcuts:

:::image type="content" source="media/preview-4/dashboard-shortcuts.png" lightbox="media/preview-4/dashboard-shortcuts.png" alt-text=".NET Aspire Dashboard shortcuts dialog.":::

### Metrics table view

With preview 4, we have introduced a screen reader compatible table view for display of metrics data. This has similar options, as the graph, for filters and selecting the duration of time range for metrics display. The default data display is set to "Only show value updates" and can be toggled to display all data points.

![image](https://github.com/dotnet/docs-aspire/assets/102772054/4622cf3a-dcf2-4149-9636-d1bef8184c5c)

## Databases, EntityFramework and Aspire

### New Enrich methods

Preview 4 introduces new methods for configuring Entity Framework. The existing `Add[Provider]DbContext()` methods used to register and configure `DbContext` classes are not sufficient for advanced cases like using a different lifetime scope, using custom service types, or configuring the underlying data sources.

To solve these advanced scenarios, new methods named `Enrich[Provider]DbContext()` have been added. These methods do not register the `DbContext` and expect you to do so before invoking them.

Usage example:

```csharp
var connectionString = builder.Configuration.GetConnectionString("catalogdb");
builder.Services.AddDbContextPool<CatalogDbContext>(dbContextOptionsBuilder => dbContextOptionsBuilder.UseSqlServer(connectionString));
builder.EnrichSqlServerDbContext<CatalogDbContext>();
```

These methods will still configure command retries, health checks, logging and telemetry.

### Changes to previously existing methods

Since these new methods provide a simpler way to configure the `DbContext`, the already existing ones (`Add[Provider]DbContext()`) have been simplified.

1- They don't provide a way to disable connection pooling through settings anymore. Instead register the `DbContext` with connection pooling disabled and call the corresponding `Enrich` method.
2- The `int MaxRetryCount` settings were removed and replaced by a `bool Retry` flag that and uses the default settings. It is enabled by default. To change the command retries count to a custom value please configure the `DbContext` registration using the specific provider options and invoke the corresponding `Enrich` method.  

### Entity Framework Migrations

TODO: Migration tooling guidance

### Changes to database servers resources

With Preview 4 the `AddDatabase(string name)` method available on Database Servers resources has been improved such that the name of the resource (and as a consequence the connection name) that was registered can be different than the database name: `AddDatabase(string name, string databaseName = null)`

Here is an example that defines a database named `customers` and registers it as `crm`:

In your app host:

```csharp
builder.AddPostgres("postgres").AddDatabase("crm", "customers");
```

And in your application, resolve the component using the `crm` connection name:

```csharp
builder.AddNpgsqlDbContext<CustomerDbContext>("crm");
```

## Changes to container resources

In .NET Aspire preview 3 for container resources, we introduced `AddXX` and `AddXXContainer`. We've removed `AddXXContainer` and have a single method `AddXX` to add a container resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis");

builder.AddProject<Projects.WebApplication1>("api")
       .WithReference(redis);

builder.Build().Run();
```

These resources are always containers, they will run locally as containers and will be deployed as containers. We've removed the notion of "abstract resources" that can morph between running locally and deploying.

The representation in the manifest for these resources is `container.v0`.

```json
{
  "resources": {
    "redis": {
      "type": "container.v0",
      "connectionString": "{redis.bindings.tcp.host}:{redis.bindings.tcp.port}",
      "image": "redis:7.2.4",
      "bindings": {
        "tcp": {
          "scheme": "tcp",
          "protocol": "tcp",
          "transport": "tcp",
          "containerPort": 6379
        }
      }
    }
  }
}
```

This makes it easier for deployment tools to support a wide range of scenarios without having to understand the nuances of different resource types.

## Parameters

Parameters express the ability to ask for an external value when running the application. Parameters can be used to provide values to the application when running locally, or to prompt for values when deploying. They can be used to model a wide range of scenarios including secrets, connection strings, and other configuration values that might vary between environments.

### Parameter values

Parameter values are read from the "Parameters" section of the app host's configuration and can be used to provide values to the application when running locally. When deploying the application, the value will be asked for the parameter value.

_Program.cs_

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var value = builder.AddParameter("value");

builder.AddProject<Projects.WebApplication1>("api")
       .WithEnvironment("SOME_VALUE", value);
```

_appsettings.json_

```json
{
    "Parameters": {
        "value": "local-value"
    }
}
```

Parameters are represented in the manifest as a new primitive called `parameter.v0`.

```json
{
  "resources": {
    "value": {
      "type": "parameter.v0",
      "value": "{value.inputs.value}",
      "inputs": {
        "value": {
          "type": "string"
        }
      }
    }
  }
}
```

### Secrets

Parameters can be used to model secrets. When a parameter is marked as a secret, this is a hint to the manifest that the value should be treated as a secret. When deploying, the value will be prompted for and stored in a secure location. When running locally, the value will be read from the "Parameters" section of the app host configuration.

_Program.cs_

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var secret = builder.AddParameter("secret", secret: true);

builder.AddProject<Projects.WebApplication1>("api")
       .WithEnvironment("SECRET", secret);

builder.Build().Run();
```

_appsettings.json_

```json
{
    "Parameters": {
        "secret": "local-secret"
    }
}
```

Manifest representation:

```json
{
  "resources": {
    "value": {
      "type": "parameter.v0",
      "value": "{value.inputs.value}",
      "inputs": {
        "value": {
          "type": "string",
          "secret": true
        }
      }
    }
  }
}
```

### Connection strings

Parameters can be used to model connection strings. When deploying, the value will be prompted for and stored in a secure location. When running locally, the value will be read from the "ConnectionStrings" section of the app host configuration.

> [!NOTE]
> Connection strings are used to represent a wide range of connection information including database connections, message brokers, and other services. In Aspire nomenclature, we use the term "connection string" to represent any kind of connection information.*

_Program.cs_

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddConnectionString("redis");

builder.AddProject<Projects.WebApplication1>("api")
       .WithReference(redis);

builder.Build().Run();
```

**appsettings.json**

```json
{
    "ConnectionStrings": {
        "redis": "local-connection-string"
    }
}
```

Manifest representation:

```json
{
  "resources": {
    "redis": {
      "type": "parameter.v0",
      "connectionString": "{redis.value}",
      "value": "{redis.inputs.value}",
      "inputs": {
        "value": {
          "type": "string",
          "secret": true
        }
      }
    }
  }
}
```

## New idioms

The following section outlines several new idioms that have been introduced in .NET Aspire preview 4. These idioms are designed to make it easier to model common scenarios when building the application model.

### The `DistributedApplicationExecutionContext`

The `DistributedApplicationExecutionContext` is a new type that provides information about the current execution context. It can be used to determine if the application is being orchestrated running locally or if it is being use to published the manifest.

This can be useful when building the application model. For example, you might want to use a different message broker when running locally than when deploying.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

if (builder.ExecutionContext.IsPublishMode)
{
    // Do something when running in publish mode
}
else
{
    // Do something when running locally
}
```

The `DistributedApplicationExecutionContext` is also available in the DI container and can be used to determine the execution context when resolving services.

### `PublishAs`, `RunAs` and `As`

.NET Aspire preview 4 introduces new idioms for describing common scenarios for modeling how resources are used when running locally and when deploying. While it's possible to model these scenarios using the `DistributedApplicationExecutionContext`, the new idioms make it easier to express these common scenarios.

- `RunAsXX`: Only affects the model when running locally.
- `PublishAsXX`: Only affects the model when publishing the manifest.
- `AsXX`: Affects the model both when running locally and when publishing the manifest.

#### Examples

The following logic will use a redis container locally and prompt for
the connection string when deploying.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis")
                   .PublishAsConnectionString();

builder.AddProject<Projects.WebApplication1>("api")
       .WithReference(redis);

builder.Build().Run();
```

## General application model improvements

### Changing container properties

We added some new methods to tweak container images, tags and volumes.

Here's an example of using the redis image from Microsoft's container registry instead of the default image from DockerHub.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis")
                   .WithImage("mcr.microsoft.com/cbl-mariner/base/redis")
                   .WithImageTag("6.2-cm2.0");

builder.AddProject<Projects.WebApplication1>("api")
       .WithReference(redis);

builder.Build().Run();
```

### Splitting bind mounts and volumes into separate methods

Splitting bind mounts and volumes into separate methods. Bind mounts are used to mount a file or directory from the host into the container. Volumes are used to mount a volume from the host into the container. Splitting these into separate methods makes it easier to understand how the container is being used.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// MySql database and table names are case-sensitive on non-Windows.
var catalogDbName = "catalog";

var catalogDb = builder.AddMySql("mysql")
    .WithEnvironment("MYSQL_DATABASE", catalogDbName)
    .WithBindMount("../MySql.ApiService/data", "/docker-entrypoint-initdb.d")
    .AddDatabase(catalogDbName);

builder.AddProject<Projects.MySql_ApiService>("api")
       .WithReference(catalogDb);

builder.Build().Run();
```

### Addition of more database admin tools

.NET Aspire Preview 3 introduced the ability to manage postgres databases using pgAdmin and redis using redis commander. Preview 4 introduces the ability to manage MySql databases using phpMyAdmin, and MongoDB databases using mongo-express.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var mongo = builder.AddMongoDB("mongo")
                   .WithMongoExpress()
                   .AddDatabase("db");

var mySql = builder.AddMySql("mysql")
                   .WithPhpMyAdmin()
                   .AddDatabase("catalog");

builder.AddProject<Projects.WebApplication1>("api")
       .WithReference(mongo)
       .WithReference(mySql);

builder.Build().Run();
```

## Ability to ignore launch profiles

Sometimes you might want to ignore launch profiles when running the application. This can be useful when you want to define your own environment or endpoints when running the application.

> [!NOTE]
> This will ignore the entire launch profile, including environment variables and other defaults.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.WebApplication1>("api")
       .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
       .WithHttpEndpoint()
       .ExcludeLaunchProfile();

builder.Build().Run();
```

## Ability to disable endpoint proxies

Any resource with an endpoint today uses a tcp proxy to route traffic to the resource. This is useful for several reasons:

1. The proxy can be used hold connection until the underlying resource is ready.
1. The proxy can be used to route traffic between different replicas of a resource. This gives consumers a stable endpoint to connect to.

Proxies may not always be desirable. If the application already has a port allocated that cannot be configured outside of the application then it's crucial to disable the proxy.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.WebApplication1>("api")
       .ExcludeLaunchProfile()
       .WithEndpoint(scheme: "http", hostPort: 5000, isProxied: false);

builder.Build().Run();
```

The above example will disable the launch profile and add a proxy-less endpoint. The orchestrator will not create a proxy and the application's port will be the only port in use. In this mode, replicas are not supported.

## Azure

This release saw an overhaul of the Azure resources shipped in .NET Aspire. We've introduced a new bicep resource that makes it easier to model a wide range of Azure services. These changes are encapsulated in the **Aspire.Hosting.Azure** package.

### New resources and components

We've added a few new Azure based resources:

- [Azure SignalR](/services/signalr-service/)
- [Azure AI Search](/services/search/)
- [Azure Application Insights](/products/monitor/)

### Containers with Azure Resource mappings

Several services that are available as containers have fully managed Azure equivalents. We've added the ability to map a container to an Azure resource. This makes it possible to develop and test using a container and then deploy using a fully managed Azure resource that will be provisioned as part of the deployment process. These extensions are provided by the **Aspire.Hosting.Azure** package.

We've enabled this for the following services:

- Redis - [Azure Redis](/products/cache/)
- Postgres - [Azure Database for PostgreSQL](/services/postgresql/)
- SQL Server - [Azure SQL Database](/services/sql-database/)

We plan to add support for the following services in the future:

- MySql - [Azure Database for MySQL](/services/mysql/)
- MongoDb - [Azure Cosmos DB](/services/cosmos-db/)
- Kafka - [Azure Event Hubs](/services/event-hubs/)

***Example: Redis***

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis")
                   .PublishAsAzureRedis();

builder.AddProject<Projects.WebApplication1>("api")
       .WithReference(redis);

builder.Build().Run();
```

> [!IMPORTANT]
> It's possible to use an existing Azure resource by providing the connection string and using `AddConnectionString`. This assumes that the resource has already been provisioned, that the resource is accessible from the development environment:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddConnectionString("redis");

builder.AddProject<Projects.WebApplication1>("api")
       .WithReference(redis);

builder.Build().Run();
```

### Azure Bicep resource

We've introduced a new primitive to model Azure Bicep modules in the application model. This makes it easier to model any set of azure resources that can be modeled using bicep. We've rebuilt the azure resources to use the new bicep primitive. Bicep files can be expressed as literal strings, embedded resources or files on disk (relative to the app host).

You can learn more about bicep [here](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/overview?tabs=bicep).

_Program.cs_

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var bicep = builder.AddBicepTemplateString("bicep", "test.bicep")
    .WithParameter("param1", "value1")
    .WithParameter("param2", "value2");

builder.AddProject<Projects.WebApplication1>("api")
    .WithEnvironment("BICEP_OUTPUT", bicep.GetOutput("both"));

builder.Build().Run();
```

_test.bicep_

```bicep
param param1 string
param param2 string
param location string

output both string = '${param1} ${param2}'
```

Manifest representation:

```json
{
  "resources": {
    "bicep": {
      "type": "azure.bicep.v0",
      "path": "test.bicep",
      "parameters": {
        "param1": "value1",
        "param2": "value2"
      }
    },
    "api": {
      "type": "project.v0",
      "path": "../WebApplication1/WebApplication1.csproj",
      "env": {
          "OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES":   "true",
          "OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EVENT_LOG_ATTRIBUTES":   "true",
          "BICEP_OUTPUT": "{bicep.outputs.both}"
      },
      "bindings": {
          "http": {
          "scheme": "http",
          "protocol": "tcp",
          "transport": "http"
          },
          "https": {
          "scheme": "https",
          "protocol": "tcp",
          "transport": "http"
          }
      }
    }
  }
}
```

As you can see, the manifest captures both the parameters and the usage of the bicep output in the environment variables of the project.

## Emulators

In .NET Aspire preview 3, we introduced the ability to run emulators for various services. We've changed the APIs to match with the idioms described earlier in this document:

**Azurite: Azure storage emulator**

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var blobs = builder.AddAzureStorage("storage")
                   .RunAsEmulator()
                   .AddBlobs("blobs");

builder.AddProject<Projects.Api>("api")
       .WithReference(blobs);

builder.Build().Run();
```

Each `RunAsEmulator` method has a callback that enables customization of the emulator container resource. For example, you can change the image, tag, or add additional volumes.

**Azure storage emulator (Azurite)**

```csharp
var storage = builder.AddAzureStorage("storage").RunAsEmulator(container =>
{
    container.UsePersistence();
});
```

**CosmosDB emulator**

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddAzureCosmosDB("cosmos")
                .RunAsEmulator()
                .AddDatabase("db");

builder.AddProject<Projects.Api>("api")
       .WithReference(db);

builder.Build().Run();
```

## The deployment manifest

The aspire manifest has undergone a significant overhaul to support the new primitives and introduced in preview 4. The new manifest focuses on a few key primitives to enable a wide range of scenarios:

| Resource type   | Description                                                   |
|-----------------|---------------------------------------------------------------|
| `project.v0`    | .NET project files                                            |
| `container.v0`  | Container images                                              |
| `dockerfile.v0` | Docker files                                                  |
| `parameter.v0`  | External Parameters                                           |
| `value.v0`      | References to other resources (or a combination of resources) |

### Azure specific resources

| Resource type    | Description           |
|------------------|-----------------------|
| `azure.bicep.v0` | Azure Bicep templates |

Tool authors can support this very small set of resource types to model lots of different apps!

We've deprecated the abstract resource types that were supported in previous versions. Deployment tools may still support those resource types, but they are no longer part of the core manifest schema.

### Visual Studio Publish to Azure

This release we've enabled a Visual Studio publish experience for Aspire applications.

## Azure Developer CLI

The Azure Developer CLI (azd) has been updated to support the new manifest types introduced in preview 4. Azd supports prompting for parameter resources, and supports the bicep resource which enables deploying any thing can be described using a bicep module.

In addition to new features, azd will automatically create secrets in Azure Container Apps for any parameter marked as a secret, and any environment variable that references a secret or uses a connection string.

TODO: Expand with images
