---
title: Accessing resources in .NET Aspire tests
description: Learn how to access the resources from the .NET Aspire app host in your tests.
ms.date: 11/09/2024
zone_pivot_groups: unit-testing-framework
---

# Accessing resources in .NET Aspire tests

In this article, you'll learn how to access the resources from the .NET Aspire app host in your tests. The app host represents the full application environment and contains all the resources that are available to the application. When writing functional or integration tests with .NET Aspire, you may need to access these resources to verify the behavior of your application.

## Accessing HTTP resources

To access an HTTP resource, we can use a `HttpClient` to interact with them, performing requests and receiving responses. The `DistributedApplication` and the `DistributionApplicationFactory` both provide a `CreateHttpClient` method that can be used to create an `HttpClient` instance for a specific resource, based off the resource name from the app host. This method also takes an optional `endpointName` parameter, so if the resource has multiple endpoints, we can specify which one to use.

## Accessing other resources

In a test, we may want to access other resources by the connection information they provide, for example, quering a database to verify the state of the data. For this, we can use the `GetConnectionString` method to retrieve the connection string for a resource, and then provide that to a client library within the test to interact with the resource.

## Ensuring resources are available

With the release of .NET Aspire 9 there is support for waiting for dependent resources to be available (via the [health check](../fundamentals/health-checks.md) mechanism). This can be useful in tests to ensure that the resources are available before running the tests. The `ResourceNotificationService` class provides a `WaitForResourcesAsync` method that can be used to wait for the resources to be available. This method takes the resource name and the desired state of the resource as parameters, and returns a `Task` that can be awaited to wait for the resource to be available. It is recommended to provide a timeout when waiting for resources, to prevent the test from hanging indefinitely if the resources are not available.

```csharp
await resourceNotificationService.WaitForResourceAsync(
    "webfrontend",
    KnownResourceStates.Running
)
.WaitAsync(TimeSpan.FromSeconds(30));
```

This pattern ensures that the resources are available before running the tests, avoiding potential issues with the tests failing due to the resources not being ready.

## See also

- [Writing your first .NET Aspire test](./writing-your-first-test.md)
- [Managing the app host in .NET Aspire tests](./manage-app-host.md)
