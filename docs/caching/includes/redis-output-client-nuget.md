---
ms.topic: include
---

To get started with the .NET Aspire Stack Exchange Redis output caching client integration, install the [📦 Aspire.StackExchange.Redis.OutputCaching](https://www.nuget.org/packages/Aspire.StackExchange.Redis.OutputCaching) NuGet package in the client-consuming project, that is, the project for the application that uses the output caching client. The Redis output caching client integration registers services required for enabling <xref:Microsoft.Extensions.DependencyInjection.OutputCacheConventionBuilderExtensions.CacheOutput*> method calls and [[OutputCache]](xref:Microsoft.AspNetCore.OutputCaching.OutputCacheAttribute) attribute usage to rely on Redis as its caching mechanism.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.StackExchange.Redis.OutputCaching
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.StackExchange.Redis.OutputCache"
                  Version="*" />
```

---
