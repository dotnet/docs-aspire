---
title: .NET Aspire MongoDB database integration
description: Learn how to use the .NET Aspire MongoDB database integration, which includes both hosting and client integrations.
ms.date: 02/19/2025
uid: database/mongodb-integration
ms.custom: sfi-ropc-nochange
---

# .NET Aspire MongoDB database integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[MongoDB](https://www.mongodb.com) is a NoSQL database that provides high performance, high availability, and easy scalability. The .NET Aspire MongoDB integration enables you to connect to existing MongoDB instances (including [MongoDB Atlas](https://mdb.link/atlas)) or create new instances from .NET with the [`docker.io/library/mongo` container image](https://hub.docker.com/_/mongo)

## Hosting integration

The MongoDB server hosting integration models the server as the <xref:Aspire.Hosting.ApplicationModel.MongoDBServerResource> type and the database as the <xref:Aspire.Hosting.ApplicationModel.MongoDBDatabaseResource> type. To access these types and APIs, add the [📦 Aspire.Hosting.MongoDB](https://www.nuget.org/packages/Aspire.Hosting.MongoDB) NuGet package in the [AppHost](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.MongoDB
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.MongoDB"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add MongoDB server resource and database resource

In your AppHost project, call <xref:Aspire.Hosting.MongoDBBuilderExtensions.AddMongoDB*> to add and return a MongoDB server resource builder. Chain a call to the returned resource builder to <xref:Aspire.Hosting.MongoDBBuilderExtensions.AddDatabase*>, to add a MongoDB database resource.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var mongo = builder.AddMongoDB("mongo")
                   .WithLifetime(ContainerLifetime.Persistent);

var mongodb = mongo.AddDatabase("mongodb");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(mongodb)
       .WaitFor(mongodb);

// After adding all resources, run the app...
```

> [!NOTE]
> The MongoDB container can be slow to start, so it's best to use a _persistent_ lifetime to avoid unnecessary restarts. For more information, see [Container resource lifetime](../fundamentals/orchestrate-resources.md#container-resource-lifetime).

When .NET Aspire adds a container image to the AppHost, as shown in the preceding example with the `docker.io/library/mongo` image, it creates a new MongoDB instance on your local machine. A reference to your MongoDB server resource builder (the `mongo` variable) is used to add a database. The database is named `mongodb` and then added to the `ExampleProject`. The MongoDB server resource includes default credentials:

- `MONGO_INITDB_ROOT_USERNAME`: A value of `admin`.
- `MONGO_INITDB_ROOT_PASSWORD`: Random `password` generated using the <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.CreateDefaultPasswordParameter*> method.

When the AppHost runs, the password is stored in the AppHost's secret store. It's added to the `Parameters` section, for example:

```json
{
  "Parameters:mongo-password": "<THE_GENERATED_PASSWORD>"
}
```

The name of the parameter is `mongo-password`, but really it's just formatting the resource name with a `-password` suffix. For more information, see [Safe storage of app secrets in development in ASP.NET Core](/aspnet/core/security/app-secrets) and [Add MongoDB server resource with parameters](#add-mongodb-server-resource-with-parameters).

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` named `mongodb` and the <xref:Aspire.Hosting.ResourceBuilderExtensions.WaitFor*> instructs the AppHost to not start the dependant service until the `mongodb` resource is ready.

> [!TIP]
> If you'd rather connect to an existing MongoDB server, call <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> instead. For more information, see [Reference existing resources](../fundamentals/app-host-overview.md#reference-existing-resources).

### Add MongoDB server resource with data volume

To add a data volume to the MongoDB server resource, call the <xref:Aspire.Hosting.MongoDBBuilderExtensions.WithDataVolume*> method on the MongoDB server resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var mongo = builder.AddMongoDB("mongo")
                   .WithDataVolume();

var mongodb = mongo.AddDatabase("mongodb");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(mongodb)
       .WaitFor(mongodb);

// After adding all resources, run the app...
```

The data volume is used to persist the MongoDB server data outside the lifecycle of its container. The data volume is mounted at the `/data/db` path in the MongoDB server container and when a `name` parameter isn't provided, the name is generated at random. For more information on data volumes and details on why they're preferred over [bind mounts](#add-mongodb-server-resource-with-data-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

> [!WARNING]
> The password is stored in the data volume. When using a data volume and if the password changes, it will not work until you delete the volume.

> [!IMPORTANT]
> Some database integrations, including the .NET Aspire MongoDB integration, can't successfully use data volumes after deployment to Azure Container Apps (ACA). This is because ACA uses Server Message Block (SMB) to connect containers to data volumes, and some systems can't use this connection. In the Aspire Dashboard, a database affected by this issue has a status of **Activating** or **Activation Failed** but is never listed as **Running**.
>
> You can resolve the problem by deploying to a Kubernetes cluster, such as Azure Kubernetes Services (AKS). For more information, see [.NET Aspire deployments](../deployment/overview.md).

### Add MongoDB server resource with data bind mount

To add a data bind mount to the MongoDB server resource, call the <xref:Aspire.Hosting.MongoDBBuilderExtensions.WithDataBindMount*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var mongo = builder.AddMongoDB("mongo")
                   .WithDataBindMount(@"C:\MongoDB\Data");

var mongodb = mongo.AddDatabase("mongodb");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(mongodb)
       .WaitFor(mongodb);

// After adding all resources, run the app...
```

[!INCLUDE [data-bind-mount-vs-volumes](../includes/data-bind-mount-vs-volumes.md)]

Data bind mounts rely on the host machine's filesystem to persist the MongoDB server data across container restarts. The data bind mount is mounted at the `C:\MongoDB\Data` on Windows (or `/MongoDB/Data` on Unix) path on the host machine in the MongoDB server container. For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

### Add MongoDB server resource with initialization data bind mount

To add an initialization folder data bind mount to the MongoDB server resource, call the <xref:Aspire.Hosting.MongoDBBuilderExtensions.WithInitBindMount*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var mongo = builder.AddMongoDB("mongo")
                   .WithInitBindMount(@"C:\MongoDB\Init");

var mongodb = mongo.AddDatabase("mongodb");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(mongodb)
       .WaitFor(mongodb);

// After adding all resources, run the app...
```

The initialization data bind mount is used to initialize the MongoDB server with data. The initialization data bind mount is mounted at the `C:\MongoDB\Init` on Windows (or `/MongoDB/Init` on Unix) path on the host machine in the MongoDB server container and maps to the `/docker-entrypoint-initdb.d` path in the MongoDB server container. MongoDB executes the scripts found in this folder, which is useful for loading data into the database.

### Add MongoDB server resource with parameters

When you want to explicitly provide the password used by the container image, you can provide these credentials as parameters. Consider the following alternative example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("username");
var password = builder.AddParameter("password", secret: true);

var mongo = builder.AddMongoDB("mongo", username, password);
var mongodb = mongo.AddDatabase("mongodb");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(mongodb)
       .WaitFor(mongodb);

// After adding all resources, run the app...
```

For more information on providing parameters, see [External parameters](../fundamentals/external-parameters.md).

### Add MongoDB Express resource

[MongoDB Express](https://github.com/mongo-express/mongo-express) is a web-based MongoDB admin user interface. To add a MongoDB Express resource that corresponds to the [`docker.io/library/mongo-express` container image](https://hub.docker.com/_/mongo-express/), call the <xref:Aspire.Hosting.MongoDBBuilderExtensions.WithMongoExpress*> method on the MongoDB server resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var mongo = builder.AddMongoDB("mongo")
                   .WithMongoExpress();

var mongodb = mongo.AddDatabase("mongodb");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(mongodb)
       .WaitFor(mongodb);

// After adding all resources, run the app...
```

> [!TIP]
> To configure the host port for the <xref:Aspire.Hosting.MongoDB.MongoExpressContainerResource> chain a call to the <xref:Aspire.Hosting.MongoDBBuilderExtensions.WithHostPort*> API and provide the desired port number.

The preceding code adds a MongoDB Express resource that is configured to connect to the MongoDB server resource. The default credentials are:

- `ME_CONFIG_MONGODB_SERVER`: The name assigned to the parent `MongoDBServerResource`, in this case it would be `mongo`.
- `ME_CONFIG_BASICAUTH`: A value of `false`.
- `ME_CONFIG_MONGODB_PORT`: Assigned from the primary endpoint's target port of the parent `MongoDBServerResource`.
- `ME_CONFIG_MONGODB_ADMINUSERNAME`: The same username as configured in the parent `MongoDBServerResource`.
- `ME_CONFIG_MONGODB_ADMINPASSWORD`: The same password as configured in the parent `MongoDBServerResource`.

Additionally, the `WithMongoExpress` API exposes an optional `configureContainer` parameter of type `Action<IResourceBuilder<MongoExpressContainerResource>>` that you use to configure the MongoDB Express container resource.

### Hosting integration health checks

The MongoDB hosting integration automatically adds a health check for the MongoDB server resource. The health check verifies that the MongoDB server resource is running and that a connection can be established to it.

The hosting integration relies on the [📦 AspNetCore.HealthChecks.MongoDb](https://www.nuget.org/packages/AspNetCore.HealthChecks.MongoDb) NuGet package.

## Client integration

To get started with the .NET Aspire MongoDB client integration, install the [📦 Aspire.MongoDB.Driver](https://www.nuget.org/packages/Aspire.MongoDB.Driver) NuGet package in the client-consuming project, that is, the project for the application that uses the MongoDB client. The MongoDB client integration registers a [IMongoClient](https://mongodb.github.io/mongo-csharp-driver/3.0.0/api/MongoDB.Driver/MongoDB.Driver.IMongoClient.html) instance that you can use to interact with the MongoDB server resource. If your AppHost adds MongoDB database resources, the [IMongoDatabase](https://mongodb.github.io/mongo-csharp-driver/3.0.0/api/MongoDB.Driver/MongoDB.Driver.IMongoDatabase.html) instance is also registered.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.MongoDB.Driver
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.MongoDB.Driver"
                  Version="*" />
```

---

> [!IMPORTANT]
> The `Aspire.MongoDB.Driver` NuGet package depends on the `MongoDB.Driver` NuGet package. With the release of version 3.0.0 of `MongoDB.Driver`, a binary breaking change was introduced. To address this, a new client integration package, `Aspire.MongoDB.Driver.v3`, was created. The original `Aspire.MongoDB.Driver` package continues to reference `MongoDB.Driver` version 2.30.0, ensuring compatibility with previous versions of the RabbitMQ client integration. The new `Aspire.MongoDB.Driver.v3` package references `MongoDB.Driver` version 3.0.0. In a future version of .NET Aspire, the `Aspire.MongoDB.Driver` will be updated to version `3.x` and the `Aspire.MongoDB.Driver.v3` package will be deprecated. For more information, see [Upgrade to version 3.0](https://www.mongodb.com/docs/drivers/csharp/v3.0/upgrade/v3/).

### Add MongoDB client

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireMongoDBDriverExtensions.AddMongoDBClient*> extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register a `IMongoClient` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddMongoDBClient(connectionName: "mongodb");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the MongoDB server resource (or the database resource when provided) in the AppHost project. In other words, when you call `AddDatabase` and provide a name of `mongodb` that same name should be used when calling `AddMongoDBClient`. For more information, see [Add MongoDB server resource and database resource](#add-mongodb-server-resource-and-database-resource).

You can then retrieve the `IMongoClient` instance using dependency injection. For example, to retrieve the client from an example service:

```csharp
public class ExampleService(IMongoClient client)
{
    // Use client...
}
```

The `IMongoClient` is used to interact with the MongoDB server resource. It can be used to create databases that aren't already known to the AppHost project. When you define a MongoDB database resource in your AppHost, you could instead require that the dependency injection container provides an `IMongoDatabase` instance. For more information on dependency injection, see [.NET dependency injection](/dotnet/core/extensions/dependency-injection).

### Add keyed MongoDB client

There might be situations where you want to register multiple `IMongoDatabase` instances with different connection names. To register keyed MongoDB clients, call the <xref:Microsoft.Extensions.Hosting.AspireMongoDBDriverExtensions.AddKeyedMongoDBClient*> method:

```csharp
builder.AddKeyedMongoDBClient(name: "mainDb");
builder.AddKeyedMongoDBClient(name: "loggingDb");
```

> [!IMPORTANT]
> When using keyed services, it's expected that your MongoDB resource configured two named databases, one for the `mainDb` and one for the `loggingDb`.

Then you can retrieve the `IMongoDatabase` instances using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(
    [FromKeyedServices("mainDb")] IMongoDatabase mainDatabase,
    [FromKeyedServices("loggingDb")] IMongoDatabase loggingDatabase)
{
    // Use databases...
}
```

For more information on keyed services, see [.NET dependency injection: Keyed services](/dotnet/core/extensions/dependency-injection#keyed-services).

### Configuration

The .NET Aspire MongoDB database integration provides multiple configuration approaches and options to meet the requirements and conventions of your project.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddMongoDBClient()`:

```csharp
builder.AddMongoDBClient("mongo");
```

The connection string is retrieved from the `ConnectionStrings` configuration section. Consider the following MongoDB example JSON configuration:

```json
{
  "ConnectionStrings": {
    "mongo": "mongodb://server:port/test",
  }
}
```

Alternatively, consider the following MongoDB Atlas example JSON configuration:

```json
{
  "ConnectionStrings": {
    "mongo": "mongodb+srv://username:password@server.mongodb.net/",
  }
}
```

For more information on how to format this connection string, see [MongoDB: ConnectionString documentation](https://www.mongodb.com/docs/v3.0/reference/connection-string).

#### Use configuration providers

The .NET Aspire MongoDB integration supports <xref:Microsoft.Extensions.Configuration>. It loads the <xref:Aspire.MongoDB.Driver.MongoDBSettings> from configuration by using the `Aspire:MongoDB:Driver` key. The following snippet is an example of a _:::no-loc text="appsettings.json":::_ file that configures some of the options:

```json
{
  "Aspire": {
    "MongoDB": {
      "Driver": {
        "ConnectionString": "mongodb://server:port/test",
        "DisableHealthChecks": false,
        "HealthCheckTimeout": 10000,
        "DisableTracing": false
      }
    }
  }
}
```

#### Use named configuration

The .NET Aspire MongoDB integration supports named configuration, which allows you to configure multiple instances of the same resource type with different settings. The named configuration uses the connection name as a key under the main configuration section.

```json
{
  "Aspire": {
    "MongoDB": {
      "Driver": {
        "mongo1": {
          "ConnectionString": "mongodb://server1:port/test",
          "DisableHealthChecks": false,
          "HealthCheckTimeout": 10000
        },
        "mongo2": {
          "ConnectionString": "mongodb://server2:port/test",
          "DisableTracing": true,
          "HealthCheckTimeout": 5000
        }
      }
    }
  }
}
```

In this example, the `mongo1` and `mongo2` connection names can be used when calling `AddMongoDBClient`:

```csharp
builder.AddMongoDBClient("mongo1");
builder.AddMongoDBClient("mongo2");
```

Named configuration takes precedence over the top-level configuration. If both are provided, the settings from the named configuration override the top-level settings.

#### Use inline configurations

You can also pass the `Action<MongoDBSettings>` delegate to set up some or all the options inline:

```csharp
builder.AddMongoDBClient("mongodb",
    static settings => settings.ConnectionString = "mongodb://server:port/test");
```

#### Configuration options

Here are the configurable options with corresponding default values:

| Name                  | Description                                                                           |
|-----------------------|---------------------------------------------------------------------------------------|
| `ConnectionString`    | The connection string of the MongoDB database database to connect to.                 |
| `DisableHealthChecks` | A boolean value that indicates whether the database health check is disabled or not.  |
| `HealthCheckTimeout`  | An `int?` value that indicates the MongoDB health check timeout in milliseconds.      |
| `DisableTracing`      | A boolean value that indicates whether the OpenTelemetry tracing is disabled or not.  |

[!INCLUDE [client-integration-health-checks](../includes/client-integration-health-checks.md)]

By default, the .NET Aspire MongoDB client integration handles the following scenarios:

- Adds a health check when enabled that verifies that a connection can be made commands can be run against the MongoDB database within a certain amount of time.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

#### Logging

The .NET Aspire MongoDB database integration uses standard .NET logging, and you see log entries from the following categories:

- `MongoDB[.*]`: Any log entries from the MongoDB namespace.

#### Tracing

The .NET Aspire MongoDB database integration emits the following Tracing activities using OpenTelemetry:

- `MongoDB.Driver.Core.Extensions.DiagnosticSources`

### Metrics

The .NET Aspire MongoDB database integration doesn't currently expose any OpenTelemetry metrics.

## See also

- [MongoDB database](https://www.mongodb.com/docs/drivers/csharp/current/quick-start)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
