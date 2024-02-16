public static partial class Program
{
    public static void EnvVarPort(IDistributedApplicationBuilder builder)
    {
        // <envvarport>
        builder.AddNpmApp("frontend", "../NodeFrontend", "watch")
               .WithHttpEndpoint(hostPort: 5067, env: "PORT");
        // </envvarport>
    }
}
