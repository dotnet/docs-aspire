---
title: .NET Aspire Seq integration
description: Learn how to use the .NET Aspire Seq integration to add OpenTelemetry Protocol (OTLP) exporters that send logs and traces to a Seq Server.
ms.date: 09/30/2025
uid: logging/seq-integration
---

# .NET Aspire Seq integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[Seq](https://datalust.co/seq) is a self-hosted search and analysis server that handles structured application logs and trace files. It includes a JSON event store and a simple query language that make it easy to use. You can use the .NET Aspire Seq integration to send OpenTelemetry Protocol (OTLP) data to Seq. The integration supports persistent logs and traces across application restarts.

During development, .NET Aspire runs and connects to the [`datalust/seq` container image](https://hub.docker.com/r/datalust/seq).

## Hosting integration

The Seq hosting integration models the server as the <xref:Aspire.Hosting.ApplicationModel.SeqResource> type. To access this type and the API, add the [📦 Aspire.Hosting.Seq](https://www.nuget.org/packages/Aspire.Hosting.Seq) NuGet package in the [AppHost](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Seq
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Seq"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add a Seq resource

In your AppHost project, call <xref:Aspire.Hosting.SeqBuilderExtensions.AddSeq*> to add and return a Seq resource builder.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var seq = builder.AddSeq("seq")
                 .ExcludeFromManifest()
                 .WithLifetime(ContainerLifetime.Persistent)
                 .WithEnvironment("ACCEPT_EULA", "Y");

var myService = builder.AddProject<Projects.ExampleProject>()
                       .WithReference(seq)
                       .WaitFor(seq);

// After adding all resources, run the app...
```

> [!NOTE]
> The Seq container may be slow to start, so it's best to use a _persistent_ lifetime to avoid unnecessary restarts. For more information, see [Container resource lifetime](../fundamentals/orchestrate-resources.md#container-resource-lifetime).

#### Accept the Seq End User License Agreement (EULA)

You must accept the [Seq EULA](https://datalust.co/doc/eula-current.pdf) for Seq to start. To accept the agreement in code, pass the environment variable `ACCEPT_EULA` to the Seq container, and set its value to `Y`. The above code passes this variable in the chained call to <xref:Aspire.Hosting.ResourceBuilderExtensions.WithEnvironment*>.

#### Seq in the .NET Aspire manifest

Seq shouldn't be part of the .NET Aspire [deployment manifest](../deployment/manifest-format.md), hence the chained call to  <xref:Aspire.Hosting.ResourceBuilderExtensions.ExcludeFromManifest*>. It's recommended you set up a secure production Seq server outside of .NET Aspire for your production environment.

### Persistent logs and traces

Register Seq with a data directory in your AppHost project to retain Seq's data and configuration across application restarts:

```csharp
var seq = builder.AddSeq("seq", seqDataDirectory: "./seqdata")
                 .ExcludeFromManifest()
                 .WithLifetime(ContainerLifetime.Persistent);
```

The directory specified must already exist.

### Add a Seq resource with a data volume

To add a data volume to the Seq resource, call the <xref:Aspire.Hosting.SeqBuilderExtensions.WithDataVolume*> method on the Seq resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var seq = builder.AddSeq("seq")
                 .WithDataVolume()
                 .ExcludeFromManifest()
                 .WithLifetime(ContainerLifetime.Persistent);

var myService = builder.AddProject<Projects.ExampleProject>()
                       .WithReference(seq)
                       .WaitFor(seq);
```

The data volume is used to persist the Seq data outside the lifecycle of its container. The data volume is mounted at the `/data` path in the Seq container and when a `name` parameter isn't provided, the name is generated at random. For more information on data volumes and details on why they're preferred over [bind mounts](#add-seq-resource-with-data-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

### Add Seq resource with data bind mount

To add a data bind mount to the Seq resource, call the <xref:Aspire.Hosting.SeqBuilderExtensions.WithDataBindMount*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var seq = builder.AddSeq("seq")
                 .WithDataBindMount(source: @"C:\Data")
                 .ExcludeFromManifest()
                 .WithLifetime(ContainerLifetime.Persistent);

var myService = builder.AddProject<Projects.ExampleProject>()
                       .WithReference(seq)
                       .WaitFor(seq);

```

[!INCLUDE [data-bind-mount-vs-volumes](../includes/data-bind-mount-vs-volumes.md)]

Data bind mounts rely on the host machine's filesystem to persist the Seq data across container restarts. The data bind mount is mounted at the `C:\Data` on Windows (or `/Data` on Unix) path on the host machine in the Seq container. For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

## Client integration

To get started with the .NET Aspire Seq client integration, install the [📦 Aspire.Seq](https://www.nuget.org/packages/Aspire.Seq) NuGet package in the client-consuming project, that is, the project for the application that uses the Seq client.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Seq
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Seq"
                  Version="*" />
```

---

### Add a Seq client

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireSeqExtensions.AddSeqEndpoint*> extension method to register OpenTelemetry Protocol exporters to send logs and traces to Seq and the .NET Aspire Dashboard. The method takes a connection name parameter.

```csharp
builder.AddSeqEndpoint(connectionName: "seq");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the Seq resource in the AppHost project. In other words, when you call `AddSeq` and provide a name of `seq` that same name should be used when calling `AddSeqEndpoint`. For more information, see [Add a Seq resource](#add-a-seq-resource).

### Configuration

The .NET Aspire Seq integration provides multiple options to configure the connection to Seq based on the requirements and conventions of your project.

#### Use configuration providers

The .NET Aspire Seq integration supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the <xref:Aspire.Seq.SeqSettings> from configuration by using the `Aspire:Seq` key. The following snippet is an example of an _:::no-loc text="appsettings.json":::_ file that configures some of the options:

```json
{
  "Aspire": {
    "Seq": {
      "DisableHealthChecks": true,
      "ServerUrl": "http://localhost:5341"
    }
  }
}
```

For the complete Seq client integration JSON schema, see [Aspire.Seq/ConfigurationSchema.json](https://github.com/dotnet/aspire/blob/v9.1.0/src/Components/Aspire.Seq/ConfigurationSchema.json).

#### Use named configuration

The .NET Aspire Seq integration supports named configuration, which allows you to configure multiple instances of the same resource type with different settings. The named configuration uses the connection name as a key under the main configuration section.

```json
{
  "Aspire": {
    "Seq": {
      "seq1": {
        "ServerUrl": "http://seq1:5341",
        "DisableHealthChecks": true
      },
      "seq2": {
        "ServerUrl": "http://seq2:5341",
        "DisableHealthChecks": false
      }
    }
  }
}
```

In this example, the `seq1` and `seq2` connection names can be used when calling `AddSeqEndpoint`:

```csharp
builder.AddSeqEndpoint("seq1");
builder.AddSeqEndpoint("seq2");
```

Named configuration takes precedence over the top-level configuration. If both are provided, the settings from the named configuration override the top-level settings.

#### Use inline delegates

Also you can pass the `Action<SeqSettings> configureSettings` delegate to set up some or all the options inline, for example to disable health checks from code:

```csharp
builder.AddSeqEndpoint("seq", static settings => 
{
    settings.DisableHealthChecks  = true;
    settings.ServerUrl = "http://localhost:5341"
});
```

[!INCLUDE [client-integration-health-checks](../includes/client-integration-health-checks.md)]

The .NET Aspire Seq integration handles the following:

- Adds the health check when <xref:Aspire.Seq.SeqSettings.DisableHealthChecks?displayProperty=nameWithType> is `false`, which attempts to connect to the Seq server's `/health` endpoint.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic.

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

#### Logging

The .NET Aspire Seq integration uses the following log categories:

- `Seq`

#### Tracing and Metrics

The .NET Aspire Seq integration doesn't emit tracing activities and or metrics because it's a telemetry sink, not a telemetry source.

## See also

- [Seq](https://datalust.co/)
- [Seq Query Language](https://docs.datalust.co/docs/the-seq-query-language)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
