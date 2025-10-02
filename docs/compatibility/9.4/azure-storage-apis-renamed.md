---
title: "Breaking change - Azure Storage APIs renamed and refactored"
description: "Learn about the breaking changes in Aspire 9.4 where Azure Storage APIs were renamed and refactored for clarity and consistency."
ms.date: 07/08/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/3930
---

# Azure Storage APIs renamed and refactored

In Aspire 9.4, several Azure Storage APIs were renamed and refactored for clarity and consistency. These changes affect how you add and configure Azure Blob, Queue, and Table storage resources and clients in your Aspire applications. The new API names better align with Azure resource naming and reduce confusion.

## Version introduced

Aspire 9.4

## Previous behavior

Previously, you used `AddBlobs` to call `AddBlobContainer` to add a blob container:

**Hosting integration example:**

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage");

var blobs = storage.AddBlobs("blobs");
var blobContainer = blobs.AddBlobContainer("container");
```

Client registration methods also used names like `AddAzureBlobClient`, `AddAzureQueueClient`, and `AddAzureTableClient`.

**Client integration example:**

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddAzureBlobClient("storage");
builder.AddAzureQueueClient("storage");
builder.AddAzureTableClient("storage");
```

## New behavior

Now, `AddBlobContainer` is a top level call on the storage resource.

**Hosting integration example:**

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var blobContainer = storage.AddBlobContainer("container");
```

Client registration methods now use names like `AddAzureBlobServiceClient`, `AddAzureQueueServiceClient`, and `AddAzureTableServiceClient`.

**Client integration example:**

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddAzureBlobServiceClient("storage");
builder.AddAzureQueueServiceClient("storage");
builder.AddAzureTableServiceClient("storage");
```

### API changes summary

The following table summarizes the key hosting integration API changes:

| Obsolete API | New API | Notes |
|--|--|--|
| `AddBlobContainer` | `AddBlobContainer` | New API uses `IResourceBuilder<AzureStorageResource>` overload. |

The following table summarizes the key client registration API changes:

| Obsolete API | New API |
|--|--|
| `AddAzureBlobClient` | `AddAzureBlobServiceClient` |
| `AddAzureQueueClient` | `AddAzureQueueServiceClient` |
| `AddAzureTableClient` | `AddAzureTableServiceClient` |

## Type of breaking change

This change is a [binary incompatible](../categories.md#binary-compatibility) and [source incompatible](../categories.md#source-compatibility) change.

## Reason for change

The new API names provide consistency with Azure client libraries and resource granularity. This reduces confusion and makes it easier to understand and maintain Aspire applications that use Azure Storage resources. For more information, see the [GitHub issue](https://github.com/dotnet/docs-aspire/issues/3930).

## Recommended action

1. Update your code to use the new method names for adding and configuring Azure Storage resources and clients.
1. Replace any obsolete method calls with their new equivalents as shown in the examples above.
1. Recompile your application to ensure compatibility with Aspire 9.4.

## Affected APIs

- `AddBlobContainer`
- `AddAzureBlobClient`
- `AddAzureQueueClient`
- `AddAzureTableClient`

For a complete list of changes, see the [pull request](https://github.com/dotnet/aspire/pull/10241).
