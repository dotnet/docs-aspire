---
ms.topic: include
ms.custom: sfi-ropc-nochange
---

The .NET Aspire Stack Exchange Redis integration supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the <xref:Aspire.StackExchange.Redis.StackExchangeRedisSettings> from configuration by using the `Aspire:StackExchange:Redis` key. Example _:::no-loc text="appsettings.json":::_ that configures some of the options:

```json
{
  "Aspire": {
    "StackExchange": {
      "Redis": {
        "ConfigurationOptions": {
          "ConnectTimeout": 3000,
          "ConnectRetry": 2
        },
        "DisableHealthChecks": true,
        "DisableTracing": false
      }
    }
  }
}
```

For the complete Redis client integration JSON schema, see [Aspire.StackExchange.Redis/ConfigurationSchema.json](https://github.com/dotnet/aspire/blob/v9.1.0/src/Components/Aspire.StackExchange.Redis/ConfigurationSchema.json).

#### Use named configuration

The .NET Aspire Stack Exchange Redis integration supports named configuration, which allows you to configure multiple instances of the same resource type with different settings. The named configuration uses the connection name as a key under the main configuration section.

```json
{
  "Aspire": {
    "StackExchange": {
      "Redis": {
        "cache1": {
          "ConfigurationOptions": {
            "ConnectTimeout": 3000,
            "ConnectRetry": 2
          },
          "DisableHealthChecks": true
        },
        "cache2": {
          "ConfigurationOptions": {
            "ConnectTimeout": 5000,
            "ConnectRetry": 3
          },
          "DisableTracing": true
        }
      }
    }
  }
}
```

In this example, the `cache1` and `cache2` connection names can be used when calling `AddKeyedRedisClient`:

```csharp
builder.AddKeyedRedisClient("cache1");
builder.AddKeyedRedisClient("cache2");
```

Named configuration takes precedence over the top-level configuration. If both are provided, the settings from the named configuration override the top-level settings.
