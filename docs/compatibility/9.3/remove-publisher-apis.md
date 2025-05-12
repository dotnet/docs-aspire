---
title: "Breaking change - Removal of DockerComposePublisher, KubernetesPublisher and AzurePublisher"
description: "Learn about the breaking change in .NET Aspire 9.3 where publisher APIs were removed in favor of new resource types."
ms.date: 5/12/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/3373
---

# Removal of DockerComposePublisher, KubernetesPublisher and AzurePublisher

In .NET Aspire 9.3, the `AddDockerComposePublisher`, `AddKubernetesPublisher`, and `AddAzurePublisher` APIs were removed. These APIs have been replaced with new resource types that provide a more streamlined and flexible publishing experience.

## Version introduced

.NET Aspire 9.3

## Previous behavior

In .NET Aspire 9.2, the publisher API was introduced in preview, allowing the use of the following publishers:

- DockerCompose
- Kubernetes
- Azure

These publishers were added using the following methods:

- <xref:Aspire.Hosting.DockerComposePublisherExtensions.AddDockerComposePublisher*>
- <xref:Aspire.Hosting.KubernetesPublisherExtensions.AddKubernetesPublisher*>
- <xref:Aspire.Hosting.AzurePublisherExtensions.AddAzurePublisher*>

Multiple publishers could be added, and the `aspire publish` CLI command allowed users to select one for publishing.

## New behavior

In .NET Aspire 9.3, the publishers have been replaced with new resource types:

<!-- TODO: Add xrefs when available. -->

- `DockerEnvironmentResource`
- `KubernetesEnvironmentResource`
- `AzureEnvironmentResource`

These resources include a `PublisherCallbackAnnotation` that defines their publishing behavior. The default publisher now automatically processes all resources with this annotation to generate assets. The `aspire publish` command no longer requires selecting a publisher; it uses the default publisher to handle all annotated resources.

Example:

```csharp
builder.AddDockerComposeEnvironment(publisher =>
{
    // Configure the Docker environment publisher
});

builder.AddKubernetesEnvironment(publisher =>
{
    // Configure the Kubernetes environment publisher
});

builder.AddAzureEnvironment(publisher =>
{
    // Configure the Azure environment publisher
});
```

## Type of breaking change

This is a [binary incompatible](../categories.md#binary-compatibility), [source incompatible](../categories.md#source-compatibility), and [behavioral change](../categories.md#behavioral-change).

## Reason for change

The change simplifies the publishing process by consolidating functionality into resource types with a unified publishing mechanism. For more details, see the [GitHub issue](https://github.com/dotnet/aspire/issues/9089).

## Recommended action

Update your code to use the new resource APIs:

- Replace `AddDockerComposePublisher` with `AddDockerComposeEnvironment(...)`.
- Replace `AddKubernetesPublisher` with `AddKubernetesEnvironment(...)`.
- Replace `AddAzurePublisher` with `AddAzureEnvironment(...)` (this is implicit with any Azure resource).

Example:

```csharp
builder.AddDockerComposeEnvironment(...);
builder.AddKubernetesEnvironment(...);
builder.AddAzureEnvironment(...);
```

## Affected APIs

- `IDistributedApplicationBuilder.AddDockerComposePublisher`
- `IDistributedApplicationBuilder.AddKubernetesPublisher`
- `IDistributedApplicationBuilder.AddAzurePublisher`
