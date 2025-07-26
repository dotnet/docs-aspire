---
title: "Breaking change - AddAzureOpenAI defaults to CognitiveServicesOpenAIUser instead of CognitiveServicesOpenAIContributor"
description: "Learn about the breaking change in .NET Aspire 9.4 where AddAzureOpenAI defaults to a lower privilege role."
ms.date: 7/11/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/3936
---

# AddAzureOpenAI defaults to CognitiveServicesOpenAIUser instead of CognitiveServicesOpenAIContributor

In .NET Aspire 9.4, the default role assigned to applications using `AddAzureOpenAI` was changed from `CognitiveServicesOpenAIContributor` to `CognitiveServicesOpenAIUser`. This change improves security by assigning a lower privilege role by default, ensuring applications only have the permissions necessary for inference tasks.

## Version introduced

.NET Aspire 9.4

## Previous behavior

Previously, applications referencing an Azure OpenAI account were assigned as the `CognitiveServicesOpenAIContributor` role by default. This role allowed applications to manage OpenAI deployments, which is a higher privilege than typically required for inference tasks.

## New behavior

Applications referencing an Azure OpenAI account are now assigned the `CognitiveServicesOpenAIUser` role by default. This role provides permissions for inference tasks without allowing management of OpenAI deployments. If higher privileges are required, you can configure the necessary roles using the `WithRoleAssignments` API.

Example:

```csharp
using Azure.Provisioning.CognitiveServices;

var openai = builder.AddAzureOpenAI("openai");

builder.AddProject<Projects.ApiService>("api")
       .WithRoleAssignments(openai, CognitiveServicesBuiltInRole.CognitiveServicesOpenAIContributor);
```

## Type of breaking change

This is a [behavioral change](../categories.md#behavioral-change).

## Reason for change

The `CognitiveServicesOpenAIContributor` role provides excessive privileges for most applications, as managing OpenAI deployments isn't typically required. Assigning the `CognitiveServicesOpenAIUser` role by default enhances security by limiting permissions to inference tasks. For applications requiring higher privileges, roles can be explicitly configured using the <xref:Aspire.Hosting.AzureOpenAIExtensions.WithRoleAssignments*> API.

For more information, see [GitHub PR #10293](https://github.com/dotnet/aspire/pull/10293).

## Recommended action

If your application requires higher privileges than the `CognitiveServicesOpenAIUser` role, explicitly configure the necessary roles using the `WithRoleAssignments` API. See the [New behavior](#new-behavior) section for an example of how to do this.

## Affected APIs

- `Aspire.Hosting.AzureOpenAIExtensions.AddAzureOpenAI`
