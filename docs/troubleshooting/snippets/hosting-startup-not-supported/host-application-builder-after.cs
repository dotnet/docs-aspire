using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

// Example of modern hosting pattern that works with Aspire
public class ModernHostingExample
{
    public static void ConfigureServices()
    {
        // Program.cs equivalent
        var builder = WebApplication.CreateBuilder();

        // Add service defaults first
        // builder.AddServiceDefaults();

        // Now you can use Aspire integrations
        // builder.AddNpgsqlDbContext<MyDbContext>("postgres");

        var app = builder.Build();

        // app.MapDefaultEndpoints();
        // app.Run();
    }
}