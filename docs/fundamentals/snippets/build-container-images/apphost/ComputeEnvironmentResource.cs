using System.Diagnostics.CodeAnalysis;
using Aspire.Hosting.Publishing;
using Microsoft.Extensions.DependencyInjection;

[Experimental("ASPIRECOMPUTE001")]
public sealed class ComputeEnvironmentResource : Resource
{
    // <ctor>
    public ComputeEnvironmentResource(string name) : base(name)
    {
        Annotations.Add(new PublishingCallbackAnnotation(PublishAsync));
        Annotations.Add(new DeployingCallbackAnnotation(DeployAsync));
    }
    // </ctor>

    // <publish>
    private static async Task PublishAsync(PublishingContext context)
    {
        var reporter = context.ActivityReporter;
        var imageBuilder = context.Services.GetRequiredService<IResourceContainerImageBuilder>();

        // Build container images for all project resources in the application
        await using (var buildStep = await reporter.CreateStepAsync(
            "Build container images", context.CancellationToken))
        {
            // Find all resources that need container images
            var projectResources = context.Model.Resources
                .OfType<ProjectResource>()
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
                    $"Building {projectResources.Count} container image(s)", context.CancellationToken);

                // Build all the container images
                await imageBuilder.BuildImagesAsync(
                    projectResources, buildOptions, context.CancellationToken);

                await buildTask.SucceedAsync(
                    $"Built {projectResources.Count} image(s) successfully", context.CancellationToken);
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
                "Write main.bicep", context.CancellationToken);

            // Write file to context.OutputPath …
            await bicepTask.SucceedAsync(
                $"main.bicep at {context.OutputPath}", context.CancellationToken);

            await manifestStep.SucceedAsync("Manifests ready", context.CancellationToken);
        }

        // Complete the publishing operation
        await reporter.CompletePublishAsync(
            completionMessage: "Publishing pipeline completed successfully",
            completionState: CompletionState.Completed,
            cancellationToken: context.CancellationToken);
    }
    // </publish>

    // <deploy>
    private static async Task DeployAsync(DeployingContext context)
    {
        var reporter = context.ActivityReporter;

        await using (var deployStep = await reporter.CreateStepAsync(
            "Deploy to target environment", context.CancellationToken))
        {
            var applyTask = await deployStep.CreateTaskAsync(
                "Apply Kubernetes manifests", context.CancellationToken);

            // Simulate deploying to Kubernetes cluster
            await Task.Delay(1_000, context.CancellationToken);

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
    // </deploy>
}

