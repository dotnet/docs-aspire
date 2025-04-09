var builder = DistributedApplication.CreateBuilder(args);

var acaEnv = builder.AddAzureContainerAppEnvironment("aca-env")
    .WithAzdResourceNaming();

acaEnv.ConfigureInfrastructure(infra =>
{
    var resources = infra.GetProvisionableResources();


});

builder.Build().Run();
