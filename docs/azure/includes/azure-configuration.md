---
ms.topic: include
---

> [!IMPORTANT]
> When you call `RunAsExisting`, `PublishAsExisting`, or `AsExisting` methods to work with resources that are already present in your Azure subscription, you must add certain configuration values to your App Host to ensure that .NET Aspire can locate them. The necessary configuration values include **SubscriptionId**, **AllowResourceGroupCreation**, **ResourceGroup**, and **Location**. If you don't set them, "Missing configuration" errors appear in the .NET Aspire dashboard. For more information about how to set them, see [Configuration](../local-provisioning.md#configuration).
