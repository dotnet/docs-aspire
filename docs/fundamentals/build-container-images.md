---
title: Build container images
description: Learn how to build container images from your .NET Aspire resources.
ms.date: 07/23/2025
ai-usage: ai-assisted
---

# Build container images

.NET Aspire provides powerful APIs for building container images from your resources during publishing and deployment operations. This article covers the key components that enable programmatic container image creation and progress reporting.

## Overview

When you publish or deploy Aspire applications, the platform automatically builds container images for your services. This process involves two main components:

- **`IResourceContainerImageBuilder`**: The service that turns resource definitions into runnable container images.
- **`PublishingActivityProgressReporter`**: The API that provides structured progress reporting during long-running operations.

These APIs give you fine-grained control over the image building process and provide real-time feedback to users during lengthy build operations.

## When to use these APIs

Consider using the container image building and progress reporting APIs in these scenarios:

- **Custom deployment targets**: When you need to deploy to platforms that require specific image formats or build configurations.
- **Complex build pipelines**: When your publishing process involves multiple steps that users need visibility into.
- **Enterprise scenarios**: When you need custom progress reporting for integration with CI/CD systems or dashboards.
- **Custom resource types**: When implementing custom resources that need to participate in the publishing and deployment process.

For most standard Aspire applications, the built-in publishing process handles container image building automatically without requiring these APIs.

## `IResourceContainerImageBuilder` API

The `IResourceContainerImageBuilder` is the core service in the <xref:Aspire.Hosting.Publishing> layer that converts resource definitions into container images. It analyzes each resource in your distributed application model and determines whether to:

- Reuse an existing image.
- Build from a .NET project using `dotnet publish /t:PublishContainer`.
- Build from a Dockerfile using the local container runtime.

### ContainerBuildOptions

Aspire 9.4 introduces the `ContainerBuildOptions` class, which provides strongly typed configuration for container builds. This class allows you to specify:

- **Image format**: Docker or OCI format.
- **Target platform**: Linux x64, Windows, ARM64, etc.
- **Output path**: Where to save the built images.
- **MSBuild properties**: Additional properties to pass to the build.
- **Build arguments**: Arguments for Dockerfile builds.

### Container runtime health checks

The builder now performs container runtime health checks (Docker/Podman) only when at least one resource requires a Dockerfile build. This change eliminates false-positive errors in projects that publish directly from .NET assemblies. If the container runtime is required but unhealthy, the builder throws an explicit `InvalidOperationException` to surface the problem early.

## `PublishingActivityProgressReporter` API

The `PublishingActivityProgressReporter` API enables structured progress reporting during `aspire publish` and `aspire deploy` commands. This reduces uncertainty during long-running operations and surfaces failures early.

### Key concepts

The progress reporter uses a hierarchical model:

- **Steps**: Top-level phases such as "Build images" or "Deploy workloads".
- **Tasks**: Discrete units of work nested under a step.
- **Completion states**: `Completed`, `Warning`, or `Error` (rendered in the CLI with ✅, ⚠️, or ❌).

### API structure

The reporter API guarantees ordered emissions, enforces completion, and streams updates to the console and dashboard in real time:

- **Acquisition**: Retrieved from `PublishingContext.ActivityReporter` or `DeployingContext.ActivityReporter`.
- **Step creation**: `CreateStepAsync(title, ct)` returns an `IPublishingActivityStep`.
- **Task creation**: `IPublishingActivityStep.CreateTaskAsync(title, ct)` returns an `IPublishingActivityTask`.
- **State transitions**: `SucceedAsync`, `WarnAsync`, `FailAsync` methods accept a summary message.
- **Completion**: `CompletePublishAsync(message, state, isDeploy, ct)` marks the entire operation.

## Building images programmatically

You can use the `IResourceContainerImageBuilder` service to build container images programmatically during publishing. The following example shows how to build images for all project resources in your distributed application:

:::code language="csharp" source="snippets/build-container-images/ImageBuildingExample.cs":::

This approach is useful when you need custom build logic or want to control exactly how and when container images are created during the publishing process.

## Progress reporting

The `PublishingActivityProgressReporter` provides structured feedback during long-running operations. Here's how to implement comprehensive progress reporting with error handling:

:::code language="csharp" source="snippets/build-container-images/ProgressReportingExample.cs":::

The progress reporter ensures users get real-time feedback about the status of their publishing or deployment operations.

## Example: Complete AppHost integration

The following example demonstrates a complete AppHost project that integrates container image building and progress reporting through a custom resource:

**AppHost.cs:**

:::code language="csharp" source="snippets/build-container-images/AppHost.cs":::

**Custom resource implementation:**

:::code language="csharp" source="snippets/build-container-images/ComputeEnvironmentResource.cs":::

This example shows how to:

- Create a realistic AppHost with multiple services and dependencies.
- Implement a custom resource that builds container images for all project resources.
- Provide detailed progress reporting throughout the publishing process.
- Handle both successful operations and error scenarios.
- Generate deployment artifacts alongside container images.

## Best practices

When using these APIs, follow these guidelines:

### Image building

- Always specify explicit `ContainerBuildOptions` for production scenarios.
- Consider target platform requirements when building for deployment.
- Use OCI format for maximum compatibility with container registries.
- Handle `InvalidOperationException` when container runtime health checks fail.

### Progress reporting

- Encapsulate long-running logical phases in steps rather than emitting raw tasks.
- Keep titles concise (under 60 characters) as the CLI truncates longer strings.
- Call `CompletePublishAsync` exactly once per publishing or deployment operation.
- Treat warnings as recoverable and allow subsequent steps to proceed.
- Treat errors as fatal and fail fast with clear diagnostics.
- Use asynchronous, cancellation-aware operations to avoid blocking event processing.

### State management

- Each step and task starts in *Running* state and transitions exactly once to *Completed*, *Warning*, or *Error*.
- Attempting multiple state transitions throws an exception.
- The reporter guarantees ordered events and prevents interleaving.
- Disposal of `IPublishingActivityStep` automatically completes unfinished steps.

## Behavior summary

| Aspect | Description |
|--------|-------------|
| **Step hierarchy** | Steps form a strict tree; tasks belong to a single step. Nested steps are unsupported. |
| **Ordering** | Creation and completion events preserve call order; updates are serialized. |
| **State machine** | Each step/task transitions exactly once to a final state. |
| **Cancellation** | All APIs accept <xref:System.Threading.CancellationToken> and propagate cancellation to the CLI. |
| **CLI integration** | The CLI renders status glyphs, elapsed time, and colored output; failures set the exit code. |
| **Concurrency** | Thread-safe implementation supports parallel task creation with deterministic completion ordering. |
| **Disposal contract** | Disposing steps automatically completes them if unfinished, preventing orphaned phases. |

## Container image formats

Aspire supports two container image formats through the `ContainerImageFormat` enumeration:

- **Docker**: The traditional Docker image format, widely supported by container registries.
- **OCI**: Open Container Initiative format, the modern standard for container images.

For maximum compatibility with container registries and deployment targets, use the OCI format in production scenarios.

## Target platforms

The `ContainerTargetPlatform` enumeration supports multiple target architectures:

- **LinuxAmd64**: Linux x86-64 architecture (most common).
- **LinuxArm64**: Linux ARM64 architecture (Apple Silicon, Advanced RISC Machines (ARM) servers).
- **WindowsAmd64**: Windows x86-64 architecture.

Choose the target platform based on your deployment environment. For cloud deployments, Linux AMD64 is typically the best choice.

## Troubleshooting

If you run into issues building container images, check that your project files and Dockerfiles are correctly configured, and confirm that all required dependencies are present. Review error messages in the progress reporter for clues, and consult the [container configuration](../app-host/configuration.md) documentation for troubleshooting tips.

### Container runtime issues

If you encounter container runtime errors:

1. **Verify Docker/Podman is running**: The `ResourceContainerImageBuilder` requires a healthy container runtime only when building from Dockerfiles.

1. **Check container runtime health**: Use `docker version` or `podman version` to verify the runtime is accessible.

1. **Review build logs**: Container build failures are reported through the progress reporter with detailed error messages.

### Progress reporting issues

If progress reporting isn't working as expected:

1. **Verify step completion**: Ensure all steps are properly disposed or explicitly completed.

1. **Check cancellation handling**: Make sure `CancellationToken` parameters are passed correctly through all async operations.

1. **Review state transitions**: Each step and task can only transition to a final state once.

## See also

- [Publishing and deployment overview](../deployment/overview.md)
- [Container configuration](../app-host/configuration.md)
- [Dockerfile integration](../app-host/withdockerfile.md)
