<!--
https://github.com/dotnet/docs-aspire/issues/1049

We are introducing support for Garnet in this PR:

dotnet/aspire#4324

It is presently a drop in alternative for Redis. We probably need an article that sites right alongside the Redis articles so that folks who might be looking for Redis content to understand how Garnet works can find it.

The article should cover:

AddGarnet
WithDataVolume/WithBindMount
WithPersistence
It can probably cover off the client side pieces by just referring back to the Redis article although the Redis article might need to more clearly call out which is service code and which is app host code.

Configuring Garnet using AddGarnet and using the data volume and persistence extension methods.

Include links to:
- https://github.com/microsoft/Garnet
- https://microsoft.github.io/garnet/docs
-->

To model the Garnet resource in the app host, install the [Aspire.Hosting.Garnet](https://www.nuget.org/packages/Aspire.Hosting.Garnet) NuGet package in the [app host](xref:aspire/app-host) project.

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

In your app host project, register the .NET Aspire Garnet as a resource using the `AddGarnet` method and consume the service using the following methods:

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
