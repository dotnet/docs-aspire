---
ms.topic: include
---

## Azure hosting integration

To deploy your PostgreSQL resources to Azure, install the [📦 Aspire.Hosting.Azure.PostgreSQL](https://www.nuget.org/packages/Aspire.Hosting.Azure.PostgreSQL) NuGet package:

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.PostgreSQL
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.PostgreSQL"
                  Version="*" />
```

---

### Add Azure PostgreSQL server resource

After you've installed the .NET Aspire hosting Azure PostgreSQL package, call the `AddAzurePostgresFlexibleServer` extension method in your app host project:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddAzurePostgresFlexibleServer("postgres");
var postgresdb = postgres.AddDatabase("postgresdb");

var exampleProject = builder.AddProject<Projects.ExampleProject>()
                            .WithReference(postgresdb);
```

The preceding call to `AddAzurePostgresFlexibleServer` configures the PostgresSQL server resource to be deployed as [Azure Postgres Flexible Server](/azure/postgresql/flexible-server/overview).

> [!IMPORTANT]
> By default, `AddAzurePostgresFlexibleServer` configures [Microsoft Entra ID](/azure/postgresql/flexible-server/concepts-azure-ad-authentication) authentication. This requires changes to applications that need to connect to these resources. For more information, see [Client integration](#client-integration).
