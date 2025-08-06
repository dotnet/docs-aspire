// IServicePlugin.cs
public interface IServicePlugin
{
    void ConfigureServices(IHostApplicationBuilder builder);
}

// DatabasePlugin.cs
public class DatabasePlugin : IServicePlugin
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<MyDbContext>("postgres");
    }
}

// Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// Register plugins
var plugins = new List<IServicePlugin>
{
    new DatabasePlugin(),
    // Add other plugins as needed
};

foreach (var plugin in plugins)
{
    plugin.ConfigureServices(builder);
}

var app = builder.Build();