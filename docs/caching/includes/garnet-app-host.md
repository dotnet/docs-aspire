---
ms.topic: include
---

The Garnet hosting integration models a Garnet resource as the <xref:Aspire.Hosting.ApplicationModel.GarnetResource> type. To access this type and APIs that allow you to add it to your [app model](xref:aspire/app-host#define-the-app-model), install the [📦 Aspire.Hosting.Garnet](https://www.nuget.org/packages/Aspire.Hosting.Garnet) NuGet package in the [app host](xref:aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Garnet
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Garnet"
                  Version="*" />
```

---

### Add Garnet resource

In your app host project, call <xref:Aspire.Hosting.GarnetBuilderExtensions.AddGarnet*> on the `builder` instance to add a Garnet resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddGarnet("cache");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(cache);

// After adding all resources, run the app...
```

When .NET Aspire adds a container image to the app host, as shown in the preceding example with the `ghcr.io/microsoft/garnet` image, it creates a new Garnet instance on your local machine. A reference to your Garnet resource (the `cache` variable) is added to the `ExampleProject`.

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` named `"cache"`. For more information, see [Container resource lifecycle](../../fundamentals/app-host-overview.md#container-resource-lifecycle).

> [!TIP]
> If you'd rather connect to an existing Garnet instance, call <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> instead. For more information, see [Reference existing resources](../../fundamentals/app-host-overview.md#reference-existing-resources).
