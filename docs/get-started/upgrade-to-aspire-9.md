---
title: Upgrade to Aspire 9.5.0
description: Learn how to upgrade all your Aspire projects to Aspire 9.5.0.
ms.date: 09/09/2025
zone_pivot_groups: dev-environment
---

# Upgrade to Aspire 9.5.0

In this article, you learn the steps involved in updating your existing Aspire projects to the latest version: Aspire 9.5.0. There are a few ways in which you can update your projects to Aspire 9.5.0:

- Manually upgrade your projects to Aspire 9.5.0.
- Use the **Upgrade Assistant** to upgrade your projects to Aspire 9.5.0.

> [!TIP]
> If you're new to Aspire, there's no reason to upgrade anything. For more information, see [Aspire setup and tooling](../fundamentals/setup-tooling.md).

## Prerequisites

Before you upgrade your projects to Aspire 9.5.0, ensure that you have the following prerequisites:

- [Install the latest tooling](../fundamentals/setup-tooling.md).
- [Use the Aspire SDK](../fundamentals/dotnet-aspire-sdk.md).
- If you have a version of Aspire older than 9.0, [remove it](#remove-the-net-aspire-workload-first-time-upgrades-from-version-8-only).

## Upgrade the Aspire project templates

Install the latest Aspire project templates by running this command:

```dotnetcli
dotnet new install Aspire.ProjectTemplates
```

> [!TIP]
> If you have the legacy Aspire workload installed, you need to pass the `--force` flag to overwrite the existing templates. For instructions on uninstalling the legacy workload, see [Remove the Aspire workload (first-time upgrades from version 8 only)](#remove-the-net-aspire-workload-first-time-upgrades-from-version-8-only).

## Manually upgrade a solution to Aspire 9.5.0

To upgrade your projects to Aspire 9.5.0, you need to update your project files. The following steps guide you through the process:

- Edit your [AppHost](xref:dotnet/aspire/app-host) project file to use the new Aspire 9.5.0 SDK (`Aspire.AppHost.Sdk`).
- Update the NuGet packages in your project files to the latest versions.
- Adjust your _Program.cs_ file to use the new APIs and remove any obsolete APIs.

### Edit your AppHost project file

To upgrade your AppHost project to Aspire 9.5.0, you need to update your project file to use the new [📦 Aspire.AppHost.Sdk](https://www.nuget.org/packages/Aspire.AppHost.Sdk):

```diff
<Project Sdk="Microsoft.NET.Sdk">

+  <Sdk Name="Aspire.AppHost.Sdk" Version="9.5.0" />

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsAspireHost>true</IsAspireHost>
    <UserSecretsId>0afc20a6-cd99-4bf7-aae1-1359b0d45189</UserSecretsId>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Aspire.Hosting.AppHost" Version="9.5.0" />
  </ItemGroup>

</Project>
```

### Optionally upgrade the target framework moniker (TFM)

Aspire 9.5.0 runs on .NET 9.0, but you can also run it on .NET 8.0. In other words, just because you're using the Aspire SDK, and pointing to version 9.5.0 packages, you can still target .NET 8.0. If you want to run your Aspire 9.5.0 project on .NET 9.0, you need to update the `TargetFramework` property in your project file:

```diff
<Project Sdk="Microsoft.NET.Sdk">

  <Sdk Name="Aspire.AppHost.Sdk" Version="9.5.0" />

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
    <PackageReference Include="Aspire.Hosting.AppHost" Version="9.5.0" />
  </ItemGroup>

</Project>
```

For more information on TFMs, see [Target frameworks in SDK-style projects: Latest versions](/dotnet/standard/frameworks#latest-versions).

### Overall AppHost project differences

If you followed all of the preceding steps, your AppHost project file should look like this:

```diff
<Project Sdk="Microsoft.NET.Sdk">

+  <Sdk Name="Aspire.AppHost.Sdk" Version="9.5.0" />

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
+   <PackageReference Include="Aspire.Hosting.AppHost" Version="9.5.0" />
  </ItemGroup>

</Project>
```

The changes include the addition of the `Aspire.AppHost.Sdk`, the update of the `TargetFramework` property to `net9.0`, and the update of the `Aspire.Hosting.AppHost` package to version `9.5.0`.

### Adjust your _Program.cs_ file

> [!IMPORTANT]
> If you're still seeing a _Program.cs_, you could change the name of this file to match the templates. Moving forward, an AppHost project contains an _AppHost.cs_ file that acts as the entry point.

With the introduction of Aspire 9.5, there are some _breaking changes_. Some APIs were originally marked as experimental (with the <xref:System.Diagnostics.CodeAnalysis.ExperimentalAttribute>) and are now removed, while other APIs are now attributed as <xref:System.ObsoleteAttribute> with details on new replacement APIs. You need to adjust your _Program.cs_ file (and potentially other affected APIs) to use the new APIs. If you're using the Upgrade Assistant to upgrade your projects, it automatically adjusts your _Program.cs_ file in most cases.

For the complete list of breaking changes in Aspire 9.5.0, see [Breaking changes in Aspire 9.5](../compatibility/9.5/index.md).

> [!IMPORTANT]
> Be sure to review breaking changes for all versions of Aspire after the one you're upgrading from. For example, if you're upgrading from Aspire 9.0, you must address breaking changes for versions [9.1](../compatibility/9.1/index.md), [9.2](../compatibility/9.2/index.md), [9.3](../compatibility/9.3/index.md), [9.4](../compatibility/9.4/index.md), and [9.5](../compatibility/9.5/index.md).

## Use the Upgrade Assistant

The [Upgrade Assistant](/dotnet/core/porting/upgrade-assistant-overview) is a tool that helps upgrade targeted projects to the latest version. If you're new to the Upgrade Assistant, there are two modalities to choose from:

- [The Visual Studio extension version](/dotnet/core/porting/upgrade-assistant-install#visual-studio-extension).
- [The .NET CLI global tool version](/dotnet/core/porting/upgrade-assistant-install#net-global-tool).

Regardless of how you install the Upgrade Assistant, you can use it to upgrade your Aspire projects to Aspire 9.5.0.

:::zone pivot="visual-studio"

To upgrade the Aspire AppHost project to Aspire 9.5.0 with Visual Studio, right-click the project in **Solution Explorer** and select **Upgrade**.

> [!IMPORTANT]
> If the **Upgrade Assistant** isn't already installed, you'll be prompted to install it.

The Upgrade Assistant displays a welcome package. Select the **Aspire upgrades** option:

:::image type="content" source="media/upgrade-assistant-welcome-aspire.png" lightbox="media/upgrade-assistant-welcome-aspire.png" alt-text="Visual Studio: Upgrade Assistant welcome page with Aspire AppHost project.":::

With the **Aspire upgrades** option selected, the Upgrade Assistant displays the selectable upgrade target components. Leave all the options checked and select **Upgrade selection**:

:::image type="content" source="media/upgrade-assistant-aspire-app-host-comps.png" lightbox="media/upgrade-assistant-aspire-app-host-comps.png" alt-text="Visual Studio: Upgrade Assistant Aspire selectable components to upgrade.":::

Finally, after selecting the components to upgrade, the Upgrade Assistant displays the results of the upgrade process. If everything was successful, you see green check marks next to each component:

:::image type="content" source="media/upgrade-assistant-aspire-upgraded.png" lightbox="media/upgrade-assistant-aspire-upgraded.png" alt-text="Visual Studio: Upgrade Assistant Aspire AppHost project upgraded successfully.":::

:::zone-end
:::zone pivot="vscode,dotnet-cli"

To upgrade the Aspire AppHost project, ensure that you installed the Upgrade Assistant CLI. Open a terminal session at the root directory of the Aspire AppHost project file, and run the following command:

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
    Upgrade to latest Aspire version (aspire.latest)

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

To take advantage of the latest updates in your Aspire solution, update all NuGet packages to version `9.5.0`.

:::zone pivot="visual-studio"

:::image type="content" source="media/visual-studio-update-nuget.png" lightbox="media/visual-studio-update-nuget.png" alt-text="Visual Studio: Update all NuGet packages for the Aspire solution.":::

:::zone-end
:::zone pivot="vscode,dotnet-cli"

To update your AppHost project, use the following .NET CLI command to update the `Aspire.Hosting.AppHost` package to version `9.5.0`:

```dotnetcli
dotnet add package Aspire.Hosting.AppHost --version 9.5.0
```

When a package reference already exists, the `dotnet add package` command updates the reference to the specified version. For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package).

:::zone-end

With the AppHost project updated, your project file should look like this:

```diff
<Project Sdk="Microsoft.NET.Sdk">

  <Sdk Name="Aspire.AppHost.Sdk" Version="9.5.0" />

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
+   <PackageReference Include="Aspire.Hosting.AppHost" Version="9.5.0" />
  </ItemGroup>

</Project>
```

> [!TIP]
> You'll want to also update the NuGet packages in your other projects to the latest versions.

## Remove the Aspire workload (first-time upgrades from version 8 only)

If you're upgrading from Aspire 8 to Aspire 9 for the first time, you need to remove the legacy workload. This step is not needed for upgrades between Aspire 9 versions (such as from 9.4.0 to 9.5.0).

Previously the **aspire workload** was required to create and run Aspire projects. But, with Aspire 9 the workload is no longer required and should be removed from your .NET environment.

> [!IMPORTANT]
> You must remove Aspire 8 (the **aspire workload**) to enable the Aspire 9 templates.

01. Find the installation source by opening a terminal and running the `dotnet workload list` command.

    The preceding command lists the workloads installed in the .NET environment. The method used to install Aspire is listed under the **Installation Source** column of the output, and is either _VS_ for Visual Studio or _SDK_ for the .NET SDK. For example, the following snippet indicates that Visual Studio was used to install Aspire:

    ```
    Installed Workload Id      Manifest Version      Installation Source
    --------------------------------------------------------------------
    aspire                     8.2.2/8.0.100         VS 17.14.36109.1
    ```

01. Remove Aspire 8.

    - If the **Installation Source** starts with _VS_:

      01. Open the **Visual Studio Installer** app.
      01. **Modify** the installation instance of Visual Studio.
      01. Select **Individual Components**.
      01. Search for `aspire`.
      01. Unselect **Aspire SDK**.
      01. Select the **Modify** button to apply the changes.

    - If the **Installation Source** starts with _SDK_, run `dotnet workload uninstall aspire` to remove Aspire.

## Verify the upgrade

As with any upgrade, ensure that the app runs as expected and that all tests pass. Build the solution and look for suggestions, warnings, or errors in the output window—address anything that wasn't an issue before. If you encounter any issues, let us know by [filing a GitHub issue](https://github.com/dotnet/aspire/issues/new/choose).
