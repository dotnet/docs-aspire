To model the SurrealDB resource in the app host, install the [Aspire.Hosting.SurrealDb](https://www.nuget.org/packages/Aspire.Hosting.SurrealDb) NuGet package in the [app host](xref:aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.SurrealDb
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.SurrealDb"
                  Version="[SelectVersion]" />
```

---

In your app host project, register a SurrealDb database and consume the connection using the following methods:
