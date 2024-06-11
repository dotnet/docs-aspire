---
title: Use custom Bicep templates
description: Learn how to customize the Bicep templates provided by .NET Aspire to better suit your needs.
ms.date: 06/11/2024
---

# Use custom Bicep templates

When you're targeting Azure as your desired cloud provider, you can use Bicep to define your infrastructure as code. [Bicep is a Domain Specific Language (DSL)](/azure/azure-resource-manager/bicep/overview) for deploying Azure resources declaratively. It aims to drastically simplify the authoring experience with a cleaner syntax and better support for modularity and code re-use.

While .NET Aspire provides a set of pre-built Bicep templates so that you don't need to write them, there may be times where you either want to customize the templates or create your own. This article explains the concepts and corresponding APIs that you can use to customize the Bicep templates.

> [!IMPORTANT]
> This article is not intended to teach Bicep, but rather to provide guidance on how to create customize Bicep templates for use with .NET Aspire.

As part of the Azure deployment story for .NET Aspire, the Azure Developer CLI (`azd`) provides an understanding of your .NET Aspire application and the ability to deploy it to Azure. The `azd` CLI uses the Bicep templates to deploy the application to Azure.

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

All of the examples in this article assume that you have installed the `Aspire.Hosting.Azure` package, and will be using the `Aspire.Hosting.Azure` namespace. Additionally, the examples assume you've created an `IDistributedApplicationBuilder` instance:

```csharp
using Aspire.Hosting.Azure;

var builder = DistributedApplication.CreateBuilder(args);

// Examples go here...

builder.Build().Run();
```

## Reference Bicep files

To add a reference to a Bicep file on disk, call the <xref:Aspire.Hosting.AzureBicepResourceExtensions.AddBicepTemplate%2A> method. Consider the following example:

:::code language="csharp" source="snippets/AppHost.Bicep/Program.ReferenceBicep.cs" id="addfile":::

The preceding code adds a reference to a Bicep file located at `../infra/storage.bicep`—file paths are relative to the _app host_ project. This results in an <xref:Aspire.Hosting.Azure.AzureBicepResource> being added to the application's resources collection with the `"storage"` name.

## Reference Bicep inline

While having a Bicep file on disk is the most common scenario, you can also add Bicep templates inline. This can be useful when you want to define a template in code or when you want to generate the template dynamically. To add an inline Bicep template, call the <xref:Aspire.Hosting.AzureBicepResourceExtensions.AddBicepTemplateString%2A> method with the Bicep template as a string. Consider the following example:

:::code language="csharp" source="snippets/AppHost.Bicep/Program.InlineBicep.cs" id="addinline":::

In this example, the Bicep template is defined as a `string` inline and added to the application's resources collection with the `"ai"` name. This example would provision an Azure AI resource.

## Pass parameters to Bicep templates

Bicep supports accepting parameters, which can be used to customize the behavior of the template. To pass parameters to a Bicep template from .NET Aspire, chain calls to the <xref:Aspire.Hosting.AzureBicepResourceExtensions.WithParameter%2A> method. Consider the following example:

:::code language="csharp" source="snippets/AppHost.Bicep/Program.PassParameter.cs" id="addparameter":::

The preceding code:

- Adds a parameter named `"region"` to the application.
- Adds a reference to a Bicep file located at `../infra/storage.bicep`.
- Passes the `"region"` parameter to the Bicep template, which is resolved using the standard parameter resolution.
- Passes the `"storageName"` parameter to the Bicep template with a hardcoded value.
- Adds the `"principalId"` and `"principalType"` parameters to the Bicep template, which are well-known parameters.
- Passes the `"tags"` parameter to the Bicep template with an array of strings.

For more information, see [Bicep parameters](/azure/azure-resource-manager/bicep/parameters).

### Well known parameters

.NET Aspire provides a set of well-known parameters that can be passed to Bicep templates. These parameters are used to provide information about the application and the environment to the Bicep templates. The following well-known parameters are available:

- <xref:Aspire.Hosting.Azure.AzureBicepResource.KnownParameters.KeyVaultName>: The name of the key vault resource used to store secret outputs (`"keyVaultName"`).
- <xref:Aspire.Hosting.Azure.AzureBicepResource.KnownParameters.Location>: The location of the resource. This is required for all resources (`"location"`).
- <xref:Aspire.Hosting.Azure.AzureBicepResource.KnownParameters.LogAnalyticsWorkspaceId>: The resource id of the log analytics workspace (`"logAnalyticsWorkspaceId"`).
- <xref:Aspire.Hosting.Azure.AzureBicepResource.KnownParameters.PrincipalId>: The principal id of the current user or managed identity (`"principalId"`).
- <xref:Aspire.Hosting.Azure.AzureBicepResource.KnownParameters.PrincipalName>: The principal name of the current user or managed identity (`"principalName"`).
- <xref:Aspire.Hosting.Azure.AzureBicepResource.KnownParameters.PrincipalType>:
The principal type of the current user or managed identity. Either 'User' or 'ServicePrincipal' (`"principalType"`).

## Get outputs from Bicep references

In addition to passing parameters to Bicep templates, you can also get outputs from the Bicep templates. Consider the following Bicep template:

:::code language="bicep" source="snippets/AppHost.Bicep/storage-out.bicep":::

The Bicep defines an output named `endpoint`. To get the output from the Bicep template, call the <xref:Aspire.Hosting.AzureBicepResourceExtensions.GetOutput%2A> method on an `IResourceBuilder<AzureBicepResource>`. Consider the following example:

:::code language="csharp" source="snippets/AppHost.Bicep/Program.GetOutputReference.cs" id="getoutput":::

In this example, the `endpoint` output from the Bicep template is retrieved and stored in the `endpoint` variable. Most often, you'll pass the output as an environment variable to another resource that depends on it. For example, if there was an ASP.NET Core API project that depended on it, we could pass the output as an environment variable:

```csharp
var apiService = builder.AddProject<Projects.AspireSample_ApiService>("apiservice")
    .WithEnvironment("STORAGE_ENDPOINT", endpoint);
```

If an output is considered a secret, meaning it should not be exposed in logs or other places, you can treat it as a secret by calling the <xref:Aspire.Hosting.AzureBicepResourceExtensions.GetSecretOutput%2A>. This is an output that is written to a keyvault using the `"keyVaultName"` convention.

For more information, see [Bicep outputs](/azure/azure-resource-manager/bicep/outputs).
