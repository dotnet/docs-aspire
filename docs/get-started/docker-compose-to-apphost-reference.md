---
title: Docker Compose to .NET Aspire AppHost API reference
description: Quick reference for converting Docker Compose YAML syntax to .NET Aspire C# API calls.
ms.date: 06/26/2025
ms.topic: reference
---

# Docker Compose to .NET Aspire AppHost API reference

This reference provides systematic mappings from Docker Compose YAML syntax to equivalent .NET Aspire C# API calls. Use these tables as a quick reference when converting your existing Docker Compose files to .NET Aspire application host configurations. Each section covers a specific aspect of container orchestration, from basic service definitions to advanced networking and resource management.

## Service definitions

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `services:` | `var builder = DistributedApplication.CreateBuilder(args);` | Root application builder used for adding and representing resources. |
| `service_name:` | `builder.Add*("service_name")` | Service name becomes resource name |

**Related links:**

- [Docker Compose services reference](https://docs.docker.com/compose/compose-file/05-services/)
- <xref:Aspire.Hosting.DistributedApplicationBuilder>

## Images and builds

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `image: nginx:latest` | `builder.AddContainer("name", "nginx:latest")` | Direct image reference |
| `build: .` | `builder.AddDockerfile("name", ".")` | Build from Dockerfile |
| `build: ./path` | `builder.AddDockerfile("name", "./path")` | Build from specific path |
| `build.context: ./app` | `builder.AddDockerfile("name", "./app")` | Build context |
| `build.dockerfile: Custom.dockerfile` | `builder.AddDockerfile("name", ".").WithDockerfile("Custom.dockerfile")` | Custom Dockerfile name |

**Related links:**

- [Docker Compose build reference](https://docs.docker.com/compose/compose-file/build/)
- <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.AddDockerfile%2A>
- <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.AddContainer%2A>

## .NET projects

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `build: ./MyApi` (for .NET) | `builder.AddProject<Projects.MyApi>("myapi")` | Direct .NET project reference |

**Related links:**

- [Docker Compose build reference](https://docs.docker.com/compose/compose-file/build/)
- <xref:Aspire.Hosting.ProjectResourceBuilderExtensions.AddProject%2A>

## Port mappings

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `ports: ["8080:80"]` | `.WithHttpEndpoint(port: 8080, targetPort: 80)` | HTTP endpoint mapping. Ports are optional; dynamic ports are used if omitted. |
| `ports: ["443:443"]` | `.WithHttpsEndpoint(port: 443, targetPort: 443)` | HTTPS endpoint mapping. Ports are optional; dynamic ports are used if omitted. |
| `expose: ["8080"]` | `.WithEndpoint(port: 8080)` | Internal port exposure. Ports are optional; dynamic ports are used if omitted. |

**Related links:**

- [Docker Compose ports reference](https://docs.docker.com/compose/compose-file/05-services/#ports)
- <xref:Aspire.Hosting.ResourceBuilderExtensions.WithHttpEndpoint%2A>
- <xref:Aspire.Hosting.ResourceBuilderExtensions.WithHttpsEndpoint%2A>
- <xref:Aspire.Hosting.ResourceBuilderExtensions.WithEndpoint%2A>

## Environment variables

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `environment: KEY=value` | `.WithEnvironment("KEY", "value")` | Static environment variable |
| `environment: KEY=${HOST_VAR}` | `.WithEnvironment(context => context.EnvironmentVariables["KEY"] = hostVar)` | Environment variable with callback context |
| `env_file: .env` | Not supported | Environment file (custom implementation) |

**Related links:**

- [Docker Compose environment reference](https://docs.docker.com/compose/compose-file/05-services/#environment)
- <xref:Aspire.Hosting.ResourceBuilderExtensions.WithEnvironment%2A>

## Volumes and storage

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `volumes: ["data:/app/data"]` | `.WithVolume("data", "/app/data")` | Named volume |
| `volumes: ["./host:/container"]` | `.WithBindMount("./host", "/container")` | Bind mount |
| `volumes: ["./config:/app:ro"]` | `.WithBindMount("./config", "/app", isReadOnly: true)` | Read-only bind mount |

**Related links:**

- [Docker Compose volumes reference](https://docs.docker.com/compose/compose-file/05-services/#volumes)
- <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithVolume%2A>
- <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithBindMount%2A>

## Dependencies and ordering

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `depends_on: [db]` | `.WithReference(db)` | Service dependency with connection |
| `depends_on: db: condition: service_started` | `.WaitFor(db)` | Wait for service start |
| `depends_on: db: condition: service_healthy` | `.WaitForCompletion(db)` | Wait for health check |

**Related links:**

- [Docker Compose depends_on reference](https://docs.docker.com/compose/compose-file/05-services/#depends_on)
- <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A>
- <xref:Aspire.Hosting.ResourceBuilderExtensions.WaitFor%2A>
- <xref:Aspire.Hosting.ResourceBuilderExtensions.WaitForCompletion%2A>

## Networks

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `networks: [backend]` | Automatic | .NET Aspire handles networking automatically |
| Custom networks | Not needed | Service discovery handles inter-service communication |

**Related links:**

- [Docker Compose networks reference](https://docs.docker.com/compose/compose-file/05-services/#networks)
- [.NET Aspire service discovery](../service-discovery/overview.md)

## Resource limits

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `deploy.resources.limits.memory: 512m` | Not supported | Resource limits aren't supported in .NET Aspire |
| `deploy.resources.limits.cpus: 0.5` | Not supported | Resource limits aren't supported in .NET Aspire |

**Related links:**

- [Docker Compose deploy reference](https://docs.docker.com/compose/compose-file/deploy/)

## Health checks

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `healthcheck.test: ["CMD", "curl", "http://localhost/health"]` | Built-in for integrations | .NET Aspire integrations include health checks |
| `healthcheck.interval: 30s` | Configurable in integration | Health check configuration varies by resource type |

**Related links:**

- [Docker Compose healthcheck reference](https://docs.docker.com/compose/compose-file/05-services/#healthcheck)
- [.NET Aspire health checks](../fundamentals/health-checks.md)

## Restart policies

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `restart: unless-stopped` | Not supported | Restart policies aren't supported in .NET Aspire |
| `restart: always` | Not supported | Restart policies aren't supported in .NET Aspire |
| `restart: no` | Default | No restart policy |

**Related links:**

- [Docker Compose restart reference](https://docs.docker.com/compose/compose-file/05-services/#restart)

## Logging

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `logging.driver: json-file` | Built-in | .NET Aspire provides integrated logging |
| `logging.options.max-size: 10m` | Dashboard configuration | Managed through .NET Aspire dashboard |

**Related links:**

- [Docker Compose logging reference](https://docs.docker.com/compose/compose-file/05-services/#logging)
- [.NET Aspire telemetry](../fundamentals/telemetry.md)

## Database services

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `image: postgres:15` | `builder.AddPostgres("name")` | PostgreSQL with automatic configuration |
| `image: mysql:8` | `builder.AddMySql("name")` | MySQL with automatic configuration |
| `image: redis:7` | `builder.AddRedis("name")` | Redis with automatic configuration |
| `image: mongo:latest` | `builder.AddMongoDB("name")` | MongoDB with automatic configuration |

**Related links:**

- [Docker Compose services reference](https://docs.docker.com/compose/compose-file/05-services/)
- <xref:Aspire.Hosting.PostgresBuilderExtensions.AddPostgres%2A>
- <xref:Aspire.Hosting.MySqlBuilderExtensions.AddMySql%2A>
- <xref:Aspire.Hosting.RedisBuilderExtensions.AddRedis%2A>
- <xref:Aspire.Hosting.MongoDBBuilderExtensions.AddMongoDB%2A>

## See also

- [Migrate from Docker Compose to .NET Aspire](migrate-from-docker-compose.md)
- [.NET Aspire orchestration overview](../fundamentals/app-host-overview.md)
- [Add Dockerfiles to your .NET app model](../app-host/withdockerfile.md)
