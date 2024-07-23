To model the Redis resource (`RedisResource`) in the app host, install the [Aspire.Hosting.Redis](https://www.nuget.org/packages/Aspire.Hosting.Redis) NuGet package in the [app host](xref:aspire/app-host) project.

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

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(cache)
```

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` project named `cache`. In the _:::no-loc text="Program.cs":::_ file of `ExampleProject`, the Redis connection can be consumed using:

```csharp
builder.AddRedis("cache");
```
