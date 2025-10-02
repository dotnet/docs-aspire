---
title: "Ollama integration updates"
description: Updates and breaking changes for Ollama integration in Aspire 9.0 GA.
ms.date: 11/4/2024
ai-usage: ai-assisted
---

# Ollama integration updates

The Aspire Community Toolkit 9.0 release of the Ollama integrations introduces significant changes. These updates enhance functionality and better integrate with the Aspire API design.

## Version introduced

Aspire Community Toolkit 9.0 GA

## Previous behavior

The Ollama integration includes both hosting and client integration updates, both are details in the following sections.

### Ollama hosting

The Ollama hosting resource had to be provided as the reference to other resources and it would provide model information as a set of environment variables. The connection string was only the HTTP endpoint and not in a real "connection string" format.

### OllamaSharp client

Supports v3 of the library and doesn't support the `Microsoft.Extensions.AI` (M.E.AI) interfaces.

## New behavior

The new behavior includes the following updates.

### Models as resources

In earlier versions, models were added to the Ollama resource, and you had to pass this resource as a reference. This approach required workarounds to set and discover the default model. In version 9.0, we introduced the `OllamaModelResource`. This resource can be passed as a reference and provides connection information to clients about which model to use.

### New connection string format

Originally the "connection string" from an Ollama resource was just the HTTP endpoint, but to support the _Model as resource_ feature better, the resources create a "real" connection string of `Endpoint=<...>;Model=<...>`. The `Model` part is only included if you're passing the `OllamaModelResource`.

### OllamaSharp 4 and Microsoft.Extensions.AI

OllamaSharp updated to a new major version and now supports the interfaces from [Microsoft.Extensions.AI](https://devblogs.microsoft.com/dotnet/introducing-microsoft-extensions-ai-preview/). You can register the `IOllamaApiClient` (native OllamaSharp client), or from M.E.AI <xref:Microsoft.Extensions.AI.IChatClient> and <xref:Microsoft.Extensions.AI.IEmbeddingGenerator`2> (depending on your model type). Using these new interfaces makes code more portable across LLM/SLM options.

### API deprecations and removals

With all the refactoring, some APIs are being deprecated or removed.

## Type of breaking change

This change is a [binary incompatible](../categories.md#binary-compatibility) and [behavioral change](../categories.md#behavioral-change).

## Reason for change

The changes aim to make the library more functional and better integrated with the Aspire API design.

## Recommended action

[Upgrade to Aspire 9.0](../../get-started/upgrade-to-aspire-9.md).

## Affected APIs

- [📦 CommunityToolkit.Aspire.Hosting.Ollama](https://www.nuget.org/packages/CommunityToolkit.Aspire.Hosting.Ollama)
- [📦 CommunityToolkit.Aspire.OllamaSharp](https://www.nuget.org/packages/CommunityToolkit.Aspire.OllamaSharp)
