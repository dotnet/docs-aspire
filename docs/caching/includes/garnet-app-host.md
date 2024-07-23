To model the Garnet resource (`GarnetResource`) in the app host, install the [Aspire.Hosting.Garnet](https://www.nuget.org/packages/Aspire.Hosting.Garnet) NuGet package in the [app host](xref:aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Garnet
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Garnet"
                  Version="[SelectVersion]" />
```

---

In your app host project, register .NET Aspire Garnet as a `GarnetResource` using the `AddGarnet` method and consume the service using the following methods:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddGarnet("cache");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(cache)
```

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` project named `cache`. In the _:::no-loc text="Program.cs":::_ file of `ExampleProject`, the Garnet connection can be consumed using:

```csharp
builder.AddGarnet("cache");
```
