---
ms.topic: include
---

The .NET Aspire Azure Cache for Redis hosting integration models an Azure Redis resource as the <xref:Aspire.Hosting.Azure.AzureRedisCacheResource> type. To access this type and APIs for expressing them as resources in your [app host](xref:dotnet/aspire/app-host) project, add the [📦 Aspire.Hosting.Azure.Redis](https://www.nuget.org/packages/Aspire.Hosting.Azure.Redis) NuGet package:

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.Redis
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.Redis"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add Azure Cache for Redis resource

In your app host project, call <xref:Aspire.Hosting.AzureRedisExtensions.AddAzureRedis*> on the `builder` instance to add an Azure Cache for Redis resource, as shown in the following example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddAzureRedis("azcache");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(cache);

// After adding all resources, run the app...
```

The preceding call to `AddAzureRedis` configures the Redis server resource to be deployed as an [Azure Cache for Redis](/azure/azure-cache-for-redis/cache-overview).

> [!IMPORTANT]
> By default, `AddAzureRedis` configures [Microsoft Entra ID](/azure/azure-cache-for-redis/cache-azure-active-directory-for-authentication) authentication. This requires changes to applications that need to connect to these resources, for example, client integrations.

> [!TIP]
> When you call <xref:Aspire.Hosting.AzureRedisExtensions.AddAzureRedis*>, it implicitly calls <xref:Aspire.Hosting.AzureProvisionerExtensions.AddAzureProvisioning*>—which adds support for generating Azure resources dynamically during app startup. The app must configure the appropriate subscription and location. For more information, see [Local provisioning: Configuration](../../azure/local-provisioning.md#configuration).

#### Provisioning-generated Bicep

If you're new to [Bicep](/azure/azure-resource-manager/bicep/overview), it's a domain-specific language for defining Azure resources. With .NET Aspire, you don't need to write Bicep by-hand, instead the provisioning APIs generate Bicep for you. When you publish your app, the generated Bicep is output alongside the manifest file. When you add an Azure Cache for Redis resource, the following Bicep is generated:

:::code language="bicep" source="../../snippets/azure/AppHost/redis.module.bicep":::

The preceding Bicep is a module that provisions an Azure Cache for Redis resource. Additionally, role assignments are created for the Azure resource in a separate module:

:::code language="bicep" source="../../snippets/azure/AppHost/redis-roles.module.bicep":::

In addition to the Azure Cache for Redis, it also provisions an access policy assignment to the application access to the cache. The generated Bicep is a starting point and is influenced by changes to the provisioning infrastructure in C#. Customizations to the Bicep file directly will be overwritten, so make changes through the C# provisioning APIs to ensure they are reflected in the generated files.

#### Customize provisioning infrastructure

All .NET Aspire Azure resources are subclasses of the <xref:Aspire.Hosting.Azure.AzureProvisioningResource> type. This type enables the customization of the generated Bicep by providing a fluent API to configure the Azure resources—using the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},System.Action{Aspire.Hosting.Azure.AzureResourceInfrastructure})> API. For example, you can configure the `kind`, `consistencyPolicy`, `locations`, and more. The following example demonstrates how to customize the Azure Cache for Redis resource:

:::code language="csharp" source="../../snippets/azure/AppHost/Program.ConfigureRedisInfra.cs" id="configure":::

The preceding code:

- Chains a call to the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure*> API:
  - The `infra` parameter is an instance of the <xref:Aspire.Hosting.Azure.AzureResourceInfrastructure> type.
  - The provisionable resources are retrieved by calling the <xref:Azure.Provisioning.Infrastructure.GetProvisionableResources> method.
  - The single <xref:Azure.Provisioning.Redis.RedisResource> is retrieved.
  - The `Sku` is set with a family of `BasicOrStandard`, a name of `Standard`, and a capacity of `1`.
  - A tag is added to the Redis resource with a key of `ExampleKey` and a value of `Example value`.

There are many more configuration options available to customize the Azure Cache for Redis resource. For more information, see <xref:Azure.Provisioning.Redis>. For more information, see [Azure.Provisioning customization](../../azure/integrations-overview.md#azureprovisioning-customization).

### Connect to an existing Azure Cache for Redis

You might have an existing Azure Cache for Redis resource that you want to connect to. You can chain a call to annotate that your <xref:Aspire.Hosting.Azure.AzureRedisCacheResource> is an existing resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var existingRedisName = builder.AddParameter("existingRedisName");
var existingRedisResourceGroup = builder.AddParameter("existingRedisResourceGroup");

var cache = builder.AddAzureRedis("azcache")
                   .AsExisting(existingRedisName, existingRedisResourceGroup);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(cache);

// After adding all resources, run the app...
```

For more information on treating Azure Cache for Redis resources as existing resources, see [Use existing Azure resources](../../azure/integrations-overview.md#use-existing-azure-resources).

> [!NOTE]
> Alternatively, instead of representing an Azure Cache for Redis resource, you can add a connection string to the app host. This approach is weakly-typed, and doesn't work with role assignments or infrastructure customizations. For more information, see [Add existing Azure resources with connection strings](../../azure/integrations-overview.md#add-existing-azure-resources-with-connection-strings).

### Run Azure Cache for Redis resource as a container

The Azure Cache for Redis hosting integration supports running the Redis server as a local container. This is beneficial for situations where you want to run the Redis server locally for development and testing purposes, avoiding the need to provision an Azure resource or connect to an existing Azure Cache for Redis server.

To make use of the [`docker.io/library/redis`](https://hub.docker.com/_/redis/) container image, and run the Azure Cache for Redis instance as a container locally, chain a call to <xref:Aspire.Hosting.AzureRedisExtensions.RunAsContainer*>, as shown in the following example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddAzureRedis("azcache")
                   .RunAsContainer();

builder.AddProject<Projects.ExampleProject>()
       .WithReference(cache);

// After adding all resources, run the app...
```

The preceding code configures the Redis resource to run locally in a container.

> [!TIP]
> The `RunAsContainer` method is useful for local development and testing. The API exposes an optional delegate that enables you to customize the underlying <xref:Aspire.Hosting.ApplicationModel.RedisResource> configuration, such adding [Redis Insights](https://redis.io/insight/), [Redis Commander](https://joeferner.github.io/redis-commander/), adding a data volume or data bind mount. For more information, see the [.NET Aspire Redis hosting integration](../stackexchange-redis-integration.md#add-redis-resource-with-redis-insights).

### Configure the Azure Cache for Redis resource to use access key authentication

By default, the Azure Cache for Redis resource is configured to use [Microsoft Entra ID](/azure/postgresql/flexible-server/concepts-azure-ad-authentication) authentication. If you want to use password authentication (not recommended), you can configure the server to use password authentication by calling the <xref:Aspire.Hosting.AzureRedisExtensions.WithAccessKeyAuthentication*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddAzureRedis("azcache")
                   .WithAccessKeyAuthentication();

builder.AddProject<Projects.ExampleProject>()
       .WithReference(cache);

// After adding all resources, run the app...
```

The preceding code configures the Azure Cache for Redis resource to use access key authentication. This alters the generated Bicep to use access key authentication instead of Microsoft Entra ID authentication. In other words, the connection string will contain a password, and will be added to an Azure Key Vault secret.
