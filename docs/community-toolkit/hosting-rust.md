---
title: Rust hosting
author: Alirexaa
description: Learn how to use the .NET Aspire Rust hosting integration to host Rust applications.
ms.date: 11/12/2024
---

# .NET Aspire Rust hosting

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

[!INCLUDE [banner](includes/banner.md)]

In this article, you learn how to use the .NET Aspire Rust hosting integration to host Rust applications.

## Hosting integration

To get started with the .NET Aspire Rust hosting integration, install the [📦 CommunityToolkit.Aspire.Hosting.Rust](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.Rust) NuGet package in the AppHost project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.Hosting.Rust
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.Hosting.Rust"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _:::no-loc text="Program.cs":::_ file of your app host project, call the `AddRustApp` method to add a Rust application to the builder.

```csharp
var rust = builder.AddRustApp("rust-app", "../rust-service");
```

The Rust application can be added as a reference to other resources in the AppHost project.

## See also

- [.NET Aspire Community Toolkit GitHub repo](https://github.com/CommunityToolkit/Aspire)
- [Sample Rust app](https://github.com/CommunityToolkit/Aspire/tree/main/examples/rust)
