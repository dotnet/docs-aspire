---
title: Upgrade to .NET Aspire 9.0
description: Learn how to upgrade all your .NET Aspire 8.x projects to .NET Aspire 9.0.
ms.date: 10/24/2024
zone_pivot_groups: dev-environment
---

# Upgrade to .NET Aspire 9.0

.NET Aspire 9.0 is now generally available. In this article, you learn the steps involved in updating your existing .NET Aspire 8.x projects to .NET Aspire 9.0. There are a few ways in which you can update your projects to .NET Aspire 9.0:

- Manually upgrade your projects to .NET Aspire 9.0.
- Use the Upgrade Assistant to upgrade your projects to .NET Aspire 9.0.

> [!TIP]
> If you're new to .NET Aspire, there's no reason to upgrade anything. For more information, see [.NET Aspire setup and tooling](../fundamentals/setup-tooling.md).

<!--

We have a series of changes customers will need to make manually during their update to Aspire 9. Whilst probably not comprehensive, the list of changes would include:

Some API changes in App Host Program.cs
NuGet package updates
SDK changes/additions in .csproj files
[Feel free to add here]

-->

## Prerequisites

Before you upgrade your projects to .NET Aspire 9.0, ensure that you have the following prerequisites:

- [Install the latest tooling](../fundamentals/setup-tooling.md).
- [Use the .NET Aspire SDK](../fundamentals/dotnet-aspire-sdk.md).

> [!NOTE]
> Feel free to uninstall the .NET Aspire workload as you'll no longer need it.
>
> ```dotnetcli
> dotnet workload uninstall aspire
> ```
>
> For more information, see [dotnet workload uninstall](/dotnet/core/tools/dotnet-workload-uninstall).

## Manually upgrade to .NET Aspire 9.0

To upgrade your projects to .NET Aspire 9.0, you need to update your project files. The following steps will guide you through the process:

- Edit your [app host](xref:dotnet/aspire/app-host) project file to use the new .NET Aspire 9.0 SDK (`Aspire.AppHost.Sdk`).
- Update the NuGet packages in your project files to the latest versions.
- Adjust your _Program.cs_ file to use the new APIs and remove any obsolete APIs.

### Edit your app host project file

To upgrade your app host project to .NET Aspire 9.0, you need to update your project file to use the new [.NET Aspire SDK](../fundamentals/dotnet-aspire-sdk.md). Open your project file in a text editor and add an `Sdk` attribute to use the new [📦 Aspire.AppHost.Sdk](https://www.nuget.org/packages/Aspire.AppHost.Sdk):

```diff
<Project Sdk="Microsoft.NET.Sdk">

+  <Sdk Name="Aspire.AppHost.Sdk" Version="9.0.0" />

  <PropertyGroup>
    <OutputType>Exe</OutputType>
-   <TargetFramework>net8.0</TargetFramework>
+   <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsAspireHost>true</IsAspireHost>
    <UserSecretsId>0afc20a6-cd99-4bf7-aae1-1359b0d45189</UserSecretsId>
  </PropertyGroup>

  <ItemGroup>
-   <PackageReference Include="Aspire.Hosting.AppHost" Version="8.0.0" />
+   <PackageReference Include="Aspire.Hosting.AppHost" Version="9.0.0" />
  </ItemGroup>

</Project>
```

### Update the NuGet packages

To take advantage of the latest updates in your .NET Aspire solution, update the NuGet packages to the latest versions.

:::zone pivot="visual-studio"

<!-- TODO: Screen capture this... -->

:::zone-end
:::zone pivot="vscode,dotnet-cli"

To update your app host project, use the following .NET CLI command to update the `Aspire.Hosting.AppHost` package to version `9.0.0`:

```dotnetcli
dotnet add package Aspire.Hosting.AppHost --version 9.0.0
```

When a package reference already exists, the `dotnet add package` command updates the reference to the specified version. For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package).

:::zone-end

After updating the app host project, your project file should look like this:

```diff
<Project Sdk="Microsoft.NET.Sdk">

  <Sdk Name="Aspire.AppHost.Sdk" Version="9.0.0" />

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsAspireHost>true</IsAspireHost>
    <UserSecretsId>0afc20a6-cd99-4bf7-aae1-1359b0d45189</UserSecretsId>
  </PropertyGroup>

  <ItemGroup>
-   <PackageReference Include="Aspire.Hosting.AppHost" Version="8.0.0" />
+   <PackageReference Include="Aspire.Hosting.AppHost" Version="9.0.0" />
  </ItemGroup>

</Project>
```

> [!TIP]
> You'll want to also update the NuGet packages in your other projects to the latest versions.

### Optionally upgrade the target framework moniker (TFM)

.NET Aspire 9.0 runs on .NET 9.0, but you can also run it on .NET 8.0. In other words, just because you're using the .NET Aspire SDK, and pointing to version 9.0 packages, you can still target .NET 8.0. If you want to run your .NET Aspire 9.0 project on .NET 9.0, you need to update the `TargetFramework` property in your project file:

```diff
<Project Sdk="Microsoft.NET.Sdk">

  <Sdk Name="Aspire.AppHost.Sdk" Version="9.0.0" />

  <PropertyGroup>
    <OutputType>Exe</OutputType>
-   <TargetFramework>net8.0</TargetFramework>
+   <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsAspireHost>true</IsAspireHost>
    <UserSecretsId>0afc20a6-cd99-4bf7-aae1-1359b0d45189</UserSecretsId>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Aspire.Hosting.AppHost" Version="9.0.0" />
  </ItemGroup>

</Project>
```

For more information on TFMs, see [Target frameworks in SDK-style projects: Latest versions](/dotnet/standard/frameworks#latest-versions).

### Overall app host project differences

If you've followed all of the preceding steps, your app host project file should look like this:

```diff
<Project Sdk="Microsoft.NET.Sdk">

+  <Sdk Name="Aspire.AppHost.Sdk" Version="9.0.0" />

  <PropertyGroup>
    <OutputType>Exe</OutputType>
-   <TargetFramework>net8.0</TargetFramework>
+   <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsAspireHost>true</IsAspireHost>
    <UserSecretsId>0afc20a6-cd99-4bf7-aae1-1359b0d45189</UserSecretsId>
  </PropertyGroup>

  <ItemGroup>
-   <PackageReference Include="Aspire.Hosting.AppHost" Version="8.0.0" />
+   <PackageReference Include="Aspire.Hosting.AppHost" Version="9.0.0" />
  </ItemGroup>

</Project>
```

The changes include the addition of the `Aspire.AppHost.Sdk` SDK, the update of the `TargetFramework` property to `net9.0`, and the update of the `Aspire.Hosting.AppHost` package to version `9.0.0`.

### Adjust your _Program.cs_ file

With the introduction of .NET Aspire 9.0, there are some breaking API changes. Some APIs were originally marked as experimental and are now removed, and/or replaced with new APIs. You need to adjust your _Program.cs_ file to use the new APIs.

The following table lists some of the APIs changes:

| .NET Aspire 8.x API | .NET Aspire 9.0 API |
|---------------------|---------------------|
| `SomeOldApi`        | `SomeNewApi`        |

## Use the Upgrade Assistant

The [Upgrade Assistant](/dotnet/core/porting/upgrade-assistant-overview) is a tool that helps upgrade targeted projects to the latest version. If you've never used the Upgrade Assistant before, there's two modalities to choose from:

- [The Visual Studio extension version](/dotnet/core/porting/upgrade-assistant-install#visual-studio-extension).
- [The .NET CLI global tool version](/dotnet/core/porting/upgrade-assistant-install#net-global-tool).

Regardless of how you've installed the Upgrade Assistant, you can use it to upgrade your .NET Aspire 8.x projects to .NET Aspire 9.0.
