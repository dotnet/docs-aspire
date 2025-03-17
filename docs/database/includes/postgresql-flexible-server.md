---
ms.topic: include
---

The .NET Aspire Azure PostgreSQL hosting integration models a PostgreSQL flexible server and database as the <xref:Aspire.Hosting.Azure.AzurePostgresFlexibleServerResource> and <xref:Aspire.Hosting.Azure.AzurePostgresFlexibleServerDatabaseResource> types. Other types that are inherently available in the hosting integration are represented in the following resources:

- <xref:Aspire.Hosting.ApplicationModel.PostgresServerResource>
- <xref:Aspire.Hosting.ApplicationModel.PostgresDatabaseResource>
- <xref:Aspire.Hosting.Postgres.PgAdminContainerResource>
- <xref:Aspire.Hosting.Postgres.PgWebContainerResource>

To access these types and APIs for expressing them as resources in your [app host](xref:dotnet/aspire/app-host) project, install the [📦 Aspire.Hosting.Azure.PostgreSQL](https://www.nuget.org/packages/Aspire.Hosting.Azure.PostgreSQL) NuGet package:

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.PostgreSQL
```

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package).

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.PostgreSQL"
                  Version="*" />
```

For more information, see [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

---

The Azure PostgreSQL hosting integration takes a dependency on the [📦 Aspire.Hosting.PostgreSQL](https://www.nuget.org/packages/Aspire.Hosting.PostgreSQL) NuGet package, extending it to support Azure. Everything that you can do with the [.NET Aspire PostgreSQL integration](../postgresql-integration.md) and [.NET Aspire PostgreSQL Entity Framework Core integration](../postgresql-entity-framework-integration.md) you can also do with this integration.

### Add Azure PostgreSQL server resource

<span id="add-postgresql-server-resource"></span>

After you've installed the .NET Aspire Azure PostgreSQL hosting integration, call the <xref:Aspire.Hosting.AzurePostgresExtensions.AddAzurePostgresFlexibleServer*> extension method in your app host project:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddAzurePostgresFlexibleServer("postgres");
var postgresdb = postgres.AddDatabase("postgresdb");

var exampleProject = builder.AddProject<Projects.ExampleProject>()
                            .WithReference(postgresdb);
```

The preceding call to `AddAzurePostgresFlexibleServer` configures the PostgresSQL server resource to be deployed as an [Azure Postgres Flexible Server](/azure/postgresql/flexible-server/overview).

> [!IMPORTANT]
> By default, `AddAzurePostgresFlexibleServer` configures [Microsoft Entra ID](/azure/postgresql/flexible-server/concepts-azure-ad-authentication) authentication. This requires changes to applications that need to connect to these resources. For more information, see [Client integration](#client-integration).

> [!TIP]
> When you call <xref:Aspire.Hosting.AzurePostgresExtensions.AddAzurePostgresFlexibleServer*>, it implicitly calls <xref:Aspire.Hosting.AzureProvisionerExtensions.AddAzureProvisioning*>—which adds support for generating Azure resources dynamically during app startup. The app must configure the appropriate subscription and location. For more information, see [Local provisioning: Configuration](../../azure/local-provisioning.md#configuration).

#### Generated provisioning Bicep

If you're new to [Bicep](/azure/azure-resource-manager/bicep/overview), it's a domain-specific language for defining Azure resources. With .NET Aspire, you don't need to write Bicep by hand, because the provisioning APIs generate Bicep for you. When you publish your app, the generated Bicep is output alongside the manifest file. When you add an Azure PostgreSQL resource, the following Bicep is generated:

:::code language="bicep" source="../../snippets/azure/AppHost/postgres-flexible.module.bicep":::

The preceding Bicep is a module that provisions an Azure PostgreSQL flexible server with the following defaults:

- `authConfig`: The authentication configuration of the PostgreSQL server. The default is `ActiveDirectoryAuth` enabled and `PasswordAuth` disabled.
- `availabilityZone`: The availability zone of the PostgreSQL server. The default is `1`.
- `backup`: The backup configuration of the PostgreSQL server. The default is `BackupRetentionDays` set to `7` and `GeoRedundantBackup` set to `Disabled`.
- `highAvailability`: The high availability configuration of the PostgreSQL server. The default is `Disabled`.
- `storage`: The storage configuration of the PostgreSQL server. The default is `StorageSizeGB` set to `32`.
- `version`: The version of the PostgreSQL server. The default is `16`.
- `sku`: The SKU of the PostgreSQL server. The default is `Standard_B1ms`.
- `tags`: The tags of the PostgreSQL server. The default is `aspire-resource-name` set to the name of the Aspire resource, in this case `postgres-flexible`.

In addition to the PostgreSQL flexible server, it also provisions an Azure Firewall rule to allow all Azure IP addresses. Finally, an administrator is created for the PostgreSQL server, and the connection string is outputted as an output variable. The generated Bicep is a starting point and is influenced by changes to the provisioning infrastructure in C#. Customizations to the Bicep file directly will be overwritten, so make changes through the C# provisioning APIs to ensure they are reflected in the generated files.

#### Customize provisioning infrastructure

All .NET Aspire Azure resources are subclasses of the <xref:Aspire.Hosting.Azure.AzureProvisioningResource> type. This type enables the customization of the generated Bicep by providing a fluent API to configure the Azure resources by using the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},System.Action{Aspire.Hosting.Azure.AzureResourceInfrastructure})> API. For example, you can configure the `kind`, `consistencyPolicy`, `locations`, and more. The following example demonstrates how to customize the PostgreSQL server resource:

:::code language="csharp" source="../../snippets/azure/AppHost/Program.ConfigurePostgresSQLInfra.cs" id="configure":::

The preceding code:

- Chains a call to the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure*> API:
  - The `infra` parameter is an instance of the <xref:Aspire.Hosting.Azure.AzureResourceInfrastructure> type.
  - The provisionable resources are retrieved by calling the <xref:Azure.Provisioning.Infrastructure.GetProvisionableResources> method.
  - The single <xref:Azure.Provisioning.PostgreSql.PostgreSqlFlexibleServer> is retrieved.
  - The `sku` is set with <xref:Azure.Provisioning.PostgreSql.PostgreSqlFlexibleServerSkuTier.Burstable?displayProperty=nameWithType>.
  - The high availability properties are set with <xref:Azure.Provisioning.PostgreSql.PostgreSqlFlexibleServerHighAvailabilityMode.ZoneRedundant?displayProperty=nameWithType> in standby availability zone `"2"`.
  - A tag is added to the flexible server with a key of `ExampleKey` and a value of `Example value`.

There are many more configuration options available to customize the PostgreSQL flexible server resource. For more information, see <xref:Azure.Provisioning.PostgreSql> and [Azure.Provisioning customization](../../azure/integrations-overview.md#azureprovisioning-customization).

### Connect to an existing Azure PostgreSQL flexible server

You might have an existing Azure PostgreSQL flexible server that you want to connect to. Instead of representing a new Azure PostgreSQL flexible server resource, you can add a connection string to the app host. To add a connection to an existing Azure PostgreSQL flexible server, call the <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddConnectionString("postgres");

builder.AddProject<Projects.WebApplication>("web")
       .WithReference(postgres);

// After adding all resources, run the app...
```

[!INCLUDE [connection-strings-alert](../../includes/connection-strings-alert.md)]

The connection string is configured in the app host's configuration, typically under [User Secrets](/aspnet/core/security/app-secrets), under the `ConnectionStrings` section. The app host injects this connection string as an environment variable into all dependent resources, for example:

```json
{
    "ConnectionStrings": {
        "postgres": "Server=<PostgreSQL-server-name>.postgres.database.azure.com;Database=<database-name>;Port=5432;Ssl Mode=Require;User Id=<username>;"
    }
}
```

The dependent resource can access the injected connection string by calling the <xref:Microsoft.Extensions.Configuration.ConfigurationExtensions.GetConnectionString*> method, and passing the connection name as the parameter, in this case `"postgres"`. The `GetConnectionString` API is shorthand for `IConfiguration.GetSection("ConnectionStrings")[name]`.

### Run Azure PostgreSQL resource as a container

The Azure PostgreSQL hosting integration supports running the PostgreSQL server as a local container. This is beneficial for situations where you want to run the PostgreSQL server locally for development and testing purposes, avoiding the need to provision an Azure resource or connect to an existing Azure PostgreSQL server.

To run the PostgreSQL server as a container, call the <xref:Aspire.Hosting.AzurePostgresExtensions.RunAsContainer*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddAzurePostgresFlexibleServer("postgres")
                      .RunAsContainer();

var postgresdb = postgres.AddDatabase("postgresdb");

var exampleProject = builder.AddProject<Projects.ExampleProject>()
                            .WithReference(postgresdb);
```

The preceding code configures an Azure PostgreSQL Flexible Server resource to run locally in a container.

> [!TIP]
> The `RunAsContainer` method is useful for local development and testing. The API exposes an optional delegate that enables you to customize the underlying <xref:Aspire.Hosting.ApplicationModel.PostgresServerResource> configuration. For example, you can add pgAdmin and pgWeb, add a data volume or data bind mount, and add an init bind mount. For more information, see the [.NET Aspire PostgreSQL hosting integration](../postgresql-integration.md#add-postgresql-pgadmin-resource) section.

### Configure the Azure PostgreSQL server to use password authentication

By default, the Azure PostgreSQL server is configured to use [Microsoft Entra ID](/azure/postgresql/flexible-server/concepts-azure-ad-authentication) authentication. If you want to use password authentication, you can configure the server to use password authentication by calling the <xref:Aspire.Hosting.AzurePostgresExtensions.WithPasswordAuthentication*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("username", secret: true);
var password = builder.AddParameter("password", secret: true);

var postgres = builder.AddAzurePostgresFlexibleServer("postgres")
                      .WithPasswordAuthentication(username, password);

var postgresdb = postgres.AddDatabase("postgresdb");

var exampleProject = builder.AddProject<Projects.ExampleProject>()
                            .WithReference(postgresdb);
```

The preceding code configures the Azure PostgreSQL server to use password authentication. The `username` and `password` parameters are added to the app host as parameters, and the `WithPasswordAuthentication` method is called to configure the Azure PostgreSQL server to use password authentication. For more information, see [External parameters](../../fundamentals/external-parameters.md).
