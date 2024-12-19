---
title: .NET Aspire Orleans integration
description: Learn how to use the .NET Aspire Orleans hosting integration, which can configure and orchestrate Orleans from a .NET Aspire app host project.
ms.date: 11/05/2024
uid: frameworks/orleans
---

# .NET Aspire Orleans integration

[Orleans](https://github.com/dotnet/orleans) has built-in support for .NET Aspire. .NET Aspire's application model lets you describe the services, databases, and other resources and infrastructure in your app and how they relate to each other. Orleans provides a straightforward way to build distributed applications that are elastically scalable and fault-tolerant. You can use .NET Aspire to configure and orchestrate Orleans and its dependencies, such as by providing Orleans with database cluster membership and storage.

Orleans is represented as a resource in .NET Aspire. Unlike other integrations, the Orleans integration doesn't create a container and doesn't include a client integration. Instead you complete the Orleans configuration in the .NET Aspire app host project. 

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

In your app host project, call <xref:Aspire.Hosting.OrleansServiceExtentions.AddOrleans*> to add and return a Orleans service resource builder. The name provided to the Orleans resource is for diagnostic purposes. For most applications, a value of `"default"` suffices.

:::code language="csharp" source="snippets/Orleans/OrleansAppHost/Program.cs" range="12":::

> AJMTODO: Using Azure storage for clustering tables and grain storage


> AJMTODO: Participating in an Orleans cluster

> AJMTODO: Referencing the Orleans resource from a frontend project

> AJMTODO: Stuff to do in the Orleans server project

> AJMTODO: Stuff to do in the Orleans client project

> AJMTODO: Enabling OpenTelemetry

> AJMTODO: Supported Orleans providers

> AJMTODO: Next steps


> AJMTODO: Old stuff

The Orleans resource builder offers methods to configure your Orleans resource.

To configure Orleans with clustering and grain storage, install the [📦 Aspire.Hosting.Azure.Storage](https://www.nuget.org/packages/Aspire.Hosting.Azure.Storage) NuGet package in the app host project:

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

In the following example, the Orleans resource is configured with clustering and grain storage using the `WithClustering` and `WithGrainStorage` methods respectively:

:::code language="csharp" source="snippets/Orleans/OrleansAppHost/Program.cs" range="3-14" highlight="4-5,11-12":::


The preceding code tells Orleans that any service referencing it also needs to reference the `clusteringTable` resource.

To participate in an Orleans cluster, reference the Orleans resource from your service project, either using `WithReference(orleans)` to participate as an Orleans server, or `WithReference(orleans.AsClient())` to participate as a client. When you reference the Orleans resource from your service, those resources are also referenced:

:::code language="csharp" source="snippets/Orleans/OrleansAppHost/Program.cs" range="16-22":::

Putting that all together, here's an example of a .NET Aspire app host project which includes:

- An Orleans resource with clustering and storage.
- An Orleans server project, **OrleansServer**.
- An Orleans client project, **OrleansClient**.

:::code language="csharp" source="snippets/Orleans/OrleansAppHost/Program.cs" :::

To consume the .NET Aspire Orleans resource from an Orleans server project, there are a few steps:

1. Add the relevant .NET Aspire integrations. In this example, you're using <xref:Aspire.Azure.Data.Tables> and <xref:Aspire.Azure.Storage.Blobs>.
1. Add the Orleans provider packages for those .NET Aspire integrations. In this example, you're using `Microsoft.Orleans.Persistence.AzureStorage` and `Microsoft.Orleans.Clustering.AzureStorage`.
1. Add Orleans to the host application builder.

In the _:::no-loc text="Program.cs":::_ file of your Orleans server project, you must configure the .NET Aspire integrations you're using and add Orleans to the host builder. The names provided must match the names used in the .NET Aspire app host project: "clustering" for the clustering provider, and "grain-state" for the grain state storage provider:

:::code language="csharp" source="snippets/Orleans/OrleansServer/Program.cs" :::

You do similarly in the _OrleansClient_ project, adding the .NET Aspire resources which your project needs to join the Orleans cluster as a client, and configuring the host builder to add an Orleans client:

:::code language="csharp" source="snippets/Orleans/OrleansClient/Program.cs" :::

## Enabling OpenTelemetry

By convention, .NET Aspire solutions include a project for defining default configuration and behavior for your service. This project is called the _service default_ project and templates create it with a name ending in _ServiceDefaults_. To configure Orleans for OpenTelemetry in .NET Aspire, you need to apply configuration to your service defaults project following the [Orleans observability](/dotnet/orleans/host/monitoring/) guide. In short, you need to modify the `ConfigureOpenTelemetry` method to add the Orleans _meters_ and _tracing_ instruments. The following code snippet shows the modified _Extensions.cs_ file from a service defaults project that includes metrics and traces from Orleans.

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
> [Explore the Orleans voting sample app](/samples/dotnet/aspire-samples/orleans-voting-sample-app-on-aspire/)
