---
title: .NET Aspire Azure OpenAI integration (Preview)
description: Learn how to use the .NET Aspire Azure OpenAI integration.
ms.date: 04/15/2025
---

# .NET Aspire Azure OpenAI integration (Preview)

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[Azure OpenAI Service](https://azure.microsoft.com/products/ai-services/openai-service) provides access to OpenAI's powerful language and embedding models with the security and enterprise promise of Azure. The .NET Aspire Azure OpenAI integration enables you to connect to Azure OpenAI Service or OpenAI's API from your .NET applications.

## Hosting integration

The .NET Aspire [Azure OpenAI](/azure/ai-services/openai/) hosting integration models Azure OpenAI resources as <xref:Aspire.Hosting.ApplicationModel.AzureOpenAIResource>. To access these types and APIs for expressing them within your [app host](xref:dotnet/aspire/app-host) project, install the [📦 Aspire.Hosting.Azure.CognitiveServices](https://www.nuget.org/packages/Aspire.Hosting.Azure.CognitiveServices) NuGet package:

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

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add an Azure OpenAI resource

To add an <xref:Aspire.Hosting.ApplicationModel.AzureOpenAIResource> to your app host project, call the <xref:Aspire.Hosting.AzureOpenAIExtensions.AddAzureOpenAI%2A> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var openai = builder.AddAzureOpenAI("openai");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(openai);

// After adding all resources, run the app...
```

The preceding code adds an Azure OpenAI resource named `openai` to the app host project. The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method passes the connection information to the `ExampleProject` project.

> [!IMPORTANT]
> When you call <xref:Aspire.Hosting.AzureOpenAIExtensions.AddAzureOpenAI%2A>, it implicitly calls <xref:Aspire.Hosting.AzureProvisionerExtensions.AddAzureProvisioning(Aspire.Hosting.IDistributedApplicationBuilder)>—which adds support for generating Azure resources dynamically during app startup. The app must configure the appropriate subscription and location. For more information, see [Local provisioning: Configuration](../azure/local-provisioning.md#configuration).

### Add an Azure OpenAI deployment resource

To add an Azure OpenAI deployment resource, call the <xref:Aspire.Hosting.AzureOpenAIExtensions.AddDeployment(Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.ApplicationModel.AzureOpenAIResource},System.String,System.String,System.String)> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var openai = builder.AddAzureOpenAI("openai");

openai.AddDeployment(
    name: "preview",
    modelName: "gpt-4.5-preview",
    modelVersion: "2025-02-27");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(openai)
       .WaitFor(openai);

// After adding all resources, run the app...
```

The preceding code:

- Adds an Azure OpenAI resource named `openai`.
- Adds an Azure OpenAI deployment resource named `preview` with a model name of `gpt-4.5-preview`. The model name must correspond to an [available model](/azure/ai-services/openai/concepts/models) in the Azure OpenAI service.

### Provisioning-generated Bicep

If you're new to [Bicep](/azure/azure-resource-manager/bicep/overview), it's a domain-specific language for defining Azure resources. With .NET Aspire, you don't need to write Bicep by-hand, instead the provisioning APIs generate Bicep for you. When you publish your app, the generated Bicep provisions an Azure OpenAI resource with standard defaults.

:::code language="bicep" source="../snippets/azure/AppHost/openai.module.bicep":::

The preceding Bicep is a module that provisions an Azure Cognitive Services resource. Additionally, role assignments are created for the Azure resource in a separate module:

:::code language="bicep" source="../snippets/azure/AppHost/openai-roles.module.bicep":::

The generated Bicep is a starting point and is influenced by changes to the provisioning infrastructure in C#. Customizations to the Bicep file directly will be overwritten, so make changes through the C# provisioning APIs to ensure they are reflected in the generated files.

### Customize provisioning infrastructure

All .NET Aspire Azure resources are subclasses of the <xref:Aspire.Hosting.Azure.AzureProvisioningResource> type. This enables customization of the generated Bicep by providing a fluent API to configure the Azure resources—using the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},System.Action{Aspire.Hosting.Azure.AzureResourceInfrastructure})> API:

:::code language="csharp" source="../snippets/azure/AppHost/Program.ConfigureOpenAIInfra.cs" id="configure":::

The preceding code:

- Chains a call to the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure*> API:
  - The `infra` parameter is an instance of the <xref:Aspire.Hosting.Azure.AzureResourceInfrastructure> type.
  - The provisionable resources are retrieved by calling the <xref:Azure.Provisioning.Infrastructure.GetProvisionableResources> method.
  - The single <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesAccount> resource is retrieved.
  - The <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesAccount.Sku?displayProperty=nameWithType> property is assigned to a new instance of <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesSku> with an `E0` name and <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesSkuTier.Enterprise?displayProperty=nameWithType> tier.
  - A tag is added to the Cognitive Services resource with a key of `ExampleKey` and a value of `Example value`.

### Connect to an existing Azure OpenAI service

You might have an existing Azure OpenAI service that you want to connect to. You can chain a call to annotate that your <xref:Aspire.Hosting.ApplicationModel.AzureOpenAIResource> is an existing resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var existingOpenAIName = builder.AddParameter("existingOpenAIName");
var existingOpenAIResourceGroup = builder.AddParameter("existingOpenAIResourceGroup");

var openai = builder.AddAzureOpenAI("openai")
                    .AsExisting(existingOpenAIName, existingOpenAIResourceGroup);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(openai);

// After adding all resources, run the app...
```

For more information on treating Azure OpenAI resources as existing resources, see [Use existing Azure resources](../azure/integrations-overview.md#use-existing-azure-resources).

> [!NOTE]
> Alternatively, instead of representing an Azure OpenAI resource, you can add a connection string to the app host. This approach is weakly-typed, and doesn't work with role assignments or infrastructure customizations. For more information, see [Add existing Azure resources with connection strings](../azure/integrations-overview.md#add-existing-azure-resources-with-connection-strings).

## Client integration

To get started with the .NET Aspire Azure OpenAI client integration, install the [📦 Aspire.Azure.AI.OpenAI](https://www.nuget.org/packages/Aspire.Azure.AI.OpenAI) NuGet package in the client-consuming project, that is, the project for the application that uses the Azure OpenAI client.

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

### Add an Azure OpenAI client

In the _Program.cs_ file of your client-consuming project, use the <xref:Microsoft.Extensions.Hosting.AspireAzureOpenAIExtensions.AddAzureOpenAIClient(Microsoft.Extensions.Hosting.IHostApplicationBuilder,System.String,System.Action{Aspire.Azure.AI.OpenAI.AzureOpenAISettings},System.Action{Azure.Core.Extensions.IAzureClientBuilder{Azure.AI.OpenAI.AzureOpenAIClient,Azure.AI.OpenAI.AzureOpenAIClientOptions}})> method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register an `OpenAIClient` for dependency injection (DI). The `AzureOpenAIClient` is a subclass of `OpenAIClient`, allowing you to request either type from DI. This ensures code not dependent on Azure-specific features remains generic. The `AddAzureOpenAIClient` method requires a connection name parameter.

```csharp
builder.AddAzureOpenAIClient(connectionName: "openai");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the Azure OpenAI resource in the app host project. For more information, see [Add an Azure OpenAI resource](#add-an-azure-openai-resource).

After adding the `OpenAIClient`, you can retrieve the client instance using dependency injection:

```csharp
public class ExampleService(OpenAIClient client)
{
    // Use client...
}
```

For more information, see:

- [Azure.AI.OpenAI documentation](https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/openai/Azure.AI.OpenAI/README.md) for examples on using the `OpenAIClient`.
- [Dependency injection in .NET](/dotnet/core/extensions/dependency-injection) for details on dependency injection.
- [Quickstart: Get started using GPT-35-Turbo and GPT-4 with Azure OpenAI Service](/azure/ai-services/openai/chatgpt-quickstart?pivots=programming-language-csharp).

### Add Azure OpenAI client with registered `IChatClient`

If you're interested in using the <xref:Microsoft.Extensions.AI.IChatClient> interface, with the OpenAI client, simply chain either of the following APIs to the `AddAzureOpenAIClient` method:

- <xref:Microsoft.Extensions.Hosting.AspireOpenAIClientBuilderChatClientExtensions.AddChatClient(Aspire.OpenAI.AspireOpenAIClientBuilder,System.String)>: Registers a singleton `IChatClient` in the services provided by the <xref:Aspire.OpenAI.AspireOpenAIClientBuilder>.
- <xref:Microsoft.Extensions.Hosting.AspireOpenAIClientBuilderChatClientExtensions.AddKeyedChatClient(Aspire.OpenAI.AspireOpenAIClientBuilder,System.String,System.String)>: Registers a keyed singleton `IChatClient` in the services provided by the <xref:Aspire.OpenAI.AspireOpenAIClientBuilder>.

For example, consider the following C# code that adds an `IChatClient` to the DI container:

```csharp
builder.AddAzureOpenAIClient(connectionName: "openai")
       .AddChatClient("deploymentName");
```

Similarly, you can add a keyed `IChatClient` with the following C# code:

```csharp
builder.AddAzureOpenAIClient(connectionName: "openai")
       .AddKeyedChatClient("serviceKey", "deploymentName");
```

For more information on the `IChatClient` and its corresponding library, see [Artificial intelligence in .NET (Preview)](/dotnet/core/extensions/artificial-intelligence).

### Configure Azure OpenAI client settings

The .NET Aspire Azure OpenAI library provides a set of settings to configure the Azure OpenAI client. The  `AddAzureOpenAIClient` method exposes an optional `configureSettings` parameter of type `Action<AzureOpenAISettings>?`. To configure settings inline, consider the following example:

```csharp
builder.AddAzureOpenAIClient(
    connectionName: "openai",
    configureSettings: settings =>
    {
        settings.DisableTracing = true;

        var uriString = builder.Configuration["AZURE_OPENAI_ENDPOINT"]
            ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");

        settings.Endpoint = new Uri(uriString);
    });
```

The preceding code sets the <xref:Aspire.Azure.AI.OpenAI.AzureOpenAISettings.DisableTracing?displayProperty=nameWithType> property to `true`, and sets the <xref:Aspire.Azure.AI.OpenAI.AzureOpenAISettings.Endpoint?displayProperty=nameWithType> property to the Azure OpenAI endpoint.

### Configure Azure OpenAI client builder options

To configure the <xref:Azure.AI.OpenAI.AzureOpenAIClientOptions> for the client, you can use the <xref:Microsoft.Extensions.Hosting.AspireAzureOpenAIExtensions.AddAzureOpenAIClient%2A> method. This method takes an optional `configureClientBuilder` parameter of type `Action<IAzureClientBuilder<OpenAIClient, AzureOpenAIClientOptions>>?`. Consider the following example:

```csharp
builder.AddAzureOpenAIClient(
    connectionName: "openai",
    configureClientBuilder: clientBuilder =>
    {
        clientBuilder.ConfigureOptions(options =>
        {
            options.UserAgentApplicationId = "CLIENT_ID";
        });
    });
```

The client builder is an instance of the <xref:Azure.Core.Extensions.IAzureClientBuilder`2> type, which provides a fluent API to configure the client options. The preceding code sets the <xref:Azure.AI.OpenAI.AzureOpenAIClientOptions.UserAgentApplicationId?displayProperty=nameWithType> property to `CLIENT_ID`. For more information, see <xref:Microsoft.Extensions.AI.ConfigureOptionsChatClientBuilderExtensions.ConfigureOptions(Microsoft.Extensions.AI.ChatClientBuilder,System.Action{Microsoft.Extensions.AI.ChatOptions})>.

### Add Azure OpenAI client from configuration

Additionally, the package provides the <xref:Microsoft.Extensions.Hosting.AspireConfigurableOpenAIExtensions.AddOpenAIClientFromConfiguration(Microsoft.Extensions.Hosting.IHostApplicationBuilder,System.String)> extension method to register an `OpenAIClient` or `AzureOpenAIClient` instance based on the provided connection string. This method follows these rules:

- If the `Endpoint` attribute is empty or missing, an `OpenAIClient` instance is registered using the provided key, for example, `Key={key};`.
- If the `IsAzure` attribute is `true`, an `AzureOpenAIClient` is registered; otherwise, an `OpenAIClient` is registered, for example, `Endpoint={azure_endpoint};Key={key};IsAzure=true` registers an `AzureOpenAIClient`, while `Endpoint=https://localhost:18889;Key={key}` registers an `OpenAIClient`.
- If the `Endpoint` attribute contains `".azure."`, an `AzureOpenAIClient` is registered; otherwise, an `OpenAIClient` is registered, for example, `Endpoint=https://{account}.azure.com;Key={key};`.

Consider the following example:

```csharp
builder.AddOpenAIClientFromConfiguration("openai");
```

> [!TIP]
> A valid connection string must contain at least an `Endpoint` or a `Key`.

Consider the following example connection strings and whether they register an `OpenAIClient` or `AzureOpenAIClient`:

| Example connection string | Registered client type |
|--|--|
| `Endpoint=https://{account_name}.openai.azure.com/;Key={account_key}` | `AzureOpenAIClient` |
| `Endpoint=https://{account_name}.openai.azure.com/;Key={account_key};IsAzure=false` | `OpenAIClient` |
| `Endpoint=https://{account_name}.openai.azure.com/;Key={account_key};IsAzure=true` | `AzureOpenAIClient` |
| `Endpoint=https://localhost:18889;Key={account_key}` | `OpenAIClient` |

### Add keyed Azure OpenAI clients

There might be situations where you want to register multiple `OpenAIClient` instances with different connection names. To register keyed Azure OpenAI clients, call the <xref:Microsoft.Extensions.Hosting.AspireAzureOpenAIExtensions.AddKeyedAzureOpenAIClient*> method:

```csharp
builder.AddKeyedAzureOpenAIClient(name: "chat");
builder.AddKeyedAzureOpenAIClient(name: "code");
```

> [!IMPORTANT]
> When using keyed services, ensure that your Azure OpenAI resource configures two named connections, one for `chat` and one for `code`.

Then you can retrieve the client instances using dependency injection. For example, to retrieve the clients from a service:

```csharp
public class ExampleService(
    [KeyedService("chat")] OpenAIClient chatClient,
    [KeyedService("code")] OpenAIClient codeClient)
{
    // Use clients...
}
```

For more information, see [Keyed services in .NET](/dotnet/core/extensions/dependency-injection#keyed-services).

### Add keyed Azure OpenAI clients from configuration

The same functionality and rules exist for keyed Azure OpenAI clients as for the nonkeyed clients. You can use the <xref:Microsoft.Extensions.Hosting.AspireConfigurableOpenAIExtensions.AddKeyedOpenAIClientFromConfiguration(Microsoft.Extensions.Hosting.IHostApplicationBuilder,System.String)> extension method to register an `OpenAIClient` or `AzureOpenAIClient` instance based on the provided connection string.

Consider the following example:

```csharp
builder.AddKeyedOpenAIClientFromConfiguration("openai");
```

This method follows the same rules as detailed in the [Add Azure OpenAI client from configuration](#add-azure-openai-client-from-configuration).

### Configuration

The .NET Aspire Azure OpenAI library provides multiple options to configure the Azure OpenAI connection based on the requirements and conventions of your project. Either a `Endpoint` or a `ConnectionString` is required to be supplied.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddAzureOpenAIClient`:

```csharp
builder.AddAzureOpenAIClient("openai");
```

The connection string is retrieved from the `ConnectionStrings` configuration section, and there are two supported formats:

##### Account endpoint

The recommended approach is to use an **Endpoint**, which works with the `AzureOpenAISettings.Credential` property to establish a connection. If no credential is configured, the <xref:Azure.Identity.DefaultAzureCredential> is used.

```json
{
  "ConnectionStrings": {
    "openai": "https://{account_name}.openai.azure.com/"
  }
}
```

For more information, see [Use Azure OpenAI without keys](/azure/developer/ai/keyless-connections?tabs=csharp%2Cazure-cli).

##### Connection string

Alternatively, a custom connection string can be used:

```json
{
  "ConnectionStrings": {
    "openai": "Endpoint=https://{account_name}.openai.azure.com/;Key={account_key};"
  }
}
```

In order to connect to the non-Azure OpenAI service, drop the `Endpoint` property and only set the Key property to set the [API key](https://platform.openai.com/account/api-keys).

#### Use configuration providers

The .NET Aspire Azure OpenAI integration supports <xref:Microsoft.Extensions.Configuration>. It loads the `AzureOpenAISettings` from configuration by using the `Aspire:Azure:AI:OpenAI` key. Example _:::no-loc text="appsettings.json":::_ that configures some of the options:

```json
{
  "Aspire": {
    "Azure": {
      "AI": {
        "OpenAI": {
          "DisableTracing": false
        }
      }
    }
  }
}
```

For the complete Azure OpenAI client integration JSON schema, see [Aspire.Azure.AI.OpenAI/ConfigurationSchema.json](https://github.com/dotnet/aspire/blob/v9.1.0/src/Components/Aspire.Azure.AI.OpenAI/ConfigurationSchema.json).

#### Use inline delegates

You can pass the `Action<AzureOpenAISettings> configureSettings` delegate to set up some or all the options inline, for example to disable tracing from code:

```csharp
builder.AddAzureOpenAIClient(
    "openai",
    static settings => settings.DisableTracing = true);
```

You can also set up the OpenAIClientOptions using the optional `Action<IAzureClientBuilder<OpenAIClient, OpenAIClientOptions>> configureClientBuilder` parameter of the `AddAzureOpenAIClient` method. For example, to set the client ID for this client:

```csharp
builder.AddAzureOpenAIClient(
    "openai",
    configureClientBuilder: builder => builder.ConfigureOptions(
        options => options.Diagnostics.ApplicationId = "CLIENT_ID"));
```

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

### Logging

The .NET Aspire Azure OpenAI integration uses the following log categories:

- `Azure`
- `Azure.Core`
- `Azure.Identity`

### Tracing

The .NET Aspire Azure OpenAI integration emits tracing activities using OpenTelemetry for operations performed with the `OpenAIClient`.

> [!IMPORTANT]
> Tracing is currently experimental with this integration. To opt-in to it, set either the `OPENAI_EXPERIMENTAL_ENABLE_OPEN_TELEMETRY` environment variable to `true` or `1`, or call `AppContext.SetSwitch("OpenAI.Experimental.EnableOpenTelemetry", true))` during app startup.

## See also

- [Azure OpenAI](https://azure.microsoft.com/products/ai-services/openai-service/)
- [.NET Aspire integrations overview](../fundamentals/integrations-overview.md)
- [.NET Aspire Azure integrations overview](../azure/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
