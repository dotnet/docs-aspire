# Issue: ASPIREPIPELINES003

**Issue Template:** https://github.com/dotnet/docs-aspire/issues/new?template=06-diagnostic-addition.yml

## Field Values

### Rule Type
Warning

### Version Introduced
9.2

### Diagnostic ID
ASPIREPIPELINES003

### Short Title
Container image building APIs are experimental and subject to change

### Description

This diagnostic is triggered when using experimental container image building APIs in Aspire. These APIs enable publishers to programmatically build container images for resources during the publishing process, supporting various image formats and target platforms.

**APIs covered by this diagnostic:**

- `IResourceContainerImageBuilder` - Interface for building container images from resources
- `ContainerBuildOptions` - Class containing options for image building (format, platform, tags, labels)
- `ContainerImageFormat` - Enum specifying the container image format:
  - `Docker` - Docker image format
  - `Oci` - OCI (Open Container Initiative) image format
- `ContainerTargetPlatform` - Enum specifying the target platform:
  - `LinuxAmd64` - Linux on x64 architecture
  - `LinuxArm64` - Linux on ARM64 architecture
  - `WindowsAmd64` - Windows on x64 architecture
- `IContainerRuntime` - Interface for interacting with container runtimes
- `DockerContainerRuntime` - Docker runtime implementation
- `PodmanContainerRuntime` - Podman runtime implementation
- `ContainerRuntimeBase` - Base class for container runtime implementations

These APIs are experimental and subject to change in future releases as container building capabilities mature.

**Example code that triggers this diagnostic:**

```csharp
using Aspire.Hosting.Publishing;

// Build a container image for a resource
var imageBuilder = serviceProvider.GetRequiredService<IResourceContainerImageBuilder>();

var buildOptions = new ContainerBuildOptions
{
    ImageFormat = ContainerImageFormat.Oci,
    TargetPlatform = ContainerTargetPlatform.LinuxAmd64,
    Tags = new[] { "myapp:latest", "myapp:v1.0" },
    Labels = new Dictionary<string, string>
    {
        ["version"] = "1.0.0",
        ["description"] = "My application"
    }
};

await imageBuilder.BuildImageAsync(myResource, buildOptions, cancellationToken);
```

Another example with Docker runtime:

```csharp
using Aspire.Hosting.Publishing;

var containerRuntime = serviceProvider.GetRequiredService<IContainerRuntime>();

// Check if Docker/Podman is available
if (await containerRuntime.IsAvailableAsync())
{
    // Build and push image
    await containerRuntime.BuildImageAsync(
        contextPath: "./app",
        dockerfilePath: "Dockerfile",
        imageName: "myapp:latest"
    );
}
```

### How to fix

This diagnostic is informational and indicates you're using experimental APIs. To proceed with using these APIs, suppress the diagnostic using one of the following methods:

**Option 1: Suppress in .editorconfig file**

```ini
[*.{cs,vb}]
dotnet_diagnostic.ASPIREPIPELINES003.severity = none
```

For more information about editor config files, see [Configuration files for code analysis rules](/dotnet/fundamentals/code-analysis/configuration-files).

**Option 2: Suppress in project file**

Add the following `PropertyGroup` to your project file:

```xml
<PropertyGroup>
    <NoWarn>$(NoWarn);ASPIREPIPELINES003</NoWarn>
</PropertyGroup>
```

**Option 3: Suppress in code using pragma directive**

```csharp
#pragma warning disable ASPIREPIPELINES003
// Your code using container image building APIs
#pragma warning restore ASPIREPIPELINES003
```

**Note:** These APIs are experimental and may change in future releases. The container building capabilities and API surface may evolve based on feedback and new requirements. Review the release notes when upgrading to ensure compatibility.
