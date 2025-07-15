using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace AnnotationsOverview
{
    // <CustomConfigAnnotation>
    public sealed class CustomConfigAnnotation : IResourceAnnotation
    {
        public string ConfigKey { get; set; } = "";
        public string ConfigValue { get; set; } = "";
        public bool IsRequired { get; set; } = true;
    }
    // </CustomConfigAnnotation>

    // <CustomConfigExtensions>
    public static class CustomConfigExtensions
    {
        public static IResourceBuilder<T> WithCustomConfig<T>(
            this IResourceBuilder<T> builder,
            string key,
            string value,
            bool isRequired = true)
            where T : class, IResource
        {
            var annotation = new CustomConfigAnnotation
            {
                ConfigKey = key,
                ConfigValue = value,
                IsRequired = isRequired
            };

            return builder.WithAnnotation(annotation);
        }
    }
    // </CustomConfigExtensions>

    // <ValidatedAnnotation>
    public sealed class ValidatedAnnotation : IResourceAnnotation
    {
        private string _value = "";

        public string Value
        {
            get => _value;
            set => _value = string.IsNullOrEmpty(value)
                ? throw new ArgumentException("Value cannot be null or empty")
                : value;
        }
    }
    // </ValidatedAnnotation>

    // <CustomConfigProcessor>
    public class CustomConfigProcessor
    {
        public static void ProcessCustomConfig(IResource resource)
        {
            var customConfigs = resource.Annotations
                .OfType<CustomConfigAnnotation>();

            foreach (var config in customConfigs)
            {
                // Process the configuration
                Console.WriteLine($"Setting {config.ConfigKey} = {config.ConfigValue}");
            }
        }
    }
    // </CustomConfigProcessor>

    // <AccessingAnnotations>
    public class AnnotationInspector
    {
        public static void InspectResource(IResource resource)
        {
            // Get all annotations of a specific type
            var customConfigs = resource.Annotations.OfType<CustomConfigAnnotation>();

            // Check if a resource has a specific annotation
            bool hasCustomConfig = resource.Annotations
                .Any(a => a is CustomConfigAnnotation);

            // Get the first annotation of a type
            var endpoint = resource.Annotations
                .OfType<EndpointAnnotation>()
                .FirstOrDefault();

            Console.WriteLine($"Resource {resource.Name} has {resource.Annotations.Count()} annotations");
        }
    }
    // </AccessingAnnotations>

    public class Program
    {
        public static void Main(string[] args)
        {
            // This is a minimal example showing annotation usage
            // In a real app host, you would use actual resources
            Console.WriteLine("Annotations overview examples");
        }
    }
}