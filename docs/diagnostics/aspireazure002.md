---
title: Compiler Error ASPIREAZURE002
description: Learn more about compiler Error ASPIREAZURE002. Azure Container App Jobs are for evaluation purposes only and are subject to change or removal in future updates.
ms.date: 09/22/2025
f1_keywords:
  - "ASPIREAZURE002"
helpviewer_keywords:
  - "ASPIREAZURE002"
---

# Compiler Error ASPIREAZURE002

**Version introduced:** 9.5

> Azure Container App Jobs are for evaluation purposes only and are subject to change or removal in future updates. Suppress this diagnostic to proceed.

The .NET Aspire Azure hosting integration now ships with support for Azure Container App Jobs. If you're using any of the `PublishAsAzureContainerAppJob` APIs, you might see a compiler error/warning indicating that the API is experimental. This behavior is expected, as the API is still in preview and the shape of this API is expected to change in the future.

## Example

The following code generates `ASPIREAZURE002`:

```csharp
builder.AddProject<Projects.DataProcessor>("data-processor")
    .PublishAsAzureContainerAppJob((_, job) =>
    {
        job.Configuration.TriggerType = ContainerAppJobTriggerType.Schedule;
        job.Configuration.ScheduleTriggerConfig.CronExpression = "0 0 * * *";
    });
```

## To correct this error

Suppress the error with either of the following methods:

- Set the severity of the rule in the _.editorconfig_ file.

  ```ini
  [*.{cs,vb}]
  dotnet_diagnostic.ASPIREAZURE002.severity = none
  ```

  For more information about editor config files, see [Configuration files for code analysis rules](/dotnet/fundamentals/code-analysis/configuration-files).

- Add the following `PropertyGroup` to your project file:

  ```xml
  <PropertyGroup>
      <NoWarn>$(NoWarn);ASPIREAZURE002</NoWarn>
  </PropertyGroup>
  ```

- Suppress in code with the `#pragma warning disable ASPIREAZURE002` directive.
