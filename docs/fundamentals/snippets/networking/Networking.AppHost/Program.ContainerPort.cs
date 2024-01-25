public static partial class Program
{
    public static void ContainerPort(IDistributedApplicationBuilder builder)
    {
        // <containerport>
        builder.AddContainer("frontend", "mcr.microsoft.com/dotnet/samples", "aspnetapp")
               .WithServiceBinding(hostPort: 8000, containerPort: 8080, scheme: "http");
        // </containerport>
    }
}
