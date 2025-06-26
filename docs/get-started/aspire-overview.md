---
title: Aspire overview
description: Learn about Aspire, an application stack designed to improve the experience of building distributed applications.
ms.date: 06/26/2025
---

# Aspire overview

Aspire streamlines building, running, debugging, and deploying distributed apps. Picture your app as a set of services, databases, and frontends—when they're deployed, they all work together seamlessly, but during development they need to be individually started and connected. With Aspire, you get a unified toolchain that eliminates complex configs and makes local debugging effortless. Launch and debug your entire app with a single command. Ready to deploy? Aspire lets you publish anywhere—Kubernetes, the cloud, or your own servers. It's also fully extensible, so you can integrate your favorite tools and services with ease.

For the official support information, see the [Aspire Support Policy](https://dotnet.microsoft.com/platform/support/policy/aspire).

## The app host

Aspire's app host is where you define your app's services and dependencies in code—no complex configs required. Easily map out your architecture and let Aspire handle the local orchestration, so you can focus on building features.

A simple example might represent a common three-tier architecture with a frontend that depends on an API, which in turn connects to a database. This could be represented in the app host as shown in the following code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add database service
var postgres = builder.AddPostgres("db")
    .AddDatabase("appdata")
    .WithDataVolume();

// Add API service and reference the database
var api = builder.AddProject<Projects.ApiService>("api")
    .WithReference(postgres)
    .WaitFor(postgres);

// Add frontend service and reference the API
var frontend = builder.AddProject<Projects.Frontend>("frontend")
    .WithReference(api);

builder.Build().Run();
```

Regardless of the language you choose, Aspire provides a consistent way to define your app's architecture. You can easily add services, set up dependencies, and configure how they interact—all in a straightforward, code-first manner.

For more information, see [Aspire orchestration overview](../fundamentals/app-host-overview.md).

## Modeled as resources

Aspire makes it easy to define everything your app needs—frontends, APIs, databases, and more—using the unified app host model. Just describe your resources in code, and Aspire handles the connections for you. Resources can include:

- AI services
- Caches
- Containers
- Databases
- Executables
- Frameworks
- Messaging services
- Projects
- Storage

One resource can depend on another, and Aspire automatically wires them together. This means you can focus on building your app without worrying about the underlying infrastructure.

Under the hood, every integration is either a container or executable, meaning you can add any container image, codebase, script, or cloud resource to your app host. Creating reusable Aspire integrations is just like creating a reusable UI component. It can be as simple or complex as you need, and is fully shareable.

For more information, see [Aspire integrations](../fundamentals/integrations-overview.md).

## Reusable app topology

When you compose your distributed app in Aspire's app host, you're not just defining services for local development and orchestration—you're also setting up the foundation for deployment. The same composition you use to run and debug locally is leveraged when you publish your app, ensuring consistency from development through to production. Likewise, Aspire doesn't get in the way of your existing deployment workflows.

Continuing from the three-tier architecture example, you can deploy the same app topology to various environments, whether it's a local machine, a cloud provider, or your own servers. Consider the following table that illustrates how the same resources can be deployed across different platforms:

| Resource | Local development | Azure | AWS |
|----------|-------------------|-------|-----|
| Frontend | `npm run` | Azure Container Apps | ECS or App Runner |
| API Service | `dotnet run` | Azure Container Apps | ECS or Lambda |
| Database | Docker container | Azure Database for PostgreSQL | RDS or Aurora |

Aspire's deployment capabilities are flexible and extensible, allowing you to adapt to your preferred infrastructure. Aspire doesn't get in the way of your existing deployment workflows, so you can continue using your favorite tools and services.

For more information, see [Deploy Aspire apps](../deployment/overview.md).

## Next steps

> [!div class="nextstepaction"]
> [Quickstart: Build your first Aspire project](build-your-first-aspire-app.md)
