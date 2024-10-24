---
title: .NET Aspire Azure Web PubSub integration
description: This article describes the .NET Aspire Azure Web PubSub integration features and capabilities.
ms.topic: how-to
ms.date: 08/12/2024
---

# .NET Aspire Azure Web PubSub integration

In this article, you learn how to use the .NET Aspire Azure Web PubSub integration. The `Aspire.Azure.Messaging.WebPubSub` library offers options for registering an <xref:Azure.Messaging.WebPubSub.WebPubSubServiceClient> in the DI container for connecting to [Azure Web PubSub](/azure/azure-web-pubsub).

## Prerequisites

- Azure subscription: [create one for free](https://azure.microsoft.com/free/).
- An existing Azure Web PubSub service instance. For more information, see [Create a Web PubSub resource](/azure/azure-web-pubsub/howto-develop-create-instance). Alternatively, you can use a connection string, which isn't recommended in production environments.

## Get started

To get started with the .NET Aspire Azure Web PubSub integration, install the [Aspire.Azure.Messaging.WebPubSub](https://www.nuget.org/packages/Aspire.Azure.Messaging.WebPubSub) NuGet package in the client-consuming project, i.e., the project for the application that uses the Azure Web PubSub client.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Azure.Messaging.WebPubSub
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Azure.Messaging.WebPubSub"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _Program.cs_ file of your project, call the `AddAzureWebPubSubHub` extension method to register a `WebPubSubServiceClient` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddAzureWebPubSubServiceClient("wps");
```

You can then retrieve the `WebPubSubServiceClient` instance using dependency injection. For example, to retrieve the client from a service:

```csharp
public class ExampleService(WebPubSubServiceClient client)
{
    // Use client...
}
```

For more information, see the [Azure.Messaging.WebPubSub documentation](https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/webpubsub/Azure.Messaging.WebPubSub/README.md).

## App host usage

To add Azure Web PubSub hosting support to your <xref:Aspire.Hosting.IDistributedApplicationBuilder>, install the [Aspire.Hosting.Azure.WebPubSub](https://www.nuget.org/packages/Aspire.Hosting.Azure.WebPubSub) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.WebPubSub
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.WebPubSub"
                  Version="*" />
```

---

In your app host project, add a Web PubSub connection and consume the connection using the following methods:

```csharp
var webPubSub = builder.AddAzureWebPubSub("wps");

var exampleService = builder.AddProject<Projects.ExampleService>()
                            .WithReference(webPubSub);
```

The `AddAzureWebPubSubHub` method reads connection information from the app host's configuration (for example, from "user secrets") under the `ConnectionStrings:wps` configuration key. The `WithReference` method passes that connection information into a connection string named `wps` in the `ExampleService` project. In the _Program.cs_ file of `ExampleService`, the connection can be consumed using:

```csharp
builder.AddAzureWebPubSubServiceClient("wps");
```

## Configuration

The .NET Aspire Azure Web PubSub library provides multiple options to configure the Azure Web PubSub connection based on the requirements and conventions of your project. Note that either a `Endpoint` or a `ConnectionString` is a required to be supplied.

### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddAzureWebPubSubHub()`:

```csharp
builder.AddAzureWebPubSubServiceClient(
    "WebPubSubConnectionName",
    "your_hub_name");
```

And then the connection information will be retrieved from the `ConnectionStrings` configuration section. Two connection formats are supported:

#### Use the service endpoint

The recommended approach is to use the service endpoint, which works with the `AzureMessagingWebPubSubSettings.Credential` property to establish a connection. If no credential is configured, the [DefaultAzureCredential](/dotnet/api/azure.identity.defaultazurecredential) is used.

```json
{
  "ConnectionStrings": {
    "WebPubSubConnectionName": "https://xxx.webpubsub.azure.com"
  }
}
```

#### Connection string

Alternatively, a connection string can be used.

```json
{
  "ConnectionStrings": {
    "WebPubSubConnectionName": "Endpoint=https://xxx.webpubsub.azure.com;AccessKey==xxxxxxx"
  }
}
```

### Use configuration providers

The .NET Aspire Azure Web PubSub library supports [Microsoft.Extensions.Configuration](/dotnet/api/microsoft.extensions.configuration). It loads the `AzureMessagingWebPubSubSettings` and `WebPubSubServiceClientOptions` from configuration by using the `Aspire:Azure:Messaging:WebPubSub` key. Consider the example _appsettings.json_ that configures some of the options:

```json
{
  "Aspire": {
    "Azure": {
      "Messaging": {
        "WebPubSub": {
          "DisableHealthChecks": true,
          "HubName": "your_hub_name"
        }
      }
    }
  }
}
```

### Use inline delegates

You can also pass the `Action<AzureMessagingWebPubSubSettings> configureSettings` delegate to set up some or all the options inline, for example to disable health checks from code:

```csharp
builder.AddAzureWebPubSubServiceClient(
    "wps",
    settings => settings.DisableHealthChecks = true);
```

You can also setup the <xref:Azure.Messaging.WebPubSub.WebPubSubServiceClientOptions> using the optional `Action<IAzureClientBuilder<WebPubSubServiceClient, WebPubSubServiceClientOptions>> configureClientBuilder` parameter of the `AddAzureWebPubSubHub` method. For example, to set the client ID for this client:

```csharp
builder.AddAzureWebPubSubServiceClient(
    "wps",
    configureClientBuilder: clientBuilder => 
        clientBuilder.ConfigureOptions(options => options.Retry.MaxRetries = 5));
```

[!INCLUDE [integration-health-checks](../includes/integration-health-checks.md)]

The .NET Aspire Azure Web PubSub integration handles exposes a configurable health check that reports as _healthy_, when the client can successfully connect to the Azure Web PubSub service.

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

### Logging

The .NET Aspire Azure Web PubSub integration uses the following log categories:

- `Azure`
- `Azure.Core`
- `Azure.Identity`
- `Azure.Messaging.WebPubSub`

### Tracing

The .NET Aspire Azure Web PubSub integration will emit the following tracing activities using OpenTelemetry:

- "Azure.Messaging.WebPubSub.*"

### Metrics

The .NET Aspire Azure Web PubSub integration currently doesn't support metrics by default due to limitations with the Azure SDK for .NET. If that changes in the future, this section will be updated to reflect those changes.

## See also

- [Azure Web PubSub](/azure/azure-web-pubsub/)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
