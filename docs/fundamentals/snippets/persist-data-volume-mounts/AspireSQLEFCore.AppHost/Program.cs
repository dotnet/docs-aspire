var builder = DistributedApplication.CreateBuilder(args);

// Retrieve the password you created from user secrets
var password = builder.Configuration["samplepassword"];

// Persist a SQL Server database to a named volume mount
var sql = builder.AddSqlServerContainer("sql", password)
    .WithVolumeMount("VolumeMount.sqlserver.data", "/var/opt/mssql", VolumeMountType.Named)
    .AddDatabase("sqldata");

// Persist a PostgreSQL database to a named volume
var postgres = builder.AddPostgresContainer("pg", password: password)
    .WithVolumeMount("VolumeMount.postgres.data", "/var/lib/postgresql/data", VolumeMountType.Named)
    .AddDatabase("postgresdb");

// Persist Azure Storage data to a named volume
var storage = builder.AddAzureStorage("Storage")
    .UseEmulator()
    .WithAnnotation(new VolumeMountAnnotation("VolumeMount.azurite.data", "/data", VolumeMountType.Named));

var blobs = storage.AddBlobs("BlobConnection");

builder.AddProject<Projects.AspireSQLEFCore>("aspiresql")
    .WithReference(sql)
    .WithReference(postgres)
    .WithReference(blobs);

builder.Build().Run();
