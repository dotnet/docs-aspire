---
title: External parameters
description: Learn how to express parameters such as secrets, connection strings, and other configuration values that might vary between environments.
ms.topic: how-to
ms.date: 12/06/2024
ms.custom: sfi-ropc-nochange
---

# External parameters

Environments provide context for the application to run in. Parameters express the ability to ask for an external value when running the app. Parameters can be used to provide values to the app when running locally, or to prompt for values when deploying. They can be used to model a wide range of scenarios including secrets, connection strings, and other configuration values that might vary between environments.

## Parameter values

Parameter values are read from the `Parameters` section of the AppHost's configuration and are used to provide values to the app while running locally. When you run or publish the app, if the value isn't configured you're prompted to provide it.

Consider the following example AppHost _:::no-loc text="Program.cs":::_ file:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add a parameter named "example-parameter-name"
var parameter = builder.AddParameter("example-parameter-name");

builder.AddProject<Projects.ApiService>("api")
       .WithEnvironment("ENVIRONMENT_VARIABLE_NAME", parameter);
```

The preceding code adds a parameter named `example-parameter-name` to the AppHost. The parameter is then passed to the `Projects.ApiService` project as an environment variable named `ENVIRONMENT_VARIABLE_NAME`.

### Configure parameter values

Adding parameters to the builder is only one aspect of the configuration. You must also provide the value for the parameter. The value can be provided in the AppHost configuration file, set as a user secret, or configured in any [other standard configuration](/dotnet/core/extensions/configuration). When parameter values aren't found, they're prompted for when you run or publish the app.

Consider the following AppHost configuration file _:::no-loc text="appsettings.json":::_:

```json
{
    "Parameters": {
        "example-parameter-name": "local-value"
    }
}
```

The preceding JSON configures a parameter in the `Parameters` section of the AppHost configuration. In other words, that AppHost is able to find the parameter as it's configured. For example, you could walk up to the <xref:Aspire.Hosting.IDistributedApplicationBuilder.Configuration?displayProperty=nameWithType> and access the value using the `Parameters:example-parameter-name` key:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var key = $"Parameters:example-parameter-name";
var value = builder.Configuration[key]; // value = "local-value"
```

> [!IMPORTANT]
> However, you don't need to access this configuration value yourself in the AppHost. Instead, the <xref:Aspire.Hosting.ApplicationModel.ParameterResource> is used to pass the parameter value to dependent resources. Most often as an environment variable.

### Prompt for parameter values in the dashboard

If your code adds parameters but doesn't set them, you'll see a prompt to configure their values in the .NET Aspire dashboard. The **Unresolved parameters** message appears, and you can select **Enter values** to resolve the problem:

:::image type="content" source="./media/dashboard-unresolved-parameters-message.png" lightbox="./media/dashboard-unresolved-parameters-message.png" alt-text="Screenshot of the .NET Aspire dashboard warning that appears when there are unresolved parameters.":::

When you select **Enter values**, .NET Aspire displays a form that you can use to configure values for each of the missing parameters.

You can also control how the dashboard displays these parameters, by using these methods:

- `WithDescription`: Use this method to provide a text description that helps users understand the purpose of the parameter. To provide a formatted description in [Markdown](https://www.markdownguide.org/basic-syntax/), use the `enableMarkdown: true` parameter.
- `WithCustomInput`: Use this method to provide a callback method that customizes the parameter dialog. For example, in this callback you can customize the default value, input type, label, and placeholder text.

This code shows how to set a description and use the callback:

:::code language="csharp" source="snippets/unresolvedparameters/AppHost.cs" id="unresolvedparameters":::

The code renders this control in the dashboard:

:::image type="content" source="./media/customized-parameter-ui.png" lightbox="./media/customized-parameter-ui.png" alt-text="Screenshot of the .NET Aspire dashboard parameter completion dialog with customizations.":::

> [!NOTE]
> The dashboard parameter dialog includes a **Save to user secret** checkbox. Select this option to store sensitive values in your AppHost's user secrets for extra protection. For more information about secret parameter values, see [Secret values](#secret-values).

### Parameter representation in the manifest

.NET Aspire uses a [deployment manifest](../deployment/manifest-format.md) to represent the app's resources and their relationships. Parameters are represented in the manifest as a new primitive called `parameter.v0`:

```json
{
  "resources": {
    "example-parameter-name": {
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

Parameters can be used to model secrets. When a parameter is marked as a secret, it serves as a hint to the manifest that the value should be treated as a secret. When you publish the app, the value is prompted for and stored in a secure location. When you run the app locally, the value is read from the `Parameters` section of the AppHost configuration.

Consider the following example AppHost _:::no-loc text="Program.cs":::_ file:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add a secret parameter named "secret"
var secret = builder.AddParameter("secret", secret: true);

builder.AddProject<Projects.ApiService>("api")
       .WithEnvironment("SECRET", secret);

builder.Build().Run();
```

Now consider the following AppHost configuration file _:::no-loc text="appsettings.json":::_:

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

Parameters can be used to model connection strings. When you publish the app, the value is prompted for and stored in a secure location. When you run the app locally, the value is read from the `ConnectionStrings` section of the AppHost configuration.

[!INCLUDE [connection-strings-alert](../includes/connection-strings-alert.md)]

Consider the following example AppHost _:::no-loc text="Program.cs":::_ file:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddConnectionString("redis");

builder.AddProject<Projects.WebApplication>("api")
       .WithReference(redis)
       .WaitFor(redis);

builder.Build().Run();
```

> [!NOTE]
> Using <xref:Aspire.Hosting.ResourceBuilderExtensions.WaitFor*> with a connection string will implicitly wait for the resource that the connection string connects to.

Now consider the following AppHost configuration file _:::no-loc text="appsettings.json":::_:

```json
{
    "ConnectionStrings": {
        "redis": "local-connection-string"
    }
}
```

For more information pertaining to connection strings and their representation in the deployment manifest, see [Connection string and binding references](../deployment/manifest-format.md#connection-string-and-binding-references).

### Build connection strings with reference expressions

If you want to construct a connection string from parameters and ensure that it's handled correctly in both development and production, use <xref:Aspire.Hosting.ConnectionStringBuilderExtensions.AddConnectionString*> with a <xref:Aspire.Hosting.ApplicationModel.ReferenceExpression>.

For example, if you have a secret parameter that stores a small part of a connection string, use this code to insert it:

:::code language="csharp" source="snippets/referenceexpressions/AspireReferenceExpressions.AppHost/Program.cs" id="secretkey":::

You can also use reference expressions to append text to connection strings created by .NET Aspire resources. For example, when you add a PostgreSQL resource to your .NET Aspire solution, the database server runs in a container and a connection string is formulated for it. In the following code, the extra property `Include Error Details` is appended to that connection string before it's passed to consuming projects:

:::code language="csharp" source="snippets/referenceexpressions/AspireReferenceExpressions.AppHost/Program.cs" id="postgresappend":::

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

The value for the `insertionRows` parameter is read from the `Parameters` section of the AppHost configuration file _:::no-loc text="appsettings.json":::_:

:::code language="json" source="snippets/params/Parameters.AppHost/appsettings.json":::

The `Parameters_ApiService` project consumes the `insertionRows` parameter. Consider the _:::no-loc text="Program.cs":::_ example file:

:::code source="snippets/params/Parameters.ApiService/Program.cs":::

## See also

- [.NET Aspire manifest format for deployment tool builders](../deployment/manifest-format.md)
- [Tutorial: Connect an ASP.NET Core app to SQL Server using .NET Aspire and Entity Framework Core](../database/sql-server-integrations.md)
