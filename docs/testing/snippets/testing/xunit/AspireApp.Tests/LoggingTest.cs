using Microsoft.Extensions.Logging;

namespace AspireApp.Tests;

public class LoggingTest
{
    [Fact]
    public async Task GetWebResourceRootReturnsOkStatusCodeWithLogging()
    {
        // Arrange
        var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.AspireApp_AppHost>();

        builder.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        // Configure logging to capture test execution logs
        builder.Services.AddLogging(logging => logging
            .AddConsole() // Outputs logs to console
            .AddFilter("Default", LogLevel.Information)
            .AddFilter("Microsoft.AspNetCore", LogLevel.Warning)
            .AddFilter("Aspire.Hosting.Dcp", LogLevel.Warning));

        await using var app = await builder.BuildAsync();

        await app.StartAsync();

        // Act
        var httpClient = app.CreateHttpClient("webfrontend");

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        await app.ResourceNotifications.WaitForResourceHealthyAsync(
            "webfrontend",
            cts.Token);

        var response = await httpClient.GetAsync("/");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}