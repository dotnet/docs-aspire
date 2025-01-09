---
ms.topic: include
---

The .NET Aspire [Azure Cosmos DB](https://azure.microsoft.com/services/cosmos-db/) hosting integration models the various Cosmos DB resources as the following types:

- <xref:Aspire.Hosting.AzureCosmosDBResource>: Represents an Azure Cosmos DB resource.
- <xref:Aspire.Hosting.Azure.AzureCosmosDBEmulatorResource>: Represents an Azure Cosmos DB emulator resource.

To access these types and APIs for expressing them, add the [📦 Aspire.Hosting.Azure.CosmosDB](https://www.nuget.org/packages/Aspire.Hosting.Azure.CosmosDB) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

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

In your app host project, call <xref:Aspire.Hosting.AzureCosmosExtensions.AddAzureCosmosDB*> to add and return an Azure Cosmos DB resource builder.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cosmos = builder.AddAzureCosmosDB("cosmos-db");

// After adding all resources, run the app...
```

When you add an <xref:Aspire.Hosting.AzureCosmosDBResource> to the app host, it exposes other useful APIs to add databases and containers. In other words, you must add an `AzureCosmosDBResource` before adding any of the other Cosmos DB resources.

> [!IMPORTANT]
> When you call <xref:Aspire.Hosting.AzureCosmosExtensions.AddAzureCosmosDB*>, it implicitly calls <xref:Aspire.Hosting.AzureProvisionerExtensions.AddAzureProvisioning*>—which adds support for generating Azure resources dynamically during app startup. The app must configure the appropriate subscription and location. For more information, see [Local provisioning: Configuration](../../azure/local-provisioning.md#configuration).

#### Generated provisioning Bicep

If you're new to [Bicep](/azure/azure-resource-manager/bicep/overview), it's a domain-specific language for defining Azure resources. With .NET Aspire, you don't need to write Bicep by-hand, instead the provisioning APIs generate Bicep for you. When you publish your app, the generated Bicep is output alongside the manifest file. When you add an Azure Cosmos DB resource, the following Bicep is generated:

<!-- markdownlint-disable MD033 -->
<br/>
<details>
<summary id="cosmos-bicep"><strong>Toggle Azure Cosmos DB Bicep.</strong></summary>
<p aria-labelledby="cosmos-bicep">

:::code language="bicep" source="../../snippets/azure/AppHost/cosmos.module.bicep":::

</p>
</details>
<!-- markdownlint-enable MD033 -->

The preceding Bicep is a module that provisions an Azure Cosmos DB account with the following defaults:

- `kind`: The kind of Cosmos DB account. The default is `GlobalDocumentDB`.
- `consistencyPolicy`: The consistency policy of the Cosmos DB account. The default is `Session`.
- `locations`: The locations for the Cosmos DB account. The default is the resource group's location.

In addition to the Cosmos DB account, it also provisions an Azure Key Vault resource. This is used to store the Cosmos DB account's connection string securely. The generated Bicep is a starting point and can be customized to meet your specific requirements.

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

There are many more configuration options available to customize the Azure Cosmos DB resource. For more information, see <xref:Azure.Provisioning.CosmosDB>. For more information, see [Azure.Provisioning customization](../../azure/integrations-overview.md#azureprovisioning-customization).

### Connect to an existing Azure Cosmos DB account

You might have an existing Azure Cosmos DB account that you want to connect to. Instead of representing a new Azure Cosmos DB resource, you can add a connection string to the app host. To add a connection to an existing Azure Cosmos DB account, call the <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cosmos = builder.AddConnectionString("cosmos-db");

builder.AddProject<Projects.WebApplication>("web")
       .WithReference(cosmos);

// After adding all resources, run the app...
```

[!INCLUDE [connection-strings-alert](../../includes/connection-strings-alert.md)]

The connection string is configured in the app host's configuration, typically under [User Secrets](/aspnet/core/security/app-secrets), under the `ConnectionStrings` section. The app host injects this connection string as an environment variable into all dependent resources, for example:

```json
{
    "ConnectionStrings": {
        "cosmos-db": "AccountEndpoint=https://{account_name}.documents.azure.com:443/;AccountKey={account_key};"
    }
}
```

The dependent resource can access the injected connection string by calling the <xref:Microsoft.Extensions.Configuration.ConfigurationExtensions.GetConnectionString*> method, and passing the connection name as the parameter, in this case `"cosmos-db"`. The `GetConnectionString` API is shorthand for `IConfiguration.GetSection("ConnectionStrings")[name]`.

### Add Azure Cosmos DB database resource

To add an Azure Cosmos DB database resource, chain a call on an `IResourceBuilder<AzureCosmosDBResource>` to the <xref:Aspire.Hosting.AzureCosmosExtensions.AddDatabase*> API:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cosmos = builder.AddAzureCosmosDB("cosmos-db")
                    .AddDatabase("db");

// After adding all resources, run the app...
```

When you call `AddDatabase`, it configures your Cosmos DB resources to have a database named `db`. The database is created in the Cosmos DB account that's represented by the `AzureCosmosDBResource` that you added earlier. The database is a logical container for collections and users. For more information, see [Databases, containers, and items in Azure Cosmos DB](/azure/cosmos-db/resource-model).

> [!NOTE]
> When using the `AddDatabase` API to add a database to an Azure Cosmos DB resource, if you're running the emulator, the database isn't actually created in the emulator. This API is intended to include a database in the [Bicep generated](#generated-provisioning-bicep) by the provisioning infrastructure.

### Add Azure Cosmos DB emulator resource

To add an Azure Cosmos DB emulator resource, chain a call on an `IResourceBuilder<AzureCosmosDBResource>` to the <xref:Aspire.Hosting.AzureCosmosExtensions.RunAsEmulator*> API:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cosmos = builder.AddAzureCosmosDB("cosmos-db")
                    .RunAsEmulator();

// After adding all resources, run the app...
```

When you call `RunAsEmulator`, it configures your Cosmos DB resources to run locally using an emulator. The emulator in this case is the [Azure Cosmos DB Emulator](/azure/cosmos-db/local-emulator). The Azure Cosmos DB Emulator provides a free local environment for testing your Azure Cosmos DB apps and it's a perfect companion to the .NET Aspire Azure hosting integration. The emulator isn't installed, instead, it's accessible to .NET Aspire as a container. When you add a container to the app host, as shown in the preceding example with the `mcr.microsoft.com/cosmosdb/emulator` image, it creates and starts the container when the app host starts. For more information, see [Container resource lifecycle](../../fundamentals/app-host-overview.md#container-resource-lifecycle).

#### Configure Cosmos DB emulator container

There are various configurations available to container resources, for example, you can configure the container's ports, environment variables, it's [lifetime](../../fundamentals/app-host-overview.md#container-resource-lifetime), and more.

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

For more information, see [Container resource lifetime](../../fundamentals/app-host-overview.md#container-resource-lifetime).

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
