To model the MySql resource in the app host, install the [Aspire.Hosting.MySql](https://www.nuget.org/packages/Aspire.Hosting.MySql) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.MySql
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.MySql"
                  Version="*" />
```

---

In your app host project, register a MySql database and consume the connection using the following methods:
