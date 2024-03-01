---
title: External parameters
description: Learn how to express parameters such as secrets, connection strings, and other configuration values that might vary between environments.
ms.date: 03/01/2024
---

# External parameters

Environments provide context for the application to run in. Parameters express the ability to ask for an external value when running the app. Parameters can be used to provide values to the app when running locally, or to prompt for values when deploying. They can be used to model a wide range of scenarios including secrets, connection strings, and other configuration values that might vary between environments.

## Parameter values

Parameter values are read from the `Parameters` section of the app host's configuration and are used to provide values to the app while running locally. When deploying the app, the value will be asked for the parameter value.

Consider the following app host _Program.cs_ example file:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add a parameter
var value = builder.AddParameter("value");

builder.AddProject<Projects.ApiService>("api")
       .WithEnvironment("EXAMPLE_VALUE", value);
```

Now consider the following app host configuration file _appsettings.json_:

```json
{
    "Parameters": {
        "value": "local-value"
    }
}
```

Parameters are represented in the manifest as a new primitive called `parameter.v0`:

```json
{
  "resources": {
    "value": {
      "type": "parameter.v0",
      "value": "{value.inputs.value}",
      "inputs": {
        "value": {
          "type": "string"
        }
      }
    }
  }
}
```

## Secret values

Parameters can be used to model secrets. When a parameter is marked as a secret, this is a hint to the manifest that the value should be treated as a secret. When deploying, the value will be prompted for and stored in a secure location. When running locally, the value will be read from the `Parameters` section of the app host configuration.

Consider the following app host _Program.cs_ example file:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add a secret parameter
var secret = builder.AddParameter("secret", secret: true);

builder.AddProject<Projects.ApiService>("api")
       .WithEnvironment("SECRET", secret);

builder.Build().Run();
```

Now consider the following app host configuration file _appsettings.json_:

```json
{
    "Parameters": {
        "secret": "local-secret"
    }
}
```

The manifest representation is as follows:

```json
{
  "resources": {
    "value": {
      "type": "parameter.v0",
      "value": "{value.inputs.value}",
      "inputs": {
        "value": {
          "type": "string",
          "secret": true
        }
      }
    }
  }
}
```

## Connection string values

Parameters can be used to model connection strings. When deploying, the value will be prompted for and stored in a secure location. When running locally, the value will be read from the `ConnectionStrings` section of the app host configuration.

> [!NOTE]
> Connection strings are used to represent a wide range of connection information including database connections, message brokers, and other services. In .NET Aspire nomenclature, the term "connection string" is used to represent any kind of connection information.

Consider the following app host _Program.cs_ example file:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddConnectionString("redis");

builder.AddProject<Projects.WebApplication1>("api")
       .WithReference(redis);

builder.Build().Run();
```

Now consider the following app host configuration file _appsettings.json_:

```json
{
    "ConnectionStrings": {
        "redis": "local-connection-string"
    }
}
```

The manifest representation is as follows:

```json
{
  "resources": {
    "redis": {
      "type": "parameter.v0",
      "connectionString": "{redis.value}",
      "value": "{redis.inputs.value}",
      "inputs": {
        "value": {
          "type": "string",
          "secret": true
        }
      }
    }
  }
}
```

## Parameter example

To express a parameter, consider the following example code:

:::code source="snippets/params/Parameters.AppHost/Program.cs":::

The following steps are performed:

- Adds a SQL Server resource named `sql` and publishes it as a connection string.
- Adds a database named `db`.
- Adds a parameter named `insertionRows`.
- Adds a project named `api` and associates it with the `Projects.Parameters_ApiService` project resource type-parameter.
- Passes the `insertionRows` parameter to the `api` project.
- References the `db` database.

The value for the `insertionRows` parameter is read from the `Parameters` section of the app host configuration file _appsettings.json_:

:::code language="json" source="snippets/params/Parameters.AppHost/appsettings.json":::

The `Parameters_ApiService` project consumes the `insertionRows` parameter, consider the _Program.cs_ example file:

:::code source="snippets/params/Parameters.ApiService/Program.cs":::

## See also

- [.NET Aspire manifest format for deployment tool builders](../deployment/manifest-format.md)
- [Tutorial: Connect an ASP.NET Core app to SQL Server using .NET Aspire and Entity Framework Core](../database/sql-server-components.md)
