---
title: "Breaking change - WithAccessKeyAuthentication and WithPasswordAuthentication create a keyvault resource in the app model"
description: "Learn about the breaking change in .NET Aspire 9.2 where key vault resources are now created or referenced directly in the app model."
ms.date: 3/25/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/2889
---

# With authentication API creates keyvault resource in the app model

Starting in .NET Aspire 9.2, calling any of the following methods:

- <xref:Aspire.Hosting.AzureRedisExtensions.WithAccessKeyAuthentication*?displayProperty=nameWithType>
- <xref:Aspire.Hosting.AzureCosmosExtensions.WithAccessKeyAuthentication*?displayProperty=nameWithType>
- <xref:Aspire.Hosting.AzurePostgresExtensions.WithPasswordAuthentication*?displayProperty=nameWithType>

Will now create (or add references to) a key vault resource directly in the app model. This change allows better customization and management of connection strings and secrets.

## Version introduced

.NET Aspire 9.2

## Previous behavior

Previously, calling `WithAccessKeyAuthentication` on CosmosDB or AzureRedis, or `WithPasswordAuthentication` on AzurePostgres, automatically created and managed Bicep resources. These resources were invisible to the app model and could not be managed or customized in C#.

## New behavior

In .NET Aspire 9.2, calling `WithAccessKeyAuthentication` or `WithPasswordAuthentication` adds an empty `keyVaultName` parameter as a known parameter in the Bicep file. The app model now directly creates the key vault resource or allows you to pass a reference to an existing AzureKeyVault resource where secrets will be stored. Key vault secret names for connection strings are now formatted as `connectionstrings--{resourcename}` to avoid conflicts with other connection strings.

## Type of breaking change

This is a [behavioral change](../categories.md#behavioral-change).

## Reason for change

This change moves resource management to the app host, making it easier to customize and manage resources. It allows sharing a key vault across multiple resources or using an existing key vault to manage connection strings and secrets.

## Recommended action

There is currently no workaround for this change. Ensure that your app model is updated to handle the new behavior for key vault resources and connection string management.

## Affected APIs

- <xref:Aspire.Hosting.AzureRedisExtensions.WithAccessKeyAuthentication*?displayProperty=nameWithType>
- <xref:Aspire.Hosting.AzureCosmosExtensions.WithAccessKeyAuthentication*?displayProperty=nameWithType>
- <xref:Aspire.Hosting.AzurePostgresExtensions.WithPasswordAuthentication*?displayProperty=nameWithType>
