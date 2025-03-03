using Aspire.Hosting;

namespace AspireApp.Tests;

public class EnvVarTests
{
    [Test]
    public async Task WebResourceEnvVarsResolveToApiService()
    {
        // Arrange
        var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.AspireApp_AppHost>();

        var frontend = builder.CreateResourceBuilder<ProjectResource>("webfrontend");

        // Act
        var envVars = await frontend.Resource.GetEnvironmentVariableValuesAsync(
            DistributedApplicationOperation.Publish);

        // Assert
        Assert.That(envVars, Does.Contain(
            new KeyValuePair<string, string>(
                key: "services__apiservice__https__0",
                value: "{apiservice.bindings.https.url}")));
    }
}
