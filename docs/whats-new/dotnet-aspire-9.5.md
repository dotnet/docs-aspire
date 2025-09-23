---
title: What's new in Aspire 9.5
description: Learn what's new in Aspire 9.5.
ms.date: 09/17/2025
---

## What's new in Aspire 9.5

📢 Aspire 9.5 is the next minor version release of Aspire. It supports:

- .NET 8.0 Long Term Support (LTS)
- .NET 9.0 Standard Term Support (STS)
- .NET 10.0 RC (release candidate) 1

If you have feedback, questions, or want to contribute to Aspire, collaborate with us on [:::image type="icon" source="../media/github-mark.svg" border="false"::: GitHub](https://github.com/dotnet/aspire) or join us on [:::image type="icon" source="../media/discord-icon.svg" border="false"::: Discord](https://aka.ms/aspire-discord) to chat with the team and other community members.

It's important to note that Aspire releases out-of-band from .NET releases. While major versions of Aspire align with major .NET versions, minor versions are released more frequently. For more information on .NET and Aspire version support, see:

- [.NET support policy](https://dotnet.microsoft.com/platform/support/policy): Definitions for LTS and STS.
- [Aspire support policy](https://dotnet.microsoft.com/platform/support/policy/aspire): Important unique product lifecycle details.

## ⬆️ Upgrade to Aspire 9.5

> [!NOTE]
> Try out `aspire update`!
> Aspire 9.5 brings a new preview CLI command - [aspire update](#new-aspire-update-command-preview) - that can update your AppHost and its packages for you. Get the latest CLI if you want to try and give us feedback about it on [GitHub](https://github.com/dotnet/aspire/issues)!

Moving between minor releases of Aspire is simple:

1. Get the latest release of the Aspire CLI:
    
    ```bash
    # Bash
    curl -sSL https://aspire.dev/install.sh | bash
    
    # PowerShell
    iex "& { $(irm https://aspire.dev/install.ps1) }"
    ```

1. In your AppHost project file (that is, _MyApp.AppHost.csproj_), update the [📦 Aspire.AppHost.Sdk](https://www.nuget.org/packages/Aspire.AppHost.Sdk) package to version `9.5.0`:

    ```xml
    <Sdk Name="Aspire.AppHost.Sdk" Version="9.5.0" />
    ```

    For more information, see [Aspire SDK](xref:dotnet/aspire/sdk).

1. Check for any NuGet package updates, either using the NuGet Package Manager in Visual Studio or the **Update NuGet Package** command from C# Dev Kit in VS Code.

1. Update to the latest [Aspire templates](../fundamentals/aspire-sdk-templates.md) by running the following .NET command line:

    ```dotnetcli
    dotnet new install Aspire.ProjectTemplates
    ```

  > [!NOTE]
  > The `dotnet new install` command will update existing Aspire templates to the latest version if they are already installed.

If your AppHost project file doesn't have the `Aspire.AppHost.Sdk` reference, you might still be using Aspire 8. To upgrade to 9, follow [the upgrade guide](../get-started/upgrade-to-aspire-9.md).

## 🛠️ CLI and tooling

### Channel-aware `aspire add` & templating

You can now pick packages from different channels or versions during `aspire add`. Additionally, friendly name generation is now more flexible for searching packages.

### New `aspire update` command (preview)

The new `aspire update` command helps you keep your Aspire projects current by automatically detecting and updating outdated packages and templates.

```bash
# Analyze and update out-of-date Aspire packages & templates
aspire update
```

This command updates your SDK, AppHost packages, and any Aspire client integrations used in the app. It validates package compatibility and asks for confirmation before applying changes. Like `add`, `update` is channel aware, so you can choose to update to stable, daily, or your own configuration of builds.

> [!IMPORTANT]
> 🧪 **Preview Feature**: The `aspire update` command is in preview and may change before general availability.

### Enhanced markdown and styling support

The CLI now has better markdown rendering support for an improved developer experience:

- **Code fences** with syntax highlighting for better readability
- **Rich text formatting** including emphasis, bold, and inline code
- **Structured lists** with bullet points and numbering
- **Safe markup escaping** to prevent XSS and rendering issues

### File-based AppHost support (preview)

Aspire 9.5 introduces infrastructure for .NET 10's new file-based apps feature, meaning you only need 1 file - and no project file! - for your Aspire apphost. The new capabilities are currently behind a feature flag that elevates the minimum .NET SDK requirement to prepare for upcoming file-based app execution scenarios.

```bash
# Enable file-based AppHost ("apphost.cs") support
aspire config set features.singlefileAppHostEnabled true
```

**SDK version requirements:**

- **Feature enabled**: Requires .NET SDK 10.0.100 RC1 or later
- **Override support**: Manual SDK version overrides continue to work with highest precedence

You can use `aspire new` to create a new, blank file-based apphost:

```csharp
#:sdk Aspire.AppHost.Sdk@9.5.0

var builder = DistributedApplication.CreateBuilder(args);

builder.Build().Run();

```

Then add some resources, use `aspire add` the same as you would with a project-based apphost, and `aspire run` to start!

### SSH Remote support for port forwarding in VS Code

Version 9.5 adds first-class support for SSH Remote development environments, extending automatic port forwarding configuration to VS Code SSH Remote scenarios alongside existing Devcontainer and Codespaces support.

**Features:**

- **Automatic environment detection**: Detects SSH Remote scenarios via `VSCODE_IPC_HOOK_CLI` and `SSH_CONNECTION` environment variables
- **Seamless port forwarding**: Automatically configures VS Code settings for Aspire application endpoints
- **Consistent developer experience**: Matches existing behavior for Devcontainers and Codespaces
- **No configuration required**: Works out-of-the-box when using VS Code SSH Remote extension

SSH Remote environments are automatically detected when both environment variables are present:

```bash
# SSH Remote environment variables (these are automatically set by VS Code)
export SSH_CONNECTION="192.168.1.1 12345 192.168.1.2 22"
export VSCODE_IPC_HOOK_CLI="/path/to/vscode/hook"

# Aspire then detects and configures port forwarding
aspire run
```

The SSH Remote support follows the exact same patterns as existing Devcontainer and Codespaces integration, ensuring a consistent experience across all VS Code remote development scenarios. Port forwarding settings are automatically written to `.vscode-server/data/Machine/settings.json` when SSH Remote environments are detected.

### `aspire exec` command (preview) enhancements

The `aspire exec` command allows you to execute commands within the context of your Aspire application environment, inheriting environment variables and configuration from your app model resources.

Building on the 9.4 preview, version 9.5 adds several key improvements:

- `--workdir` (`-w`) flag to run commands inside a specific working directory (#10912)
- Fail-fast argument validation with clearer error messages (#10606)
- Improved help and usage text for better developer experience (#10598)

#### Basic usage examples

```bash
# Execute database migrations with environment variables from your app model
aspire exec --resource my-api -- dotnet ef database update

# Run commands in a specific working directory
aspire exec --resource worker --workdir /app/tools -- dotnet run

# Wait for resource to start before executing command
aspire exec --start-resource my-worker -- npm run build
```

#### Command syntax

- Use `--resource` to execute immediately when AppHost starts
- Use `--start-resource` to wait for the resource to be running first
- Use `--workdir` to specify the working directory for the command
- Use `--` to separate aspire options from the command to execute

> [!IMPORTANT]
> 🧪 **Feature Flag**: The `aspire exec` command requires explicit enablement with:
>
> ```bash
> aspire config set features.execCommandEnabled true
> ```

### Other tweaks

- Relative path included in AppHost status messages
- Clean CLI debug logging with reduced noise
- Directory safety check for `aspire new` and consistent template inputs
- Refactored NuGet prefetch architecture reducing UI lag during `aspire new` on macOS and enabling command-aware caching. Temporary NuGet config improvements ensure wildcard mappings
- Context-sensitive completion messages for publish/deploy
- Interaction answer typing change (`object`) for future extensibility
- Improved CTRL+C message and experience

> The `aspire exec` and `aspire update` commands remain in preview behind feature flags; behavior may change in a subsequent release.

## 📊 Dashboard enhancements

### Deep-linked telemetry navigation

The dashboard now provides seamless navigation between different telemetry views with interactive elements in property grids. Trace IDs, span IDs, resource names, and log levels become clickable buttons for one-click navigation.

- **Trace IDs**: Click to view the complete distributed trace
- **Span IDs**: Navigate directly to specific trace spans
- **Resource names**: Jump to resource-specific telemetry views  
- **Log levels**: Filter logs by severity level instantly

This eliminates the need to manually copy/paste identifiers between different dashboard views, making debugging and monitoring much more efficient.

### Multi-resource console logs

A new "All" option in the console logs view streams logs from every running resource simultaneously.

**Features:**

- **Unified log stream**: See logs from all resources in chronological order
- **Color-coded prefixes**: Each resource gets a deterministic color for easy identification
- **Configurable timestamps**: Separate timestamp preference to reduce noise

### Custom resource icons

Resources can now specify custom icons and their variant - Filled (default) or Regular - using `WithIconName()` for better visual identification in dashboard views. Any [Fluent UI system icons](https://github.com/microsoft/fluentui-system-icons/blob/main/icons_filled.md) can be used.

```csharp
var postgres = builder.AddPostgres("database")
    .WithIconName("database");

var redis = builder.AddRedis("cache")
    .WithIconName("memory");

var api = builder.AddProject<Projects.Api>("api")
    .WithIconName("webAsset", ApplicationModel.IconVariant.Regular);
```

This helps teams quickly identify different types of resources in complex applications with many services. Custom resource icons now also apply to project & container resources via unified annotation, providing consistent visual identification across all resource types.

### Generative AI insights

New dialog and UI components make GenAI interactions easier to inspect and understand LLM-centric calls within your app. If GenAI-specific telemetry is found in an OTEL span, a sparkle (✨) icon will appear next to it's name in the Traces view. When selected, a new UI modal appears with relevant LLM information including the prompt, response, token usage, and more.

### Reverse proxy support

The dashboard now properly handles reverse proxy scenarios with explicit forwarded header mapping when enabled. This fixes common issues with authentication redirects and URL generation behind proxies like YARP.

```bash
# Enable forwarded headers processing
export ASPIRE_DASHBOARD_FORWARDEDHEADERS_ENABLED=true
```

**Supported scenarios:**

- **OpenID Connect authentication** works correctly behind reverse proxies
- **URL generation** respects the original client request scheme and host
- **Limited header processing** for security - only Host and X-Forwarded-Proto are processed
- **YARP integration** and other reverse proxy solutions

This is particularly useful for deployment scenarios where the dashboard is accessed through a load balancer or reverse proxy.

### Container runtime notifications

More actionable notifications now appear when Docker/Podman is installed but unhealthy, with automatic dismissal when runtime recovers. This provides immediate feedback when your container runtime needs attention, helping diagnose startup issues faster.

### Trace filtering

A new span "type" filter lets you focus on specific kinds of spans for faster investigation.

### Trace detail improvements

The trace detail page now has Expand/collapse all, clearer exemplars, an added resource column, preserved root span visibility, and more reliable span linking.

### Other improvements

- Resource action menus now use sub-menus to prevent overflow on complex apps
- Projects show their associated launch profiles
- Error spans use consistent error styling
- Better default icons for parameters and services
- Enhanced port parsing
- Message truncation for long log entries
- Optional log line wrapping
- Improved text visualizer dialog
- Comprehensive dashboard localization improvements including localized Launch Profile names
- Embedded log entries within trace spans
- Better span timing calculations
- Accessibility improvements with better toolbar/menu overflow handling, improved keyboard navigation, semantic headings, and mobile navigation scroll fixes

## 📦 Integration changes and additions

### OpenAI hosting integration

The new `AddOpenAI` integration provides first-class support for modeling OpenAI endpoints and their associated models within your Aspire application graph.

**Features:**

- **Single OpenAI endpoint** resource with child model resources using `AddModel`
- **Parameter-based API key** provisioning with `ParameterResource` support
- **Endpoint override** for local gateways, proxies, or self-hosted solutions
- **Resource referencing** so other projects automatically receive connection information

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var apiKey = builder.AddParameter("openai-api-key", secret: true);

var openai = builder.AddOpenAI("openai")
    .WithApiKey(apiKey)
    .WithEndpoint("https://api.openai.com");

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

Aspire 9.5 introduces a strongly-typed catalog for GitHub and Azure-hosted models, providing IntelliSense support and refactoring safety when working with AI models. This brings type safety and IntelliSense support for the ever-increasing AI model catalog, and takes the guesswork out of version and "format" strings. The catalog is updated daily.

```csharp
// Before: String-based approach (error-prone)
var model = github.AddModel("chat", "gpt-4o-mini"); // Typos not caught

// After: Typed catalog approach  
var chatModel = github.AddModel("chat", GitHubModel.OpenAI.Gpt4oMini);

// IntelliSense shows all available models grouped by provider
var embeddingModel = github.AddModel("embeddings", GitHubModel.OpenAI.TextEmbedding3Large);
```

### Dev Tunnels hosting integration

Aspire 9.5 introduces first-class support for Dev Tunnels, enabling seamless integration of secure public tunnels for your applications during development and testing scenarios.

**Features:**

- **Secure public tunnels**: Create public HTTPS endpoints for applications running locally
- **Automatic tunnel management**: Tunnels are created, configured, and cleaned up automatically
- **Private and anonymous tunnels**: Support for both authenticated private tunnels and public anonymous access
- **Development workflow integration**: Perfect for webhook testing, mobile app development, and external service integration

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add a basic Dev Tunnel resource (default: private access)
var tunnel = builder.AddDevTunnel("dev-tunnel");

// Add your web application
var webApp = builder.AddProject<Projects.WebApp>("webapp");

// Connect the tunnel to the web application endpoint
tunnel.WithReference(webApp.GetEndpoint("http"));

builder.Build().Run();
```

The Dev Tunnels integration automatically handles Azure authentication, tunnel lifecycle management, and provides public or private URLs (depending on configuration) to connected resources, making it easy to expose local development services securely to external consumers. Dev Tunnels also improves support for mobile dev, such as .NET MAUI, making it easy to launch both your backend and mobile app at once without complex dev-time config.

### YARP static files support

Aspire 9.5 adds comprehensive static file serving capabilities to the YARP integration, enabling you to serve static assets directly from YARP alongside reverse proxy functionality. This is perfect for single-page applications, frontend assets, and hybrid scenarios where you need both static content and API proxying.

**Features:**

- **Direct static file serving**: Serve HTML, CSS, JS, and other static assets from YARP
- **Flexible source options**: Bind mount local directories or use Docker multi-stage builds
- **Automatic configuration**: Simple API enables static files with minimal setup
- **Production ready**: Works in both development and publish scenarios

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

### Redis and RabbitMQ auto activation

Redis and RabbitMQ connections now support auto activation to prevent startup deadlocks and improve application reliability.

**Features:**

- **Eliminates blocking threads**: Connections are established proactively at startup rather than on first use
- **Prevents startup deadlocks**: Avoids synchronous connection establishment in dependency injection scenarios
- **Improves reliability**: Reduces first-request latency by pre-establishing connections
- **Configurable behavior**: Can be enabled or disabled per connection as needed

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Auto activation enabled by default for Redis
var redis = builder.AddRedis("cache");

// Use in applications - connection is already established
var api = builder.AddProject<Projects.Api>("api")
    .WithReference(redis);

builder.Build().Run();
```

#### RabbitMQ auto activation configuration

```csharp
// Application (not AppHost) code
var builder = Host.CreateApplicationBuilder(args);

// Default: auto activation ENABLED (no extra configuration required)
builder.AddRabbitMQClient("messaging");

// --- OR --- Opt out of auto activation (connection will be created lazily)
// builder.AddRabbitMQClient("messaging", settings =>
// {
//     settings.DisableAutoActivation = true; // disable auto activation
// });

var app = builder.Build();
app.Run();
```

### Redis client builder pattern

Aspire 9.5 introduces a new Redis client builder pattern that provides a fluent, type-safe approach to configuring Redis clients with integrated support for distributed caching, output caching, and Azure authentication.

**Features:**

- **Fluent configuration**: Chain multiple Redis features like distributed caching, output caching, and Azure authentication
- **Type-safe builders**: `AspireRedisClientBuilder` provides compile-time safety and IntelliSense
- **Integrated Azure authentication**: Seamless Azure AD/Entra ID authentication with `WithAzureAuthentication`
- **Service composition**: Build complex Redis configurations with multiple features in a single call chain

#### Basic Redis client builder usage

```csharp
var builder = Host.CreateApplicationBuilder(args);

builder.AddRedisClientBuilder("cache")
    .WithDistributedCache()
    .WithOutputCache();

var app = builder.Build();
app.Run();
```

**Advanced Redis builder patterns:**

#### Advanced Redis builder patterns

```csharp
// Multiple Redis instances with different configurations (application code)
var builder = Host.CreateApplicationBuilder(args);

// Cache-focused Redis instance
builder.AddRedisClientBuilder("cache")
    .WithDistributedCache(options => 
    {
        options.InstanceName = "MainCache";
        options.DefaultSlidingExpiration = TimeSpan.FromMinutes(30);
    });

// Output cache Redis instance with Azure authentication
builder.AddKeyedRedisClientBuilder("output-cache")
    .WithAzureAuthentication()
    .WithOutputCache();

// Session Redis instance
builder.AddKeyedRedisClientBuilder("sessions") 
    .WithDistributedCache(options =>
    {
        options.InstanceName = "Sessions";
    });

var app = builder.Build();
app.Run();
```

### Azure Redis Enterprise support

Aspire 9.5 introduces first-class support for Azure Redis Enterprise, providing a high-performance, fully managed Redis service with enterprise-grade features.

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

```csharp
// With access key authentication (default)
var redisEnterprise = builder.AddAzureRedisEnterprise("redis-enterprise")
    .WithAccessKeyAuthentication();

// With Key Vault integration for access keys
var keyVault = builder.AddAzureKeyVault("keyvault");
var redisEnterprise = builder.AddAzureRedisEnterprise("redis-enterprise")
    .WithAccessKeyAuthentication(keyVault);
```

Azure Redis Enterprise provides advanced caching capabilities with clustering, high availability, and enterprise security features while maintaining compatibility with the standard Redis APIs.

### Azure Storage emulator improvements

Aspire now pulls Azurite version 3.35.0 by default, resolving health check issues that previously returned HTTP 400 responses. This improves the reliability of Azure Storage emulator health checks during development.

### Broader Azure resource capability surfacing

Several Azure hosting resource types now implement `IResourceWithEndpoints` enabling uniform endpoint discovery and waiting semantics:

- `AzureAIFoundryResource`
- `AzureAppConfigurationResource`
- `AzureKeyVaultResource`
- `AzurePostgresFlexibleServerResource`
- `AzureRedisCacheResource`

### MySQL password improvements

Enhanced and standardized password handling for MySQL resources:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Secure password parameter
var password = builder.AddParameter("mysql-password", secret: true);

var mysql = builder.AddMySql("mysql")
    .WithPassword(password);

// Password can be updated during configuration
mysql.Resource.PasswordParameter = builder.AddParameter("new-mysql-password", secret: true);

// Environment-specific passwords
var devPassword = builder.Configuration["ConnectionStrings:MySQL:Password"];
if (!string.IsNullOrEmpty(devPassword))
{
    mysql.WithPassword(devPassword);
}

builder.Build().Run();
```

### Other improvements

- New reference properties have been added to Azure PostgreSQL and Redis resources for custom connection string composition and individual component access
- OpenTelemetry Protocol (OTLP) support now has protocol selection capabilities, allowing you to choose between gRPC and HTTP protobuf transports for telemetry data

## 🧩 App model enhancements

### Resource lifecycle events

Aspire 9.5 introduces new resource lifecycle event APIs that allow you to register callbacks for when resources stop, providing better control over cleanup and coordination during application shutdown.

The new `OnResourceStopped` extension method allows you to register callbacks that execute when a resource enters the stopped state:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddPostgres("postgres", "mypostgres")
    .OnResourceStopped(async (resource, stoppedEvent, cancellationToken) =>
    {
        // Perform cleanup when the database stops
        Console.WriteLine($"Database {resource.Name} has stopped");
        
        // Log final metrics, backup data, etc.
        await LogFinalMetrics(resource.Name);
    });

var api = builder.AddProject<Projects.Api>("api")
    .OnResourceStopped(async (resource, stoppedEvent, cancellationToken) =>
    {
        // Graceful API shutdown handling
        Console.WriteLine($"API service {resource.Name} is shutting down");
        await FlushPendingRequests();
    })
    .WithReference(database);

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
        Console.WriteLine($"Event timestamp: {stoppedEvent.Timestamp}");
        Console.WriteLine($"Stopping reason: {stoppedEvent.Reason}");
        
        // Perform async cleanup with cancellation support
        await CleanupCacheConnections(cancellationToken);
    });
```

**Coordination with lifecycle management:**

Resource stopped events work seamlessly with existing lifecycle features:

```csharp
var database = builder.AddPostgres("postgres")
    .OnResourceStopped(async (db, evt, ct) => 
    {
        await BackupDatabase(db.Name, ct);
    });

var worker = builder.AddProject<Projects.Worker>("worker")
    .WaitFor(database)  // Wait for startup
    .OnResourceStopped(async (svc, evt, ct) => 
    {
        await CompleteInFlightJobs(ct);
    })
    .WithReference(database);
```

New `ResourceStoppedEvent` provides lifecycle insight when resources shut down or fail:

```csharp
builder.AddProject<Projects.Api>("api")
  .OnResourceStopped(async (resource, evt, ct) =>
  {
      // Handle resource stopped event - log cleanup, notify other services, etc.
      Console.WriteLine($"Resource {resource.Name} stopped with event: {evt.ResourceEvent}");
      await NotifyDependentServices(resource.Name, ct);
  });
```

### Context-based endpoint resolution

**Breaking change**: Endpoint resolution in `WithEnvironment` callbacks now correctly resolves container hostnames instead of always using "localhost".

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

- **Container deployments**: Your apps will now receive correct container hostnames
- **Local development**: Localhost behavior preserved for non-containerized scenarios  
- **Connection strings**: Automatic connection strings continue to work as expected
- **Manual environment**: Review custom `WithEnvironment` calls that assume localhost

### HTTP health probes for resources

Aspire 9.5 introduces comprehensive HTTP health probe support that allows you to configure startup, readiness, and liveness probes for your resources, providing better health monitoring and deployment coordination.

**Features:**

- **Multiple probe types**: Configure startup, readiness, and liveness probes independently
- **Flexible endpoint targeting**: Probe any HTTP endpoint with custom paths and configurations
- **Configurable timing**: Control probe intervals, timeouts, and failure thresholds
- **Kubernetes alignment**: Probe semantics align with Kubernetes health check concepts

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

### Parameter prompting during deploy

Aspire 9.5 enhances the deployment experience by automatically prompting for unresolved parameters during `aspire deploy` operations, eliminating the need to manually specify all parameter values upfront.

#### Interactive parameter resolution

When deploying your Aspire application, any parameters without values are now automatically detected and prompted for interactively:

```bash
# Deploy command detects missing parameters and prompts automatically
aspire deploy

🔧 Resolving deployment parameters...

Enter value for 'database-password' (secret): ********
Enter value for 'api-key' (secret): **********************
Enter value for 'environment-name': production

✅ All parameters resolved, proceeding with deployment...
```

#### Benefits of interactive parameter prompting

- **Secure credential entry**: Sensitive parameters are masked during input
- **Deployment-time flexibility**: No need to pre-configure all parameter values
- **Error prevention**: Missing parameters are caught before deployment begins
- **Better developer experience**: Clear prompts with parameter descriptions

#### Parameter types supported

- **Secret parameters**: Automatically masked input for sensitive values
- **Standard parameters**: Regular text input with validation
- **Optional parameters**: Skipped if no value is provided

This feature builds on the existing parameter infrastructure and makes deployment workflows more intuitive, especially when working with multiple environments or sharing deployment scripts across team members.

### Azure Container App Jobs support

Aspire 9.5 introduces comprehensive support for Azure Container App Jobs, allowing you to deploy both project and container resources as background job workloads that can run on schedules, in response to events, or be triggered manually.

Container App Jobs complement the existing Container Apps functionality by providing a dedicated way to run finite workloads like data processing, ETL operations, batch jobs, and scheduled maintenance tasks.

#### Publishing resources as Container App Jobs

Use the new `PublishAsAzureContainerAppJob` extension method to publish resources as jobs:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Publish a project as a Container App Job
var dataProcessor = builder.AddProject<Projects.DataProcessor>("data-processor")
    .PublishAsAzureContainerAppJob((infrastructure, job) => {
        // Configure job-specific settings using Azure Provisioning APIs
        job.Configuration.TriggerType = TriggerType.Schedule;
        // Run daily at 2 AM
        job.Configuration.ScheduleTriggerConfig.CronExpression = "0 0 2 * * *";
    });

// Publish a container as a Container App Job  
var batchJob = builder.AddContainer("batch-job", "my-batch-image")
    .PublishAsAzureContainerAppJob((infrastructure, job) => {
        // Configure manual trigger job
        job.Configuration.TriggerType = TriggerType.Manual;
        job.Configuration.ReplicaRetryLimit = 3;
        job.Configuration.ReplicaTimeout = 1800; // 30 minutes
    });

builder.Build().Run();
```

#### Job customization and configuration

The new `AzureContainerAppJobCustomizationAnnotation` enables fine-grained control over job behavior:

```csharp
var scheduledJob = builder.AddProject<Projects.ScheduledWorker>("scheduled-worker")
    .PublishAsAzureContainerAppJob((infrastructure, job) => {
        // Event-driven job configuration
        job.Configuration.TriggerType = TriggerType.Event;
        job.Configuration.EventTriggerConfig = new EventTriggerConfiguration
        {
            Scale = new JobScale
            {
                MinExecutions = 0,
                MaxExecutions = 10,
                PollingInterval = 30 // seconds
            }
        };
        job.Configuration.ReplicaRetryLimit = 3;
        job.Configuration.ReplicaTimeout = 1800; // 30 minutes
    });
```

This feature addresses issue #4366 and provides a unified development and deployment experience for both long-running services (Container Apps) and finite workloads (Container App Jobs) within your Aspire applications.

#### Simplified job configuration overloads

For common scenarios, Aspire 9.5 provides simplified overloads that reduce boilerplate code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Scheduled job with cron expression
var scheduledJob = builder.AddProject<Projects.DailyProcessor>("daily-processor")
    .PublishAsAzureContainerAppJob("0 0 2 * * *"); // Run daily at 2 AM

// Manual job (default trigger type)
var manualJob = builder.AddContainer("batch-processor", "my-batch-image")
    .PublishAsAzureContainerAppJob(); // Manual trigger, ready for on-demand execution

builder.Build().Run();
```

These overloads provide convenient APIs for the most common job types while maintaining access to the full configuration API when advanced customization is needed.

### Azure provisioning & deployer

9.5 delivers the first iteration of the Azure provisioning & deployment pipeline that unifies interactive prompting, Bicep compilation, and mode-specific behavior (run vs publish) across Azure resources:

- New provisioning contexts separate run-mode and publish-mode flows ([#11094](https://github.com/dotnet/aspire/pull/11094)).
- Graph-based dependency planning (`ResourceDeploymentGraph`) ensures correct ordering of resource provisioning.
- Improved error handling and idempotency for `AddAsExistingResource` across all Azure resources ([#10562](https://github.com/dotnet/aspire/issues/10562)).
- Support for deploying compute images and resources (custom images referenced in your environment) ([#11030](https://github.com/dotnet/aspire/pull/11030)).
- Deploy individual Bicep modules instead of a monolithic `main.bicep` for clearer failure isolation and faster iteration ([#11098](https://github.com/dotnet/aspire/pull/11098)).
- Localized interaction + notification strings across all provisioning prompts (multiple OneLocBuild PRs).

Provisioning automatically prompts for required values only once per run, caches results, and reuses them in publish-mode without re-prompting. This reduces friction when iterating locally while maintaining reproducibility for production publish.

### Azure deployer interactive command handling

The AppHost now wires Azure provisioning prompts into the standard interaction system (initial work in [#10038](https://github.com/dotnet/aspire/pull/10038), extended in [#10792](https://github.com/dotnet/aspire/pull/10792) and [#10845](https://github.com/dotnet/aspire/pull/10845)). This enables:

- Consistent UX for parameter entry (names, descriptions, validation)
- Localized prompt text
- Support for non-interactive scenarios via pre-supplied parameters

### Azure resource idempotency & existing resources

Calling `AddAsExistingResource` is now idempotent across Azure hosting resource builders; repeated calls no longer cause duplicate annotations or inconsistent behavior ([#10562](https://github.com/dotnet/aspire/issues/10562)). This improves reliability when composing reusable extension methods.

### Compute image deployment

You can now reference and deploy custom compute images as part of Azure environment provisioning ([#11030](https://github.com/dotnet/aspire/pull/11030)). This lays groundwork for richer VM/container hybrid topologies.

### Module-scoped Bicep deployment

Instead of generating a single aggregated template, 9.5 deploys individual Bicep modules ([#11098](https://github.com/dotnet/aspire/pull/11098)). Failures surface with more precise context and partial successes require less rework.

### Publishing progress & activity reporting

`IPublishingActivityProgressReporter` was renamed to `IPublishingActivityReporter` and output formatting was reworked to provide clearer, structured progress (multiple commits culminating in improved messages). Expect more concise status lines and actionable error sections when using `aspire publish`.

### Parameter & interaction API updates

- `ParameterResource.Value` is now obsolete: switch to `await parameter.GetValueAsync()` or inject parameter resources directly ([#10363](https://github.com/dotnet/aspire/pull/10363)). This change improves async value acquisition and avoids accidental blocking.
- Interaction inputs enforce server-side validation and required `Name` property (breaking, [#10835](https://github.com/dotnet/aspire/pull/10835)).
- New notification terminology (renamed from MessageBar, [#10449](https://github.com/dotnet/aspire/pull/10449)).
- `ExecuteCommandResult` now includes a `Canceled` property to track whether command execution was canceled by the user or system.
- Server-side validation of interaction inputs ([#10527](https://github.com/dotnet/aspire/pull/10527)).

Migration example:

```csharp
// Before (deprecated)
var value = myParam.Value;

// After
var value = await myParam.GetValueAsync();
```

### InteractionInput API improvements

**Breaking change**: `InteractionInput` now requires `Name`; `Label` is optional (#10835).

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

This enables better form serialization and integration with interactive parameter processing.

### Executable resource configuration APIs

Enhanced APIs for configuring executable resources with command and working directory specifications.

#### WithCommand and WithWorkingDirectory APIs

New extension methods allow precise control over executable resource execution:

```csharp
// Configure executable with custom command and working directory
var processor = builder.AddExecutable("data-processor", "python")
    .WithCommand("main.py --batch-size 100")
    .WithWorkingDirectory("/app/data-processing")
    .WithArgs("--config", "production.json");

// Executable with specific working directory for relative paths
var buildTool = builder.AddExecutable("build-tool", "npm")
    .WithCommand("run build:production")
    .WithWorkingDirectory("./frontend");
```

#### Enhanced CommandLineArgsCallbackContext

The `CommandLineArgsCallbackContext` now includes resource information for context-aware argument building:

```csharp
var worker = builder.AddExecutable("worker", "dotnet")
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

### Interactive parameter processing APIs

Aspire 9.5 introduces the `ParameterProcessor` API for programmatic parameter resolution with interactive prompting capabilities.

#### ParameterProcessor API

The new experimental `ParameterProcessor` class enables applications to handle parameter resolution during runtime:

```csharp
// In your application startup or configuration
services.AddSingleton<ParameterProcessor>();

// Use parameter processor to initialize parameters
public async Task ConfigureAsync(ParameterProcessor processor)
{
    var parameters = new[]
    {
        builder.AddParameter("database-password", secret: true),
        builder.AddParameter("api-key", secret: true),
        builder.AddParameter("environment-name")
    };

    // Initialize parameters with interactive prompting
    await processor.InitializeParametersAsync(parameters, waitForResolution: true);
}
```

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

`AddDockerComposeEnvironment(...).WithDashboard()` gained `WithForwardedHeaders()` to enable forwarded `Host` and `Proto` handling for dashboard scenarios behind reverse proxies or compose networks ([#11080](https://github.com/dotnet/aspire/pull/11080)). This mirrors the standalone dashboard forwarded header support and fixes auth redirect edge cases.

```csharp
builder.AddDockerComposeEnvironment("env")
  .WithComposeFile("docker-compose.yml")
  .WithDashboard(d => d.WithForwardedHeaders());
```

### Container build customization

`ContainerBuildOptions` support (commit [#10074](https://github.com/dotnet/aspire/pull/10074)) enables customizing the underlying `dotnet publish` invocation when Aspire builds project-sourced container images (for example to change configuration, trimming, or pass additional MSBuild properties). Use the new options hook on the project container image configuration to set MSBuild properties instead of maintaining a custom Dockerfile. (Exact API surface is intentionally summarized here to avoid drift; see API docs for `ContainerBuildOptions` in the hosting namespace for usage.)

### Deployment image tag callbacks

Aspire 9.5 introduces powerful deployment image tag callback APIs that allow dynamic generation of container image tags at deployment time, supporting both synchronous and asynchronous scenarios.

#### Deployment tag callback features

- **Dynamic tag generation**: Calculate image tags based on deployment context, git commits, build numbers, or timestamps
- **Async callback support**: Perform asynchronous operations like API calls or file system access during tag generation
- **Deployment context access**: Access to deployment environment, resource information, and configuration
- **Flexible callback types**: Support for simple lambdas, context-aware callbacks, and async operations

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
