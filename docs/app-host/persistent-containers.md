---
title: Persistent container services in .NET Aspire
description: Learn how to configure containers to persist and be re-used between .NET Aspire app host runs.
ms.date: 01/15/2025
---

# Persistent container services in .NET Aspire

In .NET Aspire, containers follow a typical lifecycle where they're created when the app host starts and destroyed when it stops. However, .NET Aspire 9.0 introduces **persistent containers**, which deviate from this standard lifecycle. Persistent containers are created and started by the .NET Aspire orchestrator but are not destroyed when the app host stops, allowing them to persist between runs.

This feature is particularly beneficial for containers that have long startup times, such as databases, as it eliminates the need to wait for these services to initialize on every app host restart.

## Configure a persistent container

To configure a container resource with a persistent lifetime, use the <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithLifetime*> method and pass <xref:Aspire.Hosting.ApplicationModel.ContainerLifetime.Persistent?displayProperty=nameWithType>:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                      .WithLifetime(ContainerLifetime.Persistent);

var db = postgres.AddDatabase("inventorydb");

builder.AddProject<Projects.InventoryService>("inventory")
       .WithReference(db);

builder.Build().Run();
```

In the preceding example, the PostgreSQL container is configured to persist between app host runs, while the `inventory` project references the database as normal.

## Dashboard visualization

The .NET Aspire dashboard shows persistent containers with a distinctive pin icon (📌) to help you identify them:

:::image type="content" source="../whats-new/media/persistent-container.png" lightbox="../whats-new/media/persistent-container.png" alt-text="Screenshot of the .NET Aspire dashboard showing a persistent container with a pin icon.":::

After the app host stops, persistent containers continue running and can be seen in your container runtime (such as Docker Desktop):

:::image type="content" source="../whats-new/media/persistent-container-docker-desktop.png" lightbox="../whats-new/media/persistent-container-docker-desktop.png" alt-text="Screenshot of Docker Desktop showing a persistent RabbitMQ container still running after the app host stopped.":::

## Configuration change detection

Persistent containers are automatically recreated when the app host detects meaningful configuration changes. .NET Aspire tracks a hash of the configuration used to create each container and compares it to the current configuration on subsequent runs. If the configuration differs, the container is recreated with the new settings.

This mechanism ensures that persistent containers stay synchronized with your app host configuration without requiring manual intervention.

## Container naming and uniqueness

### Default naming

By default, persistent containers use a naming pattern that combines:

- The service name you specify in your app host
- A postfix based on a hash of the app host project path

This naming scheme ensures that persistent containers are unique to each app host project, preventing conflicts when multiple .NET Aspire projects use the same service names.

For example, if you have a service named `"postgres"` in an app host project located at `/path/to/MyApp.AppHost`, the container name might be `postgres-abc123def` where `abc123def` is derived from the project path hash.

### Custom container names

For advanced scenarios, you can set a custom container name using the <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithContainerName*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                      .WithLifetime(ContainerLifetime.Persistent)
                      .WithContainerName("my-shared-postgres");

builder.Build().Run();
```

When you specify a custom container name, .NET Aspire can reuse an existing container that wasn't created by the app host. Such containers are never automatically recreated, even if the app host configuration changes, giving you full control over their lifecycle.

## Manual cleanup

> [!IMPORTANT]
> Persistent containers are not automatically removed when you stop the app host. To delete these containers, you must manually stop and remove them using your container runtime.

You can clean up persistent containers using Docker CLI commands:

```bash
# Stop the container
docker stop my-container-name

# Remove the container
docker rm my-container-name
```

Alternatively, you can use Docker Desktop or your preferred container management tool to stop and remove persistent containers.

## Use cases and benefits

Persistent containers are ideal for:

- **Database services**: PostgreSQL, SQL Server, MySQL, and other databases that take time to initialize and load data.
- **Message brokers**: RabbitMQ, Redis, and similar services that benefit from maintaining state between runs.
- **Development data**: Containers with test data or configurations that you want to preserve during development iterations.
- **Shared services**: Services that multiple app hosts or development team members can share.

## Example scenarios

### Development database with test data

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Persistent PostgreSQL with custom name for team sharing
var postgres = builder.AddPostgres("postgres")
                      .WithLifetime(ContainerLifetime.Persistent)
                      .WithContainerName("team-dev-postgres");

var catalogDb = postgres.AddDatabase("catalogdb");
var inventoryDb = postgres.AddDatabase("inventorydb");

builder.AddProject<Projects.CatalogService>("catalog")
       .WithReference(catalogDb);

builder.AddProject<Projects.InventoryService>("inventory")
       .WithReference(inventoryDb);

builder.Build().Run();
```

### Multiple persistent services

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Persistent database
var postgres = builder.AddPostgres("postgres")
                      .WithLifetime(ContainerLifetime.Persistent);

// Persistent message broker
var rabbitmq = builder.AddRabbitMQ("messaging")
                      .WithLifetime(ContainerLifetime.Persistent);

// Persistent cache
var redis = builder.AddRedis("cache")
                   .WithLifetime(ContainerLifetime.Persistent);

var db = postgres.AddDatabase("appdb");

builder.AddProject<Projects.WebApi>("api")
       .WithReference(db)
       .WithReference(rabbitmq)
       .WithReference(redis);

builder.Build().Run();
```

## See also

- [Orchestrate resources in .NET Aspire](../fundamentals/orchestrate-resources.md)
- [.NET Aspire app host overview](../fundamentals/app-host-overview.md)
- [What's new in .NET Aspire 9.0](../whats-new/dotnet-aspire-9.md#persistent-containers)
