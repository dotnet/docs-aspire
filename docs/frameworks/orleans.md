---
title: .NET Aspire Orleans integration
description: Learn how to use the .NET Aspire Orleans hosting integration, which can configure and orchestrate Orleans from a .NET Aspire app host project.
ms.date: 11/05/2024
uid: frameworks/orleans
---

# .NET Aspire Orleans integration

[Orleans](https://github.com/dotnet/orleans) has built-in support for .NET Aspire. .NET Aspire's application model lets you describe the services, databases, and other resources and infrastructure in your app and how they relate to each other. Orleans provides a straightforward way to build distributed applications that are elastically scalable and fault-tolerant. You can use .NET Aspire to configure and orchestrate Orleans and its dependencies, such as by providing Orleans with cluster membership and storage.

Orleans is represented as a resource in .NET Aspire. Unlike other integrations, the Orleans integration doesn't create a container and doesn't require a separate client integration package. Instead you complete the Orleans configuration in the .NET Aspire app host project.

> [!NOTE]
> This integration requires Orleans version 8.1.0 or later.

## Hosting integration

The Orleans hosting integration models an Orleans service as the <xref:Aspire.Hosting.Orleans.OrleansService> type. To access this type and APIs, add the [📦 Aspire.Hosting.Orleans](https://www.nuget.org/packages/Aspire.Hosting.Orleans) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Orleans
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Orleans"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add an Orleans resource

In your app host project, call <xref:Aspire.Hosting.OrleansServiceExtensions.AddOrleans*> to add and return an Orleans service resource builder. The name provided to the Orleans resource is for diagnostic purposes. For most applications, a value of `"default"` suffices.

:::code language="csharp" source="snippets/Orleans/OrleansAppHost/Program.cs" range="12":::

### Use Azure storage for clustering tables and grain storage

In an Orleans app, the fundamental building block is a **grain**. Grains can have durable states. You must store the durable state for a grain somewhere. In a .NET Aspire application, **Azure Blob Storage** is one possible location.

Orleans hosts register themselves in a database and use that database to find each other and form a cluster. They store which servers are members of which silos in a database table. You can use either relational or NoSQL databases to store this information. In a .NET Aspire application, a popular choice to store this table is **Azure Table Storage**.

To configure Orleans with clustering and grain storage in Azure, install the [📦 Aspire.Hosting.Azure.Storage](https://www.nuget.org/packages/Aspire.Hosting.Azure.Storage) NuGet package in the app host project:

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.Storage
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.Storage"
                  Version="*" />
```

---

In your app host project, after you call <xref:Aspire.Hosting.OrleansServiceExtensions.AddOrleans*>, configure the Orleans resource with clustering and grain storage using the <xref:Aspire.Hosting.OrleansServiceExtensions.WithClustering*> and <xref:Aspire.Hosting.OrleansServiceExtensions.WithGrainStorage*> methods respectively:

:::code language="csharp" source="snippets/Orleans/OrleansAppHost/Program.cs" range="3-14" highlight="4-5,11-12":::

The preceding code tells Orleans that any service referencing it must also reference the `clusteringTable` resource.

### Add an Orleans server project in the app host

Now you can add a new project, enrolled in .NET Aspire orchestration, to your solution as an Orleans server. It will take part in the Orleans cluster as a silo with constituent grains. Reference the Orleans resource from your server project using `WithReference(orleans)`. When you reference the Orleans resource from your service, those resources are also referenced:

:::code language="csharp" source="snippets/Orleans/OrleansAppHost/Program.cs" range="16-22":::

### Add an Orleans client project in the app host

Orleans clients communicate with grains hosted on Orleans servers. In a .NET Aspire app, for example, you might have a front-end Web site that calls grains in an Orleans cluster. Reference the Orleans resource from your Orleans client using `WithReference(orleans.AsClient())`.

:::code language="csharp" source="snippets/Orleans/OrleansAppHost/Program.cs" range="24-29":::

## Create the Orleans server project

Now that the app host project is completed, you can implement the Orleans server project. Let's start by adding the necessary NuGet packages:

### [.NET CLI](#tab/dotnet-cli)

In the folder for the Orleans server project, run these commands:

```dotnetcli
dotnet add package Aspire.Azure.Data.Tables
dotnet add package Aspire.Azure.Storage.Blobs
dotnet add package Microsoft.Orleans.Server
dotnet add package Microsoft.Orleans.Persistence.AzureStorage
dotnet add package Microsoft.Orleans.Clustering.AzureStorage
```

### [PackageReference](#tab/package-reference)

In the configuration file for the Orleans server project, add these package references:

```xml
<PackageReference Include="Aspire.Azure.Data.Tables"
                  Version="*" />
<PackageReference Include="Aspire.Azure.Storage.Blobs"
                  Version="*" />
<PackageReference Include="Microsoft.Orleans.Persistence.AzureStorage"
                  Version="*" />
<PackageReference Include="Microsoft.Orleans.Clustering.AzureStorage"
                  Version="*" />

```

---

Next, in the _:::no-loc text="Program.cs":::_ file of your Orleans server project, add the Azure Storage blob and tables clients and then call <xref:Microsoft.Extensions.Hosting.GenericHostExtensions.UseOrleans*>.

:::code language="csharp" source="snippets/Orleans/OrleansServer/Program.cs" range="4-9" :::

The following code is a complete example of an Orleans server project, including a grain named `CounterGrain`:

:::code language="csharp" source="snippets/Orleans/OrleansServer/Program.cs" :::

The `CounterGrain` is an implementation of the following `ICounterGrain` interface, which is defined in the shared _OrleansContracts_ project:

:::code language="csharp" source="snippets/Orleans/OrleansContracts/ICounterGrain.cs" :::

## Create an Orleans client project

In the Orleans client project, add the same NuGet packages:

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Azure.Data.Tables
dotnet add package Aspire.Azure.Storage.Blobs
dotnet add package Microsoft.Orleans.Client
dotnet add package Microsoft.Orleans.Persistence.AzureStorage
dotnet add package Microsoft.Orleans.Clustering.AzureStorage
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Azure.Data.Tables"
                  Version="*" />
<PackageReference Include="Aspire.Azure.Storage.Blobs"
                  Version="*" />
<PackageReference Include="Microsoft.Orleans.Persistence.AzureStorage"
                  Version="*" />
<PackageReference Include="Microsoft.Orleans.Clustering.AzureStorage"
                  Version="*" />

```

---

Next, in the _:::no-loc text="Program.cs":::_ file of your Orleans client project, add the Azure table storage client and then call <xref:Microsoft.Extensions.Hosting.OrleansClientGenericHostExtensions.UseOrleansClient*>.

:::code language="csharp" source="snippets/Orleans/OrleansClient/Program.cs" range="6-7" :::

The following code is a complete example of an Orleans client project. It calls the `CounterGrain` grain defined in the Orleans server example above:

:::code language="csharp" source="snippets/Orleans/OrleansClient/Program.cs" :::

## Enabling OpenTelemetry

By convention, .NET Aspire solutions include a project for defining default configuration and behavior for your service. This project is called the _service defaults_ project and templates create it with a name ending in _ServiceDefaults_. To configure Orleans for OpenTelemetry in .NET Aspire, apply configuration to your service defaults project following the [Orleans observability](/dotnet/orleans/host/monitoring/) guide.

Modify the `ConfigureOpenTelemetry` method to add the Orleans _meters_ and _tracing_ instruments. The following code snippet shows the modified _Extensions.cs_ file from a service defaults project that includes metrics and traces from Orleans.

:::code language="csharp" source="snippets/Orleans/OrleansServiceDefaults/Extensions.cs" range="40-68" highlight="15,19-20":::

## Supported providers

The Orleans Aspire integration supports a limited subset of Orleans providers today:

- Clustering:
  - Redis
  - Azure Storage Tables
- Persistence:
  - Redis
  - Azure Storage Tables
  - Azure Storage Blobs
- Reminders:
  - Redis
  - Azure Storage Tables
- Grain directory:
  - Redis
  - Azure Storage Tables

Streaming providers aren't supported as of Orleans version 8.1.0.

## Next steps

> [!div class="nextstepaction"]
> [Microsoft Orleans documentation](/dotnet/orleans/)
> [Explore the Orleans voting sample app](/samples/dotnet/aspire-samples/orleans-voting-sample-app-on-aspire/)
