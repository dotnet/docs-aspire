When you want to explicitly provide the username and password, you can provide those as parameters. Consider the following alternative example:

```csharp
var username = builder.AddParameter("username", secret: true);
var password = builder.AddParameter("password", secret: true);

var postgres = builder.AddPostgres("postgres", username, password);

var postgresdb = postgres.AddDatabase("postgresdb");

var exampleProject = builder.AddProject<Projects.ExampleProject>()
                            .WithReference(postgresdb);
```

For more information, see [External parameters](../../fundamentals/external-parameters.md).
