namespace AspireApp.Tests;

public class EnvVarTests
{
    [Fact]
    public async Task WebResourceEnvVarsResolveToApiService()
    {
        // Arrange
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.AspireApp_AppHost>();

        var frontend = (IResourceWithEnvironment)appHost.Resources
            .Single(static r => r.Name == "webfrontend");

        // Act
        var envVars = await frontend.GetEnvironmentVariableValuesAsync(
            DistributedApplicationOperation.Publish);

        // Assert
        Assert.Contains(envVars, static (kvp) =>
        {
            var (key, value) = kvp;

            return key is "services__apiservice__https__0"
                && value is "{apiservice.bindings.https.url}";
        });
    }
}
