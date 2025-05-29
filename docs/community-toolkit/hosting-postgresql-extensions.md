---
title: PostgreSQL hosting extensions
description: Learn how to use the .NET Aspire PostgreSQL extensions package which provides extra functionality to the .NET Aspire PostgreSQL hosting package.
ms.date: 05/28/2025
---

# .NET Aspire Community Toolkit PostgreSQL hosting extensions

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

[!INCLUDE [banner](includes/banner.md)]

In this article, you learn about the .NET Aspire Community Toolkit PostgreSQL hosting extensions package which provides extra functionality to the .NET Aspire [PostgreSQL hosting package](https://nuget.org/packages/Aspire.Hosting.PostgreSQL).

This package provides the following features:

- [Adminer](https://adminer.org/) management UI
- [DbGate](https://dbgate.org/) management UI

## Hosting integration

To get started with the .NET Aspire Community Toolkit PostgreSQL hosting extensions, install the [📦 CommunityToolkit.Aspire.Hosting.PostgreSQL.Extensions](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.PostgreSQL.Extensions) NuGet package in the AppHost project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.Hosting.PostgreSQL.Extensions
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.Hosting.PostgreSQL.Extensions"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

To add the DbGate management UI to your PostgreSQL resource, call the `WithDbGate` method on the `PostgresServerResource` instance.

```csharp
var postgresServer = builder.AddPostgreSQL("PostgreSQL")
    .WithDbGate();
```

To add the Adminer management UI to your PostgreSQL resource, call the `WithAdminer` method on the `PostgresServerResource` instance.

```csharp
var postgresServer = builder.AddPostgreSQL("PostgreSQL")
    .WithAdminer();
```

This will add a new resource to the app host which will be available from the .NET Aspire dashboard.
