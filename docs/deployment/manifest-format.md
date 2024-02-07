---
title: .NET Aspire manifest format for deployment tool builders
description: Learn about the .NET Aspire manifest format in this comprehensive deployment tool builder guide.
ms.date: 12/20/2023
ms.topic: reference
---

# .NET Aspire manifest format for deployment tool builders

In this article, you learn about the .NET Aspire manifest format. This article serves as a reference guide for deployment tool builders, aiding in the creation of tooling to deploy .NET Aspire applications on specific hosting platforms, whether on-premises or in the cloud.

.NET Aspire simplifies the local development experience by helping to manage interdependencies between application components. To help simplify the deployment of applications, .NET Aspire projects can generate a "manifest" of all the resources defined as a JSON formatted file.

## Generate a manifest

A valid .NET Aspire application is required to generate a manifest. To get started, create
a .NET Aspire application using the `aspire-starter` .NET template:

```dotnetcli
dotnet new aspire-starter --use-redis-cache `
    -o AspireApp && `
    cd AspireApp
```

Manifest generation is achieved by running `dotnet build` with a special target:

```dotnetcli
dotnet run --project AspireApp.AppHost\AspireApp.AppHost.csproj `
    -- `
    --publisher manifest `
    --output-path aspire-manifest.json
```

For more information, see [dotnet build](/dotnet/core/tools/dotnet-build). The previous command produces the following output:

```Output
Building...
info: Aspire.Hosting.Publishing.ManifestPublisher[0]
      Published manifest to: .\AspireApp.AppHost\aspire-manifest.json
```

The file generated is the .NET Aspire manifest and is used by tools to support deploying into target cloud environments.

## Basic manifest format

Publishing the manifest from the default starter template for .NET Aspire produces the following JSON output:

```json
{
  "resources": {
    "cache": {
      "type": "redis.v0"
    },
    "apiservice": {
      "type": "project.v0",
      "path": "../AspireApp.ApiService/AspireApp.ApiService.csproj",
      "env": {
        "OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES": "true",
        "OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EVENT_LOG_ATTRIBUTES": "true"
      },
      "bindings": {
        "http": {
          "scheme": "http",
          "protocol": "tcp",
          "transport": "http"
        },
        "https": {
          "scheme": "https",
          "protocol": "tcp",
          "transport": "http"
        }
      }
    },
    "webfrontend": {
      "type": "project.v0",
      "path": "../AspireApp.Web/AspireApp.Web.csproj",
      "env": {
        "OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES": "true",
        "OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EVENT_LOG_ATTRIBUTES": "true",
        "ConnectionStrings__cache": "{cache.connectionString}",
        "services__apiservice__0": "{apiservice.bindings.http.url}",
        "services__apiservice__1": "{apiservice.bindings.https.url}"
      },
      "bindings": {
        "http": {
          "scheme": "http",
          "protocol": "tcp",
          "transport": "http"
        },
        "https": {
          "scheme": "https",
          "protocol": "tcp",
          "transport": "http"
        }
      }
    }
  }
}
```

The manifest format JSON consists of a single object called `resources`, which contains a property for each resource specified in _Program.cs_ (the `name` argument for each name is used as the property for each of the child resource objects in JSON).

### Connection string and binding references

In the previous example, there are two project resources and one Redis cache resource. The _webfrontend_ depends on both the _apiservice_ (project) and _cache_ (Redis) resources.

This dependency is known because the environment variables for the _webfrontend_ contain placeholders that reference the two other resources:

```json
"env": {
  // ... other environment variables omitted for clarity
  "ConnectionStrings__cache": "{cache.connectionString}",
  "services__apiservice__0": "{apiservice.bindings.http.url}",
  "services__apiservice__1": "{apiservice.bindings.https.url}"
},
```

The `apiservice` resource is referenced by `webfrontend` using the call `WithReference(apiservice)` in the app host _Program.cs_ file and `redis` is referenced using the call `WithReference(cache)`:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var apiservice = builder.AddProject<Projects.AspireApp_ApiService>("apiservice");

builder.AddProject<Projects.AspireApp_Web>("webfrontend")
       .WithReference(cache)
       .WithReference(apiservice);

builder.Build().Run();
```

References between project resource types result in service discovery variables being injected into the referencing project. References to
well know reference types such as Redis result in connection strings being injected.

:::image type="content" source="media/manifest-placeholder-strings.png" lightbox="media/manifest-placeholder-strings.png" alt-text="A diagram showing which resources contribute to which corresponding placeholder strings.":::

For more information on how resources in the app model and references between them work, see, [.NET Aspire orchestration overview](../fundamentals/app-host-overview.md).

### Placeholder string structure

Placeholder strings reference the structure of the .NET Aspire manifest:

:::image type="content" source="media/placeholder-mappings.png" lightbox="media/placeholder-mappings.png" alt-text="A diagram showing how the manifest JSON structure maps to placeholder strings.":::

The final segment of the placeholder string (`url` in this case) is generated by the tool processing the manifest. There are several suffixes that could be used on the placeholder string:

- `connectionString`: For well-known resource types such as Redis. Deployment tools translate the resource in the most appropriate infrastructure for the target cloud environment and then produce a .NET Aspire compatible connection string for the consuming application to use. On `container.v0` resources the `connectionString` field may be present and specified explicitly. This is to support scenarios where a container resource type is referenced using the <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> extension but is desired to be hosted explicitly as a container.
- `url`: For service-to-service references where a well-formed URL is required. The deployment tool produces the `url` based on the scheme, protocol, and transport defined in the manifest and the underlying compute/networking topology that was deployed.
- `host`: The host segment of the URL.
- `port`: The port segment of the URL.

## Resource types

Each resource has a `type` field. When a deployment tool reads the manifest, it should read
the type to verify whether it can correctly process the manifest. During the .NET Aspire preview period, all resource types have a `v0` suffix to indicate that they're subject to change. As
.NET Aspire approaches release a `v1` suffix will be used to signify that the structure of the
manifest for that resource type should be considered stable (subsequent updates increment
the version number accordingly).

### Common resource fields

The `type` field is the only field that is common across all resource types, however, the
`project.v0`, `container.v0`, and `executable.v0` resource types also share the `env` and `bindings`
fields.

> [!NOTE]
> The `executable.v0` resource type isn't fully implemented in the manifest due to its lack of
utility in deployment scenarios. For more information on containerizing executables, see [Dockerfile resource types](#dockerfile-resource-types).

The `env` field type is a basic key/value mapping where the values might contain [_placeholder strings_](#placeholder-string-structure).

Bindings are specified in the `bindings` field with each binding contained within its own field under the `bindings` JSON object. The fields committed by the .NET Aspire include:

- `scheme`: One of the following values `tcp`, `udp`, `http`, or `https`.
- `protocol`: One of the following values `tcp` or `udp`
- `transport`: Same as `scheme`, but used to disambiguate between `http` and `http2`.
- `containerPort`: Optional, if omitted defaults to port 80.

### The `inputs` field

Some resources generate an `inputs` field. This field is used to specify input parameters for the resource. The `inputs` field is a JSON object where each property is an input parameter that's used in placeholder structure resolution. Resources that have a `connectionString`, for example, might use the `inputs` field to specify a `password` for the connection string:

```json
"connectionString": "Host={<resourceName>.bindings.tcp.host};Port={<resourceName>.bindings.tcp.port};Username=admin;Password={<resourceName>.inputs.password};"
```

The connection string placeholder references the `password` input parameter from the `inputs` field:

```json
"inputs": {
  "password": {
    "type": "string",
    "secret": true,
    "default": {
      "generate": {
        "minLength": 10
      }
    }
  }
}
```

The preceding JSON snippet shows the `inputs` field for a resource that has a `connectionString` field. The `password` input parameter is a string type and is marked as a secret. The `default` field is used to specify a default value for the input parameter. In this case, the default value is generated using the `generate` field, with random string of a minimum length.

## Built-in resources

The following table is a list of resource types that are explicitly generated by .NET Aspire and
extensions developed by the .NET Aspire team:

### Cloud agnostic resource types

These resources are available in the _Aspire.Hosting_ package.

| App model usage | Manifest resource type | Heading link |
|--|--|--|
| <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.AddContainer%2A> | `container.v0` | [Container resource type](#container-resource-type) |
| <xref:Aspire.Hosting.ExecutableResourceBuilderExtensions.AsDockerfileInManifest%2A> | `dockerfile.v0` | [Dockerfile resource types](#dockerfile-resource-types) |
| <xref:Aspire.Hosting.MongoDBBuilderExtensions.AddDatabase%2A> | `mongodb.database.v0` | [MongoDB Server resource types](#mongodb-resource-types) |
| <xref:Aspire.Hosting.MongoDBBuilderExtensions.AddMongoDB%2A> | `mongodb.server.v0` | [MongoDB resource types](#mongodb-resource-types) |
| <xref:Aspire.Hosting.MongoDBBuilderExtensions.AddMongoDBContainer%2A> | `container.v0` | [MongoDB Server resource types](#mongodb-resource-types) |
| <xref:Aspire.Hosting.MySqlBuilderExtensions.AddDatabase%2A> | `mysql.database.v0` | [MySQL Server resource types](#mysql-resource-types) |
| <xref:Aspire.Hosting.MySqlBuilderExtensions.AddMySql%2A> | `mysql.server.v0` | [MySQL resource types](#mysql-resource-types) |
| <xref:Aspire.Hosting.MySqlBuilderExtensions.AddMySqlContainer%2A> | `container.v0` | [MySQL Server resource types](#mysql-resource-types) |
| <xref:Aspire.Hosting.PostgresBuilderExtensions.AddDatabase%2A> | `postgres.database.v0` | [Postgres resource types](#postgres-resource-types) |
| <xref:Aspire.Hosting.PostgresBuilderExtensions.AddPostgres%2A> | `postgres.server.v0` | [Postgres resource types](#postgres-resource-types) |
| <xref:Aspire.Hosting.PostgresBuilderExtensions.AddPostgresContainer%2A> | `container.v0` | [Postgres resource types](#postgres-resource-types) |
| <xref:Aspire.Hosting.ProjectResourceBuilderExtensions.AddProject%2A> | `project.v0` | [Project resource type](#project-resource-type) |
| <xref:Aspire.Hosting.RabbitMQBuilderExtensions.AddRabbitMQ%2A> | `rabbitmq.server.v0` | [RabbitMQ resource types](#rabbitmq-resource-types) |
| <xref:Aspire.Hosting.RabbitMQBuilderExtensions.AddRabbitMQContainer%2A> | `container.v0` | [RabbitMQ resource types](#rabbitmq-resource-types) |
| <xref:Aspire.Hosting.RedisBuilderExtensions.AddRedis%2A> | `redis.v0` | [Redis resource type](#redis-resource-type) |
| <xref:Aspire.Hosting.RedisBuilderExtensions.AddRedisContainer%2A> | `container.v0` | [Redis resource type](#redis-resource-type) |
| <xref:Aspire.Hosting.SqlServerBuilderExtensions.AddDatabase%2A> | `sqlserver.database.v0` | [SQL Server resource types](#sql-server-resource-types) |
| <xref:Aspire.Hosting.SqlServerBuilderExtensions.AddSqlServer%2A> | `sqlserver.server.v0` | [SQL Server resource types](#sql-server-resource-types) |
| <xref:Aspire.Hosting.SqlServerBuilderExtensions.AddSqlServerContainer%2A> | `container.v0` | [SQL Server resource types](#sql-server-resource-types) |

#### Project resource type

Example code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);
var apiservice = builder.AddProject<Projects.AspireApp_ApiService>("apiservice");
```

Example manifest:

```json
"apiservice": {
    "type": "project.v0",
    "path": "../AspireApp.ApiService/AspireApp.ApiService.csproj",
    "env": {
        "OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES": "true",
        "OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EVENT_LOG_ATTRIBUTES": "true"
    },
    "bindings": {
        "http": {
            "scheme": "http",
            "protocol": "tcp",
            "transport": "http"
        },
        "https": {
            "scheme": "https",
            "protocol": "tcp",
            "transport": "http"
        }
    }
}
```

#### Container resource type

Example code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddContainer("mycontainer", "myimage")
       .WithEnvironment("LOG_LEVEL", "WARN")
       .WithServiceBinding(3000, scheme: "http");
```

Example manifest:

```json
{
  "resources": {
    "mycontainer": {
      "type": "container.v0",
      "image": "myimage:latest",
      "env": {
        "LOG_LEVEL": "WARN"
      },
      "bindings": {
        "http": {
          "scheme": "http",
          "protocol": "tcp",
          "transport": "http",
          "containerPort": 3000
        }
      }
    }
  }
}
```

#### Dockerfile resource types

Example code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddNodeApp("nodeapp", "../nodeapp/app.js")
       .WithServiceBinding(hostPort: 5031, scheme: "http", env: "PORT")
       .AsDockerfileInManifest();
```

> [!TIP]
> The <xref:Aspire.Hosting.ExecutableResourceBuilderExtensions.AsDockerfileInManifest%2A> call is required to generate the Dockerfile resource type in the manifest, and this extension method is only available on the <xref:Aspire.Hosting.ApplicationModel.ExecutableResource> type.

Example manifest:

```json
{
  "resources": {
      "nodeapp": {
        "type": "dockerfile.v0",
      "path": "../nodeapp/Dockerfile",
      "context": "../nodeapp",
      "env": {
        "NODE_ENV": "production",
        "PORT": "{nodeapp.bindings.http.port}"
      },
      "bindings": {
        "http": {
        "scheme": "http",
        "protocol": "tcp",
        "transport": "http",
        "containerPort": 5031
        }
      }
    }
  }
}
```

#### Postgres resource types

Example code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddPostgres("postgres1");

builder.AddPostgresContainer("postgres2")
       .AddDatabase("shipping");
```

Example manifest:

```json
{
  "resources": {
    "postgres1": {
      "type": "postgres.server.v0"
    },
    "postgres2": {
      "type": "container.v0",
      "image": "postgres:latest",
      "env": {
        "POSTGRES_HOST_AUTH_METHOD": "scram-sha-256",
        "POSTGRES_INITDB_ARGS": "--auth-host=scram-sha-256 --auth-local=scram-sha-256",
        "POSTGRES_PASSWORD": "{postgres2.inputs.password}"
      },
      "bindings": {
        "tcp": {
          "scheme": "tcp",
          "protocol": "tcp",
          "transport": "tcp",
          "containerPort": 5432
        }
      },
      "connectionString": "Host={postgres2.bindings.tcp.host};Port={postgres2.bindings.tcp.port};Username=postgres;Password={postgres2.inputs.password};",
      "inputs": {
        "password": {
          "type": "string",
          "secret": true,
          "default": {
            "generate": {
              "minLength": 10
            }
          }
        }
      }
    },
    "shipping": {
      "type": "postgres.database.v0",
      "parent": "postgres2"
    }
  }
}
```

#### RabbitMQ resource types

RabbitMQ has two resource types, `rabbitmq.server.v0` and `rabbitmq.connection.v0`. The following
sample shows how they're added to the app model.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddRabbitMQ("rabbitmq1");
builder.AddRabbitMQContainer("rabbitmq2");
```

The previous code produces the following manifest:

```json
{
  "resources": {
    "rabbitmq1": {
      "type": "rabbitmq.server.v0"
    },
    "rabbitmq2": {
      "type": "container.v0",
      "image": "rabbitmq:3-management",
      "env": {
        "RABBITMQ_DEFAULT_USER": "guest",
        "RABBITMQ_DEFAULT_PASS": "{rabbitmq2.inputs.password}"
      },
      "bindings": {
        "tcp": {
          "scheme": "tcp",
          "protocol": "tcp",
          "transport": "tcp",
          "containerPort": 5672
        },
        "management": {
          "scheme": "http",
          "protocol": "tcp",
          "transport": "http",
          "containerPort": 15672
        }
      },
      "connectionString": "amqp://guest:{rabbitmq2.inputs.password}@{rabbitmq2.bindings.management.host}:{rabbitmq2.bindings.management.port}",
      "inputs": {
        "password": {
          "type": "string",
          "secret": true,
          "default": {
            "generate": {
              "minLength": 10
            }
          }
        }
      }
    }
  }
}
```

#### Redis resource type

Example code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddRedis("redis1");
builder.AddRedisContainer("redis2");
```

Example manifest:

```json
{
  "resources": {
    "redis1": {
      "type": "redis.v0",
      "connectionString": "myredis:6379"
    },
    "redis2": {
      "type": "container.v0",
      "image": "redis:latest",
      "bindings": {
        "tcp": {
          "scheme": "tcp",
          "protocol": "tcp",
          "transport": "tcp",
          "containerPort": 6379
        }
      },
      "connectionString": "{redis2.bindings.tcp.host}:{redis2.bindings.tcp.port}}"
    }
  }
}
```

#### SQL Server resource types

Example code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddSqlServer("sql1");

builder.AddSqlServerContainer("sql2")
       .AddDatabase("shipping");
```

Example manifest:

```json
{
  "resources": {
    "sql1": {
      "type": "sqlserver.server.v1"
    },
    "sql2": {
      "type": "container.v0",
      "image": "mcr.microsoft.com/mssql/server:2022-latest",
      "env": {
        "ACCEPT_EULA": "Y",
        "MSSQL_SA_PASSWORD": "{sqlservercontainer.inputs.password}"
      },
      "bindings": {
        "tcp": {
          "scheme": "tcp",
          "protocol": "tcp",
          "transport": "tcp",
          "containerPort": 1433
        }
      },
      "connectionString": "Server={sql2.bindings.tcp.host},{sql2.bindings.tcp.port};User ID=sa;Password={sql2.inputs.password};TrustServerCertificate=true;",
      "inputs": {
        "password": {
          "type": "string",
          "secret": true,
          "default": {
            "generate": {
              "minLength": 10
            }
          }
        }
      }
    },
    "shipping": {
      "type": "sqlserver.database.v1",
      "parent": "sql2"
    }
  }
}
```

#### MongoDB resource types

Example code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddMongoDB("mongodb1");

builder.AddMongoDBContainer("mongodb2")
       .AddDatabase("shipping");
```

Example manifest:

```json
{
  "resources": {
    "mongodb1": {
      "type": "mongodb.server.v0"
    },
    "mongodb2": {
      "type": "container.v0",
      "image": "mongo:latest",
      "bindings": {
        "tcp": {
          "scheme": "tcp",
          "protocol": "tcp",
          "transport": "tcp",
          "containerPort": 27017
        }
      },
      "connectionString": "{mongodb2.bindings.tcp.host}:{mongodb2.bindings.tcp.port}"
    },
    "shipping": {
      "type": "mongodb.database.v1",
      "parent": "mongodb2"
    }
  }
}
```

#### MySQL resource types

Example code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddMySql("mysql1");

builder.AddMySql("mysql2")
       .AddDatabase("shipping");
```

Example manifest:

```json
{
  "resources": {
    "mysql1": {
      "type": "mysql.server.v0"
    },
    "mysql2": {
      "type": "container.v0",
      "image": "mysql:latest",
      "env": {
        "MYSQL_ROOT_PASSWORD": "{mysql2.inputs.password}"
      },
      "bindings": {
        "tcp": {
          "scheme": "tcp",
          "protocol": "tcp",
          "transport": "tcp",
          "containerPort": 3306
        }
      },
      "connectionString": "Server={mysql2.bindings.tcp.host};Port={mysql2.bindings.tcp.port};User ID=root;Password={mysql2.inputs.password}",
      "inputs": {
        "password": {
          "type": "string",
          "secret": true,
          "default": {
            "generate": {
              "minLength": 10
            }
          }
        }
      }
    },
    "shipping": {
      "type": "mysql.database.v1",
      "parent": "mysql2"
    }
  }
}
```

### Azure-specific resource types

The following resources are available in the [Aspire.Hosting.Azure](https://www.nuget.org/packages/Aspire.Hosting.Azure) NuGet package.

| App Model usage | Manifest resource type | Heading link |
|--|--|--|
| <xref:Aspire.Hosting.AzureResourceExtensions.AddAzureAppConfiguration%2A> | `azure.appconfiguration.v0` | [Azure App Configuration resource types](#azure-app-configuration-resource-type) |
| <xref:Aspire.Hosting.AzureResourceExtensions.AddAzureKeyVault%2A> | `azure.keyvault.v0` | [Azure Key Vault resource type](#azure-key-vault-resource-type) |
| <xref:Aspire.Hosting.AzureResourceExtensions.AddAzureRedis%2A> | `azure.redis.v0` | [Azure Redis resource types](#azure-redis-resource-type) |
| <xref:Aspire.Hosting.AzureResourceExtensions.AddAzureServiceBus%2A> | `azure.servicebus.v0` | [Azure Service Bus resource type](#azure-service-bus-resource-type) |
| <xref:Aspire.Hosting.AzureResourceExtensions.AddAzureSqlServer%2A> | `azure.sql.v0` | [Azure SQL resource types](#azure-sql-resource-types) |
| <xref:Aspire.Hosting.AzureResourceExtensions.AddAzureSqlServer%2A>`.AddDatabase(...)` | `azure.sql.database.v0` | [Azure SQL resource types](#azure-sql-resource-types) |
| <xref:Aspire.Hosting.AzureResourceExtensions.AddAzureStorage%2A> | `azure.storage.v0` | [Azure Storage resource types](#azure-storage-resource-types) |
| <xref:Aspire.Hosting.AzureResourceExtensions.AddBlobs%2A> | `azure.storage.blob.v0` | [Azure Storage resource types](#azure-storage-resource-types) |
| <xref:Aspire.Hosting.AzureResourceExtensions.AddQueues%2A> | `azure.storage.queue.v0` | [Azure Storage resource types](#azure-storage-resource-types) |
| <xref:Aspire.Hosting.AzureResourceExtensions.AddTables%2A> | `azure.storage.table.v0` | [Azure Storage resource types](#azure-storage-resource-types) |

#### Azure Key Vault resource type

Example code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureKeyVault("keyvault1");
```

Example manifest:

```json
{
  "resources": {
    "keyvault1": {
      "type": "azure.keyvault.v0"
    }
  }
}
```

#### Azure Service Bus resource type

Example code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureServiceBus(
    "sb1",
    queueNames: new[] { "queue1", "queue2" },
    topicNames: new[] { "topic1", "topic2" }
);
```

Example manifest:

```json
{
  "resources": {
    "sb1": {
      "type": "azure.servicebus.v0",
      "queues": [
        "queue1",
        "queue2"
      ],
      "topics": [
        "topic1",
        "topic2"
      ]
    }
  }
}
```

#### Azure Storage resource types

Example code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("images");

storage.AddBlobs("blobs");
storage.AddQueues("queues");
storage.AddTables("tables");
```

Example manifest:

```json
{
  "resources": {
    "images": {
      "type": "azure.storage.v0"
    },
    "blobs": {
      "type": "azure.storage.blob.v0",
      "parent": "images"
    },
    "queues": {
      "type": "azure.storage.queue.v0",
      "parent": "images"
    },
    "tables": {
      "type": "azure.storage.table.v0",
      "parent": "images"
    }
  }
}
```

#### Azure Redis resource type

Example code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureRedis("azredis1");
```

Example manifest:

```json
{
  "resources": {
    "azredis1": {
      "type": "azure.redis.v0"
    }
 }
}
```

#### Azure App Configuration resource type

Example code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureAppConfiguration("appconfig1");
```

Example manifest:

```json
{
  "resources": {
    "appconfig1": {
      "type": "azure.appconfiguration.v0"
    }
  }
}
```

#### Azure SQL resource types

Example code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureSqlServer("sql1")
       .AddDatabase("inventory");
```

Example manifest:

```json
{
  "resources": {
    "sql1": {
      "type": "azure.sql.v0"
    },
    "inventory": {
      "type": "azure.sql.database.v0",
      "parent": "sql1"
    }
  }
}
```

### Resource types supported in the Azure Developer CLI

The [Azure Developer CLI](/azure/developer/azure-developer-cli/) (AZD) is a tool that can be used to deploy .NET Aspire applications to Azure Container Apps. The following table shows the resource types that are supported in AZD:

| Name | Resource type | Supported in AZD |
|--|--|:--:|
| Azure App Configuration | `azure.appconfiguration.v0` | <span aria-hidden="true">✔️</span><span class="visually-hidden">Yes</span> |
| Azure Key Vault | `azure.keyvault.v0` | <span aria-hidden="true">✔️</span><span class="visually-hidden">Yes</span> |
| Azure Redis | `azure.redis.v0` | <span aria-hidden="true">✔️</span><span class="visually-hidden">Yes</span> |
| Azure Service Bus | `azure.servicebus.v0` | <span aria-hidden="true">✔️</span><span class="visually-hidden">Yes</span> |
| Azure SQL Database | `azure.sql.database.v0` | <span aria-hidden="true">✔️</span><span class="visually-hidden">Yes</span> |
| Azure SQL | `azure.sql.v0` | <span aria-hidden="true">✔️</span><span class="visually-hidden">Yes</span> |
| Azure Storage Blobs | `azure.storage.blob.v0` | <span aria-hidden="true">✔️</span><span class="visually-hidden">Yes</span> |
| Azure Storage Queues | `azure.storage.queue.v0` | <span aria-hidden="true">✔️</span><span class="visually-hidden">Yes</span> |
| Azure Storage Tables | `azure.storage.table.v0` | <span aria-hidden="true">✔️</span><span class="visually-hidden">Yes</span> |
| Azure Storage | `azure.storage.v0` | <span aria-hidden="true">✔️</span><span class="visually-hidden">Yes</span> |
| Container | `container.v0` | <span aria-hidden="true">❌</span><span class="visually-hidden">No</span> |
| Dockerfile | `docker.v0` | <span aria-hidden="true">❌</span><span class="visually-hidden">No</span> |
| Postgres Connection | `postgres.connection.v0` | <span aria-hidden="true">✔️</span><span class="visually-hidden">Yes</span> |
| Postgres Database | `postgres.database.v0` | <span aria-hidden="true">✔️</span><span class="visually-hidden">Yes</span> |
| Postgres Server | `postgres.server.v0` | <span aria-hidden="true">✔️</span><span class="visually-hidden">Yes</span> |
| .NET Project | `project.v0` | <span aria-hidden="true">✔️</span><span class="visually-hidden">Yes</span> |
| RabbitMQ Connection | `rabbitmq.connection.v0` | <span aria-hidden="true">✔️</span><span class="visually-hidden">Yes</span> |
| RabbitMQ Server | `rabbitmq.server.v0` | <span aria-hidden="true">❌</span><span class="visually-hidden">No</span> |
| Redis | `redis.v0` | <span aria-hidden="true">✔️</span><span class="visually-hidden">Yes</span> |
| SQL Server Connection | `sqlserver.connection.v0` | <span aria-hidden="true">❌</span><span class="visually-hidden">No</span> |
| SQL Server Database | `sqlserver.database.v0` | <span aria-hidden="true">❌</span><span class="visually-hidden">No</span> |
| SQL Server Server | `sqlserver.server.v0` | <span aria-hidden="true">❌</span><span class="visually-hidden">No</span> |

For more information, see [Deploy a .NET Aspire app to Azure Container Apps using the Azure Developer CLI (in-depth guide)](azure/aca-deployment-azd-in-depth.md).

## See also

- [.NET Aspire overview](../get-started/aspire-overview.md)
- [.NET Aspire orchestration overview](../fundamentals/app-host-overview.md)
- [.NET Aspire components overview](../fundamentals/components-overview.md)
- [Service discovery in .NET Aspire](../service-discovery/overview.md)
