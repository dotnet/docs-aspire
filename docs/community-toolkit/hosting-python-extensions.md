---
title: Community Toolkit Python hosting extensions
description: Learn about the .NET Aspire Community Toolkit Python hosting extensions package which provides extra functionality to the .NET Aspire Python hosting package.
ms.date: 11/19/2024
---

# Community Toolkit Python hosting extensions

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

[!INCLUDE [banner](includes/banner.md)]

In this article, you learn about the .NET Aspire Community Toolkit Python hosting extensions package which provides extra functionality to the .NET Aspire [Python hosting package](https://nuget.org/packages/Aspire.Hosting.Python).
The extensions package lets you run [Uvicorn](https://www.uvicorn.org/) and [uv](https://docs.astral.sh/uv/) applications.

## Hosting integration

To get started with the .NET Aspire Community Toolkit Python hosting extensions, install the [📦 CommunityToolkit.Aspire.Hosting.Python.Extensions](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.Python.Extensions) NuGet package in the AppHost project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.Hosting.Python.Extensions
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.Hosting.Python.Extensions"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

The following sections detail various usages, from running Uvicorn applications to using specific package managers such as uv.

To work with Python apps, they need to be within a virtual environment. To create a virtual environment, refer to the [Initialize the Python virtual environment](../get-started/build-aspire-apps-with-python.md?tabs=powershell#initialize-the-python-virtual-environment) section.

The `PORT` environment variable is used to determine the port the Uvicorn application should listen on. By default, this port is randomly assigned by .NET Aspire. The name of the environment variable can be changed by passing a different value to the <xref:Aspire.Hosting.ResourceBuilderExtensions.WithHttpEndpoint*> method.

# [Uvicorn](#tab/uvicorn)

In the _:::no-loc text="Program.cs":::_ file of your app host project, call the `AddUvicornApp` method to add a Uvicorn application to the builder.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var uvicorn = builder.AddUvicornApp(
        name: "uvicornapp",
        projectDirectory: "../uvicornapp-api",
        appName: "main:app"
    )
    .WithHttpEndpoint(env: "PORT");

builder.Build().Run();
```

The Uvicorn application can be added as a reference to other resources in the AppHost project.

# [uv](#tab/uv)

In the _:::no-loc text="Program.cs":::_ file of your app host project, call the `AddUvApp` method to add a uv application to the builder.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var uvicorn = builder.AddUvApp(
        name: "uvapp", 
        projectDirectory: "../uv-api", 
        scriptPath: "uv-api")
    .WithHttpEndpoint(env: "PORT");

builder.Build().Run();
```

The uv application can be added as a reference to other resources in the AppHost project.

---

## See also

- [Orchestrate Python apps in .NET Aspire](../get-started/build-aspire-apps-with-Python.md)
- [.NET Aspire Community Toolkit GitHub repo](https://github.com/CommunityToolkit/Aspire)
- [Sample Python apps](https://github.com/CommunityToolkit/Aspire/tree/main/examples/python)
