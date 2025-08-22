---
title: Overview
description: An overview of the .NET Aspire Community Toolkit project.
ms.date: 01/22/2025
---

# .NET Aspire Community Toolkit

The .NET Aspire Community Toolkit is part of the [.NET Foundation](https://dotnetfoundation.org/projects/project-detail/.net-aspire-community-toolkit). The community toolkit is a collection of integrations and extensions for .NET Aspire created by the community. The .NET Aspire team doesn't officially support the integrations and extensions in the community toolkit. The community provides these tools as-is for everyone to use and contribute to. You can find the source code for the toolkit on [GitHub][github-repo].

## Why use the toolkit?

The community toolkit offers flexible, community-driven integrations that enhance the .NET Aspire ecosystem. By contributing, you help shape tools that make building cloud-native applications easier and more versatile.

## What's in the toolkit?

The community toolkit is a growing project, publishing a set of NuGet packages. It aims to provide various integrations, both hosting and client alike, that aren't otherwise part of the official .NET Aspire project. Additionally, the community toolkit packages various extensions for popular services and platforms. The following sections detail some of the integrations and extensions currently available in the toolkit.

### Hosting integrations

- The [Azure Data API Builder](/azure/data-api-builder/overview) integration enables seamless API creation for your data:
  - [📄 .NET Aspire Azure Data API Builder integration](https://github.com/CommunityToolkit/Aspire/tree/main/src/CommunityToolkit.Aspire.Hosting.Azure.DataApiBuilder).
  - [📦 CommunityToolkit.Aspire.Hosting.Azure.DataApiBuilder](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.Azure.DataApiBuilder).
- The [Bun](https://bun.sh) integration provides support for hosting Bun applications:
  - [📄 .NET Aspire Bun hosting integration](hosting-bun.md).
  - [📦 CommunityToolkit.Aspire.Hosting.Bun](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.Bun).
- The [Golang apps](https://go.dev/) integration provides support for hosting Go applications:
  - [📄 .NET Aspire Go integration](hosting-golang.md).
  - [📦 CommunityToolkit.Aspire.Hosting.Golang](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.Golang).
- The [Java](https://www.java.com/) integration runs Java code with a local Java Development Kit (JDK) or using a container:
  - [📄 .NET Aspire Java/Spring hosting integration](hosting-java.md).
  - [📦 CommunityToolkit.Aspire.Hosting.Java](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.Java).
- The [Deno](https://deno.com/) integration provides support for hosting Deno applications and running tasks.
  - [📄 .NET Aspire Deno hosting integration](https://github.com/CommunityToolkit/Aspire/tree/main/src/CommunityToolkit.Aspire.Hosting.Deno).
  - [📦 CommunityToolkit.Aspire.Hosting.Deno](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.Deno).
- The [Ollama](https://ollama.com/) integration provides extensions and resource definitions, and support for downloading models as startup.
  - [📄 .NET Aspire Ollama hosting integration](ollama.md#hosting-integration).
  - [📦 CommunityToolkit.Aspire.Hosting.Ollama](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.Ollama).
- The [Meilisearch](https://www.meilisearch.com) integration enables hosting Meilisearch containers.
  - [📄 .NET Aspire Meilisearch hosting integration](hosting-meilisearch.md#hosting-integration).
  - [📦 CommunityToolkit.Aspire.Hosting.Meilisearch](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.Meilisearch).
- The [MailPit](https://mailpit.axllent.org/) integration provides a fake SMTP server for mail testing during development:
  - [📄 .NET Aspire MailPit hosting integration](hosting-mailpit.md).
  - [📦 CommunityToolkit.Aspire.Hosting.MailPit](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.MailPit).
- The [Rust apps](https://www.rust-lang.org/) integration provides support for hosting Rust applications.
  - [📄 .NET Aspire Rust hosting integration](hosting-rust.md#hosting-integration).
  - [📦 CommunityToolkit.Aspire.Hosting.Rust](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.Rust).
- The [SQLite](https://www.sqlite.org/index.html) integration provides support for hosting SQLite databases.
  - [📄 .NET Aspire SQLite hosting integration](sqlite.md#hosting-integration)
  - [📦 CommunityToolkit.Aspire.Hosting.SQLite](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.SQLite).
- The [RavenDB](https://ravendb.net/) integration enables hosting RavenDB containers.
  - [📄 .NET Aspire RavenDB hosting integration](https://github.com/CommunityToolkit/Aspire/tree/main/src/CommunityToolkit.Aspire.Hosting.RavenDB).
  - [📦 CommunityToolkit.Aspire.Hosting.RavenDB](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.RavenDB).

### Client integrations

The following client integrations are available in the toolkit:

- **OllamaSharp** is a .NET client for the Ollama API:
  - [📄 .NET Aspire Ollama client integration](ollama.md#client-integration)
  - [📦 CommunityToolkit.Aspire.OllamaSharp](https://nuget.org/packages/CommunityToolkit.Aspire.OllamaSharp)
- **Meilisearch** is a .NET client for the Meilisearch API:
  - [📄 .NET Aspire Meilisearch client integration](hosting-meilisearch.md#client-integration)
  - [📦 CommunityToolkit.Aspire.Meilisearch](https://nuget.org/packages/CommunityToolkit.Aspire.Meilisearch)
- The [SQLite](https://www.sqlite.org/index.html) integration provides support for hosting SQLite databases.
  - [📄 .NET Aspire SQLite hosting integration](sqlite.md#client-integration)
  - [📦 CommunityToolkit.Aspire.Hosting.SQLite](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.SQLite).
- The [SQLite Entity Framework](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite) integration provides support for hosting SQLite databases with Entity Framework.
  - [📄 .NET Aspire SQLite EF hosting integration](sqlite-entity-framework-integration.md#client-integration)
  - [📦 CommunityToolkit.Aspire.Microsoft.EntityFrameworkCore.Sqlite](https://nuget.org/packages/CommunityToolkit.Aspire.Microsoft.EntityFrameworkCore.Sqlite).
- **RavenDB.Client** is a .NET client for the RavenDB Client API:
  - [📄 .NET Aspire RavenDB client integration](https://github.com/CommunityToolkit/Aspire/tree/main/src/CommunityToolkit.Aspire.RavenDB.Client)
  - [📦 CommunityToolkit.Aspire.RavenDB.Client](https://nuget.org/packages/CommunityToolkit.Aspire.RavenDB.Client)

> [!TIP]
> Always check the [GitHub repository][github-repo] for the most up-to-date information on the toolkit.

### Extensions

To expand the functionality provided by the .NET Aspire integrations, the Community Toolkit also provides extension packages for some hosting integrations. The following extensions are available in the toolkit:

- [📦 CommunityToolkit.Aspire.Hosting.NodeJS.Extensions](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.NodeJS.Extensions)
  - [📄 Docs](hosting-nodejs-extensions.md)
- [📦 CommunityToolkit.Aspire.Hosting.SqlServer.Extensions](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.SqlServer.Extensions)
  - [📄 Docs](hosting-sqlserver-extensions.md)
- [📦 CommunityToolkit.Aspire.Hosting.PostgreSQL.Extensions](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.PostgreSQL.Extensions)
  - [📄 Docs](hosting-postgresql-extensions.md)
- [📦 CommunityToolkit.Aspire.Hosting.Redis.Extensions](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.Redis.Extensions)
  - [📄 Docs](hosting-redis-extensions.md)
- [📦 CommunityToolkit.Aspire.Hosting.MongoDB.Extensions](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.MongoDB.Extensions)
  - [📄 Docs](hosting-mongodb-extensions.md)
- [📦 CommunityToolkit.Aspire.Hosting.MySql.Extensions](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.MySql.Extensions)
  - [📄 Docs](hosting-mysql-extensions.md)

If you're not seeing an integration or extension you need, you can contribute to the toolkit by creating your own integration and submitting a pull request. For more information, see [How to collaborate](#how-to-collaborate).

## How to collaborate

The community toolkit is an open-source project, and contributions from the community aren't only welcomed, but encouraged. If you're interested in contributing, see the [contributing guidelines](https://github.com/CommunityToolkit/Aspire/blob/main/CONTRIBUTING.md). As part of the .NET Foundation, contributors of the toolkit must adhere to the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/about/policies/code-of-conduct).

[github-repo]: https://github.com/CommunityToolkit/Aspire
