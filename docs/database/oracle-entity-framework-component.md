---
title: Oracle Entity Framework Component
description: Oracle Entity Framework Component
ms.date: 02/26/2024
---

# .NET Aspire Oracle Entity Framework Component

In this article, you learn how to use the The .NET Aspire Oracle Entity Framework Core component. The `Aspire.Oracle.EntityFrameworkCore` library is used to register a <xref:System.Data.Entity.DbContext?displayProperty=fullName> as a singleton in the DI container for connecting to Oracle databases. It also enables connection pooling, retries, health checks, logging and telemetry.

## Get started

You need an Oracle database and connection string for accessing the database. To get started with the The .NET Aspire Oracle Entity Framework Core component, install the [Aspire.Oracle.EntityFrameworkCore](https://www.nuget.org/packages/Aspire.Oracle.EntityFrameworkCore) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Oracle.EntityFrameworkCore --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Oracle.EntityFrameworkCore"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _Program.cs_ file of your component-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireOracleEFCoreExtensions.AddOracleDatabaseDbContext%2A> extension to register a <xref:System.Data.Entity.DbContext?displayProperty=fullName> for use via the dependency injection container.

```csharp
builder.AddOracleDatabaseDbContext<MyDbContext>("oracledb");
```

You can then retrieve the <xref:Microsoft.EntityFrameworkCore.DbContext> instance using dependency injection. For example, to retrieve the client from a service:

```csharp
public class ExampleService(MyDbContext context)
{
    // Use context...
}
```

You might also need to configure specific options of Oracle database, or register a `DbContext` in other ways. In this case call the `EnrichOracleDatabaseDbContext` extension method, for example:

```csharp
var connectionString = builder.Configuration.GetConnectionString("oracledb");

builder.Services.AddDbContextPool<MyDbContext>(
    dbContextOptionsBuilder => dbContextOptionsBuilder.UseOracle(connectionString));

builder.EnrichOracleDatabaseDbContext<MyDbContext>();
```

## App host usage

In your app host project, register an Oracle container and consume the connection using the following methods:

```csharp
var oracle = builder.AddOracleDatabase("oracle");
var oracledb = oracle.AddDatabase("oracledb");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(oracledb);
```

## Configuration

The .NET Aspire Oracle Entity Framework Core component provides multiple options to configure the database connection based on the requirements and conventions of your project.

### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddOracleDatabaseDbContext<TContext>()`:

```csharp
builder.AddOracleDatabaseDbContext<MyDbContext>("myConnection");
```

And then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "myConnection": "Data Source=TORCL;User Id=myUsername;Password=myPassword;"
  }
}
```

The `EnrichOracleDatabaseDbContext` won't make use of the `ConnectionStrings` configuration section since it expects a `DbContext` to be registered at the point it is called.

See the [ODP.NET documentation](https://www.oracle.com/database/technologies/appdev/dotnet/odp.html) for more information on how to format this connection string.

### Use configuration providers

The .NET Aspire Oracle Entity Framework Core component supports [Microsoft.Extensions.Configuration](/dotnet/api/microsoft.extensions.configuration). It loads the `OracleEntityFrameworkCoreSettings` from configuration by using the `Aspire:Oracle:EntityFrameworkCore` key.

The following example shows an _appsettings.json_ that configures some of the available options:

```json
{
  "Aspire": {
    "Oracle": {
      "EntityFrameworkCore": {
        "DisableHealthChecks": true,
        "DisableTracing": true,
        "DisableMetrics": false,
        "DisableRetry": false,
        "Timeout": 30
      }
    }
  }
}
```

> [!TIP]
> The `Timeout` property is in seconds. When set as shown in the preceding example, the timeout is 30 seconds.

### Use inline delegates

You can also pass the `Action<OracleEntityFrameworkCoreSettings> configureSettings` delegate to set up some or all the options inline, for example to disable health checks from code:

```csharp
builder.AddOracleDatabaseDbContext<MyDbContext>(
    "oracle",
    static settings => settings.DisableHealthChecks  = true);
```

or

```csharp
builder.EnrichOracleDatabaseDbContext<MyDbContext>(
    static settings => settings.DisableHealthChecks  = true);
```

[!INCLUDE [component-health-checks](../includes/component-health-checks.md)]

The The .NET Aspire Oracle Entity Framework Core component registers a basic health check that checks the database connection given a `TContext`. The health check is enabled by default and can be disabled using the `DisableHealthChecks` property in the configuration.

[!INCLUDE [component-observability-and-telemetry](../includes/component-observability-and-telemetry.md)]

### Logging

The The .NET Aspire Oracle Entity Framework Core component uses the following log categories:

- `Microsoft.EntityFrameworkCore.Database.Command.CommandCreated`
- `Microsoft.EntityFrameworkCore.Database.Command.CommandExecuting`
- `Microsoft.EntityFrameworkCore.Database.Command.CommandExecuted`
- `Microsoft.EntityFrameworkCore.Database.Command.CommandError`

### Tracing

The The .NET Aspire Oracle Entity Framework Core component will emit the following tracing activities using OpenTelemetry:

- OpenTelemetry.Instrumentation.EntityFrameworkCore

### Metrics

The The .NET Aspire Oracle Entity Framework Core component currently supports the following metrics:

- Microsoft.EntityFrameworkCore

## See also

- [Entity Framework Core docs](/ef/core)
- [.NET Aspire components](../fundamentals/components-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
