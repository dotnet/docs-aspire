---
title: Use Dapr with .NET Aspire
description: Learn how to use Dapr with .NET Aspire
ms.date: 05/14/2024
ms.topic: overview
---

# Use Dapr with .NET Aspire

[Distributed Application Runtime (Dapr)](https://docs.dapr.io/) offers developer APIs that run as a sidecar process and abstract away the common complexities of the underlying cloud platform. Dapr and .NET Aspire work together to improve your local development experience. By using Dapr with .NET Aspire, you can focus on writing and implementing .NET-based distributed applications instead of spending extra time with local onboarding.  

In this guide, you'll learn how to take advantage of Dapr's abstraction and .NET Aspire's opinionated configuration of cloud technologies to build simple, portable, resilient, and secured microservices at-scale on Azure.

## Prerequisites

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

In addition to the prerequisites for .NET Aspire, you will need:
- Dapr version 1.13 or later

Dapr installation instructions can be found [here](https://docs.dapr.io/getting-started/install-dapr-cli/). After installing the Dapr CLI, remember to run dapr init as described [here](https://docs.dapr.io/getting-started/install-dapr-selfhost/).

For more information, see [.NET Aspire setup and tooling](../fundamentals/setup-tooling.md).

## Get started

To get started you need to add the Dapr hosting package to your app host project by installing the [Aspire.Hosting.Dapr](https://www.nuget.org/packages/Aspire.Hosting.Dapr) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Dapr --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Dapr"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

The Dapr resource is added to the .NET Aspire distributed application builder using the `AddDapr()` method.
An overload of the `AddDapr()` method that accepts Dapr options is available. For most applications, the default options will suffice.

:::code language="csharp" source="snippets/Dapr/Dapr.AppHost/Program.cs" range="4":::

Dapr uses the Sidecar pattern to run alongside your application. The Dapr sidecar is a lightweight, portable, and stateless HTTP server that listens for incoming HTTP requests from your application. The sidecar is responsible for managing the lifecycle of your application, including service discovery, configuration, and secrets management. To add a sidecar to a .NET Aspire resource by using the `WithDaprSidecar(string appId)` method. The `appId` parameter is the unique identifier for the Dapr application.

:::code language="csharp" source="snippets/Dapr/Dapr.AppHost/Program.cs" range="18-21"  highlight="21":::

The `WithDaprSidecar` method offers overloads to configure your Dapr sidecar options like app ID and ports. In the following example, the Dapr sidecar is configured with specific ports for GRPC, HTTP, metrics and a specific App ID.

:::code language="csharp" source="snippets/Dapr/Dapr.AppHost/Program.cs" range="6-16"  highlight="6-12,16":::

Putting everything together, here's an example of a .NET Aspire app host project which includes:

- A backend that declares a Dapr sidecar with specific ports and app ID.
- A frontend that declares a Dapr sidecar with a specific app ID and default ports.

:::code language="csharp" source="snippets/Dapr/Dapr.AppHost/Program.cs":::

The .NET Aspire dashboard will shows the Dapr sidecar as a resource, with its status and logs.

:::image type="content" source="media/aspire-dashboard-dapr-sidecar-resources.png" lightbox="media/aspire-dashboard-dapr-sidecar-resources.png" alt-text=".NET Aspire dashboard showing Dapr sidecar resources":::

## Adding the Dapr SDK

To use Dapr APIs from .NET Aspire resources you can use the [Dapr SDK for ASP.NET Core](https://www.nuget.org/packages/Dapr.AspNetCore/). The Dapr SDK provides a set of APIs to interact with Dapr sidecars.

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

:::code language="csharp" source="snippets/Dapr/Dapr.Web/WeatherApiClient.cs" highlight="9-10":::

`InvokeMethodAsync` is a method that sends an HTTP request to the Dapr sidecar. It is a generic method that takes an HTTP verb, the Dapr app ID of the service to call, the method name and a cancellation token. Depending on the HTTP verb it can also take a request body and headers. The generic type parameter is the type of the response body.

The full `Program.cs` file for the frontend project shows the Dapr Client being added to the service builder and the `WeatherApiClient` class that uses the Dapr Client to call the backend service.

:::code language="csharp" source="snippets/Dapr/Dapr.Web/Program.cs" highlight="15,17":::

In, for example, a Blazor project the `WeatherApiClient` class can be injected into a component and used to call the backend service.

:::code language="csharp" source="snippets/Dapr/Dapr.Web/Components/Pages/Weather.razor" highlight="5,47":::

What actually happens when the Dapr SDK is used is that the Dapr sidecar is called over HTTP. The Dapr sidecar then forwards the request to the target service. The target service can be running in the same process as the Dapr sidecar or in a different process. The Dapr sidecar is responsible for service discovery and routing the request to the target service.

## Dapr and .NET Aspire

At first sight Dapr and .NET Aspire may look like they have overlapping functionality, and they do. However, they both take a different approach. .NET Aspire is an opiniated approach on how to build distributed applications on a cloud platform. Dapr is a runtime that abstracts away the common complexities of the underlying cloud platform. It relies on sidecars to provide abstractions for things like configuration, secret management and messaging. The underlying technology can be easily switched out through configuration files, while your code does not need to change.

.NET Aspire makes setting up and debugging Dapr applications easier by providing a straightforward API to configure Dapr sidecars, and by exposing the sidecars as resources in the dashboard.

[.NET Aspire Dapr sample app](/samples/dotnet/aspire-samples/aspire-dapr/)
