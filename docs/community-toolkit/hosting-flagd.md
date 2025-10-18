---
title: flagd hosting extensions
description: Learn about the Aspire Community Toolkit flagd hosting extensions package which provides functionality to host flagd feature flag evaluation containers.
ms.date: 10/16/2025
ai-usage: ai-generated
---

# Aspire flagd integration

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

[!INCLUDE [banner](includes/banner.md)]

[flagd](https://flagd.dev) is a feature flag evaluation engine that provides an OpenFeature-compliant backend system for managing and evaluating feature flags in real-time. The Aspire flagd integration enables you to create new container instances from .NET with the [`ghcr.io/open-feature/flagd` container image](https://github.com/open-feature/flagd/pkgs/container/flagd).

flagd is designed to be a flexible, open-source feature flag evaluation engine. It enables real-time flag modifications, supports various flag types (boolean, string, number, JSON), uses context-sensitive rules for targeting, and performs pseudorandom assignments for experimentation. You can aggregate flag definitions from multiple sources and expose them through gRPC or OFREP services. flagd is designed to fit well into various infrastructures and can run as a separate process or directly in your application.

## Hosting integration

The flagd hosting integration models a flagd server as the `Aspire.Hosting.ApplicationModel.FlagdResource` type. To access this type and its APIs add the [📦 CommunityToolkit.Aspire.Hosting.Flagd](https://www.nuget.org/packages/CommunityToolkit.Aspire.Hosting.Flagd) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.Hosting.Flagd
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.Hosting.Flagd"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add flagd server resource

In your AppHost project, call `AddFlagd` on the `builder` instance to add a flagd container resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var flagd = builder.AddFlagd("flagd")
                   .WithBindFileSync("./flags/");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(flagd);

// After adding all resources, run the app...
```

When Aspire adds a container image to the app host, as shown in the preceding example with the `ghcr.io/open-feature/flagd` image, it creates a new flagd server instance on your local machine. A reference to your flagd server (the `flagd` variable) is added to the `ExampleProject`.

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` named `"flagd"`. For more information, see [Built-in resource types](../fundamentals/app-host-overview.md#built-in-resource-types).

> [!IMPORTANT]
> The flagd container requires a sync source to be configured. Use the `WithBindFileSync` method to configure file-based flag synchronization.

### Add flagd server resource with bind mount

To add a bind mount to the flagd container resource, call the `Aspire.Hosting.FlagdBuilderExtensions.WithBindFileSync` method on the flagd container resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var flagd = builder.AddFlagd("flagd")
                   .WithBindFileSync(
                       fileSource: "./flags/",
                       filename: "flagd.json");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(flagd);

// After adding all resources, run the app...
```

The bind mount is used to provide flagd with access to your flag configuration files. The `fileSource` parameter specifies the path on your host machine where the flag configuration file is located, and the `filename` parameter specifies the name of the flag configuration file. The default filename is `flagd.json`.

For more information on bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

### Flag configuration format

flagd uses JSON files for flag definitions. Create a folder named `flags` in your project root and place your `flagd.json` file inside it. The flag configuration file must be named `flagd.json`.

Here's a simple example:

```json
{
    "$schema": "https://flagd.dev/schema/v0/flags.json",
    "flags": {
        "welcome-banner": {
            "state": "ENABLED",
            "variants": {
                "on": true,
                "off": false
            },
            "defaultVariant": "off"
        },
        "background-color": {
            "state": "ENABLED",
            "variants": {
                "red": "#FF0000",
                "blue": "#0000FF",
                "yellow": "#FFFF00"
            },
            "defaultVariant": "red"
        }
    }
}
```

For more information on flag configuration, see the [flagd flag definitions documentation](https://flagd.dev/reference/flag-definitions/).

### Configure logging

To configure debug logging for the flagd container resource, call the `Aspire.Hosting.FlagdBuilderExtensions.WithLogLevel` method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var flagd = builder.AddFlagd("flagd")
                   .WithBindFileSync("./flags/")
                   .WithLogLevel(Microsoft.Extensions.Logging.LogLevel.Debug);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(flagd);

// After adding all resources, run the app...
```

The `WithLogLevel` method enables debug logging in the flagd container, which provides verbose output for troubleshooting flag evaluation issues. Currently, only `Debug` log level is supported.

### Customize ports

To customize the ports used by the flagd container resource, provide the `port` and `ofrepPort` parameters to the `AddFlagd` method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var flagd = builder.AddFlagd(
    name: "flagd",
    port: 8013,
    ofrepPort: 8016)
    .WithBindFileSync("./flags/");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(flagd);

// After adding all resources, run the app...
```

The `port` parameter specifies the host port for the flagd HTTP endpoint, and the `ofrepPort` parameter specifies the host port for the OFREP (OpenFeature Remote Evaluation Protocol) endpoint. If these parameters aren't provided, random ports are assigned.

### Hosting integration health checks

The flagd hosting integration automatically adds a health check for the flagd server resource. The health check verifies that the flagd server is running and that a connection can be established to it.

The hosting integration uses the flagd `/healthz` endpoint to perform health checks.

## Client integration

To get started with the Aspire flagd client integration, install an OpenFeature provider for .NET. The most common provider for flagd is the [📦 OpenFeature.Contrib.Providers.Flagd](https://www.nuget.org/packages/OpenFeature.Contrib.Providers.Flagd) NuGet package in the client-consuming project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package OpenFeature.Contrib.Providers.Flagd
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="OpenFeature.Contrib.Providers.Flagd"
                  Version="*" />
```

---

### Add flagd client

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, configure the OpenFeature SDK to use the flagd provider:

```csharp
using OpenFeature;
using OpenFeature.Contrib.Providers.Flagd;

var connectionString = builder.Configuration.GetConnectionString("flagd");

await OpenFeature.Api.Instance.SetProviderAsync(
    new FlagdProvider(new Uri(connectionString)));

var flagClient = OpenFeature.Api.Instance.GetClient();
```

You can then use the `flagClient` to evaluate feature flags:

```csharp
var welcomeBanner = await flagClient.GetBooleanValueAsync(
    "welcome-banner", 
    defaultValue: false);

var backgroundColor = await flagClient.GetStringValueAsync(
    "background-color", 
    defaultValue: "#000000");
```

For more information on the OpenFeature SDK and flagd provider, see the [OpenFeature .NET documentation](https://openfeature.dev/docs/reference/technologies/client/dotnet/) and the [flagd provider documentation](https://flagd.dev/providers/dotnet/).

### Configuration

The flagd client integration uses the connection string from the `ConnectionStrings` configuration section. The connection string is automatically provided when you reference the flagd resource in your app host project using `WithReference`.

The connection string format is:

```plaintext
http://localhost:<port>
```

Where `<port>` is the port assigned to the flagd HTTP endpoint.

## See also

- [flagd documentation](https://flagd.dev)
- [OpenFeature documentation](https://openfeature.dev)
- [Aspire integrations](../fundamentals/integrations-overview.md)
- [Aspire GitHub repo](https://github.com/dotnet/aspire)
