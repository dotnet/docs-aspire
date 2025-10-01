---
title: SQL Server hosting extensions
description: Learn how to use the Aspire SQL Server extensions package which provides extra functionality to the Aspire SQL Server hosting package.
ms.date: 05/28/2025
---

# Aspire Community Toolkit SQL Server hosting extensions

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

[!INCLUDE [banner](includes/banner.md)]

In this article, you learn about the Aspire Community Toolkit SQL Server hosting extensions package which provides extra functionality to the Aspire [SQL Server hosting package](https://nuget.org/packages/Aspire.Hosting.SQLServer).

This package provides the following features:

- [Adminer](https://adminer.org/) management UI
- [DbGate](https://dbgate.org/) management UI

## Hosting integration

To get started with the Aspire Community Toolkit SQL Server hosting extensions, install the [📦 CommunityToolkit.Aspire.Hosting.SqlServer.Extensions](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.SqlServer.Extensions) NuGet package in the AppHost project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.Hosting.SqlServer.Extensions
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.Hosting.SqlServer.Extensions"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

To add the DbGate management UI to your SQL Server resource, call the `WithDbGate` method on the `SqlServerResourceBuilder` instance.

```csharp
var sqlserver = builder.AddSqlServer("sqlserver")
    .WithDbGate();
```

To add the Adminer management UI to your SQL Server resource, call the `WithAdminer` method on the `SqlServerResourceBuilder` instance.

```csharp
var sqlserver = builder.AddSqlServer("sqlserver")
    .WithAdminer();
```

This will add a new resource to the AppHost which will be available from the Aspire dashboard.
