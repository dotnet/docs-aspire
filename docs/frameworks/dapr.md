---
title: Use Dapr with .NET Aspire
description: Learn how to use Dapr with .NET Aspire
ms.date: 08/12/2024
ms.topic: overview
---

# Use Dapr with .NET Aspire

[Distributed Application Runtime (Dapr)](https://docs.dapr.io/) offers developer APIs that serve as a conduit for interacting with other services and dependencies and abstract the application from the specifics of those services and dependencies. Dapr and .NET Aspire work together to improve your local development experience. By using Dapr with .NET Aspire, you can focus on writing and implementing .NET-based distributed applications instead of spending extra time with local onboarding.  

In this guide, you'll learn how to take advantage of Dapr's abstraction and .NET Aspire's opinionated configuration of cloud technologies to build simple, portable, resilient, and secured microservices at-scale on Azure.

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

In addition to the prerequisites for .NET Aspire, you will need:

- Dapr version 1.13 or later

To install Dapr, see [Install the Dapr CLI](https://docs.dapr.io/getting-started/install-dapr-cli/). After installing the Dapr CLI, run the `dapr init` described in [Initialize Dapr in your local environment](https://docs.dapr.io/getting-started/install-dapr-selfhost/).

> [!IMPORTANT]
> If you attempt to run the app without the Dapr CLI, you'll receive the following error:
>
> ```plaintext
> Unable to locate the Dapr CLI.
> ```

## Get started

To get started you need to add the Dapr hosting package to your app host project by installing the [Aspire.Hosting.Dapr](https://www.nuget.org/packages/Aspire.Hosting.Dapr) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Dapr
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Dapr"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Add a Dapr sidecar

Dapr uses the [sidecar pattern](https://docs.dapr.io/concepts/dapr-services/sidecar/) to run alongside your application. The Dapr sidecar runs alongside your app as a lightweight, portable, and stateless HTTP server that listens for incoming HTTP requests from your app.  

To add a sidecar to a .NET Aspire resource, call the <xref:Aspire.Hosting.IDistributedApplicationResourceBuilderExtensions.WithDaprSidecar%2A> method on the desired resource. The `appId` parameter is the unique identifier for the Dapr application, but it's optional. If you don't provide an `appId`, the parent resource name is used instead.

:::code language="csharp" source="snippets/Dapr/Dapr.AppHost/Program.cs" range="1-7"  highlight="7":::

The `WithDaprSidecar` method offers overloads to configure your Dapr sidecar options like app ID and ports. In the following example, the Dapr sidecar is configured with specific ports for GRPC, HTTP, metrics, and a specific app ID.

:::code language="csharp" source="snippets/Dapr/Dapr.AppHost/Program.cs" range="9-19"  highlight="1-6,11":::

Putting everything together, consider the following example of a .NET Aspire app host project that includes:

- A backend API that declares a Dapr sidecar with defaults.
- A frontend that declares a Dapr sidecar with specific options, such as explict ports.

:::code language="csharp" source="snippets/Dapr/Dapr.AppHost/Program.cs":::

The .NET Aspire dashboard shows the Dapr sidecar as a resource, with its status and logs.

:::image type="content" source="media/aspire-dashboard-dapr-sidecar-resources.png" lightbox="media/aspire-dashboard-dapr-sidecar-resources.png" alt-text=".NET Aspire dashboard showing Dapr sidecar resources":::

## Adding the Dapr SDK

To use Dapr APIs from .NET Aspire resources, you can use the [Dapr SDK for ASP.NET Core (`Dapr.AspNetCore`) library](https://www.nuget.org/packages/Dapr.AspNetCore/). The Dapr SDK provides a set of APIs to interact with Dapr sidecars.

> [!NOTE]
> Use the `Dapr.AspNetCore` library for the Dapr integration with ASP.NET (DI integration, registration of subscriptions, etc.). Non-ASP.NET apps (such as console apps) can just use the [`Dapr.Client` library](https://www.nuget.org/packages/Dapr.Client) to make calls through the Dapr sidecar.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Dapr.AspNetCore
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Dapr.AspNetCore"
                  Version="[SelectVersion]" />
```

---

Once installed into an ASP.NET Core project, the SDK can be added to the service builder.

:::code language="csharp" source="snippets/Dapr/Dapr.Web/Program.cs" range="15":::

An instance of `DaprClient` can now be injected into your services to interact with the Dapr sidecar through the Dapr SDK.

:::code language="csharp" source="snippets/Dapr/Dapr.Web/WeatherApiClient.cs" highlight="11-15":::

`InvokeMethodAsync` is a method that sends an HTTP request to the Dapr sidecar. It is a generic method that takes:

- An HTTP verb
- The Dapr app ID of the service to call
- The method name
- A cancellation token

Depending on the HTTP verb, it can also take a request body and headers. The generic type parameter is the type of the response body.

The full _:::no-loc text="Program.cs":::_ file for the frontend project shows:

- The Dapr client being added to the service builder
- The `WeatherApiClient` class that uses the Dapr client to call the backend service

:::code language="csharp" source="snippets/Dapr/Dapr.Web/Program.cs" highlight="15,17":::

For example, in a Blazor project, the `WeatherApiClient` class can be injected into a integration and used to call the backend service.

:::code language="csharp" source="snippets/Dapr/Dapr.Web/Components/Pages/Weather.razor" highlight="5,47":::

When the Dapr SDK is used, the Dapr sidecar is called over HTTP. The Dapr sidecar then forwards the request to the target service. While the target service runs in a separate process from the sidecar, the integration related to the service runs in the Dapr sidecar and is responsible for service discovery and routing the request to the target service.

## Dapr and .NET Aspire

At first sight Dapr and .NET Aspire may look like they have overlapping functionality, and they do. However, they both take a different approach. .NET Aspire is an opinionated approach on how to build distributed applications on a cloud platform. Dapr is a runtime that abstracts away the common complexities of the underlying cloud platform. It relies on sidecars to provide abstractions for things like configuration, secret management, and messaging. The underlying technology can be easily switched out through configuration files, while your code does not need to change.

.NET Aspire makes setting up and debugging Dapr applications easier by providing a straightforward API to configure Dapr sidecars, and by exposing the sidecars as resources in the dashboard.

### Explore Dapr components with .NET Aspire

Dapr provides many [built-in components](https://docs.dapr.io/concepts/components-concept), and when you use Dapr with .NET Aspire you can easily explore and configure these components. Don't confuse these components with .NET Aspire integrations. For example, consider the following:

- [Dapr—State stores](https://docs.dapr.io/concepts/components-concept/#state-stores): Call <xref:Aspire.Hosting.IDistributedApplicationBuilderExtensions.AddDaprStateStore%2A> to add a configured state store to your .NET Aspire project.
- [Dapr—Pub Sub](https://docs.dapr.io/concepts/components-concept/#pubsub-brokers): Call <xref:Aspire.Hosting.IDistributedApplicationBuilderExtensions.AddDaprPubSub%2A> to add a configured pub sub to your .NET Aspire project.
- Dapr—Components: Call <xref:Aspire.Hosting.IDistributedApplicationBuilderExtensions.AddDaprComponent%2A> to add a configured integration to your .NET Aspire project.

## Next steps

> [!div class="nextstepaction"]
> [.NET Aspire Dapr sample app](/samples/dotnet/aspire-samples/aspire-dapr/)
