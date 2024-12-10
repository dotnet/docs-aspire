---
ms.topic: include
---

The MySQL hosting integration models the server as the <xref:Aspire.Hosting.ApplicationModel.MySqlServerResource> type and the database as the <xref:Aspire.Hosting.ApplicationModel.MySqlDatabaseResource> type. To access these types and APIs, add the [📦 Aspire.Hosting.MySql](https://www.nuget.org/packages/Aspire.Hosting.MySql) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

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

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add MySQL server resource and database resource

In your app host project, call <xref:Aspire.Hosting.MySqlBuilderExtensions.AddMySql*> to add and return a MySQL resource builder. Chain a call to the returned resource builder to <xref:Aspire.Hosting.MySqlBuilderExtensions.AddDatabase*>, to add a MySQL database resource.
