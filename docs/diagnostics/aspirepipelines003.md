---
title: Compiler Error ASPIREPIPELINES003
description: Learn more about compiler Error ASPIREPIPELINES003. Container image build APIs are for evaluation purposes only and are subject to change or removal in future updates.
ms.date: 10/28/2025
f1_keywords:
  - "ASPIREPIPELINES003"
helpviewer_keywords:
  - "ASPIREPIPELINES003"
ai-usage: ai-generated
---

# Compiler Error ASPIREPIPELINES003

**Version introduced:** 13.0

> Container image build APIs are for evaluation purposes only and are subject to change or removal in future updates. Suppress this diagnostic to proceed.

Aspire introduced container image build APIs as part of the pipeline infrastructure starting in version 13.0. These APIs provide functionality for building container images as part of the deployment pipeline. The container image build APIs enable you to configure build options, specify target platforms, select image formats, and integrate with container runtimes like Docker and Podman.

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
