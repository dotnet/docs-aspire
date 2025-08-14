---
title: Compiler Error ASPIRE007
description: Learn more about compiler Error ASPIRE007. 'Project' requires a reference to "Aspire.AppHost.Sdk" with version "9.0.0" or greater to work correctly.
ms.date: 05/08/2025
f1_keywords:
  - "ASPIRE007"
helpviewer_keywords:
  - "ASPIRE007"
---

# Compiler Error ASPIRE007

**Version introduced:** 9.0.0

> 'Project' requires a reference to "Aspire.AppHost.Sdk" with version "9.0.0" or greater to work correctly. Please add the following line after the Project declaration `<Sdk Name="Aspire.AppHost.Sdk" Version="9.0.0" />`.

The .NET Aspire AppHost package references require .NET Aspire SDK version 9.0.0 or greater. The SDK reference is either being omitted or is using a version older than 9.0.0.

## To correct this error

Update the referenced SDK version to 9.0.0 or the latest.

If the SDK reference is missing from your project file, add it, as demonstrated in the following snippet:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <Sdk Name="Aspire.AppHost.Sdk" Version="9.0.0" />

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsAspireHost>true</IsAspireHost>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Aspire.Hosting.AppHost" Version="9.2.0" />
  </ItemGroup>

</Project>
```
