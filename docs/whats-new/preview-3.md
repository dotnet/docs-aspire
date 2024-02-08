---
---

This is the 3rd preview for .NET Aspire. While the goal is to have monthly releases, we skipped January due to the holidays, so we have a bumper release this month.

.NET Aspire **Preview 3** is now available! Here's a summary of what's new in this preview release:

## Getting .NET Aspire Preview 3

To install or upgrade to .NET Aspire Preview 3, perform the following steps:

1. If on Windows and using Visual Studio to work with .NET Aspire, install/update to the latest [Visual Studio 2022 Preview](https://aka.ms/vspreview) release (17.10.0 Preview 1.0 at the time of writing)
    - Ensure that the ".NET Aspire SDK (Preview)" component is selected under the "Individual components" tab of the Visual Studio installer:

        **Note you must follow the remaining steps in these instructions to update to Preview 3**
        ![Visual Studio Installer component selection screen with the ".NET Aspire SDK (Preview)" component highlighted](media/preview-3/VS-aspire-sdk-component.png)
1. If on Windows and you have Visual Studio 2022 17.8.x installed but intend to only use .NET Aspire via the .NET CLI (`dotnet`), download and install the [.NET 8.0.100 SDK using the standalone installer](https://dotnet.microsoft.com/download/dotnet/thank-you/sdk-8.0.100-windows-x64-installer)
1. If on macOS or Linux, download and install the [.NET 8.0.100 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
1. From a terminal, run the following commands to update the .NET Aspire workload:

    ```dotnetcli
    dotnet workload update
    dotnet workload install aspire
    ```

    *Note that if you had previous preview versions of the .NET Aspire workload already installed, you may see messages informing you that the workload is already installed. These can be safely ignored.*

1. Refer to the docs for [detailed instructions on installing .NET Aspire](https://learn.microsoft.com/dotnet/aspire/fundamentals/setup-tooling)

After updating you can run `dotnet workload list` to see the updated version (note: your version of Visual Studio may differ):

```text
Installed Workload Id      Manifest Version                     Installation Source
----------------------------------------------------------------------------------------------

aspire                     8.0.0-preview.3.XXXX.YY/8.0.100      SDK 8.0.100, VS 17.10.XXXX.YY
```

### Update existing apps

For existing .NET Aspire apps, after performing the above steps to get the latest workload, you'll want to change your package references for any Aspire components. Update **all Aspire package references (hosting and components)** to `8.0.0-preview.3.XXXXX.YY`, for example the hosting would change to:

```xml
<PackageReference Include="Aspire.Hosting" Version="8.0.0-preview.3.XXXX.YY" />
```

All other packages being referenced should be updated to the Preview 3 version as well. If using Visual Studio, you can also use NuGet Package Manager and update all packages being used via the IDE (be sure to check the pre-release checkbox in the UI).

### API changes

Additionally we made some hosting API changes in Preview 3.

- In existing code, `builder.WithServiceBinding(...)` should now be changed to `builder.WithEndpoint(...)` to retain the same behavior (there are similar changes for other resource types as well). The `WithServiceBinding` API has been obsoleted, so it will continue to work, but with a compile warning, and will be removed in the next preview.

## Dashboard updates and refactoring

We've made a major refactoring of the Aspire Dashboard, splitting it out into its own independent component. The dashboard used to be run as part of the host project, and that caused version conflicts between the dashboard and project dependencies. The dashboard is now its own independent executable, that communicates via gRPC to an endpoint to retrieve information about the projects, containers, their status and logs.

This refactoring enables the dashboard to be more easily used for real-time runtime diagnostics after deployment, as hosts can implement the same gRPC contract/endpoint for project and dependency status.

Refactoring should not require any changes to Aspire Projects as there have been no related API changes for the projects themselves. Using the dashboard is unchanged in your development environment.

### Localization and accessibility

We've made small changes across the dashboard to support localization and accessibility. Changes include improving color contrast, adding accessibility attributes, and moving UI text into resource files.

An example of an improvement is how the dashboard indicates trace length. Previously there was a subtle color gradient on each row that didn't meet accessibility requirements. In preview 3 we've replaced the color gradient with a radial progress icon:

!["Trace details page with new radial icon"](media/preview-3/trace-duration.png)

### New resource details view

The resources page grid was bursting at the seams with information. In preview 3 we've introduced a resource details view. The grid continues to display important information, and additional details, including all of a resources properties, are available in the details view.

!["Resource details view"](media/preview-3/resource-details-view.png)

### Telemetry improvements

The dashboard telemetry pages show off a number of improvements:

- **Metrics filter UI**: The metrics filter UI has been redesigned. It's now much faster to update filters when viewing a metrics chart.
- **Histogram metrics count**: There is a new option to view the count of items recorded to a histogram metric.
- **Span events**: OpenTelemetry spans support events. For example, when was the first data received during a database call. Events are available in span details view.

## Component Changes

[Aspire components](https://learn.microsoft.com/dotnet/aspire/fundamentals/components-overview) are a curated suite of NuGet packages specifically selected to facilitate the integration of cloud-native applications with prominent services and platforms. Each component furnishes essential cloud-native functionalities through either automatic provisioning or standardized configuration patterns. .NET Aspire components can be used without an orchestrator project, but they're designed to work best with the .NET Aspire app host.

### Azure OpenAI component

This component enables integration of the Azure OpenAI or OpenAI services from an Aspire project. Assuming you already have an OpenAI service provisioned, you can use OpenAI from your projects.

```csharp
using Azure.AI.OpenAI;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.AddAzureOpenAI("OpenAI");

ar app = builder.Build();

app.MapGet("/chat-complete", async (string request, OpenAIClient client) =>
{
    var options = new ChatCompletionsOptions("gpt-35-turbo", [new ChatRequestUserMessage(request)]);
    var response = await client.GetChatCompletionsAsync(options);

    return response.Value.Choices.FirstOrDefault()?.Message.Content ?? "<No response>";
});

app.MapDefaultEndpoints();

app.Run();
```

For more details see <https://github.com/dotnet/aspire/tree/main/src/Components/Aspire.Azure.AI.OpenAI>

### Component and hosting for Apache Kafka

Guillaume Delahaye ([@g7eg6e](https://github.com/g7ed6e)) has contributed a library that wraps Confluent.Kafka binaries for producing and consuming messages to an Apache Kafka broker. The Aspire integration enables corresponding health check, logging and metrics.

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.AddKafkaProducer<string, string>("kafka");

var app = builder.Build();

app.MapGet("/produce/{message}", async (IProducer<string, string> producer, string message) =>
{
    await producer.ProduceAsync(
        "my-topic",
        new Message<string, string>() { Key = Guid.NewGuid().ToString(), Value = message });
});

app.MapDefaultEndpoints();

app.Run();
```

In addition, `Aspire.Hosting` support for Apache Kafka resources has been added, making it easy to spin up an Apache Kafka container for local development.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var messaging = builder.AddKafka("kafka");
```

For more details see <https://github.com/dotnet/aspire/tree/main/src/Components/Aspire.Confluent.Kafka>

### Component and component for Oracle with EF

A community contribution from Andre Vieira ([@andrevlins](https://github.com/andrevlins)), this provides integration of the Oracle Entity Framework provider into Aspire, enabling connection pooling, health check, logging and telemetry.

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.AddOracleDatabaseDbContext<CatalogDbContext>("catalog");

var app = builder.Build();

app.MapGet("items", async (CatalogDbContext context) =>
{
    return await catalogDbContext.CatalogItems
});

app.MapDefaultEndpoints();

app.Run();
```

In addition, `Aspire.Hosting` support for Oracle resources has been added, making it easier than ever to spin up an Oracle container for local development, or connect to an existing Oracle instance.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var catalogDb = builder.AddOracleDatabase("oracledatabase")
    .AddDatabase("catalog");
```

For more details see <https://github.com/dotnet/aspire/tree/main/src/Components/Aspire.Oracle.EntityFrameworkCore>

### Component and hosting for MySql with EF

Bradley Grainger ([@bgrainger](https://github.com/bgrainger)) has contributed an Aspire component for MySQL named `Aspire.Pomelo.EntityFrameworkCore.MySql`. This wraps the `Pomelo.EntityFrameworkCore.MySql` libray which is an Entity Framework Core provider for MySQL compatible databases. It supports EF Core up to its latest version and uses MySqlConnector for high-performance database server communication. The Aspire integration enables connection pooling, health check, logging and telemetry.

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.AddMySqlDbContext<CatalogDbContext>("catalog");

var app = builder.Build();

app.MapGet("items", async (CatalogDbContext context) =>
{
    return await catalogDbContext.CatalogItems
});

app.MapDefaultEndpoints();

app.Run();
```

You can use the existing `Aspire.Hosting` support for MySQL resources to spin up a MySQL container for local development, or connect to an existing MySQL instance.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var catalogDb = builder.AddMySqlContainer("mysql")
    // Mount the SQL scripts directory into the container so that the init scripts run.
    .WithVolumeMount("../DatabaseContainers.ApiService/data/mysql", "/docker-entrypoint-initdb.d", VolumeMountType.Bind)
    .AddDatabase("catalog");
```

For more details see <https://github.com/dotnet/aspire/tree/main/src/Components/Aspire.Pomelo.EntityFrameworkCore.MySql>

### Hosting support for CosmosDB

As part of preview 3 we worked with the Azure Developer CLI team to introduce support for provisioning Cosmos DB resources. To use Cosmos DB in your App Host you can do the following:

``` csharp
var builder = DistributedApplication.CreateBuilder(args);
var db = builder.AddAzureCosmosDB("cosmos").UseEmulator().AddDatabase("db");
var app = builder.AddProject<Projects.MyApp("app");
```

Inside the entrypoint for the application code you can wire up the Cosmos client using the following:

``` csharp
builder.AddAzureCosmosDB("db", settings =>
{
    settings.IgnoreEmulatorCertificate = true;
});

```

Note: the `UseEmulator()` call `IgnoreEmulatorCertificate` property in the code samples above. This means that when this Aspire application is run we will launch the Cosmos emulator for you and automatically configure the Cosmos client to use the emulator.

We are currently working with the Cosmos team to remove the need to set IgnoreEmulatorCertificate so this customization for settings may not be required in the future.

When deployed, azd will automatically provision the Cosmos DB resource and create the database as defined in the Aspire application model.

### StackExchange.Redis now has logging

We've updated the Aspire component to use the new feature in StackExchange.Redis which supports `Microsoft.Extensions.Logging.ILogger`. Redis logs will now be shown in both the console and structured logs in the dashboard.

## AppModel Changes

### WithServiceBinding has changed to WithEndpoint

The `WithServiceBinding` API was a little unclear as to what it actually did. The API has been deprecated and replaced with `WithEndpoint`. Additional variants for `WithHttpEndpoint` and `WithHttpsEndpoint` that are parameterized for those connection types have been added.

``` csharp
// This will launch the Grafana container image and bind it to a random 
// port (visible in the Dashboard). 
var builder = DistributedApplication.CreateBuilder(args); 
var container = builder.AddContainer("grafana", "grafana/grafana")
                       .WithHttpEndpoint(3000);
builder.Build().Run();
```

### WithEntrypoint is now supported for containers

Using `WithEntrypoint` enables the Entrypoint to be specified for container based resources.

### RedisCommander Resource Extension

[Redis commander](https://github.com/joeferner/redis-commander) is a web management tool for viewing the contents of the Redis Cache. With this change you can use the
`WithRedisCommander` extension to both IResourceBuilder<RedisResource> and IResourceBuilder<RedisContainerResource> which results in a container resource being added to the model (once) and a lifecycle hook which configures it for Redis Commander.

Once added, when run a pre-configured Redis Commander instance is spun up to interactively browse what is in the Redis Cache. It can be accessed via the Aspire Dashboard. For example:

``` csharp
var builder = DistributedApplication.CreateBuilder(args);
builder.AddRedis("myredis1").WithRedisCommander();
builder.AddRedis("myredis2").WithRedisCommander();
```

### PostgreSQL PgAdmin Resource Extension

Andre Vieira has also contributed a resource extension to make it easier to deploy the [pgAdmin](https://www.pgadmin.org/) container to manage your PostgreSQL database instances. Similar to the RedisCommander functionality above, this makes it easy to automatically deploy a pgadmin container when running an Aspire project.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var catalogDb = builder.AddPostgresContainer("postgres")
                    .WithPgAdmin()
                    .AddDatabase("catalogdb");
```

### Model Dapr sidecar as an Aspire resource

Previously Dapr sidecars were modeled as annotations on a parent resource, which indicated the need for a Dapr sidecar started for that resource. The App Model has been extended so that the Dapr sidecar is a _true_ resource, but one exposed _only_ through the existing AddDaprSidecar() API to avoid semantic errors; users cannot create an arbitrary Dapr sidecar resource, only configure the one associated with a given resource. The API allows the existing sidecar options (if any) to be specified, as well as use the existing resource annotation helpers, such as `WithEnvironment()`.

``` csharp
builder.AddProject<Projects.DaprServiceA>("servicea")
       .WithDaprSidecar(
                 sidecarBuilder =>
                 {
                     sidecarBuilder
                         .WithOptions(new DaprSidecarOptions { ... })
                         .WithEnvironment("DAPR_TEST_VAR", "Dapr Test Value");
                 }
       )
```

### Add support for Dapr --runtime-path option

This change enables running Aspire applications with an arbitrary version of the Dapr runtime, such as when you want to test against a locally-built version of Dapr.

``` csharp
builder.AddProject<Projects.DaprServiceB>("serviceb")
       .WithDaprSidecar( new DaprSidecarOptions
            {
               AppId = "serviceb",
               RuntimePath = "/some/other/runtime/folder"
            });
```

## Add support of Orleans in Aspire

[Orleans](https://learn.microsoft.com/dotnet/orleans/overview) is a framework for building robust scalable distributed applications, using the "Actor Model" to seamlessly scaling grains across the available nodes. This preview supports Orleans backed by Azure tables for membership and Azure table and blobs for grain storage. Support for other storage will be added later.

Use in the server and client are easy with:

``` csharp
// server 
builder.AddAspireOrleansServer();

// client
builder.AddAspireOrleansClient();
```

In the AppHost project, Orleans is initialized with:

``` csharp
var storage = builder.AddAzureStorage("storage");
var clusteringTable = storage.AddTables("clustering");
var grainStorage = storage.AddBlobs("grainstate");

var orleans = builder.AddOrleans("my-app")
                     .WithClustering(clusteringTable)
                     .WithGrainStorage("Default", grainStorage);

// Add a server project (also called "silo")
builder.AddProject<Projects.OrleansServer>("silo")
       .WithOrleansServer(orleans);

// Add a project with a reference to the Orleans client
builder.AddProject<Projects.FrontEnd>("frontend")
       .WithOrleansClient(orleans);
```

## Samples

The Aspire samples are located in <https://github.com/dotnet/aspire-samples>, not the main Aspire repo. New samples for Preview 3 include:

- [AspireWithJavaScript](https://github.com/dotnet/aspire-samples/tree/main/samples/AspireWithJavaScript) demonstrates an approach for integrating several Node.js apps into a .NET Aspire application.
- [ClientAppsIntegration](https://github.com/dotnet/aspire-samples/tree/main/samples/ClientAppsIntegration) demonstrates working with client apps such as WinForms, WPF, etc., in a .NET Aspire app, such that the client app is launched along with the AppHost project, can resolve services via service discovery, and logs, traces, and metrics via OpenTelemetry to the dashboard.
- [Persistent Volume Mount](https://github.com/dotnet/aspire-samples/tree/main/samples/VolumeMount) demonstrates how to configure a SQL Server container to use a persistent volume mount in .NET Aspire, so that the data is persisted across app launches. This method can be used to persist data across instances of other container types configured in .NET Aspire apps too, e.g. PostgreSQL, Redis, etc.

## Deployment to Azure using AZD

The [Azure Developer CLI (azd)](https://learn.microsoft.com/azure/developer/azure-developer-cli/) is an open-source tool that makes it easier to get your applications running in the cloud on Azure. We are working to ensure that `azd` enables the fastest and simplest way to get an Aspire app provisioned and deployed to Azure in minutes. In this release we are still primarily targeting Azure Container Apps.

In this release, we added a few new features to better support Aspire deployments:

- CosmosDB - see [section above](#hosting-support-for-cosmosdb)
- AzureSQL

```csharp
var azSqlServer = builder.AddAzureSqlServer("azsql").AddDatabase("azsqldb");
```

## Other

### Add templates for AppHost & ServiceDefaults projects

For those wishing to add Aspire functionality to an existing solution, we now have project templates for the AppHost and ServiceDefault projects. These are the same as what is included in the empty Aspire application template. They can be added through Visual Studio _Add New Project_ or via the command line using:

``` console
dotnet new aspire-apphost
dotnet new aspire-servicedefaults
```

## What's next?

We plan to release a new Aspire preview every month as we work towards a stable 8.0 release during the second quarter of 2024. Check back on the .NET blog for details of future releases or get involved over on the [Aspire project on GitHub](https://github.com/dotnet/aspire). Official samples are available in the [dotnet/aspire-samples repo](https://github.com/dotnet/aspire-samples).

## Summary

Thanks again for your response to .NET Aspire. We're having a lot of fun working hard to make building distributed applications with .NET a great experience and would love for you to [try out Preview 3](#getting-net-aspire-preview-3) and let us know what you think.
