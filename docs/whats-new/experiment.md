---
title: What's new in .NET Aspire 9.4
description: Learn what's new in the official general availability release of .NET Aspire 9.4.
ms.date: 07/25/2025
---

# What's new in .NET Aspire 9.4

📢 .NET Aspire 9.4 continues the evolution of cloud-native development with .NET, bringing enhanced developer productivity and streamlined deployment experiences. This release supports:

- .NET 8.0 Long Term Support (LTS)
- .NET 9.0 Standard Term Support (STS)
- .NET 10.0 Preview

If you have feedback, questions, or want to contribute to .NET Aspire, collaborate with us on [:::image type="icon" source="../media/github-mark.svg" border="false"::: GitHub](https://github.com/dotnet/aspire) or join us on [:::image type="icon" source="../media/discord-icon.svg" border="false"::: Discord](https://aka.ms/dotnet-discord) to chat with team members.

It's important to note that .NET Aspire releases out-of-band from .NET releases. While major versions of .NET Aspire align with major .NET versions, minor versions are released more frequently. For more information on .NET and .NET Aspire version support, see:

- [.NET support policy](https://dotnet.microsoft.com/platform/support/policy): Definitions for LTS and STS.
- [.NET Aspire support policy](https://dotnet.microsoft.com/platform/support/policy/aspire): Important unique product life cycle details.

## 🖥️ App model enhancements

### ✨ External service resources

Managing connections to external services like existing databases, APIs, or third-party endpoints has been simplified with the introduction of external service resources. This enables seamless integration of any external service into your Aspire app model with full support for service discovery and configuration management.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Connect to an existing external API
var externalApi = builder.AddExternalService("payment-api", "https://api.example.com/payment");

// Reference the external service from your project
builder.AddProject<Projects.WebApp>("webapp")
       .WithReference(externalApi);

builder.Build().Run();
```

External service resources provide a clean, consistent way to model dependencies on services that exist outside your Aspire application, making hybrid architectures and gradual migrations much more straightforward.

### 🔧 Enhanced YARP configuration

Building on the Yet Another Reverse Proxy (YARP) integration introduced in 9.3, this release adds powerful programmatic configuration capabilities that complement the existing JSON-based approach. You can now configure YARP routing rules, clusters, and policies directly in your app model.

```mermaid
flowchart LR
    subgraph Client
        A[User Browser]
    end
    subgraph ReverseProxy
        B[Reverse Proxy (YARP)]
    end
    subgraph Services
        C[Catalog Service]
        D[Basket Service]
    end

    A -- HTTP Request /catalog/... --> B
    A -- HTTP Request /basket/... --> B
    B -- Forwards /catalog/... --> C
    B -- Forwards /basket/... --> D
    C -- Response --> B
    D -- Response --> B
    B -- HTTP Response --> A
```

This diagram shows how a reverse proxy like YARP receives client requests, routes them to the appropriate backend service, and returns the response to the client.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var catalogService = builder.AddProject<Projects.CatalogService>("catalog");
var basketService = builder.AddProject<Projects.BasketService>("basket");

// Configure YARP with fluent API
builder.AddYarp("apigateway")
       .WithReference(catalogService)
       .WithReference(basketService)
       .WithRoute("catalog", route => route
           .Match(path: "/catalog/{**catch-all}")
           .ToCluster("catalog-cluster"))
       .WithRoute("basket", route => route
           .Match(path: "/basket/{**catch-all}")  
           .ToCluster("basket-cluster"));
```

This approach provides strongly-typed configuration while maintaining the flexibility to use traditional YARP JSON configuration files when needed.

### 🏗️ Enhanced resource lifecycle management

.NET Aspire 9.4 introduces improved lifecycle event handling and resource state management, building on the foundation established in 9.3. Resources now have more predictable initialization patterns and better support for custom startup sequences.

```csharp
builder.Eventing.Subscribe<InitializeResourceEvent>(myResource, async (e, ct) =>
{
    // Custom initialization logic
    await SetupCustomResource(e.Resource);
    
    // Signal the resource is ready
    await e.Notifications.PublishUpdateAsync(e.Resource,
        s => s with { State = KnownResourceStates.Running });
});
```

<!-- TODO: Fix this as it is incorrect. -->

## 📊 Dashboard delights

### ✨ Interactive command execution

The dashboard now supports interactive command execution through a new backchannel communication system. This enables direct interaction with running services, parameter validation, and real-time feedback without leaving the dashboard environment.

Key capabilities include:

- **Real-time parameter prompting**: Interactive dialogs for collecting user input
- **Command validation**: Built-in validation with helpful error messages
- **Progress tracking**: Visual feedback for long-running operations
- **Markdown support**: Rich formatting in interactive messages

For more information, see [Interaction Service (Preview)](../extensibility/interaction-service.md).

### 🔍 Enhanced trace visualization

Trace details now include correlated log entries, providing a more complete picture of application behavior. When viewing a trace, you can see relevant log messages that occurred during the same timespan, making debugging and performance analysis more effective.

### 🎛️ Improved resource filtering

Resource filtering in the dashboard has been enhanced with better persistence and more granular control options. Filter states are maintained across sessions, and you can now create custom filter sets for different debugging scenarios.

## 🚀 Deployment & publish

### 🔄 Modernized publishing architecture

The publishing model has been significantly refined to provide more granular control and better progress reporting. The new architecture supports:

- **Step-by-step progress tracking**: Detailed visibility into each phase of the publishing process
- **Enhanced error handling**: Better error messages with suggested remediation steps
- **Parallel operations**: Improved performance through concurrent resource processing
- **Extensible hooks**: Custom logic injection at various publishing stages

### ☸️ Enhanced Kubernetes support

Kubernetes manifest generation has been expanded with better customization options and improved resource mapping. You can now:

- **Customize deployment strategies**: Control rolling updates, replica counts, and resource limits
- **Configure persistent volumes**: Streamlined storage configuration for stateful workloads
- **Set custom annotations**: Add metadata for monitoring, security, or operational tooling

```csharp
builder.AddKubernetesEnvironment("k8s")
       .WithProperties(env =>
       {
           env.DefaultImagePullPolicy = "Always";
           env.DefaultNamespace = "aspire-apps";
       });

builder.AddProject<Projects.ApiService>("api")
       .PublishAsKubernetesService(resource =>
       {
           resource.Deployment!.Spec.Replicas = 3;
           resource.Deployment.Spec.Strategy = new()
           {
               Type = "RollingUpdate",
               RollingUpdate = new() { MaxUnavailable = 1 }
           };
       });
```

### 🐳 Docker Compose improvements

Docker Compose support has been enhanced with better parameter binding and more flexible configuration options:

- **Environment variable placeholders**: Dynamic configuration through build-time parameters
- **Service customization**: Programmatic control over container settings
- **Network configuration**: Improved container networking and service discovery

## 🖥️ CLI enhancements

### ✨ Enhanced app host discovery

The CLI now features intelligent app host project discovery that walks up the directory tree and caches results for faster subsequent operations. This makes it easier to run Aspire commands from anywhere within your solution.

### 🔄 Interactive command execution

The `aspire exec` command enables direct interaction with running resources from the command line, supporting scenarios like database migrations, cache clearing, or custom maintenance tasks.

### 📊 Progress reporting improvements

Command execution now provides detailed progress feedback with step-by-step status updates, making it easier to understand what's happening during complex operations like publishing or deployment.

## ☁️ Azure goodies

### 🤖 Azure AI Foundry integration

.NET Aspire 9.4 introduces support for Azure AI Foundry, Microsoft's comprehensive AI development platform. This integration enables seamless deployment and management of AI-powered applications.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add Azure AI Foundry integration
var aiFoundry = builder.AddAzureAIFoundry("ai-foundry");

// Configure the foundry with appropriate roles
builder.AddProject<Projects.ChatService>("chat")
       .WithReference(aiFoundry);
```

The integration provides:

- **Automatic role assignment**: Proper Azure RBAC configuration for AI services
- **Endpoint management**: Streamlined configuration of AI service endpoints
- **Secure authentication**: Managed identity integration for production deployments

### 🔐 Enhanced Azure SQL security model

Azure SQL integration has been updated with improved multi-application security handling. Instead of the previous "last one wins" administrator model, each application now receives appropriate database-level permissions while maintaining security isolation.

### 🗄️ Expanded Azure Storage capabilities

The Azure Storage integration has been refactored to provide more granular control over blob containers, queues, and tables with improved client injection patterns.

```csharp
var storage = builder.AddAzureStorage("storage");
var blobs = storage.AddBlobs("blobs");
var imageContainer = blobs.AddBlobContainer("images");

builder.AddProject<Projects.ImageService>("images")
       .WithReference(imageContainer);
```

### 🔑 Key Vault enhancements

Azure Key Vault integration now supports direct environment variable injection for secrets, making it easier to securely configure applications without hardcoding sensitive values.

```csharp
var keyVault = builder.AddAzureKeyVault("secrets");
var apiSecret = keyVault.Resource.GetSecret("third-party-api-key");

builder.AddContainer("worker", "myapp/worker")
       .WithEnvironment("API_KEY", apiSecret);
```

## 🧩 Component integrations

### 📊 Azure AI Inference client support

New client integration for Azure AI Inference services provides both direct SDK access and integration with Microsoft.Extensions.AI abstractions for flexible AI application development.

```csharp
// Register the Azure AI Inference client
builder.AddAzureChatCompletionsClient("ai-inference")
       .AddChatClient(); // Enables IChatClient injection

// Use in minimal API
app.MapPost("/chat", async (IChatClient chatClient, ChatRequest request) =>
{
    var response = await chatClient.GetResponseAsync(request.Message);
    return Results.Ok(response);
});
```

### ⚙️ Azure App Configuration integration

Centralized configuration management is now supported through Azure App Configuration integration, enabling dynamic feature flags and configuration updates without application restarts.

```csharp
// Add Azure App Configuration
builder.AddAzureAppConfiguration("appconfig");

// Configuration is automatically available through IConfiguration
app.MapGet("/feature-status", (IConfiguration config) =>
{
    var isEnabled = config.GetValue<bool>("Features:NewUI");
    return Results.Ok(new { NewUIEnabled = isEnabled });
});
```

### 🔐 Expanded Key Vault client types

Additional Azure Key Vault client integrations provide specialized access to keys and certificates alongside the existing secrets client.

```csharp
// Register specialized Key Vault clients
builder.AddAzureKeyVaultKeyClient("kv");
builder.AddAzureKeyVaultCertificateClient("kv");

// Use for cryptographic operations
app.MapPost("/sign", async (KeyClient keyClient, SignRequest request) =>
{
    var key = await keyClient.GetKeyAsync("signing-key");
    // Perform signing operation
});
```

## 💔 Breaking changes

With every release, we strive to make .NET Aspire better. However, some changes may break existing functionality. The following breaking changes are introduced in .NET Aspire 9.4:

### 🛡️ Azure SQL security model changes

The Azure SQL integration now uses a more secure multi-application access model. Applications receive `db_owner` role permissions instead of server administrator rights. Review your access requirements if your application relied on the previous administrator access pattern.

### 🏗️ Publishing architecture updates

The publishing architecture has been modernized with new interfaces and patterns. If you have custom publishers or publishing extensions, you may need to update them to use the new `IPublishingActivityReporter` interfaces and step-based progress reporting.

For complete details on breaking changes, see:

- [Breaking changes in .NET Aspire 9.4](../compatibility/9.4/index.md)
