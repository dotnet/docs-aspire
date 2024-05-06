---
title: .NET Aspire with Orleans
description: Learn how to use Orleans with .NET Aspire
ms.date: 05/3/2024
ms.topic: overview
---

# Use Orleans with .NET Aspire

## Overview

Orleans and .NET Aspire are built for each other. Aspire's application model lets you describe the services, databases, and other resources/infrastructure in your application and how they relate. Orleans' provides a straightforward way to build distributed applications which are elastically scalable and fault-tolerant. Aspire is used to configure and orchestrate Orleans and its dependencies, such as a by providing Orleans with databases cluster membership and storage.
Orleans is represented as a resource in Aspire. The Orleans resource includes configuration which your service needs to operate, such as cluster membership providers and storage providers.

## Prerequisites

- Orleans version 8.1.0 or later

## Getting started

To get started you need to add the Orleans hosting pacakge to the app host project, install the [Aspire.Hosting.Orleans](https://www.nuget.org/packages/Aspire.Hosting.Orleans) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Orleans --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Orleans"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

The Orleans resource is added to the Aspire Distributed Application Builder using the `AddOrleans(string name)` method, which returns an Orleans resource builder.
The name provided to the Orleans resource is for diagnostic purposes. For most applications, a value of `"default"` will suffice.

```csharp
var orleans = builder.AddOrleans("default");
```

The Orleans resource builder, `orleans` in the above example, offers methods to configure your Orleans resource. The simplest Orleans services only need a cluster membership resource:

```csharp
// Add a resource which Orleans supports (Azure Table Storage) to use for cluster membership:
var storage = builder.AddAzureStorage("storage").RunAsEmulator();
var clusteringTable = storage.AddTables("clustering");

// Tell Orleans to use that resource for cluster membership:
orleans.WithClustering(clusteringTable);
```

This tells Orleans that any service referencing it will also need to reference the `clusteringTable` resource.

To participate in an Orleans cluster, reference the Orleans resource from your service project, either using `WithReference(orleans)` to participate as an Orleans server, or `WithReference(orleans.AsClient())` to participate as a client.
When you reference the Orleans resource from your service, those resources will also be referenced:

```csharp
// Add the Orleans resource to the "silo" project:
// As described above, this implicitly add references to the required resources.
// In this case, that is the 'clusteringTable' resource declared earlier.
builder.AddProject<Projects.OrleansServer>("silo")
       .WithReference(orleans);
```

Putting that all together, here is an example of an Aspire AppHost project which includes an Orleans resource with clustering and storage, and an Orleans server and client project resource.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add the resources which we will use for Orleans clustering and grain state storage.
var storage = builder.AddAzureStorage("storage").RunAsEmulator();
var clusteringTable = storage.AddTables("clustering");
var grainStorage = storage.AddBlobs("grainstate");

// Add the Orleans resource to the Aspire DistributedApplication builder, then configure it with Azure Table Storage for clustering
// and Azure Blob Storage for grain storage.
var orleans = builder.AddOrleans("my-app")
                     .WithClustering(clusteringTable)
                     .WithGrainStorage("Default", grainStorage);

// Reference the Orleans resource from the 'silo' project so that it can join the Orleans cluster as a service.
builder.AddProject<Projects.OrleansServer>("silo")
       .WithReference(orleans)
       .WithReplicas(3);

// Reference the Orleans resource as a client from the 'frontend' project so that it can connect to the Orleans cluster.
builder.AddProject<Projects.OrleansClient>("frontend")
       .WithReference(orleans.AsClient())
       .WithExternalHttpEndpoints()
       .WithReplicas(3);

// Build and run the application.
using var app = builder.Build();
await app.RunAsync();
```

To consume the Aspire Orleans resource from an Orleans server project, there are a few steps:

1. Add the relevant Aspire components. In our example, we are using _Aspire.Azure.Data.Tables_ and _Aspire.Azure.Storage.Blobs_.
2. Add the Orleans provider packages for those Aspire components. In our example, we are using _Microsoft.Orleans.Persistence.AzureStorage_ and _Microsoft.Orleans.Clustering.AzureStorage_
3. Add Orleans to the host application builder.

In OrleansServer's _Program.cs_ file, we must configure the Aspire components we are using and add Orleans to the host builder. Note that the names provided must match the names used in the Aspire AppHost project: "clustering" for the clustering provider, and "grainstate" for the grain state storage provider:

```csharp
using Orleans.Runtime;
using OrleansContracts;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddKeyedAzureTableClient("clustering");
builder.AddKeyedAzureBlobClient("grainstate");
builder.UseOrleans();

var app = builder.Build();

app.MapGet("/", () => "OK");

await app.RunAsync();

public sealed class CounterGrain(
    [PersistentState("count")] IPersistentState<int> count) : ICounterGrain
{
    public ValueTask<int> Get()
    {
        return ValueTask.FromResult(count.State);
    }

    public async ValueTask<int> Increment()
    {
        var result = ++count.State;
        await count.WriteStateAsync();
        return result;
    }
}
```

We do similarly in the OrleansClient project, adding the Aspire resources which our project needs to join the Orleans cluster as a client, and configuring the host builder to add an Orleans client:

```csharp
using OrleansContracts;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddKeyedAzureTableClient("clustering");
builder.UseOrleansClient();

var app = builder.Build();

app.MapGet("/counter/{grainId}", async (IClusterClient client, string grainId) =>
{
    var grain = client.GetGrain<ICounterGrain>(grainId);
    return await grain.Get();
});

app.MapPost("/counter/{grainId}", async (IClusterClient client, string grainId) =>
{
    var grain = client.GetGrain<ICounterGrain>(grainId);
    return await grain.Increment();
});

app.UseFileServer();

await app.RunAsync();
```

## Supported providers

The Orleans Aspire integration supports a limited subset of Orleans providers today:

- Clustering:
  - Redis
  - Azure Storage Tables
- Persistence:
  - Redis
  - Azure Storage Tables
- Reminders:
  - Redis
  - Azure Storage Tables
- Grain directory:
  - Redis
  - Azure Storage Tables

Streaming providers are not supported as of Orleans version 8.1.0.
