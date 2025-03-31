---
ms.topic: include
---

### Add Azure authenticated Npgsql client

By default, when you call <xref:Aspire.Hosting.AzurePostgresExtensions.AddAzurePostgresFlexibleServer*>
in your PostgreSQL hosting integration, it configures [📦 Azure.Identity](https://www.nuget.org/packages/Azure.Identity) NuGet package to enable client authentication. To configure the client to connect with the correct configuration, install the [📦 Aspire.Azure.Npgsql.EntityFrameworkCore.PostgreSQL](https://www.nuget.org/packages/) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Azure.Npgsql.EntityFrameworkCore.PostgreSQL
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Azure.Npgsql.EntityFrameworkCore.PostgreSQL"
                  Version="*" />
```

---

<!-- TODO: Add xref to AddAzureNpgsqlDbContext when available -->

The PostgreSQL connection can be consumed using the client integration by calling the `AddAzureNpgsqlDataSource`:

```csharp
builder.AddAzureNpgsqlDbContext<MyDbContext>(connectionName: "postgresdb");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the PostgreSQL server resource in the app host project.

The preceding code snippet demonstrates how to use the `AddAzureNpgsqlDataSource` method to register an `NpgsqlDataSource` instance that uses Azure authentication ([Microsoft Entra ID](/azure/postgresql/flexible-server/concepts-azure-ad-authentication)). This `"postgresdb"` connection name corresponds to a connection string configuration value.

<!-- TODO: Add xref to EnrichAzureNpgsqlDbContext when available -->

You might also need to configure specific options of Npgsql, or register a <xref:System.Data.Entity.DbContext> in other ways. In this case, you do so by calling the `EnrichAzureNpgsqlDbContext` extension method, as shown in the following example:

```csharp
var connectionString = builder.Configuration.GetConnectionString("postgresdb");

builder.Services.AddDbContextPool<MyDbContext>(
    dbContextOptionsBuilder => dbContextOptionsBuilder.UseNpgsql(connectionString));

builder.EnrichAzureNpgsqlDbContext<MyDbContext>();
```

#### Configuration

The .NET Aspire Azure PostgreSQL EntityFrameworkCore Npgsql integration provides multiple options to configure the database connection based on the requirements and conventions of your project.

##### Use a connection string

When using a connection string defined in the `ConnectionStrings` configuration section, you provide the name of the connection string when calling `AddAzureNpgsqlDataSource`:

```csharp
builder.AddAzureNpgsqlDbContext<MyDbContext>("postgresdb");
```

The connection string is retrieved from the `ConnectionStrings` configuration section, for example, consider the following JSON configuration:

```json
{
  "ConnectionStrings": {
    "postgresdb": "Host=myserver;Database=test"
  }
}
```

For more information on how to configure the connection string, see the [Npgsql connection string documentation](https://www.npgsql.org/doc/connection-string-parameters.html).

> [!NOTE]
> The username and password are automatically inferred from the credential provided in the settings.

##### Use configuration providers

<!-- TODO: Add xref to AzureNpgsqlEntityFrameworkCorePostgreSQLSettings when available -->

The .NET Aspire Azure PostgreSQL EntityFrameworkCore Npgsql integration supports <xref:Microsoft.Extensions.Configuration>. It loads the `AzureNpgsqlEntityFrameworkCorePostgreSQLSettings` from configuration using the `Aspire:Npgsql:EntityFrameworkCore:PostgreSQL` key. For example, consider the following _appsettings.json_ file that configures some of the available options:

```json
{
  "Aspire": {
    "Npgsql": {
      "EntityFrameworkCore": {
        "PostgreSQL": {
          "DisableHealthChecks": true,
          "DisableTracing": true
        }
      }
    }
  }
}
```

##### Use inline delegates

You can configure settings in code, by passing the `Action<AzureNpgsqlEntityFrameworkCorePostgreSQLSettings> configureSettings` delegate to set up some or all the options inline, for example to disable health checks from code:

```csharp
builder.AddAzureNpgsqlDbContext<MyDbContext>(
    "postgresdb",
    settings => settings.DisableHealthChecks = true);
```

Alternatively, you can use the `EnrichAzureNpgsqlDbContext` extension method to configure the settings:

```csharp
builder.EnrichAzureNpgsqlDbContext<MyDbContext>(
    settings => settings.DisableHealthChecks = true);
```

<!-- TODO: Add xref to AzureNpgsqlEntityFrameworkCorePostgreSQLSettings.Credential when available -->

Use the `AzureNpgsqlEntityFrameworkCorePostgreSQLSettings.Credential` property to establish a connection. If no credential is configured, the <xref:Azure.Identity.DefaultAzureCredential> is used.

When the connection string contains a username and password, the credential is ignored.

##### Troubleshooting

In the rare case that the `Username` property isn't provided and the integration can't detect it using the application's Managed Identity, Npgsql throws an exception with a message similar to the following:

> Npgsql.PostgresException (0x80004005): 28P01: password authentication failed for user ...

In this case you can configure the `Username` property in the connection string and use `EnrichAzureNpgsqlDbContext`, passing the connection string in `UseNpgsql`:

```csharp
builder.Services.AddDbContextPool<MyDbContext>(
    options => options.UseNpgsql(newConnectionString));

builder.EnrichAzureNpgsqlDbContext<MyDbContext>();
```
