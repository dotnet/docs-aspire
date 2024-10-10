---
title: Overview
description: An overview of the .NET Aspire Community Toolkit project.
ms.date: 10/11/2024
---

# .NET Aspire Community Toolkit

The .NET Aspire Community Toolkit is a collection of integrations and extensions for .NET Aspire created by the community. These integrations aren't officially supported by the .NET Aspire team, and are provided as-is for the community to use and contribute to. The source code for the toolkit is available on [GitHub][github-repo].

## Frequently Asked Questions

The following section answers some frequently asked questions about the .NET Aspire Community Toolkit.

### What is the purpose of this project?

The goal of the project is to be a centralized home for extensions and integrations for [.NET Aspire](/dotnet/aspire), helping to provide consistency in the way that integrations are built and maintained, as well as easier discoverability for users.

### How is this project different from the official .NET Aspire project?

The .NET Aspire Community Toolkit is a community-driven project that's maintained by the community and isn't officially supported by the .NET Aspire team. The toolkit is a collection of integrations and extensions that are built on top of the .NET Aspire project.

### How can I contribute to the project?

Anyone can contribute to the .NET Aspire Community Toolkit and before you get started, be sure to read the [Contributing Guide](https://github.com/CommunityToolkit/Aspire/blob/main/CONTRIBUTING.md) to learn how to contribute to the project.

### Should I propose a new integration on the Community Toolkit or the `dotnet/aspire` repo?

If you have an idea for a new integration, you should propose it on the [.NET Aspire Community Toolkit repository][github-repo], rather than [`dotnet/aspire`](https://github.com/dotnet/aspire), as the official .NET Aspire project is focused on the core functionality of the .NET Aspire project.

If you've proposed an integration on the `dotnet/aspire` repository, you can still propose it in the Community Toolkit, but link to the existing issue on the `dotnet/aspire` repository to provide context.

### Finding Community Toolkit Integrations

Integrations from the .NET Aspire Community Toolkit appear in the **Add Aspire Integration** dialog in Visual Studio under the namespace `Aspire.CommunityToolkit.*`.

[github-repo]: https://github.com/CommunityToolkit/Aspire