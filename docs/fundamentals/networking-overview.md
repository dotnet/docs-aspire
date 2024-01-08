---
title: .NET Aspire networking overview
description: Learn how .NET Aspire handles networking and service bindings, and how you can use them in your app code.
ms.date: 01/08/2024
---

# Networking in .NET Aspire

One of the advantages of developing with .NET Aspire is that it enables you to develop, test, and debug cloud-native apps locally. Inner-loop networking is a key aspect of .NET Aspire, that allows your apps to communicate with each other in your development environment. In this article, you learn how .NET Aspire handles networking in different scenarios and environments.

## Networking in the inner loop

The inner loop is the process of developing and testing your app locally before deploying it to a target environment. .NET Aspire provides several tools and features to simplify and enhance the networking experience in the inner loop, such as:

- **Launch profiles**: Launch profiles are configuration files that specify how to run your app locally. You can use launch profiles to define the service bindings, environment variables, and launch settings for your app.
- **Service bindings**: Service bindings are the connections between your app and the services it depends on, such as databases, message queues, or APIs. Service bindings provide information such as the service name, address, port, credentials, and protocol. You can add service bindings to your app either implicitly (via launch profiles) or explicitly by calling <xref:Aspire.Hosting.ResourceBuilderExtensions.WithServiceBinding%2A>.
- **Proxies**: .NET Aspire automatically launches a proxy for each service binding you add to your app, and assigns a port for the proxy to listen on. The proxy then forwards the requests to the port that your app listens on, which may be different from the proxy port. This way, you can avoid port conflicts and access your app and services using consistent and predictable URLs.

## How service bindings work

A service binding consists of two parts: a **service** and a **binding**. A service is a representation of an external resource or dependency that your app needs to access, such as a database, a message queue, or an API. A binding is a connection between your app and a service, which provides the necessary information and credentials for your app to access the service.

.NET Aspire supports two types of service bindings: **implicit** and **explicit**. Implicit service bindings are created automatically by .NET Aspire based on the launch profiles that you specify in your app. Launch profiles are configuration files that define how your app should run in different environments, such as development, testing, or production. Explicit service bindings are created manually by you using the <xref:Aspire.Hosting.ResourceBuilderExtensions.WithServiceBinding%2A>.

When you create a service binding, either implicitly or explicitly, .NET Aspire will launch a **proxy** on the port that you specify for the binding. The proxy is a lightweight and fast reverse proxy that handles the routing and load balancing of the requests from your app to the service. The proxy is mostly an implementation detail from an Aspire perspective, and you don't need to worry about its configuration or management.

Your app, whether it is an executable, a project, or a container, will be assigned another port by .NET Aspire, which is different from the port that the proxy listens on. This is to avoid port conflicts and to enable multiple service bindings for your app. Depending on the type of your app, .NET Aspire will inject the appropriate environment variables to your app to indicate the port that it should listen on and the port that the proxy listens on.

For example, if your app is a project, .NET Aspire will automatically inject the `ASPNETCORE_URLS` environment variable to your app, which contains the URL that your app should listen on, such as `http://localhost:5000`. If your app is a container, .NET Aspire will randomly assign the ports using the magic of Docker, and you can use the `aspire service list` command or the .NET Aspire CLI to see the ports that are assigned to your container and the proxy. If your app is an executable, .NET Aspire will optionally inject an environment variable with the port assigned for each service binding, such as `ASPNETCORE_URLS_MyService=http://localhost:5001`. You can use this environment variable in your app code to figure out what port to bind to locally.

## Launch profiles

When you call <xref:Aspire.Hosting.ProjectResourceBuilderExtensions.AddProject%2A>, the app host looks for _Properties/launchSettings.json_ to determine the default set of service bindings. The app host selects a specific launch profile using the following rules:

1. An explicit <xref:Aspire.Hosting.ProjectResourceBuilderExtensions.WithLaunchProfile%2A> call on the `IResourceBuilder<ProjectResource>`.
1. The `DOTNET_LAUNCH_PROFILE` environment variable. For more information, see [.NET environment variables](/dotnet/core/tools/dotnet-environment-variables).
1. Picking the first launch profile defined in _launchSettings.json_.

Consider the following profile:

```json
{
  "$schema": "http://json.schemastore.org/launchsettings.json",
  "iisSettings": {
    "windowsAuthentication": false,
    "anonymousAuthentication": true,
    "iisExpress": {
      "applicationUrl": "http://localhost:64225",
      "sslPort": 44368
    }
  },
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "http://localhost:5066",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "https://localhost:7239;http://localhost:5066",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "IIS Express": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

### Example

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.WebApplication>("frontend")
       .WithLaunchProfile("https");

builder.Build().Run();
```

This will select the **https** launch profile from _launchSettings.json_. The `applicationUrl` of that launch profile will be used to create a service binding for this project. This is the equivalent of:

```csharp
builder.AddProject<Projects.WebApplication>("frontend")
       .WithServiceBinding(hostPort: 5066, scheme: "http")
       .WithServiceBinding(hostPort: 7239, scheme: "https");
```

## What happens if there's no launchSettings.json?

If there's no _launchSettings.json_ or there's no launch profile there will be no bindings by default.

## Ports and proxies

When defining a service binding, the host port is *always* given to the proxy that sits in front of the service. This allows single or multiple replicas of a service to behave similarly.

```csharp
builder.AddProject<Projects.WebApplication>("frontend")
       .WithServiceBinding(hostPort: 5066, scheme: "http")
       .WithReplicas(2);
```

:::image type="content" source="media/networking/proxy-with-replicas.png" lightbox="media/networking/proxy-with-replicas.png" alt-text=".NET Aspire frontend app networking diagram with specific host port and two replicas.":::

```csharp
builder.AddProject<Projects.WebApplication>("frontend")
       .WithServiceBinding(hostPort: 5066, scheme: "http")
```

There will be 2 ports defined:

- The proxy port 5066
- The port that the underlying service will be bound to (a randomly chosen port)

:::image type="content" source="media/networking/proxy-host-port-and-random-port.png" lightbox="media/networking/proxy-host-port-and-random-port.png" alt-text=".NET Aspire frontend app networking diagram with specific host port and random port.":::

The underlying service is fed this port via `ASPNETCORE_URLS` for project resources. Other resources can get access to this port by specifying an environment variable on the service binding:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddNpmApp("frontend", "../NodeFrontend", "watch")
       .WithServiceBinding(hostPort: 5067, scheme: "http", env: "PORT");

builder.Build().Run();
```

The above code will make the random port available in the PORT env variable. The application can use this to listen to incoming connections from the proxy.

:::image type="content" source="media/networking/proxy-with-env-var-port.png" lightbox="media/networking/proxy-with-env-var-port.png" alt-text=".NET Aspire frontend app networking diagram with specific host port and environment variable port.":::

## No host port

No host port on a service binding will generate a random port for both the host port and the service port.

```csharp
builder.AddProject<Projects.WebApplication>("frontend")
       .WithServiceBinding(scheme: "http");
```

:::image type="content" source="media/networking/proxy-with-random-ports.png" lightbox="media/networking/proxy-with-random-ports.png" alt-text=".NET Aspire frontend app networking diagram with random host port and proxy port.":::
