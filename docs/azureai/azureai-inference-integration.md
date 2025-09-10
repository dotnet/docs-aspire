---
title: .NET Aspire Azure AI Inference integration (Preview)
description: Learn how to use the .NET Aspire Azure AI Inference integration to deploy and manage machine learning models in the cloud.
ms.date: 05/14/2025
titleSuffix: ''
---

# .NET Aspire Azure AI Inference integration (Preview)

[!INCLUDE [includes-client](../includes/includes-client.md)]

The .NET Aspire Azure AI Inference integration provides a seamless way to deploy and manage machine learning models in the cloud. This integration allows you to leverage the power of Azure's AI services while maintaining the flexibility and ease of use of the .NET Aspire.

## Hosting integration

Although the Azure AI Inference library doesn't currently offer direct hosting integration, you can still integrate it into your AppHost project. Simply add a connection string to establish a reference to an existing Azure AI Foundry resource.

### Connect to an existing Azure AI Foundry service

If you already have an [Azure AI Foundry](https://ai.azure.com/) service, you can easily connect to it by adding a connection string to your AppHost. This approach uses a simple, string-based configuration. To establish the connection, use the <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString%2A> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var aiFoundry = builder.AddConnectionString("ai-foundry");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(aiFoundry);

// After adding all resources, run the app...
```

[!INCLUDE [connection-strings-alert](../includes/connection-strings-alert.md)]

The connection string is configured in the AppHost's configuration, typically under User Secrets, under the `ConnectionStrings` section:

```json
{
  "ConnectionStrings": {
    "ai-foundry": "Endpoint=https://{endpoint}/;DeploymentId={deploymentName}"
  }
}
```

For more information, see [Add existing Azure resources with connection strings](../azure/integrations-overview.md#add-existing-azure-resources-with-connection-strings).

## Client integration

To get started with the .NET Aspire Azure AI Inference client integration, install the [📦 Aspire.Azure.AI.Inference](https://www.nuget.org/packages/Aspire.Azure.AI.Inference) NuGet package in the client-consuming project, that is, the project for the application that uses the Azure AI Inference client.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Azure.AI.Inference
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Azure.AI.Inference"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add an Azure AI Inference client

In the _Program.cs_ file of your client-consuming project, use the `AddAzureChatCompletionsClient` method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register an <xref:Azure.AI.Inference.ChatCompletionsClient> for dependency injection (DI).

```csharp
builder.AddAzureChatCompletionsClient(connectionName: "ai-foundry");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the Azure AI Inference resource in the AppHost project. For more information, see [Connect to an existing Azure AI Foundry service](#connect-to-an-existing-azure-ai-foundry-service).

After adding the `ChatCompletionsClient`, you can retrieve the client instance using dependency injection:

```csharp
public class ExampleService(ChatCompletionsClient client)
{
    // Use client...
}
```

For more information, see:

- [What is Azure AI model inference?](/azure/ai-foundry/model-inference/overview) for details on Azure AI model interfence.
- [Dependency injection in .NET](/dotnet/core/extensions/dependency-injection) for details on dependency injection.
- [The Azure AI Foundry SDK: C#](/azure/ai-foundry/how-to/develop/sdk-overview?tabs=sync&pivots=programming-language-csharp).

### Add Azure AI Inference client with registered `IChatClient`

If you're interested in using the <xref:Microsoft.Extensions.AI.IChatClient> interface with the Azure AI Inference client, simply chain either of the following APIs to the `AddAzureChatCompletionsClient` method:

- `AddChatClient`: Registers a singleton `IChatClient` in the services.
- `AddKeyedChatClient`: Registers a keyed singleton `IChatClient` in the services.

For example, consider the following C# code that adds an `IChatClient` to the DI container:

```csharp
builder.AddAzureChatCompletionsClient(connectionName: "ai-foundry")
       .AddChatClient("deploymentName");
```

Similarly, you can add a keyed `IChatClient` with the following C# code:

```csharp
builder.AddAzureChatCompletionsClient(connectionName: "ai-foundry")
       .AddKeyedChatClient("serviceKey", "deploymentName");
```

After adding the `IChatClient`, you can retrieve the client instance using dependency injection:

```csharp
public class ExampleService(IChatClient chatClient)
{
    public async Task<string> GetResponseAsync(string userMessage)
    {
        var response = await chatClient.CompleteAsync(userMessage);
        return response.Message.Text ?? string.Empty;
    }
}
```

For more information on the `IChatClient` and its corresponding library, see [Artificial intelligence in .NET (Preview)](/dotnet/core/extensions/artificial-intelligence).

### Add keyed Azure AI Inference clients

There might be situations where you want to register multiple `ChatCompletionsClient` instances with different connection names. To register keyed Azure AI Inference clients, call the `AddKeyedAzureChatCompletionsClient` method:

```csharp
builder.AddKeyedAzureChatCompletionsClient(name: "chat");
builder.AddKeyedAzureChatCompletionsClient(name: "code");
```

> [!IMPORTANT]
> When using keyed services, ensure that your Azure AI Inference resource configures two named connections, one for `chat` and one for `code`.

Then you can retrieve the client instances using dependency injection. For example, to retrieve the clients from a service:

```csharp
public class ExampleService(
    [KeyedService("chat")] ChatCompletionsClient chatClient,
    [KeyedService("code")] ChatCompletionsClient codeClient)
{
    // Use clients...
}
```

For more information, see [Keyed services in .NET](/dotnet/core/extensions/dependency-injection#keyed-services).

### Configuration

The .NET Aspire Azure AI Inference library provides multiple options to configure the Azure AI Foundry Service based on the requirements and conventions of your project.

> [!NOTE]
> Either an `Endpoint` and `DeploymentId`, or a `ConnectionString` is required to be supplied.

#### Use a connection string

A connection can be constructed from the `Keys`, `Deployment ID` and `Endpoint` tab with the format:

```Plaintext
Endpoint={endpoint};Key={key};DeploymentId={deploymentId}`
```

You can provide the name of the connection string when calling `builder.AddAzureChatCompletionsClient()`:

```csharp
builder.AddAzureChatCompletionsClient(
    connectionName: "connection-string-name");
```

The connection string is retrieved from the `ConnectionStrings` configuration section. Two connection formats are supported:

##### Azure AI Foundry endpoint

The recommended approach is to use an `Endpoint`, which works with the `ChatCompletionsClientSettings.Credential` property to establish a connection. If no credential is configured, <xref:Azure.Identity.DefaultAzureCredential> is used.

```json
{
  "ConnectionStrings": {
    "connection-string-name": "Endpoint=https://{endpoint}/;DeploymentId={deploymentName}"
  }
}
```

##### Connection string

Alternatively, a custom connection string can be used.

```json
{
  "ConnectionStrings": {
    "connection-string-name": "Endpoint=https://{endpoint}/;Key={account_key};DeploymentId={deploymentName}"
  }
}
```

#### Use configuration providers

The .NET Aspire Azure AI Inference library supports <xref:Microsoft.Extensions.Configuration>. It loads the `ChatCompletionsClientSettings` and `AzureAIInferenceClientOptions` from configuration by using the `Aspire:Azure:AI:Inference` key. For example, consider an _appsettings.json_ that configures some of the options:

```json
{
  "Aspire": {
    "Azure": {
      "AI": {
        "Inference": {
          "DisableTracing": false,
          "EnableSensitiveTelemetryData": false,
          "ClientOptions": {
            "UserAgentApplicationId": "myapp"
          }
        }
      }
    }
  }
}
```

#### Use inline delegates

You can also pass the `Action<ChatCompletionsClientSettings> configureSettings` delegate to set up some or all the options inline, for example, to disable tracing from code:

```csharp
builder.AddAzureChatCompletionsClient(
    connectionName: "connection-string-name",
    static settings => settings.DisableTracing = true);
```

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

### Logging

The .NET Aspire Azure AI Inference integration uses the following log categories:

- `Azure.Core`
- `Azure.Identity`

### Tracing

The .NET Aspire Azure AI Inference integration will emit the following tracing activities using OpenTelemetry:

- `Experimental.Microsoft.Extensions.AI` - Used by Microsoft.Extensions.AI to record AI operations

> [!IMPORTANT]
> Telemetry is only recorded by default when using the `IChatClient` interface from Microsoft.Extensions.AI. Raw `ChatCompletionsClient` calls do not automatically generate telemetry.

#### Configuring sensitive data in telemetry

By default, telemetry includes metadata such as token counts, but not raw inputs and outputs like message content. To include potentially sensitive information in telemetry, set the `EnableSensitiveTelemetryData` configuration option:

```csharp
builder.AddAzureChatCompletionsClient(
    connectionName: "ai-foundry",
    configureSettings: settings =>
    {
        settings.EnableSensitiveTelemetryData = true;
    })
    .AddChatClient("deploymentName");
```

Or through configuration:

```json
{
  "Aspire": {
    "Azure": {
      "AI": {
        "Inference": {
          "EnableSensitiveTelemetryData": true
        }
      }
    }
  }
}
```

Alternatively, you can enable sensitive data capture by setting the environment variable:

```bash
OTEL_INSTRUMENTATION_GENAI_CAPTURE_MESSAGE_CONTENT=true
```

#### Using underlying library telemetry

If you need to access telemetry from the underlying Azure AI Inference library directly, you can manually add the appropriate activity sources and meters to your OpenTelemetry configuration:

```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource("Azure.AI.Inference.*"))
    .WithMetrics(metrics => metrics.AddMeter("Azure.AI.Inference.*"));
```

However, you'll need to enable experimental telemetry support in the Azure AI Inference library by setting the `AZURE_EXPERIMENTAL_ENABLE_ACTIVITY_SOURCE` environment variable to `"true"` or calling `AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true)` during app startup.

## See also
