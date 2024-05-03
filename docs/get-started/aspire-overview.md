---
title: .NET Aspire overview
description: Learn about .NET Aspire, an application stack designed to improve the experience of building cloud-native applications.
ms.date: 04/24/2024
---

# .NET Aspire overview

.NET Aspire is an opinionated, cloud ready stack for building observable, production ready, distributed applications.​ .NET Aspire is delivered through a collection of NuGet packages that handle specific cloud-native concerns. Cloud-native apps often consist of small, interconnected pieces or microservices rather than a single, monolithic code base. Cloud-native apps generally consume a large number of services, such as databases, messaging, and caching.

A _distributed application_ is one that uses computational resources across multiple nodes, such as containers running on different hosts. Such nodes must communicate over network boundaries to deliver responses to users. A cloud-native app is a specific type of distributed app that takes full advantage of the scalability, resilience, and manageability of cloud infrastructures.

## Why .NET Aspire?

.NET Aspire is designed to improve the experience of building .NET cloud-native apps. It provides a consistent, opinionated set of tools and patterns that help you build and run distributed apps. .NET Aspire is designed to help you with:

- [**Orchestration**](#orchestration): .NET Aspire provides features for running and connecting multi-project applications and their dependencies for local development environments.
- [**Components**](#net-aspire-components): .NET Aspire components are NuGet packages for commonly used services, such as Redis or Postgres, with standardized interfaces ensuring they connect consistently and seamlessly with your app.
- [**Tooling**](#project-templates-and-tooling): .NET Aspire comes with project templates and tooling experiences for Visual Studio and the [dotnet CLI](/dotnet/core/tools/) to help you create and interact with .NET Aspire apps.

## Orchestration

In .NET Aspire, orchestration primarily focuses on enhancing the local development experience by simplifying the management of your cloud-native app's configuration and interconnections. It's important to note that .NET Aspire's orchestration is not intended to replace the robust systems used in production environments, such as Kubernetes. Instead, it provides a set of abstractions that streamline the setup of service discovery, environment variables, and container configurations, eliminating the need to deal with low-level implementation details. These abstractions ensure a consistent setup pattern across apps with numerous components and services, making it easier to manage complex applications during the development phase.

.NET Aspire orchestration assists with the following concerns:

- **App composition**: Specify the .NET projects, containers, executables, and cloud resources that make up the application.
- **Service discovery and connection string management**: The app host manages injecting the right connection strings and service discovery information to simplify the developer experience.

For example, using .NET Aspire, the following code creates a local Redis container resource and configures the appropriate connection string in the `"frontend"` project with only two helper method calls:

```csharp
// Create a distributed application builder given the command line arguments.
var builder = DistributedApplication.CreateBuilder(args);

// Add a Redis server to the application.
var cache = builder.AddRedis("cache");

// Add the frontend project to the application and configure it to use the 
// Redis server, defined as a referenced dependency.
builder.AddProject<Projects.MyFrontend>("frontend")
       .WithReference(cache);
```

For more information, see [.NET Aspire orchestration overview](../fundamentals/app-host-overview.md).

## .NET Aspire components

[.NET Aspire components](../fundamentals/components-overview.md) are NuGet packages designed to simplify connections to popular services and platforms, such as Redis or PostgreSQL. .NET Aspire components handle many cloud-native concerns for you through standardized configuration patterns, such as adding health checks and telemetry.

Each component is designed to work with .NET Aspire orchestration, and they're capable of flowing their configurations through dependencies based on .NET project and package references. In other words, if _Example.ServiceFoo_ references _Example.ServiceBar_, _Example.ServiceFoo_ inherits the component's required configurations to allow them to communicate with each other automatically.

For example, consider the following code using the .NET Aspire Service Bus component:

```csharp
builder.AddAzureServiceBusClient("servicebus");
```

The <xref:Microsoft.Extensions.Hosting.AspireServiceBusExtensions.AddAzureServiceBusClient%2A> method handles the following concerns:

- Registers a <xref:Azure.Messaging.ServiceBus.ServiceBusClient> as a singleton in the DI container for connecting to Azure Service Bus.
- Applies `ServiceBusClient` configurations either inline through code or through configuration.
- Enables corresponding health checks, logging and telemetry specific to the Azure Service Bus usage.

A full list of available components is detailed on the [.NET Aspire components](../fundamentals/components-overview.md) overview page.

## Project templates and tooling

.NET Aspire apps follow a standardized structure designed around the default .NET Aspire project templates. Most .NET Aspire apps have at least three projects:

- **MyFirstAspireApp**: Your starter app, which could be any common .NET project such as a Blazor UI or Minimal API. You can add more projects to your app as it expands and manage orchestration between them using the **.AppHost** and **.ServiceDefaults** project.
- **MyFirstAspireApp.AppHost**: The **.AppHost** project is used to manage the high level orchestration concerns of the app. Orchestration involves putting together various parts of your app, like APIs, service containers, and executables, and setting up how they find and communicate with each other.
- **MyFirstAspireApp.ServiceDefaults**: The **.ServiceDefaults** project contains default .NET Aspire app configurations that can be extended and customized as needed. These configurations include concerns such as setting up health checks, OpenTelemetry settings, and more.

There are currently two .NET Aspire starter templates available to help you get started with this structure:

- **.NET Aspire Application**: A basic starter template that only includes the **AspireSample.AppHost** and **AspireSample.ServiceDefaults** projects. This template is designed to only provide the essentials for you to build off of.
- **.NET Aspire Starter Application**: This template includes the **AspireSample.AppHost** and **AspireSample.ServiceDefaults** projects, but also includes boilerplate UI and API projects. These projects are pre-configured with service discovery and other basic examples of common .NET Aspire functionality.

For more information, see [.NET Aspire setup and tooling](../fundamentals/setup-tooling.md).

.NET Aspire templates also include boilerplate extension methods that handle common service configurations for you:

```csharp
builder.AddServiceDefaults();
```

For more information on what `AddServiceDefaults` does, see [.NET Aspire service defaults](../fundamentals/service-defaults.md).

When added to your _Program.cs_ file, the preceding code handles the following concerns:

- **OpenTelemetry**: Sets up formatted logging, runtime metrics, built-in meters, and tracing for ASP.NET Core, gRPC, and HTTP. For more information, see [.NET Aspire telemetry](../fundamentals/telemetry.md).
- **Default health checks**: Adds default health check endpoints that tools can query to monitor your app. For more information, see [.NET app health checks in C#](/dotnet/core/diagnostics/diagnostic-health-checks).
- **Service discovery**: Enables [service discovery](../service-discovery/overview.md) for the app and configures <xref:System.Net.Http.HttpClient> accordingly.

## Next steps

> [!div class="nextstepaction"]
> [Quickstart: Build your first .NET Aspire app](build-your-first-aspire-app.md)
