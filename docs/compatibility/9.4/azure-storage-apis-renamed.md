---
title: "Breaking change - Azure Storage APIs renamed and refactored"
description: "Learn about the breaking changes in .NET Aspire 9.4 where Azure Storage APIs were renamed and refactored for clarity and consistency."
ms.date: 07/08/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/3930
---

# Azure Storage APIs renamed and refactored

In .NET Aspire 9.4, several Azure Storage APIs were renamed and refactored for clarity and consistency. These changes affect how you add and configure Azure Blob, Queue, and Table storage resources and clients in your Aspire applications. The new API names better align with Azure resource naming and reduce confusion.

## Version introduced

.NET Aspire 9.4

## Previous behavior

Previously, you used methods like `AddBlobs`, `AddBlobContainer`, `AddQueues`, and `AddTables` to add Azure Storage resources.

**Hosting integration example:**

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage");

var blobs = storage.AddBlobs("blobs");
var blobContainer = blobs.AddBlobContainer("container");

var queues = storage.AddQueues("queues");
var tables = storage.AddTables("tables");
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

Now, the API uses more explicit names that match Azure resource types. For example, use `AddBlobService`, `AddBlobContainer`, `AddQueueService`, `AddQueue`, `AddTableService`, and `AddTable`.

**Hosting integration example:**

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage");

var blobs = storage.AddBlobService("blobService");
blobs.AddBlobContainer("container");

var queues = storage.AddQueueService("queueService");
queues.AddQueue("queue");

var tables = storage.AddTableService("tableService");
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
| `AddBlobs` | `AddBlobService` | — |
| `AddBlobContainer` | `AddBlobContainer` | New API uses `IResourceBuilder<AzureStorageResource>` overload. |
| `AddTables` | `AddTableService` | — |
| `AddQueues` | `AddQueueService` | — |
| N/A | `AddQueue` | — |

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
1. Recompile your application to ensure compatibility with .NET Aspire 9.4.

## Affected APIs

- `AddBlobs`
- `AddBlobContainer`
- `AddTables`
- `AddQueues`
- `AddAzureBlobClient`
- `AddAzureQueueClient`
- `AddAzureTableClient`

For a complete list of changes, see the [pull request](https://github.com/dotnet/aspire/pull/10241).
