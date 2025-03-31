---
ms.topic: include
---

### Add Azure authenticated Npgsql client

By default, when you call <xref:Aspire.Hosting.AzurePostgresExtensions.AddAzurePostgresFlexibleServer*>
in your PostgreSQL hosting integration, it configures [📦 Azure.Identity](https://www.nuget.org/packages/Azure.Identity) NuGet package to enable client authentication. To configure the client to connect with the correct configuration, install the [📦 Aspire.Azure.Npgsql](https://www.nuget.org/packages/) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Azure.Npgsql
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Azure.Npgsql"
                  Version="*" />
```

---

<!-- TODO: Add xref to AddAzureNpgsqlDataSource when available -->

The PostgreSQL connection can be consumed using the client integration by calling the `AddAzureNpgsqlDataSource`:

```csharp
builder.AddAzureNpgsqlDataSource(connectionName: "postgresdb");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the PostgreSQL server resource in the app host project.

The preceding code snippet demonstrates how to use the `AddAzureNpgsqlDataSource` method to register an `NpgsqlDataSource` instance that uses Azure authentication ([Microsoft Entra ID](/azure/postgresql/flexible-server/concepts-azure-ad-authentication)). This `"postgresdb"` connection name corresponds to a connection string configuration value.

#### Configuration

The .NET Aspire Azure Npgsql integration provides multiple options to configure the database connection based on the requirements and conventions of your project.

##### Use a connection string

When using a connection string defined in the `ConnectionStrings` configuration section, you provide the name of the connection string when calling `AddAzureNpgsqlDataSource`:

```csharp
builder.AddAzureNpgsqlDataSource("postgresdb");
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

The .NET Aspire Azure Npgsql integration supports <xref:Microsoft.Extensions.Configuration>. It loads the `AzureNpgsqlSettings` from configuration using the `Aspire:Azure:Npgsql` key. For example, consider the following _appsettings.json_ file that configures some of the available options:

```json
{
  "Aspire": {
    "Azure": {
      "Npgsql": {
        "DisableHealthChecks": true,
        "DisableTracing": true
      }
    }
  }
}
```

##### Use inline delegates

You can configure settings in code, by passing the `Action<AzureNpgsqlSettings> configureSettings` delegate to set up some or all the options inline, for example to disable health checks from code:

```csharp
builder.AddAzureNpgsqlDataSource(
    "postgresdb",
    settings => settings.DisableHealthChecks = true);
```

Use the `AzureNpgsqlSettings.Credential` property to establish a connection. If no credential is configured, the <xref:Azure.Identity.DefaultAzureCredential> is used.

When the connection string contains a username and password, the credential is ignored.
