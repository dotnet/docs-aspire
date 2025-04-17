---
title: "Breaking change - AzureOpenAIDeployment obsolete"
description: "Learn about the breaking change in .NET Aspire 9.2 where Azure OpenAI deployments are now modeled as .NET Aspire child resources."
ms.date: 4/15/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/3059
---

# AzureOpenAIDeployment obsolete

In .NET Aspire 9.2, Azure OpenAI deployments are modeled as .NET Aspire child resources instead of simple objects. This change aligns with the design of other [hosting integrations](../../fundamentals/integrations-overview.md#hosting-integrations) and enables referencing deployments using <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference*>. As a result, the previous APIs for adding deployments are now obsolete.

## Version introduced

.NET Aspire 9.2

## Previous behavior

Previously, deployments were added as simple objects using the <xref:Aspire.Hosting.ApplicationModel.AzureOpenAIDeployment> class. For example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var openAI = builder.AddAzureOpenAI(openAIName);

openAI.AddDeployment(new AzureOpenAIDeployment("chat", "gpt-4o-mini", "2024-07-18"))
      .AddDeployment(new AzureOpenAIDeployment("embedding", "text-embedding-3-small", "1"));
```

## New behavior

Deployments are modeled as .NET Aspire child resources and are added using the new overloads. For example, consider the following code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var openAI = builder.AddAzureOpenAI(openAIName);

var chatModel = openAI.AddDeployment("chat", "gpt-4o-mini", "2024-07-18");
var embeddingModel = openAI.AddDeployment("embedding", "text-embedding-3-small", "1");
```

## Type of breaking change

This is a [source incompatible](../categories.md#source-compatibility) change.

## Reason for change

Modeling deployments as Aspire resources allows them to be referenced by other resources via `.WithReference`. This design eliminates the need to hard-code model names in .NET applications and enables configuration-based deployment name resolution. It also ensures consistency with other hosting integrations.

## Recommended action

Update your code to use the new overloads for adding deployments. Replace calls to the obsolete APIs with the new pattern. For example:

**Before:**

```csharp
openAI.AddDeployment(new AzureOpenAIDeployment("chat", "gpt-4o-mini", "2024-07-18"));
```

**After:**

```csharp
var chatModel = openAI.AddDeployment("chat", "gpt-4o-mini", "2024-07-18");
```

## Affected APIs

- <xref:Aspire.Hosting.AzureOpenAIExtensions.AddDeployment*>
