---
title: .NET Aspire SDK
description: Learn
ms.date: 10/17/2024
uid: dotnet/aspire/sdk
---

# .NET Aspire SDK

The .NET Aspire SDK is tailored for [_*.AppHost_ projects](app-host-overview.md), which serve as entry points for .NET Aspire app hosts (`<IsAspireHost>true</IsAspireHost>`).

## Overview

The [📦 Aspire.AppHost.Sdk](https://www.nuget.org/packages/Aspire.AppHost.Sdk) is an additive [MSBuild project SDK](/visualstudio/msbuild/how-to-use-project-sdk) for building [.NET Aspire apps](../index.yml). The `Aspire.AppHost.Sdk` is defined in conjunction with a top-level `Project/Sdk`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

    <Sdk Name="Aspire.AppHost.Sdk" Version="9.0.0" />
    
    <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>net9.0</TargetFramework>
        <IsAspireHost>true</IsAspireHost>
        <!-- Omitted for brevity -->
    </PropertyGroup>
    
    <!-- Omitted for brevity -->
</Project>
```

The preceding example project defines the top-level SDK as `Microsoft.NET.Sdk` and the `Aspire.AppHost.Sdk` as an additive SDK. The `IsAspireHost` property is set to `true` to indicate that this project is an .NET Aspire app host.

### SDK Features

Features enabled by using the .NET Aspire SDK:

- Projects within the solution that rely on `ProjectReferences` aren't treated as project references. This enables the [.NET Aspire app host](app-host-overview.md) to execute these projects as part of its orchestration.
- Dynamically adds references to the [.NET Aspire dashboard](dashboard/overview.md) and other app host dependencies, such as, the developer control plane (DCP) packages. These dependencies are specific to the platform you're building on.

## Properties

The following properties are available when using the .NET Aspire SDK:

| Property  | Description          |
|-----------|----------------------|
| `Example` | Example description. |
