var builder = DistributedApplication.CreateBuilder(args);

// <secretkey>
var secretKey = builder.AddParameter("secretkey", secret: true);

var connectionString = builder.AddConnectionString(
    "composedconnectionstring",
    ReferenceExpression.Create($"Endpoint=https://api.contoso.com/v1;Key={secretKey}"));

builder.AddProject<Projects.AspireReferenceExpressions_API>("api")
       .WithReference(connectionString)
       .WaitFor(connectionString);
// </secretkey>

// <postgresappend>
var postgres = builder.AddPostgres("postgres");
var database = postgres.AddDatabase("db");

var pgConnectionString = builder.AddConnectionString(
    "pgdatabase",
    ReferenceExpression.Create($"{database};Include Error Details=true"));

builder.AddProject<Projects.AspireReferenceExpressions_Web>("web")
       .WithReference(pgConnectionString)
       .WaitFor(pgConnectionString);
// </postgresappend>

builder.Build().Run();
