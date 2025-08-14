---
title: Manage the AppHost in .NET Aspire tests
description: Learn how to manage the AppHost in .NET Aspire tests.
ms.date: 02/24/2025
zone_pivot_groups: unit-testing-framework
---

# Manage the AppHost in .NET Aspire tests

When writing functional or integration tests with .NET Aspire, managing the [AppHost](../fundamentals/app-host-overview.md) instance efficiently is crucial. The AppHost represents the full application environment and can be costly to create and tear down. This article explains how to manage the AppHost instance in your .NET Aspire tests.

For writing tests with .NET Aspire, you use the [📦 `Aspire.Hosting.Testing`](https://www.nuget.org/packages/Aspire.Hosting.Testing) NuGet package which contains some helper classes to manage the AppHost instance in your tests.

## Use the `DistributedApplicationTestingBuilder` class

In the [tutorial on writing your first test](./write-your-first-test.md), you were introduced to the <xref:Aspire.Hosting.Testing.DistributedApplicationTestingBuilder> class which can be used to create the AppHost instance:

```csharp
var appHost = await DistributedApplicationTestingBuilder
    .CreateAsync<Projects.AspireApp_AppHost>();
```

The `DistributedApplicationTestingBuilder.CreateAsync<T>` method takes the AppHost project type as a generic parameter to create the AppHost instance. While this method is executed at the start of each test, it's more efficient to create the AppHost instance once and share it across tests as the test suite grows.

:::zone pivot="xunit"

With xUnit, you implement the [IAsyncLifetime](https://github.com/xunit/xunit/blob/master/src/xunit.core/IAsyncLifetime.cs) interface on the test class to support asynchronous initialization and disposal of the AppHost instance. The `InitializeAsync` method is used to create the AppHost instance before the tests are run and the `DisposeAsync` method disposes the AppHost once the tests are completed.

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

With MSTest, you use the <xref:Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute> and <xref:Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute> on static methods of the test class to provide the initialization and cleanup of the AppHost instance. The `ClassInitialize` method is used to create the AppHost instance before the tests are run and the `ClassCleanup` method disposes the AppHost instance once the tests are completed.

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

With NUnit, you use the [OneTimeSetUp](https://docs.nunit.org/articles/nunit/writing-tests/attributes/onetimesetup.html) and [OneTimeTearDown](https://docs.nunit.org/articles/nunit/writing-tests/attributes/onetimeteardown.html) attributes on methods of the test class to provide the setup and teardown of the AppHost instance. The `OneTimeSetUp` method is used to create the AppHost instance before the tests are run and the `OneTimeTearDown` method disposes the AppHost instance once the tests are completed.

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

By capturing the AppHost in a field when the test run is started, you can access it in each test without the need to recreate it, decreasing the time it takes to run the tests. Then, when the test run completes, the AppHost is disposed, which cleans up any resources that were created during the test run, such as containers.

## Pass arguments to your AppHost

You can access the arguments from your AppHost with the `args` parameter. Arguments are also passed to [.NET's configuration system](/dotnet/core/extensions/configuration), so you can override many configuration settings this way. In the following example, you override the [environment](/aspnet/core/fundamentals/environments) by specifying it as a command line option:

```csharp
var builder = await DistributedApplicationTestingBuilder
    .CreateAsync<Projects.MyAppHost>(
    [
        "--environment=Testing"
    ]);
```

Other arguments can be passed to your AppHost `Program` and made available in your AppHost. In the next example, you pass an argument to the AppHost and use it to control whether you add data volumes to a Postgres instance.

In the AppHost `Program`, you use configuration to support enabling or disabling volumes:

```csharp
var postgres = builder.AddPostgres("postgres1");
if (builder.Configuration.GetValue("UseVolumes", true))
{
    postgres.WithDataVolume();
}
```

In test code, you pass `"UseVolumes=false"` in the `args` to the AppHost:

```csharp
public async Task DisableVolumesFromTest()
{
    // Disable volumes in the test builder via arguments:
    using var builder = await DistributedApplicationTestingBuilder
        .CreateAsync<Projects.TestingAppHost1_AppHost>(
        [
            "UseVolumes=false"
        ]);

    // The container will have no volume annotation since we disabled volumes by passing UseVolumes=false
    var postgres = builder.Resources.Single(r => r.Name == "postgres1");

    Assert.Empty(postgres.Annotations.OfType<ContainerMountAnnotation>());
}
```

## Use the `DistributedApplicationFactory` class

While the `DistributedApplicationTestingBuilder` class is useful for many scenarios, there might be situations where you want more control over starting the AppHost, such as executing code before the builder is created or after the AppHost is built. In these cases, you implement your own version of the <xref:Aspire.Hosting.Testing.DistributedApplicationFactory> class. This is what the `DistributedApplicationTestingBuilder` uses internally.

```csharp
public class TestingAspireAppHost()
    : DistributedApplicationFactory(typeof(Projects.AspireApp_AppHost))
{
    // override methods here
}
```

The constructor requires the type of the AppHost project reference as a parameter. Optionally, you can provide arguments to the underlying host application builder. These arguments control how the AppHost starts and provide values to the args variable used by the _Program.cs_ file to start the AppHost instance.

### Lifecycle methods

The `DistributionApplicationFactory` class provides several lifecycle methods that can be overridden to provide custom behavior throughout the preparation and creation of the AppHost. The available methods are `OnBuilderCreating`, `OnBuilderCreated`, `OnBuilding`, and `OnBuilt`.

For example, we can use the `OnBuilderCreating` method to set configuration, such as the subscription and resource group information for Azure, before the AppHost is created and any dependent Azure resources are provisioned, resulting in our tests using the correct Azure environment.

```csharp
public class TestingAspireAppHost() : DistributedApplicationFactory(typeof(Projects.AspireApp_AppHost))
{
    protected override void OnBuilderCreating(DistributedApplicationOptions applicationOptions, HostApplicationBuilderSettings hostOptions)
    {
        hostOptions.Configuration ??= new();
        hostOptions.Configuration["environment"] = "Development";
        hostOptions.Configuration["AZURE_SUBSCRIPTION_ID"] = "00000000-0000-0000-0000-000000000000";
        hostOptions.Configuration["AZURE_RESOURCE_GROUP"] = "my-resource-group";
    }
}
```

Because of the order of precedence in the .NET configuration system, the environment variables will be used over anything in the _appsettings.json_ or _secrets.json_ file.

Another scenario you might want to use in the lifecycle is to configure the services used by the AppHost. In the following example, consider a scenario where you override the `OnBuilderCreated` API to add resilience to the `HttpClient`:

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
