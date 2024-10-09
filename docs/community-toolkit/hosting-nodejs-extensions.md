---
title: Node.js hosting extensions
author: aaronpowell
description: Extensions on the .NET Aspire hosting for Node.js applications to support alternative package managers.
ms.date: 10/11/2024
---

# .NET Aspire Community Toolkit Node.js hosting extensions

[!INCLUDE [banner](includes/banner.md)]

In this article, you'll learn about the .NET Aspire Community Toolkit Node.js hosting extensions package which provides additional functionality to the .NET Aspire [NodeJS hosting package](https://nuget.org/packages/Aspire.Hosting.NodeJS). The extensions package brings the following features:

- Running [Vite](https://vitejs.dev/) applications
- Running Node.js applications using [Yarn](https://yarnpkg.com/) and [pnpm](https://pnpm.io/)
- Ensuring that the packages are installed before running the application (using the specified package manager)

## Getting Started

To get started with the .NET Aspire Community Toolkit Node.js hosting extensions, install the [Aspire.CommunityToolkit.Hosting.NodeJS.Extensions](https://github.com/orgs/CommunityToolkit/packages/nuget/package/Aspire.CommunityToolkit.Hosting.NodeJS.Extensions) NuGet package in the AppHost project.

[!INCLUDE [githhub-packages](includes/githhub-packages.md)]

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.CommunityToolkit.Hosting.NodeJS.Extensions
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.CommunityToolkit.Hosting.NodeJS.Extensions"
                  Version="[SelectVersion]" />
```

---

## Example Usage

### Running with a specific package manager

This integration extension adds support for running Node.js applications using Yarn or pnpm as the package manager.

# [yarn](#tab/yarn)

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddYarnApp("yarn-demo")
    .WithExternalHttpEndpoints();
```

# [pnpm](#tab/pnpm)

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddPnpmApp("pnpm-demo")
    .WithExternalHttpEndpoints();
```

---

### Running Vite applications

This integration extension adds support for running the development server for Vite applications. By default, it will use the `npm` package manager to launch, but this can be overridden with the `packageManager` argument.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddViteApp("vite-demo")
    .WithExternalHttpEndpoints();

builder.AddViteApp("yarn-demo", packageManager: "yarn")
    .WithExternalHttpEndpoints();

builder.AddViteApp("pnpm-demo", packageManager: "pnpm")
    .WithExternalHttpEndpoints();

builder.Build().Run();
```

### Install packages

When using the `WithNpmPackageInstallation`, `WithYarnPackageInstallation` or `WithPnpmPackageInstallation` methods, the package manager will be used to install the packages before starting the application. This is useful to ensure that packages are installed before the application starts, similar to how a .NET application would restore NuGet packages before running.

## See also

- [Orchestrate Node.js apps in .NET Aspire](../get-started/build-aspire-apps-with-nodejs.md)
- [.NET Aspire Community Toolkit GitHub repo](https://github.com/CommunityToolkit/Aspire)
