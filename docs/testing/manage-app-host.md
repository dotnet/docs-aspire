---
title: Managing the app host in .NET Aspire tests
description: Learn how to manage the app host in .NET Aspire tests.
ms.date: 11/09/2024
zone_pivot_groups: unit-testing-framework
---

# Managing the app host in .NET Aspire tests

When writing functional or integration tests with .NET Aspire, it is important to consider how the [app host](../fundamentals/app-host-overview.md) instance is managed across tests, since the app host represents the full application environment and thus can be expensive to create and tear down. In this article, you'll learn how to manage the app host instance in your .NET Aspire tests.

For writing tests with .NET Aspire, we'll use the [`Aspire.Hosting.Testing`](https://www.nuget.org/packages/Aspire.Hosting.Testing) NuGet package which contains some helper classes to manage the app host instance in your tests.

## Using the `DistributedApplicationTestingBuilder` class

In the [tutorial on writing your first test](./writing-your-first-test.md), we were introduced to the `DistributedApplicationTestingBuilder` class which can be used to create the app host instance:

```csharp
var appHost = await DistributedApplicationTestingBuilder
    .CreateAsync<Projects.AspireApp_AppHost>();
```

The `CreateAsync` method takes the type of the app host project reference as a generic parameter so that it is able to create the app host instance. This code can be excuted at the start of each test, but as a test suite grows larger it is recommended that the app host instance is created once and shared across tests.

:::zone pivot="xunit"

With xUnit, we implement the `IAsyncLifetime` interface on the test class to support asynchroneous setup and teardown of the app host instance. The `InitializeAsync` method is used to create the app host instance before the tests are run and the `DisposeAsync` method is used to dispose the app host once the tests are completed.

```csharp
public class WebTests : IAsyncLifetime
{
    private DistributedApplication _app;

    public async Task InitializeAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.AspireApp_AppHost>();

        _app = await appHost.BuildAsync();
    }

    public async Task DisposeAsync() => await _app.DisposeAsync();

    [Fact]
    public async Task GetWebResourceRootReturnsOkStatusCode()
    {
        // test code here
    }
}
```

:::zone-end
:::zone pivot="mstest"

With MSTest, we use the `ClassInitialize` and `ClassCleanup` attributes to static methods of the test class to provide the setup and teardown of the app host instance. The `ClassInitialize` method is used to create the app host instance before the tests are run and the `ClassCleanup` method is used to dispose the app host instance once the tests are completed.

```csharp
[TestClass]
public class WebTests
{
    private static DistributedApplication _app;

    [ClassInitialize]
    public static async Task ClassInitialize(TestContext context)
    {
       var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.AspireApp_AppHost>();

        _app = await appHost.BuildAsync();
    }
    
    [ClassCleanup]
    public static async Task ClassCleanup() => await _app.DisposeAsync();

    [TestMethod]
    public async Task GetWebResourceRootReturnsOkStatusCode()
    {
        // test code here
    }
}
```

:::zone-end
:::zone pivot="nunit"

With NUnit, we use the `OneTimeSetUp` and `OneTimeTearDown` attributes to methods of the test class to provide the setup and teardown of the app host instance. The `OneTimeSetUp` method is used to create the app host instance before the tests are run and the `OneTimeTearDown` method is used to dispose the app host instance once the tests are completed.

```csharp
public class WebTests
{
    private DistributedApplication _app;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
       var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.AspireApp_AppHost>();

        _app = await appHost.BuildAsync();
    }
    
    [OneTimeTearDown]
    public async Task OneTimeTearDown() => await _app.DisposeAsync();

    [Test]
    public async Task GetWebResourceRootReturnsOkStatusCode()
    {
        // test code here
    }
}
```

:::zone-end

By capturing the app host in a field when the test run is started, we can access it in each test without the need to recreate it, decreasing the time it takes to run the test suite. Then, when the test run has completed, we dispose of the app host, which will clean up any resources that were created during the test run, such as containers.

## Using the `DistributedApplicationFactory` class

While the `DistributedApplicationTestingBuilder` class is useful for many scenarios, there may be instances that we want to have more control over starting the app host, such as executing code before the builder is created or after the app host is built. In these cases, we can implement our own version of the `DistributedApplicationFactory` class. This is what the `DistributedApplicationTestingBuilder` uses internally.

```csharp
public class TestingAspireAppHost : DistributedApplicationFactory(typeof(Projects.AspireApp_AppHost))
{
    // override methods here
}
```

The constructor requires the type of the app host project reference as a parameter, and optionally we can provide arguments to the underlying host application builder which will control how it starts the app host and provide values to the `args` variable that the `Program.cs` file uses to start the app host instance.

### Lifecycle methods

The `DistributionApplicationFactory` class provides several lifecycle methods that can be overridden to provide custom behavior throughout the preperation and creation of the app host. The available methods are `OnBuilderCreating`, `OnBuilderCreated`, `OnBuilding` and `OnBuilt`. 

For example, we can use the `OnBuilderCreating` method to set environment variables, such as the subscription and resource group information for Azure, before the app host is created and any dependent Azure resources are provisioned, resulting in our tests using the correct Azure environment.

```csharp
public class TestingAspireAppHost : DistributedApplicationFactory(typeof(Projects.AspireApp_AppHost))
{
    protected override void OnBuilderCreating(DistributedApplicationOptions applicationOptions, HostApplicationBuilderSettings hostOptions)
    {
        builder.EnvironmentVariables["AZURE_SUBSCRIPTION_ID"] = "00000000-0000-0000-0000-000000000000";
        builder.EnvironmentVariables["AZURE_RESOURCE_GROUP"] = "my-resource-group";
    }
}
```

Because of the order of precedence in the .NET configuration system, the environment variables will be used over anything in the `appsettings.json` or `secrets.json` file.

Another scenario we might want to use in the lifecycle is to configure the services used by the app host. Here we're using the `OnBuilderCreated` to add resilliance to the `HttpClient`:

```csharp
protected override void OnBuilderCreated(DistributedApplicationBuilder applicationBuilder)
{
    applicationBuilder.Services.ConfigureHttpClientDefaults(clientBuilder =>
    {
        clientBuilder.AddStandardResilienceHandler();
    });
}
```

## See also

- [Writing your first .NET Aspire test](./writing-your-first-test.md)
