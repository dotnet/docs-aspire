using Aspire.Hosting.Azure;

var builder = DistributedApplication.CreateBuilder(args);

// <secrets>
var cosmos = builder.AddBicepTemplate("cosmos", "../infra/cosmosdb.bicep")
    .WithParameter("databaseAccountName", "db-example")
    .WithParameter(AzureBicepResource.KnownParameters.KeyVaultName)
    .WithParameter("databases", ["33", "111"]);

var connectionString =
    cosmos.GetSecretOutput("connectionString");

builder.AddProject<Projects.WebHook_Api>("api")
    .WithEnvironment(
        "ConnectionStrings__cosmos",
        connectionString);
// </secrets>

builder.Build().Run();
