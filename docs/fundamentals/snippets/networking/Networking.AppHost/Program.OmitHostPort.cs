namespace Networking.AppHost;

public static partial class Program
{
    public static void OmitHostPort(DistributedApplicationBuilder builder)
    {
        // <omithostport>
        builder.AddProject<Projects.Networking_Frontend>("frontend")
               .WithServiceBinding(scheme: "http");
        // </omithostport>
    }
}
