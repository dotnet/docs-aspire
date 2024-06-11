To model the Redis resource in the app host, install the [Aspire.Hosting.Redis](https://www.nuget.org/packages/Aspire.Hosting.Redis) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Redis
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Redis"
                  Version="[SelectVersion]" />
```

---

In your app host project, register the .NET Aspire Stack Exchange Redis as a resource using the <xref:Aspire.Hosting.RedisBuilderExtensions.AddRedis%2A> method and consume the service using the following methods:
