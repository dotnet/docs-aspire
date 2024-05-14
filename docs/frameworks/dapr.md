---
title: Use Dapr with .NET Aspire
description: Learn how to use Dapr with .NET Aspire
ms.date: 05/14/2024
ms.topic: overview
---

# Use Dapr with .NET Aspire

<!--
  TODO:
    Add intro about what Dapr is, what it provides, and why it's relevant.
    Then transition into how it relates to .NET Aspire
-->

## Prerequisites

- TODO: Add prerequisites

For more information, see [.NET Aspire setup and tooling](../fundamentals/setup-tooling.md).

## Get started

To get started you need to add the Dapr hosting package to your app host project by installing the [Aspire.Hosting.Dapr](https://www.nuget.org/packages/Aspire.Hosting.Dapr) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Dapr --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Dapr"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).
