---
title: .NET Aspire preview 6
description: .NET Aspire preview 6 is now available and includes many improvements and new capabilities.
ms.date: 04/22/2024
---

# .NET Aspire preview 6

.NET Aspire preview 6 introduces changes that improve the security and reliability of .NET Aspire. The following article provides an overview of the major changes in .NET Aspire preview 6: **8.0.0-preview.6.24214.1**.

To update to the latest version, run the following commands:

```dotnetcli
dotnet workload update
dotnet workload install aspire
```

To validate that preview 6 is installed, run `dotnet workload list` to see the installed workloads:

```dotnetcli
dotnet workload list

Installed Workload Id    Manifest Version                   Installation Source
--------------------------------------------------------------------------------------------
aspire                   8.0.0-preview.6.24214.1/8.0.100    SDK 8.0.200, VS 17.10.34804.81
```

## Breaking API changes

As we march towards a stable release, we've made several breaking changes to the API to make them more consistent and easier to use. Here are some of the breaking changes in this release:

- [Service Discovery API](https://github.com/dotnet/aspnetcore/issues/53715)
- [We've removed all obsolete APIs](https://github.com/dotnet/aspire/pull/3329)

We expect there to be a few more breaking changes before we lock down the API for the stable release.

## Security improvements

We've made several changes to improve the security of .NET Aspire. This includes securing the communication between many of the components (the orchestrator and IDE and the dashboard) to all use TLS and use API keys for authentication. This prevents unauthorized users from accessing potentially sensitive information exposed by the app host project and the dashboard when running locally on your machine.

### Dashboard authentication

The biggest user-facing change this release is the addition of authentication to the dashboard.

Data displayed in the dashboard can be sensitive. For example, configuration can include secrets in environment variables, and telemetry can include sensitive runtime data. To protect this data, the dashboard now requires authentication, even when running in your local development environment.

:::image type="content" source="media/preview-6/login.png" lightbox="media/preview-6/login.png" alt-text=".NET Aspire dashboard login page":::

If you run your .NET Aspire apps from Visual Studio or the C# Dev Kit for Visual Studio Code, then you'll never see the login page. Aspire tooling has been updated to log you in automatically.

If you run your .NET Aspire apps from the command line, you'll need to login the first time you access the dashboard. The required token is printed in console output, so copy and paste it into the login page and click Login. A persistent auth cookie is saved to the browser. Hopefully you won't need to see the login page often.

:::image type="content" source="media/preview-6/console-token.png" lightbox="media/preview-6/console-token.png" alt-text=".NET Aspire console with login token highlighted":::

For more information, see [Explore the .NET Aspire Dashboard: Authentication](../fundamentals/dashboard/explore.md#dashboard-authentication).

### New resources and components

The following are the new resources and components added in this release:

**Aspire.Hosting.Qdrant**: Provides a resource definition for a .NET Aspire AppHost to configure a Qdrant vector database resource.
**Aspire.Qdrant.Client**: Provides a client library for interacting with a Qdrant vector database.

From within an app host project, you can add a Qdrant resource like this:

```csharp
var qdrant = builder.AddQdrant("qdrant");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(qdrant);
```

The component consuming project can the access the Qdrant client as shown in the following code:

```csharp
builder.AddQdrantClient("qdrant");
```

## App host updates

In .NET Aspire preview 6, the app host was updated to include several new APIs and features.

### Container runtime arguments

You can now specify custom arguments to the docker or podman run command when running a container. This can be useful for accessing specifics features of the container runtime not exposed by aspire directly.

```csharp
builder.AddContainer("ollama", "ollama/ollama")
       .WithVolume("/root/.ollama")
       .WithBindMount("./ollamaconfig", "/root/")
       .WithHttpEndpoint(port: 11434, name: "ollama")
       .WithEntrypoint("/root/entrypoint.sh")
       .WithContainerRunArgs("--gpus=all");
```

The preceding example uses the `WithContainerRunArgs` method to pass the `--gpus=all` argument to the container runtime. This is useful when running a container that requires access to a GPU.

> [!NOTE]
> .NET Aspire's orchestrator doesn't validate the arguments you pass to the container runtime. It's up to you to ensure that the arguments are valid and safe. These arguments can also conflict with the arguments that .NET Aspire uses to run the container.

### Arguments on project

Outside of using launch profiles, it's now possible to programmatically pass arguments to a project resources:

```csharp
builder.AddProject<Projects.Api>("api")
       .WithArgs("name", "Aspire");
```

### Set a custom environment variable name for a connection string

Resources in .NET Aspire can expose connection strings by implementing `IResourceWithConnectionString`. This interface is used in conjunction with the `WithReference` method to pass the connection string to a project. This sets the fixed name `ConnectionStrings__{ResourceName}`, which might make it hard to integrate with systems that already have an existing convention of naming configuration keys. To help with this problem, you can now set a custom environment variable name for the connection string:

```csharp
var cache = builder.AddRedis("cache");

builder.AddProject<Projects.WebApplication2>("api")
       .WithEnvironment("REDIS_CONN", cache);
```

### Fully qualified container images

For better compatibility with other container runtimes, we now fully qualify our default container images with docker.io instead of letting the runtime infer the registry. This can be overridden by using `WithImageRegistry`.

## Testing

Testing is a critical part of the development process. When building distributed applications, testing can be more challenging because of the complexity of the system. We've added a new testing APIs to help you test your Aspire applications.

The `DistributedApplicationTestingBuilder` follows a familiar pattern to the `WebApplicationFactory` in ASP.NET Core. It allows you to create a test host for your distributed application and run tests against it.

```csharp
using System.Net;

namespace AspireApp.Tests;

public class WebTests
{
    [Fact]
    public async Task GetWebResourceRootReturnsOkStatusCode()
    {
        // Arrange
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AspireApp_AppHost>();
        await using var app = await appHost.BuildAsync();
        await app.StartAsync();

        // Act
        var httpClient = app.CreateHttpClient("webfrontend");
        var response = await httpClient.GetAsync("/");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
```

The above example shows how to create a `DistributedApplicationTestingBuilder` in order to bootstrap the app host project and run tests against it. There's also an easy way to create an `HttpClient` that is configured to make requests to resources in the app host project.

For more information, see [Testing .NET Aspire apps](../fundamentals/testing.md).

## Templates

We updated to the latest stable versions of the OpenTelemetry SDK and Instrumentation packages to version 1.8.1. This includes changes to simplify the OTLP exporter configuration using newer APIs:

```csharp
var useOtlpExporter = !string.IsNullOrWhiteSpace(
    builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

if (useOtlpExporter)
{
    builder.Services.AddOpenTelemetry().UseOtlpExporter();
}
```

This will automatically configure the OTLP exporter to send metrics, traces, and logs to the OTLP endpoint specified in the `OTEL_EXPORTER_OTLP_ENDPOINT` environment variable.

## Components

We removed dependencies on any pre-release versions of OpenTelemetry and replaced them with the latest stable versions where possible. We also removed the dependency on `OpenTelemetry.Instrumentation.EventCounters`.

## Azure provisioning packages

The Azure provisioning packages have been broken out into a package per service. This allows you to only install the packages you need to consume these APIs. This update should be transparent to most users, but if you are using the Azure provisioning packages directly, you must update your project file to reference the new packages.

For more information, see [Azure provisioning libraries](../deployment/azure/local-provisioning.md#azure-provisioning-libraries).

## Azure Developer CLI supports multiple endpoints

The Azure Developer CLI (`azd`) now supports deploying projects, containers or docker files with multiple endpoints. These will be mapped to [Azure Container App's ingress](/azure/container-apps/ingress-overview).
