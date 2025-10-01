---
title: Access resources in Aspire tests
description: Learn how to access the resources from the Aspire AppHost in your tests.
ms.date: 02/24/2025
zone_pivot_groups: unit-testing-framework
ms.custom: sfi-ropc-nochange
---

# Access resources in Aspire tests

In this article, you learn how to access the resources from the Aspire AppHost in your tests. The AppHost represents the full application environment and contains all the resources that are available to the application. When writing functional or integration tests with Aspire, you might need to access these resources to verify the behavior of your application.

## Access HTTP resources

To access an HTTP resource, use the <xref:System.Net.Http.HttpClient> to request and receive responses. The <xref:Aspire.Hosting.DistributedApplication> and the <xref:Aspire.Hosting.Testing.DistributedApplicationFactory> both provide a <xref:Aspire.Hosting.Testing.DistributedApplicationFactory.CreateHttpClient*> method that's used to create an `HttpClient` instance for a specific resource, based on the resource name from the AppHost. This method also takes an optional `endpointName` parameter, so if the resource has multiple endpoints, you can specify which one to use.

## Access other resources

In a test, you might want to access other resources by the connection information they provide, for example, querying a database to verify the state of the data. For this, you use the <xref:Microsoft.Extensions.Configuration.ConfigurationExtensions.GetConnectionString*?displayProperty=nameWithType> method to retrieve the connection string for a resource, and then provide that to a client library within the test to interact with the resource.

## Ensure resources are available

Starting with Aspire 9, there's support for waiting on dependent resources to be available (via the [health check](../fundamentals/health-checks.md) mechanism). This is useful in tests that ensure a resource is available before attempting to access it. The <xref:Aspire.Hosting.ApplicationModel.ResourceNotificationService> class provides a <xref:Aspire.Hosting.ApplicationModel.ResourceNotificationService.WaitForResourceAsync*?displayProperty=nameWithType> method that's used to wait for a named resource to be available. This method takes the resource name and the desired state of the resource as parameters and returns a <xref:System.Threading.Tasks.Task> that yields back when the resource is available. You can access the <xref:Aspire.Hosting.ApplicationModel.ResourceNotificationService> via <xref:Aspire.Hosting.DistributedApplication.ResourceNotifications?displayProperty=nameWithType>, as in the following example.

> [!NOTE]
> It's recommended to provide a time-out when waiting for resources, to prevent the test from hanging indefinitely in situations where a resource never becomes available.

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
await app.ResourceNotifications.WaitForResourceAsync(
    "webfrontend",  
    KnownResourceStates.Running,
    cts.Token); 
```

A resource enters the <xref:Aspire.Hosting.ApplicationModel.KnownResourceStates.Running?displayProperty=nameWithType> state as soon as it starts executing, but this doesn't mean that it's ready to serve requests. If you want to wait for the resource to be ready to serve requests, and your resource has health checks, you can wait for the resource to become healthy by using the <xref:Aspire.Hosting.ApplicationModel.ResourceNotificationService.WaitForResourceHealthyAsync*?displayProperty=nameWithType> method.

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

await app.ResourceNotifications.WaitForResourceHealthyAsync(
    "webfrontend",
    cts.Token);
```

This resource-notification pattern ensures that the resources are available before running the tests, avoiding potential issues with the tests failing due to the resources not being ready.

## See also

- [Write your first Aspire test](./write-your-first-test.md)  
- [Managing the AppHost in Aspire tests](./manage-app-host.md)
