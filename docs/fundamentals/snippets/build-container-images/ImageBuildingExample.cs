using System.Diagnostics.CodeAnalysis;
using Aspire.Hosting.Publishing;
using Microsoft.Extensions.DependencyInjection;

namespace Examples.ContainerBuilding;

[Experimental("ASPIRECOMPUTE001")]
public static class ImageBuildingExample
{
    /// <summary>
    /// Demonstrates how to build container images programmatically during publishing
    /// </summary>
    public static async Task BuildImagesAsync(PublishingContext context)
    {
        // Get the image builder service from dependency injection
        var imageBuilder = context.Services.GetRequiredService<IResourceContainerImageBuilder>();

        // Find all project resources that can be built as container images
        var projectResources = context.Model.Resources
            .OfType<ProjectResource>()
            .Where(r => r.Annotations.OfType<ContainerImageAnnotation>().Any())
            .ToList();

        if (!projectResources.Any())
        {
            return; // No projects to build
        }

        // Configure build options for the container images
        var buildOptions = new ContainerBuildOptions
        {
            // Use OCI format for better compatibility
            ImageFormat = ContainerImageFormat.Oci,

            // Target Linux x64 for cloud deployment
            TargetPlatform = ContainerTargetPlatform.LinuxAmd64,

            // Specify where to save the built images
            OutputPath = Path.Combine(context.OutputPath, "images")
        };

        // Build all project images in a single operation
        await imageBuilder.BuildImagesAsync(
            projectResources,
            buildOptions,
            context.CancellationToken);
    }

    /// <summary>
    /// Demonstrates building a single resource image with custom options
    /// </summary>
    public static async Task BuildSingleImageAsync(
        PublishingContext context,
        ProjectResource projectResource)
    {
        var imageBuilder = context.Services.GetRequiredService<IResourceContainerImageBuilder>();

        // Configure options for this specific image
        var buildOptions = new ContainerBuildOptions
        {
            ImageFormat = ContainerImageFormat.Docker,
            TargetPlatform = ContainerTargetPlatform.LinuxArm64, // ARM64 for Apple Silicon
            OutputPath = context.OutputPath
        };

        // Build just this one image
        await imageBuilder.BuildImageAsync(
            projectResource,
            buildOptions,
            context.CancellationToken);
    }
}
