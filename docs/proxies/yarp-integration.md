---
title: YARP integration
description: Learn how to use the .NET Aspire YARP reverse proxy integration, which includes hosting integration for containerized YARP instances.
ms.date: 09/25/2025
ai-usage: ai-assisted
---

# .NET Aspire YARP integration

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

[YARP (Yet Another Reverse Proxy)](https://microsoft.github.io/reverse-proxy/) is a toolkit for developing high-performance reverse proxy applications. The .NET Aspire YARP integration enables you to create containerized YARP reverse proxy instances with programmatic configuration or external configuration files.

## Hosting integration

The YARP hosting integration models a containerized YARP reverse proxy as the <xref:Aspire.Hosting.Yarp.YarpResource> type. To access this type and its APIs add the [📦 Aspire.Hosting.Yarp](https://www.nuget.org/packages/Aspire.Hosting.Yarp) NuGet package in the [AppHost](xref:dotnet/aspire/app-host) project.

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

In your AppHost project, call <xref:Aspire.Hosting.YarpResourceExtensions.AddYarp*> on the `builder` instance to add a YARP resource:

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

When .NET Aspire adds a YARP resource to the AppHost, it creates a new containerized YARP instance using the [mcr.microsoft.com/dotnet/nightly/yarp](https://mcr.microsoft.com/product/dotnet/nightly/yarp/about) container image. This official Microsoft container image contains a preconfigured YARP reverse proxy server that can be dynamically configured through Aspire's hosting APIs.

The container image provides:

- A lightweight YARP reverse proxy server.
- Support for dynamic configuration through programmatic APIs.
- Integration with .NET service discovery.
- Built-in health checks and monitoring capabilities.

The YARP resource can be configured programmatically using the <xref:Aspire.Hosting.YarpResourceExtensions.WithConfiguration*> method.

### Migration from external configuration files

> [!IMPORTANT]
> Starting with .NET Aspire 9.4, the `WithConfigFile` method has been removed. YARP configuration is now done exclusively through code-based configuration using the `WithConfiguration` method. This provides better IntelliSense support, type safety, and works seamlessly with deployment scenarios.

If you were previously using external JSON configuration files with `WithConfigFile`, you need to migrate to the programmatic configuration approach. For example, the following JSON configuration:

```json
{
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
      }
    },
    "Clusters": {
      "catalog": {
        "Destinations": {
          "catalog": {
            "Address": "https://catalogservice"
          }
        }
      }
    }
  }
}
```

Should be migrated to the following programmatic configuration:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var catalogService = builder.AddProject<Projects.CatalogService>("catalogservice");

var gateway = builder.AddYarp("gateway")
                     .WithConfiguration(yarp =>
                     {
                         yarp.AddRoute("/catalog/{**catch-all}", catalogService)
                             .WithTransformPathRemovePrefix("/catalog");
                     });
```

### Programmatic configuration

The YARP integration provides a fluent API for configuring routes, clusters, and transforms programmatically using the <xref:Aspire.Hosting.IYarpConfigurationBuilder>:

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

To configure the host port that the YARP resource is exposed on, use the <xref:Aspire.Hosting.YarpResourceExtensions.WithHostPort(Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.Yarp.YarpResource},System.Nullable{System.Int32})> method:

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

The YARP integration automatically works with .NET service discovery when targeting resources that implement <xref:Aspire.Hosting.IResourceWithServiceDiscovery>. This enables dynamic endpoint resolution for backend services:

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

For external services, use <xref:Aspire.Hosting.ExternalServiceBuilderExtensions.AddExternalService*>:

```csharp
var externalApi = builder.AddExternalService("external-api")
                         .WithHttpsEndpoint("https://api.example.com");

var gateway = builder.AddYarp("gateway")
                     .WithConfiguration(yarp =>
                     {
                         yarp.AddRoute("/external/{**catch-all}", externalApi);
                     });
```

### Static file serving

YARP can serve static files alongside proxied routes. This is useful for serving frontend applications, documentation, or other static assets. The YARP integration provides two approaches for serving static files:

#### Copy files locally

Use the <xref:Aspire.Hosting.YarpResourceExtensions.WithStaticFiles*> method to copy files from a local directory into the YARP container:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var staticServer = builder.AddYarp("static")
                          .WithStaticFiles("../static");

// After adding all resources, run the app...
```

This approach copies files into the container and uses container files in run mode, and uses a bind mount in publish mode. The static files are served from the root path of the YARP server.

#### Multi-stage Docker builds

For more complex scenarios, such as building a frontend application and serving the compiled assets, you can use a Docker multi-stage build with the <xref:Aspire.Hosting.DockerfileExtensions.WithDockerfile*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var frontend = builder.AddYarp("frontend")
                      .WithStaticFiles()
                      .WithDockerfile("../npmapp");

// After adding all resources, run the app...
```

Create a `Dockerfile` in the specified directory (`../npmapp` in this example) that builds your frontend application and copies the static files to the YARP container:

```dockerfile
# Stage 1: Build React app
FROM node:20 AS builder
WORKDIR /app
COPY . .
RUN npm install
RUN npm run build

# Stage 2: Copy static files to YARP container  
FROM mcr.microsoft.com/dotnet/nightly/yarp:2.3.0-preview.4 AS yarp
WORKDIR /app
COPY --from=builder /app/dist ./wwwroot
```

This approach is useful for:

- Building and serving Single Page Applications (SPAs) like React, Vue, or Angular apps
- Compiling and serving static site generators
- Processing assets with build tools before serving

#### Combining static files with routing

You can combine static file serving with dynamic routing to create hybrid applications:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.ApiService>("api");

var gateway = builder.AddYarp("gateway")
                     .WithStaticFiles("../webapp/dist")
                     .WithConfiguration(yarp =>
                     {
                         // API routes take precedence over static files
                         yarp.AddRoute("/api/{**catch-all}", apiService);
                         
                         // Static files are served for all other routes
                     });

// After adding all resources, run the app...
```

In this configuration, requests to `/api/*` are proxied to the backend service, while all other requests serve static files from the specified directory.

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
