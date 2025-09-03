---
title: Write your first .NET Aspire test
description: Learn how to test your .NET Aspire solutions using the xUnit.net, NUnit, and MSTest testing frameworks.
ms.date: 8/15/2025
zone_pivot_groups: unit-testing-framework
---

# Write your first .NET Aspire test

In this article, you learn how to create a test project, write tests, and run them for your .NET Aspire solutions. The tests in this article aren't unit tests, but rather functional or integration tests. .NET Aspire includes several variations of [testing project templates](../fundamentals/setup-tooling.md#net-aspire-templates) that you can use to test your .NET Aspire resource dependencies—and their communications. The testing project templates are available for MSTest, NUnit, and xUnit.net testing frameworks and include a sample test that you can use as a starting point for your tests.

The .NET Aspire test project templates rely on the [📦 Aspire.Hosting.Testing](https://www.nuget.org/packages/Aspire.Hosting.Testing) NuGet package. This package exposes the <xref:Aspire.Hosting.Testing.DistributedApplicationTestingBuilder> class, which is used to create a test host for your distributed application. The distributed application testing builder launches your AppHost project with instrumentation hooks so that you can access and manipulate the host at various stages of its lifecyle. In particular, <xref:Aspire.Hosting.Testing.DistributedApplicationTestingBuilder> provides you access to <xref:Aspire.Hosting.IDistributedApplicationBuilder> and <xref:Aspire.Hosting.DistributedApplication> class to create and start the [AppHost](../fundamentals/app-host-overview.md).

## Create a test project

The easiest way to create a .NET Aspire test project is to use the testing project template. If you're starting a new .NET Aspire project and want to include test projects, the [Visual Studio tooling supports that option](../fundamentals/setup-tooling.md#create-test-project). If you're adding a test project to an existing .NET Aspire project, you can use the `dotnet new` command to create a test project:

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

For more information, see the .NET CLI [dotnet new](/dotnet/core/tools/dotnet-new) command documentation.

## Explore the test project

The following example test project was created as part of the **.NET Aspire Starter Application** template. If you're unfamiliar with it, see [Quickstart: Build your first .NET Aspire project](../get-started/build-your-first-aspire-app.md). The .NET Aspire test project takes a project reference dependency on the target AppHost. Consider the template project:

:::zone pivot="xunit"

:::code language="xml" source="snippets/testing/xunit/AspireApp.Tests/AspireApp.Tests.csproj":::

:::zone-end
:::zone pivot="mstest"

:::code language="xml" source="snippets/testing/mstest/AspireApp.Tests/AspireApp.Tests.csproj":::

:::zone-end
:::zone pivot="nunit"

:::code language="xml" source="snippets/testing/nunit/AspireApp.Tests/AspireApp.Tests.csproj":::

:::zone-end

The preceding project file is fairly standard. There's a `PackageReference` to the [📦 Aspire.Hosting.Testing](https://www.nuget.org/packages/Aspire.Hosting.Testing) NuGet package, which includes the required types to write tests for .NET Aspire projects.

The template test project includes a `IntegrationTest1` class with a single test. The test verifies the following scenario:

- The AppHost is successfully created and started.
- The `webfrontend` resource is available and running.
- An HTTP request can be made to the `webfrontend` resource and returns a successful response (HTTP 200 OK).

Consider the following test class:

:::zone pivot="xunit"

:::code language="csharp" source="snippets/testing/xunit/AspireApp.Tests/IntegrationTest1.cs":::

:::zone-end
:::zone pivot="mstest"

:::code language="csharp" source="snippets/testing/mstest/AspireApp.Tests/IntegrationTest1.cs":::

:::zone-end
:::zone pivot="nunit"

:::code language="csharp" source="snippets/testing/nunit/AspireApp.Tests/IntegrationTest1.cs":::

:::zone-end

The preceding code:

- Relies on the <xref:Aspire.Hosting.Testing.DistributedApplicationTestingBuilder.CreateAsync*?displayProperty=nameWithType> API to asynchronously create the AppHost.
  - The `appHost` is an instance of `IDistributedApplicationTestingBuilder` that represents the AppHost.
  - The `appHost` instance has its service collection configured with the standard HTTP resilience handler. For more information, see [Build resilient HTTP apps: Key development patterns](/dotnet/core/resilience/http-resilience).
- The `appHost` has its <xref:Aspire.Hosting.Testing.IDistributedApplicationTestingBuilder.BuildAsync(System.Threading.CancellationToken)?displayProperty=nameWithType> method invoked, which returns the `DistributedApplication` instance as the `app`.
  - The `app` has its service provider get the <xref:Aspire.Hosting.ApplicationModel.ResourceNotificationService> instance.
  - The `app` is started asynchronously.
- An <xref:System.Net.Http.HttpClient> is created for the `webfrontend` resource by calling `app.CreateHttpClient`.
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

- Relies on the <xref:Aspire.Hosting.Testing.DistributedApplicationTestingBuilder.CreateAsync*?displayProperty=nameWithType> API to asynchronously create the AppHost.
- The `builder` instance is used to retrieve an <xref:Aspire.Hosting.ApplicationModel.IResourceWithEnvironment> instance named "webfrontend" from the <xref:Aspire.Hosting.Testing.IDistributedApplicationTestingBuilder.Resources%2A?displayProperty=nameWithType>.
- The `webfrontend` resource is used to call <xref:Aspire.Hosting.ApplicationModel.ResourceExtensions.GetEnvironmentVariableValuesAsync%2A> to retrieve its configured environment variables.
- The <xref:Aspire.Hosting.DistributedApplicationOperation.Publish?displayProperty=nameWithType> argument is passed when calling `GetEnvironmentVariableValuesAsync` to specify environment variables that are published to the resource as binding expressions.
- With the returned environment variables, the test asserts that the `webfrontend` resource has an HTTPS environment variable that resolves to the `apiservice` resource.

## Capture logs from tests

When writing tests for your Aspire solutions, you might want to capture and view logs to help with debugging and monitoring test execution. The `DistributedApplicationTestingBuilder` provides access to the service collection, allowing you to configure logging for your test scenarios.

### Configure logging providers

To capture logs from your tests, use the `AddLogging` method on the `builder.Services` to configure logging providers specific to your testing framework:

:::zone pivot="xunit"

:::code language="csharp" source="snippets/testing/xunit/AspireApp.Tests/LoggingTest.cs":::

:::zone-end
:::zone pivot="mstest"

:::code language="csharp" source="snippets/testing/mstest/AspireApp.Tests/LoggingTest.cs":::

:::zone-end
:::zone pivot="nunit"

:::code language="csharp" source="snippets/testing/nunit/AspireApp.Tests/LoggingTest.cs":::

:::zone-end

### Configure log filters

Since the _appsettings.json_ configuration from your application isn't automatically replicated in test projects, you need to explicitly configure log filters. This is important to avoid excessive logging from infrastructure components that might overwhelm your test output. The following snippet explicitly configures log filters:

```csharp
builder.Services.AddLogging(logging => logging
    .AddFilter("Default", LogLevel.Information)
    .AddFilter("Microsoft.AspNetCore", LogLevel.Warning)
    .AddFilter("Aspire.Hosting.Dcp", LogLevel.Warning));
```

The preceding configuration:

- Sets the default log level to `Information` for most application logs.
- Reduces noise from ASP.NET Core infrastructure by setting it to `Warning` level.
- Limits Aspire hosting infrastructure logs to `Warning` level to focus on application-specific logs.

### Popular logging packages

Different testing frameworks have different logging provider packages available to assist with managing logging during test execution:

:::zone pivot="xunit"

xUnit.net doesn't capture log output from tests as test output. Tests must [use the `ITestOutputHelper` interface](https://xunit.net/docs/capturing-output) to achieve this.

For xUnit.net, consider using one of these logging packages:

- [📦 MartinCostello.Logging.XUnit](https://www.nuget.org/packages/MartinCostello.Logging.XUnit) - Outputs `ILogger` logs to `ITestOutputHelper` output.
- [📦 Xunit.DependencyInjection.Logging](https://www.nuget.org/packages/Xunit.DependencyInjection.Logging) - Integrates with xUnit.net's dependency injection.
- [📦 Serilog.Extensions.Logging.File](https://www.nuget.org/packages/Serilog.Extensions.Logging.File) - Writes logs to files.
- [📦 Microsoft.Extensions.Logging.Console](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Console) - Outputs logs to console.

:::zone-end
:::zone pivot="mstest"

For MSTest, consider using one of these logging packages:

- [📦 Extensions.Logging.MSTest](https://www.nuget.org/packages/Extensions.Logging.MSTest) - Integrates with MSTest framework.
- [📦 Serilog.Extensions.Logging.File](https://www.nuget.org/packages/Serilog.Extensions.Logging.File) - Writes logs to files.
- [📦 Microsoft.Extensions.Logging.Console](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Console) - Outputs logs to console.

:::zone-end
:::zone pivot="nunit"

For NUnit, consider using one of these logging packages:

- [📦 Extensions.Logging.NUnit](https://www.nuget.org/packages/Extensions.Logging.NUnit) - Integrates with NUnit framework.
- [📦 Serilog.Extensions.Logging.File](https://www.nuget.org/packages/Serilog.Extensions.Logging.File) - Writes logs to files.
- [📦 Microsoft.Extensions.Logging.Console](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Console) - Outputs logs to console.

:::zone-end

## Summary

The .NET Aspire testing project template makes it easier to create test projects for .NET Aspire solutions. The template project includes a sample test that you can use as a starting point for your tests. The `DistributedApplicationTestingBuilder` follows a familiar pattern to the <xref:Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory`1> in ASP.NET Core. It allows you to create a test host for your distributed application and run tests against it.

Finally, when using the `DistributedApplicationTestingBuilder` all resource logs are redirected to the `DistributedApplication` by default. The redirection of resource logs enables scenarios where you want to assert that a resource is logging correctly.

## See also

- [Unit testing C# in .NET using dotnet test and xUnit](/dotnet/core/testing/unit-testing-with-dotnet-test)
- [MSTest overview](/dotnet/core/testing/unit-testing-mstest-intro)
- [Unit testing C# with NUnit and .NET Core](/dotnet/core/testing/unit-testing-with-nunit)
