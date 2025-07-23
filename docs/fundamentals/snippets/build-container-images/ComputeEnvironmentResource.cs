using System.Diagnostics.CodeAnalysis;
using Aspire.Hosting.Publishing;
using Microsoft.Extensions.DependencyInjection;

namespace Examples.ContainerBuilding;

[Experimental("ASPIRECOMPUTE001")]
public sealed class ComputeEnvironmentResource : Resource
{
    public ComputeEnvironmentResource(string name) : base(name)
    {
        Annotations.Add(new PublishingCallbackAnnotation(PublishAsync));
        Annotations.Add(new DeployingCallbackAnnotation(DeployAsync));
    }

    private static async Task PublishAsync(PublishingContext context)
    {
        var reporter = context.ActivityReporter;
        var imageBuilder = context.Services.GetRequiredService<IResourceContainerImageBuilder>();

        // Build container images for all project resources in the application
        await using (var buildStep = await reporter.CreateStepAsync(
                     "Build container images", context.CancellationToken))
        {
            // Find all project resources that need container images
            var projectResources = context.Model.Resources
                .OfType<ProjectResource>()
                .Where(r => r.Annotations.OfType<ContainerImageAnnotation>().Any())
                .ToList();

            if (projectResources.Count > 0)
            {
                // Configure how images should be built
                var buildOptions = new ContainerBuildOptions
                {
                    ImageFormat = ContainerImageFormat.Oci,
                    TargetPlatform = ContainerTargetPlatform.LinuxAmd64,
                    OutputPath = Path.Combine(context.OutputPath, "images")
                };

                var buildTask = await buildStep.CreateTaskAsync(
                    $"Building {projectResources.Count} container images", context.CancellationToken);

                // Build all the container images
                await imageBuilder.BuildImagesAsync(
                    projectResources, buildOptions, context.CancellationToken);

                await buildTask.SucceedAsync(
                    $"Built {projectResources.Count} images successfully", context.CancellationToken);
            }
            else
            {
                var skipTask = await buildStep.CreateTaskAsync(
                    "No container images to build", context.CancellationToken);
                await skipTask.SucceedAsync("Skipped - no project resources found", context.CancellationToken);
            }

            await buildStep.SucceedAsync("Container image build completed", context.CancellationToken);
        }

        // Generate deployment manifests
        await using (var manifestStep = await reporter.CreateStepAsync(
                     "Generate deployment manifests", context.CancellationToken))
        {
            var bicepTask = await manifestStep.CreateTaskAsync(
                "Generate Bicep templates", context.CancellationToken);
            
            // Generate deployment files in the output directory
            var manifestPath = Path.Combine(context.OutputPath, "main.bicep");
            await File.WriteAllTextAsync(manifestPath, 
                "// Generated deployment template", context.CancellationToken);
            
            await bicepTask.SucceedAsync(
                $"Bicep template written to {manifestPath}", context.CancellationToken);
            
            await manifestStep.SucceedAsync("All manifests generated", context.CancellationToken);
        }

        // Complete the publishing operation
        await reporter.CompletePublishAsync(
            completionMessage: "Publishing pipeline completed successfully",
            completionState: CompletionState.Completed,
            cancellationToken: context.CancellationToken);
    }

    private static async Task DeployAsync(DeployingContext context)
    {
        var reporter = context.ActivityReporter;

        await using (var deployStep = await reporter.CreateStepAsync(
                     "Deploy to target environment", context.CancellationToken))
        {
            var applyTask = await deployStep.CreateTaskAsync(
                "Apply Kubernetes manifests", context.CancellationToken);
            
            // Simulate deploying to Kubernetes cluster
            await Task.Delay(1000, context.CancellationToken); // Simulate deployment work
            
            await applyTask.SucceedAsync("All workloads deployed", context.CancellationToken);
            await deployStep.SucceedAsync("Deployment to cluster completed", context.CancellationToken);
        }

        // Complete the deployment operation
        await reporter.CompletePublishAsync(
            completionMessage: "Deployment completed successfully",
            completionState: CompletionState.Completed,
            isDeploy: true,
            cancellationToken: context.CancellationToken);
    }
}

// Extension method to make it easier to add the custom resource
[Experimental("ASPIRECOMPUTE001")]
public static class ComputeEnvironmentResourceExtensions
{
    public static IResourceBuilder<ComputeEnvironmentResource> AddComputeEnvironment(
        this IDistributedApplicationBuilder builder,
        string name)
    {
        return builder.AddResource(new ComputeEnvironmentResource(name));
    }
}
