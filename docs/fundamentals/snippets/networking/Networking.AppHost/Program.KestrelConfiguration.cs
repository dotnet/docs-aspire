public static partial class Program
{
    public static IResourceBuilder<ProjectResource> WithKestrelConfig(IDistributedApplicationBuilder builder)
    {
        var project =
        // <hostport>
        builder.AddProject<Projects.Networking_ApiService>("apiservice")
               .WithHttpsEndpoint();
        // </hostport>

        return project;
    }
}
