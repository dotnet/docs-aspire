---
ms.topic: include
---

The .NET Aspire Stack Exchange Redis integration supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the <xref:Aspire.StackExchange.Redis.StackExchangeRedisSettings> from configuration by using the `Aspire:StackExchange:Redis` key. Example _:::no-loc text="appsettings.json":::_ that configures some of the options:

```json
{
  "Aspire": {
    "StackExchange": {
      "Redis": {
        "ConnectionString": "localhost:6379",
        "DisableHealthChecks": true,
        "DisableTracing": false
      }
    }
  }
}
```

For the complete Redis client integration JSON schema, see [Aspire.StackExchange.Redis/ConfigurationSchema.json](https://github.com/dotnet/aspire/blob/v9.0.0/src/Components/Aspire.StackExchange.Redis/ConfigurationSchema.json).
