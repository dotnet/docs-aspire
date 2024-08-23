When adding PostgreSQL resources to the `builder` with the `AddPostgres` method, you can chain calls to `WithPgWeb` to add the [**sosedoff/pgweb**](https://sosedoff.github.io/pgweb/) container. This container is a cross-platform client for PostgreSQL databases, that serves a web-based admin dashboard. Consider the following example:

```csharp
var postgres = builder.AddPostgres("postgres")
                      .WithPgWeb();

var postgresdb = postgres.AddDatabase("postgresdb");

var exampleProject = builder.AddProject<Projects.ExampleProject>()
                            .WithReference(postgresdb);
```

All registered `PostgresDatabaseResource` instances are used to create a configuration file per instance, and each config is bound to the **pgweb** container bookmark directory. For more information, see [PgWeb docs: Server connection bookmarks](https://github.com/sosedoff/pgweb/wiki/Server-Connection-Bookmarks).
