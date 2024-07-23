To model the Valkey resource (`ValkeyResource`) in the app host, install the [Aspire.Hosting.Valkey](https://www.nuget.org/packages/Aspire.Hosting.Valkey) NuGet package in the [app host](xref:aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Valkey
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Valkey"
                  Version="[SelectVersion]" />
```

---

In your app host project, register the .NET Aspire Valkey as a resource using the `AddValkey` method and consume the service using the following methods:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddValkey("cache");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(cache)
```

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` project named `cache`. In the _:::no-loc text="Program.cs":::_ file of `ExampleProject`, the Valkey connection can be consumed using:

```csharp
builder.AddValkey("cache");
```
