---
ms.topic: include
---

The .NET Aspire [Azure Cosmos DB](https://azure.microsoft.com/services/cosmos-db/) hosting integration models the various Cosmos DB resources as the following types:

- <xref:Aspire.Hosting.AzureCosmosDBResource>: Represents an Azure Cosmos DB resource.
- <xref:Aspire.Hosting.Azure.AzureCosmosDBContainerResource>: Represents an Azure Cosmos DB container resource.
- <xref:Aspire.Hosting.Azure.AzureCosmosDBDatabaseResource>: Represents an Azure Cosmos DB database resource.
- <xref:Aspire.Hosting.Azure.AzureCosmosDBEmulatorResource>: Represents an Azure Cosmos DB emulator resource.

To access these types and APIs for expressing them, add the [📦 Aspire.Hosting.Azure.CosmosDB](https://www.nuget.org/packages/Aspire.Hosting.Azure.CosmosDB) NuGet package in the [AppHost](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.CosmosDB
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.CosmosDB"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add Azure Cosmos DB resource

In your AppHost project, call <xref:Aspire.Hosting.AzureCosmosExtensions.AddAzureCosmosDB*> to add and return an Azure Cosmos DB resource builder.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cosmos = builder.AddAzureCosmosDB("cosmos-db");

// After adding all resources, run the app...
```

When you add an <xref:Aspire.Hosting.AzureCosmosDBResource> to the AppHost, it exposes other useful APIs to add databases and containers. In other words, you must add an `AzureCosmosDBResource` before adding any of the other Cosmos DB resources.

> [!IMPORTANT]
> When you call <xref:Aspire.Hosting.AzureCosmosExtensions.AddAzureCosmosDB*>, it implicitly calls <xref:Aspire.Hosting.AzureProvisionerExtensions.AddAzureProvisioning*>—which adds support for generating Azure resources dynamically during app startup. The app must configure the appropriate subscription and location. For more information, see [Local provisioning: Configuration](../../azure/local-provisioning.md#configuration).

#### Provisioning-generated Bicep

If you're new to [Bicep](/azure/azure-resource-manager/bicep/overview), it's a domain-specific language for defining Azure resources. With .NET Aspire, you don't need to write Bicep by-hand, instead the provisioning APIs generate Bicep for you. When you publish your app, the generated Bicep is output alongside the manifest file. When you add an Azure Cosmos DB resource, the following Bicep is generated:

:::code language="bicep" source="../../snippets/azure/AppHost/cosmos/cosmos.bicep":::

The preceding Bicep is a module that provisions an Azure Cosmos DB account resource. Additionally, role assignments are created for the Azure resource in a separate module:

:::code language="bicep" source="../../snippets/azure/AppHost/cosmos-roles/cosmos-roles.bicep":::

The generated Bicep is a starting point and is influenced by changes to the provisioning infrastructure in C#. Customizations to the Bicep file directly will be overwritten, so make changes through the C# provisioning APIs to ensure they are reflected in the generated files.

#### Customize provisioning infrastructure

All .NET Aspire Azure resources are subclasses of the <xref:Aspire.Hosting.Azure.AzureProvisioningResource> type. This type enables the customization of the generated Bicep by providing a fluent API to configure the Azure resources—using the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},System.Action{Aspire.Hosting.Azure.AzureResourceInfrastructure})> API. For example, you can configure the `kind`, `consistencyPolicy`, `locations`, and more. The following example demonstrates how to customize the Azure Cosmos DB resource:

:::code language="csharp" source="../../snippets/azure/AppHost/Program.ConfigureCosmosInfra.cs" id="configure":::

The preceding code:

- Chains a call to the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure*> API:
  - The `infra` parameter is an instance of the <xref:Aspire.Hosting.Azure.AzureResourceInfrastructure> type.
  - The provisionable resources are retrieved by calling the <xref:Azure.Provisioning.Infrastructure.GetProvisionableResources> method.
  - The single <xref:Azure.Provisioning.CosmosDB.CosmosDBAccount> is retrieved.
  - The <xref:Azure.Provisioning.CosmosDB.CosmosDBAccount.ConsistencyPolicy?displayProperty=nameWithType> is assigned to a <xref:Azure.Provisioning.CosmosDB.DefaultConsistencyLevel.Strong?displayProperty=nameWithType>.
  - A tag is added to the Cosmos DB account with a key of `ExampleKey` and a value of `Example value`.

There are many more configuration options available to customize the Azure Cosmos DB resource. For more information, see <xref:Azure.Provisioning.CosmosDB>. For more information, see [Azure.Provisioning customization](../../azure/customize-azure-resources.md#azureprovisioning-customization).

### Connect to an existing Azure Cosmos DB account

You might have an existing Azure Cosmos DB account that you want to connect to. You can chain a call to annotate that your <xref:Aspire.Hosting.AzureCosmosDBResource> is an existing resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var existingCosmosName = builder.AddParameter("existingCosmosName");
var existingCosmosResourceGroup = builder.AddParameter("existingCosmosResourceGroup");

var cosmos = builder.AddAzureCosmosDB("cosmos-db")
                    .AsExisting(existingCosmosName, existingCosmosResourceGroup);

builder.AddProject<Projects.WebApplication>("web")
       .WithReference(cosmos);

// After adding all resources, run the app...
```

[!INCLUDE [azure-configuration](../../azure/includes/azure-configuration.md)]

For more information on treating Azure Cosmos DB resources as existing resources, see [Use existing Azure resources](../../azure/integrations-overview.md#use-existing-azure-resources).

> [!NOTE]
> Alternatively, instead of representing an Azure Cosmos DB resource, you can add a connection string to the AppHost. This approach is weakly-typed, and doesn't work with role assignments or infrastructure customizations. For more information, see [Add existing Azure resources with connection strings](../../azure/integrations-overview.md#add-existing-azure-resources-with-connection-strings).

### Add Azure Cosmos DB database and container resources

.NET Aspire models parent child relationships between Azure Cosmos DB resources. For example, an Azure Cosmos DB account (<xref:Aspire.Hosting.AzureCosmosDBResource>) can have multiple databases (<xref:Aspire.Hosting.Azure.AzureCosmosDBDatabaseResource>), and each database can have multiple containers (<xref:Aspire.Hosting.Azure.AzureCosmosDBContainerResource>). When you add a database or container resource, you do so on a parent resource.

To add an Azure Cosmos DB database resource, call the <xref:Aspire.Hosting.AzureCosmosExtensions.AddCosmosDatabase*> method on an `IResourceBuilder<AzureCosmosDBResource>` instance:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cosmos = builder.AddAzureCosmosDB("cosmos-db");
var db = cosmos.AddCosmosDatabase("db");

// After adding all resources, run the app...
```

When you call `AddCosmosDatabase`, it adds a database named `db` to your Cosmos DB resources and returns the newly created database resource. The database is created in the Cosmos DB account that's represented by the `AzureCosmosDBResource` that you added earlier. The database is a logical container for collections and users.

An Azure Cosmos DB container is where data is stored. When you create a container, you need to supply a partition key.

To add an Azure Cosmos DB container resource, call the <xref:Aspire.Hosting.AzureCosmosExtensions.AddContainer*> method on an `IResourceBuilder<AzureCosmosDBDatabaseResource>` instance:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cosmos = builder.AddAzureCosmosDB("cosmos-db");
var db = cosmos.AddCosmosDatabase("db");
var container = db.AddContainer("entries", "/id");

// After adding all resources, run the app...
```

The container is created in the database that's represented by the `AzureCosmosDBDatabaseResource` that you added earlier. For more information, see [Databases, containers, and items in Azure Cosmos DB](/azure/cosmos-db/resource-model).

#### Parent child resource relationship example

To better understand the parent-child relationship between Azure Cosmos DB resources, consider the following example, which demonstrates adding an Azure Cosmos DB resource along with a database and container:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cosmos = builder.AddAzureCosmosDB("cosmos");

var customers = cosmos.AddCosmosDatabase("customers");
var profiles = customers.AddContainer("profiles", "/id");

var orders = cosmos.AddCosmosDatabase("orders");
var details = orders.AddContainer("details", "/id");
var history = orders.AddContainer("history", "/id");

builder.AddProject<Projects.Api>("api")
       .WithReference(profiles)
       .WithReference(details)
       .WithReference(history);

builder.Build().Run();
```

The preceding code adds an Azure Cosmos DB resource named `cosmos` with two databases: `customers` and `orders`. The `customers` database has a single container named `profiles`, while the `orders` database has two containers: `details` and `history`. The partition key for each container is `/id`.

The following diagram illustrates the parent child relationship between the Azure Cosmos DB resources:

:::image type="content" source="media/cosmos-resource-relationships-thumb.png" alt-text="A diagram depicting Azure Cosmos DB resource parent child relationships." lightbox="media/cosmos-resource-relationships.png":::

When your AppHost code expresses parent-child relationships, the client can deep-link to these resources by name. For example, the `customers` database can be referenced by name in the client project, registering a <xref:Microsoft.Azure.Cosmos.Database> instance that connects to the `customers` database. The same applies to named containers, for example, the `details` container can be referenced by name in the client project, registering a <xref:Microsoft.Azure.Cosmos.Container> instance that connects to the `details` container.

### Add Azure Cosmos DB emulator resource

To add an Azure Cosmos DB emulator resource, chain a call on an `IResourceBuilder<AzureCosmosDBResource>` to the <xref:Aspire.Hosting.AzureCosmosExtensions.RunAsEmulator*> API:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cosmos = builder.AddAzureCosmosDB("cosmos-db")
                    .RunAsEmulator();

// After adding all resources, run the app...
```

When you call `RunAsEmulator`, it configures your Cosmos DB resources to run locally using an emulator. The emulator in this case is the [Azure Cosmos DB Emulator](/azure/cosmos-db/local-emulator). The Azure Cosmos DB Emulator provides a free local environment for testing your Azure Cosmos DB apps and it's a perfect companion to the .NET Aspire Azure hosting integration. The emulator isn't installed, instead, it's accessible to .NET Aspire as a container. When you add a container to the AppHost, as shown in the preceding example with the `mcr.microsoft.com/cosmosdb/emulator` image, it creates and starts the container when the AppHost starts. For more information, see [Container resource lifecycle](../../fundamentals/orchestrate-resources.md#container-resource-lifecycle).

#### Configure Cosmos DB emulator container

There are various configurations available to container resources, for example, you can configure the container's ports, environment variables, it's [lifetime](../../fundamentals/orchestrate-resources.md#container-resource-lifetime), and more.

##### Configure Cosmos DB emulator container gateway port

By default, the Cosmos DB emulator container when configured by .NET Aspire, exposes the following endpoints:

| Endpoint | Container port | Host port |
|----------|----------------|-----------|
| `https`  | 8081           | dynamic   |

The port that it's listening on is dynamic by default. When the container starts, the port is mapped to a random port on the host machine. To configure the endpoint port, chain calls on the container resource builder provided by the `RunAsEmulator` method as shown in the following example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cosmos = builder.AddAzureCosmosDB("cosmos-db").RunAsEmulator(
                     emulator =>
                     {
                         emulator.WithGatewayPort(7777);
                     });

// After adding all resources, run the app...
```

The preceding code configures the Cosmos DB emulator container's existing `https` endpoint to listen on port `8081`. The Cosmos DB emulator container's port is mapped to the host port as shown in the following table:

| Endpoint name | Port mapping (`container:host`) |
|--------------:|---------------------------------|
| `https`       | `8081:7777`                     |

##### Configure Cosmos DB emulator container with persistent lifetime

To configure the Cosmos DB emulator container with a persistent lifetime, call the <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithLifetime*> method on the Cosmos DB emulator container resource and pass <xref:Aspire.Hosting.ApplicationModel.ContainerLifetime.Persistent?displayProperty=nameWithType>:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cosmos = builder.AddAzureCosmosDB("cosmos-db").RunAsEmulator(
                     emulator =>
                     {
                         emulator.WithLifetime(ContainerLifetime.Persistent);
                     });

// After adding all resources, run the app...
```

For more information, see [Container resource lifetime](../../fundamentals/orchestrate-resources.md#container-resource-lifetime).

##### Configure Cosmos DB emulator container with data volume

To add a data volume to the Azure Cosmos DB emulator resource, call the <xref:Aspire.Hosting.AzureCosmosExtensions.WithDataVolume*> method on the Azure Cosmos DB emulator resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cosmos = builder.AddAzureCosmosDB("cosmos-db").RunAsEmulator(
                     emulator =>
                     {
                         emulator.WithDataVolume();
                     });

// After adding all resources, run the app...
```

The data volume is used to persist the Cosmos DB emulator data outside the lifecycle of its container. The data volume is mounted at the `/tmp/cosmos/appdata` path in the Cosmos DB emulator container and when a `name` parameter isn't provided, the name is generated. The emulator has its `AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE` environment variable set to `true`. For more information on data volumes and details on why they're preferred over bind mounts, see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

#### Configure Cosmos DB emulator container partition count

To configure the partition count of the Cosmos DB emulator container, call the <xref:Aspire.Hosting.AzureCosmosExtensions.WithPartitionCount*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cosmos = builder.AddAzureCosmosDB("cosmos-db").RunAsEmulator(
                     emulator =>
                     {
                         emulator.WithPartitionCount(100); // Defaults to 25
                     });

// After adding all resources, run the app...
```

The preceding code configures the Cosmos DB emulator container to have a partition count of `100`. This is a shorthand for setting the `AZURE_COSMOS_EMULATOR_PARTITION_COUNT` environment variable.

#### Use Linux-based emulator (preview)

The [next generation of the Azure Cosmos DB emulator](/azure/cosmos-db/emulator-linux) is entirely Linux-based and is available as a Docker container. It supports running on a wide variety of processors and operating systems.

To use the preview version of the Cosmos DB emulator, call the <xref:Aspire.Hosting.AzureCosmosExtensions.RunAsPreviewEmulator*> method. Since this feature is in preview, you need to explicitly opt into the preview feature by suppressing the `ASPIRECOSMOSDB001` experimental diagnostic.

The preview emulator also supports exposing a "Data Explorer" endpoint which allows you to view the data stored in the Cosmos DB emulator via a web UI. To enable the Data Explorer, call the <xref:Aspire.Hosting.AzureCosmosExtensions.WithDataExplorer*> method.

```csharp
#pragma warning disable ASPIRECOSMOSDB001

var builder = DistributedApplication.CreateBuilder(args);

var cosmos = builder.AddAzureCosmosDB("cosmos-db").RunAsPreviewEmulator(
                     emulator =>
                     {
                         emulator.WithDataExplorer();
                     });

// After adding all resources, run the app...
```

The preceding code configures the Linux-based preview Cosmos DB emulator container, with the Data Explorer endpoint, to use at run time.
