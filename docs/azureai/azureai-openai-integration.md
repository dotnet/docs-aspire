---
title: .NET Aspire Azure OpenAI integration
description: Learn how to use the .NET Aspire Azure OpenAI integration.
ms.topic: how-to
ms.date: 09/27/2024
---

# .NET Aspire Azure OpenAI integration

In this article, you learn how to use the .NET Aspire Azure OpenAI client. The `Aspire.Azure.AI.OpenAI` library is used to register an `OpenAIClient` in the dependency injection (DI) container for consuming Azure OpenAI or OpenAI functionality. It enables corresponding logging and telemetry.

For more information on using the `OpenAIClient`, see [Quickstart: Get started generating text using Azure OpenAI Service](/azure/ai-services/openai/quickstart?tabs=command-line%2Cpython&pivots=programming-language-csharp).

## Get started

- Azure subscription: [create one for free](https://azure.microsoft.com/free/).
- Azure OpenAI or OpenAI account: [create an Azure OpenAI Service resource](/azure/ai-services/openai/how-to/create-resource).

To get started with the .NET Aspire Azure OpenAI integration, install the [Aspire.Azure.AI.OpenAI](https://www.nuget.org/packages/Aspire.Azure.AI.OpenAI) NuGet package in the client-consuming project, i.e., the project for the application that uses the Azure OpenAI client.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Azure.AI.OpenAI
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Azure.AI.OpenAI"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the extension method to register an `OpenAIClient` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddAzureOpenAIClient("openAiConnectionName");
```

In the preceding code, the `AddAzureOpenAIClient` method adds an `OpenAIClient` to the DI container. The `openAiConnectionName` parameter is the name of the connection string in the configuration. You can then retrieve the `OpenAIClient` instance using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(OpenAIClient client)
{
    // Use client...
}
```

## App host usage

To add Azure hosting support to your <xref:Aspire.Hosting.IDistributedApplicationBuilder>, install the [Aspire.Hosting.Azure.CognitiveServices](https://www.nuget.org/packages/Aspire.Hosting.Azure.CognitiveServices) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.CognitiveServices
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.CognitiveServices"
                  Version="*" />
```

---

In your app host project, register an Azure OpenAI resource using the following methods, such as <xref:Aspire.Hosting.AzureOpenAIExtensions.AddAzureOpenAI%2A>:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var openai = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureOpenAI("openAiConnectionName")
    : builder.AddConnectionString("openAiConnectionName");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(openai);
```

The `AddAzureAIOpenAI` method will read connection information from the app host's configuration (for example, from "user secrets") under the `ConnectionStrings:openAiConnectionName` config key. The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method passes that connection information into a connection string named `openAiConnectionName` in the `ExampleProject` project. In the _:::no-loc text="Program.cs":::_ file of ExampleProject, the connection can be consumed using:

```csharp
builder.AddAzureAIOpenAI("openAiConnectionName");
```

## Configuration

The .NET Aspire Azure OpenAI integration provides multiple options to configure the connection based on the requirements and conventions of your project.

### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddAzureAIOpenAI`:

```csharp
builder.AddAzureAIOpenAI("openAiConnectionName");
```

The connection string is retrieved from the `ConnectionStrings` configuration section, and there are two supported formats, either the account endpoint used in conjunction with the default Azure credential or a connection string with the account key.

#### Account endpoint

The recommended approach is to use an **Endpoint**, which works with the `AzureOpenAISettings.Credential` property to establish a connection. If no credential is configured, the <xref:Azure.Identity.DefaultAzureCredential> is used.

```json
{
  "ConnectionStrings": {
    "openAiConnectionName": "https://{account_name}.openai.azure.com/"
  }
}
```

For more information, see [Use Azure OpenAI without keys](/azure/developer/ai/keyless-connections).

#### Connection string

Alternatively, a custom connection string can be used.

```json
{
  "ConnectionStrings": {
    "openAiConnectionName": "Endpoint=https://{account_name}.openai.azure.com/;Key={account_key};"
  }
}
```

In order to connect to the non-Azure OpenAI service, drop the `Endpoint` property and only set the Key property to set the [API key](https://platform.openai.com/account/api-keys).

### Use configuration providers

The .NET Aspire Azure OpenAI integration supports <xref:Microsoft.Extensions.Configuration>. It loads the `AzureOpenAISettings` from configuration by using the `Aspire:Azure:AI:OpenAI` key. Example _:::no-loc text="appsettings.json":::_ that configures some of the options:

```json
{
  "Aspire": {
    "Azure": {
      "AI": {
        "OpenAI": {
          "DisableTracing": false,
        }
      }
    }
  }
}
```

### Use inline delegates

Also you can pass the `Action<AzureOpenAISettings> configureSettings` delegate to set up some or all the options inline, for example to disable tracing from code:

```csharp
builder.AddAzureAIOpenAI(
    "openAiConnectionName",
    static settings => settings.DisableTracing = true);
```

You can also setup the OpenAIClientOptions using the optional `Action<IAzureClientBuilder<OpenAIClient, OpenAIClientOptions>> configureClientBuilder` parameter of the `AddAzureAIOpenAI` method. For example, to set the client ID for this client:

```csharp
builder.AddAzureAIOpenAI(
    "openAiConnectionName",
    configureClientBuilder: builder => builder.ConfigureOptions(
        options => options.Diagnostics.ApplicationId = "CLIENT_ID"));
```

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

### Logging

The .NET Aspire Azure OpenAI integration uses the following log categories:

- `Azure`
- `Azure.Core`
- `Azure.Identity`

## See also

- [Azure OpenAI docs](/azure/ai-services/openai/overview)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
