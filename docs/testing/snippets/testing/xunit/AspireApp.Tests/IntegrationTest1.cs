namespace AspireApp.Tests;

public class IntegrationTest1
{
    [Fact]
    public async Task GetWebResourceRootReturnsOkStatusCode()
    {
        // Arrange
        var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.AspireApp_AppHost>();

        builder.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        // To output logs to the xUnit.net ITestOutputHelper, 
        // consider adding a package from https://www.nuget.org/packages?q=xunit+logging

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
