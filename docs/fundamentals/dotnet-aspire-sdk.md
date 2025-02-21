---
title: .NET Aspire SDK
description: Learn
ms.date: 11/09/2024
uid: dotnet/aspire/sdk
---

# .NET Aspire SDK

The .NET Aspire SDK is intended for [_*.AppHost_ projects](app-host-overview.md#app-host-project), which serve as the .NET Aspire orchestrator. These projects are designated using the `<IsAspireHost>true</IsAspireHost>` property, as well as specifying the `Aspire.AppHost.Sdk` in the project file. The SDK provides a set of features that simplify the development of .NET Aspire apps.

## Overview

The [📦 Aspire.AppHost.Sdk](https://www.nuget.org/packages/Aspire.AppHost.Sdk) is an additive [MSBuild project SDK](/visualstudio/msbuild/how-to-use-project-sdk) for building [.NET Aspire apps](../index.yml). The `Aspire.AppHost.Sdk` is defined with a top-level `Project/Sdk`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

    <Sdk Name="Aspire.AppHost.Sdk" Version="9.1.0" />
    
    <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>net9.0</TargetFramework>
        <IsAspireHost>true</IsAspireHost>
        <!-- Omitted for brevity -->
    </PropertyGroup>
    
    <ItemGroup>
        <PackageReference Include="Aspire.Hosting.AppHost" Version="9.1.0" />
    </ItemGroup>

    <!-- Omitted for brevity -->
</Project>
```

The preceding example project defines the top-level SDK as `Microsoft.NET.Sdk` and the `Aspire.AppHost.Sdk` as an additive SDK. The `IsAspireHost` property is set to `true` to indicate that this project is an .NET Aspire app host. The project also references the `Aspire.Hosting.AppHost` package which brings in a number of Aspire-related dependencies.

## SDK Features

The .NET Aspire SDK provides several key features.

### Project references

Each `ProjectReference` in the [.NET Aspire app host][app-host] project isn't treated as standard project references. Instead, they enable the _app host_ to execute these projects as part of its orchestration. Each project reference triggers a generator to create a `class` that represents the project as an <xref:Aspire.Hosting.IProjectMetadata>. This metadata is used to populate the named projects in the generated `Projects` namespace. When you call the <xref:Aspire.Hosting.ProjectResourceBuilderExtensions.AddProject*?displayProperty=fullName> API, the `Projects` namespace is used to reference the project—passing the generated class as a generic-type parameter.

> [!TIP]
> If you need to reference a project in the tranditional way within the app host, set the `IsAspireProjectResource` attribute on the `ProjectReference` element to `false`, as shown in the following example:
>
> ```xml
> <ProjectReference Include="..\MyProject\MyProject.csproj" IsAspireProjectResource="false" />
> ```

### Orchestrator dependencies

The .NET Aspire SDK dynamically adds references to the [.NET Aspire dashboard](dashboard/overview.md) and other app host dependencies, such as the developer control plane (DCP) packages. These dependencies are specific to the platform that the app host is built on.

When the app host project runs, the orchestrator relies on these dependencies to provide the necessary functionality to the app host. For more information, see [.NET Aspire orchestration overview][app-host].

[app-host]: xref:dotnet/aspire/app-host
