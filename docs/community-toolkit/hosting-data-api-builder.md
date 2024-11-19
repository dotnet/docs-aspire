---
title: Community Toolkit Azure Data API Builder hosting integration
author: tommasodotNET
description: Learn how to use the .NET Aspire Community Toolkit Azure Data API Builder hosting integration to host DAB as a container.
ms.date: 11/18/2024
---

# Community Toolkit Azure Data API Builder hosting integration

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

[!INCLUDE [banner](includes/banner.md)]

In this article, you learn how to use the .NET Aspire Data API Builder hosting integration to run [Data API Builder](https://learn.microsoft.com/azure/data-api-builder/overview) as a container.

## Hosting integration

To get started with the .NET Aspire Azure Data API Builder hosting integration, install the [📦 CommunityToolkit.Aspire.Hosting.Azure.DataApiBuilder](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.Azure.DataApiBuilder) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.Hosting.Azure.DataApiBuilder
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.Hosting.Azure.DataApiBuilder"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Usage

In the `Program.cs` file of your app host project, call the `AddDataAPIBuilder` method to add a Go application to the builder.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add Data API Builder using dab-config.json 
var dab = builder.AddDataAPIBuilder("dab")
    .WithReference(sqlDatabase)
    .WaitFor(sqlServer);

builder.Build().Run();
```

### Configuration

| Parameter | Description |
|--|--|--|
| `name` | The name of the resource is a required `string` and it's validated by the <xref: Aspire.Hosting.ApplicationModel.ResourceNameAttribute>. |
| `configFilePaths` | The paths to the configuration or schema file(s) for Data API builder. These are optional and are available as a `params string[]`, meaning you can omit them altogether, or provide one or more path inline. When omitted, it defaults to `"./dab-config.json"`. |
| `httpPort` | The port number for the Data API Builder container is represented as a an `int?`. By default, the port is `null`, .NET Aspire assigns a port when this isn't otherwise provided. |

### Data API Builder container image configuration

You can specify custom container `registry/image/tag` values by using the following APIs chained to the `IResourceBuilder<DataApiBuilderContainerResource>`:

- `WithImageRegistry`: Pass the desired registry name, such as `ghcr.io` for the GitHub Container Registry or `docker.io` for Docker.
- `WithImage`: Provide the name of the image, such as `azure-databases/data-api-builder`.
- `WithImageTag`: Specify an image tag to use other than `latest`, which is the default in most cases.

Consider the following example that demonstrates chaining these APIs together, to fluently express that the Data API Builder's container image is fully qualified as `mcr.microsoft.com/azure-databases/data-api-builder:latest`:

```csharp
var dab = builder.AddDataAPIBuilder("dab")
    .WithImageRegistry("mcr.microsoft.com")
    .WithImage("azure-databases/data-api-builder")
    .WithImageTag("latest");
```

### Database Configuration

In the following example, you use a generated password for the database and aren't persisting the data. In a production scenario, you probably want to specify the password and persist the data, so it doesn't get lost when the container restarts. Consider the following C# code that defines a password parameter named `sql-password`:

```csharp
// Add a SQL Server container
var sqlPassword = builder.AddParameter("sql-password");
var sqlServer = builder
    .AddSqlServer("sql", sqlPassword)
    .WithDataVolume("MyDataVolume");

var sqlDatabase = sqlServer.AddDatabase("your-database-name");
```

### Example 2: Multiple data sources

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var sqlDatabase1 = builder
    .AddSqlServer("your-server-name")
    .AddDatabase("your-database-name");

var sqlDatabase2 = builder
    .AddSqlServer("your-server-name")
    .AddDatabase("your-database-name");

var dab = builder.AddDataAPIBuilder("dab", 
        "./dab-config-1.json", 
        "./dab-config-2.json")
    .WithReference(sqlDatabase1)
    .WaitFor(sqlDatabase1)
    .WithReference(sqlDatabase2)
    .WaitFor(sqlDatabase2);

var app = builder
    .AddProject<Projects.Client>()
    .WithReference(dab);

builder.Build().Run();
```

> [!NOTE] 
> All files are mounted/copied to the same `/App` folder.

## See also

- [.NET Aspire Community Toolkit GitHub repo](https://github.com/CommunityToolkit/Aspire)
- [Sample DAB](https://github.com/CommunityToolkit/Aspire/tree/main/examples/data-api-builder)
