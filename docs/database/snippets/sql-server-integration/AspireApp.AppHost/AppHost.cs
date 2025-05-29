var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
                 .WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase("database");

builder.AddProject<Projects.ExampleProject>("exampleproject")
       .WithReference(db)
       .WaitFor(db);

// After adding all resources, run the app...

builder.Build().Run();
