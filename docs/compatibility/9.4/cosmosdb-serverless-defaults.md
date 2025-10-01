---
title: "Breaking change - Azure Cosmos DB now provisions serverless accounts by default"
description: "Learn about the breaking change in Aspire 9.4 where Azure Cosmos DB resources default to serverless accounts so they can scale to zero."
ms.date: 08/14/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/4244
---

# Azure Cosmos DB now provisions serverless accounts by default

Starting in Aspire 9.4, Azure Cosmos DB resources provision with the `EnableServerless` capability by default. New Cosmos DB accounts created by Aspire use the serverless model so they can scale to zero when idle.

## Version introduced

Aspire 9.4

## Previous behavior

Azure Cosmos DB accounts were created using the default, provisioned throughput capacity. Unless you explicitly enabled serverless, deployments used provisioned capacity.

Example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cosmos = builder.AddAzureCosmosDB("cosmos");
// Result: provisioned throughput (non-serverless)
```

## New behavior

By default, Azure Cosmos DB accounts are created with the `EnableServerless` capability so they can scale to zero.

Example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cosmos = builder.AddAzureCosmosDB("cosmos");
// Result: serverless (EnableServerless capability applied)
```

## Type of breaking change

This is a [behavioral change](../categories.md#behavioral-change).

## Reason for change

Serverless is recommended for dev/test workloads because it scales to zero cost when not in use. Aligning the default with serverless helps avoid unexpected charges and improves the out‑of‑box experience. See also the related deployment issue about duplicate capabilities when upgrading: <https://github.com/dotnet/aspire/issues/10808>.

## Recommended action

- To restore the previous behavior (provisioned throughput), call the default SKU extension on the Cosmos resource builder:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var provisionedCosmos = builder.AddAzureCosmosDB("cosmos")
                               .WithDefaultAzureSku();
```

- If you previously added the `EnableServerless` capability manually, remove that customization. Leaving it in place can result in duplicate capabilities and failed deployments when upgrading (see <https://github.com/dotnet/aspire/issues/10808>).

## Affected APIs

- <xref:Aspire.Hosting.AzureCosmosExtensions.AddAzureCosmosDB*?displayProperty=fullName>
- <xref:Aspire.Hosting.AzureCosmosExtensions.WithDefaultAzureSku*?displayProperty=fullName>
