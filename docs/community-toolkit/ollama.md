---
title: Ollama hosting
author: aaronpowell
description: A .NET Aspire hosting integration for hosting Ollama models using the Ollama container.
ms.date: 10/11/2024
---

# .NET Aspire Community Toolkit Ollama hosting integration

[!INCLUDE [banner](includes/banner.md)]

In this article, you'll learn how to use the .NET Aspire Ollama hosting integration to host [Ollama](https://ollama.com) models using the Ollama container.

## Getting Started

To get started with the .NET Aspire Ollama hosting integration, install the [Aspire.CommunityToolkit.Hosting.Ollama](https://github.com/orgs/CommunityToolkit/packages/nuget/package/Aspire.CommunityToolkit.Hosting.Ollama) NuGet package in the AppHost project.

[!INCLUDE [github-packages](includes/github-packages.md)]

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.CommunityToolkit.Hosting.Ollama
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.CommunityToolkit.Hosting.Ollama"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _:::no-loc text="Program.cs":::_ file of your projects, call the `AddOllama` extension method to add the Ollama container to the application builder. You can then add models to the container, which will be downloaded and run when the container starts, using the `AddModel` extension method.

```csharp
var ollama = builder.AddOllama("ollama")
                    .AddModel("llama3");
```

### Downloading the LLM

When the Ollama container for this integration first spins up, this integration will download the LLM(s). The progress of this download will be displayed in the State column for this integration on the Aspire orchestration app.

> [!IMPORTANT]
> Keep the .NET Aspire orchestration app open until the download is complete, otherwise the download will be cancelled.

### Caching the LLM

The LLM(s) will be downloaded into the container which Ollama is running from, and by default this container is ephemeral. If you need to persist the LLM(s) across container restarts, you will need to mount a volume into the container using the `AddDataVolume` method.

```csharp
var ollama = builder.AddOllama("ollama")
                    .AddModel("llama3")
                    .AddDataVolume();
```

## Accessing the Ollama server in other services

The Ollama hosting integration exposes the endpoint of the Ollama server as a connection string that can be accessed from other services in the application.

```csharp
var connectionString = builder.Configuration.GetConnectionString("ollama");
```

## Open WebUI support

The Ollama integration also provided support for running [Open WebUI](https://openwebui.com/) and having it communicate with the Ollama container.

```csharp
var ollama = builder.AddOllama("ollama")
                    .AddModel("llama3")
                    .WithOpenWebUI();
```

## See also

- [Ollama](https://ollama.com)
- [Open WebUI](https://openwebui.com)
- [.NET Aspire Community Toolkit GitHub repo](https://github.com/CommunityToolkit/Aspire)
