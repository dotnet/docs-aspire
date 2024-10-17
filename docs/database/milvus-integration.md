---
title: .NET Aspire Milvus database integration
description: This article describes the .NET Aspire Milvus database integration.
ms.topic: how-to
ms.date: 08/22/2024
---

# .NET Aspire Milvus database integration

In this article, you learn how to use the .NET Aspire Milvus database integration. The `Aspire.Milvus.Client` library registers a [MilvusClient](https://github.com/milvus-io/milvus-sdk-csharp) in the DI container for connecting to a Milvus server.

## Prerequisites

- Milvus server and connection string for accessing the server API endpoint.

## Get started

To get started with the .NET Aspire Milvus database integration, install the [Aspire.Milvus.Client](https://www.nuget.org/packages/Aspire.Milvus.Client) NuGet package in the client-consuming project, i.e., the project for the application that uses the Milvus database client.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Milvus.Client
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Milvus.Client"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _Program.cs_ file of your project, call the `AddMilvusClient` extension method to register a `MilvusClient` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddMilvusClient("milvus");
```

## App host usage

To model the Milvus resource in the app host, install the [Aspire.Hosting.Milvus](https://www.nuget.org/packages/Aspire.Hosting.Milvus) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Milvus
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Milvus"
                  Version="*" />
```

---

In the _Program.cs_ file of `AppHost`, register a Milvus server and consume the connection using the following methods:

```csharp
var milvus = builder.AddMilvus("milvus");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(milvus);
```

The `WithReference` method configures a connection in the `MyService` project named `milvus`. In the _Program.cs_ file of `MyService`, the Milvus connection can be consumed using:

```csharp
builder.AddMilvusClient("milvus");
```

Milvus supports configuration-based (environment variable `COMMON_SECURITY_DEFAULTROOTPASSWORD`) default passwords. The default user is `root` and the default password is `Milvus`. To change the default password in the container, pass an `apiKey` parameter when calling the `AddMilvus` hosting API:

```csharp
var apiKey = builder.AddParameter("apiKey");

var milvus = builder.AddMilvus("milvus", apiKey);

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(milvus);
```

The preceding code gets a parameter to pass to the `AddMilvus` API, and internally assigns the parameter to the `COMMON_SECURITY_DEFAULTROOTPASSWORD` environment variable of the Milvus container. The `apiKey` parameter is usually specified as a _user secret_:

```json
{
  "Parameters": {
    "apiKey": "Non-default P@ssw0rd"
  }
}
```

For more information, see [External parameters](../fundamentals/external-parameters.md).

## Configuration

The .NET Aspire Milvus Client integration provides multiple options to configure the server connection based on the requirements and conventions of your project.

> [!TIP]
> The default use is `root` and the default password is `Milvus`. Currently, Milvus doesn't support changing the superuser password at startup. It needs to be manually changed with the client.

### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddMilvusClient()`:

```csharp
builder.AddMilvusClient("milvus");
```

And then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "milvus": "Endpoint=http://localhost:19530/;Key=root:123456!@#$%"
  }
}
```

By default the `MilvusClient` uses the gRPC API endpoint.

### Use configuration providers

The .NET Aspire Milvus Client integration supports [Microsoft.Extensions.Configuration](/dotnet/api/microsoft.extensions.configuration). It loads the `MilvusSettings` from configuration by using the `Aspire:Milvus:Client` key. Consider the following example _appsettings.json_ that configures some of the options:

```json
{
  "Aspire": {
    "Milvus": {
      "Client": {
        "Key": "root:123456!@#$%"
      }
    }
  }
}
```

### Use inline delegates

Also you can pass the `Action<MilvusSettings> configureSettings` delegate to set up some or all the options inline, for example to set the API key from code:

```csharp
builder.AddMilvusClient(
    "milvus",
    settings => settings.Key = "root:12345!@#$%");
```

[!INCLUDE [integration-health-checks](../includes/integration-health-checks.md)]

The .NET Aspire Milvus database integration uses the configured client to perform a `HealthAsync`. If the result _is healthy_, the health check is considered healthy, otherwise it's unhealthy. Likewise, if there's an exception, the health check is considered unhealthy with the error propagating through the health check failure.

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

### Logging

The .NET Aspire Milvus database integration uses standard .NET logging, and you'll see log entries from the following category:

- `Milvus.Client`

## See also

- [Milvus .NET SDK](https://github.com/milvus-io/milvus-sdk-csharp)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)

<!--
https://github.com/dotnet/docs-aspire/issues/1039

We added a new Aspire.Milvus.Client integration and Aspire.Hosting.Milvus hosting library in main. See:

Add Milvus Aspire Component aspire#796
Adds Milvus to the Aspire hosting/integration packages aspire#4179
https://github.com/dotnet/aspire/tree/main/src/Components/Aspire.Milvus.Client

Include links to:
- https://milvus.io/
- https://github.com/milvus-io/milvus
-->