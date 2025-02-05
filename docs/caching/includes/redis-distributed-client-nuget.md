---
ms.topic: include
---

To get started with the .NET Aspire Redis distributed caching integration, install the [📦 Aspire.StackExchange.Redis.DistributedCaching](https://www.nuget.org/packages/Aspire.StackExchange.Redis.DistributedCaching) NuGet package in the client-consuming project, i.e., the project for the application that uses the Redis distributed caching client. The Redis client integration registers an <xref:Microsoft.Extensions.Caching.Distributed.IDistributedCache> instance that you can use to interact with Redis.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.StackExchange.Redis.DistributedCaching
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.StackExchange.Redis.DistributedCaching"
                  Version="*" />
```

---
