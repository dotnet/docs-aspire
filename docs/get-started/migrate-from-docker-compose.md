---
title: Migrate from Docker Compose to .NET Aspire
description: Learn how to migrate your Docker Compose applications to .NET Aspire and understand the key conceptual differences.
ms.date: 01/17/2025
ms.topic: how-to
---

# Migrate from Docker Compose to .NET Aspire

This guide helps you understand how to migrate applications from Docker Compose to .NET Aspire, highlighting the key conceptual differences and providing practical examples for common migration scenarios.

## Understanding the differences

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

- **From YAML to C#**: Configuration moves from declarative YAML to imperative, strongly-typed C# code
- **From containers to resources**: .NET Aspire manages not just containers, but .NET projects, databases, and cloud resources
- **From manual networking to service discovery**: .NET Aspire automatically configures service discovery and connection strings
- **From development gaps to integrated experience**: .NET Aspire provides dashboard, telemetry, and debugging integration

## Common migration patterns

This section shows how to migrate common Docker Compose patterns to .NET Aspire.

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

- **Service dependencies**: `depends_on` becomes `WithReference()` which also configures service discovery
- **Environment variables**: Connection strings are automatically generated and injected
- **Build context**: .NET projects are referenced directly instead of using Dockerfile builds
- **Data persistence**: Volumes are automatically managed by .NET Aspire

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

- **Automatic connection strings**: Database and cache URLs are automatically generated
- **Typed parameters**: External configuration uses strongly-typed parameters instead of environment variable substitution
- **Service discovery**: References automatically configure the correct service endpoints

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

- **Simplified networking**: .NET Aspire automatically handles container networking
- **Volume management**: Named volumes are created and managed through the resource model
- **Bind mounts**: Host directories can be mounted with `WithBindMount()`

## Migration strategy

### 1. Assess your current setup

Before migrating, inventory your Docker Compose setup:

- **Services**: Identify all services (databases, caches, APIs, web apps)
- **Dependencies**: Map out service dependencies from `depends_on`
- **Data persistence**: Catalog volumes and bind mounts
- **Environment variables**: List all configuration and secrets
- **Custom networks**: Identify any custom networking requirements

### 2. Create the .NET Aspire app host

Start by creating a new .NET Aspire project:

```bash
dotnet new aspire-apphost -o MyApp.AppHost
```

### 3. Migrate services incrementally

Migrate services one by one, starting with backing services (databases, caches) and then applications:

1. **Add backing services** (PostgreSQL, Redis, etc.)
1. **Convert .NET applications** to project references
1. **Convert other containers** using `AddContainer()`
1. **Configure dependencies** with `WithReference()`
1. **Set up environment variables** and parameters

### 4. Handle data migration

For persistent data:

- Use `WithVolume()` for named volumes
- Use `WithBindMount()` for host directory mounts
- Consider using `WithDataVolume()` for database persistence

### 5. Test and validate

- Start the .NET Aspire app host and verify all services start correctly
- Check the dashboard to confirm service health and connectivity
- Validate that inter-service communication works as expected
- Test with your existing client applications

## Publishing to Docker Compose

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

- Use .NET Aspire for development
- Deploy with Docker Compose in production
- Maintain consistency between environments

## Migration troubleshooting

### Common issues and solutions

**Service discovery not working**

- Ensure you're using `WithReference()` to establish dependencies
- Check that services are using the correct environment variable names
- Review the dashboard to verify environment variables are injected correctly

**Database connections failing**

- Verify the database is fully started before dependent services
- Use `WaitFor()` to ensure proper startup ordering:

  ```csharp
  var api = builder.AddProject<Projects.Api>("api")
      .WithReference(database)
      .WaitFor(database);
  ```

**Volume mounting issues**

- Use absolute paths for bind mounts
- Ensure the host directory exists and has proper permissions
- Consider using named volumes instead of bind mounts where possible

**Port conflicts**

- .NET Aspire automatically assigns ports to avoid conflicts
- Use `WithHttpEndpoint()` to specify custom ports if needed
- Check the dashboard for actual assigned ports

### Getting help

If you encounter issues during migration:

- Check the [.NET Aspire troubleshooting documentation](../troubleshooting/allow-unsecure-transport.md)
- Review the [GitHub discussions](https://github.com/dotnet/aspire/discussions) for similar migration scenarios
- Ask questions on [Stack Overflow](https://stackoverflow.com/questions/tagged/dotnet-aspire) with the `dotnet-aspire` tag
- Join the [.NET Aspire Discord community](https://aka.ms/aspire/discord)

## Next steps

After migrating to .NET Aspire:

- Explore [.NET Aspire integrations](../fundamentals/integrations-overview.md) to replace custom container configurations
- Set up [health checks](../fundamentals/health-checks.md) for better monitoring
- Configure [telemetry](../fundamentals/telemetry.md) for observability
- Learn about [deployment options](../deployment/overview.md) for production environments
- Consider [testing](../testing/overview.md) your distributed application

## See also

- [.NET Aspire overview](aspire-overview.md)
- [.NET Aspire orchestration overview](../fundamentals/app-host-overview.md)
- [Docker Compose enhancements in .NET Aspire 9.3](../whats-new/dotnet-aspire-9.3.md#-docker-compose-enhancements)
- [Add Dockerfiles to your .NET app model](../app-host/withdockerfile.md)
