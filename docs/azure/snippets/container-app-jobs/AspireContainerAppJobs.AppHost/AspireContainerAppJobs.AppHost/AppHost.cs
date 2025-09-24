var builder = DistributedApplication.CreateBuilder(args);

// Example: Scheduled data processing job
builder.AddProject<Projects.DataProcessor>("data-processor")
    .PublishAsAzureContainerAppJob((_, job) =>
    {
        job.Configuration.TriggerType = ContainerAppJobTriggerType.Schedule;
        job.Configuration.ScheduleTriggerConfig.CronExpression = "0 0 * * *"; // Every day at midnight
    });

// Example: Manual trigger job with environment variables
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

// Example: Container resource as a job
builder.AddContainer("batch-processor", "myregistry.azurecr.io/batch-processor:latest")
    .PublishAsAzureContainerAppJob((_, job) =>
    {
        job.Configuration.TriggerType = ContainerAppJobTriggerType.Schedule;
        job.Configuration.ScheduleTriggerConfig.CronExpression = "0 2 * * 0"; // Weekly on Sunday at 2 AM
    });

builder.Build().Run();