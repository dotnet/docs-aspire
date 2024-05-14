---
title: Use Dapr with .NET Aspire
description: Learn how to use Dapr with .NET Aspire
ms.date: 05/14/2024
ms.topic: overview
---

# Use Dapr with .NET Aspire

[Distributed Application Runtime (Dapr)](https://docs.dapr.io/) offers developer APIs that run as a sidecar process and abstract away the common complexities of the underlying cloud platform. Dapr and Aspire work together to improve your local development experience. By using Dapr with Aspire, you can focus on writing and implementing .NET-based distributed applications instead of spending extra time with local onboarding.  

In this guide, you'll learn how to take advantage of Dapr's abstraction and Aspire's opinionated configuration of cloud technologies to build simple, portable, resilient, and secured microservices at-scale on Azure.

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
