---
title: .NET Aspire Community Toolkit Ollama integration
description: Learn how to use the .NET Aspire Ollama hosting and client integration to host Ollama models using the Ollama container and accessing it via the OllamaSharp client.
ms.date: 10/24/2024
---

# .NET Aspire Community Toolkit Ollama integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[!INCLUDE [banner](includes/banner.md)]

[Ollama](https://ollama.com) is a powerful, open source language model that can be used to generate text based on a given prompt. The .NET Aspire Ollama integration provides a way to host Ollama models using the [`docker.io/ollama/ollama` container image](https://hub.docker.com/r/ollama/ollama) and access them via the [OllamaSharp](https://www.nuget.org/packages/OllamaSharp) client.

## Hosting integration

The Ollama hosting integration models an Ollama server as the `OllamaResource` type, and provides the ability to add models to the server using the `AddModel` extension method, which represents the model as an `OllamaModelResource` type. To access these types and APIs that allow you to add the [📦 CommunityToolkit.Aspire.Hosting.Ollama](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.Ollama) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.Hosting.Ollama
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.Hosting.Ollama"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add Ollama resource

In the app host project, register and consume the Ollama integration using the `AddOllama` extension method to add the Ollama container to the application builder. You can then add models to the container, which downloads and run when the container starts, using the `AddModel` extension method.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var ollama = builder.AddOllama("ollama");

var phi35 = ollama.AddModel("phi3.5");

var exampleProject = builder.AddProject<Projects.ExampleProject>()
                            .WithReference(phi35);
```

Alternatively, if you want to use a model from the [Hugging Face](https://huggingface.co/) model hub, you can use the `AddHuggingFaceModel` extension method.

```csharp
var llama = ollama.AddHuggingFaceModel("llama", "bartowski/Llama-3.2-1B-Instruct-GGUF:IQ4_XS");
```

When .NET Aspire adds a container image to the app host, as shown in the preceding example with the `docker.io/ollama/ollama` image, it creates a new Ollama instance on your local machine. For more information, see [Container resource lifecycle](../fundamentals/app-host-overview.md#container-resource-lifecycle).

### Download the LLM

When the Ollama container for this integration first spins up, it downloads the configured LLMs. The progress of this download displays in the **State** column for this integration on the .NET Aspire dashboard.

> [!IMPORTANT]
> Keep the .NET Aspire orchestration app open until the download is complete, otherwise the download will be cancelled.

### Cache the LLM

One or more LLMs are downloaded into the container which Ollama is running from, and by default this container is ephemeral. If you need to persist one or more LLMs across container restarts, you need to mount a volume into the container using the `WithDataVolume` method.

```csharp
var ollama = builder.AddOllama("ollama")
                    .WithDataVolume();

var llama = ollama.AddModel("llama3");
```

### Use GPUs when available

One or more LLMs are downloaded into the container which Ollama is running from, and by default this container runs on CPU. If you need to run the container in GPU you need to pass a parameter to the container runtime args.

#### [Docker](#tab/docker)

```csharp
var ollama = builder.AddOllama("ollama")
                    .AddModel("llama3")
                    .WithContainerRuntimeArgs("--gpus=all");
```

For more information, see [GPU support in Docker Desktop](https://docs.docker.com/desktop/gpu/).

#### [Podman](#tab/podman)

```csharp
var ollama = builder.AddOllama("ollama")
                    .AddModel("llama3")
                    .WithContainerRuntimeArgs("--device", "nvidia.com/gpu=all");
```

For more information, see [GPU support in Podman](https://github.com/containers/podman/issues/19005).

---

### Hosting integration health checks

The Ollama hosting integration automatically adds a health check for the Ollama server and model resources. For the Ollama server, a health check is added to verify that the Ollama server is running and that a connection can be established to it. For the Ollama model resources, a health check is added to verify that the model is running and that the model is available, meaning the resource will be marked as unhealthy until the model has been downloaded.

### Open WebUI support

The Ollama integration also provided support for running [Open WebUI](https://openwebui.com/) and having it communicate with the Ollama container.

```csharp
var ollama = builder.AddOllama("ollama")
                    .AddModel("llama3")
                    .WithOpenWebUI();
```

## Client integration

To get started with the .NET Aspire OllamaSharp integration, install the [📦 CommunityToolkit.Aspire.OllamaSharp](https://nuget.org/packages/CommunityToolkit.Aspire.OllamaSharp) NuGet package in the client-consuming project, that is, the project for the application that uses the Ollama client.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.OllamaSharp
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.OllamaSharp"
                  Version="*" />
```

---

### Add Ollama client API

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the `AddOllamaClientApi` extension to register an `IOllamaClientApi` for use via the dependency injection container. If the resource provided in the app host, and referenced in the client-consuming project, is an `OllamaModelResource`, then the `AddOllamaClientApi` method will register the model as the default model for the `IOllamaClientApi`.

```csharp
builder.AddOllamaClientApi("llama3");
```

After adding `IOllamaClientApi` to the builder, you can get the `IOllamaClientApi` instance using dependency injection. For example, to retrieve your context object from service:

```csharp
public class ExampleService(IOllamaClientApi ollama)
{
    // Use ollama...
}
```

### Add keyed Ollama client API

There might be situations where you want to register multiple `IOllamaClientApi` instances with different connection names. To register keyed Ollama clients, call the `AddKeyedOllamaClientApi` method:

```csharp
builder.AddKeyedOllamaClientApi(name: "chat");
builder.AddKeyedOllamaClientApi(name: "embeddings");
```

Then you can retrieve the `IOllamaClientApi` instances using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(
    [FromKeyedServices("chat")] IOllamaClientApi chatOllama,
    [FromKeyedServices("embeddings")] IOllamaClientApi embeddingsOllama)
{
    // Use ollama...
}
```

### Configuration

The Ollama client integration provides multiple configuration approaches and options to meet the requirements and conventions of your project.

### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling the `AddOllamaClientApi` method:

```csharp
builder.AddOllamaClientApi("llama");
```

Then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "llama": "Endpoint=http//localhost:1234;Model=llama3"
  }
}
```

### Integration with `Microsoft.Extensions.AI`

The Microsoft.Extensions.AI library provides an abstraction over the Ollama client API, using generic interfaces. OllamaSharp supports these interfaces, and they can be registered using the `AddOllamaSharpChatClient` and `AddOllamaSharpEmbeddingGenerator` extension methods. These methods will also register the `IOllamaClientApi` instances with the dependency injection container, and have keyed versions for multiple instances.

```csharp
builder.AddOllamaSharpChatClient("llama");
```

After adding `IChatClient` to the builder, you can get the `IChatClient` instance using dependency injection. For example, to retrieve your context object from service:

```csharp
public class ExampleService(IChatClient chatClient)
{
    // Use chat client...
}
```

## See also

- [Ollama](https://ollama.com)
- [Open WebUI](https://openwebui.com)
- [.NET Aspire Community Toolkit GitHub repo](https://github.com/CommunityToolkit/Aspire)
- [OllamaSharp](https://github.com/awaescher/OllamaSharp)
- [Microsoft.Extensions.AI](https://devblogs.microsoft.com/dotnet/introducing-microsoft-extensions-ai-preview/)
