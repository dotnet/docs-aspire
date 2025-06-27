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
| `services:` | `var builder = DistributedApplication.CreateBuilder(args);` | Root application builder |
| `service_name:` | `builder.Add*("service_name")` | Service name becomes resource name |

## Images and builds

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `image: nginx:latest` | `builder.AddContainer("name", "nginx:latest")` | Direct image reference |
| `build: .` | `builder.AddDockerfile("name", ".")` | Build from Dockerfile |
| `build: ./path` | `builder.AddDockerfile("name", "./path")` | Build from specific path |
| `build.context: ./app` | `builder.AddDockerfile("name", "./app")` | Build context |
| `build.dockerfile: Custom.dockerfile` | `builder.AddDockerfile("name", ".").WithDockerfile("Custom.dockerfile")` | Custom Dockerfile name |

## .NET projects

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `build: ./MyApi` (for .NET) | `builder.AddProject<Projects.MyApi>("myapi")` | Direct .NET project reference |

## Port mappings

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `ports: ["8080:80"]` | `.WithHttpEndpoint(port: 8080, targetPort: 80)` | HTTP endpoint mapping |
| `ports: ["443:443"]` | `.WithHttpsEndpoint(port: 443, targetPort: 443)` | HTTPS endpoint mapping |
| `expose: ["8080"]` | `.WithEndpoint(port: 8080)` | Internal port exposure |

## Environment variables

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `environment: KEY=value` | `.WithEnvironment("KEY", "value")` | Static environment variable |
| `environment: KEY=${HOST_VAR}` | `.WithEnvironment("KEY", builder.AddParameter("hostVar"))` | Parameter-based variable |
| `env_file: .env` | `.WithEnvironment(envFile)` | Environment file (custom implementation) |

## Volumes and storage

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `volumes: ["data:/app/data"]` | `.WithVolume("data", "/app/data")` | Named volume |
| `volumes: ["./host:/container"]` | `.WithBindMount("./host", "/container")` | Bind mount |
| `volumes: ["./config:/app:ro"]` | `.WithBindMount("./config", "/app", isReadOnly: true)` | Read-only bind mount |

## Dependencies and ordering

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `depends_on: [db]` | `.WithReference(db)` | Service dependency with connection |
| `depends_on: db: condition: service_started` | `.WaitFor(db)` | Wait for service start |
| `depends_on: db: condition: service_healthy` | `.WaitForCompletion(db)` | Wait for health check |

## Networks

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `networks: [backend]` | Automatic | .NET Aspire handles networking automatically |
| Custom networks | Not needed | Service discovery handles inter-service communication |

## Resource limits

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `deploy.resources.limits.memory: 512m` | Not supported | Resource limits aren't supported in .NET Aspire |
| `deploy.resources.limits.cpus: 0.5` | Not supported | Resource limits aren't supported in .NET Aspire |

## Health checks

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `healthcheck.test: ["CMD", "curl", "http://localhost/health"]` | Built-in for integrations | .NET Aspire integrations include health checks |
| `healthcheck.interval: 30s` | Configurable in integration | Health check configuration varies by resource type |

## Restart policies

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `restart: unless-stopped` | `.WithRestart(RestartPolicy.UnlessStopped)` | Available in container resources |
| `restart: always` | `.WithRestart(RestartPolicy.Always)` | Always restart |
| `restart: no` | Default | No restart policy |

## Labels and metadata

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `labels: app=myapp` | `.WithAnnotation("app", "myapp")` | Resource annotations |

## Logging

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `logging.driver: json-file` | Built-in | .NET Aspire provides integrated logging |
| `logging.options.max-size: 10m` | Dashboard configuration | Managed through .NET Aspire dashboard |

## Database services

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `image: postgres:15` | `builder.AddPostgres("name")` | PostgreSQL with automatic configuration |
| `image: mysql:8` | `builder.AddMySql("name")` | MySQL with automatic configuration |
| `image: redis:7` | `builder.AddRedis("name")` | Redis with automatic configuration |
| `image: mongo:latest` | `builder.AddMongoDB("name")` | MongoDB with automatic configuration |

## See also

- [Migrate from Docker Compose to .NET Aspire](migrate-from-docker-compose.md)
- [.NET Aspire orchestration overview](../fundamentals/app-host-overview.md)
- [Add Dockerfiles to your .NET app model](../app-host/withdockerfile.md)
