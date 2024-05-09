var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.VolumeMounts_ApiService>("apiservice");

var mode = Enum.TryParse(args[0], true, out ExampleMode result)
    ? result
    : ExampleMode.Implicit;

var sql = mode switch
{
    ExampleMode.Explicit => ExplicitStable(builder),
    _ => Implicit(builder),
};

var password = builder.Configuration["Parameters:sql-password"];

var postgresDatabase = builder.AddPostgres("postgres")
    .WithDataVolume()
    .AddDatabase("postgres");

var blobs = builder.AddAzureStorage("storage")
    .RunAsEmulator(emulator => emulator.WithDataVolume())
    .AddBlobs("blobs");

builder.AddProject<Projects.VolumeMounts_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WithReference(sql)
    .WithReference(postgresDatabase)
    .WithReference(blobs);

builder.Build().Run();

file enum ExampleMode { Implicit, Explicit };
