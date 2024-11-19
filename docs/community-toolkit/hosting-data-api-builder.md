---
title: Community Toolkit Azure Data API Builder hosting integration
author: tommasodotNET
description: Learn how to use the .NET Aspire Data API Builder hosting integration to host DAB as a container.
ms.date: 11/18/2024
---

# .NET Aspire Data API Builder hosting

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

[!INCLUDE [banner](includes/banner.md)]

In this article, you learn how to use the .NET Aspire Data API Builder hosting integration to run [Data API Builder](https://learn.microsoft.com/azure/data-api-builder/overview) as a container.

## Hosting integration

To get started with the .NET Aspire Go hosting integration, install the [📦 CommunityToolkit.Aspire.Hosting.Azure.DataApiBuilder](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.Azure.DataApiBuilder) NuGet package in the AppHost project.

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

- `name` - The name of the resource.
- `configFilePaths` - Opiotnal paths to the config/schema file(s) for Data API builder. Default is `./dab-config.json`.
- `httpPort` - The port number for the Data API Builder container. Defaults to `null` so that Aspire can assign a random port.

### Data API Builder Container Image Configuration

You can specify custom registry/image/tag values by using the `WithImageRegistry`/`WithImage`/`WithImageTag` methods:

```csharp
var dab = builder.AddDataAPIBuilder("dab")
    .WithImageRegistry("mcr.microsoft.com")
    .WithImage("azure-databases/data-api-builder")
    .WithImageTag("latest");
```

### Database Configuration

In the example we are using a generated password for the database and are not persisting the data. In a production scenario, you probably want to specify the password and persist the data so it does not get lost when the container is restarted.
Here is an example of how you can configure the database:

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
    .WithReference(sqlDatabase2)
    .WaitFor(sqlDatabase1)
    .WaitFor(sqlDatabase2);

var app = builder
    .AddProject<Projects.Client>()
    .WithReference(dab);

builder.Build().Run();
```

> Note: All files are mounted/copied to the same `/App` folder.

## See also

- [.NET Aspire Community Toolkit GitHub repo](https://github.com/CommunityToolkit/Aspire)
- [Sample DAB](https://github.com/CommunityToolkit/Aspire/tree/main/examples/data-api-builder)
