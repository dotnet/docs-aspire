---
title: .NET Aspire Qdrant component
description: This article describes the .NET Aspire Qdrant component.
ms.topic: how-to
ms.date: 05/22/2024
---

# .NET Aspire Qdrant component

In this article, you learn how to use the .NET Aspire Qdrant component. Use this component to register a [QdrantClient](https://github.com/qdrant/qdrant-dotnet) in the DI container for connecting to a Qdrant server.

## Get started

To get started with the .NET Aspire Qdrant component, install the [Aspire.Qdrant.Client](https://www.nuget.org/packages/Aspire.Qdrant.Client) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Qdrant.Client
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Qdrant.Client"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _:::no-loc text="Program.cs":::_ file of your component-consuming project, call the `AddQdrantClient` extension method to register a `QdrantClient` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddQdrantClient("qdrant");
```

To retrieve your `QdrantClient` object consider the following example service:

```csharp
public class ExampleService(QdrantClient client)
{
    // Use client...
}
```

## App host usage

To model the Qdrant server resource in the app host, install the [Aspire.Hosting.Qdrant](https://www.nuget.org/packages/Aspire.Hosting.Qdrant) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Qdrant
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Qdrant"
                  Version="[SelectVersion]" />
```

---

In your app host project, register a Qdrant server and consume the connection using the following methods:

```csharp
var qdrant = builder.AddQdrant("qdrant");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(qdrant);
```

## Configuration

The .NET Aspire Qdrant Client component provides multiple options to configure the server connection based on the requirements and conventions of your project.

### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddQdrantClient()`:

```csharp
builder.AddQdrantClient("qdrant");
```

And then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "qdrant": "Endpoint=http://localhost:6334;Key=123456!@#$%"
  }
}
```

By default the `QdrantClient` uses the gRPC API endpoint.

### Use configuration providers

The .NET Aspire Qdrant Client component supports [Microsoft.Extensions.Configuration](/dotnet/api/microsoft.extensions.configuration). It loads the `QdrantClientSettings` from configuration by using the `Aspire:Qdrant:Client` key. Example `appsettings.json` that configures some of the options:

```json
{
  "Aspire": {
    "Qdrant": {
      "Client": {
        "Key": "123456!@#$%"
      }
    }
  }
}
```

### Use inline delegates

You can also pass the `Action<QdrantClientSettings> configureSettings` delegate to set up some or all the options inline, for example to set the API key from code:

```csharp
builder.AddQdrantClient("qdrant", settings => settings.ApiKey = "12345!@#$%");
```

[!INCLUDE [component-observability-and-telemetry](../includes/component-observability-and-telemetry.md)]

### Logging

The .NET Aspire Qdrant component uses the following logging categories:

- "Qdrant.Client"

## See also

- [.NET Aspire components](../fundamentals/components-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
