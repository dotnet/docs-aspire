public static partial class Program
{
    public static void OmitHostPort(IDistributedApplicationBuilder builder)
    {
        // <omithostport>
        builder.AddProject<Projects.Networking_Frontend>("frontend")
               .WithServiceBinding(scheme: "http");
        // </omithostport>
    }
}
