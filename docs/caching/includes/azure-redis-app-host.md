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

#### Generated provisioning Bicep

If you're new to [Bicep](/azure/azure-resource-manager/bicep/overview), it's a domain-specific language for defining Azure resources. With .NET Aspire, you don't need to write Bicep by-hand, instead the provisioning APIs generate Bicep for you. When you publish your app, the generated Bicep is output alongside the manifest file. When you add an Azure Cache for Redis resource, the following Bicep is generated:

<!-- markdownlint-disable MD033 -->
<br/>
<details>
<summary id="azure-redis"><strong>Toggle Azure Cache for Redis Bicep.</strong></summary>
<p aria-labelledby="azure-redis">

:::code language="bicep" source="../../snippets/azure/AppHost/redis.module.bicep":::

</p>
</details>
<!-- markdownlint-enable MD033 -->

The preceding Bicep is a module that provisions an Azure Cache for Redis with the following defaults:

- `location`: The location of the Azure Cache for Redis resource. The default is the location of the resource group.
- `principalId`: The principal ID of the Azure Cache for Redis resource.
- `principalName`: The principal name of the Azure Cache for Redis resource.
- `sku`: The SKU of the Azure Cache for Redis resource. The default is `Basic` with a capacity of `1`.
- `enableNonSslPort`: The non-SSL port of the Azure Cache for Redis resource. The default is `false`.
- `disableAccessKeyAuthentication`: The access key authentication of the Azure Cache for Redis resource. The default is `true`.
- `minimumTlsVersion`: The minimum TLS version of the Azure Cache for Redis resource. The default is `1.2`.
- `redisConfiguration`: The Redis configuration of the Azure Cache for Redis resource. The default is `aad-enabled` set to `true`.
- `tags`: The tags of the Azure Cache for Redis resource. The default is `aspire-resource-name` set to the name of the Aspire resource, in this case `redis`.
- `redis_contributor`: The contributor of the Azure Cache for Redis resource, with an access policy name of `Data Contributor`.
- `connectionString`: The connection string of the Azure Cache for Redis resource.

In addition to the Azure Cache for Redis, it also provisions an Azure Firewall rule to allow all Azure IP addresses. Finally, an administrator is created for the Redis server, and the connection string is outputted as an output variable. The generated Bicep is a starting point and can be customized to meet your specific requirements.

#### Customize provisioning infrastructure

All .NET Aspire Azure resources are subclasses of the <xref:Aspire.Hosting.Azure.AzureProvisioningResource> type. This type enables the customization of the generated Bicep by providing a fluent API to configure the Azure resources—using the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},System.Action{Aspire.Hosting.Azure.AzureResourceInfrastructure})> API. For example, you can configure the `kind`, `consistencyPolicy`, `locations`, and more. The following example demonstrates how to customize the Azure Cache for Redis resource:

:::code language="csharp" source="../../snippets/azure/AppHost/Program.ConfigureRedisInfra.cs" id="configure":::

The preceding code:

- Chains a call to the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure*> API:
  - The `infra` parameter is an instance of the <xref:Aspire.Hosting.Azure.AzureResourceInfrastructure> type.
  - The provisionable resources are retrieved by calling the <xref:Azure.Provisioning.Infrastructure.GetProvisionableResources> method.
  - The single <xref:Azure.Provisioning.Redis.RedisResource> is retrieved.
  - The `RedisVersion` is set to `"6.0"`.
  - The `Sku` is set with a family of `BasicOrStandard`, a name of `Standard`, and a capacity of `1`.
  - A tag is added to the Redis resource with a key of `ExampleKey` and a value of `Example value`.

There are many more configuration options available to customize the Azure Cache for Redis resource. For more information, see <xref:Azure.Provisioning.Redis>. For more information, see [Azure.Provisioning customization](../../azure/integrations-overview.md#azureprovisioning-customization).

### Connect to an existing Azure Cache for Redis

You might have an existing Azure Cache for Redis that you want to connect to. Instead of representing a new Azure Cache for Redis resource, you can add a connection string to the app host. To add a connection to an existing Azure Cache for Redis, call the <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddConnectionString("azure-redis");

builder.AddProject<Projects.WebApplication>("web")
       .WithReference(cache);

// After adding all resources, run the app...
```

[!INCLUDE [connection-strings-alert](../../includes/connection-strings-alert.md)]

The connection string is configured in the app host's configuration, typically under [User Secrets](/aspnet/core/security/app-secrets), under the `ConnectionStrings` section. The app host injects this connection string as an environment variable into all dependent resources, for example:

```json
{
    "ConnectionStrings": {
        "azure-redis": "localhost:6379,ssl=true,abortConnect=False"
    }
}
```

The dependent resource can access the injected connection string by calling the <xref:Microsoft.Extensions.Configuration.ConfigurationExtensions.GetConnectionString*> method, and passing the connection name as the parameter, in this case `"azure-redis"`. The `GetConnectionString` API is shorthand for `IConfiguration.GetSection("ConnectionStrings")[name]`.

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

The preceding code configures an Azure Cache for Redis resource to run locally in a container.

> [!TIP]
> The `RunAsContainer` method is useful for local development and testing. The API exposes an optional delegate that enables you to customize the underlying <xref:Aspire.Hosting.Azure.AzureRedisCacheResource> configuration, such adding [Redis Insights](https://redis.io/insight/), [Redis Commander](https://joeferner.github.io/redis-commander/), adding a data volume or data bind mount. For more information, see the [.NET Aspire Redis hosting integration](../stackexchange-redis-integration.md#add-redis-resource-with-redis-insights).

### Configure the Azure Cache for Redis resource to use access key authentication

By default, the Azure Cache for Redis resource is configured to use [Microsoft Entra ID](/azure/postgresql/flexible-server/concepts-azure-ad-authentication) authentication. If you want to use password authentication, you can configure the server to use password authentication by calling the <xref:Aspire.Hosting.AzureRedisExtensions.WithAccessKeyAuthentication*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddAzureRedis("azcache")
                   .WithAccessKeyAuthentication();

builder.AddProject<Projects.ExampleProject>()
       .WithReference(cache);

// After adding all resources, run the app...
```

The preceding code configures the Azure Cache for Redis resource to use access key authentication. This alters the generated Bicep to use access key authentication instead of Microsoft Entra ID authentication. In other words, the connection string will contain a password, and will be added to an Azure Key Vault secret.
