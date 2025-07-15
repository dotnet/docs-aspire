using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace AnnotationsOverview
{
    // <ServiceMetricsAnnotation>
    public sealed class ServiceMetricsAnnotation : IResourceAnnotation
    {
        public string MetricsPath { get; set; } = "/metrics";
        public int Port { get; set; } = 9090;
        public bool Enabled { get; set; } = true;
        public string[] AdditionalLabels { get; set; } = [];
    }
    // </ServiceMetricsAnnotation>

    // <ServiceMetricsExtensions>
    public static class ServiceMetricsExtensions
    {
        public static IResourceBuilder<T> WithMetrics<T>(
            this IResourceBuilder<T> builder,
            string path = "/metrics",
            int port = 9090,
            params string[] labels)
            where T : class, IResource
        {
            var annotation = new ServiceMetricsAnnotation
            {
                MetricsPath = path,
                Port = port,
                Enabled = true,
                AdditionalLabels = labels
            };

            return builder.WithAnnotation(annotation);
        }

        public static IResourceBuilder<T> WithoutMetrics<T>(
            this IResourceBuilder<T> builder)
            where T : class, IResource
        {
            return builder.WithAnnotation(new ServiceMetricsAnnotation { Enabled = false });
        }
    }
    // </ServiceMetricsExtensions>

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

    // <ServiceMetricsProcessor>
    public class ServiceMetricsProcessor
    {
        public static void ProcessMetricsConfiguration(IResource resource)
        {
            var metricsConfig = resource.Annotations
                .OfType<ServiceMetricsAnnotation>()
                .FirstOrDefault();

            if (metricsConfig?.Enabled == true)
            {
                // Configure metrics collection
                Console.WriteLine($"Enabling metrics for {resource.Name} at {metricsConfig.MetricsPath}:{metricsConfig.Port}");
                
                if (metricsConfig.AdditionalLabels.Length > 0)
                {
                    Console.WriteLine($"Additional labels: {string.Join(", ", metricsConfig.AdditionalLabels)}");
                }
            }
        }
    }
    // </ServiceMetricsProcessor>

    // <AccessingAnnotations>
    public class AnnotationInspector
    {
        public static void InspectResource(IResource resource)
        {
            // Get all annotations of a specific type
            var metricsConfigs = resource.Annotations.OfType<ServiceMetricsAnnotation>();

            // Check if a resource has a specific annotation
            bool hasMetricsConfig = resource.Annotations
                .Any(a => a is ServiceMetricsAnnotation);

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