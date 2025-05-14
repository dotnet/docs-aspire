---
title: Azure AI Inference
description: Learn how to use the .NET Aspire Azure AI Inference integration to deploy and manage machine learning models in the cloud.
ms.date: 05/14/2025
titleSuffix: ''
---

# .NET Aspire Azure AI Inference integration (Preview)

[!INCLUDE [includes-client](../includes/includes-client.md)]

The .NET Aspire Azure AI Inference integration provides a seamless way to deploy and manage machine learning models in the cloud. This integration allows you to leverage the power of Azure's AI services while maintaining the flexibility and ease of use of the .NET Aspire.

## Hosting integration

Although the Azure AI Inference library doesn't currently offer direct hosting integration, you can still integrate it into your app host project. Simply add a connection string to establish a reference to an existing Azure AI Foundry resource.

### Connect to an existing Azure AI Foundry service

If you already have an Azure AI Foundry service, you can easily connect to it by adding a connection string to your app host. This approach uses a simple, string-based configuration. To establish the connection, use the <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString%2A> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var aiFoundry = builder.AddConnectionString("ai-foundry");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(aiFoundry);

// After adding all resources, run the app...
```

[!INCLUDE [connection-strings-alert](../includes/connection-strings-alert.md)]

The connection string is configured in the app host's configuration, typically under User Secrets, under the `ConnectionStrings` section:

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

In the _Program.cs_ file of your client-consuming project, use the `AddChatCompletionsClient` method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register an <xref:Azure.AI.Inference.ChatCompletionsClient> for dependency injection (DI).

```csharp
builder.AddChatCompletionsClient(connectionName: "ai-foundry");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the Azure AI Inference resource in the app host project. For more information, see [Connect to an existing Azure AI Foundry service](#connect-to-an-existing-azure-ai-foundry-service).

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

You can provide the name of the connection string when calling `builder.AddChatCompletionsClient()`:

```csharp
builder.AddChatCompletionsClient(
    connectionName: "connection-string-name");
```

The connection string is retrieved from the `ConnectionStrings` configuration section. Two connection formats are supported:

##### Azure AI Foundry endpoint

The recommended approach is to use an `Endpoint`, which works with the `ChatCompletionsClientSettings.Credential` property to establish a connection. If no credential is configured, the <xref:Azure.Identity.DefaultAzureCredential> is used.

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

You can also pass the `Action<ChatCompletionsClientSettings> configureSettings` delegate to set up some or all the options inline, for example to disable tracing from code:

```csharp
builder.AddChatCompletionsClient(
    connectionName: "connection-string-name",
    static settings => settings.DisableTracing = true);
```

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

### Logging

The .NET Aspire Azure AI Inference integration uses the following log categories:

- `Azure.Core`
- `Azure.Identity`

### Tracing

The .NET Aspire Azure AI Inference integration emits tracing activities using OpenTelemetry for operations performed with the `OpenAIClient`.

> [!IMPORTANT]
> Azure AI Inference telemetry support is experimental, and the shape of traces may change in the future without notice. It can be enabled by invoking:
>
> ```csharp
> AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);
> ```
>
> Alternatively, you can set the `AZURE_EXPERIMENTAL_ENABLE_ACTIVITY_SOURCE` environment variable to `"true"`.

## See also
