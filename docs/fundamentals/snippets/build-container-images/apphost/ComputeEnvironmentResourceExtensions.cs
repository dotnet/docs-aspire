using System.Diagnostics.CodeAnalysis;

[Experimental("ASPIRECOMPUTE001")]
public static class ComputeEnvironmentResourceExtensions
{
    public static IResourceBuilder<ComputeEnvironmentResource> AddComputeEnvironment(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name)
    {
        var resource = new ComputeEnvironmentResource(name);

        return builder.AddResource(resource);
    }
}
