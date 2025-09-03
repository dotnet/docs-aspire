---
title: ".NET Aspire Azure AI Foundry integration (Preview)"
description: "Learn how to integrate Azure AI Foundry with .NET Aspire applications, including hosting and client integration."
ms.date: 08/07/2025
ai-usage: ai-assisted
titleSuffix: ''
---

# .NET Aspire Azure AI Foundry integration (Preview)

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[Azure AI Foundry](https://ai.azure.com/) is an AI platform that provides access to cutting-edge foundation models, tools for AI development, and scalable infrastructure for building intelligent applications. The .NET Aspire Azure AI Foundry integration enables you to connect to Azure AI Foundry or run models locally using Foundry Local from your .NET applications.

## Hosting integration

The .NET Aspire [Azure AI Foundry](/azure/ai-foundry/) hosting integration models Azure AI Foundry resources as `AzureAIFoundryResource`. To access these types and APIs for expressing them within your [AppHost](../fundamentals/app-host-overview.md) project, install the [📦 Aspire.Hosting.Azure.AIFoundry](https://www.nuget.org/packages/Aspire.Hosting.Azure.AIFoundry) NuGet package:

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.AIFoundry
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.AIFoundry"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add an Azure AI Foundry resource

To add an `AzureAIFoundryResource` to your AppHost project, call the `AddAzureAIFoundry` method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var foundry = builder.AddAzureAIFoundry("foundry");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(foundry);

// After adding all resources, run the app...
```

The preceding code adds an Azure AI Foundry resource named `foundry` to the AppHost project. The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference*> method passes the connection information to the `ExampleProject` project.

> [!IMPORTANT]
> When you call <xref:Aspire.Hosting.AzureAIFoundryExtensions.AddAzureAIFoundry*>, it implicitly calls <xref:Aspire.Hosting.AzureProvisionerExtensions.AddAzureProvisioning(Aspire.Hosting.IDistributedApplicationBuilder)>—which adds support for generating Azure resources dynamically during app startup. The app must configure the appropriate subscription and location. For more information, see [Local provisioning: Configuration](/dotnet/aspire/azure/local-provisioning#configuration).

### Add an Azure AI Foundry deployment resource

To add an Azure AI Foundry deployment resource, call the <xref:Aspire.Hosting.AzureAIFoundryExtensions.AddDeployment*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var foundry = builder.AddAzureAIFoundry("foundry");

var chat = foundry.AddDeployment("chat", "Phi-4", "1", "Microsoft");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(chat)
       .WaitFor(chat);

// After adding all resources, run the app...
```

The preceding code:

- Adds an Azure AI Foundry resource named `foundry`.
- Adds an Azure AI Foundry deployment resource named `chat` with a model name of `Phi-4`. The model name must correspond to an [available model](/azure/ai-foundry/foundry-models/concepts/models) in the Azure AI Foundry service.

> [!NOTE]
> The `format` parameter of the `AddDeployment(...)` method can be found in the Azure AI Foundry portal in the details page of the model, right after the `Quick facts` text.

### Configure deployment properties

You can customize deployment properties using the <xref:Aspire.Hosting.AzureAIFoundryExtensions.WithProperties*> method:

```csharp
var chat = foundry.AddDeployment("chat", "Phi-4", "1", "Microsoft")
                  .WithProperties(deployment =>
                  {
                      deployment.SkuName = "Standard";
                      deployment.SkuCapacity = 10;
                  });
```

The preceding code sets the SKU name to `Standard` and capacity to `10` for the deployment.

### Connect to an existing Azure AI Foundry service

You might have an existing Azure AI Foundry service that you want to connect to. You can chain a call to annotate that your `AzureAIFoundryResource` is an existing resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var existingFoundryName = builder.AddParameter("existingFoundryName");
var existingFoundryResourceGroup = builder.AddParameter("existingFoundryResourceGroup");

var foundry = builder.AddAzureAIFoundry("foundry")
                     .AsExisting(existingFoundryName, existingFoundryResourceGroup);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(foundry);

// After adding all resources, run the app...
```

[!INCLUDE [azure-configuration](../azure/includes/azure-configuration.md)]

For more information on treating Azure AI Foundry resources as existing resources, see [Use existing Azure resources](/dotnet/aspire/azure/integrations-overview#use-existing-azure-resources).

> [!NOTE]
> Alternatively, instead of representing an Azure AI Foundry resource, you can add a connection string to the AppHost. This approach is weakly typed, and doesn't work with role assignments or infrastructure customizations. For more information, see [Add existing Azure resources with connection strings](/dotnet/aspire/azure/integrations-overview#add-existing-azure-resources-with-connection-strings).

### Use Foundry Local for development

Aspire supports the usage of Foundry Local for local development. Add the following to your AppHost project:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var foundry = builder.AddAzureAIFoundry("foundry")
                     .RunAsFoundryLocal();

var chat = foundry.AddDeployment("chat", "phi-3.5-mini", "1", "Microsoft");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(chat)
       .WaitFor(chat);

// After adding all resources, run the app...
```

When the AppHost starts up, the local foundry service is also started. This requires the local machine to have [Foundry Local](/azure/ai-foundry/foundry-local/get-started) installed and running.

The <xref:Aspire.Hosting.AzureAIFoundryExtensions.RunAsFoundryLocal*> method configures the resource to run as an emulator. It downloads and loads the specified models locally. The method provides health checks for the local service and automatically manages the Foundry Local lifecycle.

### Assign roles to resources

You can assign specific roles to resources that need to access the Azure AI Foundry service. Use the <xref:Aspire.Hosting.AzureAIFoundryExtensions.WithRoleAssignments*> method:

```csharp
var foundry = builder.AddAzureAIFoundry("foundry");

builder.AddProject<Projects.Api>("api")
       .WithRoleAssignments(foundry, CognitiveServicesBuiltInRole.CognitiveServicesUser)
       .WithReference(foundry);
```

The preceding code assigns the `CognitiveServicesUser` role to the `api` project, granting it the necessary permissions to access the Azure AI Foundry resource.

### Provisioning-generated Bicep

If you're new to [Bicep](/azure/azure-resource-manager/bicep/overview), it's a domain-specific language for defining Azure resources. With .NET Aspire, you don't need to write Bicep by-hand, instead the provisioning APIs generate Bicep for you. When you publish your app, the generated Bicep provisions an Azure AI Foundry resource with standard defaults.

:::code language="bicep" source="../snippets/azure/AppHost/ai-foundry/ai-foundry.bicep":::

The preceding Bicep is a module that provisions an Azure Cognitive Services resource configured for AI Services. Additionally, role assignments are created for the Azure resource in a separate module:

:::code language="bicep" source="../snippets/azure/AppHost/ai-foundry-roles/ai-foundry-roles.bicep":::

The generated Bicep is a starting point and is influenced by changes to the provisioning infrastructure in C#. Customizations to the Bicep file directly will be overwritten, so make changes through the C# provisioning APIs to ensure they're reflected in the generated files.

#### Customize provisioning infrastructure

All .NET Aspire Azure resources are subclasses of the <xref:Aspire.Hosting.Azure.AzureProvisioningResource> type. This enables customization of the generated Bicep by providing a fluent API to configure the Azure resources—using the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},System.Action{Aspire.Hosting.Azure.AzureResourceInfrastructure})> API:

:::code language="csharp" source="../snippets/azure/AppHost/Program.ConfigureAIFoundryInfra.cs" id="configure":::

The preceding code:

- Chains a call to the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure*> API:
  - The `infra` parameter is an instance of the <xref:Aspire.Hosting.Azure.AzureResourceInfrastructure> type.
  - The provisionable resources are retrieved by calling the <xref:Azure.Provisioning.Infrastructure.GetProvisionableResources> method.
  - The single <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesAccount> resource is retrieved.
  - The <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesAccount.Sku?displayProperty=nameWithType> property is assigned to a new instance of <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesSku> with an `E0` name and <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesSkuTier.Enterprise?displayProperty=nameWithType> tier.
  - A tag is added to the Cognitive Services resource with a key of `ExampleKey` and a value of `Example value`.

## Client integration

To get started with the .NET Aspire Azure AI Foundry client integration, install the [📦 Aspire.Azure.AI.Inference](https://www.nuget.org/packages/Aspire.Azure.AI.Inference) NuGet package in the client-consuming project, that is, the project for the application that uses the Azure AI Foundry client.

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

### Add an Azure AI Foundry client

In the Program.cs file of your client-consuming project, use the [AddAzureChatCompletionsClient(IHostApplicationBuilder, String)](/dotnet/api/microsoft.extensions.hosting.aspireazureaiinferenceextensions.addazurechatcompletionsclient) method to register a `ChatCompletionsClient` for dependency injection (DI). The method requires a connection name parameter.

```csharp
builder.AddAzureChatCompletionsClient(connectionName: "chat");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the Azure AI Foundry deployment resource in the AppHost project. For more information, see Add an Azure AI Foundry deployment resource.

After adding the `ChatCompletionsClient`, you can retrieve the client instance using dependency injection:

```csharp
public class ExampleService(ChatCompletionsClient client)
{
    // Use client...
}
```

For more information, see:

- [Azure.AI.Inference documentation](https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/ai/Azure.AI.Inference/README.md) for examples on using the `ChatCompletionsClient`.
- [Dependency injection in .NET](/dotnet/core/extensions/dependency-injection) for details on dependency injection.

### Add Azure AI Foundry client with registered `IChatClient`

If you're interested in using the [IChatClient](/dotnet/api/microsoft.extensions.ai.ichatclient) interface, with the Azure AI Foundry client, simply chain the <xref:Microsoft.Extensions.Hosting.AspireAzureAIInferenceExtensions.AddChatClient*> API to the `AddAzureChatCompletionsClient` method:

```csharp
builder.AddAzureChatCompletionsClient(connectionName: "chat")
       .AddChatClient();
```

For more information on the `IChatClient` and its corresponding library, see [Artificial intelligence in .NET (Preview)](/dotnet/core/extensions/artificial-intelligence).

### Alternative: Use OpenAI client for compatible models

For models that are compatible with the OpenAI API, you can also use the [📦 Aspire.OpenAI](https://www.nuget.org/packages/Aspire.OpenAI) client integration:

```csharp
builder.AddOpenAIClient("chat")
       .AddChatClient();
```

This approach works well with models that support the OpenAI API format.

### Configuration

The .NET Aspire Azure AI Foundry library provides multiple options to configure the Azure AI Foundry connection based on the requirements and conventions of your project. Either an `Endpoint` and `DeploymentId`, or a `ConnectionString` is required to be supplied.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddAzureChatCompletionsClient`:

```csharp
builder.AddAzureChatCompletionsClient("chat");
```

The connection string is retrieved from the `ConnectionStrings` configuration section, and there are two supported formats:

##### Azure AI Foundry Endpoint

The recommended approach is to use an Endpoint, which works with the `ChatCompletionsClientSettings.Credential` property to establish a connection. If no credential is configured, the `DefaultAzureCredential` is used.

```json
{
  "ConnectionStrings": {
    "chat": "Endpoint=https://{endpoint}/;DeploymentId={deploymentName}"
  }
}
```

##### Connection string

Alternatively, a custom connection string can be used:

```json
{
  "ConnectionStrings": {
    "chat": "Endpoint=https://{endpoint}/;Key={account_key};DeploymentId={deploymentName}"
  }
}
```

#### Use configuration providers

The .NET Aspire Azure AI Inference library supports <xref:Microsoft.Extensions.Configuration>. It loads the `ChatCompletionsClientSettings` from configuration by using the `Aspire:Azure:AI:Inference` key. Example appsettings.json that configures some of the options:

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

For the complete Azure AI Inference client integration JSON schema, see [Aspire.Azure.AI.Inference/ConfigurationSchema.json](https://github.com/dotnet/aspire/blob/main/src/Components/Aspire.Azure.AI.Inference/ConfigurationSchema.json).

#### Use inline delegates

You can pass the `Action<ChatCompletionsClientSettings> configureSettings` delegate to set up some or all the options inline, for example to disable tracing from code:

```csharp
builder.AddAzureChatCompletionsClient(
    "chat",
    static settings => settings.DisableTracing = true);
```

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

#### Logging

The .NET Aspire Azure AI Foundry integration uses the following log categories:

- `Azure`
- `Azure.Core`
- `Azure.Identity`

#### Tracing

The .NET Aspire Azure AI Foundry integration emits tracing activities using OpenTelemetry for operations performed with the `ChatCompletionsClient`.

## See also

- [Azure AI Foundry](https://ai.azure.com/?cid=learnDocs)
- [.NET Aspire integrations overview](/dotnet/aspire/fundamentals/integrations-overview)
- [.NET Aspire Azure integrations overview](/dotnet/aspire/azure/integrations-overview)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
