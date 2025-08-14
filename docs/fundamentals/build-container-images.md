---
title: Build container images
description: Learn how to build container images from your .NET Aspire resources.
ms.date: 08/07/2025
ai-usage: ai-assisted
---

# Build container images (Preview)

.NET Aspire provides powerful APIs for building container images from your resources during publishing and deployment operations. This article covers the key components that enable programmatic container image creation and progress reporting.

## Overview

During publishing and deployment, the container image builder is available to create images for resources that need them. Aspire uses this builder when a resource requires a container image, such as when publishing with Docker Compose. The process involves two main components:

- <xref:Aspire.Hosting.Publishing.IResourceContainerImageBuilder>: The service that turns resource definitions into runnable container images.
- <xref:Aspire.Hosting.Publishing.IPublishingActivityReporter>: The API that provides structured progress reporting during long-running operations.

These APIs give you fine-grained control over the image building process and provide real-time feedback to users during lengthy build operations.

> [!IMPORTANT]
> These APIs are currently in preview and subject to change. They are designed for advanced scenarios where you need custom control over container image building and progress reporting. To suppress warnings for these APIs, see [Compiler Error ASPIREPUBLISHERS001](../diagnostics/aspirepublishers001.md).

## When to use these APIs

Consider using the container image building and progress reporting APIs in these scenarios:

- **Custom deployment targets**: When you need to deploy to platforms that require specific image formats or build configurations.
- **Complex build pipelines**: When your publishing process involves multiple steps that users should see.
- **Enterprise scenarios**: When you need custom progress reporting for integration with CI/CD systems or dashboards.
- **Custom resource types**: When implementing custom resources that need to participate in the publishing and deployment process.

> [!NOTE]
> For most standard Aspire applications, the built-in publishing process builds container images automatically without requiring these APIs.

## Resource container image builder API

The `IResourceContainerImageBuilder` is the core service in the <xref:Aspire.Hosting.Publishing> layer that converts resource definitions into container images. It analyzes each resource in your distributed application model and determines whether to:

- Reuse an existing image.
- Build from a .NET project using `dotnet publish /t:PublishContainer`.
- Build from a Dockerfile using the local container runtime.

### Container build options

The <xref:Aspire.Hosting.Publishing.ContainerBuildOptions> class provides strongly typed configuration for container builds. This class allows you to specify:

- **Image format**: Docker or Open Container Initiative (OCI) format.
- **Target platform**: Linux x64, Windows, ARM64, etc.
- **Output path**: Where to save the built images.

### Container runtime health checks

The builder performs container runtime health checks (Docker/Podman) only when at least one resource requires a Dockerfile build. This change eliminates false-positive errors in projects that publish directly from .NET assemblies. If the container runtime is required but unhealthy, the builder throws an explicit `InvalidOperationException` to surface the problem early.

## Publishing activity reporter API

The `PublishingActivityProgressReporter` API enables structured progress reporting during [`aspire publish`](../cli-reference/aspire-publish.md) and [`aspire deploy`](../cli-reference/aspire-deploy.md) commands. This reduces uncertainty during long-running operations and surfaces failures early.

### API overview and behavior

The progress reporter uses a hierarchical model with guaranteed ordering and thread-safe operations:

| Concept | Description | CLI Rendering | Behavior |
|---------|-------------|---------------|----------|
| **Step** | Top-level phase, such as "Build images" or "Deploy workloads". | Step message with status glyph and elapsed time. | Forms a strict tree structure; nested steps are unsupported. |
| **Task** | Discrete unit of work nested under a step. | Task message with indentation. | Belongs to a single step; supports parallel creation with deterministic completion ordering. |
| **Completion state** | Final status: `Completed`, `Warning`, or `Error`. | ✅ (Completed), ⚠️ (Warning), ❌ (Error) | Each step/task transitions exactly once to a final state. |

### API structure and usage

The reporter API provides structured access to progress reporting with the following characteristics:

- **Acquisition**: Retrieved from `PublishingContext.ActivityReporter` or `DeployingContext.ActivityReporter`.
- **Step creation**: `CreateStepAsync(title, ct)` returns an `IPublishingActivityStep`.
- **Task creation**: `IPublishingActivityStep.CreateTaskAsync(title, ct)` returns an `IPublishingActivityTask`.
- **State transitions**: `SucceedAsync`, `WarnAsync`, `FailAsync` methods accept a summary message.
- **Completion**: `CompletePublishAsync(message, state, isDeploy, ct)` marks the entire operation.
- **Ordering**: Creation and completion events preserve call order; updates are serialized.
- **Cancellation**: All APIs accept <xref:System.Threading.CancellationToken> and propagate cancellation to the CLI.
- **Disposal contract**: Disposing steps automatically completes them if unfinished, preventing orphaned phases.

## Example: Build container images and report progress

To use these APIs, add a `PublishingCallbackAnnotation`, a `DeployingCallbackAnnotation`, or both to a resource in your app model. You can annotate custom (or built-in) resources by adding annotations to the <xref:Aspire.Hosting.ApplicationModel.IResource.Annotations?displayProperty=nameWithType> collection.

As a developer, you can choose to:

- Use both annotations if your resource needs to do work in both publishing and deployment. For example, build images and generate manifests during publishing, then push images or configure deployment targets during deployment. Publishing always happens before deployment, so you can keep logic for each phase separate.

- Use only `PublishingCallbackAnnotation` if your resource only needs to do something during publishing. This is common when you just need to build artifacts or images, but don't need to do anything during deployment.

- Use only `DeployingCallbackAnnotation` if your resource only needs to do something during deployment. This fits cases where you use prebuilt images and just need to deploy or configure them.

Choose one or more annotations that match your resource's responsibilities to keep your application model clear and maintainable. This separation lets you clearly define logic for each phase, but you can use both the activity reporter and the resource container image builder in either callback as needed.

### Example resource with annotations

For example, consider the `ComputeEnvironmentResource` constructor:

:::code source="snippets/build-container-images/apphost/ComputeEnvironmentResource.cs" id="ctor":::

When instantiated, it defines both a publishing and deploying callback annotation.

Given the example `ComputeEnvironmentResource` (<xref:Aspire.Hosting.ApplicationModel.Resource>) type, imagine you have an extension method that you expose so consumers are able to add the compute environment:

:::code source="snippets/build-container-images/apphost/ComputeEnvironmentResourceExtensions.cs":::

The preceding code:

- Defines an extension method on the <xref:Aspire.Hosting.IDistributedApplicationBuilder>.
- Accepts a `name` for the compute environment resource, protected by the <xref:Aspire.Hosting.ApplicationModel.ResourceNameAttribute>.
- Instantiates a `ComputeEnvironmentResource` given the `name` and adds it to the `builder`.

### Example AppHost

In your AppHost, you can add the `ComputeEnvironmentResource` to the application model like this:

:::code source="snippets/build-container-images/apphost/AppHost.cs":::

The preceding code uses the `AddComputeEnvironment` extension method to add the `ComputeEnvironmentResource` to the application model.

### Publishing callback annotation

When you add the `ComputeEnvironmentResource`, it registers a `PublishingCallbackAnnotation`. The callback uses the `PublishAsync` method:

:::code source="snippets/build-container-images/apphost/ComputeEnvironmentResource.cs" id="publish":::

The preceding code:

- Implements a publishing pipeline that builds container images and generates deployment manifests.
- Uses the `IResourceContainerImageBuilder` API to build container images.
- Reports progress and completion status using the `PublishingActivityProgressReporter` API.

Your publishing callback might use `IResourceContainerImageBuilder` to build container images, while your deployment callback might use the built images and push them to a registry or deployment target.

### Deploying callback annotation

Like the publishing callback, the deploying callback is registered using the `DeployingCallbackAnnotation` and calls the `DeployAsync` method:

:::code source="snippets/build-container-images/apphost/ComputeEnvironmentResource.cs" id="deploy":::

The preceding code:

- Simulates deploying workloads to a Kubernetes cluster.
- Uses the `PublishingActivityProgressReporter` API to create and manage deployment steps and tasks.
- Reports progress and marks each deployment phase as completed.
- Completes the deployment operation with a final status update.
- Handles cancellation through the provided `CancellationToken`.

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
- Throw an exception when attempting multiple state transitions.
- Leverage the reporter to guarantee ordered events and prevent interleaving.
- Dispose of `IPublishingActivityStep` to automatically complete unfinished steps.

## See also

- [Publishing and deployment overview](../deployment/overview.md)
- [Container configuration](../app-host/configuration.md)
- [Dockerfile integration](../app-host/withdockerfile.md)
