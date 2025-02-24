---
title: Manage the app host in .NET Aspire tests
description: Learn how to manage the app host in .NET Aspire tests.
ms.date: 2/24/2025
zone_pivot_groups: unit-testing-framework
---

# Manage the app host in .NET Aspire tests

When writing functional or integration tests with .NET Aspire, it's important to consider how the [app host](../fundamentals/app-host-overview.md) instance is managed across tests, since the app host represents the full application environment and thus can be expensive to create and tear down. In this article, you'll learn how to manage the app host instance in your .NET Aspire tests.

For writing tests with .NET Aspire, you use the [📦 `Aspire.Hosting.Testing`](https://www.nuget.org/packages/Aspire.Hosting.Testing) NuGet package which contains some helper classes to manage the app host instance in your tests.

## Use the `DistributedApplicationTestingBuilder` class

In the [tutorial on writing your first test](./write-your-first-test.md), you were introduced to the <xref:Aspire.Hosting.Testing.DistributedApplicationTestingBuilder> class which can be used to create the app host instance:

```csharp
var appHost = await DistributedApplicationTestingBuilder
    .CreateAsync<Projects.AspireApp_AppHost>();
```

The <xref:Aspire.Hosting.Testing.DistributedApplicationTestingBuilder.CreateAsync*> method takes the type of the app host project reference as a generic-type parameter so that it's able to create the app host instance. This code is executed at the start of each test, but as a test suite grows larger it's recommended that the app host instance is created once and shared across tests.

:::zone pivot="xunit"

With xUnit, you implement the [IAsyncLifetime](https://github.com/xunit/xunit/blob/master/src/xunit.core/IAsyncLifetime.cs) interface on the test class to support asynchronous initialization and disposal of the app host instance. The `InitializeAsync` method is used to create the app host instance before the tests are run and the `DisposeAsync` method disposes the app host once the tests are completed.

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

With MSTest, you use the <xref:Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute> and <xref:Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute> on static methods of the test class to provide the initialization and cleanup of the app host instance. The `ClassInitialize` method is used to create the app host instance before the tests are run and the `ClassCleanup` method disposes the app host instance once the tests are completed.

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

With NUnit, you use the [OneTimeSetUp](https://docs.nunit.org/articles/nunit/writing-tests/attributes/onetimesetup.html) and [OneTimeTearDown](https://docs.nunit.org/articles/nunit/writing-tests/attributes/onetimeteardown.html) attributes on methods of the test class to provide the setup and teardown of the app host instance. The `OneTimeSetUp` method is used to create the app host instance before the tests are run and the `OneTimeTearDown` method disposes the app host instance once the tests are completed.

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

By capturing the app host in a field when the test run is started, you can access it in each test without the need to recreate it, decreasing the time it takes to run the tests. Then, when the test run has completed, the app host is disposed, which will clean up any resources that were created during the test run, such as containers.

## Passing arguments to your app host

You can access the args from your app host via the `args` parameter. Arguments are also passed to .NET's configuration system, so you can override many configuration settings this way. In the following example, we override the [environment](/aspnet/core/fundamentals/environments) by specifying it as a command line option:

```csharp
var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.MyAppHost>(["--environment=Testing"]);
```

Other arguments can be passed to your app host program and made available in your app host. In the next example, we pass an argument to the app host and use it to control whether we add data volumes to a postgres instance.

In the app host program, we use configuration to support enabling or disabling volumes:

```csharp
var postgres = builder.AddPostgres("postgres1");
if (builder.Configuration.GetValue("UseVolumes", true))
{
    postgres.WithDataVolume();
}
```

In test code, we pass `"UseVolumes=false"` in the args to the app host:

```csharp
public async Task DisableVolumesFromTest()
{
    // Disable volumes in the test builder via arguments:
    using var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.TestingAppHost1_AppHost>(["UseVolumes=false"]);

    // The container will have no volume annotation since we disabled volumes by passing UseVolumes=false
    var postgres = builder.Resources.Single(r => r.Name == "postgres1");
    Assert.Empty(postgres.Annotations.OfType<ContainerMountAnnotation>());
}
```

## Use the `DistributedApplicationFactory` class

While the `DistributedApplicationTestingBuilder` class is useful for many scenarios, there might be situations where you want more control over starting the app host, such as executing code before the builder is created or after the app host is built. In these cases, you implement your own version of the <xref:Aspire.Hosting.Testing.DistributedApplicationFactory> class. This is what the `DistributedApplicationTestingBuilder` uses internally.

```csharp
public class TestingAspireAppHost
    : DistributedApplicationFactory(typeof(Projects.AspireApp_AppHost))
{
    // override methods here
}
```

The constructor requires the type of the app host project reference as a parameter. Optionally, you can provide arguments to the underlying host application builder. These arguments control how the app host starts and provide values to the args variable used by the _Program.cs_ file to start the app host instance.

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

Because of the order of precedence in the .NET configuration system, the environment variables will be used over anything in the _appsettings.json_ or _secrets.json_ file.

Another scenario you might want to use in the lifecycle is to configure the services used by the app host. In the following example, consider a scenario where you override the `OnBuilderCreated` API to add resilience to the `HttpClient`:

```csharp
protected override void OnBuilderCreated(
    DistributedApplicationBuilder applicationBuilder)
{
    applicationBuilder.Services.ConfigureHttpClientDefaults(clientBuilder =>
    {
        clientBuilder.AddStandardResilienceHandler();
    });
}
```

## See also

- [Write your first .NET Aspire test](./write-your-first-test.md)
