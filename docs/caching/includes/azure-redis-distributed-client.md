---
ms.topic: include
ms.custom: sfi-ropc-nochange
---

### Add Azure Cache for Redis authenticated distributed client

By default, when you call <xref:Aspire.Hosting.AzureRedisExtensions.AddAzureRedis*> in your AppHost project, the Redis hosting integration configures Microsoft Entra ID. To enable authentication in your client application, use the `WithAzureAuthentication()` method:

```csharp
builder.AddRedisDistributedCache("cache")
       .WithAzureAuthentication();
```

This simplified approach automatically configures the Redis distributed cache client to use Azure authentication with the appropriate credentials.
