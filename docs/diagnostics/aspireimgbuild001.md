---
title: Compiler Error ASPIREIMGBUILD001
description: Learn more about compiler Error ASPIREIMGBUILD001. Container image build APIs are for evaluation purposes only and are subject to change or removal in future updates.
ms.date: 10/27/2025
f1_keywords:
  - "ASPIREIMGBUILD001"
helpviewer_keywords:
  - "ASPIREIMGBUILD001"
ai-usage: ai-generated
---

# Compiler Error ASPIREIMGBUILD001

**Version introduced:** 9.2

> Container image build APIs are for evaluation purposes only and are subject to change or removal in future updates. Suppress this diagnostic to proceed.

Aspire introduced container image build APIs starting in version 9.2. These APIs provide functionality for building container images as part of the deployment pipeline. The container image build APIs enable you to configure build options, specify target platforms, select image formats, and integrate with container runtimes like Docker and Podman.

Container image build APIs are considered experimental and are expected to change in future updates.

## APIs affected

This diagnostic applies to the following container image build APIs:

- `IResourceContainerImageBuilder` - Interface for building container images
- `ContainerBuildOptions` - Options for configuring container builds
- `ContainerImageFormat` - Enumeration for specifying image format
- `ContainerTargetPlatform` - Type for specifying target platform
- `ContainerTargetPlatformExtensions` - Extension methods for platform configuration
- Docker and Podman container runtime implementations
- Related extension methods and implementations

## To correct this error

Suppress the error with one of the following methods:

- Set the severity of the rule in the _.editorconfig_ file.

  ```ini
  [*.{cs,vb}]
  dotnet_diagnostic.ASPIREIMGBUILD001.severity = none
  ```

  For more information about editor config files, see [Configuration files for code analysis rules](/dotnet/fundamentals/code-analysis/configuration-files).

- Add the following `PropertyGroup` to your project file:

  ```xml
  <PropertyGroup>
      <NoWarn>$(NoWarn);ASPIREIMGBUILD001</NoWarn>
  </PropertyGroup>
  ```

- Suppress in code with the `#pragma warning disable ASPIREIMGBUILD001` directive.
