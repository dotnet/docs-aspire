---
ms.topic: include
ms.custom: sfi-ropc-nochange
---

### Add Azure Cache for Redis authenticated client

By default, when you call <xref:Aspire.Hosting.AzureRedisExtensions.AddAzureRedis*> in your Redis hosting integration, it configures Microsoft Entra ID. To enable authentication in your client application, use the `WithAzureAuthentication()` method:

```csharp
builder.AddRedisClient("cache")
       .WithAzureAuthentication();
```

This simplified approach automatically configures the Redis client to use Azure authentication with the appropriate credentials.
