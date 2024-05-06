---
title: .NET Aspire Azure AI OpenAI component
description: Learn how to use the .NET Aspire Azure AI OpenAI component.
ms.topic: how-to
ms.date: 04/24/2024
---

# .NET Aspire Azure AI OpenAI component

In this article, you learn how to use the .NET Aspire Azure AI OpenAI client. The `Aspire.Azure.AI.OpenAI` library is used to register an <xref:Azure.AI.OpenAI.OpenAIClient> in the dependency injection (DI) container for consuming Azure AI OpenAI or OpenAI functionality. It enables corresponding logging and telemetry.

For more information on using the `OpenAIClient`, see [Quickstart: Get started generating text using Azure OpenAI Service](/azure/ai-services/openai/quickstart?tabs=command-line%2Cpython&pivots=programming-language-csharp).

## Get started

- Azure subscription: [create one for free](https://azure.microsoft.com/free/).
- Azure AI OpenAI or OpenAI account: [create an Azure OpenAI Service resource](/azure/ai-services/openai/how-to/create-resource).

To get started with the .NET Aspire Azure AI OpenAI component, install the [Aspire.Azure.AI.OpenAI](https://www.nuget.org/packages/Aspire.Azure.AI.OpenAI) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Azure.AI.OpenAI --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Azure.AI.OpenAI"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _Program.cs_ file of your component-consuming project, call the extension method to register an `OpenAIClient` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddAzureOpenAIClient("openAiConnectionName");
```

In the preceding code, the <xref:Microsoft.Extensions.Hosting.AspireAzureOpenAIExtensions.AddAzureOpenAIClient%2A> method adds an `OpenAIClient` to the DI container. The `openAiConnectionName` parameter is the name of the connection string in the configuration. You can then retrieve the `OpenAIClient` instance using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(OpenAIClient client)
{
    // Use client...
}
```

## App host usage

To add Azure AI hosting support to your <xref:Aspire.Hosting.IDistributedApplicationBuilder>, install the [Aspire.Hosting.Azure.CognitiveServices](https://www.nuget.org/packages/Aspire.Hosting.Azure.CognitiveServices) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.CognitiveServices --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.CognitiveServices"
                  Version="[SelectVersion]" />
```

---

In your app host project, register an Azure AI OpenAI resource using the following methods, such as <xref:Aspire.Hosting.AzureOpenAIExtensions.AddAzureOpenAI%2A>:

```csharp
// Service registration
var openai = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureOpenAI("openAiConnectionName")
    : builder.AddConnectionString("openAiConnectionName");

// Service consumption
builder.AddProject<Projects.ExampleProject>()
       .WithReference(openai);
```

The `AddAzureAIOpenAI` method will read connection information from the app host's configuration (for example, from "user secrets") under the `ConnectionStrings:openAiConnectionName` config key. The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method passes that connection information into a connection string named `openAiConnectionName` in the `ExampleProject` project. In the _Program.cs_ file of ExampleProject, the connection can be consumed using:

```csharp
builder.AddAzureAIOpenAI("openAiConnectionName");
```

## Configuration

The .NET Aspire Azure AI OpenAI component provides multiple options to configure the connection based on the requirements and conventions of your project.

### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddAzureAIOpenAI`:

```csharp
builder.AddAzureAIOpenAI("openAiConnectionName");
```

And then the connection string will be retrieved from the `ConnectionStrings` configuration section:

#### Account endpoint

The recommended approach is to use an `Endpoint`, which works with the `AzureOpenAISettings.Credential` property to establish a connection. If no credential is configured, the <xref:Azure.Identity.DefaultAzureCredential> is used.

```json
{
  "ConnectionStrings": {
    "openAiConnectionName": "https://{account_name}.openai.azure.com/"
  }
}
```

#### Connection string

Alternatively, a custom connection string can be used.

```json
{
  "ConnectionStrings": {
    "openaiConnectionName": "Endpoint=https://{account_name}.openai.azure.com/;Key={account_key};"
  }
}
```

In order to connect to the non-Azure OpenAI service, drop the `Endpoint` property and only set the Key property to set the [API key](https://platform.openai.com/account/api-keys).

### Use configuration providers

The .NET Aspire Azure AI OpenAI component supports <xref:Microsoft.Extensions.Configuration>. It loads the `AzureOpenAISettings` from configuration by using the `Aspire:Azure:AI:OpenAI` key. Example `appsettings.json` that configures some of the options:

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

[!INCLUDE [component-observability-and-telemetry](../includes/component-observability-and-telemetry.md)]

### Logging

The .NET Aspire Azure AI OpenAI component uses the following log categories:

- `Azure`
- `Azure.Core`
- `Azure.Identity`

## See also

- [Azure AI OpenAI docs](/azure/ai-services/openai/overview)
- [.NET Aspire components](../fundamentals/components-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
