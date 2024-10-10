---
title: Azure Static Web Apps emulator
author: aaronpowell
description: A .NET Aspire hosting integration for the Azure Static Web Apps emulator.
ms.date: 10/11/2024
---

# .NET Aspire Azure Static Web Apps emulator integration

[!INCLUDE [banner](includes/banner.md)]

In this article, you'll learn how to use the .NET Aspire [Azure Static Web Apps emulator](/azure/static-web-apps/local-development) integration to run Azure Static Web Apps locally using the emulator. The emulator provides support for proxying both the static frontend and the API backend using resources defined in the app host.

## Getting Started

To get started with the .NET Aspire Azure Static Web Apps emulator integration, install the [Aspire.CommunityToolkit.Hosting.Azure.StaticWebApps](https://github.com/orgs/CommunityToolkit/packages/nuget/package/Aspire.CommunityToolkit.Hosting.Azure.StaticWebApps) NuGet package in the AppHost project. You'll also need the Azure Static Web Apps CLI, be sure to follow the [Getting Started guide](/azure/static-web-apps/local-development#get-started) for it.

[!INCLUDE [github-packages](includes/github-packages.md)]

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.CommunityToolkit.Hosting.Azure.StaticWebApps
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.CommunityToolkit.Hosting.Azure.StaticWebApps"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example Usage

In the _::no-loc text="Program.cs"_ file of your AppHost project, define the backend and frontend resources and call the `AddSwaEmulator` method to create the emulator, and pass the resources using the `WithAppResource` and `WithApiResource` methods.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Define the API resource
var api = builder.AddProject<Projects.Aspire_CommunityToolkit_StaticWebApps_ApiApp>("api");

// Define the frontend resource
var web = builder
    .AddNpmApp("web", Path.Combine("..", "Aspire.CommunityToolkit.StaticWebApps.WebApp"), "dev")
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints();

// Create a SWA emulator with the frontend and API resources
_ = builder
    .AddSwaEmulator("swa")
    .WithAppResource(web)
    .WithApiResource(api);

builder.Build().Run();
```

## See also

- [Azure Static Web Apps emulator](/azure/static-web-apps/local-development)
- [Azure Static Web Apps](/azure/static-web-apps/)
- [.NET Aspire Community Toolkit GitHub repo](https://github.com/CommunityToolkit/Aspire)
