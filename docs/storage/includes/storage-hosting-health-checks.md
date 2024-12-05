---
ms.topic: include
---

### Hosting integration health checks

The Azure Storage hosting integration automatically adds a health check for the storage resource, but it's limited to blobs—assuming queues and tables are healthy when blobs are. Additionally, this health check is only added when running as an emulator, and it verifies that Azurite is running and that a connection can be established to it.

The hosting integration relies on the [📦 AspNetCore.HealthChecks.Azure.Storage.Blobs](https://www.nuget.org/packages/AspNetCore.HealthChecks.Azure.Storage.Blobs) NuGet package.
