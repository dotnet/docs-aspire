# Issue: ASPIREPIPELINES001

**Issue Template:** https://github.com/dotnet/docs-aspire/issues/new?template=06-diagnostic-addition.yml

## Field Values

### Rule Type
Warning

### Version Introduced
9.2

### Diagnostic ID
ASPIREPIPELINES001

### Short Title
Pipeline infrastructure APIs are experimental and subject to change

### Description

This diagnostic is triggered when using experimental pipeline infrastructure APIs in Aspire that provide core publishing pipeline functionality. These APIs allow developers to report progress, create steps and tasks, and track completion states during publishing operations.

**APIs covered by this diagnostic:**

- `IPipelineActivityReporter` - Interface for reporting pipeline activities
- `IReportingStep` - Interface representing a publishing step that can contain multiple tasks
- `IReportingTask` - Interface representing a publishing task within a step
- `CompletionState` - Enum representing completion state (InProgress, Completed, CompletedWithWarning, CompletedWithError)
- `PublishingContext` - Context object passed to publishing callbacks
- `PublishingCallbackAnnotation` - Annotation for registering publishing callbacks on resources
- `PipelineContext` - Context for pipeline operations
- `PipelineStepContext` - Context for individual pipeline steps
- All other types in the `Aspire.Hosting.Pipelines` namespace

These APIs are experimental and subject to change in future releases as the publishing pipeline infrastructure matures.

**Example code that triggers this diagnostic:**

```csharp
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Pipelines;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddResource(someResource)
    .WithPublishingCallback(async (PublishingContext context) =>
    {
        var reporter = context.ServiceProvider.GetRequiredService<IPipelineActivityReporter>();
        var step = await reporter.CreateStepAsync("Deploy resources");
        
        try
        {
            var task = await step.CreateTaskAsync("Deploying resource...");
            // Perform deployment
            await task.CompleteAsync("Resource deployed", CompletionState.Completed);
            await step.CompleteAsync("All resources deployed", CompletionState.Completed);
        }
        catch (Exception ex)
        {
            await step.CompleteAsync($"Deployment failed: {ex.Message}", CompletionState.CompletedWithError);
        }
    });
```

### How to fix

This diagnostic is informational and indicates you're using experimental APIs. To proceed with using these APIs, suppress the diagnostic using one of the following methods:

**Option 1: Suppress in .editorconfig file**

```ini
[*.{cs,vb}]
dotnet_diagnostic.ASPIREPIPELINES001.severity = none
```

For more information about editor config files, see [Configuration files for code analysis rules](/dotnet/fundamentals/code-analysis/configuration-files).

**Option 2: Suppress in project file**

Add the following `PropertyGroup` to your project file:

```xml
<PropertyGroup>
    <NoWarn>$(NoWarn);ASPIREPIPELINES001</NoWarn>
</PropertyGroup>
```

**Option 3: Suppress in code using pragma directive**

```csharp
#pragma warning disable ASPIREPIPELINES001
// Your code using pipeline infrastructure APIs
#pragma warning restore ASPIREPIPELINES001
```

**Note:** These APIs are experimental and may change in future releases. Review the release notes when upgrading to ensure compatibility.
