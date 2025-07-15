---
title: Annotations in .NET Aspire
description: Learn about annotations in .NET Aspire, how they work, and how to create custom annotations for extending resource behavior.
ms.date: 01/20/2025
ms.topic: conceptual
---

# Annotations in .NET Aspire

Annotations are a key extensibility mechanism in .NET Aspire that allow you to attach metadata and behavior to resources. They provide a way to customize how resources are configured, deployed, and managed throughout the application lifecycle. This article explains how annotations work and how to use them effectively in your .NET Aspire applications.

## What are annotations

Annotations in .NET Aspire are objects that implement the <xref:Aspire.Hosting.ApplicationModel.IResourceAnnotation> interface. They're attached to resources to provide additional metadata, configuration, or behavior. Annotations are consumed by various parts of the .NET Aspire stack, including:

- The dashboard for displaying custom URLs and commands
- Deployment tools for generating infrastructure as code
- The hosting layer for configuring runtime behavior
- Testing infrastructure for resource inspection

Every annotation is associated with a specific resource and can contain any data or behavior needed to extend that resource's functionality.

## How annotations work

When you add a resource to your app model, you can attach annotations using various extension methods. These annotations are stored with the resource and can be retrieved and processed by different components of the system.

Here's a simple example of how annotations are used:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("cache")
    .WithCommand("clear-cache", "Clear Cache", OnClearCache)
    .WithUrl("admin", "http://localhost:8080/admin");

builder.Build().Run();
```

In this example:

- `WithCommand` adds a `CommandAnnotation` that defines a custom command
- `WithUrl` adds a `ResourceUrlAnnotation` that defines a custom URL

## Built-in annotation types

.NET Aspire includes several built-in annotation types for common scenarios:

### EndpointAnnotation

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

### ResourceUrlAnnotation

The <xref:Aspire.Hosting.ApplicationModel.ResourceUrlAnnotation> defines custom URLs that appear in the dashboard, often pointing to management interfaces or documentation.

```csharp
var database = builder.AddPostgres("postgres")
    .WithUrl("admin", "https://localhost:5050");
```

### EnvironmentCallbackAnnotation

The <xref:Aspire.Hosting.ApplicationModel.EnvironmentCallbackAnnotation> allows you to modify environment variables at runtime based on the state of other resources.

```csharp
// This is typically used internally by WithReference
var app = builder.AddProject<Projects.MyApp>("app")
    .WithReference(database);
```

### ContainerMountAnnotation

The `ContainerMountAnnotation` defines volume mounts for containerized resources.

```csharp
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume(); // Adds a ContainerMountAnnotation
```

### CommandAnnotation

The `CommandAnnotation` defines custom commands that can be executed from the dashboard.

```csharp
var redis = builder.AddRedis("cache")
    .WithCommand("clear", "Clear Cache", OnClearCache);
```

## Creating custom annotations

You can create custom annotations by implementing the <xref:Aspire.Hosting.ApplicationModel.IResourceAnnotation> interface. Here's how to create a custom annotation:

### 1. Define the annotation class

:::code source="snippets/annotations-overview/Program.cs" id="CustomConfigAnnotation":::

### 2. Create extension methods

Create extension methods to make your annotation easy to use:

:::code source="snippets/annotations-overview/Program.cs" id="CustomConfigExtensions":::

### 3. Use the custom annotation

```csharp
var api = builder.AddProject<Projects.Api>("api")
    .WithCustomConfig("feature-flag", "enabled");
```

### 4. Process the annotation

Create services or hooks to process your custom annotations:

:::code source="snippets/annotations-overview/Program.cs" id="CustomConfigProcessor":::

## Advanced annotation patterns

### Conditional annotations

You can add annotations conditionally based on environment or configuration:

```csharp
var apiBuilder = builder.AddProject<Projects.Api>("api");

if (builder.Environment.IsDevelopment())
{
    apiBuilder.WithUrl("swagger", "/swagger");
}
```

### Annotation composition

Combine multiple annotations to create complex behaviors:

```csharp
public static IResourceBuilder<T> WithMonitoring<T>(
    this IResourceBuilder<T> builder)
    where T : class, IResource
{
    return builder
        .WithUrl("metrics", "/metrics")
        .WithUrl("health", "/health");
}
```

### Annotation validation

Validate annotations to ensure they're correctly configured:

:::code source="snippets/annotations-overview/Program.cs" id="ValidatedAnnotation":::

## Accessing annotations

You can access annotations on resources for inspection or processing:

:::code source="snippets/annotations-overview/Program.cs" id="AccessingAnnotations":::

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
    Assert.NotEmpty(resource.Annotations.OfType<CustomConfigAnnotation>());

    // Assert annotation properties
    var config = resource.Annotations
        .OfType<CustomConfigAnnotation>()
        .First();
    
    Assert.Equal("expected-value", config.ConfigValue);
}
```

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
    where T : class, IResource
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
