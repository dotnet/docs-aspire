---
title: Customize deployments with Aspire
description: Learn how to use Aspire's APIs to customize various aspects of the deployment process, including resource configuration, health probes, lifecycle events, and deployment-specific settings.
ms.date: 09/23/2025
ai-usage: ai-assisted
---

# Customize deployments with Aspire

Aspire provides APIs to customize how your applications are deployed. This article demonstrates how to use these APIs to configure deployment-specific settings, including dynamic image tagging, resource configuration, and deployment behaviors.

## Prerequisites

- Aspire 9.5 or later
- An existing Aspire project

For information about creating an Aspire project, see [Build your first Aspire app](../../get-started/build-your-first-aspire-app.md).

## Dynamic container image tagging

Use the `WithDeploymentImageTag` method to dynamically generate container image tags at deployment time. This enables flexible versioning strategies based on deployment context, git commits, timestamps, or external systems. Aspire's built-in Azure deployer will use the deployment image tag when pushing images to the remote container registry provisioned for your deployment.

### Basic usage

Configure static or computed tags for your resources:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Static tag
var api = builder.AddProject<Projects.Api>("api")
    .WithDeploymentImageTag(() => "v1.2.3-stable");

// Dynamic tag with timestamp
var worker = builder.AddProject<Projects.Worker>("worker")
    .WithDeploymentImageTag(() => $"build-{DateTime.UtcNow:yyyyMMdd-HHmm}");

builder.Build().Run();
```

### Async tag generation

Use async callbacks for complex tag generation scenarios:

```csharp
var service = builder.AddProject<Projects.Service>("service")
    .WithDeploymentImageTag(async context =>
    {
        // Fetch version from external API
        var buildInfo = await GetBuildInfoFromApi();
        return $"service-{buildInfo.Version}";
    });
```

### Apply tags to all resources using eventing

Use the eventing APIs to automatically apply deployment image tags to all resources in your application. This approach is useful when you want consistent tagging across all containerized resources:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Subscribe to the BeforeStartEvent to configure all resources
builder.Eventing.Subscribe<BeforeStartEvent>(async (evt, ct) =>
{
    var timestamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
    foreach (var resource in evt.Model.Resources)
    {
        builder.CreateResourceBuilder(resource).WithDeploymentImageTag((context) =>
        {
            var resourceName = resource.Name.ToLowerInvariant();
            return $"{resourceName}-{timestamp}";
        });
    }
});

builder.Build().Run();
```

## Customize Azure Bicep resources

Use the `ConfigureInfrastructure` method to customize the underlying Azure Bicep templates that Aspire generates during deployment. This allows you to modify Azure resource properties, add custom configurations, or integrate with existing Azure infrastructure.

### Basic infrastructure customization

Customize Azure resources by modifying their Bicep properties:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Customize Azure Service Bus configuration
var serviceBus = builder.AddAzureServiceBus("servicebus");
serviceBus.AddServiceBusQueue("queue1");
serviceBus.ConfigureInfrastructure(infrastructure =>
{
    var queue = infrastructure.GetProvisionableResources().OfType<ServiceBusQueue>().Single(q => q.BicepIdentifier == "queue1");
    queue.MaxDeliveryCount = 5;
    queue.LockDuration = TimeSpan.FromMinutes(5);
});
```

## Related content

- [Deploy to Azure Container Apps using aspire deploy](aca-deployment-aspire-cli.md)
- [App host overview](../../fundamentals/app-host-overview.md)
