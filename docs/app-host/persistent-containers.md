---
title: Persistent container lifetimes in .NET Aspire
description: Learn how to configure containers to persist and be re-used between .NET Aspire AppHost runs.
ms.date: 07/15/2025
---

# Persistent container lifetimes in .NET Aspire

In .NET Aspire, containers follow a typical lifecycle where they're created when the AppHost starts and destroyed when it stops. However, you can specify that you want to use **persistent containers**, which deviate from this standard lifecycle. Persistent containers are created and started by the .NET Aspire orchestrator but aren't destroyed when the AppHost stops, allowing them to persist between runs.

This feature is particularly beneficial for containers that have long startup times, such as databases, as it eliminates the need to wait for these services to initialize on every AppHost restart.

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

In the preceding example, the PostgreSQL container is configured to persist between AppHost runs, while the `inventory` project references the database as normal.

## Dashboard visualization

The .NET Aspire dashboard shows persistent containers with a distinctive pin icon (📌) to help you identify them:

:::image type="content" source="../whats-new/media/persistent-container.png" lightbox="../whats-new/media/persistent-container.png" alt-text="Screenshot of the .NET Aspire dashboard showing a persistent container with a pin icon.":::

After the AppHost stops, persistent containers continue running and can be seen in your container runtime (such as Docker Desktop):

:::image type="content" source="../whats-new/media/persistent-container-docker-desktop.png" lightbox="../whats-new/media/persistent-container-docker-desktop.png" alt-text="Screenshot of Docker Desktop showing a persistent RabbitMQ container still running after the AppHost stopped.":::

## Configuration change detection

Persistent containers are automatically recreated when the AppHost detects meaningful configuration changes. .NET Aspire tracks a hash of the configuration used to create each container and compares it to the current configuration on subsequent runs. If the configuration differs, the container is recreated with the new settings.

This mechanism ensures that persistent containers stay synchronized with your AppHost configuration without requiring manual intervention.

## Container naming and uniqueness

By default, persistent containers use a naming pattern that combines:

- The service name you specify in your AppHost.
- A postfix based on a hash of the AppHost project path.

This naming scheme ensures that persistent containers are unique to each AppHost project, preventing conflicts when multiple .NET Aspire projects use the same service names.

For example, if you have a service named `"postgres"` in an AppHost project located at `/path/to/MyApp.AppHost`, the container name might be `postgres-abc123def` where `abc123def` is derived from the project path hash.

### Custom container names

For advanced scenarios, you can set a custom container name using the <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithContainerName*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                      .WithLifetime(ContainerLifetime.Persistent)
                      .WithContainerName("my-shared-postgres");

builder.Build().Run();
```

When you specify a custom container name, .NET Aspire first checks if a container with that name already exists. If a container with that name exists and was previously created by .NET Aspire, it follows the normal persistent container behavior and can be automatically recreated if the configuration changes. If a container with that name exists but wasn't created by .NET Aspire, it won't be managed or recreated by the AppHost. If no container with the custom name exists, .NET Aspire creates a new one.

## Manual cleanup

> [!IMPORTANT]
> Persistent containers aren't automatically removed when you stop the AppHost. To delete these containers, you must manually stop and remove them using your container runtime.

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
- **Shared services**: Services that multiple AppHosts or development team members can share.

## See also

- [Orchestrate resources in .NET Aspire](../fundamentals/orchestrate-resources.md)
- [.NET Aspire AppHost overview](../fundamentals/app-host-overview.md)
- [What's new in .NET Aspire 9.0](../whats-new/dotnet-aspire-9.md#persistent-containers)
