---
title: NET Aspire Dapr integration
description: Learn how to use the .NET Aspire Dapr integration, which can configure and orchestrate Dapr from a .NET Aspire app host project.
ms.date: 01/16/2025
uid: frameworks/dapr
---

# .NET Aspire Dapr integration

[Distributed Application Runtime (Dapr)](https://docs.dapr.io/) offers developer APIs that serve as a conduit for interacting with other services and dependencies and abstract the application from the specifics of those services and dependencies. Dapr and .NET Aspire can work together to improve your local development experience. By using Dapr with .NET Aspire, you can focus on writing and implementing .NET-based distributed applications instead of local on-boarding.

In this guide, you'll learn how to take advantage of Dapr's abstraction and .NET Aspire's opinionated configuration of cloud technologies to build simple, portable, resilient, and secured microservices at scale.

## Comparing .NET Aspire and Dapr

At first sight Dapr and .NET Aspire may look like they have overlapping functionality, and they do. However, they take different approaches. .NET Aspire is opinionated on how to build distributed applications on a cloud platform and focuses on improving the local development experience. Dapr is a runtime that abstracts away the common complexities of the underlying cloud platform both during development and in production. It relies on sidecars to provide abstractions for things like configuration, secret management, and messaging. The underlying technology can be easily switched out through configuration files, while your code does not need to change.

| Aspect | .NET Aspire | Dapr |
| --- | --- | --- |
| Purpose | Designed to make it easier to develop cloud-native solutions on local development computers. | Designed to make it easier to develop and run distributed apps with common APIs that can be easily swapped. |
| APIs | Developers must call resource APIs using their specific SDKs | Developers call APIs in the Dapr sidecar, which forwards the call to the correct API. It's easy to swap resource APIs without changing code in your microservices. |
| Languages | You write microservices in .NET languages, Go, Python, Javascript, and others. | You can call Dapr sidecar functions in any language that supports HTTP/gRPC interfaces. |
| Security policies | Doesn't include security policies but can securely configure connections between inter-dependent resources. | Includes customizable security policies that control which microservices have access to other services or resources. |
| Deployment | There are deployment tools for Azure and Kubernetes. | Doesn't include deployment tools. Apps are usually deployed with Continuous Integration/Continuous Development (CI/CD) systems. |
| Dashboard | Provides a comprehensive view of the resources and their telemetry and supports listening on any OTEL supported resource. | Limited to Dapr resources only. |

.NET Aspire makes setting up and debugging Dapr applications easier by providing a straightforward API to configure Dapr sidecars, and by exposing the sidecars as resources in the dashboard.

### Explore Dapr components with .NET Aspire

Dapr provides many [built-in components](https://docs.dapr.io/concepts/components-concept), and when you use Dapr with .NET Aspire you can easily explore and configure these components. Don't confuse these components with .NET Aspire integrations. For example, consider the following:

- [Dapr—State stores](https://docs.dapr.io/concepts/components-concept/#state-stores): Call <xref:Aspire.Hosting.IDistributedApplicationBuilderExtensions.AddDaprStateStore%2A> to add a configured state store to your .NET Aspire project.
- [Dapr—Pub Sub](https://docs.dapr.io/concepts/components-concept/#pubsub-brokers): Call <xref:Aspire.Hosting.IDistributedApplicationBuilderExtensions.AddDaprPubSub%2A> to add a configured pub sub to your .NET Aspire project.
- Dapr—Components: Call <xref:Aspire.Hosting.IDistributedApplicationBuilderExtensions.AddDaprComponent%2A> to add a configured integration to your .NET Aspire project.

## Install Dapr

This integration requires Dapr version 1.13 or later. To install Dapr, see [Install the Dapr CLI](https://docs.dapr.io/getting-started/install-dapr-cli/). After installing the Dapr CLI, run the `dapr init`, as described in [Initialize Dapr in your local environment](https://docs.dapr.io/getting-started/install-dapr-selfhost/).

> [!IMPORTANT]
> If you attempt to run the .NET Aspire solution without the Dapr CLI, you'll receive the following error:
>
> ```plaintext
> Unable to locate the Dapr CLI.
> ```

## Hosting integration

In your .NET Aspire solution, to integrate Dapr and access its types and APIs, add the [📦 Aspire.Hosting.Dapr](https://www.nuget.org/packages/Aspire.Hosting.Dapr) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Dapr
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Dapr"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add Dapr sidecar to .NET Aspire resources

Dapr uses the [sidecar pattern](https://docs.dapr.io/concepts/dapr-services/sidecar/). The Dapr sidecar runs alongside your app as a lightweight, portable, and stateless HTTP server that listens for incoming HTTP requests from your app.

To add a sidecar to a .NET Aspire resource, call the <xref:Aspire.Hosting.IDistributedApplicationResourceBuilderExtensions.WithDaprSidecar%2A> method on it. The `appId` parameter is the unique identifier for the Dapr application, but it's optional. If you don't provide an `appId`, the parent resource name is used instead.

:::code language="csharp" source="snippets/Dapr/Dapr.AppHost/Program.cs" range="1-7"  highlight="7":::

### Configure Dapr sidecars

The `WithDaprSidecar` method offers overloads to configure your Dapr sidecar options like `AppId` and various ports. In the following example, the Dapr sidecar is configured with specific ports for GRPC, HTTP, metrics, and a specific app ID.

:::code language="csharp" source="snippets/Dapr/Dapr.AppHost/Program.cs" range="9-20"  highlight="1-7,12":::

### Complete Dapr app host example

Putting everything together, consider the following example of a .NET Aspire app host project that includes:

- A backend API service that declares a Dapr sidecar with defaults.
- A web frontend project that declares a Dapr sidecar with specific options, such as explict ports.

:::code language="csharp" source="snippets/Dapr/Dapr.AppHost/Program.cs":::

When you start the .NET Aspire solution, the dashboard shows the Dapr sidecar as a resource, with its status and logs.

:::image type="content" source="media/aspire-dashboard-dapr-sidecar-resources.png" lightbox="media/aspire-dashboard-dapr-sidecar-resources.png" alt-text=".NET Aspire dashboard showing Dapr sidecar resources":::

## Use Dapr sidecars in consuming .NET Aspire projects

To use Dapr APIs from .NET Aspire resources, you can use the [📦 Dapr.AspNetCore/](https://www.nuget.org/packages/Dapr.AspNetCore/) NuGet package. The Dapr SDK provides a set of APIs to interact with Dapr sidecars.

> [!NOTE]
> Use the `Dapr.AspNetCore` library for the Dapr integration with ASP.NET (DI integration, registration of subscriptions, etc.). Non-ASP.NET apps (such as console apps) can just use the [📦 Dapr.Client](https://www.nuget.org/packages/Dapr.Client) to make calls through the Dapr sidecar.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Dapr.AspNetCore
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Dapr.AspNetCore"
                  Version="*" />
```

---

### Add Dapr client

Once installed into an ASP.NET Core project, the SDK can be added to the service builder.

:::code language="csharp" source="snippets/Dapr/Dapr.Web/Program.cs" range="15":::

### Invoke Dapr methods

An instance of `DaprClient` can now be injected into your services to interact with the Dapr sidecar through the Dapr SDK:

:::code language="csharp" source="snippets/Dapr/Dapr.Web/WeatherApiClient.cs" highlight="5,11-15":::

`InvokeMethodAsync` is the method that sends an HTTP request to the Dapr sidecar. It is a generic method that takes:

- An HTTP verb.
- The Dapr app ID of the service to call.
- The method name.
- A cancellation token.

Depending on the HTTP verb, it can also take a request body and headers. The generic type parameter is the type of the response body.

The full _:::no-loc text="Program.cs":::_ file for the frontend project shows:

- The Dapr client being added to the service builder.
- The `WeatherApiClient` class that uses the Dapr client to call the backend service.

:::code language="csharp" source="snippets/Dapr/Dapr.Web/Program.cs" highlight="15,17":::

For example, in a Blazor project, you can inject the `WeatherApiClient` class into a razor page and use it to call the backend service:

:::code language="csharp" source="snippets/Dapr/Dapr.Web/Components/Pages/Weather.razor" highlight="5,47":::

When the Dapr SDK is used, the Dapr sidecar is called over HTTP. The Dapr sidecar then forwards the request to the target service. While the target service runs in a separate process from the sidecar, the integration related to the service runs in the Dapr sidecar and is responsible for service discovery and routing the request to the target service.

## Next steps

- [Dapr](https://dapr.io/)
- [Dapr documentation](https://docs.dapr.io/)
- [Dapr GitHub repo](https://github.com/dapr/dapr)
- [.NET Aspire Dapr sample app](https://github.com/CommunityToolkit/Aspire/tree/main/examples/dapr)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
