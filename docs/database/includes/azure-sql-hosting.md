---
ms.topic: include
---

The Azure SQL hosting integration models the Azure SQL server as the <xref:Aspire.Hosting.Azure.AzureSqlServerResource> type and the database as the <xref:Aspire.Hosting.Azure.AzureSqlDatabaseResource> type. To access these types and APIs, add the [📦 Aspire.Hosting.Azure.Sql](https://www.nuget.org/packages/Aspire.Hosting.Azure.Sql) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.Sql
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.Sql"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

The Azure SQL hosting integration takes a dependency on the [📦 Aspire.Hosting.SqlServer](https://www.nuget.org/packages/Aspire.Hosting.SqlServer/) NuGet package, extending it to support Azure. Everything that you can do with the [.NET Aspire SQL Server integration](sql-server-integration.md) and [.NET Aspire SQL Server Entity Framework Core integration](sql-server-entity-framework-integration.md) you can also do with this integration.

### Add Azure SQL server resource and database resource

In your app host project, call <xref:Aspire.Hosting.AzureSqlExtensions.AddAzureSqlServer*> to add and return an Azure SQL server resource builder. Chain a call to the returned resource builder to <xref:Aspire.Hosting.AzureSqlExtensions.AddDatabase*>, to add an Azure SQL database resource:

```csharp
var azureSql = builder.AddAzureSqlServer("azuresql")
                      .AddDatabase("database");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(azureSql);
```

The preceding call to `AddAzureSqlServer` configures the Azure SQL server resource to be deployed as an [Azure SQL Database server](/azure/azure-sql/database/sql-database-paas-overview).

> [!IMPORTANT]
> By default, `AddAzureSqlServer` configures [Microsoft Entra ID](/azure/azure-sql/database/authentication-aad-overview) authentication. This requires changes to applications that need to connect to these resources. For more information, see [Client integration](#client-integration).

> [!TIP]
> When you call <xref:Aspire.Hosting.AzureSqlExtensions.AddAzureSqlServer*>, it implicitly calls <xref:Aspire.Hosting.AzureProvisionerExtensions.AddAzureProvisioning*> — which adds support for generating Azure resources dynamically during app startup. The app must configure the appropriate subscription and location. For more information, see [Local provisioning: Configuration](../azure/local-provisioning.md#configuration).

### Connect to an existing Azure SQL server

You might have an existing Azure SQL database that you want to connect to. Instead of representing a new Azure SQL server resource, you can add a connection string to the app host. To add a connection to an existing Azure SQL server, call the <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var azureSql = builder.AddConnectionString("database");

builder.AddProject<Projects.WebApplication>("web")
       .WithReference(azureSql);

// After adding all resources, run the app...
```

[!INCLUDE [connection-strings-alert](../includes/connection-strings-alert.md)]

The connection string is configured in the app host's configuration, typically under [User Secrets](/aspnet/core/security/app-secrets), under the `ConnectionStrings` section. The app host injects this connection string as an environment variable into all dependent resources, for example:

```json
{
    "ConnectionStrings": {
        "database": "Server=tcp:<Azure-SQL-server-name>.database.windows.net,1433;Initial Catalog=<database-name>;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;User ID=<username>;"
    }
}
```

The dependent resource can access the injected connection string by calling the <xref:Microsoft.Extensions.Configuration.ConfigurationExtensions.GetConnectionString*> method, and passing the connection name as the parameter, in this case `"database"`. The `GetConnectionString` API is shorthand for `IConfiguration.GetSection("ConnectionStrings")[name]`.

### Run Azure SQL server resource as a container

The Azure SQL Server hosting integration supports running the Azure SQL server as a local container. This is beneficial for situations where you want to run the Azure SQL server locally for development and testing purposes, avoiding the need to provision an Azure resource or connect to an existing Azure SQL server.

To run the Azure SQL server as a container, call the <xref:Aspire.Hosting.AzureSqlExtensions.RunAsContainer*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var azureSql = builder.AddAzureSqlServer("azuresql")
                      .RunAsContainer();

var azureSqlData = postgres.AddDatabase("database");

var exampleProject = builder.AddProject<Projects.ExampleProject>()
                            .WithReference(azureSqlData);
```

The preceding code configures an Azure SQL Database resource to run locally in a container.

> [!TIP]
> The `RunAsContainer` method is useful for local development and testing. The API exposes an optional delegate that enables you to customize the underlying <xref:Aspire.Hosting.ApplicationModel.SqlServerServerResource> configuration. For example, you can add a data volume or data bind mount. For more information, see the [.NET Aspire SQL Server hosting integration](sql-server-integration.md#add-sql-server-resource-with-data-volume) section.
