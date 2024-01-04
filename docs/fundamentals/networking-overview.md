---
title: .NET Aspire networking overview
description: Learn how .NET Aspire handles networking and service bindings, and how you can use them in your app code.
ms.date: 01/04/2024
---

# Networking in .NET Aspire

One of the advantages of developing with .NET Aspire is that it enables you to develop, test, and debug cloud-native apps locally. Networking is a key aspect of .NET Aspire, as it allows your apps to communicate with each other and with external services. In this article, you will learn how .NET Aspire handles networking in different scenarios and environments.

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

## How .NET Aspire handles ports and proxies

When you create a service binding, either implicitly or explicitly, .NET Aspire will assign two ports to your service binding: a **host port** and a **service port**. The host port is the port that the clients of your service will connect to, and the service port is the port that your service will listen on.

.NET Aspire will always launch a **proxy** on the host port that you specify for the service binding. The proxy is a lightweight and fast reverse proxy that handles the routing and load balancing of the requests from your clients to your service. The proxy is mostly an implementation detail from an Aspire perspective, and you don't need to worry about its configuration or management.

Your service, whether it is an executable, a project, or a container, will be assigned another port by .NET Aspire, which is different from the host port that the proxy listens on. This is to avoid port conflicts and to enable multiple service bindings for your service. Depending on the type of your service, .NET Aspire will inject the appropriate environment variables to your service to indicate the port that it should listen on and the port that the proxy listens on.

The following diagram illustrates how .NET Aspire handles ports and proxies for a service binding:

:::image type="content" source="media/networking/networking-proxies.png" lightbox="media/networking/networking-proxies.png" alt-text="A visual representation of a frontend, api, and redis resource, with their corresponding ports.":::

As you can see, the client connects to the host port 5066, which is handled by the proxy. The proxy then forwards the request to the service port 5000, which is listened by the service. The service can use the `ASPNETCORE_URLS` environment variable to get the URL that it should listen on, such as `http://localhost:5000`.

If you specify multiple host ports or schemes for a service binding, .NET Aspire will launch multiple proxies for each port and scheme combination, and assign different service ports for each proxy. For example, if you specify a host port 5066 with the scheme `http`, and another host port 7239 with the scheme `https`, .NET Aspire will launch two proxies, one on port 5066 and another on port 7239, and assign two different service ports, such as 5000 and 5001, for each proxy.

If you specify multiple replicas for a service binding, .NET Aspire will launch multiple instances of your service, each with a different service port, and use the proxy to load balance the requests among the replicas. For example, if you specify two replicas for a service binding, .NET Aspire will launch two instances of your service, each with a different service port, such as 5000 and 5001, and use the proxy to distribute the requests between them.

If you do not specify a host port for a service binding, .NET Aspire will generate a random port for both the host port and the service port. This can be useful if you want to create a service binding without exposing it to the clients, or if you want to avoid port conflicts with other services.

## Networking in the outer loop

The outer loop is the process of deploying and running your app in a target environment, such as Azure, Kubernetes, or Docker. Networking in the outer loop is different from networking in the inner loop, as it involves different tools, platforms, and configurations. .NET Aspire provides several features and guidance to help you with the networking challenges and considerations in the outer loop, such as:

- **Deployment profiles**: Deployment profiles are configuration files that specify how to deploy your app to a target environment. You can use deployment profiles to define the deployment settings, environment variables, and service bindings for your app. .NET Aspire automatically creates and updates deployment profiles for your app based on the target environment and the services you use.
- **Service discovery**: Service discovery is the process of finding and connecting to the services that your app depends on in a target environment. Service discovery can be challenging in a dynamic and distributed environment, where the services may change their location, address, or port frequently. .NET Aspire provides several options and recommendations for service discovery in different target environments, such as using DNS, environment variables, or service meshes.

## Summary

In this article, you learned how .NET Aspire handles networking and service bindings, and how you can use them in your app code. You learned how to create and manage service bindings, both implicitly and explicitly, and how .NET Aspire launches and manages the Proxies for your service bindings. You learned how to access service bindings from your app code using the standard .NET configuration APIs, and how .NET Aspire exposes the service bindings as configuration sources in your app. You also learned how .NET Aspire handles the networking and service bindings for you, both locally and in the cloud, and how you can deploy your app to different target environments using the `aspire deploy` command or the .NET Aspire CLI.

Service bindings are a powerful and convenient feature of .NET Aspire that enable you to develop, test, and deploy cloud-native apps using .NET. By using service bindings, you can easily access the services and resources that your app depends on, without having to worry about the configuration details or the deployment environment. .NET Aspire takes care of the networking and service bindings for you, and lets you focus on your app logic and code.
