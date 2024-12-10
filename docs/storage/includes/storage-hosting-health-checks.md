---
ms.topic: include
---

### Hosting integration health checks

The Azure Storage hosting integration automatically adds a health check for the storage resource. It's added only when running as an emulator, and verifies the Azurite container is running and that a connection can be established to it. The hosting integration relies on the [📦 AspNetCore.HealthChecks.Azure.Storage.Blobs](https://www.nuget.org/packages/AspNetCore.HealthChecks.Azure.Storage.Blobs) NuGet package.
