---
title: Node.js hosting extensions
description: Learn about the Aspire Community Toolkit Node.js hosting extensions package which provides extra functionality to the Aspire NodeJS hosting package.
ms.date: 10/11/2024
---

# Aspire Community Toolkit Node.js hosting extensions

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

[!INCLUDE [banner](includes/banner.md)]

In this article, you learn about the Aspire Community Toolkit Node.js hosting extensions package which provides extra functionality to the Aspire [NodeJS hosting package](https://nuget.org/packages/Aspire.Hosting.NodeJS). The extensions package brings the following features:

- Running [Vite](https://vitejs.dev/) applications
- Running Node.js applications using [Yarn](https://yarnpkg.com/) and [pnpm](https://pnpm.io/)
- Ensuring that the packages are installed before running the application (using the specified package manager)

## Hosting integration

To get started with the Aspire Community Toolkit Node.js hosting extensions, install the [📦 CommunityToolkit.Aspire.Hosting.NodeJS.Extensions](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.NodeJS.Extensions) NuGet package in the AppHost project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.Hosting.NodeJS.Extensions
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.Hosting.NodeJS.Extensions"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

The following sections detail various usages, from running Vite applications to using specific package managers.

### Run specific package managers

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

### Run Vite apps

This integration extension adds support for running the development server for Vite applications. By default, it uses the `npm` package manager to launch, but this can be overridden with the `packageManager` argument.

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

When using the `WithNpmPackageInstallation`, `WithYarnPackageInstallation` or `WithPnpmPackageInstallation` methods, the package manager is used to install the packages before starting the application. These methods are useful to ensure that packages are installed before the application starts, similar to how a .NET application would restore NuGet packages before running.

## See also

- [Orchestrate Node.js apps in Aspire](../get-started/build-aspire-apps-with-nodejs.md)
- [Aspire Community Toolkit GitHub repo](https://github.com/CommunityToolkit/Aspire)
- [Sample Node.js apps](https://github.com/CommunityToolkit/Aspire/tree/main/examples/nodejs-ext)
