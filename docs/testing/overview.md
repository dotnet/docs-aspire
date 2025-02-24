---
title: .NET Aspire testing overview
description: Learn how .NET Aspire helps you to test your applications.
ms.date: 2/24/2025
zone_pivot_groups: unit-testing-framework
---

# .NET Aspire testing overview

.NET Aspire includes support for automated testing of your application through the [📦 Aspire.Hosting.Testing](https://www.nuget.org/packages/Aspire.Hosting.Testing) NuGet package. This package exposes the <xref:Aspire.Hosting.Testing.DistributedApplicationTestingBuilder> class, which is used to create a test host for your application. The testing builder launches your app host project in a background thread and instruments its lifecycle to pass control back to your <xref:Aspire.Hosting.Testing.DistributedApplicationTestingBuilder> or <xref:Aspire.Hosting.DistributedApplication> instances, allowing you to access and manipulate the application and its resources. Aside from instrumentation, the testing builder disables the dashboard by default and randomizes the ports of proxied resources so that multiple instances of your application can run concurrently. Once your test has completed, disposing the application or testing builder will clean up your app resources.

To get started writing your first integration test with .NET Aspire, see the [Write your first .NET Aspire test](./write-your-first-test.md) article.

## Disabling port randomization

The testing builder will randomize port assignment for proxied endpoints by default during testing to allow multiple instances of your app to run concurrently without interfering with each other, relying on [.NET Aspire's service discovery](../service-discovery/overview.md) mechanism so that applications are able to find each other's endpoints. To disable port randomization, pass `"DcpPublisher:RandomizePorts=false"` as an argument when constructing your testing builder, as in the following snippet:

```csharp
var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.MyAppHost>(["DcpPublisher:RandomizePorts=false"]);
```

## Enabling the dashboard

The testing builder disables the [.NET Aspire dashboard](../fundamentals/dashboard) by default. To enable it, you can set the `DisableDashboard` property to `false`, when creating your testing builder as in this snippet:

```csharp
var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.MyAppHost>(args: [], configureBuilder: (appOptions, hostSettings) =>
{
    appOptions.DisableDashboard = false;
});
```

## See also

- [Write your first .NET Aspire test](./write-your-first-test.md)
- [Managing the app host in .NET Aspire tests](./manage-app-host.md)
- [Access resources in .NET Aspire tests](./accessing-resources.md)