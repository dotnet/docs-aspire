---
title: .NET Aspire testing overview
description: Learn how .NET Aspire helps you to test your applications.
ms.date: 02/24/2025
---

# .NET Aspire testing overview

.NET Aspire supports automated testing of your application through the [📦 Aspire.Hosting.Testing](https://www.nuget.org/packages/Aspire.Hosting.Testing) NuGet package. This package provides the <xref:Aspire.Hosting.Testing.DistributedApplicationTestingBuilder> class, which is used to create a test host for your application. The testing builder launches your app host project in a background thread and manages its lifecycle, allowing you to control and manipulate the application and its resources through <xref:Aspire.Hosting.Testing.DistributedApplicationTestingBuilder> or <xref:Aspire.Hosting.DistributedApplication> instances.

By default, the testing builder disables the dashboard and randomizes the ports of proxied resources to enable multiple instances of your application to run concurrently. Once your test completes, disposing of the application or testing builder cleans up your app resources.

To get started writing your first integration test with .NET Aspire, see the [Write your first .NET Aspire test](./write-your-first-test.md) article.

## Disable port randomization

By default, .NET Aspire uses random ports to allow multiple instances of your application to run concurrently without interference. It uses [.NET Aspire's service discovery](../service-discovery/overview.md) to ensure applications can locate each other's endpoints. To disable port randomization, pass `"DcpPublisher:RandomizePorts=false"` when constructing your testing builder, as shown in the following snippet:

```csharp
var builder = await DistributedApplicationTestingBuilder
    .CreateAsync<Projects.MyAppHost>(
        [
            "DcpPublisher:RandomizePorts=false"
        ]);
```

## Enable the dashboard

The testing builder disables the [.NET Aspire dashboard](../fundamentals/dashboard) by default. To enable it, you can set the `DisableDashboard` property to `false`, when creating your testing builder as shown in the following snippet:

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

- [Write your first .NET Aspire test](./write-your-first-test.md)
- [Managing the app host in .NET Aspire tests](./manage-app-host.md)
- [Access resources in .NET Aspire tests](./accessing-resources.md)
