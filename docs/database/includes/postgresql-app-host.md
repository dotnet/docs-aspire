---
ms.topic: include
---

The PostgreSQL hosting integration models various PostgreSQL resources as the following types.

- <xref:Aspire.Hosting.ApplicationModel.PostgresServerResource>
- <xref:Aspire.Hosting.ApplicationModel.PostgresDatabaseResource>
- <xref:Aspire.Hosting.Postgres.PgAdminContainerResource>
- <xref:Aspire.Hosting.Postgres.PgWebContainerResource>

To access these types and APIs for expressing them as resources in your [app host](xref:dotnet/aspire/app-host) project, install the [📦 Aspire.Hosting.PostgreSQL](https://www.nuget.org/packages/Aspire.Hosting.PostgreSQL) NuGet package:

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.PostgreSQL
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.PostgreSQL"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add PostgreSQL server resource

In your app host project, call <xref:Aspire.Hosting.PostgresBuilderExtensions.AddPostgres*> on the `builder` instance to add a PostgreSQL server resource then call <xref:Aspire.Hosting.PostgresBuilderExtensions.AddDatabase*> on the `postgres` instance to add a database resource as shown in the following example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var postgresdb = postgres.AddDatabase("postgresdb");

var exampleProject = builder.AddProject<Projects.ExampleProject>()
                            .WithReference(postgresdb);

// After adding all resources, run the app...
```

When .NET Aspire adds a container image to the app host, as shown in the preceding example with the `docker.io/library/postgres` image, it creates a new PostgreSQL server instance on your local machine. A reference to your PostgreSQL server and database instance (the `postgresdb` variable) are used to add a dependency to the `ExampleProject`.

When adding a database resource to the app model, the database is created if it doesn't already exist. The creation of the database relies on the [app host eventing APIs](../../app-host/eventing.md), specifically <xref:Aspire.Hosting.ApplicationModel.ResourceReadyEvent>. In other words, when the `postgres` resource is _ready_, the event is raised and the database resource is created.

The PostgreSQL server resource includes default credentials with a `username` of `"postgres"` and randomly generated `password` using the <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.CreateDefaultPasswordParameter*> method.

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` named `"messaging"`. For more information, see [Container resource lifecycle](../../fundamentals/orchestrate-resources.md#container-resource-lifecycle).

> [!TIP]
> If you'd rather connect to an existing PostgreSQL server, call <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> instead. For more information, see [Reference existing resources](../../fundamentals/app-host-overview.md#reference-existing-resources).

### Add PostgreSQL resource with database scripts

By default, when you add a <xref:Aspire.Hosting.ApplicationModel.PostgresDatabaseResource>, it relies on the following script to create the database:

```sql
CREATE DATABASE "<QUOTED_DATABASE_NAME>"
```

<!-- TODO: Use xref here when available

To alter the default script, chain a call to the <xref:Aspire.Hosting.PostgresBuilderExtensions.WithCreationScript*> method on the database resource builder:

-->

To alter the default script, chain a call to the `WithCreationScript` method on the database resource builder:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");

var databaseName = "app_db";
var creationScript = $$"""
    -- Create the database
    CREATE DATABASE {{databaseName}};

    """;

var db = postgres.AddDatabase(databaseName)
                 .WithCreationScript(creationScript);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(db)
       .WaitFor(db);

// After adding all resources, run the app...
```

The preceding example creates a database named `app_db`. The script is run when the database resource is created. The script is passed as a string to the `WithCreationScript` method, which is then run in the context of the PostgreSQL resource.

> [!NOTE]
> The connect to a database command (`\c`) isn't supported when using the creation script.

### Add PostgreSQL pgAdmin resource

When adding PostgreSQL resources to the `builder` with the `AddPostgres` method, you can chain calls to <xref:Aspire.Hosting.PostgresBuilderExtensions.WithPgAdmin*> to add the [**dpage/pgadmin4**](https://www.pgadmin.org/) container. This container is a cross-platform client for PostgreSQL databases, that serves a web-based admin dashboard. Consider the following example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                      .WithPgAdmin();

var postgresdb = postgres.AddDatabase("postgresdb");

var exampleProject = builder.AddProject<Projects.ExampleProject>()
                            .WithReference(postgresdb);

// After adding all resources, run the app...
```

The preceding code adds a container based on the `docker.io/dpage/pgadmin4` image. The container is used to manage the PostgreSQL server and database resources. The `WithPgAdmin` method adds a container that serves a web-based admin dashboard for PostgreSQL databases.

#### Configure the pgAdmin host port

To configure the host port for the pgAdmin container, call the <xref:Aspire.Hosting.PostgresBuilderExtensions.WithHostPort*> method on the PostgreSQL server resource. The following example shows how to configure the host port for the pgAdmin container:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                      .WithPgAdmin(pgAdmin => pgAdmin.WithHostPort(5050));

var postgresdb = postgres.AddDatabase("postgresdb");

var exampleProject = builder.AddProject<Projects.ExampleProject>()
                            .WithReference(postgresdb);

// After adding all resources, run the app...
```

The preceding code adds and configures the host port for the pgAdmin container. The host port is otherwise randomly assigned.

### Add PostgreSQL pgWeb resource

When adding PostgreSQL resources to the `builder` with the `AddPostgres` method, you can chain calls to <xref:Aspire.Hosting.PostgresBuilderExtensions.WithPgWeb*> to add the [**sosedoff/pgweb**](https://sosedoff.github.io/pgweb/) container. This container is a cross-platform client for PostgreSQL databases, that serves a web-based admin dashboard. Consider the following example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                      .WithPgWeb();

var postgresdb = postgres.AddDatabase("postgresdb");

var exampleProject = builder.AddProject<Projects.ExampleProject>()
                            .WithReference(postgresdb);

// After adding all resources, run the app...
```

The preceding code adds a container based on the `docker.io/sosedoff/pgweb` image. All registered <xref:Aspire.Hosting.ApplicationModel.PostgresDatabaseResource> instances are used to create a configuration file per instance, and each config is bound to the **pgweb** container bookmark directory. For more information, see [PgWeb docs: Server connection bookmarks](https://github.com/sosedoff/pgweb/wiki/Server-Connection-Bookmarks).

#### Configure the pgWeb host port

To configure the host port for the pgWeb container, call the <xref:Aspire.Hosting.PostgresBuilderExtensions.WithHostPort*> method on the PostgreSQL server resource. The following example shows how to configure the host port for the pgAdmin container:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                      .WithPgWeb(pgWeb => pgWeb.WithHostPort(5050));

var postgresdb = postgres.AddDatabase("postgresdb");

var exampleProject = builder.AddProject<Projects.ExampleProject>()
                            .WithReference(postgresdb);

// After adding all resources, run the app...
```

The preceding code adds and configures the host port for the pgWeb container. The host port is otherwise randomly assigned.

### Add PostgreSQL server resource with data volume

To add a data volume to the PostgreSQL server resource, call the <xref:Aspire.Hosting.PostgresBuilderExtensions.WithDataVolume*> method on the PostgreSQL server resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                      .WithDataVolume(isReadOnly: false);

var postgresdb = postgres.AddDatabase("postgresdb");

var exampleProject = builder.AddProject<Projects.ExampleProject>()
                            .WithReference(postgresdb);

// After adding all resources, run the app...
```

The data volume is used to persist the PostgreSQL server data outside the lifecycle of its container. The data volume is mounted at the `/var/lib/postgresql/data` path in the PostgreSQL server container and when a `name` parameter isn't provided, the name is generated at random. For more information on data volumes and details on why they're preferred over [bind mounts](#add-postgresql-server-resource-with-data-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

### Add PostgreSQL server resource with data bind mount

To add a data bind mount to the PostgreSQL server resource, call the <xref:Aspire.Hosting.PostgresBuilderExtensions.WithDataBindMount*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                      .WithDataBindMount(
                          source: @"C:\PostgreSQL\Data",
                          isReadOnly: false);

var postgresdb = postgres.AddDatabase("postgresdb");

var exampleProject = builder.AddProject<Projects.ExampleProject>()
                            .WithReference(postgresdb);

// After adding all resources, run the app...
```

[!INCLUDE [data-bind-mount-vs-volumes](../../includes/data-bind-mount-vs-volumes.md)]

Data bind mounts rely on the host machine's filesystem to persist the PostgreSQL server data across container restarts. The data bind mount is mounted at the `C:\PostgreSQL\Data` on Windows (or `/PostgreSQL/Data` on Unix) path on the host machine in the PostgreSQL server container. For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

### Add PostgreSQL server resource with init bind mount

To add an init bind mount to the PostgreSQL server resource, call the <xref:Aspire.Hosting.PostgresBuilderExtensions.WithInitBindMount*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                      .WithInitBindMount(@"C:\PostgreSQL\Init");

var postgresdb = postgres.AddDatabase("postgresdb");

var exampleProject = builder.AddProject<Projects.ExampleProject>()
                            .WithReference(postgresdb);

// After adding all resources, run the app...
```

The init bind mount relies on the host machine's filesystem to initialize the PostgreSQL server database with the containers _init_ folder. This folder is used for initialization, running any executable shell scripts or _.sql_ command files after the _postgres-data_ folder is created. The init bind mount is mounted at the `C:\PostgreSQL\Init` on Windows (or `/PostgreSQL/Init` on Unix) path on the host machine in the PostgreSQL server container.

### Add PostgreSQL server resource with parameters

When you want to explicitly provide the username and password used by the container image, you can provide these credentials as parameters. Consider the following alternative example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("username", secret: true);
var password = builder.AddParameter("password", secret: true);

var postgres = builder.AddPostgres("postgres", username, password);
var postgresdb = postgres.AddDatabase("postgresdb");

var exampleProject = builder.AddProject<Projects.ExampleProject>()
                            .WithReference(postgresdb);

// After adding all resources, run the app...
```

For more information on providing parameters, see [External parameters](../../fundamentals/external-parameters.md).
