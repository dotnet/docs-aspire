---
ms.topic: include
---

### Add Azure Cache for Redis authenticated client

By default, when you call `AddAzureRedis` in your Redis hosting integration, it configures [📦 Microsoft.Azure.StackExchangeRedis](https://www.nuget.org/packages/Microsoft.Azure.StackExchangeRedis) NuGet package to enable authentication:

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Microsoft.Azure.StackExchangeRedis
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Microsoft.Azure.StackExchangeRedis"
                  Version="*" />
```

---

The Redis connection can be consumed using the client integration and `Microsoft.Azure.StackExchangeRedis`. Consider the following configuration code:

```csharp
var azureOptionsProvider = new AzureOptionsProvider();

var configurationOptions = ConfigurationOptions.Parse(
    builder.Configuration.GetConnectionString("cache") ?? 
    throw new InvalidOperationException("Could not find a 'cache' connection string."));

if (configurationOptions.EndPoints.Any(azureOptionsProvider.IsMatch))
{
    await configurationOptions.ConfigureForAzureWithTokenCredentialAsync(
        new DefaultAzureCredential());
}

builder.AddRedisClient("cache", configureOptions: options =>
{
    options.Defaults = configurationOptions.Defaults;
});
```

For more information, see the [Microsoft.Azure.StackExchangeRedis](https://github.com/Azure/Microsoft.Azure.StackExchangeRedis) repo.
