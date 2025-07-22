---
title: GitHub Models integration
description: Learn how to integrate .NET Aspire with GitHub Models for AI model access and management.
ms.date: 07/22/2025
ai-usage: ai-assisted
---

# .NET Aspire GitHub Models integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[GitHub Models](https://docs.github.com/github-models) provides access to various AI models including OpenAI's GPT models, DeepSeek, Microsoft's Phi models, and other leading AI models, all accessible through GitHub's infrastructure. The .NET Aspire GitHub Models integration enables you to connect to GitHub Models from your .NET applications for prototyping and production scenarios.

## Hosting integration

The .NET Aspire [GitHub Models](https://docs.github.com/github-models) hosting integration models GitHub Models resources as `GitHubModelResource`. To access these types and APIs for expressing them within your [AppHost project](../fundamentals/app-host-overview.md), install the [📦 Aspire.Hosting.GitHub.Models](https://www.nuget.org/packages/Aspire.Hosting.GitHub.Models) NuGet package:

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.GitHub.Models
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.GitHub.Models"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add a GitHub Model resource

To add a `GitHubModelResource` to your app host project, call the `AddGitHubModel` method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var chat = builder.AddGitHubModel("chat", "openai/gpt-4o-mini");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(chat);

// After adding all resources, run the app...
```

The preceding code adds a GitHub Model resource named `chat` using the `openai/gpt-4o-mini` model. The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference*> method passes the connection information to the `ExampleProject` project.

### Specify an organization

For organization-specific requests, you can specify an organization parameter:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var organization = builder.AddParameter("github-org");
var chat = builder.AddGitHubModel("chat", "openai/gpt-4o-mini", organization);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(chat);

// After adding all resources, run the app...
```

When an organization is specified, the token must be attributed to that organization.

### Configure API key authentication

The GitHub Models integration supports multiple ways to configure authentication:

#### Default API key parameter

By default, the integration creates a parameter named `{resource_name}-gh-apikey` that automatically falls back to the `GITHUB_TOKEN` environment variable:

```csharp
var chat = builder.AddGitHubModel("chat", "openai/gpt-4o-mini");
```

Then in user secrets:

```json
{
  "Parameters": {
    "chat-gh-apikey": "YOUR_GITHUB_TOKEN_HERE"
  }
}
```

#### Custom API key parameter

You can also specify a custom parameter for the API key:

```csharp
var apiKey = builder.AddParameter("my-api-key", secret: true);
var chat = builder.AddGitHubModel("chat", "openai/gpt-4o-mini")
                  .WithApiKey(apiKey);
```

Then in user secrets:

```json
{
  "Parameters": {
    "my-api-key": "YOUR_GITHUB_TOKEN_HERE"
  }
}
```

### Health checks

You can add health checks to verify the GitHub Models endpoint accessibility and API key validity:

```csharp
var chat = builder.AddGitHubModel("chat", "openai/gpt-4o-mini")
                  .WithHealthCheck();
```

> [!IMPORTANT]
> Because health checks are included in the rate limit of the GitHub Models API, use this health check sparingly, such as when debugging connectivity issues. The health check runs only once per application instance to minimize API usage.

### Available models

GitHub Models supports various AI models. Some popular options include:

- `openai/gpt-4o-mini`
- `openai/gpt-4o`
- `deepseek/DeepSeek-V3-0324`
- `microsoft/Phi-4-mini-instruct`

Check the [GitHub Models documentation](https://docs.github.com/github-models) for the most up-to-date list of available models.

## Client integration

To get started with the .NET Aspire GitHub Models client integration, you can use either the Azure AI Inference client or the OpenAI client, depending on your needs and model compatibility.

### Using Azure AI Inference client

Install the [📦 Aspire.Azure.AI.Inference](https://www.nuget.org/packages/Aspire.Azure.AI.Inference) NuGet package in the client-consuming project:

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

#### Add a ChatCompletionsClient

In the _Program.cs_ file of your client-consuming project, use the `AddAzureChatCompletionsClient` method to register a `ChatCompletionsClient` for dependency injection:

```csharp
builder.AddAzureChatCompletionsClient("chat");
```

You can then retrieve the `ChatCompletionsClient` instance using dependency injection:

```csharp
public class ExampleService(ChatCompletionsClient client)
{
    public async Task<string> GetResponseAsync(string prompt)
    {
        var response = await client.GetChatCompletionsAsync(
            "openai/gpt-4o-mini",
            new[]
            {
                new ChatMessage(ChatRole.User, prompt)
            });
        
        return response.Value.Choices[0].Message.Content;
    }
}
```

#### Add ChatCompletionsClient with registered IChatClient

If you're using the Microsoft.Extensions.AI abstractions, you can register an `IChatClient`:

```csharp
builder.AddAzureChatCompletionsClient("chat")
       .AddChatClient();
```

Then use it in your services:

```csharp
public class StoryService(IChatClient chatClient)
{
    public async Task<string> GenerateStoryAsync(string prompt)
    {
        var response = await chatClient.GetResponseAsync(prompt);

        return response.Text;
    }
}
```

### Using OpenAI client

For models compatible with the OpenAI API (such as `openai/gpt-4o-mini`), you can use the OpenAI client. Install the [📦 Aspire.OpenAI](https://www.nuget.org/packages/Aspire.OpenAI) NuGet package:

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.OpenAI
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.OpenAI"
                  Version="*" />
```

---

#### Add an OpenAI client

```csharp
builder.AddOpenAIClient("chat");
```

You can then use the OpenAI client:

```csharp
public class ChatService(OpenAIClient client)
{
    public async Task<string> GetChatResponseAsync(string prompt)
    {
        var chatClient = client.GetChatClient("openai/gpt-4o-mini");

        var response = await chatClient.CompleteChatAsync(
            new[]
            {
                new UserChatMessage(prompt)
            });
        
        return response.Value.Content[0].Text;
    }
}
```

#### Add OpenAI client with registered IChatClient

```csharp
builder.AddOpenAIClient("chat")
       .AddChatClient();
```

### Configuration

The GitHub Models integration supports configuration through user secrets, environment variables, or app settings. The integration automatically uses the `GITHUB_TOKEN` environment variable if available, or you can specify a custom API key parameter.

#### Authentication

The GitHub Models integration requires a GitHub personal access token with `models: read` permission. The token can be provided in several ways:

##### Environment variables in Codespaces and GitHub Actions

When running an app in GitHub Codespaces or GitHub Actions, the `GITHUB_TOKEN` environment variable is automatically available and can be used without additional configuration. This token has the necessary permissions to access GitHub Models for the repository context.

```csharp
// No additional configuration needed in Codespaces/GitHub Actions
var chat = builder.AddGitHubModel("chat", "openai/gpt-4o-mini");
```

##### Personal access tokens for local development

For local development, you need to create a [fine-grained personal access token](https://github.com/settings/tokens) with the `models: read` scope and configure it in user secrets:

```json
{
  "Parameters": {
    "chat-gh-apikey": "github_pat_YOUR_TOKEN_HERE"
  }
}
```

#### Connection string format

The connection string follows this format:

```Plaintext
Endpoint=https://models.github.ai/inference;Key={api_key};Model={model_name};DeploymentId={model_name}
```

For organization-specific requests:

```Plaintext
Endpoint=https://models.github.ai/orgs/{organization}/inference;Key={api_key};Model={model_name};DeploymentId={model_name}
```

### Rate limits and costs

> [!IMPORTANT]
> Each model has rate limits that vary by model and usage tier. Some models include costs if you exceed free tier limits. Check the [GitHub Models documentation](https://docs.github.com/github-models) for current rate limits and pricing information.

> [!TIP]
> Use health checks sparingly to avoid consuming your rate limit allowance. The integration caches health check results to minimize API calls.

### Sample application

The `dotnet/aspire` repo contains an example application demonstrating the GitHub Models integration. You can find the sample in the [Aspire GitHub repository](https://github.com/dotnet/aspire/tree/main/playground/GitHubModelsEndToEnd).

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

### Logging

The GitHub Models integration uses standard HTTP client logging categories:

- `System.Net.Http.HttpClient`
- `Microsoft.Extensions.Http`

### Tracing

HTTP requests to the GitHub Models API are automatically traced when using the Azure AI Inference or OpenAI clients.

## See also

- [GitHub Models](https://docs.github.com/github-models)
- [.NET Aspire integrations overview](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
- [GitHub Models API documentation](https://docs.github.com/rest/models/inference)
