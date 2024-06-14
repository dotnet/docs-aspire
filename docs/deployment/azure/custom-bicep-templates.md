---
title: Use custom Bicep templates
description: Learn how to customize the Bicep templates provided by .NET Aspire to better suit your needs.
ms.date: 06/11/2024
---

# Use custom Bicep templates

When you're targeting Azure as your desired cloud provider, you can use Bicep to define your infrastructure as code. [Bicep is a domain-specific language (DSL)](/azure/azure-resource-manager/bicep/overview) for deploying Azure resources declaratively. It aims to drastically simplify the authoring experience with a cleaner syntax and better support for modularity and code reuse.

While .NET Aspire provides a set of pre-built Bicep templates so that you don't need to write them, there might be times when you either want to customize the templates or create your own. This article explains the concepts and corresponding APIs that you can use to customize the Bicep templates.

> [!IMPORTANT]
> This article is not intended to teach Bicep, but rather to provide guidance on how to create customize Bicep templates for use with .NET Aspire.

As part of the Azure deployment story for .NET Aspire, the Azure Developer CLI (`azd`) provides an understanding of your .NET Aspire project and the ability to deploy it to Azure. The `azd` CLI uses the Bicep templates to deploy the application to Azure.

## Install App Host package

To use any of this functionality, you must install the [Aspire.Hosting.Azure](https://nuget.org/packages/Aspire.Hosting.Azure) NuGet package:

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

All of the examples in this article assume that you've installed the `Aspire.Hosting.Azure` package and imported the `Aspire.Hosting.Azure` namespace. Additionally, the examples assume you've created an `IDistributedApplicationBuilder` instance:

```csharp
using Aspire.Hosting.Azure;

var builder = DistributedApplication.CreateBuilder(args);

// Examples go here...

builder.Build().Run();
```

> [!TIP]
> By default, when you call any of the Bicep-related APIs, a call is also made to <xref:Aspire.Hosting.AzureProvisionerExtensions.AddAzureProvisioning%2A> that adds support for generating Azure resources dynamically during application startup.

## Reference Bicep files

Imagine that you've defined a Bicep template in a file named `storage.bicep` that provisions an Azure Storage Account:

:::code language="bicep" source="snippets/AppHost.Bicep/storage.bicep":::

To add a reference to the Bicep file on disk, call the <xref:Aspire.Hosting.AzureBicepResourceExtensions.AddBicepTemplate%2A> method. Consider the following example:

:::code language="csharp" source="snippets/AppHost.Bicep/Program.ReferenceBicep.cs" id="addfile":::

The preceding code adds a reference to a Bicep file located at `../infra/storage.bicep`. The file paths should be relative to the _app host_ project. This reference results in an <xref:Aspire.Hosting.Azure.AzureBicepResource> being added to the application's resources collection with the `"storage"` name, and the API returns an `IResourceBuilder<AzureBicepResource>` instance that can be used to further customize the resource.

## Reference Bicep inline

While having a Bicep file on disk is the most common scenario, you can also add Bicep templates inline. Inline templates can be useful when you want to define a template in code or when you want to generate the template dynamically. To add an inline Bicep template, call the <xref:Aspire.Hosting.AzureBicepResourceExtensions.AddBicepTemplateString%2A> method with the Bicep template as a `string`. Consider the following example:

:::code language="csharp" source="snippets/AppHost.Bicep/Program.InlineBicep.cs" id="addinline":::

In this example, the Bicep template is defined as an inline `string` and added to the application's resources collection with the name `"ai"`. This example provisions an Azure AI resource.

## Pass parameters to Bicep templates

[Bicep supports accepting parameters](/azure/azure-resource-manager/bicep/parameters), which can be used to customize the behavior of the template. To pass parameters to a Bicep template from .NET Aspire, chain calls to the <xref:Aspire.Hosting.AzureBicepResourceExtensions.WithParameter%2A> method as shown in the following example:

:::code language="csharp" source="snippets/AppHost.Bicep/Program.PassParameter.cs" id="addparameter":::

The preceding code:

- Adds a parameter named `"region"` to the `builder` instance.
- Adds a reference to a Bicep file located at `../infra/storage.bicep`.
- Passes the `"region"` parameter to the Bicep template, which is resolved using the standard parameter resolution.
- Passes the `"storageName"` parameter to the Bicep template with a _hardcoded_ value.
- Passes the `"tags"` parameter to the Bicep template with an array of strings.

For more information, see [External parameters](../../fundamentals/external-parameters.md).

### Well-known parameters

.NET Aspire provides a set of well-known parameters that can be passed to Bicep templates. These parameters are used to provide information about the application and the environment to the Bicep templates. The following well-known parameters are available:

| Field | Description | Value |
|--|--|--|
| <xref:Aspire.Hosting.Azure.AzureBicepResource.KnownParameters.KeyVaultName?displayProperty=nameWithType> | The name of the key vault resource used to store secret outputs. | `"keyVaultName"` |
| <xref:Aspire.Hosting.Azure.AzureBicepResource.KnownParameters.Location?displayProperty=nameWithType> | The location of the resource. This is required for all resources. | `"location"` |
| <xref:Aspire.Hosting.Azure.AzureBicepResource.KnownParameters.LogAnalyticsWorkspaceId?displayProperty=nameWithType> | The resource ID of the log analytics workspace. | `"logAnalyticsWorkspaceId"` |
| <xref:Aspire.Hosting.Azure.AzureBicepResource.KnownParameters.PrincipalId?displayProperty=nameWithType> | The principal ID of the current user or managed identity. | `"principalId"` |
| <xref:Aspire.Hosting.Azure.AzureBicepResource.KnownParameters.PrincipalName?displayProperty=nameWithType> | The principal name of the current user or managed identity. | `"principalName"` |
| <xref:Aspire.Hosting.Azure.AzureBicepResource.KnownParameters.PrincipalType?displayProperty=nameWithType> | The principal type of the current user or managed identity. Either `User` or `ServicePrincipal`. | `"principalType"` |

To use a well-known parameter, pass the parameter name to the <xref:Aspire.Hosting.AzureBicepResourceExtensions.WithParameter%2A> method, such as `WithParameter(AzureBicepResource.KnownParameters.KeyVaultName)`. You don't pass values for well-known parameters, as they're resolved automatically by .NET Aspire.

Consider an example where you want to setup an Azure Event Grid webhook. You might define the Bicep template as follows:

 :::code language="bicep" source="snippets/AppHost.Bicep/event-grid-webhook.bicep" highlight="3-4,27-35":::

This Bicep template defines several parameters, including the `topicName`, `webHookEndpoint`, `principalId`, `principalType`, and the optional `location`. To pass these parameters to the Bicep template, you can use the following code snippet:

:::code language="csharp" source="snippets/AppHost.Bicep/Program.PassParameter.cs" id="addwellknownparams":::

- The `webHookApi` project is added as a reference to the `builder`.
- The `topicName` parameter is passed a hardcoded name value.
- The `webHookEndpoint` parameter is passed as an expression that resolves to the URL from the `api` project references' "https" endpoint with the `/hook` route.
- The `principalId` and `principalType` parameters are passed as well-known parameters.

The well-known parameters are convention-based and shouldn't be accompanied with a corresponding value when passed using the `WithParameter` API. Well-known parameters simplify some common functionality, such as _role assignments_, when added to the Bicep templates, as shown in the preceding example. Role assignments are required for the Event Grid webhook to send events to the specified endpoint. For more information, see [EventGrid Data Sender role assignment](/azure/role-based-access-control/built-in-roles/integration#eventgrid-data-sender).

## Get outputs from Bicep references

In addition to passing parameters to Bicep templates, you can also get outputs from the Bicep templates. Consider the following Bicep template, as it defines an `output` named `endpoint`:

:::code language="bicep" source="snippets/AppHost.Bicep/storage-out.bicep":::

The Bicep defines an output named `endpoint`. To get the output from the Bicep template, call the <xref:Aspire.Hosting.AzureBicepResourceExtensions.GetOutput%2A> method on an `IResourceBuilder<AzureBicepResource>` instance as demonstrated in following C# code snippet:

:::code language="csharp" source="snippets/AppHost.Bicep/Program.GetOutputReference.cs" id="getoutput":::

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

:::code language="bicep" source="snippets/AppHost.Bicep/cosmosdb.bicep" highlight="2,41":::

The preceding Bicep template expects a `keyVaultName` parameter, among several other parameters. It then defines an Azure Cosmos DB resource and stashes a secret into Azure Key Vault, named `connectionString` which represents the fully qualified connection string to the Cosmos DB instance. To access this secret connection string value, you can use the following code snippet:

:::code language="csharp" source="snippets/AppHost.Bicep/Program.cs" id="secrets":::

In the preceding code snippet, the `cosmos` Bicep template is added as a reference to the `builder`. The `connectionString` secret output is retrieved from the Bicep template and stored in a variable. The secret output is then passed as an environment variable (`ConnectionStrings__cosmos`) to the `api` project. This environment variable is used to connect to the Cosmos DB instance.

When this resource is deployed, the underlying deployment mechanism with automatically [Reference secrets from Azure Key Vault](/azure/container-apps/manage-secrets?tabs=azure-portal#reference-secret-from-key-vault). To guarantee secret isolation, .NET Aspire creates a Key Vault per source.

> [!NOTE]
> In _local provisioning_ mode, the secret is extracted from Key Vault and set it in an environment variable. For more information, see [Local Azure provisioning](local-provisioning.md).

## See also

For continued learning, see the following resources as they relate to .NET Aspire and Azure deployment:

- [Deploy a .NET Aspire project to Azure Container Apps](aca-deployment.md)
- [Deploy a .NET Aspire project to Azure Container Apps using the Azure Developer CLI (in-depth guide)](aca-deployment-azd-in-depth.md)
- [.NET Aspire manifest format for deployment tool builders](../manifest-format.md)
