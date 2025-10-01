---
title: Aspire Bun hosting integration
author: aaronpowell
description: Learn how to use the Aspire Bun hosting integration to host Bun applications.
ms.date: 11/15/2024
---

# Aspire Bun hosting

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

[!INCLUDE [banner](includes/banner.md)]

[Bun](https://bun.sh) is a modern, fast, and lightweight framework for building web applications with TypeScript. The Aspire Bun hosting integration allows you to host Bun applications in your Aspire AppHost project, and provide it to other resources in your application.

## Hosting integration

The Bun hosting integration models a Bun application as the `Aspire.Hosting.ApplicationModel.BunAppResource` type. To access this type and APIs that allow you to add it to your AppHost project, install the [📦 CommunityToolkit.Aspire.Hosting.Bun](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.Bun) NuGet package in the AppHost project.

This integration expects that the Bun executable has already been installed on the host machine, and that it's available in the system path.

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

## Add a Bun resource

In your AppHost project, call the `Aspire.Hosting.BunAppExtensions.AddBunApp` on the `builder` instance to add a Bun application resource as shown in the following example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddBunApp("api")
                 .WithHttpEndpoint(env: "PORT");

var exampleProject = builder.AddProject<Projects.ExampleProject>()
                            .WithReference(api);

// After adding all resources, run the app...
```

By default the working directory of the application will be a sibling folder to the AppHost matching the name provided to the resource, and the entrypoint will be _:::no-loc text="index.ts"::_. Both of these can be customized by passing additional parameters to the `AddBunApp` method.

```csharp
var api = builder.AddBunApp("api", "../api-service", "start")
    .WithHttpEndpoint(env: "PORT");
```

The Bun application can be added as a reference to other resources in the AppHost project.

### Ensuring packages are installed

To ensure that the Bun application has all the dependencies installed as defined in the lockfile, you can use the `Aspire.Hosting.BunAppExtensions.WithBunPackageInstaller` method to ensure that package installation is run before the application is started.

```csharp
var api = builder.AddBunApp("api")
                 .WithHttpEndpoint(env: "PORT")
                 .WithBunPackageInstaller();
```

## See also

- [Aspire Community Toolkit GitHub repo](https://github.com/CommunityToolkit/Aspire)
- [Sample Bun app](https://github.com/CommunityToolkit/Aspire/tree/main/examples/bun)
