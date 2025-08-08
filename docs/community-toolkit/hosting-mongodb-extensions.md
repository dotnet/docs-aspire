---
title: MongoDB hosting extensions
description: Learn how to use the .NET Aspire MongoDB extensions package which provides extra functionality to the .NET Aspire MongoDB hosting package.
ms.date: 03/04/2025
---

# .NET Aspire Community Toolkit MongoDB hosting extensions

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

[!INCLUDE [banner](includes/banner.md)]

In this article, you learn about the .NET Aspire Community Toolkit MongoDB hosting extensions package which provides extra functionality to the .NET Aspire [MongoDB hosting package](https://nuget.org/packages/Aspire.Hosting.MongoDB).

This package provides the following features:

- [DbGate](https://dbgate.org/) management UI

## Hosting integration

To get started with the .NET Aspire Community Toolkit MongoDB hosting extensions, install the [📦 CommunityToolkit.Aspire.Hosting.MongoDB.Extensions](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.MongoDB.Extensions) NuGet package in the AppHost project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.Hosting.MongoDB.Extensions
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.Hosting.MongoDB.Extensions"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

To add the DbGate management UI to your MongoDB resource, call the `WithDbGate` method on the `MongoDBResourceBuilder` instance.

```csharp
var MongoDB = builder.AddMongoDB("MongoDB")
    .WithDbGate();
```

This will add a new resource to the AppHost which will be available from the .NET Aspire dashboard.
