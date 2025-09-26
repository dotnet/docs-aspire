---
title: .NET Aspire Docker hosting integration
description: Learn how to use the .NET Aspire Docker hosting integration to deploy your app with Docker Compose.
ms.date: 09/26/2025
ai-usage: ai-generated
---

# .NET Aspire Docker hosting integration

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

The .NET Aspire Docker hosting integration enables you to deploy your .NET Aspire applications using Docker Compose. This integration provides a hosting environment that generates Docker Compose files from your app model, allowing you to deploy your distributed application as a collection of Docker containers.

## Overview

The Docker hosting integration models Docker Compose environments as compute resources that can host your application services. When you use this integration, Aspire generates Docker Compose files that define all the services, networks, and volumes needed to run your application in a containerized environment.

## Supported scenarios

The Docker hosting integration supports the following scenarios:

- **Docker Compose deployment**: Generate Docker Compose files from your app model for deployment.
- **Container orchestration**: Orchestrate multiple services using Docker Compose.
- **Telemetry integration**: Include an Aspire dashboard for telemetry visualization.
- **Environment configuration**: Configure environment variables and service dependencies.
- **Network management**: Manage container networking and service discovery.

## Hosting integration

The Docker hosting integration is available in the [📦 Aspire.Hosting.Docker](https://www.nuget.org/packages/Aspire.Hosting.Docker) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Docker
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Docker"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Usage example

The following example demonstrates how to add a Docker Compose environment to your app model:

:::code source="snippets/docker/AppHost.cs":::

The preceding code:

- Creates a Docker Compose environment named `compose`.
- Adds a Redis cache service that will be included in the Docker Compose deployment.
- Adds an API service project that will be containerized and included in the deployment.
- Adds a web application that references both the cache and API service.
- Configures all services to be published as Docker Compose services.

## Configuration options

### Configure environment properties

You can configure various properties of the Docker Compose environment:

```csharp
var compose = builder.AddDockerComposeEnvironment("compose")
                     .WithProperties(env =>
                     {
                         env.DefaultContainerRegistry = "myregistry.azurecr.io";
                         env.DefaultNetworkName = "my-network";
                         env.BuildContainerImages = true;
                     });
```

### Configure the Docker Compose file

You can customize the generated Docker Compose file:

```csharp
var compose = builder.AddDockerComposeEnvironment("compose")
                     .ConfigureComposeFile(composeFile =>
                     {
                         composeFile.Networks.Add("custom-network", new()
                         {
                             Driver = "bridge"
                         });
                     });
```

### Configure the dashboard

The Docker hosting integration includes an Aspire dashboard for telemetry visualization. You can configure or disable it:

```csharp
// Enable dashboard with custom configuration
var compose = builder.AddDockerComposeEnvironment("compose")
                     .WithDashboard(dashboard =>
                     {
                         dashboard.WithHostPort(8080)
                                  .WithForwardedHeaders(enabled: true);
                     });

// Disable dashboard
var compose = builder.AddDockerComposeEnvironment("compose")
                     .WithDashboard(enabled: false);
```

## Publishing and deployment

To deploy your application using Docker Compose, use the `aspire publish` command:

```dotnetcli
aspire publish -o docker-compose-artifacts
```

This command generates Docker Compose files and all necessary artifacts in the specified output directory. The generated files include:

- `docker-compose.yml`: The main Docker Compose file defining all services
- `docker-compose.override.yml`: Override file for development-specific settings
- `.env`: Environment variables file
- Service-specific configuration files and scripts

After publishing, you can deploy your application using Docker Compose:

```bash
cd docker-compose-artifacts
docker compose up -d
```

## Environment variables

The Docker hosting integration captures environment variables from your app model and includes them in a `.env` file. This ensures that all configuration is properly passed to the containerized services.

## Next steps

- [Deploy .NET Aspire projects to Azure Container Apps using the Aspire CLI](aspire-deploy/aca-deployment-aspire-cli.md)
- [Building custom deployment pipelines](../fundamentals/custom-deployments.md)
- [Docker Compose to AppHost API reference](../get-started/docker-compose-to-apphost-reference.md)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
