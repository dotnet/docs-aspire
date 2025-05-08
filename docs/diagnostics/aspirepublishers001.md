---
title: Compiler Error ASPIREPUBLISHERS001
description: Learn more about compiler Error ASPIREPUBLISHERS001. Publishers are for evaluation purposes only and are subject to change or removal in future updates.
ms.date: 04/21/2025
f1_keywords:
  - "ASPIREPUBLISHERS001"
helpviewer_keywords:
  - "ASPIREPUBLISHERS001"
---

# Compiler Error ASPIREPUBLISHERS001

**Version introduced:** 9.2

> Publishers are for evaluation purposes only and are subject to change or removal in future updates. Suppress this diagnostic to proceed.

.NET Aspire introduced the concept of _Publishers_ starting in version 9.2. Publishers play a pivotal role in the deployment process, enabling the transformation of your distributed app into deployable assets. This alleviates the intermediate step of producing the publishing [manifest](../deployment/manifest-format.md) for tools to act on, instead empowering the developer to express their intent directly in C#.

Publishers are considered experimental and are expected to change in the future.

## To correct this Error

Suppress the Error with either of the following methods:

- Set the severity of the rule in the _.editorconfig_ file.

  ```ini
  [*.{cs,vb}]
  dotnet_diagnostic.ASPIREPUBLISHERS001.severity = none
  ```

  For more information about editor config files, see [Configuration files for code analysis rules](/dotnet/fundamentals/code-analysis/configuration-files).

- Add the following `PropertyGroup` to your project file:

  ```xml
  <PropertyGroup>
      <NoWarn>$(NoWarn);ASPIREPUBLISHERS001</NoWarn>
  </PropertyGroup>
  ```

- Suppress in code with the `#pragma warning disable ASPIREPUBLISHERS001` directive.
