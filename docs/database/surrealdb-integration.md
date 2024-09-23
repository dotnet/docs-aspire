---
title: .NET Aspire SurrealDB integration
description: This article describes the .NET Aspire SurrealDB integration.
ms.topic: how-to
ms.date: 09/17/2024
---

# .NET Aspire SurrealDB integration

<!-- TODO : intro & get started sections -->

## App host usage

[!INCLUDE [surrealdb-app-host](includes/surrealdb-app-host.md)]

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var surrealServer = builder.AddSurrealServer("surreal");
var surrealDb = mysql.AddDatabase("mydb", "ns", "db");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(surrealDb);
```

When you want to explicitly provide a root password, you can provide it as a parameter. Consider the following alternative example:

```csharp
var password = builder.AddParameter("password", secret: true);

var surrealServer = builder.AddSurrealServer("surreal", password);
var surrealDb = mysql.AddDatabase("mydb", "ns", "db");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(surrealDb);
```

For more information, see [External parameters](../fundamentals/external-parameters.md).

<!-- TODO : configuration section -->

## See also

- [SurrealDB database](https://surrealdb.com/)
- [.NET SDK documentation for SurrealDB](https://surrealdb.com/docs/sdk/dotnet)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
