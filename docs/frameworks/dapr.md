---
title: Use Dapr with .NET Aspire
description: Learn how to use Dapr with .NET Aspire
ms.date: 05/14/2024
ms.topic: overview
---

# Use Dapr with .NET Aspire

[Distributed Application Runtime (Dapr)](https://docs.dapr.io/) offers developer APIs, which run as a sidecar process and abstract away the common complexities of the underlying cloud platform, resulting in a low-maintenance, serverless, and scalable platform. [.NET Aspire](../get-started/aspire-overview.md) provides an opinionated configuration around the underlying cloud platform. 

Dapr and Aspire work hand-in-hand to help you build simple, portable, resilient, and secured microservices at-scale on Azure. Dapr provides APIs for building reliable, secure microservices, while Aspire provides built-in service discovery, resiliency, and health checks.

By combining Dapr with Aspire, you can improve your local development experience by simplifying how Dapr sidecars are attached to your application with imperative code. Aspire orchestrates the local developer inner loop and streamlines deployment. You can focus on writing and implementing .NET-based distributed applications instead of setting up your local environment.  

In this guide, you'll learn how to take advantage of Dapr's abstraction layer and Aspire's opinionated configuration of cloud technologies.

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
