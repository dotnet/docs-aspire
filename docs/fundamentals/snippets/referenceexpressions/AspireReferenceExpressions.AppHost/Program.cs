var builder = DistributedApplication.CreateBuilder(args);

var secretKey = builder.AddParameter("secretkey", secret: true);

var connectionString = builder.AddConnectionString("composedconnectionstring", ReferenceExpression.Create($"Endpoint=https://api.contoso.com/v1;Key={secretKey}"));

var postgres = builder.AddPostgres("postgres");
var database = postgres.AddDatabase("db");

var pgConnectionString = builder.AddConnectionString("pgdatabase", ReferenceExpression.Create($"{database};Include Error Details=true"));

builder.AddProject<Projects.AspireReferenceExpressions_Web>("web")
       .WithReference(pgConnectionString)
       .WaitFor(pgConnectionString);

builder.AddProject<Projects.AspireReferenceExpressions_API>("api")
       .WithReference(connectionString)
       .WaitFor(connectionString);

builder.Build().Run();
