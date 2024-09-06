When adding PostgreSQL resources to the `builder` with the `AddPostgres` method, you can chain calls to `WithPgAdmin` to add the [**dpage/pgadmin4**](https://www.pgadmin.org/) container. This container is a cross-platform client for PostgreSQL databases, that serves a web-based admin dashboard. Consider the following example:

```csharp
var postgres = builder.AddPostgres("postgres")
                      .WithPgAdmin();

var postgresdb = postgres.AddDatabase("postgresdb");

var exampleProject = builder.AddProject<Projects.ExampleProject>()
                            .WithReference(postgresdb);
```
