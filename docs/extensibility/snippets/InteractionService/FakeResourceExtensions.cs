namespace Aspire.Hosting;

public static class FakeResourceExtensions
{
    public static IResourceBuilder<FakeResource> AddFakeResource(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name)
    {
        var fakeResource = new FakeResource(name);

        return builder.AddResource(fakeResource)
            .WithInitialState(new()
            {
                ResourceType = "Fake Resource",
                State = KnownResourceStates.Running,
                Properties =
                [
                    new(CustomResourceKnownProperties.Source, "Fake")
                ]
            })
            .ExcludeFromManifest();
    }

    public static IResourceBuilder<FakeResource> WithDeployment(
        this IResourceBuilder<FakeResource> builder,
        Func<DeployingContext, Task> callback)
    {
        builder.WithAnnotation(new DeployingCallbackAnnotation(callback));

        return builder;
    }
}