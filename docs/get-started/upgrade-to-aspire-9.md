---
title: Upgrade to .NET Aspire 9.0
description: Learn how to upgrade all your .NET Aspire 8.x projects to .NET Aspire 9.0.
ms.date: 10/17/2024
---

# Upgrade to .NET Aspire 9.0

.NET Aspire 9.0 is now generally available. This article will guide you through the steps involved in updating your existing .NET Aspire 8.x projects to .NET Aspire 9.0. There are a few ways in which you can update your projects to .NET Aspire 9.0:

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

- [Install the latest .NET bits](../fundamentals/setup-tooling.md).

> [!NOTE]
> Feel free to uninstall the .NET Aspire workload as you'll no longer need it.
>
> ```dotnetcli
> dotnet workload uninstall aspire
> ```
>
> For more information, see [dotnet workload uninstall](/dotnet/core/tools/dotnet-workload-uninstall).

## Manually upgrade to .NET Aspire 9.0

To upgrade your projects to .NET Aspire 9.0, you need to manually update your project files. The following steps will guide you through the process:

- Edit your app host project file to use the new .NET Aspire 9.0 SDK (`Aspire.AppHost.Sdk`).
- Update the NuGet packages in your project files to the latest versions.
- Adjust your _Program.cs_ file to use the new API changes.

### Edit your app host project file

To upgrade your project to .NET Aspire 9.0, you need to update your project file to use the new .NET Aspire 9.0 SDK. Open your project file in a text editor and update the `Sdk` attribute to use the new [📦 Aspire.AppHost.Sdk](https://www.nuget.org/packages/Aspire.AppHost.Sdk):

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <Sdk Name="Aspire.AppHost.Sdk" Version="9.0.0" />

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net9.0</TargetFramework>
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

### Update the NuGet packages

Update the NuGet packages in your project files to the latest versions. You can use the following command to update the NuGet packages in your project:

```dotnetcli
dotnet add package Aspire.Hosting.AppHost --version 9.0.0
```

### Adjust your _Program.cs_ file

With the introduction of .NET Aspire 9.0, there are some breaking API changes. Some APIs were originally marked as experimental and are now removed, and/or replaced with new APIs. You need to adjust your _Program.cs_ file to use the new APIs.

The following table lists some of the APIs changes:

| .NET Aspire 8.x API | .NET Aspire 9.0 API |
|---------------------|---------------------|
| `SomeOldApi`        | `SomeNewApi`        |

## Use the Upgrade Assistant

The [Upgrade Assistant](/dotnet/core/porting/upgrade-assistant-overview) is a tool that can help you update your projects to .NET Aspire 9.0.
