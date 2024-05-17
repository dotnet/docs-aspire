To model the PostgreSQL server resource in the app host, install the [Aspire.Hosting.PostgreSQL](https://www.nuget.org/packages/Aspire.Hosting.PostgreSQL) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.PostgreSQL
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.PostgreSQL"
                  Version="[SelectVersion]" />
```

---

In your app host project, register and consume the PostgreSQL component using the following methods, such as <xref:Aspire.Hosting.PostgresBuilderExtensions.AddPostgres%2A>:
