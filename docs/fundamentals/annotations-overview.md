---
title: Resource annotations
description: Learn about annotations in .NET Aspire, how they work, and how to create custom annotations for extending resource behavior.
ms.date: 07/25/2025
---

# Resource annotations in .NET Aspire

Annotations are a key extensibility mechanism in .NET Aspire that allow you to attach metadata and behavior to resources. They provide a way to customize how resources are configured, deployed, and managed throughout the application lifecycle. This article explains how annotations work and how to use them effectively in your .NET Aspire applications.

## What are annotations

Annotations in .NET Aspire are objects that implement the <xref:Aspire.Hosting.ApplicationModel.IResourceAnnotation> interface. They're attached to resources to provide additional metadata, configuration, or behavior. Annotations are consumed by various parts of the .NET Aspire stack, including:

- The dashboard for displaying custom URLs and commands.
- Deployment tools for generating infrastructure as code.
- The hosting layer for configuring runtime behavior.
- Testing infrastructure for resource inspection.

Every annotation is associated with a specific resource and can contain any data or behavior needed to extend that resource's functionality.

## How annotations work

When you add a resource to your app model, you can attach annotations using various extension methods. These annotations are stored with the resource and can be retrieved and processed by different components of the system.

Here's a simple example of how annotations are used:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("cache")
    .WithCommand("clear-cache", "Clear Cache", 
        async context => new ExecuteCommandResult { Success = true })
    .WithUrl("admin", "http://localhost:8080/admin");

builder.Build().Run();
```

In this example:

- <xref:Aspire.Hosting.ResourceBuilderExtensions.WithCommand*> adds a <xref:Aspire.Hosting.ApplicationModel.ResourceCommandAnnotation> that defines a custom command.
- <xref:Aspire.Hosting.ResourceBuilderExtensions.WithUrl*> adds a <xref:Aspire.Hosting.ApplicationModel.ResourceUrlAnnotation> that defines a custom URL.

## Built-in annotation types

.NET Aspire includes many built-in annotation types for common scenarios. This section covers _some_ of the more commonly used annotations, but there are [_many more_](xref:Aspire.Hosting.ApplicationModel.IResourceAnnotation) available for specific use cases.

### `EndpointAnnotation`

The <xref:Aspire.Hosting.ApplicationModel.EndpointAnnotation> defines network endpoints for resources. It contains information about ports, protocols, and endpoint configuration.

```csharp
var api = builder.AddProject<Projects.Api>("api")
    .WithEndpoint(callback: endpoint =>
    {
        endpoint.Port = 5000;
        endpoint.IsExternal = true;
        endpoint.Protocol = Protocol.Tcp;
        endpoint.Transport = "http";
    });
```

### `ResourceUrlAnnotation`

The <xref:Aspire.Hosting.ApplicationModel.ResourceUrlAnnotation> defines custom URLs that appear in the dashboard, often pointing to management interfaces or documentation.

```csharp
var database = builder.AddPostgres("postgres")
    .WithUrl("admin", "https://localhost:5050");
```

### `EnvironmentCallbackAnnotation`

The <xref:Aspire.Hosting.ApplicationModel.EnvironmentCallbackAnnotation> allows you to modify environment variables at runtime based on the state of other resources.

```csharp
// This is typically used internally by WithReference
var app = builder.AddProject<Projects.MyApp>("app")
    .WithReference(database);
```

### `ContainerMountAnnotation`

The `ContainerMountAnnotation` defines volume mounts for containerized resources.

```csharp
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume(); // Adds a ContainerMountAnnotation
```

### `PublishingCallbackAnnotation`

The <xref:Aspire.Hosting.ApplicationModel.PublishingCallbackAnnotation> allows you to register callbacks that execute during the publishing process. This is useful for performing custom operations when resources are being published.

```csharp
var api = builder.AddProject<Projects.Api>("api")
    .WithPublishingCallback(async context =>
    {
        // Custom logic during publishing
        await CustomPublishLogicAsync(context);
    });
```

For more information, see <xref:Aspire.Hosting.ResourceBuilderExtensions.WithPublishingCallback*>.

### `DeployingCallbackAnnotation`

<!-- <xref:Aspire.Hosting.ApplicationModel.DeployingCallbackAnnotation> -->

The `DeployingCallbackAnnotation` allows you to register callbacks that execute during the deployment process. This annotation is used internally by deployment tools to customize resource deployment behavior.

```csharp
var api = builder.AddProject<Projects.Api>("api");

api.Resource.Annotations.Add(
    new DeployingCallbackAnnotation(async context =>
    {
        // Custom deployment logic
        await ConfigureDeploymentAsync(context);
    });
);
```

For more information about publishing and deploying Aspire apps, see [publishing and deploying](../deployment/overview.md).

## Creating custom annotations

Custom annotations in .NET Aspire are designed to capture resource-specific metadata and behavior that can be leveraged throughout the application lifecycle. They're commonly used by:

- **Extension methods** to infer user intent and configure resource behavior.
- **Deployment tools** to generate deployment-specific configuration.
- **Lifecycle hooks** to query the app model and make runtime decisions.
- **Development tools** like the dashboard to display resource information.

Custom annotations should focus on a single concern and provide clear value to consumers of the resource metadata.

### 1. Define the annotation class

Start by creating a class that implements `IResourceAnnotation`. Focus on capturing specific metadata that other components need to reason about your resource:

:::code source="snippets/annotations-overview/Program.cs" id="ServiceMetricsAnnotation":::

### 2. Create extension methods

Create fluent extension methods that follow .NET Aspire's builder pattern. These methods should make it easy for AppHost authors to configure your annotation:

:::code source="snippets/annotations-overview/Program.cs" id="ServiceMetricsExtensions":::

The extension methods serve as the primary API surface for your annotation. They should:

- Follow naming conventions (start with `With` for additive operations).
- Provide sensible defaults.
- Return the builder for method chaining.
- Include comprehensive XML documentation.

### 3. Use the custom annotation

Once defined, annotations integrate seamlessly into the AppHost model:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.Api>("api")
    .WithMetrics("/api/metrics", 9090, "environment:production", "service:api");
```

## Testing with annotations

When writing tests, you can inspect annotations to verify resource configuration:

```csharp
[Fact]
public async Task Resource_Should_Have_Expected_Annotations()
{
    var appHost = await DistributedApplicationTestingBuilder
        .CreateAsync<Projects.MyApp_AppHost>();

    await using var app = await appHost.BuildAsync();

    var resource = app.Resources.GetResource("my-resource");

    // Assert that specific annotations exist
    Assert.NotEmpty(resource.Annotations.OfType<ServiceMetricsAnnotation>());

    // Assert annotation properties
    var metricsConfig = resource.Annotations
        .OfType<ServiceMetricsAnnotation>()
        .First();
    
    Assert.True(metricsConfig.Enabled);
    Assert.Equal("/metrics", metricsConfig.MetricsPath);
}
```

For more information, see [Testing with Aspire](../testing/overview.md).

## Best practices

When working with annotations, consider these best practices:

### Use meaningful names

Choose descriptive names for your annotation classes and properties:

```csharp
// Good
public sealed class DatabaseConnectionPoolAnnotation : IResourceAnnotation

// Avoid
public sealed class DbAnnotation : IResourceAnnotation
```

### Follow the builder pattern

Create fluent extension methods that follow .NET Aspire's builder pattern:

```csharp
var resource = builder.AddMyResource("name")
    .WithCustomBehavior()
    .WithAnotherFeature();
```

### Document your annotations

Provide XML documentation for custom annotations and extension methods:

```csharp
/// <summary>
/// Configures custom caching behavior for the resource.
/// </summary>
/// <param name="builder">The resource builder.</param>
/// <param name="ttl">The time-to-live for cached items.</param>
/// <returns>The resource builder for chaining.</returns>
public static IResourceBuilder<T> WithCaching<T>(
    this IResourceBuilder<T> builder,
    TimeSpan ttl)
    where T : class, IResource { /*...*/ }
```

### Keep annotations simple

Annotations should be focused on a single responsibility:

```csharp
// Good - single responsibility
public sealed class RetryPolicyAnnotation : IResourceAnnotation
{
    public int MaxRetries { get; set; }
    public TimeSpan Delay { get; set; }
}

// Avoid - multiple responsibilities
public sealed class ConfigAnnotation : IResourceAnnotation
{
    public RetryPolicy RetryPolicy { get; set; }
    public LoggingSettings Logging { get; set; }
    public SecuritySettings Security { get; set; }
}
```

## Next steps

- Learn about [custom resource commands](custom-resource-commands.md)
- Explore [custom resource URLs](custom-resource-urls.md)
- See [create custom hosting integrations](../extensibility/custom-hosting-integration.md)
- Review the [.NET Aspire app model API reference](/dotnet/api/aspire.hosting.applicationmodel)
