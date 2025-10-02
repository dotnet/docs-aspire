---
title: Compiler Error ASPIRE008
description: Learn more about compiler Error ASPIRE008. The Aspire workload that this project depends on is now deprecated.
ms.date: 07/22/2025
f1_keywords:
  - "ASPIRE008"
helpviewer_keywords:
  - "ASPIRE008"
---

# Compiler Error ASPIRE008

**Version introduced:** 8.2.3

> The Aspire workload that this project depends on is now deprecated.

This error appears when a project uses a version of Aspire that relies on the SDK workload, which is now deprecated. The error guides you to migrate your project to a supported version of Aspire that uses the SDK approach instead of the workload.

## Example

The following AppHost project file uses the deprecated Aspire workload:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsAspireHost>true</IsAspireHost>
    <UserSecretsId>98048c9c-bf28-46ba-a98e-63767ee5e3a8</UserSecretsId>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Aspire.Hosting.AppHost" Version="8.2.2" />
  </ItemGroup>

</Project>
```

For more information on the Aspire SDK, see [Aspire SDK](../fundamentals/dotnet-aspire-sdk.md).

## To correct this error

Follow the migration guide at <https://aka.ms/aspire/update-to-sdk> to upgrade your project to a supported version of Aspire that uses the SDK approach.

The migration typically involves:

1. Updating your AppHost project file to use the `Aspire.AppHost.Sdk`.
1. Removing references to the deprecated workload.
1. Updating package references to supported versions.

## Suppress the error

> [!WARNING]
> Suppressing this error isn't recommended, as it leaves your project depending on an unsupported version of Aspire.

If you need to temporarily suppress this error, add the following property to your project file:

```xml
<PropertyGroup>
  <SuppressAspireWorkloadDeprecationError>true</SuppressAspireWorkloadDeprecationError>
</PropertyGroup>
```
