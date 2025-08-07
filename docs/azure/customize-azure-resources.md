---
title: Customize Azure resources
description: Describes how to customize your Azure infrastructure using code in .NET Aspire solutions.
ms.date: 07/22/2025
uid: dotnet/aspire/integrations/customize-azure-resources
---

# Customize Azure resources

If you're using Azure to host resources for a .NET Aspire solution, you have granular control over those resources. You can either let .NET Aspire configure them if the default properties suit your needs, or override defaults to control their behavior. Let's examine how you can customize your Azure infrastructure from .NET Aspire code.

The Azure SDK for .NET provides the [📦 Azure.Provisioning](https://www.nuget.org/packages/Azure.Provisioning) NuGet package and a suite of service-specific [Azure provisioning packages](https://www.nuget.org/packages?q=owner%3A+azure-sdk+description%3A+declarative+resource+provisioning&sortby=relevance). These Azure provisioning libraries make it easy to declaratively specify Azure infrastructure natively in .NET. Their APIs enable you to write object-oriented infrastructure in C#, resulting in Bicep. [Bicep is a domain-specific language (DSL)](/azure/azure-resource-manager/bicep/overview) for deploying Azure resources declaratively.

<!-- TODO: Add link from here to the Azure docs when they're written. -->

While it's possible to provision Azure resources manually, .NET Aspire simplifies the process by providing a set of APIs to express Azure resources. These APIs are available as extension methods in .NET Aspire Azure hosting libraries, extending the <xref:Aspire.Hosting.IDistributedApplicationBuilder> interface. When you add Azure resources to your app host, they add the appropriate provisioning functionality implicitly. In other words, you don't need to call any provisioning APIs directly.

Since .NET Aspire models Azure resources within Azure hosting integrations, the Azure SDK is used to provision these resources. Bicep files are generated that define the Azure resources you need. The generated Bicep files are output alongside the manifest file when you publish your app.

There are several ways to influence the generated Bicep files:

- [Azure.Provisioning customization](#azureprovisioning-customization):
  - [Configure infrastructure](#configure-infrastructure): Customize Azure resource infrastructure.
  - [Add Azure infrastructure](#add-azure-infrastructure): Manually add Azure infrastructure to your app host.
- [Use custom Bicep templates](#use-custom-bicep-templates):
  - [Reference Bicep files](#reference-bicep-files): Add a reference to a Bicep file on disk.
  - [Reference Bicep inline](#reference-bicep-inline): Add an inline Bicep template.

## Local provisioning and `Azure.Provisioning`

To avoid conflating terms and to help disambiguate "provisioning," it's important to understand the distinction between _local provisioning_ and _Azure provisioning_:

- **_Local provisioning:_**

  By default, when you call any of the Azure hosting integration APIs to add Azure resources, the <xref:Aspire.Hosting.AzureProvisionerExtensions.AddAzureProvisioning(Aspire.Hosting.IDistributedApplicationBuilder)> API is called implicitly. This API registers services in the dependency injection (DI) container that are used to provision Azure resources when the AppHost starts. This concept is known as _local provisioning_.  For more information, see [Local Azure provisioning](local-provisioning.md).

- **_`Azure.Provisioning`:_**

  `Azure.Provisioning` refers to the NuGet package, and is a set of libraries that lets you use C# to generate Bicep. The Azure hosting integrations in .NET Aspire use these libraries under the covers to generate Bicep files that define the Azure resources you need. For more information, see [`Azure.Provisioning` customization](#azureprovisioning-customization).

## `Azure.Provisioning` customization

All .NET Aspire Azure hosting integrations expose various Azure resources, and they're all subclasses of the <xref:Aspire.Hosting.Azure.AzureProvisioningResource> type—which itself inherits the <xref:Aspire.Hosting.Azure.AzureBicepResource>. This enables extensions that are generically type-constrained to this type, allowing for a fluent API to customize the infrastructure to your liking. While .NET Aspire provides defaults, you're free to influence the generated Bicep using these APIs.

### Configure infrastructure

Regardless of the Azure resource you're working with, to configure its underlying infrastructure, you chain a call to the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure*> extension method. This method allows you to customize the infrastructure of the Azure resource by passing a `configure` delegate of type `Action<AzureResourceInfrastructure>`. The <xref:Aspire.Hosting.Azure.AzureResourceInfrastructure> type is a subclass of the <xref:Azure.Provisioning.Infrastructure?displayProperty=fullName>. This type exposes a massive API surface area for configuring the underlying infrastructure of the Azure resource.

Consider the following example:

:::code language="csharp" source="../snippets/azure/AppHost/Program.ConfigureInfrastructure.cs" id="infra":::

The preceding code:

- Adds a parameter named `storage-sku`.
- Adds Azure Storage with the <xref:Aspire.Hosting.AzureStorageExtensions.AddAzureStorage*> API named `storage`.
- Chains a call to `ConfigureInfrastructure` to customize the Azure Storage infrastructure:
  - Gets the provisionable resources.
  - Filters to a single <xref:Azure.Provisioning.Storage.StorageAccount>.
  - Assigns the `storage-sku` parameter to the <xref:Azure.Provisioning.Storage.StorageAccount.Sku?displayProperty=nameWithType> property:
    - A new instance of the <xref:Azure.Provisioning.Storage.StorageSku> has its `Name` property assigned from the result of the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.AsProvisioningParameter*> API.

This exemplifies flowing an [external parameter](../fundamentals/external-parameters.md) into the Azure Storage infrastructure, resulting in the generated Bicep file reflecting the desired configuration.

### Add Azure infrastructure

Not all Azure services are exposed as .NET Aspire integrations. While they might be at a later time, you can still provision services that are available in `Azure.Provisioning.*` libraries. Imagine a scenario where you have worker service that's responsible for managing an Azure Container Registry. Now imagine that an AppHost project takes a dependency on the [📦 Azure.Provisioning.ContainerRegistry](https://www.nuget.org/packages/Azure.Provisioning.ContainerRegistry) NuGet package.

You can use the `AddAzureInfrastructure` API to add the Azure Container Registry infrastructure to your app host:

:::code language="csharp" source="../snippets/azure/AppHost/Program.AddAzureInfra.cs" id="add":::

The preceding code:

- Calls <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.AddAzureInfrastructure*> with a name of `acr`.
- Provides a `configureInfrastructure` delegate to customize the Azure Container Registry infrastructure:
  - Instantiates a <xref:Azure.Provisioning.ContainerRegistry.ContainerRegistryService> with the name `acr` and a standard SKU.
  - Adds the Azure Container Registry service to the `infra` variable.
  - Instantiates a <xref:Azure.Provisioning.ProvisioningOutput> with the name `registryName`, a type of `string`, and a value that corresponds to the name of the Azure Container Registry.
  - Adds the output to the `infra` variable.
- Adds a project named `worker` to the builder.
- Chains a call to <xref:Aspire.Hosting.ResourceBuilderExtensions.WithEnvironment*> to set the `ACR_REGISTRY_NAME` environment variable in the project to the value of the `registryName` output.

The functionality demonstrates how to add Azure infrastructure to your AppHost project, even if the Azure service isn't directly exposed as a .NET Aspire integration. It further shows how to flow the output of the Azure Container Registry into the environment of a dependent project.

Consider the resulting Bicep file:

:::code language="bicep" source="../snippets/azure/AppHost/acr/acr.bicep":::

The Bicep file reflects the desired configuration of the Azure Container Registry, as defined by the `AddAzureInfrastructure` API.

## Use custom Bicep templates

When you're targeting Azure as your desired cloud provider, you can use Bicep to define your infrastructure as code. It aims to drastically simplify the authoring experience with a cleaner syntax and better support for modularity and code reuse.

While .NET Aspire provides a set of prebuilt Bicep templates, there might be times when you either want to customize the templates or create your own. This section explains the concepts and corresponding APIs that you can use to customize the Bicep templates.

> [!IMPORTANT]
> This section isn't intended to teach you Bicep, but rather to provide guidance on how to create custom Bicep templates for use with .NET Aspire.

As part of the [Azure deployment story for .NET Aspire](../deployment/overview.md), the Azure Developer CLI (`azd`) provides an understanding of your .NET Aspire project and the ability to deploy it to Azure. The `azd` CLI uses the Bicep templates to deploy the application to Azure.

### Install `Aspire.Hosting.Azure` package

When you want to reference Bicep files, it's possible that you're not using any of the Azure hosting integrations. In this case, you can still reference Bicep files by installing the `Aspire.Hosting.Azure` package. This package provides the necessary APIs to reference Bicep files and customize the Azure resources.

> [!TIP]
> If you're using any of the Azure hosting integrations, you don't need to install the `Aspire.Hosting.Azure` package, as it's a transitive dependency.

To use any of this functionality, the [📦 Aspire.Hosting.Azure](https://www.nuget.org/packages/Aspire.Hosting.Azure) NuGet package must be installed:

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### What to expect from the examples

All the examples in this section assume that you're using the <xref:Aspire.Hosting.Azure> namespace. Additionally, the examples assume you have an <xref:Aspire.Hosting.IDistributedApplicationBuilder> instance:

```csharp
using Aspire.Hosting.Azure;

var builder = DistributedApplication.CreateBuilder(args);

// Examples go here...

builder.Build().Run();
```

By default, when you call any of the Bicep-related APIs, a call is also made to <xref:Aspire.Hosting.AzureProvisionerExtensions.AddAzureProvisioning%2A> that adds support for generating Azure resources dynamically during application startup. For more information, see [Local provisioning and `Azure.Provisioning`](#local-provisioning-and-azureprovisioning).

### Reference Bicep files

Imagine that you have a Bicep template in a file named `storage.bicep` that provisions an Azure Storage Account:

:::code language="bicep" source="snippets/bicep/AppHost.Bicep/storage.bicep":::

To add a reference to the Bicep file on disk, call the <xref:Aspire.Hosting.AzureBicepResourceExtensions.AddBicepTemplate%2A> method. Consider the following example:

:::code language="csharp" source="snippets/bicep/AppHost.Bicep/Program.ReferenceBicep.cs" id="addfile":::

The preceding code adds a reference to a Bicep file located at `../infra/storage.bicep`. The file paths should be relative to the _app host_ project. This reference results in an <xref:Aspire.Hosting.Azure.AzureBicepResource> being added to the application's resources collection with the `"storage"` name, and the API returns an `IResourceBuilder<AzureBicepResource>` instance that can be used to further customize the resource.

### Reference Bicep inline

While having a Bicep file on disk is the most common scenario, you can also add Bicep templates inline. Inline templates can be useful when you want to define a template in code or when you want to generate the template dynamically. To add an inline Bicep template, call the <xref:Aspire.Hosting.AzureBicepResourceExtensions.AddBicepTemplateString%2A> method with the Bicep template as a `string`. Consider the following example:

:::code language="csharp" source="snippets/bicep/AppHost.Bicep/Program.InlineBicep.cs" id="addinline":::

In this example, the Bicep template is defined as an inline `string` and added to the application's resources collection with the name `"ai"`. This example provisions an Azure AI resource.

### Pass parameters to Bicep templates

[Bicep supports accepting parameters](/azure/azure-resource-manager/bicep/parameters), which can be used to customize the behavior of the template. To pass parameters to a Bicep template from .NET Aspire, chain calls to the <xref:Aspire.Hosting.AzureBicepResourceExtensions.WithParameter%2A> method as shown in the following example:

:::code language="csharp" source="snippets/bicep/AppHost.Bicep/Program.PassParameter.cs" id="addparameter":::

The preceding code:

- Adds a parameter named `"region"` to the `builder` instance.
- Adds a reference to a Bicep file located at `../infra/storage.bicep`.
- Passes the `"region"` parameter to the Bicep template, which is resolved using the standard parameter resolution.
- Passes the `"storageName"` parameter to the Bicep template with a _hardcoded_ value.
- Passes the `"tags"` parameter to the Bicep template with an array of strings.

For more information, see [External parameters](../fundamentals/external-parameters.md).

#### Well-known parameters

.NET Aspire provides a set of well-known parameters that can be passed to Bicep templates. These parameters are used to provide information about the application and the environment to the Bicep templates. The following well-known parameters are available:

| Field | Description | Value |
|--|--|--|
| <xref:Aspire.Hosting.Azure.AzureBicepResource.KnownParameters.KeyVaultName?displayProperty=nameWithType> | The name of the key vault resource used to store secret outputs. | `"keyVaultName"` |
| <xref:Aspire.Hosting.Azure.AzureBicepResource.KnownParameters.Location?displayProperty=nameWithType> | The location of the resource. This is required for all resources. | `"location"` |
| <xref:Aspire.Hosting.Azure.AzureBicepResource.KnownParameters.LogAnalyticsWorkspaceId?displayProperty=nameWithType> | The resource ID of the log analytics workspace. | `"logAnalyticsWorkspaceId"` |
| <xref:Aspire.Hosting.Azure.AzureBicepResource.KnownParameters.PrincipalId?displayProperty=nameWithType> | The principal ID of the current user or managed identity. | `"principalId"` |
| <xref:Aspire.Hosting.Azure.AzureBicepResource.KnownParameters.PrincipalName?displayProperty=nameWithType> | The principal name of the current user or managed identity. | `"principalName"` |
| <xref:Aspire.Hosting.Azure.AzureBicepResource.KnownParameters.PrincipalType?displayProperty=nameWithType> | The principal type of the current user or managed identity. Either `User` or `ServicePrincipal`. | `"principalType"` |

To use a well-known parameter, pass the parameter name to the <xref:Aspire.Hosting.AzureBicepResourceExtensions.WithParameter%2A> method, such as `WithParameter(AzureBicepResource.KnownParameters.KeyVaultName)`. You don't pass values for well-known parameters, as .NET Aspire resolves them on your behalf.

Consider an example where you want to set up an Azure Event Grid webhook. You might define the Bicep template as follows:

 :::code language="bicep" source="snippets/bicep/AppHost.Bicep/event-grid-webhook.bicep" highlight="3-4,27-35":::

This Bicep template defines several parameters, including the `topicName`, `webHookEndpoint`, `principalId`, `principalType`, and the optional `location`. To pass these parameters to the Bicep template, you can use the following code snippet:

:::code language="csharp" source="snippets/bicep/AppHost.Bicep/Program.PassParameter.cs" id="addwellknownparams":::

- The `webHookApi` project is added as a reference to the `builder`.
- The `topicName` parameter is passed a hardcoded name value.
- The `webHookEndpoint` parameter is passed as an expression that resolves to the URL from the `api` project references' "https" endpoint with the `/hook` route.
- The `principalId` and `principalType` parameters are passed as well-known parameters.

The well-known parameters are convention-based and shouldn't be accompanied with a corresponding value when passed using the `WithParameter` API. Well-known parameters simplify some common functionality, such as _role assignments_, when added to the Bicep templates, as shown in the preceding example. Role assignments are required for the Event Grid webhook to send events to the specified endpoint. For more information, see [Event Grid Data Sender role assignment](/azure/role-based-access-control/built-in-roles/integration#eventgrid-data-sender).

## Get outputs from Bicep references

In addition to passing parameters to Bicep templates, you can also get outputs from the Bicep templates. Consider the following Bicep template, as it defines an `output` named `endpoint`:

:::code language="bicep" source="snippets/bicep/AppHost.Bicep/storage-out.bicep":::

The Bicep defines an output named `endpoint`. To get the output from the Bicep template, call the <xref:Aspire.Hosting.AzureBicepResourceExtensions.GetOutput%2A> method on an `IResourceBuilder<AzureBicepResource>` instance as demonstrated in following C# code snippet:

:::code language="csharp" source="snippets/bicep/AppHost.Bicep/Program.GetOutputReference.cs" id="getoutput":::

In this example, the output from the Bicep template is retrieved and stored in an `endpoint` variable. Typically, you would pass this output as an environment variable to another resource that relies on it. For instance, if you had an ASP.NET Core Minimal API project that depended on this endpoint, you could pass the output as an environment variable to the project using the following code snippet:

```csharp
var storage = builder.AddBicepTemplate(
                name: "storage",
                bicepFile: "../infra/storage.bicep"
            );

var endpoint = storage.GetOutput("endpoint");

var apiService = builder.AddProject<Projects.AspireSample_ApiService>(
        name: "apiservice"
    )
    .WithEnvironment("STORAGE_ENDPOINT", endpoint);
```

For more information, see [Bicep outputs](/azure/azure-resource-manager/bicep/outputs).

## Get secret outputs from Bicep references

It's important to [avoid outputs for secrets](/azure/azure-resource-manager/bicep/scenarios-secrets#avoid-outputs-for-secrets) when working with Bicep. If an output is considered a _secret_, meaning it shouldn't be exposed in logs or other places, you can treat it as such. This can be achieved by storing the secret in Azure Key Vault and referencing it in the Bicep template. .NET Aspire's Azure integration provides a pattern for securely storing outputs from the Bicep template by allows resources to use the `keyVaultName` parameter to store secrets in Azure Key Vault.

Consider the following Bicep template as an example the helps to demonstrate this concept of securing secret outputs:

:::code language="bicep" source="snippets/bicep/AppHost.Bicep/cosmosdb.bicep" highlight="2,41":::

The preceding Bicep template expects a `keyVaultName` parameter, among several other parameters. It then defines an Azure Cosmos DB resource and stashes a secret into Azure Key Vault, named `connectionString` which represents the fully qualified connection string to the Cosmos DB instance. To access this secret connection string value, you can use the following code snippet:

:::code language="csharp" source="snippets/bicep/AppHost.Bicep/Program.cs" id="secrets":::

In the preceding code snippet, the `cosmos` Bicep template is added as a reference to the `builder`. The `connectionString` secret output is retrieved from the Bicep template and stored in a variable. The secret output is then passed as an environment variable (`ConnectionStrings__cosmos`) to the `api` project. This environment variable is used to connect to the Cosmos DB instance.

When this resource is deployed, the underlying deployment mechanism will automatically [Reference secrets from Azure Key Vault](/azure/container-apps/manage-secrets?tabs=azure-portal#reference-secret-from-key-vault). To guarantee secret isolation, .NET Aspire creates a Key Vault per source.

> [!NOTE]
> In _local provisioning_ mode, the secret is extracted from Key Vault and set it in an environment variable. For more information, see [Local Azure provisioning](local-provisioning.md).
