---
title: What's new in Aspire 9.5
description: Learn what's new in Aspire 9.5.
ms.date: 09/25/2025
---

# What's new in Aspire 9.5

📢 Aspire 9.5 is the next minor version release of Aspire. It supports:

- .NET 8.0 Long Term Support (LTS).
- .NET 9.0 Standard Term Support (STS).
- .NET 10.0 Release Candidate (RC) 1.

If you have feedback, questions, or want to contribute to Aspire, collaborate with us on [:::image type="icon" source="../media/github-mark.svg" border="false"::: GitHub](https://github.com/dotnet/aspire) or join us on [:::image type="icon" source="../media/discord-icon.svg" border="false"::: Discord](https://aka.ms/aspire-discord) to chat with the team and other community members.

It's important to note that Aspire releases are out-of-band from .NET releases. While major versions of Aspire align with major .NET versions, minor versions are released more frequently. For more information on .NET and Aspire version support, see:

- [.NET support policy](https://dotnet.microsoft.com/platform/support/policy): Definitions for LTS and STS.
- [Aspire support policy](https://dotnet.microsoft.com/platform/support/policy/aspire): Important unique product lifecycle details.

## ⬆️ Upgrade to Aspire 9.5

> [!NOTE]
> Try out `aspire update`!
> Aspire 9.5 brings a new preview CLI command - [aspire update](#new-aspire-update-command-preview) - that can update your AppHost and its packages for you. Get the latest CLI if you want to try to give us feedback about it on [GitHub](https://github.com/dotnet/aspire/issues)!

Moving between minor releases of Aspire is simple:

1. Get the latest release of the Aspire CLI:

    ```bash
    curl -sSL https://aspire.dev/install.sh | bash
    ```

    ```powershell
    iex "& { $(irm https://aspire.dev/install.ps1) }"
    ```

1. In your AppHost project file (that is, _MyApp.AppHost.csproj_), update the [📦 Aspire.AppHost.Sdk](https://www.nuget.org/packages/Aspire.AppHost.Sdk) package to version `9.5.1`:

    ```xml
    <Sdk Name="Aspire.AppHost.Sdk" Version="9.5.1" />
    ```

    For more information, see [Aspire SDK](xref:dotnet/aspire/sdk).

1. Check for any NuGet package updates, either using the NuGet Package Manager in Visual Studio or the **Update NuGet Package** command from C# Dev Kit in Visual Studio (VS) Code.

1. Update to the latest [Aspire templates](../fundamentals/aspire-sdk-templates.md) by running the following .NET command line:

    ```dotnetcli
    dotnet new install Aspire.ProjectTemplates
    ```

  > [!NOTE]
  > The `dotnet new install` command updates existing Aspire templates to the latest version if they're already installed.

If your AppHost project file doesn't have the `Aspire.AppHost.Sdk` reference, you might still be using Aspire 8. To upgrade to 9, follow [the upgrade guide](../get-started/upgrade-to-aspire-9.md).

## 🛠️ CLI and tooling

Aspire 9.5 adds targeted CLI and tooling updates that speed up setup and maintenance.

**Key improvements:**

- Channel-aware package selection in `aspire add` so you can choose stable, daily, or custom builds.
- Preview `aspire update` command that scans your AppHost, validates versions, and applies safe upgrades.
- Experimental single-file AppHost (`apphost.cs`) behind a feature flag.
- SSH Remote port forwarding support in VS Code, matching Dev Container and Codespaces behavior.
- Enhanced `aspire exec` with `--workdir`, clearer help, and early argument validation.
- Performance refinements: package search disk cache, cleaner debug output, clearer status messages, and faster repeat package resolution.

Run `aspire update` to upgrade, enable the single-file AppHost to experiment with a minimal setup, and use `aspire exec` to script tasks with inherited environment context.

### Channel-aware `aspire add` and templating

You can now pick packages from different channels or versions during `aspire add`. Additionally, friendly name generation is now more flexible for searching packages. When adding packages, you should use versions that are aligned to the `Aspire.Hosting.AppHost` package that you're using. To update your entire AppHost and its referenced project, you can use the `aspire update` command as described in the following section.

### New `aspire update` command (preview)

The new `aspire update` command helps you keep your Aspire projects current by automatically detecting and updating outdated packages and templates.

```Aspire
# Analyze and update out-of-date Aspire packages & templates
aspire update
```

:::image type="content" source="media/aspire-update.gif" lightbox="media/aspire-update.gif" alt-text="Recording of aspire update running on eShop sample.":::

This command updates your SDK, AppHost packages, and any Aspire client integrations used in the app. It validates package compatibility and asks for confirmation before applying changes. Like `add`, `update` is channel aware, so you can choose to update to stable, daily, or your own configuration of builds.

> [!IMPORTANT]
> 🧪 **Preview feature**: The `aspire update` command is in preview and might change before general availability. The `aspire update` command makes changes to project files, central package management, and NuGet.config files. We recommend using version control and inspecting changes after `aspire update` is run to verify the changes.

### File-based AppHost support (preview)

Aspire 9.5 introduces infrastructure for .NET 10's new file-based apps feature, meaning you only need one file &mdash; and no project file &mdash; for your Aspire Apphost. The new capabilities are currently behind a feature flag that elevates the minimum .NET SDK requirement to prepare for upcoming file-based app execution scenarios.

```Aspire
# Enable file-based AppHost ("apphost.cs") support
aspire config set features.singlefileAppHostEnabled true
# Disable the minimum SDK version check
aspire config set features.minimumSdkCheckEnabled false
```

For more information, see [aspire config set command](../cli-reference/aspire-config-set.md).

**SDK version requirements:**

- **Feature enabled**: Requires .NET SDK 10.0.100 RC1 or later
- **Override support**: Manual SDK version overrides continue to work with highest precedence

You can use `aspire new` to create a new, blank file-based AppHost. Select the _Single-file AppHost (experimental)_ option from the project template list:

```csharp
#:sdk Aspire.AppHost.Sdk@9.5.1

var builder = DistributedApplication.CreateBuilder(args);

builder.Build().Run();
```

Then add some resources, use `aspire add` the same as you would with a project-based Apphost, and `aspire run` to start!

### SSH Remote support for port forwarding in VS Code

Aspire 9.5 adds SSH Remote support in VS Code. Ports forward automatically in SSH Remote sessions the same way they do in Dev Containers and GitHub Codespaces.

### `aspire exec` command (preview) enhancements

The `aspire exec` command allows you to execute commands within the context of your Aspire application environment, inheriting environment variables, and configuration from your app model resources.

Aspire 9.5 builds on the 9.4 preview and adds several key improvements:

- `--workdir` (`-w`) flag to run commands inside a specific working directory.
- Fail-fast argument validation with clearer error messages.
- Improved help and usage text for better developer experience.

For more information, see [aspire exec command](../cli-reference/aspire-exec.md) reference.

#### Basic usage examples

```Aspire
# Execute database migrations with environment variables from your app model
aspire exec --resource my-api -- dotnet ef database update

# Run commands in a specific working directory
aspire exec --resource worker --workdir /app/tools -- dotnet run

# Wait for resource to start before executing command
aspire exec --start-resource my-worker -- npm run build
```

#### Command syntax

- Use `--resource` to execute immediately when AppHost starts.
- Use `--start-resource` to wait for the resource to be running first.
- Use `--workdir` to specify the working directory for the command.
- Use `--` to separate `aspire` options from the command to execute.

> [!NOTE]
> This command is disabled by default. To use it, turn on the feature toggle by running:
>
> ```Aspire
> aspire config set features.execCommandEnabled true
> ```
>
> For more information, see [aspire config command](../cli-reference/aspire-config.md).

### Other tweaks

- Relative path included in AppHost status messages.
- Clean CLI debug logging with reduced noise.
- Directory safety check for `aspire new` and consistent template inputs.
- Refactored NuGet prefetch architecture reducing UI lag during `aspire new`.
- Package search disk cache to speed up `aspire new | add | update` commands.
- Context-sensitive completion messages for publish/deploy.

## 📊 Dashboard enhancements

### Generative AI visualizer

Aspire 9.5 introduces the GenAI visualizer, which collates, summarizes, and visualizes LLM-centric calls within your app:

- 🗃️ Explore input and output messages.
- 🚀 JSON/XML payloads highlighted and indented.
- 🖼️ Preview Markdown and multimodal content (for example, images).

If GenAI-specific telemetry is found in an OpenTelemetry (OTEL) span, a sparkle (✨) icon appears next to its name in the Traces view. Clicking the icon launches the visualizer dialog.

The [GenAI telemetry semantic conventions](https://opentelemetry.io/docs/specs/semconv/gen-ai/) are evolving rapidly. The visualizer supports multiple versions of the telemetry, and we update it as the conventions move toward a stable release.

:::image type="content" source="media/dashboard-geni-visualizer.gif" lightbox="media/dashboard-geni-visualizer.gif" alt-text="Recording of the GenAI visualizer.":::

### Rich property grid content

In the Aspire 9.5 Dashboard, there are icons and clickable buttons in property grids in resource details, log entry details, and span details.

- **Icons** improve visual clarity. For example, quickly see that a resource isn't in a healthy state if the icon is red or yellow.
- **Clickable buttons** improve navigation. For example, select a resource name or telemetry ID to navigate to a different page for more information.

:::image type="content" source="media/dashboard-rich-property-grid.png" lightbox="media/dashboard-rich-property-grid.png" alt-text="Screenshot of property grid with icons.":::

### Multi-resource console logs

A new "All" option in the console logs view streams logs from every running resource simultaneously.

- **Unified log stream**: See logs from all resources in chronological order.
- **Color-coded prefixes**: Each resource gets a deterministic color for easy identification.

:::image type="content" source="media/console-logs-all.png" lightbox="media/console-logs-all.png" alt-text="Screenshot of the console logs page displaying (All) logs.":::

### Custom resource icons

Resources can now specify custom icons and their variant (Filled, which is the default, or Regular) using `WithIconName()` for better visual identification in dashboard views. Any [Fluent UI system icons](https://github.com/microsoft/fluentui-system-icons/blob/main/icons_filled.md) can be used.

```csharp
var postgres = builder.AddPostgres("database")
    .WithIconName("database");

var redis = builder.AddRedis("cache")
    .WithIconName("memory");

var api = builder.AddProject<Projects.Api>("api")
    .WithIconName("webAsset", ApplicationModel.IconVariant.Regular);
```

This iconography helps teams quickly identify different types of resources in complex applications with many services. Custom resource icons now also apply to project and container resources through unified annotation, providing consistent visual identification across all resource types.

### Reverse proxy support

The dashboard now properly handles reverse proxy scenarios with explicit forwarded header mapping when enabled. This fixes common issues with authentication redirects and URL generation behind proxies like YARP.

```bash
# Enable forwarded headers processing
export ASPIRE_DASHBOARD_FORWARDEDHEADERS_ENABLED=true
```

This is useful for deployment scenarios where the dashboard is accessed through a load balancer or reverse proxy.

### Container runtime notifications

Notifications now appear when Docker or Podman is installed but unhealthy, with automatic dismissal when the runtime recovers. This provides immediate feedback when your container runtime needs attention, helping diagnose startup issues faster.

:::image type="content" source="media/dashboard-container-warning.png" lightbox="media/dashboard-container-warning.png" alt-text="Screenshot of the dashboard showing the container runtime warning.":::

### Trace filtering

The Traces page now has a type filter, a quick way to find traces and spans for the selected operation type.

For example, choose Messaging 📬 to see only traces from your app that interact with RabbitMQ, Azure Service Bus, etc.

:::image type="content" source="media/dashboard-trace-type.png" lightbox="media/dashboard-trace-type.png" alt-text="Screenshot of the traces page showing the type filter.":::

### Trace detail improvements

The trace detail page includes several quality-of-life improvements:

🏷️ Span names are clearer, with resources split into their own column
🪵 Logs are now shown in the waterfall chart—hover for a tooltip, or select for full details
↕️ New "Expand all" and "Collapse all" buttons

:::image type="content" source="media/dashboard-trace-detail-logs.png" lightbox="media/dashboard-trace-detail-logs.png" alt-text="Screenshot of trace details showing the new resources column and log tooltips.":::

### Other improvements

- Resource action menus now use submenus to prevent overflow on complex apps.
- Projects show their associated launch profiles.
- Error spans use consistent error styling.
- Better default icons for parameters and services.
- Enhanced port parsing.
- Message truncation for long log entries.
- Optional log line wrapping.
- Improved text visualizer dialog.
- Comprehensive dashboard localization improvements including localized Launch Profile names.
- Embedded log entries within trace spans.
- Better span timing calculations.
- Accessibility improvements with better toolbar/menu overflow handling, improved keyboard navigation, semantic headings, and mobile navigation scroll fixes.

## 📦 Integration changes and additions

### OpenAI hosting integration

The new `AddOpenAI` integration provides first-class support for modeling OpenAI endpoints and their associated models within your Aspire application model. For more information, see [Aspire OpenAI integration (Preview)](../openai/openai-integration.md).

**Features:**

- **Single OpenAI endpoint** resource with child model resources using `AddModel`.
- **Resource referencing** so other projects automatically receive connection information.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var openai = builder.AddOpenAI("openai");

var chatModel = openai.AddModel("chat", "gpt-4o-mini");

var api = builder.AddProject<Projects.Api>("api")
    .WithReference(chatModel);

builder.Build().Run();
```

**Local development scenario:**

```csharp
// Use with local OpenAI-compatible services
var localOpenAI = builder.AddOpenAI("local-openai")
    .WithApiKey(builder.AddParameter("local-api-key"))
    .WithEndpoint("http://localhost:11434"); // Ollama or similar

var localModel = localOpenAI.AddModel("local-chat", "llama3.2");
```

### GitHub Models and Azure AI Foundry typed catalogs

Aspire 9.5 introduces a typed catalog for GitHub and Azure-hosted models, providing IntelliSense support and refactoring safety when working with AI models. This brings type safety and IntelliSense support for the ever-increasing AI model catalog, and takes the guesswork out of version and "format" strings. The catalog is updated daily.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Before: String-based approach (error-prone)
var model = builder.AddGitHubModel("chat", "gpt-4o-mini"); // Typos not caught

// After: Typed catalog approach
var chatModel = builder.AddGitHubModel("chat", GitHubModel.OpenAI.OpenAIGPT4oMini);

// IntelliSense shows all available models grouped by provider
var embeddingModel = builder.AddGitHubModel("embeddings", GitHubModel.OpenAI.OpenAITextEmbedding3Large);
```

For more information, see:

- [Aspire GitHub Models integration (Preview)](../github/github-models-integration.md).
- [Aspire Azure AI Foundry integration (Preview)](../azureai/azureai-foundry-integration.md).

### Dev Tunnels hosting integration

Aspire 9.5 introduces first-class support for Dev Tunnels, enabling seamless integration of secure public tunnels for your applications during development and testing scenarios.

**Features:**

- **Secure public tunnels**: Create public HTTPS endpoints for applications running locally.
- **Automatic tunnel management**: Tunnels are created, configured, and cleaned up automatically.
- **Private and anonymous tunnels**: Support for both authenticated private tunnels and public anonymous access.
- **Development workflow integration**: Perfect for webhook testing, mobile app development, and external service integration.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add a basic Dev Tunnel resource (default: private access)
var tunnel = builder.AddDevTunnel("dev-tunnel");

// Add your web application
var webApp = builder.AddProject<Projects.WebApp>("webapp");

// Connect the tunnel to the web application endpoint
tunnel.WithReference(webApp);

builder.Build().Run();
```

The Dev Tunnels integration automatically handles Azure authentication, tunnel lifecycle management, and provides public or private URLs (depending on configuration) to connected resources, making it easy to expose local development services securely to external consumers. Dev Tunnels also improves support for mobile dev, such as .NET MAUI, making it easy to launch both your backend and mobile app at once without complex dev-time config. For more information, see [Aspire Dev Tunnels integration (Preview)](../extensibility/dev-tunnels-integration.md).

### YARP static files support

Aspire 9.5 adds comprehensive static file serving capabilities to the YARP integration, enabling you to serve static assets directly from YARP alongside reverse proxy functionality. This is perfect for single-page applications, frontend assets, and hybrid scenarios where you need both static content and API proxying.

**Features:**

- **Direct static file serving**: Serve HTML, CSS, JS, and other static assets from YARP.
- **Flexible source options**: Bind mount local directories or use Docker multi-stage builds.
- **Automatic configuration**: Simple API enables static files with minimal setup.
- **Production ready**: Works in both development and publish scenarios.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Enable static file serving (serves from wwwroot folder)
var yarp = builder.AddYarp("gateway")
    .WithStaticFiles();

builder.Build().Run();
```

**Docker multi-stage build scenario:**

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Use Dockerfile to build and copy static assets
var frontend = builder.AddYarp("frontend")
    .WithStaticFiles()
    .WithDockerFile("../react-app");

builder.Build().Run();
```

For more information, see [Multi-stage Docker builds](../proxies/yarp-integration.md#multi-stage-docker-builds).

**Hybrid static + API gateway:**

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var backendApi = builder.AddProject<Projects.Api>("api");
var authService = builder.AddProject<Projects.Auth>("auth");

// YARP serves static files AND proxies API requests
var gateway = builder.AddYarp("app-gateway")
    .WithStaticFiles("./frontend/dist")
    .WithConfiguration(yarp =>
    {
        // API routes
        yarp.AddRoute("/api/{**catch-all}", backendApi)
            .WithTransformPathRemovePrefix("/api");
          
        // Auth routes
        yarp.AddRoute("/auth/{**catch-all}", authService)
            .WithTransformPathRemovePrefix("/auth");
          
        // Static files are served for all other routes
    });

builder.Build().Run();
```

This feature enables modern web application architectures where YARP acts as both a reverse proxy for backend services and a static file server for frontend assets, providing a unified entry point for your distributed application.

For more information, see [Aspire YARP integration](../proxies/yarp-integration.md).

### Azure Kusto / Azure Data Explorer

A new **preview** package `Aspire.Hosting.Azure.Kusto` has been added. Once the package has been added to the AppHost, it's possible to start a Kusto emulator with just a few lines of code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddAzureKustoCluster("kusto")
    .RunAsEmulator()
    .AddReadWriteDatabase("sensorreadings");

builder.Build().Run();
```

### RabbitMQ auto activation

RabbitMQ client connections now support auto activation to prevent startup deadlocks and improve application reliability. Auto activation is disabled by default in 9.5, but planned to be enabled by default in a future release.

```csharp
var builder = WebApplication.CreateBuilder(args);

// Auto activation is disabled by default for RabbitMQ, enable using DisableAutoActivation=false
builder.AddRabbitMQClient("messaging", o => o.DisableAutoActivation = false);
```

### Redis integration improvements

#### Auto activation

Redis client connections also now support auto activation, and are also disabled by default, but planned to be enabled by default in a future release.

```csharp
var builder = WebApplication.CreateBuilder(args);

// Auto activation is disabled by default for Redis, enable using DisableAutoActivation=false
var redisClient = builder.AddRedisClient("redis", c => c.DisableAutoActivation = false);
```

#### Client builder pattern

Aspire 9.5 introduces a new Redis client builder pattern that provides a fluent, type-safe approach to configuring Redis clients with integrated support for distributed caching, output caching, and Azure authentication.

Basic usage:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddRedisClientBuilder("redis") 
  .WithDistributedCache(options => 
  { 
    options.InstanceName = "MyApp";
  });
```

To enable Azure authentication, add a reference to the new `Aspire.Microsoft.Azure.StackExchangeRedis` package, and chain a call to `.WithAzureAuthentication()`:

```csharp
builder.AddRedisClientBuilder("redis") 
  .WithAzureAuthentication()
  .WithDistributedCache(options => 
  { 
    options.InstanceName = "MyApp";
  });
```

### Azure Redis Enterprise support

Aspire 9.5 introduces first-class support for [Azure Redis Enterprise](/azure/redis/overview), providing a high-performance, fully managed Redis service with enterprise-grade features. Azure Redis Enterprise provides advanced caching capabilities with clustering, high availability, and enterprise security features while maintaining compatibility with the standard Redis APIs.

The new `AddAzureRedisEnterprise` extension method enables Redis Enterprise resource modeling:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add Azure Redis Enterprise resource
var redisEnterprise = builder.AddAzureRedisEnterprise("redis-enterprise");

// Use in your applications
var api = builder.AddProject<Projects.Api>("api")
    .WithReference(redisEnterprise);

builder.Build().Run();
```

**Local development with container emulation:**

```csharp
var redisEnterprise = builder.AddAzureRedisEnterprise("redis-enterprise")
    .RunAsContainer(container => container
        .WithHostPort(6379));
```

**Authentication options:**

Like other Azure integrations, Azure Redis Enterprise uses Microsoft Entra ID authentication by default. This is the recommended authentication strategy since secrets aren't used. To enable access key authentication, you can use the following:

```csharp
// With access key authentication (default)
var redisEnterprise = builder.AddAzureRedisEnterprise("redis-enterprise")
    .WithAccessKeyAuthentication();

// With Key Vault integration for access keys
var keyVault = builder.AddAzureKeyVault("keyvault");
var redisEnterprise = builder.AddAzureRedisEnterprise("redis-enterprise")
    .WithAccessKeyAuthentication(keyVault);
```

For more information, see [Aspire Azure Managed Redis integration](../caching/azure-redis-enterprise-integration.md).

### Azure App Configuration emulator (preview)

Aspire 9.5 adds preview support for running an Azure App Configuration emulator as part of your distributed application model. The emulator lets you develop locally with full feature parity for key/value configuration data—without provisioning a live Azure resource.

Container image (preview): `mcr.microsoft.com/azure-app-configuration/app-configuration-emulator:1.0.0-preview`

You can use this container **with or without Aspire**—it's just a normal container image. Aspire adds first-class resource modeling, lifecycle management, environment wiring, and (optionally) persistent storage.

**Features:**

- Local development parity for Azure App Configuration scenarios.
- Built-in browser UI for CRUD (create, read, update, delete) of configuration entries.
- Persistent data via a mounted volume (optional).
- Seamless integration with other Aspire resources via `WithReference`.
- Supports inner-loop testing without network latency or secrets in Azure.

**Add emulator via Aspire:**

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var appConfig = builder.AddAzureAppConfiguration("config")
  .RunAsEmulator(emulator => emulator
    .WithDataVolume()); // Persists configuration across restarts

var api = builder.AddProject<Projects.Api>("api")
    .WithReference(appConfig);

builder.Build().Run();
```

**Accessing the UI:**

When running under Aspire, the emulator dashboard (configuration explorer) is exposed as a normal endpoint. Open it from the Aspire Dashboard resource details panel to manage key-values interactively.

**Standalone (without Aspire):**

```bash
docker run --rm -p 8080:8080 \
  mcr.microsoft.com/azure-app-configuration/app-configuration-emulator:1.0.0-preview
```

Then browse to `http://localhost:8080` to launch the UI and manage configuration entries.

> [!IMPORTANT]
> The Azure App Configuration emulator is in **preview**. Behavior, image tags, and API surface may change before general availability.

**Typical project configuration usage:**

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddAzureAppConfiguration("config");

var app = builder.Build();

app.MapGet("/message", (IConfiguration config) =>
{
    var message = config["Message"];
    return new { Message = message };
});
app.Run();
```

This enables fast local iteration with a real UI and no external dependencies, while keeping deployment parity with Azure-hosted configuration.

### Azure Storage emulator improvements

Aspire now pulls Azurite version 3.35.0 by default, resolving health check issues that previously returned HTTP 400 responses. This improves the reliability of Azure Storage emulator health checks during development.

### MySQL password improvements

The MySQL integration added support for specifying a password parameter:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Secure password parameter
var password = builder.AddParameter("mysql-password", secret: true);

var mysql = builder.AddMySql("mysql")
    .WithPassword(password);


builder.Build().Run();
```

### Other improvements

- New reference properties have been added to Azure PostgreSQL and Redis resources for custom connection string composition and individual component access.
- OpenTelemetry Protocols (OTLP) support now has protocol selection capabilities, allowing you to choose between gRPC and HTTP protobuf transports for telemetry data.

## 🧩 App model enhancements

### Resource lifecycle events

Aspire 9.5 introduces new resource lifecycle event APIs that allow you to register callbacks for when resources stop, providing better control over cleanup and coordination during application shutdown.

The new `OnResourceStopped` extension method allows you to register callbacks that execute when a resource enters the stopped state:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var invoicing = builder.AddPostgres("postgres", "mypostgres").AddDatabase("invoicing");

var api = builder.AddProject<Projects.Api>("api")
    .OnResourceStopped(async (resource, stoppedEvent, cancellationToken) =>
    {
        // Use events to clean up the system to allow rapid
        // inner loop debugging.
      
        await ResetSystemState();
    })
    .WithReference(invoicing);

builder.Build().Run();
```

**Resource stopped event details:**

The `ResourceStoppedEvent` provides information about the stopping event:

```csharp
// Register detailed stopped event handler
var redis = builder.AddRedis("cache")
    .OnResourceStopped(async (resource, stoppedEvent, cancellationToken) =>
    {
        // Access event details
        Console.WriteLine($"Resource: {resource.Name}");
        Console.WriteLine($"Event timestamp: {stoppedEvent.Snapshot.StopTimeStamp}");
        Console.WriteLine($"Exit code: {stoppedEvent.Snapshot.ExitCode}");
      
        // Perform async cleanup with cancellation support
        await CleanupCacheConnections(cancellationToken);
    });
```

### Context-based endpoint resolution

**Breaking change**: Endpoint resolution in `WithEnvironment` callbacks now correctly resolves container hostnames instead of always using "localhost."

```csharp
var redis = builder.AddRedis("redis");

// Another container getting endpoint info from Redis container
var rabbitmq = builder.AddContainer("myapp", "mycontainerapp")
    .WithEnvironment(context =>
    {
        var endpoint = redis.GetEndpoint("tcp");
        var redisHost = endpoint.Property(EndpointProperty.Host);
        var redisPort = endpoint.Property(EndpointProperty.Port);

        // Previously: redisHost would always resolve to "localhost" 
        // Now: redisHost correctly resolves to "redis" (container name)
        context.EnvironmentVariables["REDIS_HOST"] = redisHost;
        context.EnvironmentVariables["REDIS_PORT"] = redisPort;
    })
    .WithReference(redis);
```

**What you need to review:**

- **Container deployments**: Your apps will now receive correct container hostnames.
- **Local development**: Localhost behavior is preserved for noncontainerized scenarios.
- **Connection strings**: Automatic connection strings continue to work as expected.
- **Manual environment**: Review custom `WithEnvironment` calls that assume localhost.

### HTTP health probes for resources

Aspire 9.5 introduces comprehensive HTTP health probe support that allows you to configure startup, readiness, and liveness probes for your resources, providing better health monitoring and deployment coordination.

**Features:**

- **Multiple probe types**: Configure startup, readiness, and liveness probes independently.
- **Flexible endpoint targeting**: Probe any HTTP endpoint with custom paths and configurations.
- **Configurable timing**: Control probe intervals, timeouts, and failure thresholds.
- **Kubernetes alignment**: Probe semantics align with Kubernetes health check concepts.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add readiness probe to ensure service is ready before routing traffic
var api = builder.AddProject<Projects.Api>("api")
    .WithHttpProbe(ProbeType.Readiness, "/health/ready");

// Add liveness probe to detect if service needs restart
var worker = builder.AddProject<Projects.Worker>("worker")
    .WithHttpProbe(ProbeType.Liveness, "/health/live");

builder.Build().Run();
```

**Advanced probe configuration:**

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Comprehensive probe setup with custom timing
var api = builder.AddProject<Projects.Api>("api")
    .WithHttpProbe(
        type: ProbeType.Startup,
        path: "/health/startup",
        initialDelaySeconds: 30,    // Wait 30s before first probe
        periodSeconds: 10,          // Probe every 10 seconds
        timeoutSeconds: 5,          // 5 second timeout per probe
        failureThreshold: 5,        // Consider failed after 5 failures
        successThreshold: 1         // Consider healthy after 1 success
    );

builder.Build().Run();
```

**Integration with resource dependencies:**

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddPostgres("postgres");
var cache = builder.AddRedis("redis");

// API with probes that check dependencies
var api = builder.AddProject<Projects.Api>("api")
    .WithHttpProbe(ProbeType.Readiness, "/health/ready") // Checks DB & Redis connectivity
    .WaitFor(database)  // Wait for database startup
    .WaitFor(cache)     // Wait for cache startup
    .WithReference(database)
    .WithReference(cache);

// Frontend waits for API to be ready (not just started)
var frontend = builder.AddProject<Projects.Frontend>("frontend")
    .WaitFor(api)  // Waits for API readiness probe to pass
    .WithReference(api);

builder.Build().Run();
```

This feature enhances deployment reliability by providing fine-grained health monitoring that integrates seamlessly with Aspire's resource orchestration and dependency management.

### Enhanced resource waiting patterns

New `WaitForStart` method provides granular control over startup ordering, complementing existing `WaitFor` semantics. It also pairs with improved `ExternalService` health honoring which ensures dependents truly wait for external resources to be healthy.

**Understanding wait behaviors:**

- **`WaitFor`**: Waits for dependency to be Running AND pass all health checks.
- **`WaitForStart`**: Waits only for dependency to reach Running (ignores health checks).
- **`WaitForCompletion`**: Waits for dependency to reach a terminal state.

```csharp
var postgres = builder.AddPostgres("postgres");
var redis = builder.AddRedis("redis");

var api = builder.AddProject<Projects.Api>("api")
    .WaitForStart(postgres)  // New: startup only
    .WaitFor(redis)          // Healthy state
    .WithReference(postgres)
    .WithReference(redis);
```

**ExternalService health integration:**

`WaitFor` now honors `ExternalService` health checks. Previously a dependent could start even if the external target failed its readiness probe.

```csharp
var externalApi = builder.AddExternalService("backend-api", "http://api.company.com")
    .WithHttpHealthCheck("/health/ready");

var frontend = builder.AddProject<Projects.Frontend>("frontend")
    .WaitFor(externalApi)    // Now waits for healthy external API
    .WithReference(externalApi);
```

If you need the old (lenient) behavior:

```csharp
// Do not wait for health
var frontend = builder.AddProject<Projects.Frontend>("frontend")
    .WithReference(externalApi);

// Or only wait for startup
var frontend2 = builder.AddProject<Projects.Frontend>("frontend2")
    .WaitForStart(externalApi)
    .WithReference(externalApi);
```

### Enhanced resource lifetime support

**Breaking change**: Resources like `ParameterResource`, `ConnectionStringResource`, and GitHub Model resources now participate in lifecycle operations and support `WaitFor`.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var connectionString = builder.AddConnectionString("database");
var apiKey = builder.AddParameter("api-key", secret: true);

var api = builder.AddProject<Projects.Api>("api")
    .WaitFor(connectionString)
    .WaitFor(apiKey)
    .WithEnvironment("DB_CONNECTION", connectionString)
    .WithEnvironment("API_KEY", apiKey);

var github = builder.AddGitHubModels("github");
var model = github.AddModel("gpt4", GitHubModel.OpenAI.Gpt4o);

var aiService = builder.AddProject<Projects.AIService>("ai-service")
    .WaitFor(model)
    .WithReference(model);

builder.Build().Run();
```

These resources no longer implement `IResourceWithoutLifetime`; they surface as Running and can be waited on just like services.

## ☁️ Publishing and deployment

### Azure Container App Jobs support

Aspire 9.5 introduces comprehensive support for Azure Container App Jobs, allowing you to deploy both project and container resources as background job workloads that can run on schedules, in response to events, or be triggered manually. For more information, see [Azure Container App Jobs](../azure/container-app-jobs.md)

Container App Jobs complement the existing Container Apps functionality by providing a dedicated way to run finite workloads like data processing, ETL operations, batch jobs, and scheduled maintenance tasks.

#### Publishing resources as Container App Jobs

Use the new `PublishAsAzureContainerAppJob` extension method to publish resources as jobs:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Publish a project as a Container App Job
var dataProcessor = builder.AddProject<Projects.DataProcessor>("data-processor")
    .PublishAsAzureContainerAppJob(); // Deploys as a job which must be manually started.

builder.Build().Run();
```

Or use the `PublishAsScheduledAzureContainerAppJob` extension method to publish a resource as a job that runs on a schedule:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Publish a project as a Container App Job
var dataProcessor = builder.AddProject<Projects.DataProcessor>("data-processor")
    .PublishAsScheduledAzureContainerAppJob("0 0 * * *"); // Every day at midnight

builder.Build().Run();
```

#### Job customization and configuration

Use the callback on the `PublishAsAzureContainerAppJob(...)` method to customize the job:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Publish a project as a Container App Job
var dataProcessor = builder.AddProject<Projects.DataProcessor>("data-processor")
    .PublishAsAzureContainerAppJob((infrastructure, job) => {
        job.Configuration.ReplicaTimeout = 3600; // 1 hour
    });

builder.Build().Run();
```

### Built-in Azure deployment with Aspire CLI

Aspire 9.5 delivers the first iteration of a unified Azure provisioning and deployment pipeline through the `aspire deploy` command. The deployment experience features graph-based dependency planning through `ResourceDeploymentGraph` for correct resource provisioning order and maximum parallelism, support for interactive prompting to gather values required for deployment, and enhanced error reporting for identifying issues during deployment. The AppHost integrates Azure provisioning prompts into the standard interaction system for consistent UX, providing deployment-time flexibility with automatic infrastructure provisioning, container image building and registry pushing, and compute resource deployment to Azure Container Apps—all orchestrated through a single command with real-time progress monitoring and comprehensive error reporting.

For more information on deploying to Azure with the Aspire CLI, read [the official documentation](../deployment/aspire-deploy/aca-deployment-aspire-cli.md).

### Executable resource configuration APIs

Enhanced APIs for configuring executable resources with command and working directory specifications.

#### WithCommand and WithWorkingDirectory APIs

New extension methods enable fluent, mutable configuration of executable resources, allowing you to modify command and working directory after initial setup:

```csharp
// Configure executable with custom command and working directory
var processor = builder.AddExecutable("data-processor", "python", "/app/data-processing")
    .WithCommand("main.py --batch-size 100")
    .WithArgs("--config", "production.json");

// Change working directory after initial configuration
var buildTool = builder.AddExecutable("build-tool", "npm", ".")
    .WithCommand("run build:production")
    .WithWorkingDirectory("./frontend");
```

#### Enhanced CommandLineArgsCallbackContext

The `CommandLineArgsCallbackContext` now includes resource information for context-aware argument building:

```csharp
var worker = builder.AddExecutable("worker", "dotnet", ".")
    .WithArgs(context =>
    {
        // Access to the resource instance for dynamic configuration
        var resourceName = context.Resource.Name;
        var environment = context.ExecutionContext.IsRunMode ? "Development" : "Production";
      
        context.Args.Add("--resource-name");
        context.Args.Add(resourceName);
        context.Args.Add("--environment");
        context.Args.Add(environment);
    });
```

These APIs provide fine-grained control over executable resource configuration, enabling complex deployment scenarios and dynamic argument construction based on execution context.

#### InteractionInputCollection enhancements

Enhanced parameter input handling with the new `InteractionInputCollection` type:

```csharp
// Enhanced interaction service with typed input collection
public async Task<InteractionResult<InteractionInputCollection>> ProcessParametersAsync()
{
    var inputs = new List<InteractionInput>
    {
        new() { Name = "username", Label = "Username", InputType = InputType.Text },
        new() { Name = "password", Label = "Password", InputType = InputType.Password },
        new() { Name = "environment", Label = "Environment", InputType = InputType.Select,
                Options = new[] { ("dev", "Development"), ("prod", "Production") } }
    };

    var result = await interactionService.PromptInputsAsync(
        "Configure Parameters", 
        "Enter application configuration:", 
        inputs);

    if (result.Success)
    {
        // Access inputs by name with type safety
        var username = result.Value["username"].Value;
        var password = result.Value["password"].Value;
        var environment = result.Value["environment"].Value;
    }

    return result;
}
```

The `InteractionInputCollection` provides indexed access by name and improved type safety for parameter processing workflows.

### Docker Compose Aspire Dashboard forwarding headers

`AddDockerComposeEnvironment(...).WithDashboard()` gained `WithForwardedHeaders()` to enable forwarded `Host` and `Proto` handling for dashboard scenarios behind reverse proxies or compose networks. This mirrors the standalone dashboard forwarded header support and fixes auth redirect edge cases.

```csharp
builder.AddDockerComposeEnvironment("env")
  .WithDashboard(d => d.WithForwardedHeaders());
```

### Container build customization

`ContainerBuildOptions` support enables customizing the underlying `dotnet publish` invocation when Aspire builds project-sourced container images (for example to change configuration, trimming, or pass other MSBuild properties). Use the new options hook on the project container image configuration to set MSBuild properties instead of maintaining a custom Dockerfile. (Exact API surface is intentionally summarized here to avoid drift; see API docs for `ContainerBuildOptions` in the hosting namespace for usage.)

### Deployment image tag callbacks

Aspire 9.5 introduces powerful deployment image tag callback APIs that allow dynamic generation of container image tags at deployment time, supporting both synchronous and asynchronous scenarios.

#### Deployment tag callback features

- **Dynamic tag generation**: Calculate image tags based on deployment context, git commits, build numbers, or timestamps.
- **Async callback support**: Perform asynchronous operations like API calls or file system access during tag generation.
- **Deployment context access**: Access the deployment environment, resource information, and configuration.
- **Flexible callback types**: Support simple lambdas, context-aware callbacks, and async operations.

#### Basic deployment tag examples

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Simple static tag callback
var api = builder.AddProject<Projects.Api>("api")
    .WithDeploymentImageTag(() => "v1.2.3-stable");

// Dynamic tag with timestamp
var worker = builder.AddProject<Projects.Worker>("worker")
    .WithDeploymentImageTag(() => $"build-{DateTime.UtcNow:yyyyMMdd-HHmm}");

builder.Build().Run();
```

#### Context-aware deployment tags

```csharp
// Access deployment context for dynamic tag generation
var api = builder.AddProject<Projects.Api>("api")
    .WithDeploymentImageTag(context =>
    {
        // Access resource information
        var resourceName = context.Resource.Name;
        var environment = context.Environment;
      
        return $"{resourceName}-{environment}-{GetBuildNumber()}";
    });

// Git-based tagging
var frontend = builder.AddProject<Projects.Frontend>("frontend")
    .WithDeploymentImageTag(context =>
    {
        var gitCommit = GetGitCommitHash();
        var branch = GetCurrentBranch();
        return $"{branch}-{gitCommit[..8]}";
    });
```

#### Async deployment tag callbacks

```csharp
// Async callback for complex tag generation
var database = builder.AddProject<Projects.Database>("database")
    .WithDeploymentImageTag(async context =>
    {
        // Perform async operations during deployment
        var buildInfo = await GetBuildInfoFromApi();
        var version = await ReadVersionFromFile();
      
        return $"db-{version}-build{buildInfo.Number}";
    });

// API-based version lookup
var api = builder.AddProject<Projects.Api>("api")
    .WithDeploymentImageTag(async context =>
    {
        using var client = new HttpClient();
        var latestTag = await client.GetStringAsync("https://api.company.com/latest-tag");
        return $"api-{latestTag.Trim()}";
    });
```

#### Advanced deployment scenarios

```csharp
// Environment-specific tagging
var service = builder.AddProject<Projects.Service>("service")
    .WithDeploymentImageTag(context =>
    {
        return context.Environment switch
        {
            "Production" => $"prod-{GetReleaseVersion()}",
            "Staging" => $"staging-{GetBuildNumber()}",
            "Development" => $"dev-{DateTime.UtcNow:yyyyMMdd}",
            _ => "latest"
        };
    });

// Multi-resource coordination
var sharedVersion = await GetSharedVersionAsync();

var frontend = builder.AddProject<Projects.Frontend>("frontend")
    .WithDeploymentImageTag(() => $"frontend-{sharedVersion}");

var backend = builder.AddProject<Projects.Backend>("backend")
    .WithDeploymentImageTag(() => $"backend-{sharedVersion}");
```

## 💔 Breaking changes

For the complete listing, see [Breaking changes in Aspire 9.5](../compatibility/9.5/index.md).

### InteractionInput API requires Name property

**Breaking change**: `InteractionInput` now requires a `Name` property, while `Label` becomes optional ([#10835](https://github.com/dotnet/aspire/pull/10835)).

#### Migration example

```csharp
// Before (9.4 and earlier)
var input = new InteractionInput
{
    Label = "Database Password",
    InputType = InputType.SecretText,
    Required = true
};

// After (9.5+)
var input = new InteractionInput
{
    Name = "database_password", // Required field identifier
    Label = "Database Password", // Optional (defaults to Name)
    InputType = InputType.SecretText,
    Required = true
};
```

This change enables better form serialization and integration with interactive parameter processing.

### Notification terminology renamed from MessageBar

**Breaking change**: Notification terminology has been updated, with MessageBar renamed to use new notification terminology ([#10449](https://github.com/dotnet/aspire/pull/10449)).

This change affects APIs and terminology used in the notification system, requiring updates to code that references the old MessageBar naming conventions.
