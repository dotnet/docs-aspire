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
