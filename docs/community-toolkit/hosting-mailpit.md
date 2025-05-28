---
title: .NET Aspire Community Toolkit MailPit integration
description: Learn how to use the .NET Aspire MailPit hosting integration to run the MailPit container.
ms.date: 02/17/2025
---

# .NET Aspire Community Toolkit MailPit integration

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

[!INCLUDE [banner](includes/banner.md)]

In this article, you learn how to use the .NET Aspire MailPit hosting integration to run [MailPit](https://mailpit.axllent.org/) container.

## Hosting integration

To run the MailPit container, install the [📦 CommunityToolkit.Aspire.Hosting.MailPit](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.MailPit) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.Hosting.MailPit
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.Hosting.MailPit"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add MailPit resource

In the app host project, register and consume the MailPit integration using the `AddMailPit` extension method to add the MailPit container to the application builder.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var mailpit = builder.AddMailPit("mailpit");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(mailpit);

// After adding all resources, run the app...
```

When .NET Aspire adds a container image to the app host, as shown in the preceding example with the `docker.io/axllent/mailpit` image, it creates a new MailPit instance on your local machine. A reference to your MailPit resource (the `mailpit` variable) is added to the `ExampleProject`.

For more information, see [Container resource lifecycle](../fundamentals/app-host-overview.md#container-resource-lifecycle).

### Add MailPit resource with data volume

To add a data volume to the MailPit resource, call the `Aspire.Hosting.MailPitBuilderExtensions.WithDataVolume` method on the MailPit resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var mailpit = builder.AddMailPit("mailpit")
                         .WithDataVolume("mailpit-data");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(mailpit);

// After adding all resources, run the app...
```

The data volume is used to persist the MailPit data outside the lifecycle of its container. The data volume is mounted at the `/data` path in the MailPit container and when a `name` parameter isn't provided, the name is generated at random. For more information on data volumes and details on why they're preferred over [bind mounts](#add-mailpit-resource-with-data-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

### Add MailPit resource with data bind mount

To add a data bind mount to the MailPit resource, call the `Aspire.Hosting.MailPitBuilderExtensions.WithDataBindMount` method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var mailpit = builder.AddMailPit("mailpit")
                     .WithDataBindMount(source: @"C:\MailPit\Data");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(mailpit);

// After adding all resources, run the app...
```

[!INCLUDE [data-bind-mount-vs-volumes](../includes/data-bind-mount-vs-volumes.md)]

Data bind mounts rely on the host machine's filesystem to persist the MailPit data across container restarts. The data bind mount is mounted at the `C:\MailPit\Data` on Windows (or `/MailPit/Data` on Unix) path on the host machine in the MailPit container. For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

## Client integration

There isn't a Community Toolkit client integration, however, if you're consuming the resource, it's good to know how to configure your clients.

### Use a connection string

MailPit automatically creates a `ConnectionStrings` configuration section which can be consumed by your client application.

Then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "mailpit": "Endpoint=smtp://localhost:19530"
  }
}
```

## See also

- [MailPit](https://mailpit.axllent.org/)
- [.NET Aspire Community Toolkit GitHub repo](https://github.com/CommunityToolkit/Aspire)
