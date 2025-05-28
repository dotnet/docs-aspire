---
ms.topic: include
---

### Add Azure Storage emulator resource

To add an Azure Storage emulator resource, chain a call on an `IResourceBuilder<AzureStorageResource>` to the <xref:Aspire.Hosting.AzureStorageExtensions.RunAsEmulator*> API:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage")
                     .RunAsEmulator();

// After adding all resources, run the app...
```

When you call `RunAsEmulator`, it configures your storage resources to run locally using an emulator. The emulator in this case is [Azurite](/azure/storage/common/storage-use-azurite). The Azurite open-source emulator provides a free local environment for testing your Azure Blob, Queue Storage, and Table Storage apps and it's a perfect companion to the .NET Aspire Azure hosting integration. Azurite isn't installed; instead, it's accessible to .NET Aspire as a container. When you add a container to the app host, as shown in the preceding example with the `mcr.microsoft.com/azure-storage/azurite` image, it creates and starts the container when the app host starts. For more information, see [Container resource lifecycle](../../fundamentals/orchestrate-resources.md#container-resource-lifecycle).

#### Configure Azurite container

There are various configurations available to container resources, for example, you can configure the container's ports, environment variables, [lifetime](../../fundamentals/orchestrate-resources.md#container-resource-lifetime), and more.

##### Configure Azurite container ports

By default, the Azurite container, when configured by .NET Aspire, exposes the following endpoints:

| Endpoint | Container port | Host port |
|----------|----------------|-----------|
| `blob`   | 10000          | dynamic   |
| `queue`  | 10001          | dynamic   |
| `table`  | 10002          | dynamic   |

The port that they're listening on is dynamic by default. When the container starts, the ports are mapped to a random port on the host machine. To configure the endpoint ports, chain calls on the container resource builder provided by the `RunAsEmulator` method as shown in the following example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage").RunAsEmulator(
                     azurite =>
                     {
                         azurite.WithBlobPort(27000)
                                .WithQueuePort(27001)
                                .WithTablePort(27002);
                     });

// After adding all resources, run the app...
```

The preceding code configures the Azurite container's existing `blob`, `queue`, and `table` endpoints to listen on ports `27000`, `27001`, and `27002`, respectively. The Azurite container's ports are mapped to the host ports as shown in the following table:

| Endpoint name | Port mapping (`container:host`) |
|---------------|---------------------------------|
| `blob`        | `10000:27000`                   |
| `queue`       | `10001:27001`                   |
| `table`       | `10002:27002`                   |

##### Configure Azurite container with persistent lifetime

To configure the Azurite container with a persistent lifetime, call the <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithLifetime*> method on the Azurite container resource and pass <xref:Aspire.Hosting.ApplicationModel.ContainerLifetime.Persistent?displayProperty=nameWithType>:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage").RunAsEmulator(
                     azurite =>
                     {
                         azurite.WithLifetime(ContainerLifetime.Persistent);
                     });

// After adding all resources, run the app...
```

For more information, see [Container resource lifetime](../../fundamentals/orchestrate-resources.md#container-resource-lifetime).

##### Configure Azurite container with data volume

To add a data volume to the Azure Storage emulator resource, call the <xref:Aspire.Hosting.AzureStorageExtensions.WithDataVolume*> method on the Azure Storage emulator resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage").RunAsEmulator(
                     azurite =>
                     {
                         azurite.WithDataVolume();
                     });

// After adding all resources, run the app...
```

The data volume is used to persist the Azurite data outside the lifecycle of its container. The data volume is mounted at the `/data` path in the Azurite container and when a `name` parameter isn't provided, the name is formatted as `.azurite/{resource name}`. For more information on data volumes and details on why they're preferred over [bind mounts](#configure-azurite-container-with-data-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

##### Configure Azurite container with data bind mount

To add a data bind mount to the Azure Storage emulator resource, call the <xref:Aspire.Hosting.AzureStorageExtensions.WithDataBindMount*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage").RunAsEmulator(
                     azurite =>
                     {
                         azurite.WithDataBindMount("../Azurite/Data");
                     });

// After adding all resources, run the app...
```

[!INCLUDE [data-bind-mount-vs-volumes](../../includes/data-bind-mount-vs-volumes.md)]

Data bind mounts rely on the host machine's filesystem to persist the Azurite data across container restarts. The data bind mount is mounted at the `../Azurite/Data` path on the host machine relative to the app host directory (<xref:Aspire.Hosting.IDistributedApplicationBuilder.AppHostDirectory?displayProperty=nameWithType>) in the Azurite container. For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).
