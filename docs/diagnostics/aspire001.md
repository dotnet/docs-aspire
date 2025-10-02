---
title: Compiler Warning ASPIRE001
description: Learn more about compiler Warning ASPIRE001. The code language isn't fully supported by Aspire - some code generation targets will not run, so will require manual authoring.
ms.date: 05/08/2025
f1_keywords:
  - "ASPIRE001"
helpviewer_keywords:
  - "ASPIRE001"
---

# Compiler Warning ASPIRE001

**Version introduced:** 8.0.0

> The 'CODELANGUAGE' language isn't fully supported by Aspire - some code generation targets will not run, so will require manual authoring.

This diagnostic warning is reported when using a code language other than C#.

## To correct this warning

In your Aspire project, use C#.

## Suppress the warning

Suppress the warning with either of the following methods:

- Set the severity of the rule in the _.editorconfig_ file.

  ```ini
  [*.{cs,vb}]
  dotnet_diagnostic.ASPIRE001.severity = none
  ```

  For more information about editor config files, see [Configuration files for code analysis rules](/dotnet/fundamentals/code-analysis/configuration-files).

- Add the following `PropertyGroup` to your project file:

  ```xml
  <PropertyGroup>
      <NoWarn>$(NoWarn);ASPIRE001</NoWarn>
  </PropertyGroup>
  ```
