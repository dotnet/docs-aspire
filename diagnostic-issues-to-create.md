# Diagnostic Issues to Create

This document contains the information needed to create three GitHub issues for new diagnostic codes introduced in PR #12416.

Each issue should be created using the template at: https://github.com/dotnet/docs-aspire/issues/new?template=06-diagnostic-addition.yml

---

## Issue 1: ASPIREPIPELINES001

**Rule Type:** Warning

**Version Introduced:** 9.2

**Diagnostic ID:** ASPIREPIPELINES001

**Short Title:** Pipeline infrastructure APIs are experimental and subject to change

**Description:**
This diagnostic is triggered when using experimental pipeline infrastructure APIs in Aspire. These APIs include:

- `IPipelineActivityReporter` - Interface for reporting pipeline activities
- `IReportingStep` - Interface representing a publishing step
- `IReportingTask` - Interface representing a publishing task  
- `CompletionState` - Enum representing completion state of activities
- `PublishingContext` - Context for publishing operations
- `PublishingCallbackAnnotation` - Annotation for publishing callbacks
- `PipelineContext` - Context for pipeline operations
- All types in the `Aspire.Hosting.Pipelines` namespace

These APIs are experimental and are expected to change in future releases.

Example code that triggers this diagnostic:

```csharp
using Aspire.Hosting.Pipelines;

builder.WithPublishingCallback(async (PublishingContext context) =>
{
    var reporter = context.ServiceProvider.GetRequiredService<IPipelineActivityReporter>();
    var step = await reporter.CreateStepAsync("My Step");
    // ...
});
```

**How to fix:**

Suppress the diagnostic using one of the following methods:

1. Set the severity in `.editorconfig`:

```ini
[*.{cs,vb}]
dotnet_diagnostic.ASPIREPIPELINES001.severity = none
```

2. Add to project file:

```xml
<PropertyGroup>
    <NoWarn>$(NoWarn);ASPIREPIPELINES001</NoWarn>
</PropertyGroup>
```

3. Use pragma directive in code:

```csharp
#pragma warning disable ASPIREPIPELINES001
// Your code using pipeline APIs
#pragma warning restore ASPIREPIPELINES001
```

---

## Issue 2: ASPIREPIPELINES002

**Rule Type:** Warning

**Version Introduced:** 9.2

**Diagnostic ID:** ASPIREPIPELINES002

**Short Title:** Deployment state management APIs are experimental and subject to change

**Description:**
This diagnostic is triggered when using experimental deployment state management APIs in Aspire. These APIs include:

- `IDeploymentStateManager` - Interface for managing deployment state
- Implementation classes: `FileDeploymentStateManager`, `UserSecretsDeploymentStateManager`
- `PublishingOptions.Deploy` - Property to enable deployment after publishing
- `PublishingOptions.ClearCache` - Property to clear deployment cache
- `PublishingOptions.Step` - Property to specify a specific deployment step to run

These APIs allow publishers to maintain state between deployments, enabling incremental deployments and caching. They are experimental and are expected to change in future releases.

Example code that triggers this diagnostic:

```csharp
using Aspire.Hosting.Publishing;

var publishingOptions = new PublishingOptions
{
    Deploy = true,
    ClearCache = false,
    Step = "MyDeploymentStep"
};

var stateManager = serviceProvider.GetRequiredService<IDeploymentStateManager>();
await stateManager.SetStateAsync("myKey", "myValue");
```

**How to fix:**

Suppress the diagnostic using one of the following methods:

1. Set the severity in `.editorconfig`:

```ini
[*.{cs,vb}]
dotnet_diagnostic.ASPIREPIPELINES002.severity = none
```

2. Add to project file:

```xml
<PropertyGroup>
    <NoWarn>$(NoWarn);ASPIREPIPELINES002</NoWarn>
</PropertyGroup>
```

3. Use pragma directive in code:

```csharp
#pragma warning disable ASPIREPIPELINES002
// Your code using deployment state APIs
#pragma warning restore ASPIREPIPELINES002
```

---

## Issue 3: ASPIREPIPELINES003

**Rule Type:** Warning

**Version Introduced:** 9.2

**Diagnostic ID:** ASPIREPIPELINES003

**Short Title:** Container image building APIs are experimental and subject to change

**Description:**
This diagnostic is triggered when using experimental container image building APIs in Aspire. These APIs include:

- `IResourceContainerImageBuilder` - Interface for building container images
- `ContainerBuildOptions` - Options for building container images
- `ContainerImageFormat` - Enum specifying image format (Docker or OCI)
- `ContainerTargetPlatform` - Enum specifying target platform (LinuxAmd64, LinuxArm64, etc.)
- Docker and Podman container runtime implementations

These APIs enable publishers to build container images for resources during the publishing process. They are experimental and are expected to change in future releases.

Example code that triggers this diagnostic:

```csharp
using Aspire.Hosting.Publishing;

var builder = serviceProvider.GetRequiredService<IResourceContainerImageBuilder>();

var options = new ContainerBuildOptions
{
    ImageFormat = ContainerImageFormat.Oci,
    TargetPlatform = ContainerTargetPlatform.LinuxAmd64
};

await builder.BuildImageAsync(resource, options);
```

**How to fix:**

Suppress the diagnostic using one of the following methods:

1. Set the severity in `.editorconfig`:

```ini
[*.{cs,vb}]
dotnet_diagnostic.ASPIREPIPELINES003.severity = none
```

2. Add to project file:

```xml
<PropertyGroup>
    <NoWarn>$(NoWarn);ASPIREPIPELINES003</NoWarn>
</PropertyGroup>
```

3. Use pragma directive in code:

```csharp
#pragma warning disable ASPIREPIPELINES003
// Your code using container image building APIs
#pragma warning restore ASPIREPIPELINES003
```

---

## Notes

These three diagnostics replace the single `ASPIREPUBLISHERS001` diagnostic, which was previously used for all publishing-related experimental APIs. The split allows developers to selectively suppress diagnostics based on which specific experimental features they are using.
