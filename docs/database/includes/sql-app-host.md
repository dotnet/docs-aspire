To model the SqlServer resource in the app host, install the [Aspire.Hosting.SqlServer](https://www.nuget.org/packages/Aspire.Hosting.SqlServer) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.SqlServer
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.SqlServer"
                  Version="[SelectVersion]" />
```

---

In your app host project, register a SqlServer database and consume the connection using the following methods:
