---
title: Default container image name and tag changed
description: Default container image name and tag changes in Aspire 9.1 for AddDockerfile and WithDockerfile methods.
ms.date: 02/13/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/2557
---

# Default container image name and tag changed

In Aspire 9.1, the default container image name and tag assigned to the container resource when using <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.AddDockerfile*> or <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithDockerfile*> changed. This change affects how container images are named and tagged by default, improving consistency and safety.

## Version introduced

Aspire 9.1

## Previous behavior

In Aspire 9.0:

- The default container image name used was based on this format: `{ResourceName}-image-{HashOfAppHostDirectory}`
- The default container tag used was simply `latest`

## New behavior

In Aspire 9.1:

- The default container image name used is now simply the resource name lowercased.
- The default container tag used is now a hash derived from the AppHost directory combined with a timestamp of when the method was called.

## Type of breaking change

This change is a [behavioral change](../categories.md#behavioral-change).

## Reason for change

The previous behavior led to [a bug](https://github.com/dotnet/aspire/issues/7462). The resource name wasn't properly sanitized, resulting in unsafe, and unfriendly container image names. Additionally, tagging all images as `latest` made it difficult to manage and roll back deployments.

## Recommended action

Users should update any deployment tools, scripts, and processes to accommodate the new image names and tags.

> [!NOTE]
> The default image name and tag can be overridden by calling <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithImage*> and <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithImageTag*> on the `IResourceBuilder<ContainerResource>` respectively.

## Affected APIs

- <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.AddDockerfile*>
- <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithDockerfile*>
