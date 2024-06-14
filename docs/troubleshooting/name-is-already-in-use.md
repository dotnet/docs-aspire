---
title: The specified name is already in use
description: Learn how to troubleshoot the error "The specified name is already in use" when deploying to Azure.
ms.date: 06/03/2024
---

# The specified name is already in use

When deploying to Azure initial deployments may fail with an error similar to the following:

> "The specified name is already in use"

This article describes several techniques to avoid this problem.

## Symptoms

When deploying a .NET Aspire project to Azure, the resources in the [app model](../fundamentals/app-host-overview.md#define-the-app-model) are transformed into Azure resources. Some Azure resources have globally scoped names, such as Azure App Configuration, where all instances are in the `[name].azconfig.io` global namespace.

The value of `[name]` is derived from the .NET Aspire resource name, along with random characters based on the resource group name. However, the generated string may exceed the allowable length for the resource name in App Configuration. As a result, some characters are truncated to ensure compliance.

When a conflict occurs in the global namespace, the resource fails to deploy because the combination of `[name]+[truncated hash]` isn't unique enough.

## Possible solutions

One workaround is to avoid using common names like `appconfig` or `storage` for resources. Instead, choose a more meaningful and specific name. This reduces the potential for conflict, but does not completely eliminate it. In such cases, you can use callback methods to set a specific name and avoid using the computed string altogether.

Consider the following example:

```csharp
var appConfig = builder.AddAzureAppConfiguration(
    "appConfig",
    (resource, construct, store) =>
{
    store.AssignProperty(p => p.Name, "'noncalculatedname'");
});
```
