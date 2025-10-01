---
title: Aspire Community Toolkit Deno hosting integration
description: Learn about the Aspire Community Toolkit Deno hosting extensions package which provides functionality to run Deno applications and tasks.
ms.date: 10/25/2024
---

# Aspire Community Toolkit Deno hosting integration

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

[!INCLUDE [banner](includes/banner.md)]

In this article, you learn about the Aspire Community Toolkit Deno package. The extensions package brings the following features:

- Running [Deno](https://deno.com/) applications
- Running Node.js applications via Deno tasks
- Ensuring that the packages are installed before running the application via Deno installer

## Hosting integration

To get started with the Aspire Community Toolkit Deno extensions, install the [📦 CommunityToolkit.Aspire.Hosting.Deno](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.Deno) NuGet package in the AppHost project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.Hosting.Deno
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.Hosting.Deno"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

The following sections detail various usages, from running Vite applications to using specific package managers.

### Run Deno apps

This integration extension adds support for running a Deno application defined in a script. Since [Deno is secure by default](https://docs.deno.com/runtime/fundamentals/security), permission flags must be specified in `permissionFlags` argument of `AddDenoApp`.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddDenoApp("oak-demo", "main.ts", permissionFlags: ["--allow-env", "--allow-net"])
    .WithHttpEndpoint(env: "PORT")
    .WithEndpoint();

builder.Build().Run();
```

The preceding code uses the fully qualified switches. Alternatively, you can use the equivalent alias as well. For more information, see [Deno docs: Security and permissions](https://docs.deno.com/runtime/fundamentals/security/#permissions).

### Run Deno tasks

This integration extension adds support for running tasks that are either specified in a _package.json_ or _deno.json_.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddDenoTask("vite-demo", taskName: "dev")
    .WithHttpEndpoint(env: "PORT")
    .WithEndpoint();

builder.Build().Run();
```

### Deno package installation

This integration extension adds support for installing dependencies that utilizes `deno install` behind the scenes by simply using
`WithDenoPackageInstallation`.

> [!NOTE]
> This API only works when a _deno.lock_ file present.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddDenoTask("vite-demo", taskName: "dev")
    .WithDenoPackageInstallation()
    .WithHttpEndpoint(env: "PORT")
    .WithEndpoint();
```

## See also

- [Aspire Community Toolkit GitHub repo](https://github.com/CommunityToolkit/Aspire)
- [Sample Deno apps](https://github.com/CommunityToolkit/Aspire/tree/main/examples/deno)
- [Deno Docs](https://docs.deno.com/)
