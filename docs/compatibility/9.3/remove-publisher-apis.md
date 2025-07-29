---
title: "Breaking change - Removal of DockerComposePublisher, KubernetesPublisher, and AzurePublisher"
description: "Learn about the breaking change in .NET Aspire 9.3 where publisher APIs were removed in favor of new resource types."
ms.date: 5/12/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/3373
---

# Removal of DockerComposePublisher, KubernetesPublisher, and AzurePublisher

In .NET Aspire 9.3, the `AddDockerComposePublisher`, `AddKubernetesPublisher`, and `AddAzurePublisher` APIs were removed. These APIs are now replaced with new resource types that provide a more composable experience.

## Version introduced

.NET Aspire 9.3

## Previous behavior

In .NET Aspire 9.2, the publisher API was introduced in preview, allowing the use of the following publishers:

- DockerCompose
- Kubernetes
- Azure

These publishers were added using the following methods:

- `Aspire.Hosting.DockerComposePublisherExtensions.AddDockerComposePublisher*`
- `Aspire.Hosting.KubernetesPublisherExtensions.AddKubernetesPublisher*`
- `Aspire.Hosting.AzurePublisherExtensions.AddAzurePublisher*`

Multiple publishers could be added, and the `aspire publish` CLI command allowed users to select one for publishing.

## New behavior

In .NET Aspire 9.3, the publishers are now replaced with new resource types:

<!-- TODO: Add xrefs when available. -->

- `DockerComposeEnvironmentResource`
- `KubernetesEnvironmentResource`
- `AzureEnvironmentResource` (Automatically added when you use any Azure resource)

These resources include a `PublisherCallbackAnnotation` that defines their publishing behavior. The default publisher now automatically processes all resources with this annotation to generate assets. The `aspire publish` command no longer requires selecting a publisher; it uses the default publisher to handle all annotated resources.

Example:

```csharp
builder.AddDockerComposeEnvironment("docker-compose");

builder.AddKubernetesEnvironment("kubernetes");

builder.AddAzureEnvironment("azure");
```

## Type of breaking change

This is a [binary incompatible](../categories.md#binary-compatibility), [source incompatible](../categories.md#source-compatibility), and [behavioral change](../categories.md#behavioral-change).

## Reason for change

The change simplifies the publishing process by consolidating functionality into resource types with a unified publishing mechanism. For more information, see the [GitHub issue](https://github.com/dotnet/aspire/issues/9089).

## Recommended action

Update your code to use the new resource APIs:

- Replace `AddDockerComposePublisher` with `AddDockerComposeEnvironment("...")`.
- Replace `AddKubernetesPublisher` with `AddKubernetesEnvironment("...")`.
- Replace `AddAzurePublisher` with `AddAzureEnvironment("...")`.

Example:

```csharp
var dockerCompose = builder.AddDockerComposeEnvironment("docker-compose");
var kubernetes = builder.AddKubernetesEnvironment("kubernetes");
var azure = builder.AddAzureEnvironment("azure");
```

## Affected APIs

- Aspire.Hosting.DockerComposePublisherExtensions.AddDockerComposePublisher*
- Aspire.Hosting.KubernetesPublisherExtensions.AddKubernetesPublisher*
- Aspire.Hosting.AzurePublisherExtensions.AddAzurePublisher*
