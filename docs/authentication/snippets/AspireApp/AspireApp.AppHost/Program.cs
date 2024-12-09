var builder = DistributedApplication.CreateBuilder(args);

var keycloak = builder.AddKeycloak("keycloak", 8080)
                      .WithDataVolume()
                      .WithRealmImport("./Realms");

var apiService = builder.AddProject<Projects.AspireApp_ApiService>("apiservice")
                        .WithReference(keycloak)
                        .WaitFor(keycloak);

builder.AddProject<Projects.AspireApp_Web>("webfrontend")
       .WithExternalHttpEndpoints()
       .WithReference(keycloak)
       .WithReference(apiService)
       .WaitFor(apiService);

builder.Build().Run();
