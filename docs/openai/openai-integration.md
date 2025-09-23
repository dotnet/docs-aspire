---
title: .NET Aspire OpenAI integration (Preview)
description: Learn how to integrate OpenAI models into your .NET Aspire applications using the built-in hosting and client support.
ms.date: 09/23/2025
---

# .NET Aspire OpenAI integration (Preview)

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[OpenAI](https://openai.com/) provides access to chat/completions, embeddings, image, and audio models via a REST API. The .NET Aspire OpenAI integration lets you:

- Model an OpenAI account (endpoint + API key) once in the AppHost.
- Add one or more model resources that compose their connection strings from the parent.
- Reference those model resources from projects to get strongly‑named connection strings.
- Consume those connection strings with the `Aspire.OpenAI` component to obtain an `OpenAIClient` and (optionally) an `IChatClient`.

## Hosting integration

The hosting integration models OpenAI with two resource types:

- `OpenAIResource`: Parent that holds the shared API key and base endpoint (defaults to `https://api.openai.com/v1`).
- `OpenAIModelResource`: Child representing a specific model; composes a connection string from the parent (`Endpoint` + `Key` + `Model`).

To access these types and APIs for expressing them within your [AppHost](xref:dotnet/aspire/app-host) project, install the [📦 Aspire.Hosting.OpenAI](https://www.nuget.org/packages/Aspire.Hosting.OpenAI) NuGet package:

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.OpenAI
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.OpenAI"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add an OpenAI parent resource

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var openai = builder.AddOpenAI("openai");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(openai);

// After adding all resources, run the app...
```

### Add OpenAI model resources

Add one or more model children beneath the parent and reference them from projects:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var openai = builder.AddOpenAI("openai");

var chat = openai.AddModel("chat", "gpt-4o-mini");
var embeddings = openai.AddModel("embeddings", "text-embedding-3-small");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(chat);

// After adding all resources, run the app...
```

Referencing `chat` passes a connection string named `chat` to the project. Multiple models can share the single API key and endpoint via the parent resource.

### Use default API key parameter

Calling `AddOpenAI("openai")` creates a secret parameter named `openai-openai-apikey`. Aspire resolves its value in this order:

1. The `Parameters:openai-openai-apikey` configuration key (user secrets, `appsettings.*`, or environment variables).
1. The `OPENAI_API_KEY` environment variable.

If neither source provides a value, startup throws a <xref:Aspire.Hosting.MissingParameterValueException>. Set one of the values to avoid the exception.

Provide the key via user-secrets:

```dotnetcli
dotnet user-secrets set Parameters:openai-openai-apikey sk-your-api-key
```

### Use custom API key parameter

Replace the default parameter by creating your own secret parameter and calling `WithApiKey` on the parent:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var apiKey = builder.AddParameter("my-api-key", secret: true);

var openai = builder.AddOpenAI("openai")
                    .WithApiKey(apiKey);

var chat = openai.AddModel("chat", "gpt-4o-mini");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(chat);

// After adding all resources, run the app...
```

The original generated parameter is removed from the resource graph when replaced. Custom parameters must be marked `secret: true`.

### Add a custom endpoint

Override the default endpoint (for example to use a proxy or compatible gateway):

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var openai = builder.AddOpenAI("openai")
                    .WithEndpoint("https://my-gateway.example.com/v1");

var chat = openai.AddModel("chat", "gpt-4o-mini");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(chat);

// After adding all resources, run the app...
```

Both parent and model connection strings include the custom endpoint.

### Health checks

Add an optional single‑run health check per model when diagnosing issues:

```csharp
var chat = builder.AddOpenAI("openai")
                  .AddModel("chat", "gpt-4o-mini")
                  .WithHealthCheck();
```

The model health check validates endpoint reachability, API key validity (401), and model existence (404). It executes only once per application instance to limit rate‑limit implications. A status‑page check against `https://status.openai.com/api/v2/status.json` is automatically registered for each parent resource.

### Available models

Common identifiers:

- `gpt-5`
- `gpt-4o-mini`
- `gpt-4o`
- `gpt-4-turbo`
- `gpt-realtime`
- `text-embedding-3-small`
- `text-embedding-3-large`
- `dall-e-3`
- `whisper-1`

> [!NOTE]
> The model name is case-insensitive, but we usually write it in lowercase.

For more information, see the [OpenAI models documentation](https://platform.openai.com/docs/models).

## Client integration

To get started with the Aspire OpenAI client integration, install the [📦 Aspire.OpenAI](https://www.nuget.org/packages/Aspire.OpenAI) NuGet package in the client-consuming project, that is, the project for the application that uses the OpenAI client.

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

### Add an OpenAI client

In the _Program.cs_ file of your client-consuming project, use the `AddOpenAIClient` to register an `OpenAIClient` for dependency injection (DI). The `AddOpenAIClient` method requires a connection name parameter.

```csharp
builder.AddOpenAIClient(connectionName: "chat");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the Azure OpenAI resource in the AppHost project. For more information, see [Add an OpenAI parent resource](#add-an-openai-parent-resource) or [Add OpenAI model resources](#add-openai-model-resources).

After adding the `OpenAIClient`, you can retrieve the client instance using dependency injection:

```csharp
public class ExampleService(OpenAIClient client)
{
    // Use client...
}
```

#### Add OpenAI client with registered IChatClient

```csharp
builder.AddOpenAIClient("chat")
       .AddChatClient(); // Model inferred from connection string (Model=...)
```

If only a parent resource was defined (no child model), provide the model name explicitly:

```csharp
builder.AddOpenAIClient("openai")
       .AddChatClient("gpt-4o-mini");
```

`AddChatClient` optionally accepts a model/deployment name; if omitted it comes from the connection string's `Model` entry. Inject `OpenAIClient` or `IChatClient` as needed.

### Configuration

The Aspire OpenAI library provides multiple options to configure the OpenAI connection based on the requirements and conventions of your project. Either a `Endpoint` or a `ConnectionString` is required to be supplied.

#### Use a connection string

Resolved connection string shapes:

Parent (no model):

```console
Endpoint={endpoint};Key={api_key}
```

Model child:

```console
Endpoint={endpoint};Key={api_key};Model={model_name}
```

#### Use configuration providers

Configure via `Aspire:OpenAI` keys (global) and `Aspire:OpenAI:{connectionName}` (per named client). Supported settings include `Key`, `Endpoint`, `DisableTracing`, `DisableMetrics`, and the `ClientOptions` subtree (`UserAgentApplicationId`, `OrganizationId`, `ProjectId`, `NetworkTimeout`, logging options, etc.).

```json
{
  "ConnectionStrings": {
    "chat": "Endpoint=https://api.openai.com/v1;Key=${OPENAI_API_KEY};Model=gpt-4o-mini"
  },
  "Aspire": {
    "OpenAI": {
      "DisableTracing": false,
      "DisableMetrics": false,
      "ClientOptions": {
        "UserAgentApplicationId": "myapp",
        "NetworkTimeout": "00:00:30"
      }
    }
  }
}
```

Inline configuration:

```csharp
builder.AddOpenAIClient("chat", settings => settings.DisableTracing = true);
builder.AddOpenAIClient("chat", configureOptions: o => o.NetworkTimeout = TimeSpan.FromSeconds(30));
```

Telemetry (traces + metrics) is experimental in the OpenAI .NET SDK. Enable globally via the `OpenAI.Experimental.EnableOpenTelemetry` AppContext switch or `OPENAI_EXPERIMENTAL_ENABLE_OPEN_TELEMETRY=true`. Use `DisableTracing` / `DisableMetrics` to opt out when enabled.

### Sample application

Explore the end-to-end sample that wires up the hosting and client integrations, sets the API key by parameter, registers a chat client, and performs a simple prompt/response round-trip. Clone the repo, run it, then adapt it for your models: <https://github.com/dotnet/aspire/tree/main/playground/OpenAIEndToEnd>.

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

#### Logging

- `OpenAI.*`

#### Tracing

- `OpenAI.*` (when telemetry enabled and not disabled)

#### Metrics

- `OpenAI.*` meter (when telemetry enabled and not disabled)

## See also

- [OpenAI documentation](https://platform.openai.com/docs)
- [OpenAI .NET SDK](https://github.com/openai/openai-dotnet)
- [.NET Aspire integrations overview](/dotnet/aspire/fundamentals/integrations-overview)
- [.NET Aspire GitHub Models integration](/dotnet/aspire/github/github-models-integration)
