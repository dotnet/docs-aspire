---
title: Compiler Error ASPIREPIPELINES003
description: Learn more about compiler Error ASPIREPIPELINES003. Container image building APIs are for evaluation purposes only and are subject to change or removal in future updates.
ms.date: 10/27/2025
f1_keywords:
  - "ASPIREPIPELINES003"
helpviewer_keywords:
  - "ASPIREPIPELINES003"
---

# Compiler Error ASPIREPIPELINES003

**Version introduced:** 9.2

> Container image building APIs are for evaluation purposes only and are subject to change or removal in future updates. Suppress this diagnostic to proceed.

Aspire introduced container image building APIs starting in version 9.2. These APIs provide functionality for building container images that represent resources in your distributed application, including support for different container runtimes (Docker and Podman) and image formats.

The following types are marked with this diagnostic:

- `IResourceContainerImageBuilder` - Interface for building container images
- `ContainerBuildOptions` - Options for building container images
- `ContainerImageFormat` - Enum specifying container image format (Docker or OCI)
- `ContainerTargetPlatform` - Enum specifying target platform (Linux AMD64, ARM64, Windows, etc.)
- Container runtime implementations:
  - `ContainerRuntimeBase`
  - `DockerContainerRuntime`
  - `PodmanContainerRuntime`
  - `IContainerRuntime`

These APIs are considered experimental and are expected to change in future releases.

## To correct this Error

Suppress the Error with either of the following methods:

- Set the severity of the rule in the _.editorconfig_ file.

  ```ini
  [*.{cs,vb}]
  dotnet_diagnostic.ASPIREPIPELINES003.severity = none
  ```

  For more information about editor config files, see [Configuration files for code analysis rules](/dotnet/fundamentals/code-analysis/configuration-files).

- Add the following `PropertyGroup` to your project file:

  ```xml
  <PropertyGroup>
      <NoWarn>$(NoWarn);ASPIREPIPELINES003</NoWarn>
  </PropertyGroup>
  ```

- Suppress in code with the `#pragma warning disable ASPIREPIPELINES003` directive.
