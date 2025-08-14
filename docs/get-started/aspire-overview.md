---
title: Aspire overview
description: Learn about Aspire, an application stack designed to improve the experience of building distributed applications.
ms.date: 07/21/2025
ms.custom: sfi-ropc-nochange
---

# Aspire overview

**Aspire** provides tools, templates, and packages for building **observable, production-ready distributed apps**. At the center is the **app model**—a **code-first, single source of truth** that defines your app's services, resources, and connections.

Aspire gives you a **unified toolchain**: launch and debug your entire app locally with one command, then deploy anywhere—**Kubernetes, the cloud, or your own servers**—using the same composition.

Extensibility is a core focus. Aspire's APIs are designed so you can adapt the platform to your infrastructure, services, and workflows.

### Key capabilities

- **AppHost orchestration:** Define services, dependencies, and configuration in code.
- **Rich integrations:** NuGet packages for popular services with standardized interfaces.
- **Consistent tooling:** Project templates and experiences for **Visual Studio, VS Code, and the CLI.**

For the official support information, see the [Aspire Support Policy](https://dotnet.microsoft.com/platform/support/policy/aspire).

## The AppHost

Aspire's AppHost is where you define your app's services and dependencies in code—no complex configuration files required. The AppHost provides orchestration for your local development environment by simplifying the management of service discovery, environment variables, and container configurations.

Picture a common three-tier architecture: a frontend, which depends on an API, which connects to a database. In Aspire, this topology is represented in the AppHost as shown in the following code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add database service
var postgres = builder.AddPostgres("db")
    .AddDatabase("appdata")
    .WithDataVolume();

// Add API service and reference dependencies
var api = builder.AddProject<Projects.ApiService>("api")
    .WithReference(postgres)
    .WaitFor(postgres);

// Add frontend service and reference the API
var frontend = builder.AddProject<Projects.Frontend>("frontend")
    .WithReference(api);

builder.Build().Run();
```

The AppHost assists with the following concerns:

- **App composition**: Specify the projects, containers, executables, and cloud resources that make up your application.
- **Service discovery and connection string management**: Automatically inject the right connection strings and network configurations.

It's important to note that Aspire's orchestration focuses on enhancing the _local development_ experience. It's not intended to replace production systems like Kubernetes, but rather provides abstractions that eliminate low-level implementation details during development.

For more information, see [Aspire orchestration overview](../fundamentals/app-host-overview.md).

## Aspire integrations

Aspire makes it easy to define everything your app needs using integrations—NuGet packages designed to simplify connections to popular services and platforms. Each integration handles cloud resource setup and provides standardized patterns for health checks, telemetry, and configuration.

Resources you can integrate include:

- **AI Services**: Large Language Models, AI endpoints, and cognitive services.
- **Caches**: Redis, in-memory caches, and distributed caching solutions.
- **Containers**: Docker containers for databases, message brokers, and other services.
- **Databases**: SQL Server, PostgreSQL, MySQL, MongoDB, and other data stores.
- **Executables**: Console applications, scripts, and background services.
- **Frameworks**: Web applications, APIs, and microservices built with various frameworks.
- **Messaging Services**: Azure Service Bus, RabbitMQ, Kafka, and other messaging systems.
- **Projects**: .NET projects, Node.js applications, Python services, and more.
- **Storage**: Blob storage, file systems, and cloud storage services.

Integrations are two-fold: "hosting" integrations represent the service you're connecting to, while "client" integrations represent the consumer of that service.

> [!TIP]
> Under the hood, a _hosting_ [integration](../fundamentals/integrations-overview.md) can represent a container, an executable, or even just C# code that configures resources without running a separate process. You can add any container image, codebase, script, or cloud resource to your AppHost. Creating reusable Aspire integrations is similar to building reusable components for your apps.

## Monitor and troubleshoot with the Aspire dashboard

Aspire includes a powerful developer dashboard that gives you real-time visibility into your distributed app. The dashboard lets you inspect resources, view logs, traces, and metrics, and manage your app's services—all from a single UI.

When you run your Aspire app, the dashboard launches automatically. You can:

- See all your app's resources and their status.
- Drill into logs, traces, and metrics for any service.
- Start, stop, or restart resources directly from the dashboard.
- Visualize dependencies and troubleshoot issues faster.

The dashboard is available both as part of an Aspire solution or as a [standalone tool](../fundamentals/dashboard/standalone.md) for any app that emits OpenTelemetry data.

Learn more in the [dashboard overview](../fundamentals/dashboard/overview.md), or dive deeper into [dashboard features and usage](../fundamentals/dashboard/explore.md).

## From development to deployment

When you compose your distributed app in Aspire's AppHost, you're not just defining services for local development—you're setting up the foundation for deployment. The same composition you use to run and debug locally becomes the blueprint for production deployment, ensuring consistency from development through to production.

Aspire provides project templates and tooling experiences for your favorite development environments. These [templates include opinionated defaults](../fundamentals/aspire-sdk-templates.md) with boilerplate code for health checks, logging, and telemetry. The templates also include service defaults that handle common configurations:

```csharp
builder.AddServiceDefaults();
```

When added to your C# code, this method configures:

- **OpenTelemetry**: Formatted logging, runtime metrics, and tracing for ASPCore, gRPC, and HTTP.
- **Health checks**: Default endpoints that tools can query to monitor your app.
- **Service discovery**: Enables service discovery and configures <xref:System.Net.Http.HttpClient> accordingly.

For more information, see [Aspire service defaults](../fundamentals/service-defaults.md).

Consider how the three-tier architecture example can be deployed across different environments:

| **Resource** | **Local development** | **Azure** | **AWS** |
|----------|-------------------|-------|-----|---------|
| Frontend | `npm run` | Azure Container Apps | Amazon Elastic Container Service |
| API service | `dotnet run` | Azure Container Apps | AWS Lambda |
| Database | `docker.io/library/postgres` | Azure Database for PostgreSQL | Amazon Relational Database Service |

> [!TIP]
> These are just a few examples of how you can deploy Aspire apps.

Aspire's deployment capabilities are flexible and don't interfere with your existing workflows. You can continue using your preferred tools and services while benefiting from the consistent app topology defined in your AppHost.

For more information, see [Deploy Aspire apps](../deployment/overview.md).

## Next steps

> [!div class="nextstepaction"]
> [Build your first Aspire solution](build-your-first-aspire-app.md)
