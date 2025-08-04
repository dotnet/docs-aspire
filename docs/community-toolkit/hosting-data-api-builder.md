---
title: Community Toolkit Azure Data API Builder hosting integration
author: tommasodotNET
description: Learn how to use the .NET Aspire Community Toolkit Azure Data API Builder hosting integration to host DAB as a container.
ms.date: 11/18/2024
ms.custom: sfi-ropc-nochange
---

# Community Toolkit Azure Data API Builder hosting integration

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

[!INCLUDE [banner](includes/banner.md)]

In this article, you learn how to use the .NET Aspire Data API Builder hosting integration to run [Data API Builder](/azure/data-api-builder/overview) as a container.

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

 In the app host project, register and consume the Data API Builder integration using the `AddDataAPIBuilder` extension method to add the Data API Builder container to the application builder.

```csharp
 var builder = DistributedApplication.CreateBuilder(); 

// Add Data API Builder using dab-config.json 
var dab = builder.AddDataAPIBuilder("dab");

builder.AddProject<Projects.ExampleProject>() 
        .WithReference(dab); 

 // After adding all resources, run the app... 
```

When the .NET Aspire adds a container image to the app host, as shown in the preceding example with the `mcr.microsoft.com/azure-databases/data-api-builder` image, it creates a new Data API Builder instance on your local machin. A reference to the DAB resource (the `dab` variable) is added to the `ExampleProject` project.

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

If you need to configure your own local database, you can refer to the [SQL Server integration](../database/sql-server-integration.md) documentation.

Once you have your database added as a resource, you can reference it using the following APIs chained to the `IResourceBuilder<DataApiBuilderContainerResource>`:

```csharp
var dab = builder.AddDataAPIBuilder("dab")
    .WithReference(sqlDatabase)
    .WaitFor(sqlDatabase);
```

The `WaitFor` method ensures that the database is ready before starting the Data API Builder container.

Referencing the `sqlDatabase` resource will inject its connection string into the Data API Builder container with the name `ConnectionStrings__<DATABASE_RESOURCE_NAME>`.
Next, update the `dab-config.json` file to include the connection string for the database:

```json
"data-source": {
    "connection-string": "@env('ConnectionStrings__<DATABASE_RESOURCE_NAME>')",
}
```

### Using multiple data sources

You can pass multiple configuration files to the `AddDataAPIBuilder` method:

```csharp
var dab = builder.AddDataAPIBuilder("dab", 
        "./dab-config-1.json", 
        "./dab-config-2.json")
    .WithReference(sqlDatabase1)
    .WaitFor(sqlDatabase1)
    .WithReference(sqlDatabase2)
    .WaitFor(sqlDatabase2);
```

> [!NOTE]
> All files are mounted/copied to the same `/App` folder.

## See also

- [.NET Aspire Community Toolkit GitHub repo](https://github.com/CommunityToolkit/Aspire)
- [Sample DAB](https://github.com/CommunityToolkit/Aspire/tree/main/examples/data-api-builder)
- [Further usage examples](https://github.com/CommunityToolkit/Aspire/blob/main/src/CommunityToolkit.Aspire.Hosting.Azure.DataApiBuilder/README.md#usage)
