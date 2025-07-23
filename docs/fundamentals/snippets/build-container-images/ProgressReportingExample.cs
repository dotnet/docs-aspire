using Aspire.Hosting.Publishing;

namespace Examples.ContainerBuilding;

public static class ProgressReportingExample
{
    /// <summary>
    /// Demonstrates comprehensive progress reporting during a publishing operation
    /// </summary>
    public static async Task ReportProgressAsync(PublishingContext context)
    {
        var reporter = context.ActivityReporter;

        // Step 1: Build container images with detailed progress
        await using (var buildStep = await reporter.CreateStepAsync(
            "Build container images", context.CancellationToken))
        {
            var projectResources = context.Model.Resources
                .OfType<ProjectResource>()
                .ToList();

            foreach (var project in projectResources)
            {
                var projectTask = await buildStep.CreateTaskAsync(
                    $"Building {project.Name} image", context.CancellationToken);
                
                try
                {
                    // Simulate building the project image
                    await SimulateProjectBuildAsync(project.Name);
                    await projectTask.SucceedAsync(
                        $"{project.Name} image built successfully", context.CancellationToken);
                }
                catch (Exception ex)
                {
                    await projectTask.FailAsync(
                        $"Failed to build {project.Name}: {ex.Message}", context.CancellationToken);
                    throw; // Re-throw to fail the overall operation
                }
            }

            await buildStep.SucceedAsync(
                $"All {projectResources.Count} images built successfully", context.CancellationToken);
        }

        // Step 2: Generate manifests with warning handling
        await using (var manifestStep = await reporter.CreateStepAsync(
            "Generate deployment manifests", context.CancellationToken))
        {
            var bicepTask = await manifestStep.CreateTaskAsync(
                "Generate Bicep templates", context.CancellationToken);
            
            // Simulate some issues that result in warnings
            var hasWarnings = await SimulateManifestGenerationAsync();
            
            if (hasWarnings)
            {
                await bicepTask.WarnAsync(
                    "Manifests generated with warnings - check output", context.CancellationToken);
            }
            else
            {
                await bicepTask.SucceedAsync(
                    "Bicep templates generated successfully", context.CancellationToken);
            }
            
            await manifestStep.SucceedAsync("Manifest generation completed", context.CancellationToken);
        }

        // Complete the entire publishing operation
        await reporter.CompletePublishAsync(
            completionMessage: "Publishing completed with all steps successful",
            completionState: CompletionState.Completed,
            cancellationToken: context.CancellationToken);
    }

    /// <summary>
    /// Demonstrates error handling and failure reporting
    /// </summary>
    public static async Task ReportProgressWithErrorAsync(PublishingContext context)
    {
        var reporter = context.ActivityReporter;

        await using (var buildStep = await reporter.CreateStepAsync(
            "Build container images", context.CancellationToken))
        {
            var buildTask = await buildStep.CreateTaskAsync(
                "Building API service", context.CancellationToken);
            
            try
            {
                // Simulate a build failure
                throw new InvalidOperationException("Docker runtime not available");
            }
            catch (Exception ex)
            {
                // Report the failure with details
                await buildTask.FailAsync(
                    $"Build failed: {ex.Message}", context.CancellationToken);
                
                // Fail the overall step
                await buildStep.FailAsync(
                    "Container image build failed", context.CancellationToken);
                
                // Complete the publishing operation with error state
                await reporter.CompletePublishAsync(
                    completionMessage: "Publishing failed due to build errors",
                    completionState: CompletionState.Error,
                    cancellationToken: context.CancellationToken);
                
                throw; // Re-throw to propagate the error
            }
        }
    }

    private static async Task SimulateProjectBuildAsync(string projectName)
    {
        // Simulate different build times for different projects
        var delay = projectName.Contains("api", StringComparison.OrdinalIgnoreCase) ? 2000 : 1500;
        await Task.Delay(delay);
    }

    private static async Task<bool> SimulateManifestGenerationAsync()
    {
        await Task.Delay(500); // Simulate work
        
        // Randomly return true to simulate warnings
        return Random.Shared.Next(0, 2) == 1;
    }
}
