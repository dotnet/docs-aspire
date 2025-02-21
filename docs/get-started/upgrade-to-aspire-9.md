---
title: Upgrade to .NET Aspire 9.0
description: Learn how to upgrade all your .NET Aspire 8.x projects to .NET Aspire 9.0.
ms.date: 11/11/2024
zone_pivot_groups: dev-environment
---

# Upgrade to .NET Aspire 9.0

.NET Aspire 9.0 is now generally available. In this article, you learn the steps involved in updating your existing .NET Aspire 8.x projects to .NET Aspire 9.0. There are a few ways in which you can update your projects to .NET Aspire 9.0:

- Manually upgrade your projects to .NET Aspire 9.0.
- Use the **Upgrade Assistant** to upgrade your projects to .NET Aspire 9.0.

> [!TIP]
> If you're new to .NET Aspire, there's no reason to upgrade anything. For more information, see [.NET Aspire setup and tooling](../fundamentals/setup-tooling.md).

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

If you don't uninstall the .NET Aspire workload, and you're using the new [.NET Aspire SDK](../fundamentals/dotnet-aspire-sdk.md) and templates, you see both .NET Aspire 8.0 and .NET Aspire 9.0 templates.

## Manually upgrade to .NET Aspire 9.0

To upgrade your projects to .NET Aspire 9.0, you need to update your project files. The following steps guide you through the process:

- Edit your [app host](xref:dotnet/aspire/app-host) project file to use the new .NET Aspire 9.0 SDK (`Aspire.AppHost.Sdk`).
- Update the NuGet packages in your project files to the latest versions.
- Adjust your _Program.cs_ file to use the new APIs and remove any obsolete APIs.

### Edit your app host project file

To upgrade your app host project to .NET Aspire 9.0, you need to update your project file to use the new [📦 Aspire.AppHost.Sdk](https://www.nuget.org/packages/Aspire.AppHost.Sdk):

```diff
<Project Sdk="Microsoft.NET.Sdk">

+  <Sdk Name="Aspire.AppHost.Sdk" Version="9.0.0" />

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsAspireHost>true</IsAspireHost>
    <UserSecretsId>0afc20a6-cd99-4bf7-aae1-1359b0d45189</UserSecretsId>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Aspire.Hosting.AppHost" Version="8.0.0" />
  </ItemGroup>

</Project>
```

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
    <PackageReference Include="Aspire.Hosting.AppHost" Version="9.1.0" />
  </ItemGroup>

</Project>
```

For more information on TFMs, see [Target frameworks in SDK-style projects: Latest versions](/dotnet/standard/frameworks#latest-versions).

### Overall app host project differences

If you followed all of the preceding steps, your app host project file should look like this:

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
+   <PackageReference Include="Aspire.Hosting.AppHost" Version="9.1.0" />
  </ItemGroup>

</Project>
```

The changes include the addition of the `Aspire.AppHost.Sdk`, the update of the `TargetFramework` property to `net9.0`, and the update of the `Aspire.Hosting.AppHost` package to version `9.0.0`.

### Adjust your _Program.cs_ file

With the introduction of .NET Aspire 9.0, there are some _breaking changes_. Some APIs were originally marked as experimental (with the <xref:System.Diagnostics.CodeAnalysis.ExperimentalAttribute>) and are now removed, while other APIs are now attributed as <xref:System.ObsoleteAttribute> with details on new replacement APIs. You need to adjust your _Program.cs_ file (and potentially other affected APIs) to use the new APIs. If you're using the Upgrade Assistant to upgrade your projects, it automatically adjusts your _Program.cs_ file in most cases.

For the complete list of breaking changes in .NET Aspire 9.0, see [Breaking changes in .NET Aspire 9.0](../compatibility/9.0/index.md).

## Use the Upgrade Assistant

The [Upgrade Assistant](/dotnet/core/porting/upgrade-assistant-overview) is a tool that helps upgrade targeted projects to the latest version. If you're new to the Upgrade Assistant, there's two modalities to choose from:

- [The Visual Studio extension version](/dotnet/core/porting/upgrade-assistant-install#visual-studio-extension).
- [The .NET CLI global tool version](/dotnet/core/porting/upgrade-assistant-install#net-global-tool).

Regardless of how you install the Upgrade Assistant, you can use it to upgrade your .NET Aspire 8.x projects to .NET Aspire 9.0.

:::zone pivot="visual-studio"

To upgrade the .NET Aspire app host project to .NET Aspire 9.0 with Visual Studio, right-click the project in **Solution Explorer** and select **Upgrade**.

> [!IMPORTANT]
> If the **Upgrade Assistant** isn't already installed, you'll be prompted to install it.

The Upgrade Assistant displays a welcome package. Select the **Aspire upgrades** option:

:::image type="content" source="media/upgrade-assistant-welcome-aspire.png" lightbox="media/upgrade-assistant-welcome-aspire.png" alt-text="Visual Studio: Upgrade Assistant welcome page with .NET Aspire app host project.":::

With the **Aspire upgrades** option selected, the Upgrade Assistant displays the selectable upgrade target components. Leave all the options checked and select **Upgrade selection**:

:::image type="content" source="media/upgrade-assistant-aspire-app-host-comps.png" lightbox="media/upgrade-assistant-aspire-app-host-comps.png" alt-text="Visual Studio: Upgrade Assistant .NET Aspire selectable components to upgrade.":::

Finally, after selecting the components to upgrade, the Upgrade Assistant displays the results of the upgrade process. If everything was successful, you see green check marks next to each component:

:::image type="content" source="media/upgrade-assistant-aspire-upgraded.png" lightbox="media/upgrade-assistant-aspire-upgraded.png" alt-text="Visual Studio: Upgrade Assistant .NET Aspire app host project upgraded successfully.":::

:::zone-end
:::zone pivot="vscode,dotnet-cli"

To upgrade the .NET Aspire app host project, ensure that you installed the Upgrade Assistant CLI. Open a terminal session at the root directory of the .NET Aspire app host project file, and run the following command:

```dotnetcli
upgrade-assistant upgrade
```

The output is interactive, expecting you to select the upgrade type. Choose the **Aspire upgrades** option:

```dotnetcli
 Selected options
────────────────────────────────────────────────────────────────────────────────────
 Source project     ..\AspireSample\AspireSample.AppHost\AspireSample.AppHost.csproj

 Steps
────────────────────────────────
 Source project / Upgrade type
────────────────────────────────

How do you want to upgrade project AspireSample.AppHost?

> Aspire upgrades
    Upgrade to latest .NET Aspire version (aspire.latest)

  Upgrade project to a newer .NET version
    In-place project upgrade (framework.inplace)

  NuGet upgrades
    NuGet central package management (CPM) (nuget.cpm)

  Navigation
    Back
    Exit
```

Use your keyboard to navigate up <kbd>↑</kbd> or down <kbd>↓</kbd>, and select the **Aspire upgrades** option. The Upgrade Assistant prompts for final confirmation. Enter <kbd>Y</kbd> to continue with the upgrade:

```dotnetcli
 Selected options
────────────────────────────────────────────────────────────────────────────────────
 Source project     ..\AspireSample\AspireSample.AppHost\AspireSample.AppHost.csproj
 Upgrade type       aspire.latest

 Steps
──────────────────────────────────────────
 Source project / Upgrade type / Upgrade
──────────────────────────────────────────

We have gathered all required options and are ready to do the upgrade. Do you want to continue? [y/n] (y):
```

Finally, after the upgrade process is complete, the Upgrade Assistant displays the results of the upgrade process:

```dotnetcli
Finalizing operation...
Complete: 3 succeeded, 0 failed, 7 skipped.
```

:::zone-end

### Update the NuGet packages

To take advantage of the latest updates in your .NET Aspire solution, update all NuGet packages to version `9.0.0`.

:::zone pivot="visual-studio"

:::image type="content" source="media/visual-studio-update-nuget.png" lightbox="media/visual-studio-update-nuget.png" alt-text="Visual Studio: Update all NuGet packages for the .NET Aspire solution.":::

:::zone-end
:::zone pivot="vscode,dotnet-cli"

To update your app host project, use the following .NET CLI command to update the `Aspire.Hosting.AppHost` package to version `9.0.0`:

```dotnetcli
dotnet add package Aspire.Hosting.AppHost --version 9.0.0
```

When a package reference already exists, the `dotnet add package` command updates the reference to the specified version. For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package).

:::zone-end

With the app host project updated, your project file should look like this:

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
+   <PackageReference Include="Aspire.Hosting.AppHost" Version="9.1.0" />
  </ItemGroup>

</Project>
```

> [!TIP]
> You'll want to also update the NuGet packages in your other projects to the latest versions.

## Verify the upgrade

As with any upgrade, ensure that the app runs as expected and that all tests pass. Build the solution and look for suggestions, warnings, or errors in the output window—address anything that wasn't an issue before. If you encounter any issues, let us know by [filing a GitHub issue](https://github.com/dotnet/aspire/issues/new/choose).
