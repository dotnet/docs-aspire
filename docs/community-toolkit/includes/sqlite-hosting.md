---
ms.topic: include
---

The SQLite hosting integration models a SQLite database as the `SQLiteResource` type and will create the database file in the specified location. To access these types and APIs that allow you to add the [📦 CommunityToolkit.Aspire.Hosting.SQLite](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.SQLite) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.Hosting.SQLite
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.Hosting.SQLite"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add SQLite resource

In the app host project, register and consume the SQLite integration using the `AddSQLite` extension method to add the SQLite database to the application builder.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var sqlite = builder.AddSQLite("my-database");

var exampleProject = builder.AddProject<Projects.ExampleProject>()
                            .WithReference(sqlite);
```

When .NET Aspire adds a SQLite database to the app host, as shown in the preceding example, it creates a new SQLite database file in the users temp directory. For more information, see [File resource lifecycle](../fundamentals/app-host-overview.md#file-resource-lifecycle).

Alternatively, if you want to specify a custom location for the SQLite database file, provide the relevant arguments to the `AddSqlite` method.

```csharp
var sqlite = builder.AddSQLite("my-database", "C:\\Database\\Location", "my-database.db");
```

### Add SQLiteWeb resource

When adding the SQLite resource, you can also add the SQLiteWeb resource, which provides a web interface to interact with the SQLite database. To do this, use the `WithSqliteWeb` extension method.

```csharp
var sqlite = builder.AddSQLite("my-database")
                    .WithSqliteWeb();
```

This code adds a container based on  `ghcr.io/coleifer/sqlite-web` to the app host, which provides a web interface to interact with the SQLite database it is connected to. Each SQLiteWeb instance is connected to a single SQLite database, meaning that if you add multiple SQLiteWeb instances, there will be multiple SQLiteWeb containers.

### Adding SQLite extensions

SQLite supports extensions that can be added to the SQLite database. Extensions can either be provided via a NuGet package, or via a location on disk. Use either the `WithNuGetExtension` or `WithLocalExtension` extension methods to add extensions to the SQLite database.

!> [!NOTE]
!> The SQLite extensions support is considered experimental and produces a `CTASPIRE002` warning.
