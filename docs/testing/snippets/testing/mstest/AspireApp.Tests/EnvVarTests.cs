using Aspire.Hosting;

namespace AspireApp.Tests;

[TestClass]
public class EnvVarTests
{
    [TestMethod]
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
        CollectionAssert.Contains(envVars,
            new KeyValuePair<string, string>(
                key: "services__apiservice__https__0",
                value: "{apiservice.bindings.https.url}"));
    }
}
