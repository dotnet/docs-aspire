---
title: .NET Aspire Seq component
description: Learn how to use the .NET Aspire Seq component to add OpenTelemetry Protocol (OTLP) exporters that send logs and traces to a Seq Server.
ms.topic: how-to
ms.date: 04/09/2024
---

# .NET Aspire Seq component

In this article, you learn how to use the .NET Aspire Seq component to add OpenTelemetry Protocol (OTLP) exporters that send logs and traces to a Seq Server. The component supports persistent logs and traces across application restarts via configuration.

## Get started

To get started with the .NET Aspire Seq component, install the [Aspire.Seq](https://www.nuget.org/packages/Aspire.Seq) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Seq --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Seq"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

<!-- TODO: <xref:Microsoft.Extensions.Hosting.AspireSeqExtensions.AddSeqEndpoint%2A> -->

In the _Program.cs_ file of your projects, call the `AddSeqEndpoint` extension method to register OpenTelemetry Protocol exporters to send logs and traces to Seq and the .NET Aspire Dashboard. The method takes a connection name parameter.

```csharp
builder.AddSeqEndpoint("seq");
```

## Configuration

The .NET Aspire Seq component provides options to configure the connection to Seq.

### Use configuration providers

The .NET Aspire Seq component supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the `SeqSettings` from configuration by using the `Aspire:Seq` key. Example `appsettings.json` that configures some of the options:

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

### Use inline delegates

You can pass the `Action<SeqSettings> configureSettings` delegate to set up some or all the options inline, for example to disable health checks from code:

```csharp
builder.AddSeqEndpoint("seq", static settings => 
{
    settings.DisableHealthChecks  = true;
    settings.ServerUrl = "http://localhost:5341"
});
```

## AppHost extensions

In your AppHost project, install the `Aspire.Hosting.Seq` library with [NuGet](https://www.nuget.org):

```dotnetcli
dotnet add package Aspire.Hosting.Seq
```

Then, in the _Program.cs_ file of the **.AppHost** project, register a Seq server and propagate its configuration using the following methods (note that you must accept the [Seq End User Licence Agreement](https://datalust.co/doc/eula-current.pdf) for Seq to start):

```csharp
var seq = builder.AddSeq("seq");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(seq);
```

In the _Program.cs_ file of the **MyService** project, configure logging and tracing to Seq using the following code:

```csharp
builder.AddSeqEndpoint("seq");
```

### Persistent logs and traces

Register Seq with a data directory in your AppHost project to retain Seq's data and configuration across application restarts.

```csharp
var seq = builder.AddSeq("seq", seqDataDirectory: "./seqdata");
```

The directory specified must already exist.

### Seq in the .NET Aspire manifest

Seq isn't part of the .NET Aspire deployment manifest. It's recommended you set up a secure production Seq server outside of .NET Aspire.

[!INCLUDE [component-health-checks](../includes/component-health-checks.md)]

The .NET Aspire Seq component handles the following:

- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic.

[!INCLUDE [component-observability-and-telemetry](../includes/component-observability-and-telemetry.md)]

### Logging

The .NET Aspire Seq component uses the following log categories:

- `Seq`

## See also

- [.NET Aspire Seq README](https://github.com/dotnet/aspire/tree/main/src/Components/README.md)
- [SEQ Query Language](https://docs.datalust.co/docs/the-seq-query-language)
