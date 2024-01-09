public static partial class Program
{
    public static void EnvVarPort(IDistributedApplicationBuilder builder)
    {
        // <envvarport>
        builder.AddNpmApp("frontend", "../NodeFrontend", "watch")
               .WithServiceBinding(hostPort: 5067, scheme: "http", env: "PORT");
        // </envvarport>
    }
}
