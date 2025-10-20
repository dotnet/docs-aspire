---
title: "Breaking change - Activity reporter and pipeline context renamed"
description: "Learn about the breaking change in Aspire 13.0 where publishing activity reporter APIs and context types were renamed to reflect pipeline architecture."
ms.date: 10/20/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/aspire/pull/12137
---

# Activity reporter and pipeline context renamed

In Aspire 13.0, the publishing activity reporter APIs and context types have been renamed to better reflect the pipeline architecture. The publishing step creation process has been simplified, with steps now automatically created during pipeline execution rather than requiring explicit creation within step actions.

## Version introduced

Aspire 13.0 Preview 1

## Previous behavior

The previous API used publishing-specific names and required explicit step creation within pipeline actions:

```csharp
builder.Pipeline.AddStep("assign-storage-role", async (context) =>
{
    var roleAssignmentStep = await context.ActivityReporter
        .CreateStepAsync($"assign-storage-role", context.CancellationToken);
    
    await using (roleAssignmentStep.ConfigureAwait(false))
    {
        var assignRoleTask = await roleAssignmentStep
            .CreateTaskAsync($"Granting file share access...", context.CancellationToken);
        
        await using (assignRoleTask.ConfigureAwait(false))
        {
            // ... task work
        }
    }
});
```

The types used were:

- `DeployingContext` - Provided context for pipeline execution
- `IPublishingActivityReporter` - Interface for reporting activities
- `PublishingActivityReporter` - Implementation of the activity reporter
- `NullPublishingActivityReporter` - Null object pattern implementation
- `IPublishingStep` - Interface for publishing steps
- `IPublishingTask` - Interface for publishing tasks

## New behavior

The API now uses pipeline-specific names and automatically creates steps during pipeline execution:

```csharp
builder.Pipeline.AddStep("assign-storage-role", async (deployingContext) =>
{
    var assignRoleTask = await deployingContext.ReportingStep
        .CreateTaskAsync($"Granting file share access...", deployingContext.CancellationToken);
    
    await using (assignRoleTask.ConfigureAwait(false))
    {
        // ... task work
    }
});
```

The types have been renamed as follows:

- `DeployingContext` → `PipelineContext` - Shared context across all pipeline steps
- `IPublishingActivityReporter` → `IPipelineActivityReporter` - Interface for reporting pipeline activities
- `PublishingActivityReporter` → `PipelineActivityReporter` - Implementation of the pipeline activity reporter
- `NullPublishingActivityReporter` → `NullPipelineActivityReporter` - Null object pattern implementation
- `IPublishingStep` → `IReportingStep` - Interface for reporting steps
- `IPublishingTask` → `IReportingTask` - Interface for reporting tasks

Additionally, a new `PipelineStepContext` type has been introduced that combines the shared `PipelineContext` with a step-specific `IReportingStep`, allowing each step to track its own tasks and completion state independently.

## Type of breaking change

This change can affect [binary compatibility](../categories.md#binary-compatibility) and [source compatibility](../categories.md#source-compatibility).

## Reason for change

The previous three-level hierarchy (`ActivityReporter → Step → Task`) was unnecessarily complex. The new architecture simplifies this by automatically creating steps during pipeline execution and integrating step management directly into the pipeline. This provides a cleaner separation between the shared pipeline context and step-specific execution context, making the API more intuitive and reducing boilerplate code.

## Recommended action

Update your code to use the new type names and simplified step creation pattern:

1. Replace `DeployingContext` with `PipelineContext` for shared pipeline context or `PipelineStepContext` for step-specific context.
1. Replace `IPublishingActivityReporter` with `IPipelineActivityReporter`.
1. Replace `PublishingActivityReporter` with `PipelineActivityReporter`.
1. Replace `NullPublishingActivityReporter` with `NullPipelineActivityReporter`.
1. Replace `IPublishingStep` with `IReportingStep`.
1. Replace `IPublishingTask` with `IReportingTask`.
1. Update pipeline step actions to accept `PipelineStepContext` instead of `DeployingContext`.
1. Remove explicit step creation calls within pipeline actions and use the automatically created `context.ReportingStep` instead.

Migration example:

```csharp
// Before
builder.Pipeline.AddStep("my-step", async (context) =>
{
    var step = await context.ActivityReporter
        .CreateStepAsync("my-step", context.CancellationToken);
    
    await using (step.ConfigureAwait(false))
    {
        var task = await step.CreateTaskAsync("Doing work...", context.CancellationToken);
        await using (task.ConfigureAwait(false))
        {
            // Do work
            await task.CompleteAsync("Done", CompletionState.Completed, context.CancellationToken);
        }
    }
});

// After
builder.Pipeline.AddStep("my-step", async (stepContext) =>
{
    var task = await stepContext.ReportingStep
        .CreateTaskAsync("Doing work...", stepContext.CancellationToken);
    
    await using (task.ConfigureAwait(false))
    {
        // Do work
        await task.CompleteAsync("Done", CompletionState.Completed, stepContext.CancellationToken);
    }
});
```

## Affected APIs

- <xref:Aspire.Hosting.ApplicationModel.DeployingContext?displayProperty=fullName>
- <xref:Aspire.Hosting.Publishing.IPublishingActivityReporter?displayProperty=fullName>
- <xref:Aspire.Hosting.Publishing.PublishingActivityReporter?displayProperty=fullName>
- <xref:Aspire.Hosting.Publishing.NullPublishingActivityReporter?displayProperty=fullName>
- <xref:Aspire.Hosting.Publishing.IPublishingStep?displayProperty=fullName>
- <xref:Aspire.Hosting.Publishing.IPublishingTask?displayProperty=fullName>
- <xref:Aspire.Hosting.Publishing.PublishingStep?displayProperty=fullName>
- <xref:Aspire.Hosting.Publishing.PublishingTask?displayProperty=fullName>
- <xref:Aspire.Hosting.Publishing.PublishingExtensions?displayProperty=fullName>
