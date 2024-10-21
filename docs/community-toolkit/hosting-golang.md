---
title: Go hosting
author: tommasodotNET
description: Learn how to use the .NET Aspire Go hosting integration to host Go applications.
ms.date: 10/11/2024
---

# .NET Aspire Go hosting

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

[!INCLUDE [banner](includes/banner.md)]

In this article, you learn how to use the .NET Aspire Go hosting integration to host Go applications.

## Hosting integration

To get started with the .NET Aspire Go hosting integration, install the [📦 Aspire.CommunityToolkit.Hosting.Go](https://dev.azure.com/dotnet/CommunityToolkit/_artifacts/feed/CommunityToolkit-MainLatest/NuGet/Aspire.CommunityToolkit.Hosting.Golang) NuGet package in the AppHost project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.CommunityToolkit.Hosting.Golang
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.CommunityToolkit.Hosting.Golang"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _:::no-loc text="Program.cs":::_ file of your app host project, call the `AddGolangApp` method to add a Go application to the builder.

```csharp
var golang = builder.AddGolangApp("golang", "../go-service");
```

The Go application can be added as a reference to other resources in the AppHost project.

## See also

- [.NET Aspire Community Toolkit GitHub repo](https://github.com/CommunityToolkit/Aspire)
- [Sample Go app](https://github.com/CommunityToolkit/Aspire/tree/main/examples/golang)
