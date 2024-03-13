---
title: .NET Aspire manifest format for deployment tool builders
description: Learn about the .NET Aspire manifest format in this comprehensive deployment tool builder guide.
ms.date: 03/13/2024
ms.topic: reference
---

# .NET Aspire manifest format for deployment tool builders

In this article, you learn about the .NET Aspire manifest format. This article serves as a reference guide for deployment tool builders, aiding in the creation of tooling to deploy .NET Aspire applications on specific hosting platforms, whether on-premises or in the cloud.

.NET Aspire [simplifies the local development experience](../fundamentals/networking-overview.md) by helping to manage interdependencies between application components. To help simplify the deployment of applications, .NET Aspire projects can generate a manifest of all the resources defined as a JSON formatted file.

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
    --output-path ../aspire-manifest.json
```

> [!TIP]
> The `--output-path` supports relative paths. The previous command uses `../aspire-manifest.json` to place the manifest file in the root of the project directory.

For more information, see [dotnet run](/dotnet/core/tools/dotnet-run). The previous command produces the following output:

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
      "type": "container.v0",
      "connectionString": "{cache.bindings.tcp.host}:{cache.bindings.tcp.port}",
      "image": "redis:7.2.4",
      "bindings": {
        "tcp": {
          "scheme": "tcp",
          "protocol": "tcp",
          "transport": "tcp",
          "containerPort": 6379
        }
      }
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

var apiService = builder.AddProject<Projects.AspireApp_ApiService>("apiservice");

builder.AddProject<Projects.AspireApp_Web>("webfrontend")
    .WithReference(cache)
    .WithReference(apiService);

builder.Build().Run();
```

References between project resource types result in [service discovery](../service-discovery/overview.md) variables being injected into the referencing project. References to well known reference types such as Redis result in connection strings being injected.

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

Each resource has a `type` field. When a deployment tool reads the manifest, it should read the type to verify whether it can correctly process the manifest. During the .NET Aspire preview period, all resource types have a `v0` suffix to indicate that they're subject to change. As .NET Aspire approaches release a `v1` suffix will be used to signify that the structure of the manifest for that resource type should be considered stable (subsequent updates increment the version number accordingly).

### Common resource fields

The `type` field is the only field that is common across all resource types, however, the `project.v0`, `container.v0`, and `executable.v0` resource types also share the `env` and `bindings` fields.

> [!NOTE]
> The `executable.v0` resource type isn't fully implemented in the manifest due to its lack of utility in deployment scenarios. For more information on containerizing executables, see [Dockerfile resource types](#dockerfile-resource-types).

The `env` field type is a basic key/value mapping where the values might contain [_placeholder strings_](#placeholder-string-structure).

Bindings are specified in the `bindings` field with each binding contained within its own field under the `bindings` JSON object. The fields omitted by the .NET Aspire manifest in the `bindings` node include:

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

### Cloud-agnostic resource types

These resources are available in the [Aspire.Hosting](https://www.nuget.org/packages/Aspire.Hosting) NuGet package.

| App model usage | Manifest resource type | Heading link |
|--|--|--|
| <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.AddContainer%2A> | `container.v0` | [Container resource type](#container-resource-type) |
| `PublishAsDockerFile` | `dockerfile.v0` | [Dockerfile resource types](#dockerfile-resource-types) |
| <xref:Aspire.Hosting.MongoDBBuilderExtensions.AddDatabase%2A> | `value.v0` | [MongoDB Server resource types](#mongodb-resource-types) |
| <xref:Aspire.Hosting.MongoDBBuilderExtensions.AddMongoDB%2A> | `container.v0` | [MongoDB resource types](#mongodb-resource-types) |
| <xref:Aspire.Hosting.MySqlBuilderExtensions.AddDatabase%2A> | `value.v0` | [MySQL Server resource types](#mysql-resource-types) |
| <xref:Aspire.Hosting.MySqlBuilderExtensions.AddMySql%2A> | `container.v0` | [MySQL resource types](#mysql-resource-types) |
| <xref:Aspire.Hosting.PostgresBuilderExtensions.AddDatabase%2A> | `value.v0` | [Postgres resource types](#postgres-resource-types) |
| <xref:Aspire.Hosting.PostgresBuilderExtensions.AddPostgres%2A> | `container.v0` | [Postgres resource types](#postgres-resource-types) |
| <xref:Aspire.Hosting.ProjectResourceBuilderExtensions.AddProject%2A> | `project.v0` | [Project resource type](#project-resource-type) |
| <xref:Aspire.Hosting.RabbitMQBuilderExtensions.AddRabbitMQ%2A> | `container.v0` | [RabbitMQ resource types](#rabbitmq-resource-types) |
| <xref:Aspire.Hosting.RedisBuilderExtensions.AddRedis%2A> | `container.v0` | [Redis resource type](#redis-resource-type) |
| <xref:Aspire.Hosting.SqlServerBuilderExtensions.AddDatabase%2A> | `value.v0` | [SQL Server resource types](#sql-server-resource-types) |
| <xref:Aspire.Hosting.SqlServerBuilderExtensions.AddSqlServer%2A> | `container.v0` | [SQL Server resource types](#sql-server-resource-types) |

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
       .WithHttpEndpoint(3000);
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
       .WithHttpEndpoint(hostPort: 5031, env: "PORT")
       .PublishAsDockerFile();
```

> [!TIP]
> The `PublishAsDockerFile` call is required to generate the Dockerfile resource type in the manifest, and this extension method is only available on the <xref:Aspire.Hosting.ApplicationModel.ExecutableResource> type.

Example manifest:

```json
{
  "resources": {
    "nodeapp": {
      "type": "dockerfile.v0",
      "path": "../nodeapp/Dockerfile",
      "context": "../nodeapp",
      "env": {
        "NODE_ENV": "development",
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

builder.AddPostgres("postgres1")
       .AddDatabase("shipping");
```

Example manifest:

```json
{
  "resources": {
    "postgres1": {
      "type": "container.v0",
      "connectionString": "Host={postgres1.bindings.tcp.host};Port={postgres1.bindings.tcp.port};Username=postgres;Password={postgres1.inputs.password}",
      "image": "postgres:16.2",
      "env": {
        "POSTGRES_HOST_AUTH_METHOD": "scram-sha-256",
        "POSTGRES_INITDB_ARGS": "--auth-host=scram-sha-256 --auth-local=scram-sha-256",
        "POSTGRES_PASSWORD": "{postgres1.inputs.password}"
      },
      "bindings": {
        "tcp": {
          "scheme": "tcp",
          "protocol": "tcp",
          "transport": "tcp",
          "containerPort": 5432
        }
      },
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
      "type": "value.v0",
      "connectionString": "{postgres1.connectionString};Database=shipping"
    }
  }
}
```

#### RabbitMQ resource types

RabbitMQ is modeled as a container resource `container.v0`. The following sample shows how they're added to the app model.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddRabbitMQ("rabbitmq1");
```

The previous code produces the following manifest:

```json
{
  "resources": {
    "rabbitmq1": {
      "type": "container.v0",
      "connectionString": "amqp://guest:{rabbitmq1.inputs.password}@{rabbitmq1.bindings.tcp.host}:{rabbitmq1.bindings.tcp.port}",
      "image": "rabbitmq:3",
      "env": {
        "RABBITMQ_DEFAULT_USER": "guest",
        "RABBITMQ_DEFAULT_PASS": "{rabbitmq1.inputs.password}"
      },
      "bindings": {
        "tcp": {
          "scheme": "tcp",
          "protocol": "tcp",
          "transport": "tcp",
          "containerPort": 5672
        }
      },
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
```

Example manifest:

```json
{
  "resources": {
    "redis1": {
      "type": "container.v0",
      "connectionString": "{redis1.bindings.tcp.host}:{redis1.bindings.tcp.port}",
      "image": "redis:7.2.4",
      "bindings": {
        "tcp": {
          "scheme": "tcp",
          "protocol": "tcp",
          "transport": "tcp",
          "containerPort": 6379
        }
      }
    }
  }
}
```

#### SQL Server resource types

Example code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddSqlServer("sql1")
       .AddDatabase("shipping");
```

Example manifest:

```json
{
  "resources": {
    "sql1": {
      "type": "container.v0",
      "connectionString": "Server={sql1.bindings.tcp.host},{sql1.bindings.tcp.port};User ID=sa;Password={sql1.inputs.password};TrustServerCertificate=true",
      "image": "mcr.microsoft.com/mssql/server:2022-latest",
      "env": {
        "ACCEPT_EULA": "Y",
        "MSSQL_SA_PASSWORD": "{sql1.inputs.password}"
      },
      "bindings": {
        "tcp": {
          "scheme": "tcp",
          "protocol": "tcp",
          "transport": "tcp",
          "containerPort": 1433
        }
      },
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
      "type": "value.v0",
      "connectionString": "{sql1.connectionString};Database=shipping"
    }
  }
}
```

#### MongoDB resource types

Example code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddMongoDB("mongodb1")
       .AddDatabase("shipping");
```

Example manifest:

```json
{
  "resources": {
    "mongodb1": {
      "type": "container.v0",
      "connectionString": "mongodb://{mongodb1.bindings.tcp.host}:{mongodb1.bindings.tcp.port}",
      "image": "mongo:7.0.5",
      "bindings": {
        "tcp": {
          "scheme": "tcp",
          "protocol": "tcp",
          "transport": "tcp",
          "containerPort": 27017
        }
      }
    },
    "shipping": {
      "type": "value.v0",
      "connectionString": "{mongodb1.connectionString}/shipping"
    }
  }
}
```

#### MySQL resource types

Example code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddMySql("mysql1")
       .AddDatabase("shipping");
```

Example manifest:

```json
{
  "resources": {
    "mysql1": {
      "type": "container.v0",
      "connectionString": "Server={mysql1.bindings.tcp.host};Port={mysql1.bindings.tcp.port};User ID=root;Password={mysql1.inputs.password}",
      "image": "mysql:8.3.0",
      "env": {
        "MYSQL_ROOT_PASSWORD": "{mysql1.inputs.password}"
      },
      "bindings": {
        "tcp": {
          "scheme": "tcp",
          "protocol": "tcp",
          "transport": "tcp",
          "containerPort": 3306
        }
      },
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
      "type": "value.v0",
      "connectionString": "{mysql1.connectionString};Database=shipping"
    }
  }
}
```

### Azure-specific resource types

The following resources are available in the [Aspire.Hosting.Azure](https://www.nuget.org/packages/Aspire.Hosting.Azure) NuGet package.

| App Model usage | Manifest resource type | Heading link |
|--|--|--|
| <xref:Aspire.Hosting.AzureAppConfigurationExtensions.AddAzureAppConfiguration%2A> | `azure.bicep.v0` | [Azure App Configuration resource types](#azure-app-configuration-resource-type) |
| <xref:Aspire.Hosting.AzureKeyVaultResourceExtensions.AddAzureKeyVault%2A> | `azure.bicep.v0` | [Azure Key Vault resource type](#azure-key-vault-resource-type) |
| <xref:Aspire.Hosting.RedisBuilderExtensions.AddRedis%2A>`.AsAzureRedis()` | `azure.bicep.v0` | [Azure Redis resource types](#azure-redis-resource-type) |
| <xref:Aspire.Hosting.AzureServiceBusExtensions.AddAzureServiceBus%2A> | `azure.bicep.v0` | [Azure Service Bus resource type](#azure-service-bus-resource-type) |
| <xref:Aspire.Hosting.SqlServerBuilderExtensions.AddSqlServer%2A>`.AsAzureSqlDatabase()` | `azure.bicep.v0` | [Azure SQL resource types](#azure-sql-resource-types) |
| <xref:Aspire.Hosting.SqlServerBuilderExtensions.AddSqlServer%2A>`.AsAzureSqlDatabase().AddDatabase(...)` | `value.v0` | [Azure SQL resource types](#azure-sql-resource-types) |
| <xref:Aspire.Hosting.PostgresBuilderExtensions.AddPostgres%2A>`.AsAzurePostgresFlexibleServer(...)` | `azure.bicep.v0` | [Azure Postgres resource types](#azure-postgres-resource-types) |
| <xref:Aspire.Hosting.PostgresBuilderExtensions.AddPostgres%2A>`.AsAzurePostgresFlexibleServer(...).AddDatabase(...)` | `value.v0` | [Azure Postgres resource types](#azure-postgres-resource-types) |
| <xref:Aspire.Hosting.AzureStorageExtensions.AddAzureStorage%2A> | `azure.storage.v0` | [Azure Storage resource types](#azure-storage-resource-types) |
| <xref:Aspire.Hosting.AzureStorageExtensions.AddBlobs%2A> | `value.v0` | [Azure Storage resource types](#azure-storage-resource-types) |
| <xref:Aspire.Hosting.AzureStorageExtensions.AddQueues%2A> | `value.v0` | [Azure Storage resource types](#azure-storage-resource-types) |
| <xref:Aspire.Hosting.AzureStorageExtensions.AddTables%2A> | `value.v0` | [Azure Storage resource types](#azure-storage-resource-types) |

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
      "type": "azure.bicep.v0",
      "connectionString": "{keyvault1.outputs.vaultUri}",
      "path": "aspire.hosting.azure.bicep.keyvault.bicep",
      "params": {
        "principalId": "",
        "principalType": "",
        "vaultName": "keyvault1"
      }
    }
  }
}
```

#### Azure Service Bus resource type

Example code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureServiceBus("sb1")
       .AddTopic("topic1", [])
       .AddTopic("topic2", [])
       .AddQueue("queue1")
       .AddQueue("queue2");
```

Example manifest:

```json
{
  "resources": {
    "sb1": {
      "type": "azure.bicep.v0",
      "connectionString": "{sb1.outputs.serviceBusEndpoint}",
      "path": "aspire.hosting.azure.bicep.servicebus.bicep",
      "params": {
        "serviceBusNamespaceName": "sb1",
        "principalId": "",
        "principalType": "",
        "queues": [
          "queue1",
          "queue2"
        ],
        "topics": [
          {
            "name": "topic1",
            "subscriptions": []
          },
          {
            "name": "topic2",
            "subscriptions": []
          }
        ]
      }
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
      "type": "azure.bicep.v0",
      "path": "aspire.hosting.azure.bicep.storage.bicep",
      "params": {
        "principalId": "",
        "principalType": "",
        "storageName": "images"
      }
    },
    "blobs": {
      "type": "value.v0",
      "connectionString": "{images.outputs.blobEndpoint}"
    },
    "queues": {
      "type": "value.v0",
      "connectionString": "{images.outputs.queueEndpoint}"
    },
    "tables": {
      "type": "value.v0",
      "connectionString": "{images.outputs.tableEndpoint}"
    }
  }
}
```

#### Azure Redis resource type

Example code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddRedis("azredis1")
       .PublishAsAzureRedis();
```

Example manifest:

```json
{
  "resources": {
    "azredis1": {
      "type": "azure.bicep.v0",
      "connectionString": "{azredis1.secretOutputs.connectionString}",
      "path": "aspire.hosting.azure.bicep.redis.bicep",
      "params": {
        "redisCacheName": "azredis1",
        "keyVaultName": ""
      }
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
      "type": "azure.bicep.v0",
      "connectionString": "{appconfig1.outputs.appConfigEndpoint}",
      "path": "aspire.hosting.azure.bicep.appconfig.bicep",
      "params": {
        "configName": "appconfig1",
        "principalId": "",
        "principalType": ""
      }
    }
  }
}
```

#### Azure SQL resource types

Example code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddSqlServer("sql1")
       .PublishAsAzureSqlDatabase()
       .AddDatabase("inventory");
```

Example manifest:

```json
{
  "resources": {
    "sql1": {
      "type": "azure.bicep.v0",
      "connectionString": "Server=tcp:{sql1.outputs.sqlServerFqdn},1433;Encrypt=True;Authentication=\u0022Active Directory Default\u0022",
      "path": "aspire.hosting.azure.bicep.sql.bicep",
      "params": {
        "serverName": "sql1",
        "principalId": "",
        "principalName": "",
        "databases": [
          "inventory"
        ]
      }
    },
    "inventory": {
      "type": "value.v0",
      "connectionString": "{sql1.connectionString};Database=inventory"
    }
  }
}
```

#### Azure Postgres resource types

Example code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var administratorLogin = builder.AddParameter("administratorLogin");
var administratorLoginPassword = builder.AddParameter(
    "administratorLoginPassword", secret: true);

var pg = builder.AddPostgres("postgres")
                .PublishAsAzurePostgresFlexibleServer(
                    administratorLogin, administratorLoginPassword)
                .AddDatabase("db");
```

Example manifest:

```json
{
  "resources": {
    "administratorLogin": {
      "type": "parameter.v0",
      "value": "{administratorLogin.inputs.value}",
      "inputs": {
        "value": {
          "type": "string"
        }
      }
    },
    "administratorLoginPassword": {
      "type": "parameter.v0",
      "value": "{administratorLoginPassword.inputs.value}",
      "inputs": {
        "value": {
          "type": "string",
          "secret": true
        }
      }
    },
    "postgres": {
      "type": "azure.bicep.v0",
      "connectionString": "{postgres.secretOutputs.connectionString}",
      "path": "aspire.hosting.azure.bicep.postgres.bicep",
      "params": {
        "serverName": "postgres",
        "keyVaultName": "",
        "administratorLogin": "{administratorLogin.value}",
        "administratorLoginPassword": "{administratorLoginPassword.value}",
        "databases": [
          "db"
        ]
      }
    },
    "db": {
      "type": "value.v0",
      "connectionString": "{postgres.connectionString};Database=db"
    }
  }
}
```

### Resource types supported in the Azure Developer CLI

The [Azure Developer CLI](/azure/developer/azure-developer-cli/) (azd) is a tool that can be used to deploy .NET Aspire applications to Azure Container Apps. With the `azure.bicep.v0` resource type, cloud-agnostic resource container types can be mapped to Azure-specific resources. The following table lists the resource types that are supported in the Azure Developer CLI:

| Name | Cloud-agnostic API | Configure as Azure resource |
| ---- | ---------------------------- | ---------------------------------- |
| Redis | <xref:Aspire.Hosting.RedisBuilderExtensions.AddRedis%2A> | `PublishAsAzureRedis` |
| Postgres | <xref:Aspire.Hosting.PostgresBuilderExtensions.AddPostgres%2A> | `PublishAsAzurePostgresFlexibleServer` |
| SQL Server | <xref:Aspire.Hosting.SqlServerBuilderExtensions.AddSqlServer%2A> | `PublishAsAzureSqlDatabase` |

When resources as configured as Azure resources, the `azure.bicep.v0` resource type is generated in the manifest. For more information, see [Deploy a .NET Aspire app to Azure Container Apps using the Azure Developer CLI (in-depth guide)](azure/aca-deployment-azd-in-depth.md).

## See also

- [.NET Aspire overview](../get-started/aspire-overview.md)
- [.NET Aspire orchestration overview](../fundamentals/app-host-overview.md)
- [.NET Aspire components overview](../fundamentals/components-overview.md)
- [Service discovery in .NET Aspire](../service-discovery/overview.md)
