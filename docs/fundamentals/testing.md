---
title: Testing .NET Aspire projects
description: Learn how to test your .NET Aspire projects using the xUnit testing framework.
ms.date: 07/26/2024
zone_pivot_groups: unit-testing-framework
---

# Testing .NET Aspire projects

In this article, you'll learn how to create a test project, write, and run tests for your .NET Aspire projects. The tests in this article are not unit tests, but rather functional or integration tests. .NET Aspire include an [testing project templates](setup-tooling.md#net-aspire-project-templates) that you can use to test your .NET Aspire projects. The testing project templates are available for MSTest, NUnit, and xUnit testing frameworks and includes a sample test that you can use as a starting point for your tests.

## Create a test project

The easiest way to create a .NET Aspire test project is to use the testing project template. If you're starting a new .NET Aspire project and want to include test projects, the [Visual Studio tooling supports that option](setup-tooling.md#create-test-project). If you're adding a test project to an existing .NET Aspire project, you can use the `dotnet new` command to create a test project:

:::zone pivot="xunit"

```dotnetcli
dotnet new aspire-xunit
```

:::zone-end
:::zone pivot="mstest"

```dotnetcli
dotnet new aspire-mstest
```

:::zone-end
:::zone pivot="nunit"

```dotnetcli
dotnet new aspire-nunit
```

:::zone-end

## Explore the test project

The following example test project was created as part of the **.NET Aspire Starter Application** template. If you're unfamiliar with it, see [Quickstart: Build your first .NET Aspire project](../get-started/build-your-first-aspire-app.md). The .NET Aspire test project takes a project reference dependency on the target app host. Consider the template project:

:::code language="xml" source="snippets/testing/AspireApp1/AspireApp1.Tests/AspireApp1.Tests.csproj":::

The preceding project file is fairly standard. There's a `PackageReference` to the [Aspire.Hosting.Testing](https://www.nuget.org/packages/Aspire.Hosting.Testing) NuGet package, which includes the required types to write tests for .NET Aspire projects.

The template test project includes a `WebTests` class with a single test fact. The test fact verifies the following scenario:

- The app host is successfully created and started.
- An HTTP request can be made to the `webfrontend` resource and returns a successful response.

Consider the following test class:

:::zone pivot="xunit"

:::code language="csharp" source="snippets/testing/xunit/AspireApp.Tests/WebTests.cs":::

:::zone-end
:::zone pivot="mstest"

:::code language="csharp" source="snippets/testing/mstest/AspireApp.Tests/WebTests.cs":::

:::zone-end
:::zone pivot="nunit"

:::code language="csharp" source="snippets/testing/nunit/AspireApp.Tests/WebTests.cs":::

:::zone-end

The preceding code:

- Relies on the `DistributedApplicationTestingBuilder` to asynchronously create the app host.
  - The `appHost` is an instance of `IDistributedApplicationTestingBuilder` that represents the app host.
  - The `appHost` instance has its service collection configured with the standard HTTP resilience handler. For more information, see [Build resilient HTTP apps: Key development patterns](/dotnet/core/resilience/http-resilience).
- The `appHost` has its `BuildAsync` method invoked, which returns the `DistributedApplication` instance as the `app`.
  - The `app` has its service provider get the <xref:Aspire.Hosting.ApplicationModel.ResourceNotificationService> instance.
  - The `app` is started asynchronously.
- An `HttpClient` is created for the `webfrontend` resource by calling `app.CreateHttpClient`.
- The `resourceNotificationService` is used to wait for the `webfrontend` resource to be available and running.
- A simple HTTP GET request is made to the root of the `webfrontend` resource.
- The test asserts that the response status code is `OK`.

## Summary

By using the .NET Aspire testing project template, you can easily create test projects for your .NET Aspire projects. The template project includes a sample test that you can use as a starting point for your tests. The `DistributedApplicationTestingBuilder` follows a familiar pattern to the `WebApplicationFactory` in ASP.NET Core. It allows you to create a test host for your distributed application and run tests against it.
