### Azure app host usage

To deploy your PostgreSQL resources to Azure, you need to install the appropriate .NET Aspire hosting package:

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.PostgreSQL
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.PostgreSQL"
                  Version="[SelectVersion]" />
```

---

After you've installed this package, you specify that your PostgreSQL resources will be hosted in Azure by calling the <xref:Aspire.Hosting.AzurePostgresExtensions.PublishAsAzurePostgresFlexibleServer%2A> extension method in your app host project:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                      .PublishAsAzurePostgresFlexibleServer();

var postgresdb = postgres.AddDatabase("postgresdb");

var exampleProject = builder.AddProject<Projects.ExampleProject>()
                            .WithReference(postgresdb);
```

The preceding call to `PublishAsAzurePostgresFlexibleServer` configures Postgres Server resource to be deployed as Azure Postgres Flexible Server. For more information, see [Azure Postgres Flexible Server](/azure/postgresql/flexible-server/overview).
