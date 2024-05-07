---
title: MySQL Entity Framework Component
description: MySQL Entity Framework Component
ms.date: 02/27/2024
---

# .NET Aspire Pomelo MySQL Entity Framework Component

In this article, you learn how to use the The .NET Aspire Pomelo MySQL Entity Framework Core component. The `Aspire.Pomelo.EntityFrameworkCore.MySql` library is used to register a <xref:System.Data.Entity.DbContext?displayProperty=fullName> as a singleton in the DI container for connecting to MySQL databases. It also enables connection pooling, retries, health checks, logging and telemetry.

## Get started

You need a MySQL database and connection string for accessing the database. To get started with the The .NET Aspire Pomelo MySQL Entity Framework Core component, install the [Aspire.Pomelo.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Aspire.Pomelo.EntityFrameworkCore.MySql) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Pomelo.EntityFrameworkCore.MySql --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Pomelo.EntityFrameworkCore.MySql"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _Program.cs_ file of your component-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireEFMySqlExtensions.AddMySqlDbContext%2A> extension to register a <xref:System.Data.Entity.DbContext?displayProperty=fullName> for use via the dependency injection container.

```csharp
builder.AddMySqlDbContext<MyDbContext>("mysqldb");
```

You can then retrieve the <xref:Microsoft.EntityFrameworkCore.DbContext> instance using dependency injection. For example, to retrieve the client from a service:

```csharp
public class ExampleService(MyDbContext context)
{
    // Use context...
}
```

You might also need to configure specific options of your MySQL connection, or register a `DbContext` in other ways. In this case call the `EnrichMySqlDbContext` extension method, for example:

```csharp
var connectionString = builder.Configuration.GetConnectionString("mysqldb");

builder.Services.AddDbContextPool<MyDbContext>(
    dbContextOptionsBuilder => dbContextOptionsBuilder.UseMySql(connectionString, serverVersion));
builder.EnrichMySqlDbContext<MyDbContext>();
```

## App host usage

In your app host project, register an MySQL container and consume the connection using the following methods:

```csharp
var mysql = builder.AddMySql("mysql");
var mysqldb = mysql.AddDatabase("mysqldb");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(mysqldb);
```

## Configuration

The .NET Aspire Pomelo MySQL Entity Framework Core component provides multiple options to configure the database connection based on the requirements and conventions of your project.

### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddMySqlDatabaseDbContext<TContext>()`:

```csharp
builder.AddMySqlDatabaseDbContext<MyDbContext>("myConnection");
```

And then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "myConnection": "Server=myserver;Database=mysqldb"
  }
}
```

The `EnrichMySqlDbContext` won't make use of the `ConnectionStrings` configuration section since it expects a `DbContext` to be registered at the point it is called.

See the [MySqlConnector documentation](https://mysqlconnector.net/connection-options/) for more information on how to format this connection string.

### Use configuration providers

The .NET Aspire Pomelo MySQL Entity Framework Core component supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the `PomeloEntityFrameworkCoreMySqlSettings` from configuration by using the `Aspire:Pomelo:EntityFrameworkCore:MySql` key.

The following example shows an _appsettings.json_ that configures some of the available options:

```json
{
  "Aspire": {
    "Pomelo": {
      "EntityFrameworkCore": {
        "MySql": {
          "DisableHealthChecks": true,
          "DisableTracing": true
        }
      }
    }
  }
}
```

### Use inline delegates

You can also pass the `Action<PomeloEntityFrameworkCoreMySqlSettings> configureSettings` delegate to set up some or all the options inline, for example to disable health checks from code:

```csharp
builder.AddMySqlDbContext<MyDbContext>(
    "mysqldb1",
    static settings => settings.DisableHealthChecks  = true);
```

or

```csharp
builder.EnrichMySqlDbContext<MyDbContext>(
    static settings => settings.DisableHealthChecks  = true);
```

[!INCLUDE [component-health-checks](../includes/component-health-checks.md)]

The The .NET Aspire Pomelo MySQL Entity Framework Core component registers a basic health check that checks the database connection given a `TContext`. The health check is enabled by default and can be disabled using the `DisableHealthChecks` property in the configuration.

[!INCLUDE [component-observability-and-telemetry](../includes/component-observability-and-telemetry.md)]

### Logging

The The .NET Aspire Pomelo MySQL Entity Framework Core component uses the following log categories:

- `Microsoft.EntityFrameworkCore.Database.Command.CommandCreated`
- `Microsoft.EntityFrameworkCore.Database.Command.CommandExecuting`
- `Microsoft.EntityFrameworkCore.Database.Command.CommandExecuted`
- `Microsoft.EntityFrameworkCore.Database.Command.CommandError`

### Tracing

The The .NET Aspire Pomelo MySQL Entity Framework Core component will emit the following tracing activities using OpenTelemetry:

- OpenTelemetry.Instrumentation.EntityFrameworkCore

### Metrics

The The .NET Aspire Pomelo MySQL Entity Framework Core component currently supports the following metrics:

- Microsoft.EntityFrameworkCore

## See also

- [Entity Framework Core docs](/ef/core)
- [.NET Aspire components](../fundamentals/components-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
