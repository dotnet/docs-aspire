namespace Networking.AppHost;

public static partial class Program
{
    public static void EnvVarPort(DistributedApplicationBuilder builder)
    {
        // <envvarport>
        builder.AddNpmApp("frontend", "../NodeFrontend", "watch")
               .WithServiceBinding(hostPort: 5067, scheme: "http", env: "PORT");
        // </envvarport>
    }
}
