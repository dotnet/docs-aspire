---
title: Compiler Warning ASPIRE004
description: Learn more about compiler Warning ASPIRE004. Project is referenced by an Aspire Host project, but it is not an executable.
ms.date: 05/08/2025
f1_keywords:
  - "ASPIRE004"
helpviewer_keywords:
  - "ASPIRE004"
---

# Compiler Warning ASPIRE004

**Version introduced:** 8.0.0

> 'Project' is referenced by an Aspire Host project, but it is not an executable. Did you mean to set IsAspireProjectResource="false"?

The project being referenced byt the .NET Aspire AppHost isn't an executable, but is being treated like one for the purposes of orchestration.

## To correct this warning

Either change the build type of the project to an executable, or add the `IsAspireProjectResource="false"` setting to the project reference in your .NET Aspire AppHost project file, as demonstrated in the following snippet:

```xml
<ItemGroup>
  <ProjectReference Include="..\OtherProjects\Contracts.csproj" IsAspireProjectResource="false" />
</ItemGroup>
```

## Suppress the warning

Suppress the warning with either of the following methods:

- Set the severity of the rule in the _.editorconfig_ file.

  ```ini
  [*.{cs,vb}]
  dotnet_diagnostic.ASPIRE004.severity = none
  ```

  For more information about editor config files, see [Configuration files for code analysis rules](/dotnet/fundamentals/code-analysis/configuration-files).

- Add the following `PropertyGroup` to your project file:

  ```xml
  <PropertyGroup>
      <NoWarn>$(NoWarn);ASPIRE004</NoWarn>
  </PropertyGroup>
  ```
