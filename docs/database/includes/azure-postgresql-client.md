---
ms.topic: include
---

### Add Azure authenticated Npgsql client

By default, when you call `AddAzurePostgresFlexibleServer` in your PostgreSQL hosting integration, it configures [📦 Azure.Identity](https://www.nuget.org/packages/Azure.Identity) NuGet package to enable authentication:

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

The PostgreSQL connection can be consumed using the client integration and <xref:Azure.Identity>:

```csharp
builder.AddNpgsqlDataSource(
    "postgresdb", 
    configureDataSourceBuilder: (dataSourceBuilder) =>
{
    if (string.IsNullOrEmpty(dataSourceBuilder.ConnectionStringBuilder.Password))
    {
        var credentials = new DefaultAzureCredential();
        var tokenRequest = new TokenRequestContext(["https://ossrdbms-aad.database.windows.net/.default"]);

        dataSourceBuilder.UsePasswordProvider(
            passwordProvider: _ => credentials.GetToken(tokenRequest).Token,
            passwordProviderAsync: async (_, ct) => (await credentials.GetTokenAsync(tokenRequest, ct)).Token);
    }
});
```

The preceding code snippet demonstrates how to use the <xref:Azure.Identity.DefaultAzureCredential> class from the <xref:Azure.Identity> package to authenticate with [Microsoft Entra ID](/azure/postgresql/flexible-server/concepts-azure-ad-authentication) and retrieve a token to connect to the PostgreSQL database. The [UsePasswordProvider](https://www.npgsql.org/doc/api/Npgsql.NpgsqlDataSourceBuilder.html#Npgsql_NpgsqlDataSourceBuilder_UsePasswordProvider_System_Func_Npgsql_NpgsqlConnectionStringBuilder_System_String__System_Func_Npgsql_NpgsqlConnectionStringBuilder_System_Threading_CancellationToken_System_Threading_Tasks_ValueTask_System_String___) method is used to provide the token to the data source builder.
