---
title: Aspire AI integrations compatibility matrix
description: Learn which AI client integrations are compatible with which AI hosting integrations in .NET Aspire.
ms.date: 01/23/2025
ai-usage: ai-generated
---

# Aspire AI integrations compatibility matrix

.NET Aspire provides several AI hosting and client integrations that enable you to work with different AI services and platforms. This article provides a compatibility matrix showing which client integrations work with which hosting integrations, along with guidance on the recommended pairings.

## Compatibility matrix

The following table shows the compatibility between Aspire AI hosting and client integrations:

| **Hosting Integration**                    | **Aspire.OpenAI**            | **Aspire.Azure.AI.OpenAI**                    | **Aspire.Azure.AI.Inference** |
|--------------------------------------------|------------------------------|-----------------------------------------------|-------------------------------|
| `Aspire.Hosting.Azure.AI.Foundry`          | ❌ No                       | ⚠️ Partial                                    | ✅ Yes (preferred)           |
| `Aspire.Hosting.Azure.CognitiveServices`   | ❌ No                       | ✅ Yes (preferred)                            | ❌ No                        |
| `Aspire.Hosting.OpenAI`                    | ✅ Yes (preferred)          | ✅ Yes                                        | ❌ No                        |
| `Aspire.Hosting.GitHub.Models`              | ⚠️ Partial                  | ❌ No                                         | ✅ Yes (preferred)           |

### Legend

- ✅ **Yes**: The client integration is compatible with the hosting integration
- ⚠️ **Partial**: The client integration might not support all models or APIs
- ❌ **No**: The client integration does not support the hosting environment
- **(preferred)**: The recommended client integration for this hosting integration

## Recommended pairings

In general, use **Aspire.Azure.AI.Inference** to connect to Azure hosted models, or **Aspire.OpenAI** to connect directly to OpenAI.

### Azure AI Foundry

For Azure AI Foundry resources, use the **Aspire.Azure.AI.Inference** client integration. This provides the best compatibility with the diverse range of models available through Azure AI Foundry.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var foundry = builder.AddAzureAIFoundry("foundry");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(foundry);
```

In your service, use the Azure AI Inference client:

```csharp
builder.AddAzureChatCompletionsClient("foundry");
```

### Azure Cognitive Services (Azure OpenAI)

For Azure OpenAI resources, use the **Aspire.Azure.AI.OpenAI** client integration for full Azure-specific features and authentication support.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var openai = builder.AddAzureOpenAI("openai");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(openai);
```

In your service, use the Azure OpenAI client:

```csharp
builder.AddAzureOpenAIClient("openai");
```

### Direct OpenAI

For direct OpenAI API access, use the **Aspire.OpenAI** client integration.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var openai = builder.AddOpenAI("openai");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(openai);
```

In your service, use the OpenAI client:

```csharp
builder.AddOpenAIClient("openai");
```

### GitHub Models

For GitHub Models, use the **Aspire.Azure.AI.Inference** client integration for the best compatibility with the GitHub Models API.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var chat = builder.AddGitHubModel("chat", "openai/gpt-4o-mini");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(chat);
```

In your service, use the Azure AI Inference client:

```csharp
builder.AddAzureChatCompletionsClient("chat");
```

## Connection string formats

Each hosting integration generates connection strings in different formats that are consumed by the client integrations.

### Hosting integration connection strings

#### Aspire.Hosting.Azure.AI.Foundry

**Azure:**

```
Endpoint={Endpoint};EndpointAIInference={AIFoundryApiEndpoint}models
```

**Foundry local:**

```
Endpoint={EmulatorServiceUri};Key={ApiKey}
```

**Deployment:**

```
{Parent};DeploymentId={DeploymentName};Model={DeploymentName}
```

#### Aspire.Hosting.Azure.CognitiveServices

**Deployment:**

```
{ConnectionString};Deployment={deploymentName}
```

#### Aspire.Hosting.OpenAI

```
Endpoint={Endpoint};Key={Key};Model={Model}
```

#### Aspire.Hosting.GitHub.Models

```
Endpoint=https://models.github.ai/inference;Key={Key};Model={Model};DeploymentId={Model}
```

### Client integration connection string requirements

#### Aspire.OpenAI

Expects connection strings in the format:

```
Endpoint={Endpoint};Key={Key};DeploymentId={DeploymentId};Deployment={Deployment};Model={Model}
```

Uses either `Deployment` or `Model` (in that order). `Deployment` is set by `Aspire.Hosting.Azure.CognitiveServices` while `Model` is set by `Aspire.Hosting.OpenAI`.

#### Aspire.Azure.AI.OpenAI

Expects connection strings in the format:

```
Endpoint={Endpoint};Key={Key};Deployment={Deployment};Model={Model}
```

Uses either `Deployment` or `Model` (in that order). `Deployment` is set by `Aspire.Hosting.Azure.CognitiveServices` while `Model` is set by `Aspire.Hosting.OpenAI`.

This integration is a superset of `Aspire.OpenAI` and supports `TokenCredential` and Azure-specific features.

#### Aspire.Azure.AI.Inference

Expects connection strings in the format:

```
Endpoint={Endpoint};EndpointAIInference={EndpointAIInference};Key={Key};DeploymentId={DeploymentId}
```

Uses `EndpointAIInference` if available, otherwise `Endpoint`.

## See also

- [Azure AI Foundry integration](azureai-foundry-integration.md)
- [Azure OpenAI integration](azureai-openai-integration.md)
- [Azure AI Inference integration](azureai-inference-integration.md)
- [GitHub Models integration](../github/github-models-integration.md)
- [.NET Aspire integrations overview](../fundamentals/integrations-overview.md)
