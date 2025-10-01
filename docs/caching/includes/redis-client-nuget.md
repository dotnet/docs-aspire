---
ms.topic: include
---

To get started with the Aspire Stack Exchange Redis client integration, install the [📦 Aspire.StackExchange.Redis](https://www.nuget.org/packages/Aspire.StackExchange.Redis) NuGet package in the client-consuming project, that is, the project for the application that uses the Redis client. The Redis client integration registers an [IConnectionMultiplexer](https://stackexchange.github.io/StackExchange.Redis/Basics) instance that you can use to interact with Redis.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.StackExchange.Redis
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.StackExchange.Redis"
                  Version="*" />
```

---
