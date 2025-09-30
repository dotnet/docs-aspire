---
title: Aspire Docker hosting integration
description: Learn how to use the Aspire Docker hosting integration to deploy your app with Docker Compose.
ms.date: 09/30/2025
ai-usage: ai-generated
---

# Aspire Docker hosting integration

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

The Aspire Docker hosting integration enables you to deploy your Aspire applications using Docker Compose. This integration models Docker Compose environments as compute resources that can host your application services. When you use this integration, Aspire generates Docker Compose files that define all the services, networks, and volumes needed to run your application in a containerized environment. It supports generating Docker Compose files from your app model for deployment, orchestrating multiple services, including an Aspire dashboard for telemetry visualization, configuring environment variables and service dependencies, and managing container networking and service discovery.

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

### Add Docker Compose environment resource

The following example demonstrates how to add a Docker Compose environment to your app model using the <xref:Aspire.Hosting.DockerComposeEnvironmentExtensions.AddDockerComposeEnvironment%2A> method:

:::code source="snippets/docker/AppHost.cs":::

The preceding code:

- Creates a Docker Compose environment named `compose`.
- Adds a Redis cache service that will be included in the Docker Compose deployment.
- Adds an API service project that will be containerized and included in the deployment.
- Adds a web application that references both the cache and API service.
- Configures all services to be published as Docker Compose services using <xref:Aspire.Hosting.DockerComposeServiceExtensions.PublishAsDockerComposeService%2A>.

### Add Docker Compose environment resource with properties

You can configure various properties of the Docker Compose environment using the <xref:Aspire.Hosting.DockerComposeEnvironmentExtensions.WithProperties%2A> method:

```csharp
var compose = builder.AddDockerComposeEnvironment("compose")
                     .WithProperties(env =>
                     {
                         env.DefaultContainerRegistry = "myregistry.azurecr.io";
                         env.DefaultNetworkName = "my-network";
                         env.BuildContainerImages = true;
                     });
```

### Add Docker Compose environment resource with compose file

You can customize the generated Docker Compose file using the <xref:Aspire.Hosting.DockerComposeEnvironmentExtensions.ConfigureComposeFile%2A> method:

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

### Add Aspire dashboard resource to environment

The Docker hosting integration includes an Aspire dashboard for telemetry visualization. You can configure or disable it using the <xref:Aspire.Hosting.DockerComposeEnvironmentExtensions.WithDashboard%2A> method:

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

The <xref:Aspire.Hosting.DockerComposeAspireDashboardResourceBuilderExtensions.WithHostPort%2A> method configures the port used to access the Aspire dashboard from a browser. The <xref:Aspire.Hosting.DockerComposeAspireDashboardResourceBuilderExtensions.WithForwardedHeaders(Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.Docker.DockerComposeAspireDashboardResource},System.Boolean)> method enables forwarded headers processing when the dashboard is accessed through a reverse proxy or load balancer.

### Publishing and deployment

To deploy your application using Docker Compose, use the <xref:Aspire.Cli.Commands.PublishCommand>:

```dotnetcli
aspire publish -o docker-compose-artifacts
```

This command generates Docker Compose files and all necessary artifacts in the specified output directory. The generated files include:

- `docker-compose.yml`: The main Docker Compose file defining all services.
- `docker-compose.override.yml`: Override file for development-specific settings.
- `.env`: Environment variables file.
- Service-specific configuration files and scripts.

After publishing, you can deploy your application using Docker Compose:

```bash
cd docker-compose-artifacts
docker compose up -d
```

### Environment variables

The Docker hosting integration captures environment variables from your app model and includes them in a `.env` file. This ensures that all configuration is properly passed to the containerized services.

## Next steps

- [Deploy Aspire projects to Azure Container Apps using the Aspire CLI](aspire-deploy/aca-deployment-aspire-cli.md)
- [Building custom deployment pipelines](../fundamentals/custom-deployments.md)
- [Docker Compose to AppHost API reference](../get-started/docker-compose-to-apphost-reference.md)
- [Aspire integrations](../fundamentals/integrations-overview.md)
- [Aspire GitHub repo](https://github.com/dotnet/aspire)
