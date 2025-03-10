---
ms.topic: include
---

### Add Azure authenticated Npgsql client

By default, when you call `AddAzurePostgresFlexibleServer` in your PostgreSQL hosting integration, it requires [📦 Azure.Identity](https://www.nuget.org/packages/Azure.Identity) NuGet package to enable authentication:

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Azure.Identity
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Azure.Identity"
                  Version="*" />
```

---

The PostgreSQL connection can be consumed using the client integration and <xref:Azure.Identity>.

The following code snippets demonstrate how to use the <xref:Azure.Identity.DefaultAzureCredential> class from the <xref:Azure.Identity> package to authenticate with [Microsoft Entra ID](/azure/postgresql/flexible-server/concepts-azure-ad-authentication) and retrieve a token to connect to the PostgreSQL database. The [UsePasswordProvider](https://www.npgsql.org/doc/api/Npgsql.NpgsqlDataSourceBuilder.html#Npgsql_NpgsqlDataSourceBuilder_UsePasswordProvider_System_Func_Npgsql_NpgsqlConnectionStringBuilder_System_String__System_Func_Npgsql_NpgsqlConnectionStringBuilder_System_Threading_CancellationToken_System_Threading_Tasks_ValueTask_System_String___) method is used to provide the token to the data source builder.

### EF Core version 8

```csharp
var dsBuilder = new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("postgresdb"));
if (string.IsNullOrEmpty(dsBuilder.ConnectionStringBuilder.Password))
{
    var credentials = new DefaultAzureCredential();
    var tokenRequest = new TokenRequestContext(["https://ossrdbms-aad.database.windows.net/.default"]);

    dsBuilder.UsePasswordProvider(
        passwordProvider: _ => credentials.GetToken(tokenRequest).Token,
        passwordProviderAsync: async (_, ct) => (await credentials.GetTokenAsync(tokenRequest, ct)).Token);
}

builder.AddNpgsqlDbContext<MyDb1Context>(
    "postgresdb",
    configureDbContextOptions: (options) => options.UseNpgsql(dsBuilder.Build()));
```

### EF Core version 9+

With EF Core version 9, you can use the `ConfigureDataSource` method to configure the `NpgsqlDataSourceBuilder` that's used by the integration instead of building one outside of the integration and passing it in.

```csharp
builder.AddNpgsqlDbContext<MyDb1Context>(
    "postgresdb",
    configureDbContextOptions: (options) => options.UseNpgsql(npgsqlOptions =>
        npgsqlOptions.ConfigureDataSource(dsBuilder =>
        {
            if (string.IsNullOrEmpty(dsBuilder.ConnectionStringBuilder.Password))
            {
                var credentials = new DefaultAzureCredential();
                var tokenRequest = new TokenRequestContext(["https://ossrdbms-aad.database.windows.net/.default"]);

                dsBuilder.UsePasswordProvider(
                    passwordProvider: _ => credentials.GetToken(tokenRequest).Token,
                    passwordProviderAsync: async (_, ct) => (await credentials.GetTokenAsync(tokenRequest, ct)).Token);
            }
        })));
```
