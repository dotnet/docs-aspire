public sealed class FakeResource(string name) : IResource
{
    string IResource.Name => name;

    ResourceAnnotationCollection IResource.Annotations { get; } = [];
}
