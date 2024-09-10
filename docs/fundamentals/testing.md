---
title: Testing .NET Aspire solutions
description: Learn how to test your .NET Aspire solutions using the xUnit, NUnit, and MSTest testing frameworks.
ms.date: 09/09/2024
zone_pivot_groups: unit-testing-framework
---

# Testing .NET Aspire solutions

In this article, you learn how to create a test project, write tests, and run them for your .NET Aspire solutions. The tests in this article aren't unit tests, but rather functional or integration tests. .NET Aspire includes several variations of [testing project templates](setup-tooling.md#net-aspire-project-templates) that you can use to test your .NET Aspire resource dependencies—and their communications. The testing project templates are available for MSTest, NUnit, and xUnit testing frameworks and include a sample test that you can use as a starting point for your tests.

The .NET Aspire test project templates rely on the [Aspire.Hosting.Testing](https://www.nuget.org/packages/Aspire.Hosting.Testing) NuGet package. This package exposes the `DistributedApplicationTestingBuilder` class, which is used to create a test host for your distributed application. The distributed application testing builder relies on the `DistributedApplication` class to create and start the [app host](app-host-overview.md).

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

:::zone pivot="xunit"

:::code language="xml" source="snippets/testing/xunit/AspireApp.Tests/AspireApp.Tests.csproj":::

:::zone-end
:::zone pivot="mstest"

:::code language="xml" source="snippets/testing/mstest/AspireApp.Tests/AspireApp.Tests.csproj":::

:::zone-end
:::zone pivot="nunit"

:::code language="xml" source="snippets/testing/nunit/AspireApp.Tests/AspireApp.Tests.csproj":::

:::zone-end

The preceding project file is fairly standard. There's a `PackageReference` to the [Aspire.Hosting.Testing](https://www.nuget.org/packages/Aspire.Hosting.Testing) NuGet package, which includes the required types to write tests for .NET Aspire projects.

The template test project includes a `WebTests` class with a single test. The test verifies the following scenario:

- The app host is successfully created and started.
- The `webfrontend` resource is available and running.
- An HTTP request can be made to the `webfrontend` resource and returns a successful response (HTTP 200 OK).

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

## Test resource environment variables

To further test resources and their expressed dependencies in your .NET Aspire solution, you can assert that environment variables are injected correctly. The following example demonstrates how to test that the `webfrontend` resource has an HTTPS environment variable that resolves to the `apiservice` resource:

:::zone pivot="xunit"

:::code language="csharp" source="snippets/testing/xunit/AspireApp.Tests/EnvVarTests.cs":::

:::zone-end
:::zone pivot="mstest"

:::code language="csharp" source="snippets/testing/mstest/AspireApp.Tests/EnvVarTests.cs":::

:::zone-end
:::zone pivot="nunit"

:::code language="csharp" source="snippets/testing/nunit/AspireApp.Tests/EnvVarTests.cs":::

:::zone-end

The preceding code:

- Relies on the `DistributedApplicationTestingBuilder` to asynchronously create the app host.
- The `builder` instance is used to retrieve an `IResourceWithEnvironment` instance named "webfrontend" from the <xref:Aspire.Hosting.Testing.IDistributedApplicationTestingBuilder.Resources%2A?displayProperty=nameWithType>.
- The `webfrontend` resource is used to call <xref:Aspire.Hosting.ApplicationModel.ResourceExtensions.GetEnvironmentVariableValuesAsync%2A> to retrieve its configured environment variables.
- The `DistributedApplicationOperation.Publish` argument is passed when calling `GetEnvironmentVariableValuesAsync` to specify environment variables that are published to the resource as binding expressions.
- With the returned environment variables, the test asserts that the `webfrontend` resource has an HTTPS environment variable that resolves to the `apiservice` resource.

## Summary

The .NET Aspire testing project template makes it easier to create test projects for .NET Aspire solutions. The template project includes a sample test that you can use as a starting point for your tests. The `DistributedApplicationTestingBuilder` follows a familiar pattern to the `WebApplicationFactory` in ASP.NET Core. It allows you to create a test host for your distributed application and run tests against it.

Finally, when using the `DistributedApplicationTestingBuilder` all resource logs are redirected to the `DistributedApplication` by default. The redirection of resource logs enables scenarios where you want to assert that a resource is logging correctly.

## See also

- [Unit testing C# in .NET using dotnet test and xUnit](/dotnet/core/testing/unit-testing-with-dotnet-test)
- [MSTest overview](/dotnet/core/testing/unit-testing-mstest-intro)
- [Unit testing C# with NUnit and .NET Core](/dotnet/core/testing/unit-testing-with-nunit)
