---
title: YARP integration
description: Learn how to use the .NET Aspire YARP reverse proxy integration, which includes hosting integration for containerized YARP instances.
ms.date: 07/25/2025
ai-usage: ai-assisted
---

# .NET Aspire YARP integration

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

[YARP (Yet Another Reverse Proxy)](https://microsoft.github.io/reverse-proxy/) is a toolkit for developing high-performance reverse proxy applications. The .NET Aspire YARP integration enables you to create containerized YARP reverse proxy instances with programmatic configuration or external configuration files.

## Hosting integration

The YARP hosting integration models a containerized YARP reverse proxy as the `YarpResource` type <!-- TODO: Add xref:Aspire.Hosting.Yarp.YarpResource when available -->. To access this type and its APIs add the [📦 Aspire.Hosting.Yarp](https://www.nuget.org/packages/Aspire.Hosting.Yarp) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Yarp
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Yarp"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add YARP resource

In your app host project, call `AddYarp` <!-- TODO: Add xref:Aspire.Hosting.YarpResourceExtensions.AddYarp* when available --> on the `builder` instance to add a YARP resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var catalogService = builder.AddProject<Projects.CatalogService>("catalogservice");
var basketService = builder.AddProject<Projects.BasketService>("basketservice");

var gateway = builder.AddYarp("gateway")
                     .WithConfiguration(yarp =>
                     {
                         // Configure routes programmatically
                         yarp.AddRoute(catalogService);
                         yarp.AddRoute("/api/{**catch-all}", basketService);
                     });

// After adding all resources, run the app...
```

When .NET Aspire adds a YARP resource to the app host, it creates a new containerized YARP instance using the [mcr.microsoft.com/dotnet/nightly/yarp](https://mcr.microsoft.com/product/dotnet/nightly/yarp/about) container image. This official Microsoft container image contains a preconfigured YARP reverse proxy server that can be dynamically configured through Aspire's hosting APIs.

The container image provides:

- A lightweight YARP reverse proxy server.
- Support for dynamic configuration through JSON files or programmatic APIs.
- Integration with .NET service discovery.
- Built-in health checks and monitoring capabilities.

The YARP resource can be configured programmatically using the `WithConfiguration` <!-- TODO: Add xref:Aspire.Hosting.YarpResourceExtensions.WithConfiguration* when available --> method or through external configuration files.

### Add YARP resource with external configuration

To configure the YARP resource using an external JSON configuration file, use the `WithConfigFile` <!-- TODO: Add xref:Aspire.Hosting.YarpServiceExtensions.WithConfigFile* when available --> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var catalogService = builder.AddProject<Projects.CatalogService>("catalogservice");
var basketService = builder.AddProject<Projects.BasketService>("basketservice");

var gateway = builder.AddYarp("gateway")
                     .WithConfigFile("yarp.json")
                     .WithReference(catalogService)
                     .WithReference(basketService);

// After adding all resources, run the app...
```

The `yarp.json` configuration file can reference services using their resource names:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Information"
    }
  },
  "AllowedHosts": "*",
  "ReverseProxy": {
    "Routes": {
      "catalog": {
        "ClusterId": "catalog",
        "Match": {
          "Path": "/catalog/{**catch-all}"
        },
        "Transforms": [
          { "PathRemovePrefix": "/catalog" }
        ]
      },
      "basket": {
        "ClusterId": "basket",
        "Match": {
          "Path": "/basket/{**catch-all}"
        },
        "Transforms": [
          { "PathRemovePrefix": "/basket" }
        ]
      }
    },
    "Clusters": {
      "catalog": {
        "Destinations": {
          "catalog": {
            "Address": "https://catalogservice"
          }
        }
      },
      "basket": {
        "Destinations": {
          "basket": {
            "Address": "https://basketservice"
          }
        }
      }
    }
  }
}
```

> [!NOTE]
> When you use the `WithConfigFile`, programmatic configuration via `WithConfiguration` isn't supported. You must choose one approach or the other.

### Programmatic configuration

The YARP integration provides a fluent API for configuring routes, clusters, and transforms programmatically using the `IYarpConfigurationBuilder` <!-- TODO: Add xref:Aspire.Hosting.IYarpConfigurationBuilder when available -->:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var catalogService = builder.AddProject<Projects.CatalogService>("catalogservice");
var basketService = builder.AddProject<Projects.BasketService>("basketservice");

var gateway = builder.AddYarp("gateway")
                     .WithConfiguration(yarp =>
                     {
                         // Add catch-all route for frontend service 
                         yarp.AddRoute(catalogService);
                         
                         // Add specific path route with transforms
                         yarp.AddRoute("/api/{**catch-all}", basketService)
                             .WithTransformPathRemovePrefix("/api");
                         
                         // Configure route matching
                         yarp.AddRoute("/catalog/api/{**catch-all}", catalogService)
                             .WithMatchMethods("GET", "POST")
                             .WithTransformPathRemovePrefix("/catalog");
                     });

// After adding all resources, run the app...
```

#### Route configuration

Routes define how incoming requests are matched and forwarded to backend services. The YARP integration provides several methods for configuring routes:

- `AddRoute(resource)` - Creates a catch-all route for the specified resource
- `AddRoute(path, resource)` - Creates a route with a specific path pattern
- `AddRoute(path, externalService)` - Creates a route targeting an external service

#### Route matching

You can configure route matching criteria using various `WithMatch*` methods:

```csharp
yarp.AddRoute("/api/{**catch-all}", basketService)
    .WithMatchMethods("GET", "POST")
    .WithMatchHeaders(new RouteHeader("X-Version", "v1"));
```

#### Transforms

Transforms modify requests and responses as they pass through the proxy. The YARP integration supports various transform extensions:

```csharp
yarp.AddRoute("/api/{**catch-all}", basketService)
    .WithTransformPathRemovePrefix("/api")
    .WithTransformPathPrefix("/v1")
    .WithTransformRequestHeader("X-Forwarded-Host", "gateway.example.com")
    .WithTransformResponseHeader("X-Powered-By", "YARP");
```

Common transform methods include:

- **Path transforms**: `WithTransformPathRemovePrefix`, `WithTransformPathPrefix`, `WithTransformPathSet`
- **Header transforms**: `WithTransformRequestHeader`, `WithTransformResponseHeader`
- **Query transforms**: `WithTransformQueryParameter`, `WithTransformQueryRemoveParameter`
- **Custom transforms**: `WithTransform` for custom transformation logic

#### Cluster configuration

Clusters define backend destinations and can be configured with load balancing, health checks, and other policies:

```csharp
yarp.AddRoute("/api/{**catch-all}", basketService)
    .AddCluster(basketService)
    .WithLoadBalancingPolicy("Random")
    .WithHealthCheckConfig(new HealthCheckConfig
    {
        Active = new ActiveHealthCheckConfig
        {
            Enabled = true,
            Interval = TimeSpan.FromSeconds(30),
            Timeout = TimeSpan.FromSeconds(5)
        }
    })
    .WithSessionAffinityConfig(new SessionAffinityConfig
    {
        Enabled = true,
        Policy = "Cookie"
    });
```

### Customize host port

To configure the host port that the YARP resource is exposed on, use the `WithHostPort` <!-- TODO: Add xref:Aspire.Hosting.YarpResourceExtensions.WithHostPort* when available --> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var gateway = builder.AddYarp("gateway")
                     .WithHostPort(8080)
                     .WithConfiguration(yarp =>
                     {
                         // Configure routes...
                     });

// After adding all resources, run the app...
```

By default, YARP uses a randomly assigned port. Using `WithHostPort` allows you to specify a fixed port for consistent access during development.

### Service discovery integration

<!-- TODO : Add xref:Aspire.Hosting.ApplicationModel.IResourceWithServiceDiscovery when available -->

The YARP integration automatically works with .NET service discovery when targeting resources that implement `IResourceWithServiceDiscovery`. This enables dynamic endpoint resolution for backend services:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var catalogService = builder.AddProject<Projects.CatalogService>("catalogservice");

var gateway = builder.AddYarp("gateway")
                     .WithConfiguration(yarp =>
                     {
                         // Service discovery automatically resolves catalogservice endpoints
                         yarp.AddRoute("/catalog/{**catch-all}", catalogService);
                     });
```

<!-- TODO
    Add xref:Aspire.Hosting.DistributedApplicationBuilderExtensions.AddExternalService when available
-->

For external services, use `AddExternalService`:

```csharp
var externalApi = builder.AddExternalService("external-api")
                         .WithHttpsEndpoint("https://api.example.com");

var gateway = builder.AddYarp("gateway")
                     .WithConfiguration(yarp =>
                     {
                         yarp.AddRoute("/external/{**catch-all}", externalApi);
                     });
```

## Client integration

There's no client integration provided for YARP. YARP operates as a reverse proxy server that sits between clients and backend services. Applications interact with YARP through standard HTTP requests to the proxy endpoints, not through a dedicated client library.

To consume services through a YARP proxy in your .NET applications, use standard HTTP client patterns:

```csharp
// In your consuming application
var httpClient = new HttpClient();
var response = await httpClient.GetAsync("https://gateway/api/catalog/products");
```

For applications within the same .NET Aspire solution, you can reference the YARP resource directly:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var gateway = builder.AddYarp("gateway")
                     .WithConfiguration(/* ... */);

// Reference the YARP gateway from other services
builder.AddProject<Projects.WebApp>("webapp")
       .WithReference(gateway);
```

## See also

- [YARP documentation](https://microsoft.github.io/reverse-proxy/)
- [.NET Aspire orchestration overview](../fundamentals/orchestrate-resources.md)
- [Tutorial: Add .NET Aspire to an existing .NET app](../get-started/add-aspire-existing-app.md)
