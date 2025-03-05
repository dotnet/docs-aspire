---
title: Redis hosting extensions
description: Learn how to use the .NET Aspire Redis extensions package which provides extra functionality to the .NET Aspire Redis hosting package.
ms.date: 03/04/2025
---

# .NET Aspire Community Toolkit Redis hosting extensions

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

[!INCLUDE [banner](includes/banner.md)]

In this article, you learn about the .NET Aspire Community Toolkit Redis hosting extensions package which provides extra functionality to the .NET Aspire [Redis hosting package](https://nuget.org/packages/Aspire.Hosting.Redis).

This package provides the following features:

- [DbGate](https://dbgate.org/) management UI

## Hosting integration

To get started with the .NET Aspire Community Toolkit Redis hosting extensions, install the [📦 CommunityToolkit.Aspire.Hosting.Redis.Extensions](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.Redis.Extensions) NuGet package in the AppHost project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.Hosting.Redis.Extensions
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.Hosting.Redis.Extensions"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

To add the DbGate management UI to your Redis resource, call the `WithDbGate` method on the `RedisResource` instance.

```csharp
var redis = builder.AddRedis("Redis")
    .WithDbGate();
```

This will add a new resource to the app host which will be available from the .NET Aspire dashboard.
