---
title: Azure Managed Redis integration
description: Learn how to integrate Azure Managed Redis with the .NET Aspire stack.
ms.date: 01/28/2025
ai-usage: ai-generated
---

# .NET Aspire Azure Managed Redis&reg;<sup>**[*](#registered)**</sup> integration

<a name="heading"></a>

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

Azure Managed Redis is the most advanced Redis offering on Azure to date, supporting both traditional caching and caching for AI apps and workloads. It offers vector data structures and vector search, alongside secondary indexing for full-text search, exact matching, geospatial queries, numeric data handling, and fast data processing.

The .NET Aspire Azure Managed Redis integration enables you to connect to existing Azure Managed Redis instances, or create new instances from .NET with advanced Redis features.

## Hosting integration

The .NET Aspire Azure Managed Redis hosting integration models an Azure Managed Redis resource as an Azure resource type. To access this type and APIs for expressing them as resources in your [AppHost](xref:dotnet/aspire/app-host) project, add the [📦 Aspire.Hosting.Azure.Redis](https://www.nuget.org/packages/Aspire.Hosting.Azure.Redis) NuGet package:

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

### Add Azure Managed Redis resource

In your AppHost project, call `AddAzureRedisEnterprise` on the `builder` instance to add an Azure Managed Redis resource, as shown in the following example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddAzureRedisEnterprise("azredisent");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(cache);

// After adding all resources, run the app...
```

The preceding call to `AddAzureRedisEnterprise` configures the Redis server resource to be deployed as an [Azure Managed Redis](/azure/azure-cache-for-redis/cache-overview) instance with enterprise features.

> [!IMPORTANT]
> By default, `AddAzureRedisEnterprise` configures [Microsoft Entra ID](/azure/azure-cache-for-redis/cache-azure-active-directory-for-authentication) authentication. This requires changes to applications that need to connect to these resources, for example, client integrations.

> [!TIP]
> When you call `AddAzureRedisEnterprise`, it implicitly calls <xref:Aspire.Hosting.AzureProvisionerExtensions.AddAzureProvisioning*>—which adds support for generating Azure resources dynamically during app startup. The app must configure the appropriate subscription and location. For more information, see [Local provisioning: Configuration](../azure/local-provisioning.md#configuration).

### Connect to an existing Azure Managed Redis

You might have an existing Azure Managed Redis resource that you want to connect to. You can chain a call to annotate that your Azure Managed Redis resource is an existing resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var existingRedisName = builder.AddParameter("existingRedisName");
var existingRedisResourceGroup = builder.AddParameter("existingRedisResourceGroup");

var cache = builder.AddAzureRedisEnterprise("azredisent")
                   .AsExisting(existingRedisName, existingRedisResourceGroup);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(cache);

// After adding all resources, run the app...
```

[!INCLUDE [azure-configuration](../azure/includes/azure-configuration.md)]

For more information on treating Azure Managed Redis resources as existing resources, see [Use existing Azure resources](../azure/integrations-overview.md#use-existing-azure-resources).

> [!NOTE]
> Alternatively, instead of representing an Azure Managed Redis resource, you can add a connection string to the AppHost. This approach is weakly-typed, and doesn't work with role assignments or infrastructure customizations. For more information, see [Add existing Azure resources with connection strings](../azure/integrations-overview.md#add-existing-azure-resources-with-connection-strings).

### Configure the Azure Managed Redis resource to use access key authentication

By default, the Azure Managed Redis resource is configured to use [Microsoft Entra ID](/azure/azure-cache-for-redis/cache-azure-active-directory-for-authentication) authentication. If you want to use password authentication (not recommended), you can configure the server to use password authentication by calling the `WithAccessKeyAuthentication` method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddAzureRedisEnterprise("azredisent")
                   .WithAccessKeyAuthentication();

builder.AddProject<Projects.ExampleProject>()
       .WithReference(cache);

// After adding all resources, run the app...
```

The preceding code configures the Azure Managed Redis resource to use access key authentication. This alters the generated Bicep to use access key authentication instead of Microsoft Entra ID authentication. In other words, the connection string will contain a password, and will be added to an Azure Key Vault secret.

## Client integration

To get started with the .NET Aspire Azure Managed Redis client integration, install the [📦 Aspire.Microsoft.Azure.StackExchangeRedis](https://www.nuget.org/packages/Aspire.Microsoft.Azure.StackExchangeRedis) NuGet package in the client-consuming project, that is, the project for the application that uses the Azure Managed Redis client. The Azure Managed Redis client integration registers an [IConnectionMultiplexer](https://stackexchange.github.io/StackExchange.Redis/Basics) instance that you can use to interact with Azure Managed Redis.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Microsoft.Azure.StackExchangeRedis
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Microsoft.Azure.StackExchangeRedis"
                  Version="*" />
```

---

### Add Redis client

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireRedisExtensions.AddRedisClient*> extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register an `IConnectionMultiplexer` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddRedisClient(connectionName: "azredisent");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the Azure Managed Redis resource in the AppHost project. For more information, see [Add Azure Managed Redis resource](#add-azure-managed-redis-resource).

You can then retrieve the `IConnectionMultiplexer` instance using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(IConnectionMultiplexer connectionMux)
{
    // Use connection multiplexer...
}
```

For more information on dependency injection, see [.NET dependency injection](/dotnet/core/extensions/dependency-injection).

### Add Azure Managed Redis authenticated client

By default, when you call `AddAzureRedisEnterprise` in your Azure Managed Redis hosting integration, it configures Microsoft Entra ID. To enable authentication in your client application, use the `WithAzureAuthentication()` method:

```csharp
builder.AddRedisClient("azredisent")
       .WithAzureAuthentication();
```

This simplified approach automatically configures the Redis client to use Azure authentication with the appropriate credentials.

### Add keyed Redis client

There might be situations where you want to register multiple `IConnectionMultiplexer` instances with different connection names. To register keyed Redis clients, call the <xref:Microsoft.Extensions.Hosting.AspireRedisExtensions.AddKeyedRedisClient*> method:

```csharp
builder.AddKeyedRedisClient(name: "chat");
builder.AddKeyedRedisClient(name: "queue");
```

Then you can retrieve the `IConnectionMultiplexer` instances using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(
    [FromKeyedServices("chat")] IConnectionMultiplexer chatConnectionMux,
    [FromKeyedServices("queue")] IConnectionMultiplexer queueConnectionMux)
{
    // Use connections...
}
```

For more information on keyed services, see [.NET dependency injection: Keyed services](/dotnet/core/extensions/dependency-injection#keyed-services).

### Configuration

The .NET Aspire Stack Exchange Redis client integration provides multiple options to configure the Redis connection based on the requirements and conventions of your project.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `AddAzureRedisEnterprise`:

```csharp
builder.AddAzureRedisEnterprise("azredisent");
```

Then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "azredisent": "your-azure-redis-enterprise-connection-string"
  }
}
```

For more information on how to format this connection string, see the [Stack Exchange Redis configuration docs](https://stackexchange.github.io/StackExchange.Redis/Configuration.html#basic-configuration-strings).

#### Use configuration providers

[!INCLUDE [redis-client-json-settings](includes/redis-client-json-settings.md)]

#### Use inline delegates

You can also pass the `Action<StackExchangeRedisSettings>` delegate to set up some or all the options inline, for example to configure `DisableTracing`:

```csharp
builder.AddRedisClient(
    "azredisent",
    static settings => settings.DisableTracing = true);
```

[!INCLUDE [redis-client-health-checks-and-diagnostics](includes/redis-client-health-checks-and-diagnostics.md)]

## See also

- [Azure Cache for Redis docs](/azure/azure-cache-for-redis/)
- [Azure Managed Redis blog post](https://redis.io/blog/introducing-azure-managed-redis/)
- [Stack Exchange Redis docs](https://stackexchange.github.io/StackExchange.Redis/)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)

[!INCLUDE [redis-trademark](includes/redis-trademark.md)]
