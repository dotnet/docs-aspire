---
ms.topic: include
---

### Azure Redis hosting integration

To deploy your Redis resources to Azure, install the [📦 Aspire.Hosting.Azure.Redis](https://www.nuget.org/packages/Aspire.Hosting.Azure.Redis) NuGet package:

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.Redis
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.Redis"
                  Version="*" />
```

---

#### Add Azure Cache for Redis server resource

After you've installed the .NET Aspire hosting Azure Redis package, call the `AddAzureRedis` extension method in your app host project:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddAzureRedis("azcache")

var exampleProject = builder.AddProject<Projects.ExampleProject>()
                            .WithReference(cache);
```

The preceding call to `AddAzureRedis` configures the Redis server resource to be deployed as an [Azure Cache for Redis](/azure/azure-cache-for-redis/cache-overview).

> [!IMPORTANT]
> By default, `AddAzureRedis` configures [Microsoft Entra ID](/azure/azure-cache-for-redis/cache-azure-active-directory-for-authentication) authentication. This requires changes to applications that need to connect to these resources, for example, client integrations.
