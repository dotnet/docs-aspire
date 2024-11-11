---
title: External parameters
description: Learn how to express parameters such as secrets, connection strings, and other configuration values that might vary between environments.
ms.topic: how-to
ms.date: 11/08/2024
---

# External parameters

Environments provide context for the application to run in. Parameters express the ability to ask for an external value when running the app. Parameters can be used to provide values to the app when running locally, or to prompt for values when deploying. They can be used to model a wide range of scenarios including secrets, connection strings, and other configuration values that might vary between environments.

## Parameter values

Parameter values are read from the `Parameters` section of the app host's configuration and are used to provide values to the app while running locally. When you publish the app, if the value isn't configured you're prompted to provide it.

Consider the following example app host _:::no-loc text="Program.cs":::_ file:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add a parameter named "value"
var value = builder.AddParameter("value");

builder.AddProject<Projects.ApiService>("api")
       .WithEnvironment("EXAMPLE_VALUE", value);
```

The preceding code adds a parameter named `value` to the app host. The parameter is then passed to the `Projects.ApiService` project as an environment variable named `EXAMPLE_VALUE`.

### Configure parameter values

Adding parameters to the builder is only one aspect of the configuration. You must also provide the value for the parameter. The value can be provided in the app host configuration file, set as a user secret, or configured in any [other standard configuration](/dotnet/core/extensions/configuration). When parameter values aren't found, they're prompted for when publishing the app.

Consider the following app host configuration file _:::no-loc text="appsettings.json":::_:

```json
{
    "Parameters": {
        "value": "local-value"
    }
}
```

The preceding JSON configures a parameter in the `Parameters` section of the app host configuration. In other words, that app host is able to find the parameter as its configured. For example, you could walk up to the <xref:Aspire.Hosting.IDistributedApplicationBuilder.Configuration?displayProperty=nameWithType> and access the value using the `Parameters:value` key:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var key = $"Parameters:value";
var value = builder.Configuration[key]; // value = "local-value"
```

> [!IMPORTANT]
> However, you don't need to access this configuration value yourself in the app host. Instead, the <xref:Aspire.Hosting.ApplicationModel.ParameterResource> is used to pass the parameter value to dependent resources. Most often as an environment variable.

### Parameter representation in the manifest

.NET Aspire uses a [deployment manifest](../deployment/manifest-format.md) to represent the app's resources and their relationships. Parameters are represented in the manifest as a new primitive called `parameter.v0`:

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

Parameters can be used to model secrets. When a parameter is marked as a secret, it serves as a hint to the manifest that the value should be treated as a secret. When you publish the app, the value is prompted for and stored in a secure location. When you run the app locally, the value is read from the `Parameters` section of the app host configuration.

Consider the following example app host _:::no-loc text="Program.cs":::_ file:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add a secret parameter named "secret"
var secret = builder.AddParameter("secret", secret: true);

builder.AddProject<Projects.ApiService>("api")
       .WithEnvironment("SECRET", secret);

builder.Build().Run();
```

Now consider the following app host configuration file _:::no-loc text="appsettings.json":::_:

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

Parameters can be used to model connection strings. When you publish the app, the value is prompted for and stored in a secure location. When you run the app locally, the value is read from the `ConnectionStrings` section of the app host configuration.

> [!NOTE]
> Connection strings are used to represent a wide range of connection information including database connections, message brokers, and other services. In .NET Aspire nomenclature, the term "connection string" is used to represent any kind of connection information.

Consider the following example app host _:::no-loc text="Program.cs":::_ file:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddConnectionString("redis");

builder.AddProject<Projects.WebApplication>("api")
       .WithReference(redis);

builder.Build().Run();
```

Now consider the following app host configuration file _:::no-loc text="appsettings.json":::_:

```json
{
    "ConnectionStrings": {
        "redis": "local-connection-string"
    }
}
```

For more information pertaining to connection strings and their representation in the deployment manifest, see[Connection string and binding references](../deployment/manifest-format.md#connection-string-and-binding-references).

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

The value for the `insertionRows` parameter is read from the `Parameters` section of the app host configuration file _:::no-loc text="appsettings.json":::_:

:::code language="json" source="snippets/params/Parameters.AppHost/appsettings.json":::

The `Parameters_ApiService` project consumes the `insertionRows` parameter. Consider the _:::no-loc text="Program.cs":::_ example file:

:::code source="snippets/params/Parameters.ApiService/Program.cs":::

## See also

- [.NET Aspire manifest format for deployment tool builders](../deployment/manifest-format.md)
- [Tutorial: Connect an ASP.NET Core app to SQL Server using .NET Aspire and Entity Framework Core](../database/sql-server-integrations.md)
