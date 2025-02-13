---
title: Cosmos DB hosting integration obsolete API and default managed identity support
description: Breaking changes in Aspire.Hosting.Azure.CosmosDB regarding obsolete methods and default managed identity support.
ms.date: 02/13/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/2428
---

# Cosmos DB hosting integration obsolete API and default managed identity support

In .NET Aspire 9.1, the `AddDatabase()` method is obsolete and replaced by `WithDatabase()`. Additionally, `AddAzureCosmosDB()` now uses Managed Identity by default during provisioning instead of creating a Key Vault instance with a random access key.

## Version introduced

.NET Aspire 9.1

## Previous behavior

Previously, the `AddDatabase()` method was used to add a database. The `AddAzureCosmosDB()` method created a Key Vault instance with a random access key by default.

## New behavior

The `AddDatabase()` method is now obsolete and replaced by `WithDatabase()`. Consider the following example that uses the new API:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cosmosdb = builder.AddAzureCosmosDB("cosmos")
                      .WithDatabase("database");
```

The `AddAzureCosmosDB()` method now uses Managed Identity by default. To revert to the previous behavior's use of key-based authentication, call `WithAccessKeyAuthentication()`:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cosmosdb = builder.AddAzureCosmosDB("cosmos")
                      .WithAccessKeyAuthentication();
```

## Type of breaking change

This change is a [source incompatible](../categories.md#source-incompatible) and [behavioral change](../categories.md#behavioral-change).

## Reason for change

The change follows the new API pattern when an existing resource is returned instead of a new resource (`Add` vs `With`). It also enhances security by using token credentials instead of secrets in connection strings. For more information, see [Add support for managed identity in Azure Cosmos DB hosting component](https://github.com/dotnet/aspire/pull/7092).

## Recommended action

Use `WithDatabase()` instead of `AddDatabase()`. Update applications to use token credentials instead of secrets in connection strings.

## Affected APIs

- `Aspire.Hosting.AzureCosmosExtensions.AddDatabase`
