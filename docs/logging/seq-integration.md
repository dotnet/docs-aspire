---
title: .NET Aspire Seq integration
description: Learn how to use the .NET Aspire Seq integration to add OpenTelemetry Protocol (OTLP) exporters that send logs and traces to a Seq Server.
ms.topic: how-to
ms.date: 08/12/2024
---

# .NET Aspire Seq integration

In this article, you learn how to use the .NET Aspire Seq integration to add OpenTelemetry Protocol (OTLP) exporters that send logs and traces to a Seq Server. The integration supports persistent logs and traces across application restarts via configuration.

## Get started

To get started with the .NET Aspire Seq integration, install the [Aspire.Seq](https://www.nuget.org/packages/Aspire.Seq) NuGet package in the client-consuming project, i.e., the project for the application that uses the Seq client.

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

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

<!-- TODO: <xref:Microsoft.Extensions.Hosting.AspireSeqExtensions.AddSeqEndpoint%2A> -->

In the _:::no-loc text="Program.cs":::_ file of your projects, call the `AddSeqEndpoint` extension method to register OpenTelemetry Protocol exporters to send logs and traces to Seq and the .NET Aspire Dashboard. The method takes a connection name parameter.

```csharp
builder.AddSeqEndpoint("seq");
```

## App host usage

To model the Seq resource in the app host, install the [Aspire.Hosting.Seq](https://www.nuget.org/packages/Aspire.Hosting.Seq) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

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

In your app host project, register a Seq database and consume the connection using the following methods:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var seq = builder.AddSeq("seq")
                 .ExcludeFromManifest();

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(seq);
```

The preceding code registers a Seq server and propagates its configuration.

> [!IMPORTANT]
> You must accept the [Seq End User Licence Agreement](https://datalust.co/doc/eula-current.pdf) for Seq to start):

In the _:::no-loc text="Program.cs":::_ file of the **MyService** project, configure logging and tracing to Seq using the following code:

```csharp
builder.AddSeqEndpoint("seq");
```

### Seq in the .NET Aspire manifest

Seq shouldn't be part of the .NET Aspire deployment manifest, hence the chained call to `ExcludeFromManifest`. It's recommended you set up a secure production Seq server outside of .NET Aspire.

### Persistent logs and traces

Register Seq with a data directory in your AppHost project to retain Seq's data and configuration across application restarts.

```csharp
var seq = builder.AddSeq("seq", seqDataDirectory: "./seqdata")
                 .ExcludeFromManifest();
```

The directory specified must already exist.

## Configuration

The .NET Aspire Seq integration provides options to configure the connection to Seq.

### Use configuration providers

The .NET Aspire Seq integration supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the `SeqSettings` from configuration by using the `Aspire:Seq` key. Example _:::no-loc text="appsettings.json":::_ that configures some of the options:

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

[!INCLUDE [integration-health-checks](../includes/integration-health-checks.md)]

The .NET Aspire Seq integration handles the following:

- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic.

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

### Logging

The .NET Aspire Seq integration uses the following log categories:

- `Seq`

## See also

- [SEQ Query Language](https://docs.datalust.co/docs/the-seq-query-language)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
