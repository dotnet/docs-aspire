# Issue: ASPIREPIPELINES002

**Issue Template:** https://github.com/dotnet/docs-aspire/issues/new?template=06-diagnostic-addition.yml

## Field Values

### Rule Type
Warning

### Version Introduced
9.2

### Diagnostic ID
ASPIREPIPELINES002

### Short Title
Deployment state management APIs are experimental and subject to change

### Description

This diagnostic is triggered when using experimental deployment state management APIs in Aspire. These APIs enable publishers to persist and retrieve deployment state between publishing operations, allowing for incremental deployments, caching, and step-by-step deployment workflows.

**APIs covered by this diagnostic:**

- `IDeploymentStateManager` - Interface for managing deployment state with methods to get, set, and clear state
- `FileDeploymentStateManager` - Implementation that stores state in the file system
- `UserSecretsDeploymentStateManager` - Implementation that stores state using user secrets
- `DeploymentStateManagerBase` - Base class for state manager implementations
- `PublishingOptions.Deploy` - Property to enable automatic deployment after publishing
- `PublishingOptions.ClearCache` - Property to clear deployment state cache
- `PublishingOptions.Step` - Property to specify a specific deployment step to execute

These APIs are experimental and subject to change in future releases as deployment state management capabilities mature.

**Example code that triggers this diagnostic:**

```csharp
using Aspire.Hosting.Publishing;

// Configure publishing options with deployment state
var publishingOptions = new PublishingOptions
{
    OutputPath = "./output",
    Deploy = true,              // Triggers ASPIREPIPELINES002
    ClearCache = false,         // Triggers ASPIREPIPELINES002
    Step = "DeployToAzure"      // Triggers ASPIREPIPELINES002
};

// Use deployment state manager
var stateManager = serviceProvider.GetRequiredService<IDeploymentStateManager>();
await stateManager.SetStateAsync("lastDeployment", DateTime.UtcNow.ToString());

var previousState = await stateManager.GetStateAsync("deploymentConfig");
if (previousState != null)
{
    // Use cached state for incremental deployment
}
```

### How to fix

This diagnostic is informational and indicates you're using experimental APIs. To proceed with using these APIs, suppress the diagnostic using one of the following methods:

**Option 1: Suppress in .editorconfig file**

```ini
[*.{cs,vb}]
dotnet_diagnostic.ASPIREPIPELINES002.severity = none
```

For more information about editor config files, see [Configuration files for code analysis rules](/dotnet/fundamentals/code-analysis/configuration-files).

**Option 2: Suppress in project file**

Add the following `PropertyGroup` to your project file:

```xml
<PropertyGroup>
    <NoWarn>$(NoWarn);ASPIREPIPELINES002</NoWarn>
</PropertyGroup>
```

**Option 3: Suppress in code using pragma directive**

```csharp
#pragma warning disable ASPIREPIPELINES002
// Your code using deployment state management APIs
#pragma warning restore ASPIREPIPELINES002
```

**Note:** These APIs are experimental and may change in future releases. The shape of the state management API and the storage implementations may evolve based on feedback and requirements. Review the release notes when upgrading to ensure compatibility.
