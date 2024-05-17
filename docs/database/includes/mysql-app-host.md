To model the MySql resource in the app host, install the [Aspire.Hosting.MySql](https://www.nuget.org/packages/Aspire.Hosting.MySql) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.MySql
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.MySql"
                  Version="[SelectVersion]" />
```

---

In your app host project, register a MySql database and consume the connection using the following methods:
