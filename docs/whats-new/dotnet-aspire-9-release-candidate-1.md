---
title: .NET Aspire 9.0 (Release Candidate 1)
description: Learn what's new with .NET Aspire 9.0 (Release Candidate 1).
ms.date: 10/15/2024
zone_pivot_groups: dev-environment
---

# .NET Aspire 9.0 (Release Candidate 1)

.NET Aspire 9.0 Release Candidate 1 (RC1) is the next major release of .NET Aspire; it supports _both_ .NET 8 Long Term Support (LTS) and .NET 9 Standard Term Support. .NET Aspire 9 RC1 addresses some of the most highly requested features and pain points from the community. For more information on .NET version support, see the official [.NET support policy](https://dotnet.microsoft.com/platform/support/policy).

## Upgrade to .NET Aspire 9 RC1

Instructions on how to upgrade are detailed in the following:

- [Install .NET Aspire 9 RC1 templates](#templates)
- [Upgrade an existing project manually](#upgrade-an-existing-project-to-net-aspire-9-rc1-manually)
- [Upgrade an existing project using the Upgrade Assistant](#upgrade-an-existing-project-to-net-aspire-9-rc1-using-the-upgrade-assistant)

### Acquisition

.NET Aspire 9 RC1 makes it simpler to configure your environment to develop .NET Aspire applications. You no longer need a .NET workload. Instead, add the [📦 `Aspire.AppHost.Sdk`](https://www.nuget.org/packages/Aspire.AppHost.Sdk) MSBuild SDK to your project and it will be downloaded via NuGet automatically.

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <Sdk Name="Aspire.AppHost.Sdk" Version="9.0.0-rc.1.24511.1" />

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsAspireHost>true</IsAspireHost>
    <UserSecretsId>0afc20a6-cd99-4bf7-aae1-1359b0d45189</UserSecretsId>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Aspire.Hosting.AppHost" Version="9.0.0-rc.1.24511.1" />
  </ItemGroup>

</Project>
```

For more information, see [.NET Aspire app host project](../fundamentals/app-host-overview.md#app-host-project).

### Templates

.NET Aspire 9 RC1 is moving the contents that used to be installed via the SDK Workload into separate NuGet Packages. This includes the templates for creating new .NET Aspire projects. These templates can now be installed using the [`dotnet new install` command](/dotnet/core/tools/dotnet-new). These can be installed by running the following command:

```dotnetcli
dotnet new install Aspire.ProjectTemplates::9.0.0-rc.1.24511.1
```

> [!TIP]
> If you already have the .NET Aspire workload installed, you need to pass the `--force` flag to overwrite the existing templates. Feel free to uninstall the .NET Aspire workload.

This is the output you should see after installing the templates in a machine that has the .NET Aspire workload installed:

```dotnetcli
> dotnet new install Aspire.ProjectTemplates::9.0.0-rc.1.24511.1 --force
The following template packages will be installed:
   Aspire.ProjectTemplates::9.0.0-rc.1.24511.1

Installing the template package(s) will override the available template package(s).
The following template package(s) are already available:
   Aspire.ProjectTemplates::8.2.1

Success: Aspire.ProjectTemplates::9.0.0-rc.1.24511.1 installed the following templates:
Template Name                    Short Name                                       Language  Tags
-------------------------------  -----------------------------------------------  --------  -------------------------------------------------------
.NET Aspire 8 App Host           aspire-apphost-8                                 [C#]      Common/.NET Aspire/Cloud
.NET Aspire 8 Empty App          aspire-8                                         [C#]      Common/.NET Aspire/Cloud/Web/Web API/API/Service
.NET Aspire 8 Service Defaults   aspire-servicedefaults-8                         [C#]      Common/.NET Aspire/Cloud/Web/Web API/API/Service
.NET Aspire 8 Starter App        aspire-starter-8                                 [C#]      Common/.NET Aspire/Blazor/Web/Web API/API/Service/Cloud
.NET Aspire 8 Test Project (...  aspire-mstest-8                                  [C#]      Common/.NET Aspire/Cloud/Web/Web API/API/Service/Test
.NET Aspire 8 Test Project (...  aspire-nunit-8                                   [C#]      Common/.NET Aspire/Cloud/Web/Web API/API/Service/Test
.NET Aspire 8 Test Project (...  aspire-xunit-8                                   [C#]      Common/.NET Aspire/Cloud/Web/Web API/API/Service/Test
.NET Aspire 9 App Host           aspire-apphost,aspire-apphost-9                  [C#]      Common/.NET Aspire/Cloud
.NET Aspire 9 Empty App          aspire,aspire-9                                  [C#]      Common/.NET Aspire/Cloud/Web/Web API/API/Service
.NET Aspire 9 Service Defaults   aspire-servicedefaults,aspire-servicedefaults-9  [C#]      Common/.NET Aspire/Cloud/Web/Web API/API/Service
.NET Aspire 9 Starter App        aspire-starter,aspire-starter-9                  [C#]      Common/.NET Aspire/Blazor/Web/Web API/API/Service/Cloud
.NET Aspire 9 Test Project (...  aspire-mstest,aspire-mstest-9                    [C#]      Common/.NET Aspire/Cloud/Web/Web API/API/Service/Test
.NET Aspire 9 Test Project (...  aspire-nunit,aspire-nunit-9                      [C#]      Common/.NET Aspire/Cloud/Web/Web API/API/Service/Test
.NET Aspire 9 Test Project (...  aspire-xunit,aspire-xunit-9                      [C#]      Common/.NET Aspire/Cloud/Web/Web API/API/Service/Test
```

Now you have the .NET Aspire 9 RC1 templates installed on your machine.

:::zone pivot="dotnet-cli,vscode"
You can create a new .NET Aspire 9 RC1 project by running the following command:

```dotnetcli
dotnet new aspire-starter
```

If you need to target .NET 8, you can do so by passing the `-f net8.0` flag:

```dotnetcli
dotnet new aspire-starter -f net8.0
```

:::zone-end
:::zone pivot="visual-studio"

Visual Studio 2022 17.12 or higher supports .NET Aspire 9.

You can create a new .NET Aspire 9 RC1 project by selecting one of the .NET Aspire 9 templates. You can use Aspire 9 to target either .NET 9.0 or .NET 8.0.

:::image type="content" source="media/create-new-project-vs.png" lightbox="media/create-new-project-vs.png" alt-text="Create New Project":::

:::image type="content" source="media/create-aspire9-starterapp.png" lightbox="media/create-aspire9-starterapp.png" alt-text="Create New Aspire 9 Starter App":::

If you want to use the older .NET Aspire 8 project templates, those are still available and you can target .NET 8.0.

:::image type="content" source="media/create-aspire8-starterapp.png" lightbox="media/create-aspire8-starterapp.png" alt-text="Create New Aspire 8 Starter App":::

:::zone-end

### Upgrade an existing project to .NET Aspire 9 RC1 manually

In order to upgrade an existing project to .NET Aspire 9 RC1, you need to update the version of the `Aspire.Hosting.AppHost` package in your project file. In addition to that, you need to add an SDK reference to the `Aspire.AppHost.Sdk` package. Here's an example of how to upgrade an existing project to .NET Aspire 9 RC1:

```diff
<Project Sdk="Microsoft.NET.Sdk">
+
+  <Sdk Name="Aspire.AppHost.Sdk" Version="9.0.0-rc.1.24511.1" />

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsAspireHost>true</IsAspireHost>
    <UserSecretsId>84e68884-8bc6-4b5b-9880-9544745c89e1</UserSecretsId>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\MyApiService\MyApiService.csproj" />
    <ProjectReference Include="..\MyWebFrontend\MyWebFrontend.csproj" />
  </ItemGroup>

  <ItemGroup>
-    <PackageReference Include="Aspire.Hosting.AppHost" Version="8.2.1" />
+    <PackageReference Include="Aspire.Hosting.AppHost" Version="9.0.0-rc.1.24511.1" />
  </ItemGroup>

</Project>
```

That's it! You've successfully upgraded your project to .NET Aspire 9 RC1. If you're using Visual Studio, it's recommended to upgrade to version 17.9 Preview 3 or later in order to take advantage of the new features and improvements for .NET Aspire 9 RC1.

### Upgrade an existing project to .NET Aspire 9 RC1 using the Upgrade Assistant

For this release, we're introducing .NET Aspire support to the [Upgrade Assistant](/dotnet/core/porting/upgrade-assistant-overview). The Upgrade Assistant is a tool that helps you upgrade your .NET projects to the latest .NET version. Now you can also use the Upgrade Assistant to upgrade your .NET Aspire projects to 9 RC1.

To start the upgrade process, right-click your _*.AppHost_ project in Visual Studio and select the option `Upgrade`.

:::image type="content" source="media/upgrade-assistant-1.png" lightbox="media/upgrade-assistant-1.png" alt-text="Upgrade using upgrade assistant":::

If you don't yet have the Upgrade Assistant installed, then Visual Studio guides you to the steps of how to install the extension. Once you have it installed, you see the following option when you select the `Upgrade` option on your AppHost project:

:::image type="content" source="media/upgrade-assistant-2.png" lightbox="media/upgrade-assistant-2.png" alt-text="Upgrade assistant options":::

This will then do some analysis in your project and provide you with a report similar to the following detailing what will be updated. All you need to do now is to select the `Upgrade Selection` button to start the upgrade process:

:::image type="content" source="media/upgrade-assistant-confirmation.png" lightbox="media/upgrade-assistant-confirmation.png" alt-text="Upgrade confirmation":::

That's it! You have successfully upgraded your project to .NET Aspire 9 RC1 using the Upgrade Assistant.

## Dashboard

The [.NET Aspire dashboard](../fundamentals/dashboard/overview.md) continues to improve with each release.

### Manage resource lifecycle

The most requested feature for the dashboard is to manage the life-cycles of your orchestrated named resources. Specifically, the ability to stop, start, and restart resources.

<!-- > [!VIDEO 3ad0b6db-7027-4ac0-b42e-03cb60644e5e] -->

This feature works for projects, containers, and executables. It enables restarting individual resources without having to restart the entire app host. For projects, if the debugger is attached, it's reattached on restart.

### Mobile and responsive support

The .NET Aspire dashboard is now mobile-friendly, responsively adapting to a wide range of screen sizes and enabling on-the-go management of deployed Aspire applications. Additional accessibility improvements have been made, including the display of settings and content overflow on mobile.

### Sensitive properties, volumes, and health checks in resource details

The display of resource details has seen several improvements:

- Properties can be marked as sensitive, automatically masking them in the dashboard UI. This security feature helps to avoid accidentally disclosing keys or passwords when screen sharing the dashboard with other people. For example, container arguments could pass sensitive information and so are masked by default.

- Configured container volumes are listed in resource details.

- .NET Aspire 9 RC1 adds support for health checks. Detailed information about these checks can now be viewed in the resource details pane, showing why a resource might be marked as unhealthy or degraded. Find out more about health checks [here](#resource-health-checks).

### Colorful console log

[ANSI escape codes](https://wikipedia.org/wiki/ANSI_escape_code) format text in terminals by controlling colors (foreground and background) and styles like bold, underline, and italics. Previously, the dashboard's console logs page could only render one ANSI escape code at a time, failing when multiple codes were combined. For example, it could display red text, but not text that was both red and bold.

A community contribution from [@mangeg](https://github.com/mangeg) improved support for ANSI escape codes and removed this limitation.

:::image type="content" source="media/console-logs-ansi-text-format.png" lightbox="media/console-logs-ansi-text-format.png" alt-text="Colorful console logs":::

Another improvement to console logs is hiding unsupported escape codes. Codes that aren't related to displaying text, such as positioning the cursor or communicating with the operating system don't make sense in this UI, and are hidden.

## Telemetry

[Telemetry](../fundamentals/telemetry.md) remains a vital aspect of .NET Aspire. In this release we're introducing many new features to the Telemetry service.

### Improve telemetry filtering

Traces can now be filtered with attribute values. For example, if you only want to view traces for one endpoint in your app, the `http.route` attribute on HTTP requests can be filtered to a specified value.

Telemetry filtering also supports autocomplete of existing values. The **Added filter** dialog provides a combo box for selecting from values that dashboard has received. This feature makes it much easier to filter to real data and helps avoid typos by entered a value yourself.

<!-- > [!VIDEO 1e158316-4048-4f16-9afa-883c949f8063] -->

### Combine telemetry from multiple resources

When a resource has multiple replicas, you can now filter telemetry to view data from all instances at once. Select the parent resource, labeled `(application)`.

:::image type="content" source="media/telemetry-resource-filter.png" lightbox="media/telemetry-resource-filter.png" alt-text="Filter by all instances of a resource":::

### Browser telemetry support

The dashboard now supports OpenTelemetry Protocol (OTLP) over HTTP and cross-origin resource sharing (CORS). These features unlock the ability to send OpenTelemetry from browser apps to the .NET Aspire dashboard.

For example, a browser-based single page app (SPA) can configure the [JavaScript OTEL SDK](https://opentelemetry.io/docs/languages/js/getting-started/browser/) to send structured logs, traces, and metrics created in the browser to the dashboard. Browser telemetry is displayed alongside server telemetry.

:::image type="content" source="media/dashboard-browser-telemetry.png" lightbox="media/dashboard-browser-telemetry.png" alt-text="Trace detail page with browser telemetry":::

For more information on configuring browser telemetry, see [Enable browser telemetry](../fundamentals/dashboard/enable-browser-telemetry.md) documentation.

## App Host (Orchestration)

The [.NET Aspire app host](../fundamentals/app-host-overview.md) is one of the most important features of .NET Aspire. In this release we're introducing many new features to the app host.

### Waiting for dependencies

You can now specify that a resource should wait for another resource before starting. This can help avoid connection errors during startup by only starting resources when their dependencies are ready.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var rabbit = builder.AddRabbitMQ("rabbit");

builder.AddProject<Projects.WebApplication1>("api")
       .WithReference(rabbit)
       .WaitFor(rabbit); // Don't start "api" until "rabbit" is ready...

builder.Build().Run();
```

When the app host starts, it waits for the `rabbit` resource to be ready before starting the `api` resource.

<!-- > [!VIDEO d3636113-9e0d-4796-879c-4b80275c644d] -->

There are two methods exposed to wait for a resource:

- `WaitFor`: Wait for a resource to be ready before starting another resource.
- `WaitForCompletion`: Wait for a resource to complete before starting another resource.

#### Resource health checks

`WaitFor` uses health checks to determine if a resource is ready. If a resource doesn't have any health checks, the app host waits for the resource to be in the "Running" state before starting the dependent resource.

For resources that expose HTTP endpoints, you can easily add a health check that polls a specific path for an HTTP 200 response.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var catalogApi = builder.AddContainer("catalog-api", "catalog-api")
                        .WithHttpEndpoint(targetPort: 8080)
                        .WithHttpHealthCheck("/health");

builder.AddProject<Projects.WebApplication1>("store")
       .WithReference(catalogApi.GetEndpoint("http"))
       .WaitFor(catalogApi);

builder.Build().Run();
```

The preceding example adds a health check to the `catalog-api` resource. The app host waits for the health check to return a healthy status before starting the `store` resource. It determines that the resource is ready when the the `/health` endpoint returns an HTTP 200 status code.

While `store` is waiting for `catalog-api` to become healthy, the resources in the dashboard appear as:

:::image type="content" source="media/waiting-for-unhealthy-resource.png" lightbox="media/waiting-for-unhealthy-resource.png" alt-text="Waiting for an unhealthy resource before starting":::

The app host's health check mechanism builds upon the `IHealthChecksBuilder` implementation from the `Microsoft.Extensions.Diagnostics.HealthChecks` namespace.

Health checks report data, which is displayed in the dashboard:

:::image type="content" source="media/health-check-details.png" lightbox="media/health-check-details.png" alt-text="Health check details in the dashboard's resource details view":::

Creating a custom health check is straightforward. Start by defining the health check, then associate its name with any resources it applies to.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var healthyAfter = DateTime.Now.AddSeconds(20);

builder.Services.AddHealthChecks().AddCheck(
    "delay20secs",
    () => DateTime.Now > healthyAfter 
        ? HealthCheckResult.Healthy() 
        : HealthCheckResult.Unhealthy()
    );

var cache = builder.AddRedis("cache")
                   .WithHealthCheck("delay20secs");

builder.AddProject<Projects.MyApp>("myapp")
       .WithReference(cache).WaitFor(cache);
```

The above example adds a health check that considers resources unhealthy until 20 seconds after the app host starts, after which it reports them as healthy. This health check is added to the `cache` resource. As `myapp` waits for `cache`, `myapp` won't start until those 20 seconds have elapsed.

The `AddCheck` and `WithHealthCheck` methods provide a simple mechanism to create health checks and associate them with specific resources.

### Persistent containers

The app host now supports persistent containers. This is useful when you want to keep the container running even after the app host has stopped. These containers won't be stopped until they're stopped manually using the container runtime.

To do this, call the `WithLifetime` method and pass in `ContainerLifetime.Persistent`:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var queue = builder.AddRabbitMQ("rabbit")
                   .WithLifetime(ContainerLifetime.Persistent);

builder.AddProject<Projects.WebApplication1>("api")
       .WithReference(queue).WaitFor(queue);

builder.Build().Run();
```

The dashboard shows persistent containers with a pin icon:

:::image type="content" source="media/persistent-container.png" lightbox="media/persistent-container.png" alt-text="Persistent containers":::

After the app host has stopped, the container will continue to run:

:::image type="content" source="media/persistent-container-docker-desktop.png" lightbox="media/persistent-container-docker-desktop.png" alt-text="Docker desktop showing RabbitMQ.":::

The container persistence mechanism attempts to identify when you may wish to recreate the container. For example if the environment for the container changes then the container will be restarted so that you don't need to manually stop the container if you have changed the input configuration for the resource.

### Resource commands

The app host supports adding custom commands to resources. This is useful when you want to run custom commands on a resource.

> [!IMPORTANT]
> These .NET Aspire dashboard commands are only available when running the dashboard locally. They are not available when running the dashboard in Azure Container Apps.

The following example uses an extension method to add some additional commands.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
                   .WithClearCommand();

builder.AddProject<Projects.WebApplication1>("api")
       .WithReference(cache)
       .WaitFor(cache);

builder.Build().Run();
```

Custom commands can be added by calling the `WithCommand*` method and passing in the command to run:

```csharp
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

public static class RedisCommandExtensions
{
    public static IResourceBuilder<RedisResource> WithClearCommand(
        this IResourceBuilder<RedisResource> builder)
    {
        builder.WithCommand(
            "clear-cache",
            "Clear Cache",
            async context =>
            {
                var redisConnectionString = await builder.Resource.GetConnectionStringAsync() ??
                    throw new InvalidOperationException(
                        "Unable to get the Redis connection string.");
                
                using var connection = ConnectionMultiplexer.Connect(redisConnectionString);

                await connection.GetDatabase().ExecuteAsync("FLUSHALL");

                return CommandResults.Success();
            },
            context =>
            {
                if (context.ResourceSnapshot.HealthStatus is HealthStatus.Healthy)
                {
                    return ResourceCommandState.Enabled;
                }

                return ResourceCommandState.Disabled;
            });
        return builder;
    }
}
```

These commands can be run from the dashboard:

:::image type="content" source="media/clear-cache-command.png" lightbox="media/clear-cache-command.png" alt-text="Clear cache command on dashboard":::

<!-- > [!VIDEO 7fc32183-7342-4e79-a30a-c977e5b1d005] -->

### Container networking

The app host now adds all containers to a common network named `default-aspire-network`. This is useful when you want to communicate between containers without going through the host network.

This also makes it easier to migrate from docker compose to the app host, as containers can communicate with each other using the container name.

### Eventing model

The eventing model allows developers to hook into the lifecycle of the application and resources. This is useful for running custom code at specific points in the application lifecycle. There are various ways to subscribe to events, including global events and per-resource events.

**Global events:**

- `BeforeStartEvent`: An event that is triggered before the application starts. This is the last place that changes to the app model will be observed. This runs in both Run and Publish modes. This is a blocking event, meaning that the application won't start until all handlers have completed.
- `AfterResourcesCreatedEvent`: An event that is triggered after the resources have been created. This runs in Run mode only.
- `AfterEndpointsAllocatedEvent`: An event that is triggered after the endpoints have been allocated for all resources. This runs in Run mode only.

**Per-resource events:**

- `BeforeResourceStartedEvent`: An event that is triggered before a single resource starts. This runs in Run mode only. This is a blocking event, meaning that the resource won't start until all handlers have completed.
- `ConnectionStringAvailableEvent`: An event that is triggered when a connection string is available for a resource. This runs in Run mode only.
- `ResourceReadyEvent`: An event that is triggered when a resource is ready to be used. This runs in Run mode only.

Here are some code examples of subscribing to events:

- Subscribe to a global event.

  ```csharp
  var builder = DistributedApplication.CreateBuilder(args);

  // Subscribe to a global event
  builder.Eventing.Subscribe<BeforeStartEvent>((e, token) =>
  {
      Console.WriteLine("Before the application starts!");

      return Task.CompletedTask;
  });

  builder.Build().Run();
  ```
  
- Subscribe to a per-resource event for a specific resource.

  ```csharp
  var builder = DistributedApplication.CreateBuilder(args);

  var cache = builder.AddContainer("cache", "redis")
                      .WithEndpoint(targetPort: 6379);

  cache.Subscribe<ResourceReadyEvent>(cache.Resource, (e, token) =>
  {
      Console.WriteLine($"Resource {e.Resource.Name} is ready!");

      return Task.CompletedTask;
  });
  ```

- Subscribe to a per-resource event for *all* resources.

  ```csharp
  var builder = DistributedApplication.CreateBuilder(args);

  var cache = builder.AddContainer("cache", "redis")
                      .WithEndpoint(targetPort: 6379);

  // Subscribe to a per-resource event for *all* resources
  builder.Eventing.Subscribe<BeforeResourceStartedEvent>((e, token) =>
  {
      Console.WriteLine($"Before {e.Resource.Name}");

      return Task.CompletedTask;
  });

  builder.Build().Run();
  ```

- Run custom code before a resource starts and when a resource is ready.
  
  ```csharp
  var builder = DistributedApplication.CreateBuilder(args);

  var queue = builder.AddRabbitMQ("rabbit");

  // Subscribe to a per-resource event for *all* resources
  builder.Eventing.Subscribe<BeforeResourceStartedEvent>(queue.Resource, (e, token) =>
  {
      Console.WriteLine($"Before {e.Resource.Name}");

      return Task.CompletedTask;
  });

  builder.Eventing.Subscribe<ResourceReadyEvent>(queue.Resource, (e, token) =>
  {
      Console.WriteLine($"Resource {e.Resource.Name} is ready");
      return Task.CompletedTask;
  });

  builder.Build().Run();
  ```

## Integrations

.NET Aspire continues to add integrations that make it easy to get started with your favorite services and tools. For more information, see [.NET Aspire integrations overview](../fundamentals/integrations-overview.md).

### Redis Insight

A new extension method has been added to support starting up Redis Insight on a Redis resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);
builder.AddRedis("redis")
       .WithRedisInsight(); // Starts a Redis Insight container image
                            // that is pre-configured to work with the
                            // Redis instance.
```

The `WithRedisInsight` extension method can be applied to multiple Redis resources and they'll each be visible on the Redis Insight dashboard.

:::image type="content" source="media/redis-insight.png" lightbox="media/redis-insight.png" alt-text="Redis Insight dashboard showing multiple Redis instances":::

For more information, see [Add Redis resource with Redis Insights](../caching/stackexchange-redis-integration.md?pivots=redis#add -redis-resource-with-redis-insights).

### OpenAI (Preview)

Starting with .NET Aspire 9 RC1, an additional OpenAI integration is available which allows to use the latest official OpenAI dotnet library directly. The client integration registers the [OpenAIClient](https://github.com/openai/openai-dotnet?tab=readme-ov-file#using-the-openaiclient-class) as a singleton service in the service collection. The client can be used to interact with the OpenAI REST API.

- [📦 Aspire.OpenAI (Preview)](https://www.nuget.org/packages/Aspire.OpenAI/9.0.0-preview.4.24511.1)

Moreover, the already available [.NET Aspire Azure OpenAI integration](../azureai/azureai-openai-integration.md) was improved to provide a flexible way to configure an `OpenAIClient` for either an Azure AI OpenAI service or a dedicated OpenAI REST API one with the new `AddOpenAIClientFromConfiguration` builder method. The following example will detect if the connection string is for an Azure AI OpenAI service and register the most appropriate `OpenAIClient` instance automatically.

```csharp
builder.AddOpenAIClientFromConfiguration("openai");
```

For instance, if the `openai` connection looked like `Endpoint=https://{account}.azure.com;Key={key};` it would guess it can register an Azure AI OpenAI client because of the domain name. Otherwise a common `OpenAIClient` would be used.

Read [Azure-agnostic client resolution](https://github.com/dotnet/aspire/blob/release/9.0-rc1/src/Components/Aspire.Azure.AI.OpenAI/README.md#azure-agnostic-client-resolution) for more details.

### MongoDB

Added support for specifying the MongoDB username and password when using the `AddMongoDB` extension method. If not specified, a random username and password will be generated but can be manually specified using parameter resources.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("mongousername");
var password = builder.AddParameter("mongopassword", secret: true);

var db = builder.AddMongo("db", username, password);
```

### Azure

The following sections describe Azure improvements added in .NET Aspire 9.

#### Azure resource customization

In .NET Aspire 8, customizing Azure resources was marked experimental because the underlying `Azure.Provisioning` libraries were new and gathering feedback before they could be marked stable. In .NET Aspire 9 these APIs have been updated and will remove the experimental attribute.

**Azure Resource Naming Breaking Change**

As part of the update to the `Azure.Provisioning` libraries, the default naming scheme for Azure resources was updated with better support for various naming policies. However, this update resulted in a change to how resources are named. The new naming policy might result in the existing Azure resources being abandoned and new Azure resources being created, after updating your .NET Aspire application from 8 to 9. To keep using the same naming policies from .NET Aspire 8, you can add the following code to your AppHost _Program.cs_:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.Services.Configure<AzureResourceOptions>(options =>
{
    options.ProvisioningContext.PropertyResolvers.Insert(0, new AspireV8ResourceNamePropertyResolver());
});
```

#### Azure SQL, PostgreSQL, and Redis Update

Azure SQL, PostgreSQL, and Redis resources are different than other Azure resources because there are local container resources for these technologies. In .NET Aspire 8, in order to create these Azure resources you needed to start with a local container resource and then either "As" or "PublishAs" it to an Azure resource. This design introduced problems and didn't fit with other APIs.

For example, you might have this code in .NET Aspire 8:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
                 .PublishAsAzureSqlDatabase();
var pgsql = builder.AddPostgres("pgsql")
                   .PublishAsAzurePostgresFlexibleServer();
var cache = builder.AddRedis("cache")
                   .PublishAsAzureSqlDatabase();
```

In .NET Aspire 9 these APIs have been obsoleted and a new API pattern implemented:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddAzureSqlServer("sql")
                 .RunAsContainer();
var pgsql = builder.AddAzurePostgresFlexibleServer("pgsql")
                   .RunAsContainer();
var cache = builder.AddAzureRedis("cache")
                   .RunAsContainer();
```

##### Microsoft Entra ID by default

In order to make .NET Aspire applications more secure, Azure Database for PostgreSQL and Azure Cache for Redis resources were updated to use Microsoft Entra ID by default. This requires changes to applications that need to connect to these resources. See the following for updating applications to use Microsoft Entra ID to connect to these resources:

- [Azure Database for PostgreSQL](https://devblogs.microsoft.com/dotnet/using-postgre-sql-with-dotnet-and-entra-id/)
- [Azure Cache for Redis](https://github.com/Azure/Microsoft.Azure.StackExchangeRedis)

The following is example code for how to configure your application to connect to the Azure resource using Microsoft Entra ID:

**Azure Database for PostgreSQL**

```csharp
using Azure.Core;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.AddNpgsqlDataSource("db1", configureDataSourceBuilder: dataSourceBuilder =>
{
    if (string.IsNullOrEmpty(dataSourceBuilder.ConnectionStringBuilder.Password))
    {
        dataSourceBuilder.UsePeriodicPasswordProvider(async (_, ct) =>
        {
            var credentials = new DefaultAzureCredential();
            var token = await credentials.GetTokenAsync(new TokenRequestContext(["https://ossrdbms-aad.database.windows.net/.default"]), ct);
            return token.Token;
        }, TimeSpan.FromHours(24), TimeSpan.FromSeconds(10));
    }
});
```

**Azure Cache for Redis**

```csharp
using Azure.Identity;
using StackExchange.Redis;
using StackExchange.Redis.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

var azureOptionsProvider = new AzureOptionsProvider();
var configurationOptions = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("cache") ?? throw new InvalidOperationException("Could not find a 'cache' connection string."));
if (configurationOptions.EndPoints.Any(azureOptionsProvider.IsMatch))
{
    await configurationOptions.ConfigureForAzureWithTokenCredentialAsync(new DefaultAzureCredential());
}

builder.AddRedisClient("cache", configureOptions: options =>
{
    options.Defaults = configurationOptions.Defaults;
});
```

If you need to use password or access key authentication (not recommended), you can opt-in with the following code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var pgsql = builder.AddAzurePostgresFlexibleServer("pgsql")
                   .WithPasswordAuthentication();
var cache = builder.AddAzureRedis("cache")
                   .WithAccessKeyAuthentication();
```

#### Azure Functions Support (Preview)

Support for [Azure Functions](/azure/azure-functions/functions-overview?pivots=programming-language-csharp) is one of the most widely requested features on the .NET Aspire issue tracker and we're excited to introduce preview support for it in this release. To demonstrate this support, let's use .NET Aspire to create and deploy a webhook.

To get started, create a new Azure Functions project using the **Visual Studio New Project** dialog. When prompted, select the **Enlist in Aspire orchestration** checkbox when creating the project.

:::image type="content" source="media/functions-step-1.gif" lightbox="media/functions-step-1.gif" alt-text="Create new .NET Aspire Azure Functions project.":::

In the app host project, observe that there's a `PackageReference` to the new [📦 Aspire.Hosting.Azure.Functions](https://www.nuget.org/packages/Aspire.Hosting.Azure.Functions) NuGet package:

```xml
<ItemGroup>
    <PackageReference Include="Aspire.Hosting.AppHost" Version="9.0.0-rc.1.24511.1" />
    <PackageReference Include="Aspire.Hosting.Azure.Functions" Version="9.0.0-preview.5.24513.1" />
</ItemGroup>
```

This package provides an `AddAzureFunctionsProject` API that can be invoked in the app host to configure Azure Functions projects within an .NET Aspire host:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureFunctionsProject<Projects.PigLatinApp>("piglatinapp");

builder.Build().Run();
```

In this example, the webhook is responsible for translating an input string into Pig Latin. Update the contents of our trigger with the following code:

```csharp
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text;
using FromBodyAttribute = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;

namespace PigLatinApp;

public class Function1(ILogger<Function1> logger)
{
    public record InputText(string Value);
    public record PigLatinText(string Value);

    [Function("Function1")]
    public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
        [FromBody] InputText inputText)
    {
        logger.LogInformation("C# HTTP trigger function processed a request.");

        var result = TranslateToPigLatin(inputText.Value);

        return new OkObjectResult(new PigLatinText(result));
    }

    private static string TranslateToPigLatin(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        var words = input.Split(' ');
        StringBuilder pigLatin = new();

        foreach (string word in words)
        {
            if (IsVowel(word[0]))
            {
                pigLatin.Append(word + "yay ");
            }
            else
            {
                int vowelIndex = FindFirstVowelIndex(word);
                if (vowelIndex is -1)
                {
                    pigLatin.Append(word + "ay ");
                }
                else
                {
                    pigLatin.Append(
                        word.Substring(vowelIndex) + word.Substring(0, vowelIndex) + "ay ");
                }
            }
        }

        return pigLatin.ToString().Trim();
    }

    private static int FindFirstVowelIndex(string word)
    {
        for (var i = 0; i < word.Length; i++)
        {
            if (IsVowel(word[i]))
            {
                return i;
            }
        }
        return -1;
    }

    private static bool IsVowel(char c) =>
        char.ToLower(c) is 'a' or 'e' or 'i' or 'o' or 'u';
}
```

Set a breakpoint on the first line of the `Run` method and press <kbd>F5</kbd> to start the Functions host. Once the .NET Aspire dashboard launches, you observe the following:

:::image type="content" source="media/functions-dashboard-screenshot.png" lightbox="media/functions-dashboard-screenshot.png" alt-text="Screenshot of the .NET Aspire running with an Azure Function app.":::

.NET Aspire has:

- Configured an emulated Azure Storage resource to be used for bookkeeping by the host.
- Launched the Functions host locally with the target as the Functions project registered.
- Wired the port defined in _launchSettings.json_ of the functions project for listening.

Use your favorite HTTP client of choice to send a request to the trigger and observe the inputs bound from the request body in the debugger.

## [Unix](#tab/unix)

```bash
curl --request POST \
  --url http://localhost:7282/api/Function1 \
  --header 'Content-Type: application/json' \
  --data '{
  "value": "Welcome to Azure Functions"
}'
```

## [Windows](#tab/windows)

```powershell
curl --request POST `
  --url http://localhost:7282/api/Function1 `
  --header 'Content-Type: application/json' `
  --data '{
  "value": "Welcome to Azure Functions"
}'
```

---

:::image type="content" source="media/functions-debug-screenshot.png" lightbox="media/functions-debug-screenshot.png" alt-text="Screenshot of the .NET Aspire dashboard: Debugging an Azure Function app.":::

Now you're ready to deploy our application to Azure Container Apps (ACA). Deployment currently depends on preview builds of Azure Functions Worker and Worker SDK packages. If necessary, upgrade the versions referenced in the Functions project:

```xml
<ItemGroup>
    <PackageReference Include="Microsoft.Azure.Functions.Worker" Version="2.0.0-preview1" />
    <PackageReference Include="Microsoft.Azure.Functions.Worker.Sdk" Version="2.0.0-preview2" />
</ItemGroup>
```

You also need to expose a public endpoint for our Azure Functions project so that requests can be sent to our HTTP trigger:

```csharp
builder.AddAzureFunctionsProject<Projects.PigLatinApp>("piglatinapp")
       .WithExternalHttpEndpoints();
```

To deploy the application with [the `azd` CLI](/azure/developer/azure-developer-cli/install-azd), you'll need get the latest version first. To install the latest version, you'll see a warning if your version is out of date. Follow the instructions to update to the latest version.

After it's installed, navigate to the folder containing the app host project and run `azd init`:

```azdeveloper
$ azd init

Initializing an app to run on Azure (azd init)

? How do you want to initialize your app? Use code in the current directory

  (✓) Done: Scanning app code in current directory

Detected services:

  .NET (Aspire)
  Detected in: ./PigLatinApp/PigLatinApp.AppHost/PigLatinApp.AppHost.csproj

azd will generate the files necessary to host your app on Azure using Azure Container Apps.

? Select an option Confirm and continue initializing my app
? Enter a new environment name: azfunc-piglatin

Generating files to run your app on Azure:

  (✓) Done: Generating ./azure.yaml
  (✓) Done: Generating ./next-steps.md

SUCCESS: Your app is ready for the cloud!
```

Then, deploy the application by running `azd up`:

```azdeveloper
$ azd up 
? Select an Azure Subscription to use: 130. [redacted]
? Select an Azure location to use: 50. (US) West US 2 (westus2)

Packaging services (azd package)


Provisioning Azure resources (azd provision)
Provisioning Azure resources can take some time.

Subscription: [redacted]
Location: West US 2

  You can view detailed progress in the Azure Portal:
  [redacted]

  (✓) Done: Resource group: rg-azfunc-piglatin (967ms)
  (✓) Done: Container Registry: [redacted] (13.316s)
  (✓) Done: Log Analytics workspace: [redacted] (16.467s)
  (✓) Done: Container Apps Environment: [redacted] (1m35.531s)
  (✓) Done: Storage account: [redacted] (21.37s)

Deploying services (azd deploy)

  (✓) Done: Deploying service piglatinapp
  - Endpoint: {{endpoint-url}}

  Aspire Dashboard: {{dashboard-url}}
```

Finally, test your deployed Functions application using your favorite HTTP client:

## [Unix](#tab/unix)

```bash
curl --request POST \
  --url {{endpoint-url}}/api/Function1 \
  --header 'Content-Type: application/json' \
  --data '{
  "value": "Welcome to Azure Functions"
}'
```

## [Windows](#tab/windows)

```powershell
curl --request POST `
  --url {{endpoint-url}}/api/Function1 `
  --header 'Content-Type: application/json' `
  --data '{
  "value": "Welcome to Azure Functions"
}'
```

---

Support for Azure Functions in .NET Aspire is still in preview with support for a limited set of triggers including:

- [HTTP triggers](/azure/azure-functions/functions-triggers-bindings?pivots=programming-language-csharp#supported-bindings)
- [Azure Storage Queue triggers](/azure/azure-functions/functions-bindings-storage-queue?pivots=programming-language-csharp)
- [Azure Storage Blob triggers](/azure/azure-functions/functions-bindings-storage-blob?pivots=programming-language-csharp)
- [Azure Service Bus triggers](/azure/azure-functions/functions-bindings-service-bus?pivots=programming-language-csharp)
- [Azure Event Hubs triggers](/azure/azure-functions/functions-bindings-event-hubs?pivots=programming-language-csharp)

For the latest information on features support by the Azure Functions integration, see [the tracking issue](https://github.com/dotnet/aspire/issues/920).

#### Customization of Azure Container Apps

One of the most requested features is the ability to customize the Azure Container Apps that are created by the app host without dropping to bicep. This is now possible by using the `PublishAsAzureContainerApp` method in the `Aspire.Hosting.Azure.AppContainers` namespace. This method customizes the Azure Container App definition that's created by the app host.

Add the package reference to your project file:

```xml
<ItemGroup>
  <PackageReference Include="Aspire.Hosting.Azure.AppContainers"
                    Version="9.0.0-rc.1.24511.1" />
</ItemGroup>
```

The following example demonstrates how to scale an Azure Container App to zero (`0`) replicas:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddAzurePostgresFlexibleServer("pg")
                .RunAsContainer()
                .AddDatabase("db");

// Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable AZPROVISION001

builder.AddProject<Projects.WebApplication1>("api")
       .WithReference(db)
       .PublishAsAzureContainerApp((module, containerApp) =>
       {
           // Scale to 0
           containerApp.Template.Value!.Scale.Value!.MinReplicas = 0;
       });

#pragma warning restore AZPROVISION001

builder.Build().Run();
```

The preceding code example defers generation of the Azure Container App definition to the app host. This allows you to customize the Azure Container App definition without needing to run `azd infra synth` and unsafely modifying the generated bicep files.
