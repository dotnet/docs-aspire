---
title: Create non-container custom resources
description: Learn how to create custom .NET Aspire resources that don't rely on containers using lifecycle hooks and dashboard integration.
ms.date: 06/18/2025
ms.topic: how-to
---

# Create non-container custom resources

While many .NET Aspire resources are container-based, you can also create custom resources that run in-process or manage external services without containers. This article shows how to build a non-container custom resource that integrates with the Aspire dashboard using lifecycle hooks, status notifications, and logging.

## When to use non-container resources

Before creating a custom resource, consider whether your scenario might be better served by simpler approaches:

- **Connection strings only**: If you just need to connect to an external service, <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> might suffice.
- **Configuration values**: For simple configuration, <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddParameter*> might be enough.

Custom non-container resources are valuable when you need:

- Dashboard integration with status updates and logs
- Lifecycle management (starting/stopping services)
- In-process services that benefit from Aspire's orchestration
- External resource management with rich feedback

Examples include:

- In-process HTTP proxies or middleware
- Cloud service provisioning and management
- External API integrations with health monitoring
- Background services that need dashboard visibility

## Key components

Non-container custom resources use these key Aspire services:

- **<xref:Aspire.Hosting.Lifecycle.IDistributedApplicationLifecycleHook>**: Hook into app startup/shutdown
- **<xref:Microsoft.Extensions.Logging.ILogger>**: Standard .NET logging that appears in console and dashboard

> [!NOTE]
> Advanced dashboard integration is possible using services like `ResourceNotificationService` and `ResourceLoggerService` for real-time status updates and log streaming. These APIs provide richer dashboard experiences but require more complex implementation.

## Example: HTTP proxy resource

This example creates an in-process HTTP proxy resource that demonstrates the core concepts of lifecycle management and logging integration with the Aspire dashboard.

### Define the resource

First, create the resource class:

:::code language="csharp" source="snippets/HttpProxyResource/HttpProxy.Hosting/HttpProxyResource.cs":::

The resource implements <xref:Aspire.Hosting.ApplicationModel.IResource> and includes properties for the proxy configuration.

### Create the extension method

Next, create the builder extension:

:::code language="csharp" source="snippets/HttpProxyResource/HttpProxy.Hosting/HttpProxyResourceBuilderExtensions.cs":::

This extension method adds the resource to the application model and configures an HTTP endpoint.

### Implement lifecycle management

Create a lifecycle hook to manage the proxy:

:::code language="csharp" source="snippets/HttpProxyResource/HttpProxy.Hosting/HttpProxyLifecycleHook.cs":::

The lifecycle hook:

1. **Manages lifecycle**: Starts services when resources are created
2. **Integrates logging**: Uses standard .NET logging that appears in the Aspire dashboard
3. **Handles background tasks**: Runs long-running services in background tasks
4. **Provides resource management**: Manages resources like HTTP listeners and cleanup

### Register the lifecycle hook

The extension method automatically registers the lifecycle hook:

```csharp
public static IResourceBuilder<HttpProxyResource> AddHttpProxy(
    this IDistributedApplicationBuilder builder,
    string name,
    string targetUrl,
    int? port = null)
{
    var resource = new HttpProxyResource(name, targetUrl);
    
    // Register the lifecycle hook for this resource type
    builder.Services.TryAddSingleton<HttpProxyLifecycleHook>();
    builder.Services.AddLifecycleHook<HttpProxyLifecycleHook>();

    return builder.AddResource(resource)
                  .WithHttpEndpoint(port: port, name: "http");
}
```

### Use the resource

Now you can use the proxy in your app host:

:::code language="csharp" source="snippets/HttpProxyResource/HttpProxySample.AppHost/Program.cs":::

## Dashboard integration

The resource integrates with the Aspire dashboard through:

### Standard logging

Use standard .NET logging patterns that automatically appear in the dashboard:

```csharp
_logger.LogInformation("Starting HTTP proxy {ResourceName} -> {TargetUrl}", 
    resource.Name, resource.TargetUrl);
_logger.LogError(ex, "Failed to start HTTP proxy {ResourceName}", resource.Name);
```

### Advanced dashboard features

For more sophisticated dashboard integration, you can use:

- **Status notifications**: Update resource state and properties in real-time
- **Log streaming**: Send structured logs directly to the dashboard
- **Health monitoring**: Report resource health and performance metrics

These advanced features require additional Aspire hosting APIs and more complex implementation patterns.

## Best practices

When creating non-container resources:

1. **Resource cleanup**: Always implement proper disposal in lifecycle hooks
2. **Error handling**: Catch and log exceptions, update status appropriately  
3. **Status updates**: Provide meaningful status information to users
4. **Performance**: Avoid blocking operations in lifecycle methods
5. **Dependencies**: Use dependency injection for required services

## Summary

Non-container custom resources extend .NET Aspire beyond containers to include in-process services and external resource management. By implementing lifecycle hooks and integrating with the dashboard through status notifications and logging, you can create rich development experiences for any type of resource your application needs.

## Next steps

> [!div class="nextstepaction"]
> [Create custom .NET Aspire client integrations](custom-client-integration.md)
