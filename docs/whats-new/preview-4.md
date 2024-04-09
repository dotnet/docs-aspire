---
title: .NET Aspire preview 4
description: .NET Aspire preview 4 is now available and includes many improvements and new capabilities
ms.date: 03/12/2024
---

# .NET Aspire preview 4

.NET Aspire preview 4 introduces significant enhancements across various parts of the stack, including addressing highly requested features from the community. Notable focus areas include improvements Entity Framework components, Podman support, and changes to the application model to easily choose between using existing resources or provisioning new ones.

The .NET Aspire preview 4 version is `8.0.0-preview.4.24156.9`.

## Podman support

.NET Aspire preview 4 introduces support for running applications using Podman. Podman is a daemonless container engine for developing, managing, and running OCI Containers on your Linux System. It's a great alternative to Docker for Linux users who want to run containers without a daemon.

Docker or Podman will be auto-detected; if both are present, Docker is preferred. Podman can be explicitly enabled/forced via an environment variable `DOTNET_ASPIRE_CONTAINER_RUNTIME=podman`.

## Dashboard

The [.NET Aspire dashboard](../fundamentals/dashboard/overview.md) has been really well received by the developer community. With preview 4, we've made several improvements to the dashboard to make it easier to use and more accessible.

### Dashboard user experience updates

:::image type="content" source="media/preview-4/dashboard-face-lift.png" lightbox="media/preview-4/dashboard-face-lift.png" alt-text=".NET Aspire Dashboard: Updates showing landing page.":::

The dashboard has been updated with a new look and feel. The new dashboard is designed to reduce the space used by the navigation tabs and to make it easier to navigate between logs, metrics, and traces.

### Run the .NET Aspire dashboard standalone

The .NET Aspire dashboard can now be run as a standalone container image. This makes it easier to use the dashboard to manage applications that are running on a different machine or in a different environment. The dashboard can be used as an [OTLP](https://opentelemetry.io/docs/specs/otlp/) collector and viewer for applications that want to send and visualize telemetry data.

Here's the command you can run to start the dashboard:

```docker
docker run --rm -it \
  -p 18888:18888 \
  -p 4317:18889 \
  -d --name aspire-dashboard \
  mcr.microsoft.com/dotnet/nightly/aspire-dashboard:8.0.0-preview.4
```

:::image type="content" source="media/preview-4/dashboard-as-container.png" lightbox="media/preview-4/dashboard-as-container.png" alt-text=".NET Aspire Dashboard: running in Docker Desktop.":::

There are two ports that are exposed:

1. `18888`: The port that serves the dashboard UI.
1. `4317`: The port that serves the OTLP grpc endpoint.

That command brings up a dashboard that you can use view logs, metrics, and traces from your apps. For more information, see the [sample application](/samples/dotnet/aspire-samples/aspire-standalone-dashboard/) that sends telemetry data to the dashboard.

### Dashboard shortcuts

The .NET Aspire dashboard now supports keyboard navigation via keyboard shortcuts. Click <kbd>Shift</kbd>+<kbd>?</kbd> to display the list of available shortcuts:

:::image type="content" source="media/preview-4/dashboard-shortcuts.png" lightbox="media/preview-4/dashboard-shortcuts.png" alt-text=".NET Aspire Dashboard shortcuts dialog.":::

### Metrics table view

With preview 4, we have introduced a screen reader compatible table view for display of metrics data. This has similar options, as the graph, for filters and selecting the duration of time range for metrics display. The default data display is set to "Only show value updates" and can be toggled to display all data points.

:::image type="content" source="media/preview-4/metrics-table.png" lightbox="media/preview-4/metrics-table.png" alt-text=".NET Aspire Dashboard metrics table.":::

## Databases and Entity Framework improvements

In addition to new APIs, the .NET Aspire preview 4 release includes several improvements to the Entity Framework components. These improvements are designed to make it easier to configure and use Entity Framework in .NET Aspire applications.

### More database management tools

.NET Aspire preview 4 [introduces two new database management tools](#addition-of-more-database-admin-tools) for managing MySQL and MongoDB databases, phpMyAdmin and mongo-express.

### New `Enrich` methods

Preview 4 introduces new methods for configuring Entity Framework. The existing `Add[Provider]DbContext()` methods used to register and configure `DbContext` classes are not sufficient for advanced cases like using a different lifetime scope, using custom service types, or configuring the underlying data sources.

To solve these advanced scenarios, new methods named `Enrich[Provider]DbContext()` have been added. These methods do not register the `DbContext` and expect you to do so before invoking them.

Usage example:

```csharp
var connectionString = builder.Configuration
                              .GetConnectionString("catalogdb");

builder.Services.AddDbContextPool<CatalogDbContext>(
    dbContextOptionsBuilder => 
        dbContextOptionsBuilder.UseSqlServer(connectionString));

builder.EnrichSqlServerDbContext<CatalogDbContext>();
```

These methods will still configure command retries, health checks, logging and telemetry.

### Changes to previously existing methods

Since these new methods provide a simpler way to configure the `DbContext`, the already existing ones (`Add[Provider]DbContext()`) have been simplified.

1. They don't provide a way to disable connection pooling through settings anymore. Instead register the `DbContext` with connection pooling disabled and call the corresponding `Enrich` method.
1. The `int MaxRetryCount` settings were removed and replaced by a `bool Retry` flag that and uses the default settings. It is enabled by default. To change the command retries count to a custom value please configure the `DbContext` registration using the specific provider options and invoke the corresponding `Enrich` method.

### Entity Framework migrations

We've improved the process of using [EF Core tooling to create migrations](/ef/core/managing-schemas/migrations/) within .NET Aspire apps. Previously, EF Core tooling would fail, displaying an error that the database connection string is missing. This error occurred because EF Core tooling initiated the app, not .NET Aspire hosting, resulting in a failure to inject a connection string into the app. In preview 4, .NET Aspire detects whether a project is launched with EF Core tooling and disables connection string validation, allowing migrations to be successfully created.

Another challenge with EF Core migrations is applying them to a transient database that starts up with the app. An approach we've been exploring involves adding a .NET background worker resource to the .NET Aspire solution. This worker executes migrations when the app host starts.

Here's a [sample application](/samples/dotnet/aspire-samples/aspire-efcore-migrations/) that shows to create and apply migrations in an .NET Aspire solution.

We're still exploring best practices for using .NET Aspire with EF Core. We plan to publish guidance for using EF Core migrations and Aspire: [Write guidance for using Entity Framework migrations with .NET Aspire solutions (#64)](https://github.com/dotnet/docs-aspire/issues/64).

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

## External parameters

In .NET Aspire preview 4, we've introduced the ability to model external parameters. External parameters are used to model values that are not known at build time and can vary by environment. These values are prompted for when deploying the application.

For more information, see [.NET Aspire fundamentals: External parameters](../fundamentals/external-parameters.md).

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

There are several general improvements to the application model that make it easier to model common scenarios.

### Changing container properties

We added some new methods to tweak container images, tags and volumes.

Here's an example of using the "latest" image tag for the [redis image from DockerHub](https://hub.docker.com/_/redis/).

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis")
                   .WithImageTag("latest"); // Prefer the latest

builder.AddProject<Projects.WebApplication1>("api")
       .WithReference(redis);

builder.Build().Run();
```

### Split bind mounts and volumes into separate methods

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

The preceding code creates a MySQL and MongoDB container with PhpMyAdmin and mongo-express configured.

:::image type="content" source="media/preview-4/dashboard-resources-with-mysql-and-mongoex.png" lightbox="media/preview-4/dashboard-resources-with-mysql-and-mongoex.png" alt-text=".NET Aspire dashboard: Resources tab showing mongo, mongo-mongoexpress, mysql, mysql-phpmyadmin, and an API.":::

**Mongo express**

:::image type="content" source="media/preview-4/mongo-express.png" lightbox="media/preview-4/mongo-express.png" alt-text="Mongo Express database UX.":::

<!--
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
-->

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

This release saw an overhaul of the Azure resources shipped in .NET Aspire. We've introduced a new bicep resource that makes it easier to model a wide range of Azure services. These changes are encapsulated in the [Aspire.Hosting.Azure](https://www.nuget.org/packages/Aspire.Hosting.Azure) NuGet package.

### New resources and components

The following list outlines new components and their corresponding component articles:

- [Azure SignalR](https://azure.microsoft.com/products/signalr-service): [.NET Aspire support for Azure SignalR Service](../real-time/azure-signalr-scenario.md).
- [Azure AI Search](https://azure.microsoft.com/products/ai-services/ai-search): [NET Aspire Azure AI Search Documents component](../azureai/azureai-search-document-component.md).
- [Azure Application Insights](https://azure.microsoft.com/products/monitor): [Use Application Insights for .NET Aspire telemetry](../deployment/azure/application-insights.md).

### Containers with Azure resource mappings

Several services that are available as containers have fully managed Azure equivalents. We've added the ability to map a container to an Azure resource. This makes it possible to develop and test using a container and then deploy using a fully managed Azure resource that will be provisioned as part of the deployment process. These extensions are provided by the [Aspire.Hosting.Azure](https://www.nuget.org/packages/Aspire.Hosting.Azure) NuGet package.

We've enabled this for the following services:

- Redis - [Azure Redis](https://azure.microsoft.com/products/cache)
- Postgres - [Azure Database for PostgreSQL](https://azure.microsoft.com/products/postgresql)
- SQL Server - [Azure SQL Database](https://azure.microsoft.com/products/azure-sql/database)

For more information, see [Azure-specific resource types](../deployment/manifest-format.md#azure-specific-resource-types).

We plan to add support for the following services in the future:

- MySql - [Azure Database for MySQL](https://azure.microsoft.com/products/mysql)
- MongoDb - [Azure Cosmos DB](https://azure.microsoft.com/products/cosmos-db)
- Kafka - [Azure Event Hubs](https://azure.microsoft.com/products/event-hubs)

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

For more information, see [What is Bicep?](/azure/azure-resource-manager/bicep/overview?tabs=bicep).

Consider the following example _Program.cs_ file:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var eventhub = builder.AddBicepTemplate("eventhubs", "eventhub.bicep")
    .WithParameter("eventHubNamespaceName", "mynamespace")
    .WithParameter("principalId")
    .WithParameter("principalType")
    .WithParameter("eventHubs", ["hub1"]);

builder.AddProject<Projects.WebApplication1>("api")
    .WithEnvironment("EventHubsEndpoint", eventhub.GetOutput("eventHubsEndpoint"));

builder.Build().Run();
```

Now consider the corresponding example _eventhub.bicep_ file:

```bicep
@description('Specifies a project name that is used to generate the Event Hub name and the Namespace name.')
@minLength(6)
param eventHubNamespaceName string

param principalId string
param principalType string

param eventHubs array = []

@description('Specifies the Azure location for all resources.')
param location string = resourceGroup().location

@description('Specifies the messaging tier for Event Hub Namespace.')
@allowed([
    'Basic'
    'Standard'
])
param eventHubSku string = 'Basic'

var resourceToken = uniqueString(resourceGroup().id)

resource eventHubNamespace 'Microsoft.EventHub/namespaces@2023-01-01-preview' = {
    name: '${eventHubNamespaceName}${resourceToken}'
    location: location
    sku: {
        name: eventHubSku
        tier: eventHubSku
        capacity: 1
    }
    properties: {
        isAutoInflateEnabled: false
        maximumThroughputUnits: 0
    }

    resource hub 'eventhubs' = [for name in eventHubs: {
        name: name
        properties: {
            messageRetentionInDays: 1
            partitionCount: 1
        }
    }
    ]
}

// https://learn.microsoft.com/azure/role-based-access-control/built-in-roles#azure-event-hubs-data-owner

resource eventHubRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
    name: guid(eventHubNamespace.id, principalId, subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'f526a384-b230-433a-b45c-95f59c4a2dec'))
    scope: eventHubNamespace
    properties: {
        principalId: principalId
        principalType: principalType
        roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'f526a384-b230-433a-b45c-95f59c4a2dec')
    }
}

output eventHubsEndpoint string = eventHubNamespace.properties.serviceBusEndpoint
```

Finally, the app host and the bicep file would result in the following manifest JSON representation:

```json
{
  "resources": {
    "eventhubs": {
      "type": "azure.bicep.v0",
      "path": "eventhub.bicep",
      "parameters": {
        "eventHubNamespaceName": "mynamespace",
        "principalId": "",
        "principalType": "",
        "eventHubs": ["hub1"]
      }
    },
    "api": {
      "type": "project.v0",
      "path": "../WebApplication1/WebApplication1.csproj",
      "env": {
          "OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES": "true",
          "OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EVENT_LOG_ATTRIBUTES": "true",
          "EventHubsEndpoint": "{eventhubs.outputs.eventHubsEndpoint}"
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

The manifest captures both the parameters and the usage of the bicep output in the environment variables of the project. The [Azure Developer CLI](#azure-developer-cli-azd) has been updated to understand this new resource type (you can learn more below) and can be used to deploy this application to **Azure Container Apps**.

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

The [.NET Aspire manifest](../deployment/manifest-format.md) has undergone a significant overhaul to support the new primitives and introduced in preview 4. The new manifest focuses on a few key primitives to enable a wide range of scenarios:

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

### Prompting for parameters

When azd detects a parameter.v0 resource in the manifest, it will prompt for a value. For example, consider the following code in the apphost project:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var administratorLogin = builder.AddParameter("administratorLogin");
var administratorLoginPassword = builder.AddParameter(
    "administratorLoginPassword", secret: true);

var pg = builder.AddPostgres("postgres")
                .AsAzurePostgresFlexibleServer(
                    administratorLogin, administratorLoginPassword)
                .AddDatabase("db");
```

In the preceding code, two parameters are created to represent the admin login and password. They're then passed to the `AsAzurePostgresFlexibleServer` API. This causes `azd` to ask for both parameter values.

:::image type="content" source="media/preview-4/promptUser.gif" lightbox="media/preview-4/promptUser.gif" alt-text="azd parameters prompting":::

To skip interactive prompting, users can set an environment variable for each parameter. For the `administratorLogin` parameter, azd looks for an environment variable with the name `AZURE_ADMINISTRATOR_LOGIN`.

> [!TIP]
> These environment variables should be prefixed with `AZURE_` and followed by the name of the parameter, converted from camel case to upper-snake case.

To see the parameter mapping, run `azd infra synth` and view the `main.parameters.json` file in the _/infra_ folder.

```json
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {
    "administratorLogin": {
      "value": "${AZURE_ADMINISTRATOR_LOGIN}"
    },
    "administratorLoginPassword": {
      "value": "${AZURE_ADMINISTRATOR_LOGIN_PASSWORD}"
    }
  }
}
```

### Find parameters values after provisioning

Azd sets the parameter values for a project as part of the [App Container configuration](/azure/container-apps/containers#configuration), using its environment. When the parameter is defined as `secret: true` (like `administratorLoginPassword` in the previous example), azd creates a secret for the App Container configuration and then links the environment to that secret:

:::image type="content" source="media/preview-4/secretValue.png" lightbox="media/preview-4/secretValue.png" alt-text="azd secret parameter":::

In the previous example, azd takes the bicep resource that was generated by .NET Aspire and sets the input parameter value for it:

```bicep
module postgres 'postgres2/aspire.hosting.azure.bicep.postgres.bicep' = {
  name: 'postgres'
  scope: rg
  params: {
    location: location
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorLoginPassword
    databases: ['db']
    keyVaultName: resources.outputs.SERVICE_BINDING_POSTGRES2KV_NAME
    serverName: 'postgres'
  }
}
```

In this case, `azd` automatically generates an Azure Key Vault resource for the Postgres module to securely persist the connection string using a secret. When the project is deployed to Container Apps, the secret is used to set the full connection string as a Container App secret as shown in the following image.

If the secret is referenced directly, the Container App secret will be a reference to the Key Vault secret as shown in the following image:

:::image type="content" source="media/preview-4/secretAndRefToSecret.png" lightbox="media/preview-4/secretAndRefToSecret.png" alt-text="azd secret and secret reference configuration":::

The `connectionstring--server` holds the reference for the Key Vault secret. The secret is pulled by the Container App when it's used by the service project. On the other hand, the `connectionstring--db2` was created pulling the `connectionString` secret from Key Vault and adding the `Database=` configuration.

## Azure Developer CLI (azd)

The [Azure Developer CLI](https://aka.ms/azd) (azd) has been updated to support the new manifest types introduced in preview 4. Azd supports prompting for parameter resources, and supports the bicep resource which enables deploying any thing. All Azure resources can be described using a bicep module.

In addition to new features, azd will automatically create secrets in Azure Container Apps for any parameter marked as a secret, and any environment variable that references a secret or uses a connection string.

## Visual Studio Publish to Azure

This release we've enabled a Visual Studio publish experience for .NET Aspire applications. This new deployment experience enables a single right-click-publish gesture that results in *all* of the nodes of your .NET Aspire project being published as individual Azure Container Apps in an Azure Container Apps Environment, ideal for dev/test scenarios. Even better - the publishing experience in Visual Studio uses `azd` under the hood, so you'll be able to publish .NET Aspire apps directly from Visual Studio to any `azd` environment you have running in Azure.

:::image type="content" source="media/preview-4/aspire-azd-publish.gif" lightbox="media/preview-4/aspire-azd-publish.gif" alt-text=".NET Aspire right-click publish in Visual Studio":::

With so many similarities between Container Apps' support for [add-on services](/azure/container-apps/services) like PostgreSQL, Kafka, and Redis and the support offered by mirroring .NET Aspire components ([PostgreSQL](/dotnet/aspire/database/postgresql-component?tabs=dotnet-cli), [Kafka](/dotnet/aspire/messaging/kafka-component?tabs=dotnet-cli), and [Redis](/dotnet/aspire/caching/stackexchange-redis-component?tabs=dotnet-cli), for example), targeting Azure Container Apps as our **first** publishing destination was an easy decision. You'll be able to use Azure Container Apps as your development publishing target out-of-the-box using Visual Studio Preview's upcoming 17.10 preview 3 release.

### Considerations during Visual Studio 17.10 Preview 2

In this release, we have two important points you'll want to consider. The Visual Studio publish experience currently lacks support for parameter prompting. We'll add this in a subsequent release.

The second, and more important thing to note, is that you'll need to perform an `azd auth login` before the Visual Studio publish features will work. Once you do the `azd auth login`, you'll be able to right-click publish to any Azure subscription you can access. We're working on this integration and plan to release an integrated auth flow experience from Visual Studio to `azd` prior to Visual Studio 17.10 preview 4. Once you do the `azd auth login`, you'll be good to go.

## Known issues

The following are some known issues with .NET Aspire preview 4.

### Console logs for a container don't show up when using Podman

When using Podman, the console logs for a container don't show up in the .NET Aspire dashboard. For more information, see issues [aspire#2701](https://github.com/dotnet/aspire/issues/2701) and [aspire#2705](https://github.com/dotnet/aspire/issues/2705). This issue is because some assumptions were made about the availability of Docker, see GitHub Aspire repository [issue #2701 comment](https://github.com/dotnet/aspire/issues/2701#issue-2173228010).

**Workaround**: You can add a symlink from `docker` to `podman` as described in [issue comment](https://github.com/dotnet/aspire/issues/2701#issuecomment-1987227953) or you can simply alias Docker to Podman (`alias docker=podman`) as mentioned in [What is Podman](https://docs.podman.io/en/latest/index.html#what-is-podman).

### When running .NET Aspire app, we sometimes see `KubeConfigException : kubeconfig file not found when the host is trying to start up`

As described in [issue #2542](https://github.com/dotnet/aspire/issues/2542), you might see a `KubeConfigException` sometimes when running the Aspire application. For more information, see [issue #2542 comment](https://github.com/dotnet/aspire/issues/2542#issuecomment-1974253166) for a description of why this issue might likely happen.

**Workaround**: Close the Visual Studio debug console window, close and re-run the app again and that should work.

### After .NET 9 SDK (Preview) is installed, you may find that the .NET Aspire 8.0 workload and templates aren't available anymore

If you've installed .NET Aspire 8.0 Preview (either using .NET CLI or acquired through Visual Studio 2022 version 17.10 Preview), and then installed .NET 9 SDK (Preview), you may find that the .NET Aspire 8.0 workload and templates are no longer available. This is a known issue with the way workloads behave and we're working on improving the experience in future previews.

**Workaround**:

- If you're using the CLI, you can use a _global.json_ to pin the SDK to the version 8.0. For more information, see [issue #1951 comment](https://github.com/dotnet/aspire/issues/1951#issue-2105883589).
- If you're using Visual Studio, set the `VS_TEMPLATELOCATOR_SDKVERSION` environment variable that allows Visual Studio to discover version 8.0 of the .NET Aspire templates. For more information, see [issue #2186 comment](https://github.com/dotnet/aspire/issues/2186#issuecomment-1967502080).
