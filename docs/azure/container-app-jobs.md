---
title: Azure Container App Jobs
description: Learn how to use Azure Container App Jobs in .NET Aspire to run containerized tasks that execute for a finite duration.
ms.date: 09/16/2025
ms.topic: how-to
---

# Azure Container App Jobs

Azure Container Apps jobs enable you to run containerized tasks that execute for a finite duration and exit. You can use jobs to perform tasks such as data processing, machine learning, or any scenario where on-demand processing is required. Unlike traditional Azure Container Apps that run continuously, Container App Jobs are designed for batch processing, scheduled tasks, and event-driven workloads.

.NET Aspire provides support for Azure Container App Jobs through the `PublishAsAzureContainerAppJob` extension method, allowing you to deploy your .NET projects, containers, and executables as jobs in Azure Container Apps.

## Prerequisites

- Azure subscription
- .NET Aspire project
- Understanding of [Azure Container Apps Jobs](/azure/container-apps/jobs)

## Supported resource types

.NET Aspire allows you to publish the following resource types as Azure Container App Jobs:

- <xref:Aspire.Hosting.ApplicationModel.ContainerResource>: Represents a specified container.
- <xref:Aspire.Hosting.ApplicationModel.ExecutableResource>: Represents a specified executable process.  
- <xref:Aspire.Hosting.ApplicationModel.ProjectResource>: Represents a specified .NET project.

## Publishing as Azure Container App Jobs

To publish resources as Azure Container App Jobs, use the following APIs:

- <xref:Aspire.Hosting.AzureContainerAppContainerExtensions.PublishAsAzureContainerAppJob``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},System.Action{Aspire.Hosting.Azure.AzureResourceInfrastructure,Azure.Provisioning.AppContainers.ContainerAppJob})?displayProperty=nameWithType>
- <xref:Aspire.Hosting.AzureContainerAppExecutableExtensions.PublishAsAzureContainerAppJob``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},System.Action{Aspire.Hosting.Azure.AzureResourceInfrastructure,Azure.Provisioning.AppContainers.ContainerAppJob})?displayProperty=nameWithType>
- <xref:Aspire.Hosting.AzureContainerAppProjectExtensions.PublishAsAzureContainerAppJob``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},System.Action{Aspire.Hosting.Azure.AzureResourceInfrastructure,Azure.Provisioning.AppContainers.ContainerAppJob})?displayProperty=nameWithType>

## Basic usage

The following example demonstrates how to configure a .NET project as an Azure Container App Job with a scheduled trigger:

```csharp
var builder = DistributedApplication.CreateBuilder();

builder.AddProject<Projects.DataProcessor>("data-processor")
    .PublishAsAzureContainerAppJob((_, job) =>
    {
        job.Configuration.TriggerType = ContainerAppJobTriggerType.Schedule;
        job.Configuration.ScheduleTriggerConfig.CronExpression = "0 0 * * *"; // Every day at midnight
    });
```

This code:

- Creates a new distributed application builder.
- Adds a project named `data-processor` to the builder.
- Calls `PublishAsAzureContainerAppJob` to configure the project as a Container App Job.
- Sets the trigger type to `Schedule` with a cron expression to run daily at midnight.

## Job trigger types

Azure Container App Jobs support different trigger types:

### Manual trigger

Use manual triggers for on-demand job execution:

```csharp
builder.AddProject<Projects.ManualTask>("manual-task")
    .PublishAsAzureContainerAppJob((_, job) =>
    {
        job.Configuration.TriggerType = ContainerAppJobTriggerType.Manual;
    });
```

### Schedule trigger

Use schedule triggers for time-based job execution:

```csharp
builder.AddProject<Projects.ScheduledTask>("scheduled-task")
    .PublishAsAzureContainerAppJob((_, job) =>
    {
        job.Configuration.TriggerType = ContainerAppJobTriggerType.Schedule;
        job.Configuration.ScheduleTriggerConfig.CronExpression = "0 */6 * * *"; // Every 6 hours
        job.Configuration.ScheduleTriggerConfig.Parallelism = 1;
        job.Configuration.ScheduleTriggerConfig.CompletionCount = 1;
    });
```

### Event trigger

Use event triggers for reactive job execution based on external events:

```csharp
builder.AddProject<Projects.EventDrivenTask>("event-task")
    .PublishAsAzureContainerAppJob((_, job) =>
    {
        job.Configuration.TriggerType = ContainerAppJobTriggerType.Event;
        job.Configuration.EventTriggerConfig.Scale.MinReplicas = 0;
        job.Configuration.EventTriggerConfig.Scale.MaxReplicas = 10;
        job.Configuration.EventTriggerConfig.Parallelism = 1;
        job.Configuration.EventTriggerConfig.CompletionCount = 1;
    });
```

## Advanced configuration

### Setting resource requirements

Configure CPU and memory resources for your job:

```csharp
builder.AddProject<Projects.ResourceIntensiveTask>("intensive-task")
    .PublishAsAzureContainerAppJob((_, job) =>
    {
        job.Configuration.TriggerType = ContainerAppJobTriggerType.Manual;
        job.Template.InitContainers[0].Resources.Cpu = "1.0";
        job.Template.InitContainers[0].Resources.Memory = "2Gi";
    });
```

### Environment variables

Add environment variables to your job:

```csharp
var connectionString = builder.AddParameter("connectionString");

builder.AddProject<Projects.DatabaseTask>("db-task")
    .PublishAsAzureContainerAppJob((infra, job) =>
    {
        job.Configuration.TriggerType = ContainerAppJobTriggerType.Manual;
        job.Template.InitContainers[0].Env.Add(new ContainerAppEnvironmentVariable
        {
            Name = "ConnectionString",
            Value = connectionString.AsProvisioningParameter(infra)
        });
    });
```

### Timeout and retry policy

Configure job execution timeout and retry behavior:

```csharp
builder.AddProject<Projects.RetryableTask>("retryable-task")
    .PublishAsAzureContainerAppJob((_, job) =>
    {
        job.Configuration.TriggerType = ContainerAppJobTriggerType.Manual;
        job.Configuration.TimeoutInSeconds = 1800; // 30 minutes
        job.Configuration.RetryPolicy.RetryLimit = 3;
        job.Configuration.RetryPolicy.RetryLimitPolicy = ContainerAppJobRetryLimitPolicy.RestartFailedContainers;
    });
```

## Container resources

You can also publish container resources as Azure Container App Jobs:

```csharp
builder.AddContainer("batch-processor", "myregistry.azurecr.io/batch-processor:latest")
    .PublishAsAzureContainerAppJob((_, job) =>
    {
        job.Configuration.TriggerType = ContainerAppJobTriggerType.Schedule;
        job.Configuration.ScheduleTriggerConfig.CronExpression = "0 2 * * 0"; // Weekly on Sunday at 2 AM
    });
```

## Executable resources

Executable resources can also be published as jobs:

```csharp
builder.AddExecutable("data-script", "python", ".", "process_data.py")
    .PublishAsAzureContainerAppJob((_, job) =>
    {
        job.Configuration.TriggerType = ContainerAppJobTriggerType.Manual;
    });
```

## Experimental feature notice

> [!IMPORTANT]
> Azure Container App Jobs support in .NET Aspire is an experimental feature. The APIs are subject to change in future releases. You might encounter the diagnostic `ASPIREAZURE002` when using these features. For more information, see [ASPIREAZURE002](../diagnostics/aspireazure002.md).

## Related content

- [Configure Azure Container Apps environments](configure-aca-environments.md)
- [Azure Container Apps Jobs documentation](/azure/container-apps/jobs)
- [Deploy a .NET Aspire project to Azure Container Apps](../deployment/azure/aca-deployment.md)
- [Customize Azure resources](customize-azure-resources.md)
