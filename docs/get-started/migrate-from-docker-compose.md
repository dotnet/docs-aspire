---
title: Migrate from Docker Compose to .NET Aspire
description: Learn how to migrate your Docker Compose applications to .NET Aspire and understand the key conceptual differences.
ms.date: 01/17/2025
ms.topic: how-to
---

# Migrate from Docker Compose to .NET Aspire

This guide helps you understand how to migrate applications from Docker Compose to .NET Aspire, highlighting the key conceptual differences and providing practical examples for common migration scenarios.

## Understand the differences

While Docker Compose and .NET Aspire might seem similar at first glance, they serve different purposes and operate at different levels of abstraction.

### Docker Compose vs .NET Aspire

| Aspect | Docker Compose | .NET Aspire |
|--------|----------------|-------------|
| **Primary purpose** | Container orchestration | Development-time orchestration and app composition |
| **Scope** | Container-focused | Multi-resource (containers, .NET projects, cloud resources) |
| **Configuration** | YAML-based | C#-based, strongly typed |
| **Target environment** | Any Docker runtime | Development and cloud deployment |
| **Service discovery** | DNS-based container discovery | Built-in service discovery with environment variables |
| **Development experience** | Manual container management | Integrated tooling, dashboard, and telemetry |

### Key conceptual shifts

When migrating from Docker Compose to .NET Aspire, consider these conceptual differences:

- **From YAML to C#**: Configuration moves from declarative YAML to imperative, strongly-typed C# code.
- **From containers to resources**: .NET Aspire manages not just containers, but .NET projects, executables, parameters, all as resources.
- **From manual networking to service discovery**: .NET Aspire automatically configures service discovery and connection strings.
- **From development gaps to integrated experience**: .NET Aspire provides dashboard, telemetry, and debugging integration.

## Docker Compose to .NET Aspire reference

The following reference tables provide systematic mappings from Docker Compose YAML syntax to equivalent .NET Aspire C# API calls. Use these tables as a quick reference when converting your existing Docker Compose files to .NET Aspire application host configurations. Each section covers a specific aspect of container orchestration, from basic service definitions to advanced networking and resource management.

### Service definitions

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `services:` | `var builder = DistributedApplication.CreateBuilder(args);` | Root application builder |
| `service_name:` | `builder.Add*("service_name")` | Service name becomes resource name |

### Images and builds

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `image: nginx:latest` | `builder.AddContainer("name", "nginx:latest")` | Direct image reference |
| `build: .` | `builder.AddDockerfile("name", ".")` | Build from Dockerfile |
| `build: ./path` | `builder.AddDockerfile("name", "./path")` | Build from specific path |
| `build.context: ./app` | `builder.AddDockerfile("name", "./app")` | Build context |
| `build.dockerfile: Custom.dockerfile` | `builder.AddDockerfile("name", ".").WithDockerfile("Custom.dockerfile")` | Custom Dockerfile name |

### .NET projects

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `build: ./MyApi` (for .NET) | `builder.AddProject<Projects.MyApi>("myapi")` | Direct .NET project reference |

### Port mappings

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `ports: ["8080:80"]` | `.WithHttpEndpoint(port: 8080, targetPort: 80)` | HTTP endpoint mapping |
| `ports: ["443:443"]` | `.WithHttpsEndpoint(port: 443, targetPort: 443)` | HTTPS endpoint mapping |
| `expose: ["8080"]` | `.WithEndpoint(port: 8080)` | Internal port exposure |

### Environment variables

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `environment: KEY=value` | `.WithEnvironment("KEY", "value")` | Static environment variable |
| `environment: KEY=${HOST_VAR}` | `.WithEnvironment("KEY", builder.AddParameter("hostVar"))` | Parameter-based variable |
| `env_file: .env` | `.WithEnvironment(envFile)` | Environment file (custom implementation) |

### Volumes and storage

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `volumes: ["data:/app/data"]` | `.WithVolume("data", "/app/data")` | Named volume |
| `volumes: ["./host:/container"]` | `.WithBindMount("./host", "/container")` | Bind mount |
| `volumes: ["./config:/app:ro"]` | `.WithBindMount("./config", "/app", isReadOnly: true)` | Read-only bind mount |

### Dependencies and ordering

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `depends_on: [db]` | `.WithReference(db)` | Service dependency with connection |
| `depends_on: db: condition: service_started` | `.WaitFor(db)` | Wait for service start |
| `depends_on: db: condition: service_healthy` | `.WaitForCompletion(db)` | Wait for health check |

### Networks

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `networks: [backend]` | Automatic | .NET Aspire handles networking automatically |
| Custom networks | Not needed | Service discovery handles inter-service communication |

### Resource limits

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `deploy.resources.limits.memory: 512m` | `.WithResourceLimits(memory: 0.5)` | Memory limit in GB |
| `deploy.resources.limits.cpus: 0.5` | `.WithResourceLimits(cpu: 0.5)` | CPU limit |

### Health checks

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `healthcheck.test: ["CMD", "curl", "http://localhost/health"]` | Built-in for integrations | .NET Aspire integrations include health checks |
| `healthcheck.interval: 30s` | Configurable in integration | Health check configuration varies by resource type |

### Restart policies

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `restart: unless-stopped` | `.WithRestart(RestartPolicy.UnlessStopped)` | Available in container resources |
| `restart: always` | `.WithRestart(RestartPolicy.Always)` | Always restart |
| `restart: no` | Default | No restart policy |

### Labels and metadata

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `labels: app=myapp` | `.WithAnnotation("app", "myapp")` | Resource annotations |

### Logging

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `logging.driver: json-file` | Built-in | .NET Aspire provides integrated logging |
| `logging.options.max-size: 10m` | Dashboard configuration | Managed through .NET Aspire dashboard |

### Database services

| Docker Compose | .NET Aspire | Notes |
|----------------|-------------|-------|
| `image: postgres:15` | `builder.AddPostgres("name")` | PostgreSQL with automatic configuration |
| `image: mysql:8` | `builder.AddMySql("name")` | MySQL with automatic configuration |
| `image: redis:7` | `builder.AddRedis("name")` | Redis with automatic configuration |
| `image: mongo:latest` | `builder.AddMongoDB("name")` | MongoDB with automatic configuration |

## Common migration patterns

This section demonstrates practical migration scenarios that you'll likely encounter when moving from Docker Compose to .NET Aspire. Each pattern shows a complete Docker Compose example alongside its .NET Aspire equivalent, highlighting the key differences and benefits of the migration.

The patterns covered include:

- **Multi-service web applications** with frontend, API, and database components.
- **Container-based services** using existing Docker images.
- **Environment variables and configuration** management strategies.
- **Custom networks and volumes** for data persistence and service isolation.

These examples represent the most common use cases, but .NET Aspire's flexibility allows for many other scenarios. If your specific use case isn't covered here, you can combine these patterns or refer to the comprehensive API reference above.

### Multi-service web application

**Docker Compose example:**

```yaml
version: '3.8'
services:
  frontend:
    build: ./frontend
    ports:
      - "3000:3000"
    depends_on:
      - api
    environment:
      - API_URL=http://api:5000

  api:
    build: ./api
    ports:
      - "5000:5000"
    depends_on:
      - database
    environment:
      - ConnectionStrings__DefaultConnection=Host=database;Database=myapp;Username=postgres;Password=secret

  database:
    image: postgres:15
    environment:
      - POSTGRES_DB=myapp
      - POSTGRES_USER=postgres
      - POSTGRES_PASSWORD=secret
    volumes:
      - postgres_data:/var/lib/postgresql/data

volumes:
  postgres_data:
```

**.NET Aspire equivalent:**

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add the database
var database = builder.AddPostgres("postgres")
    .WithEnvironment("POSTGRES_DB", "myapp")
    .AddDatabase("myapp");

// Add the API project
var api = builder.AddProject<Projects.MyApp_Api>("api")
    .WithReference(database);

// Add the frontend project  
var frontend = builder.AddProject<Projects.MyApp_Frontend>("frontend")
    .WithReference(api);

builder.Build().Run();
```

**Key differences:**

- **Service dependencies**: `depends_on` becomes `WithReference()` which also configures service discovery.
- **Environment variables**: Connection strings are automatically generated and injected.
- **Build context**: Build context is capable of using Dockerfiles, .NET projects, Node.js apps, and more instead of just Dockerfile builds.
- **Data persistence**: Volumes are automatically managed by .NET Aspire.

### Container-based services

**Docker Compose example:**

```yaml
version: '3.8'
services:
  web:
    build: .
    ports:
      - "8080:8080"
    depends_on:
      - redis
      - postgres

  redis:
    image: redis:7
    ports:
      - "6379:6379"

  postgres:
    image: postgres:15
    environment:
      POSTGRES_PASSWORD: secret
    volumes:
      - postgres_data:/var/lib/postgresql/data

volumes:
  postgres_data:
```

**.NET Aspire equivalent:**

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add backing services
var cache = builder.AddRedis("redis");
var database = builder.AddPostgres("postgres", password: "secret")
    .AddDatabase("main");

// Add the containerized web application
var web = builder.AddContainer("web", "myapp:latest")
    .WithHttpEndpoint(port: 8080, targetPort: 8080)
    .WithReference(cache)
    .WithReference(database);

builder.Build().Run();
```

### Environment variables and configuration

**Docker Compose approach:**

```yaml
services:
  app:
    image: myapp:latest
    environment:
      - DATABASE_URL=postgresql://user:pass@db:5432/myapp
      - REDIS_URL=redis://cache:6379
      - API_KEY=${API_KEY}
      - LOG_LEVEL=info
```

**.NET Aspire approach:**

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add external parameter
var apiKey = builder.AddParameter("apiKey", secret: true);

var database = builder.AddPostgres("db")
    .AddDatabase("myapp");
    
var cache = builder.AddRedis("cache");

var app = builder.AddContainer("app", "myapp:latest")
    .WithReference(database)      // Automatically sets DATABASE_URL
    .WithReference(cache)         // Automatically sets REDIS_URL  
    .WithEnvironment("API_KEY", apiKey)
    .WithEnvironment("LOG_LEVEL", "info");

builder.Build().Run();
```

**Key differences:**

- **Automatic connection strings**: Database and cache URLs are automatically generated.
- **Typed parameters**: External configuration uses strongly-typed parameters instead of environment variable substitution.
- **Service discovery**: References automatically configure the correct service endpoints.

### Custom networks and volumes

**Docker Compose example:**

```yaml
version: '3.8'
services:
  app:
    image: myapp:latest
    volumes:
      - app_data:/data
      - ./config:/app/config:ro
    networks:
      - backend

  worker:
    image: myworker:latest
    volumes:
      - app_data:/shared
    networks:
      - backend

networks:
  backend:

volumes:
  app_data:
```

**.NET Aspire equivalent:**

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Create a named volume
var appData = builder.AddVolume("app-data");

var app = builder.AddContainer("app", "myapp:latest")
    .WithVolume(appData, "/data")
    .WithBindMount("./config", "/app/config", isReadOnly: true);

var worker = builder.AddContainer("worker", "myworker:latest")
    .WithVolume(appData, "/shared");

builder.Build().Run();
```

**Key differences:**

- **Simplified networking**: .NET Aspire automatically handles container networking.
- **Volume management**: Named volumes are created and managed through the resource model.
- **Bind mounts**: Host directories can be mounted with `WithBindMount()`.

## Migration strategy

Successfully migrating from Docker Compose to .NET Aspire requires a systematic approach. The following steps provide a proven methodology for moving your applications while minimizing disruption and ensuring all components work correctly in the new environment.

### 1. Assess your current setup

Before migrating, inventory your Docker Compose setup:

- **Services**: Identify all services including databases, caches, APIs, and web applications.
- **Dependencies**: Map out service dependencies from `depends_on` declarations.
- **Data persistence**: Catalog all volumes and bind mounts used for data storage.
- **Environment variables**: List all configuration variables and secrets.
- **Custom networks**: Identify any custom networking requirements and configurations.

### 2. Create the .NET Aspire app host

Start by creating a new .NET Aspire project:

```bash
dotnet new aspire-apphost -o MyApp.AppHost
```

### 3. Migrate services incrementally

Migrate services one by one, starting with backing services (databases, caches) and then applications:

1. **Add backing services** like PostgreSQL, Redis, and other infrastructure components.
1. **Convert .NET applications** to project references for better integration.
1. **Convert other containers** using `AddContainer()` for existing Docker images.
1. **Configure dependencies** with `WithReference()` to establish service relationships.
1. **Set up environment variables** and parameters for configuration management.

### 4. Handle data migration

For persistent data:

- Use `WithVolume()` for named volumes that need to persist data.
- Use `WithBindMount()` for host directory mounts when you need direct access to host files.
- Consider using `WithDataVolume()` for database persistence with automatic volume management.

### 5. Test and validate

- Start the .NET Aspire app host and verify all services start correctly.
- Check the dashboard to confirm service health and connectivity status.
- Validate that inter-service communication works as expected.
- Test with your existing client applications to ensure compatibility.

## Publish to Docker Compose

.NET Aspire 9.3 introduced the ability to publish your app model back to Docker Compose, enabling a hybrid workflow:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Configure Docker Compose publishing
builder.AddDockerComposeEnvironment("docker-compose")
    .WithProperties(env =>
    {
        env.BuildContainerImages = true;
    })
    .ConfigureComposeFile(file =>
    {
        file.Name = "my-app";
    });

// Add your resources
var cache = builder.AddRedis("cache");
var database = builder.AddPostgres("postgres").AddDatabase("main");

var api = builder.AddProject<Projects.MyApp_Api>("api")
    .WithReference(database)
    .WithReference(cache)
    .PublishAsDockerComposeService((resource, service) =>
    {
        service.Restart = "unless-stopped";
    });

builder.Build().Run();
```

This generates a `docker-compose.yml` file from your .NET Aspire configuration, allowing you to:

- Use .NET Aspire for development with its rich tooling and dashboard experience.
- Deploy with Docker Compose in production environments where it's already established.
- Maintain consistency between environments by using the same configuration model.

## Migration troubleshooting

### Common issues and solutions

**Service discovery not working**

- Ensure you're using `WithReference()` to establish dependencies between services.
- Check that services are using the correct environment variable names for connections.
- Review the dashboard to verify environment variables are injected correctly.

**Database connections failing**

- Verify the database is fully started before dependent services attempt to connect.
- Use `WaitFor()` to ensure proper startup ordering:

  ```csharp
  var api = builder.AddProject<Projects.Api>("api")
      .WithReference(database)
      .WaitFor(database);
  ```

**Volume mounting issues**

- Use absolute paths for bind mounts to avoid path resolution issues.
- Ensure the host directory exists and has proper permissions before mounting.
- Consider using named volumes instead of bind mounts where possible for better portability.

**Port conflicts**

- .NET Aspire automatically assigns ports to avoid conflicts between services.
- Use `WithHttpEndpoint()` to specify custom ports if needed for external access.
- Check the dashboard for actual assigned ports during development.

## Next steps

After migrating to .NET Aspire:

- Explore [.NET Aspire integrations](../fundamentals/integrations-overview.md) to replace custom container configurations.
- Set up [health checks](../fundamentals/health-checks.md) for better monitoring.
- Configure [telemetry](../fundamentals/telemetry.md) for observability.
- Learn about [deployment options](../deployment/overview.md) for production environments.
- Consider [testing](../testing/overview.md) your distributed application.

## See also

- [.NET Aspire overview](aspire-overview.md)
- [.NET Aspire orchestration overview](../fundamentals/app-host-overview.md)
- [Docker Compose enhancements in .NET Aspire 9.3](../whats-new/dotnet-aspire-9.3.md#-docker-compose-enhancements)
- [Add Dockerfiles to your .NET app model](../app-host/withdockerfile.md)
