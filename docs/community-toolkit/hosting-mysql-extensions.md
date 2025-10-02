---
title: MySql hosting extensions
description: Learn how to use the Aspire MySql extensions package which provides extra functionality to the Aspire MySql hosting package.
ms.date: 05/28/2025
author: Alirexaa
---

# Aspire Community Toolkit MySql hosting extensions

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

[!INCLUDE [banner](includes/banner.md)]

In this article, you learn about the Aspire Community Toolkit MySql hosting extensions package which provides extra functionality to the Aspire [MySql hosting package](https://nuget.org/packages/Aspire.Hosting.MySql).

This package provides the following features:

- [Adminer](https://adminer.org/) management UI
- [DbGate](https://dbgate.org/) management UI

## Hosting integration

To get started with the Aspire Community Toolkit MySql hosting extensions, install the [📦 CommunityToolkit.Aspire.Hosting.MySql.Extensions](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.MySql.Extensions) NuGet package in the AppHost project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.Hosting.MySql.Extensions
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.Hosting.MySql.Extensions"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

To add the DbGate management UI to your MySql resource, call the `WithDbGate` method on the `MySqlServerResourceBuilder` instance.

```csharp
var mysql = builder.AddMySql("mysql")
    .WithDbGate();
```

To add the Adminer management UI to your MySql resource, call the `WithAdminer` method on the `MySqlServerResourceBuilder` instance.

```csharp
var mysql = builder.AddMySql("mysql")
    .WithAdminer();
```

This will add a new resource to the AppHost which will be available from the Aspire dashboard.
