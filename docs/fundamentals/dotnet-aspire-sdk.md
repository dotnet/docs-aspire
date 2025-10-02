---
title: Aspire SDK
description: Learn
ms.date: 08/07/2025
uid: dotnet/aspire/sdk
---

# Aspire SDK

The Aspire SDK is intended for [_*.AppHost_ projects](app-host-overview.md#apphost-project), which serve as the Aspire orchestrator. These projects are designated by their usage of the `Aspire.AppHost.Sdk` in the project file. The SDK provides a set of features that simplify the development of Aspire apps.

## Overview

The [📦 Aspire.AppHost.Sdk](https://www.nuget.org/packages/Aspire.AppHost.Sdk) is an additive [MSBuild project SDK](/visualstudio/msbuild/how-to-use-project-sdk) for building [Aspire apps](../index.yml). The `Aspire.AppHost.Sdk` is defined with a top-level `Project/Sdk`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

    <Sdk Name="Aspire.AppHost.Sdk" Version="9.5.0" />
    
    <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>net9.0</TargetFramework>
        <!-- Omitted for brevity -->
    </PropertyGroup>
    
    <ItemGroup>
        <PackageReference Include="Aspire.Hosting.AppHost" Version="9.5.0" />
    </ItemGroup>

    <!-- Omitted for brevity -->
</Project>
```

The preceding example project defines the top-level SDK as `Microsoft.NET.Sdk` and the `Aspire.AppHost.Sdk` as an additive SDK. The project also references the `Aspire.Hosting.AppHost` package which brings in a number of Aspire-related dependencies.

## SDK Features

The Aspire SDK provides several key features.

### Project references

Each `ProjectReference` in the [Aspire AppHost][app-host] project isn't treated as standard project references. Instead, they enable the _AppHost_ to execute these projects as part of its orchestration. Each project reference triggers a generator to create a `class` that represents the project as an <xref:Aspire.Hosting.IProjectMetadata>. This metadata is used to populate the named projects in the generated `Projects` namespace. When you call the <xref:Aspire.Hosting.ProjectResourceBuilderExtensions.AddProject*?displayProperty=fullName> API, the `Projects` namespace is used to reference the project—passing the generated class as a generic-type parameter.

> [!TIP]
> If you need to reference a project in the traditional way within the AppHost, set the `IsAspireProjectResource` attribute on the `ProjectReference` element to `false`, as shown in the following example:
>
> ```xml
> <ProjectReference Include="..\MyProject\MyProject.csproj" IsAspireProjectResource="false" />
> ```
>
> Otherwise, by default, the `ProjectReference` is treated as Aspire project resource.

### Orchestrator dependencies

The Aspire SDK dynamically adds references to the [Aspire dashboard](dashboard/overview.md) and other AppHost dependencies, such as the developer control plane (DCP) packages. These dependencies are specific to the platform that the AppHost is built on.

When the AppHost project runs, the orchestrator relies on these dependencies to provide the necessary functionality to the AppHost. For more information, see [Aspire orchestration overview][app-host].

[app-host]: xref:dotnet/aspire/app-host
