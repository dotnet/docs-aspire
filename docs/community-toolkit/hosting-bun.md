---
title: Bun hosting
author: aaronpowell
description: Learn how to use the .NET Aspire Bun hosting integration to host Bun applications.
ms.date: 11/15/2024
---

# .NET Aspire Bun hosting

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

[!INCLUDE [banner](includes/banner.md)]

In this article, you learn how to use the .NET Aspire Bun hosting integration to host [Bun](https://bun.sh) applications.

## Hosting integration

To get started with the .NET Aspire Bun hosting integration, install the [📦 CommunityToolkit.Aspire.Hosting.Bun](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.Bun) NuGet package in the AppHost project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.Hosting.Bun
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.Hosting.Bun"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _:::no-loc text="Program.cs":::_ file of your app host project, call the `AddBunApp` method to add a Bun application to the builder.

```csharp
var api = builder.AddBunApp("api")
    .WithHttpEndpoint(env: "PORT");
```

By default the working directory of the application will be a sibling folder to the app host matching the name provided to the resource, and the entrypoint will be _:::no-loc text="index.ts"::_. Both of these can be customized by passing additional parameters to the `AddBunApp` method.

```csharp
var api = builder.AddBunApp("api", "../api-service", "start")
    .WithHttpEndpoint(env: "PORT");
```

The Bun application can be added as a reference to other resources in the app host project.

## See also

- [.NET Aspire Community Toolkit GitHub repo](https://github.com/CommunityToolkit/Aspire)
- [Sample Bun app](https://github.com/CommunityToolkit/Aspire/tree/main/examples/bun)
