---
title: Aspire testing overview
description: Learn how Aspire helps you to test your applications.
ms.date: 03/11/2025
---

# Aspire testing overview

Aspire supports automated testing of your application through the [📦 Aspire.Hosting.Testing](https://www.nuget.org/packages/Aspire.Hosting.Testing) NuGet package. This package provides the <xref:Aspire.Hosting.Testing.DistributedApplicationTestingBuilder> class, which is used to create a test host for your application. The testing builder launches your AppHost project in a background thread and manages its lifecycle, allowing you to control and manipulate the application and its resources through <xref:Aspire.Hosting.Testing.DistributedApplicationTestingBuilder> or <xref:Aspire.Hosting.DistributedApplication> instances.

By default, the testing builder disables the dashboard and randomizes the ports of proxied resources to enable multiple instances of your application to run concurrently. Once your test completes, disposing of the application or testing builder cleans up your app resources.

To get started writing your first integration test with Aspire, see the [Write your first Aspire test](./write-your-first-test.md) article.

## Testing Aspire solutions

Aspire's testing capabilities are designed specifically for closed-box integration testing of your entire distributed application. Unlike unit tests or open-box integration tests, which typically run individual components in isolation, Aspire tests launch your complete solution (the AppHost and all its resources) as separate processes, closely simulating real-world scenarios.

Consider the following diagram that shows how the Aspire testing project starts the AppHost, which then starts the application and its resources:

:::image type="content" source="media/testing-diagram-thumb.png" alt-text="Aspire testing diagram" lightbox="media/testing-diagram.png":::

1. The **test project** starts the AppHost.
1. The **AppHost** process starts.
1. The **AppHost** runs the `Database`, `API`, and `Frontend` applications.
1. The **test project** sends an HTTP request to the `Frontend` application.

The diagram illustrates that the **test project** starts the AppHost, which then orchestrates all dependent app resources—regardless of their type. The test project is able to send an HTTP request to the `Frontend` app, which depends on an `API` app, and the `API` app depends on a `Database`. A successful request confirms that the `Frontend` app can communicate with the `API` app, and that the `API` app can successfully get data from the `Database`. For more information on seeing this approach in action, see the [Write your first Aspire test](write-your-first-test.md) article.

> [!IMPORTANT]
> Aspire testing doesn't enable scenarios for mocking, substituting, or replacing services in dependency injection—as the tests run in a separate process.

Use Aspire testing when you want to:

- Verify end-to-end functionality of your distributed application.
- Ensure interactions between multiple services and resources (such as databases) behave correctly in realistic conditions.
- Confirm data persistence and integration with real external dependencies, like a PostgreSQL database.

If your goal is to test a single project in isolation, run components in-memory, or mock external dependencies, consider using <xref:Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory%601> instead.

> [!NOTE]
> Aspire tests run your application as separate processes, meaning you don't have direct access to internal services or components from your test code. You can influence application behavior through environment variables or configuration settings, but internal state and services remain encapsulated within their respective processes.

## Disable port randomization

By default, Aspire uses random ports to allow multiple instances of your application to run concurrently without interference. It uses [Aspire's service discovery](../service-discovery/overview.md) to ensure applications can locate each other's endpoints. To disable port randomization, pass `"DcpPublisher:RandomizePorts=false"` when constructing your testing builder, as shown in the following snippet:

```csharp
var builder = await DistributedApplicationTestingBuilder
    .CreateAsync<Projects.MyAppHost>(
        [
            "DcpPublisher:RandomizePorts=false"
        ]);
```

## Enable the dashboard

The testing builder disables the [Aspire dashboard](../fundamentals/dashboard/overview.md) by default. To enable it, you can set the `DisableDashboard` property to `false`, when creating your testing builder as shown in the following snippet:

```csharp
var builder = await DistributedApplicationTestingBuilder
    .CreateAsync<Projects.MyAppHost>(
        args: [],
        configureBuilder: (appOptions, hostSettings) =>
        {
            appOptions.DisableDashboard = false;
        });
```

## See also

- [Write your first Aspire test](./write-your-first-test.md)
- [Managing the AppHost in Aspire tests](./manage-app-host.md)
- [Access resources in Aspire tests](./accessing-resources.md)
