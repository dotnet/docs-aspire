---
title: .NET Aspire Community Toolkit SQLite integration
description: Learn how to use the .NET Aspire SQLite integration for efficient data management within your applications.
ms.date: 03/04/2025
---

# .NET Aspire Community Toolkit SQLite integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[!INCLUDE [banner](includes/banner.md)]

[SQLite](https://www.sqlite.org/index.html) is a lightweight, serverless, self-contained SQL database engine that is widely used for local data storage in applications. The .NET Aspire SQLite integration provides a way to use SQLite databases within your .NET Aspire applications, and access them via the [`Microsoft.Data.Sqlite`](https://www.nuget.org/packages/Microsoft.Data.Sqlite) client.

## Hosting integration

[!INCLUDE [sqlite-hosting](includes/sqlite-hosting.md)]

## Client integration

To get started with the .NET Aspire SQLite client integration, install the [📦 CommunityToolkit.Aspire.Microsoft.Data.Sqlite](https://www.nuget.org/packages/CommunityToolkit.Aspire.Microsoft.Data.Sqlite) NuGet package in the client-consuming project, that is, the project for the application that uses the SQLite client. The SQLite client integration registers a [`SqliteConnection`](/dotnet/api/microsoft.data.sqlite.sqliteconnection) instance that you can use to interact with SQLite.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.Microsoft.Data.Sqlite
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.Microsoft.Data.Sqlite" Version="*" />
```

---

### Add Sqlite client

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the `Microsoft.Extensions.Hosting.AspireSqliteExtensions.AddSqliteConnection%2A> extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register a `SqliteConnection` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddSqliteConnection(connectionName: "sqlite");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the SQLite resource in the app host project. For more information, see [Add SQLite resource](#add-sqlite-resource).

After adding `SqliteConnection` to the builder, you can get the `SqliteConnection` instance using dependency injection. For example, to retrieve your connection object from an example service define it as a constructor parameter and ensure the `ExampleService` class is registered with the dependency injection container:

```csharp
public class ExampleService(SqliteConnection connection)
{
    // Use connection...
}
```

For more information on dependency injection, see [.NET dependency injection](/dotnet/core/extensions/dependency-injection).

### Add keyed Sqlite client

There might be situations where you want to register multiple `SqliteConnection` instances with different connection names. To register keyed Sqlite clients, call the `Microsoft.Extensions.Hosting.AspireSqliteExtensions.AddKeyedSqliteConnection` method:

```csharp
builder.AddKeyedSqliteConnection(name: "chat");
builder.AddKeyedSqliteConnection(name: "queue");
```

Then you can retrieve the `SqliteConnection` instances using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(
    [FromKeyedServices("chat")] SqliteConnection chatConnection,
    [FromKeyedServices("queue")] SqliteConnection queueConnection)
{
    // Use connections...
}
```

### Configuration

The SQLite client integration provides multiple configuration approaches and options to meet the requirements and conventions of your project.

### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling the `Microsoft.Extensions.Hosting.AspireSqliteExtensions.AddSqliteConnection` method:

```csharp
builder.AddSqliteConnection("sqlite");
```

Then the connection string will be retrieved from the `ConnectionStrings` configuration section.

```json
{
  "ConnectionStrings": {
    "sqlite": "Data Source=C:\\Database\\Location\\my-database.db"
  }
}
```

#### Use configuration providers

The SQLite client integration supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the `Microsoft.Extensions.Hosting.SqliteConnectionSettings` from the _:::no-loc text="appsettings.json":::_ or other configuration providers by using the `Aspire:Sqlite:Client` key. Example _:::no-loc text="appsettings.json":::_ that configures some of the options:

```json
{
  "Aspire": {
    "Sqlite": {
      "Client": {
        "ConnectionString": "Data Source=C:\\Database\\Location\\my-database.db",
        "DisableHealthCheck": true
      }
    }
  }
}
```
