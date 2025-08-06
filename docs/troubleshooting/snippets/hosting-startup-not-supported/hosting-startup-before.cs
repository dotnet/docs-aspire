// MyDatabaseStartup.cs
public class MyDatabaseStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // This won't work with Aspire integrations
            services.AddDbContext<MyDbContext>(options =>
                options.UseNpgsql(connectionString));
        });
    }
}