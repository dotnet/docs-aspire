public static partial class Program
{
    public static void ContainerPort(IDistributedApplicationBuilder builder)
    {
        // <containerport>
        builder.AddContainer("frontend", "mcr.microsoft.com/dotnet/samples", "aspnetapp")
               .WithHttpEndpoint(port: 8000, targetPort: 8080);
        // </containerport>
    }
}
