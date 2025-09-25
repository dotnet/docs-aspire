---
ms.topic: include
ms.custom: sfi-ropc-nochange
---

### Add Azure Cache for Redis authenticated client

By default, when you call <xref:Aspire.Hosting.AzureRedisExtensions.AddAzureRedis*> in your AppHost project, the Redis hosting integration configures Microsoft Entra ID. To enable authentication in your client application, add a reference to the `Aspire.Microsoft.Azure.StackExchangeRedis` package and use the following code:

```csharp
builder.AddRedisClientBuilder("cache")
       .WithAzureAuthentication();
```

This simplified approach automatically configures the Redis client to use Azure authentication with the appropriate credentials.
