public static partial class Program
{
    public static IResourceBuilder<ProjectResource> WithKestrelConfig(IDistributedApplicationBuilder builder)
    {
        var project =
        // <kestrel>
        builder.AddProject<Projects.Networking_ApiService>(
            name: "apiservice",
            configure: static project =>
            {
                project.ExcludeLaunchProfile = true;
                project.ExcludeKestrelEndpoints = false;
            })
            .WithHttpsEndpoint();
        // </kestrel>

        return project;
    }
}
