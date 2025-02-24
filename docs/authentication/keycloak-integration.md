---
title: .NET Aspire Keycloak integration (Preview)
description: Learn how to use the .NET Aspire Keycloak integration, which includes both hosting and client integrations.
ms.date: 12/06/2024
uid: authentication/keycloak-integration
---

# .NET Aspire Keycloak integration (Preview)

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[Keycloak](https://www.keycloak.org/) is an open-source Identity and Access Management solution aimed at modern applications and services. The .NET Aspire Keycloak integration enables you to connect to existing Keycloak instances or create new instances from .NET with the [`quay.io/keycloak/keycloak` container image](https://quay.io/repository/keycloak/keycloak).

## Hosting integration

The .NET Aspire Keycloak hosting integration models the server as the <xref:Aspire.Hosting.ApplicationModel.KeycloakResource> type. To access these types and APIs, add the [📦 Aspire.Hosting.Keycloak](https://www.nuget.org/packages/Aspire.Hosting.Keycloak) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Keycloak --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Keycloak"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add Keycloak resource

In your app host project, call <xref:Aspire.Hosting.KeycloakResourceBuilderExtensions.AddKeycloak*> to add and return a Keycloak resource builder. Chain a call to the returned resource builder to configure the Keycloak.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var keycloak = builder.AddKeycloak("keycloak", 8080);

var apiService = builder.AddProject<Projects.Keycloak_ApiService>("apiservice")
                        .WithReference(keycloak)
                        .WaitFor(keycloak);

builder.AddProject<Projects.Keycloak_Web>("webfrontend")
       .WithExternalHttpEndpoints()
       .WithReference(keycloak)
       .WithReference(apiService)
       .WaitFor(apiService);

// After adding all resources, run the app...
```

> [!TIP]
> For local development use a stable port for the Keycloak resource (`8080` in the preceding example). It can be any port, but it should be stable to avoid issues with browser cookies that will persist OIDC tokens (which include the authority URL, with port) beyond the lifetime of the _app host_.

When .NET Aspire adds a container image to the app host, as shown in the preceding example with the `quay.io/keycloak/keycloak` image, it creates a new Keycloak instance on your local machine. The Keycloak resource includes default credentials:

- `KEYCLOAK_ADMIN`: A value of `admin`.
- `KEYCLOAK_ADMIN_PASSWORD`: Random `password` generated using the <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.CreateDefaultPasswordParameter*> method.

When the app host runs, the password is stored in the app host's secret store. It's added to the `Parameters` section, for example:

```json
{
  "Parameters:keycloak-password": "<THE_GENERATED_PASSWORD>"
}
```

The name of the parameter is `keycloak-password`, but really it's just formatting the resource name with a `-password` suffix. For more information, see [Safe storage of app secrets in development in ASP.NET Core](/aspnet/core/security/app-secrets) and [Add Keycloak resource](#add-keycloak-resource).

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` named `keycloak` and the <xref:Aspire.Hosting.ResourceBuilderExtensions.WaitFor*> instructs the app host to not start the dependant service until the `keycloak` resource is ready.

> [!TIP]
> If you'd rather connect to an existing Keycloak instance, call <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> instead. For more information, see [Reference existing resources](../fundamentals/app-host-overview.md#reference-existing-resources).

### Add Keycloak resource with data volume

To add a data volume to the Keycloak resource, call the <xref:Aspire.Hosting.KeycloakResourceBuilderExtensions.WithDataVolume*> method on the Keycloak resource:

```csharp
var keycloak = builder.AddKeycloak("keycloak", 8080)
                      .WithDataVolume();

var apiService = builder.AddProject<Projects.Keycloak_ApiService>("apiservice")
                        .WithReference(keycloak)
                        .WaitFor(keycloak);

builder.AddProject<Projects.Keycloak_Web>("webfrontend")
       .WithExternalHttpEndpoints()
       .WithReference(keycloak)
       .WithReference(apiService)
       .WaitFor(apiService);

// After adding all resources, run the app...
```

The data volume is used to persist the Keycloak data outside the lifecycle of its container. The data volume is mounted at the `/opt/keycloak/data` path in the Keycloak container and when a `name` parameter isn't provided, the name is generated at random. For more information on data volumes and details on why they're preferred over [bind mounts](#add-keycloak-resource-with-data-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

> [!WARNING]
> The admin credentials are stored in the data volume. When using a data volume and if the credentials change, it will not work until you delete the volume.

### Add Keycloak resource with data bind mount

To add a data bind mount to the Keycloak resource, call the <xref:Aspire.Hosting.KeycloakResourceBuilderExtensions.WithDataBindMount*> method:

```csharp
var keycloak = builder.AddKeycloak("keycloak", 8080)
                      .WithDataBindMount(@"C:\Keycloak\Data");

var apiService = builder.AddProject<Projects.Keycloak_ApiService>("apiservice")
                        .WithReference(keycloak)
                        .WaitFor(keycloak);

builder.AddProject<Projects.Keycloak_Web>("webfrontend")
       .WithExternalHttpEndpoints()
       .WithReference(keycloak)
       .WithReference(apiService)
       .WaitFor(apiService);

// After adding all resources, run the app...
```

[!INCLUDE [data-bind-mount-vs-volumes](../includes/data-bind-mount-vs-volumes.md)]

Data bind mounts rely on the host machine's filesystem to persist the Keycloak data across container restarts. The data bind mount is mounted at the `C:\Keycloak\Data` on Windows (or `/Keycloak/Data` on Unix) path on the host machine in the Keycloak container. For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

### Add Keycloak resource with parameters

When you want to explicitly provide the admin username and password used by the container image, you can provide these credentials as parameters. Consider the following alternative example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("username");
var password = builder.AddParameter("password", secret: true);

var keycloak = builder.AddKeycloak("keycloak", 8080, username, password);

var apiService = builder.AddProject<Projects.Keycloak_ApiService>("apiservice")
                        .WithReference(keycloak)
                        .WaitFor(keycloak);

builder.AddProject<Projects.Keycloak_Web>("webfrontend")
       .WithExternalHttpEndpoints()
       .WithReference(keycloak)
       .WithReference(apiService)
       .WaitFor(apiService);

// After adding all resources, run the app...
```

The `username` and `password` parameters are usually provided as environment variables or secrets. The parameters are used to set the `KEYCLOAK_ADMIN` and `KEYCLOAK_ADMIN_PASSWORD` environment variables in the container. For more information on providing parameters, see [External parameters](../fundamentals/external-parameters.md).

### Add Keycloak resource with realm import

To import a realm into Keycloak, call the <xref:Aspire.Hosting.KeycloakResourceBuilderExtensions.WithRealmImport*> method:

:::code language="csharp" source="snippets/AspireApp/AspireApp.AppHost/Program.cs":::

The realm import files are mounted at `/opt/keycloak/data/import` in the Keycloak container. Realm import files are JSON files that represent the realm configuration. For more information on realm import, see [Keycloak docs: Importing a realm](https://www.keycloak.org/docs/latest/server_admin/index.html#_import).

As an example, the following JSON file could be added to the app host project in a _/Realms_ folder—to serve as a source realm configuration file:

:::code language="json" source="snippets/AspireApp/AspireApp.AppHost/Realms/weathershop-realm.json":::

### Hosting integration health checks

The Keycloak hosting integration doesn't currently support a health checks, nor does it automatically add them.

## Client integration

To get started with the .NET Aspire Keycloak client integration, install the [📦 Aspire.Keycloak.Authentication](https://www.nuget.org/packages/Aspire.Keycloak.Authentication) NuGet package in the client-consuming project, that is, the project for the application that uses the Keycloak client. The Keycloak client integration registers JwtBearer and OpenId Connect authentication handlers in the DI container for connecting to a Keycloak.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Keycloak.Authentication
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Keycloak.Authentication"
                  Version="*" />
```

---

### Add JWT bearer authentication

In the _:::no-loc text="Program.cs":::_ file of your ASP.NET Core API project, call the <xref:Microsoft.Extensions.DependencyInjection.AspireKeycloakExtensions.AddKeycloakJwtBearer*> extension method to add JwtBearer authentication, using a connection name, realm and any required JWT Bearer options:

```csharp
builder.Services.AddAuthentication()
                .AddKeycloakJwtBearer(
                    serviceName: "keycloak",
                    realm: "api",
                    options =>
                    {
                        options.Audience = "store.api";
                    });
```

You can set many other options via the `Action<JwtBearerOptions> configureOptions` delegate.

#### JWT bearer authentication example

To further exemplify the JWT bearer authentication, consider the following example:

:::code language="csharp" source="snippets/AspireApp/AspireApp.ApiService/Program.cs" highlight="12-20,22,49":::

The preceding ASP.NET Core Minimal API `Program` class demonstrates:

- Adding authentication services to the DI container with the <xref:Microsoft.Extensions.DependencyInjection.AuthenticationServiceCollectionExtensions.AddAuthentication*> API.
- Adding JWT bearer authentication with the <xref:Microsoft.Extensions.DependencyInjection.AspireKeycloakExtensions.AddKeycloakJwtBearer*> API and configuring:
  - The `serviceName` as `keycloak`.
  - The `realm` as `WeatherShop`.
  - The `options` with the `Audience` set to `weather.api` and sets `RequireHttpsMetadata` to `false`.
- Adds authorization services to the DI container with the <xref:Microsoft.Extensions.DependencyInjection.PolicyServiceCollectionExtensions.AddAuthorizationBuilder*> API.
- Calls the <xref:Microsoft.AspNetCore.Builder.AuthorizationEndpointConventionBuilderExtensions.RequireAuthorization*> API to require authorization on the `/weatherforecast` endpoint.

For a complete working sample, see [.NET Aspire playground: Keycloak integration](https://github.com/dotnet/aspire/tree/01ed51919f8df692ececce51048a140615dc759d/playground/keycloak).

### Add OpenId Connect authentication

In the _:::no-loc text="Program.cs":::_ file of your API-consuming project (for example, Blazor), call the <xref:Microsoft.Extensions.DependencyInjection.AspireKeycloakExtensions.AddKeycloakOpenIdConnect*> extension method to add OpenId Connect authentication, using a connection name, realm and any required OpenId Connect options:

```csharp
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddKeycloakOpenIdConnect(
                    serviceName: "keycloak",
                    realm: "api",
                    options =>
                    {
                        options.ClientId = "StoreWeb";
                        options.ResponseType = OpenIdConnectResponseType.Code;
                        options.Scope.Add("store:all");
                    });
```

You can set many other options via the `Action<OpenIdConnectOptions>? configureOptions` delegate.

#### OpenId Connect authentication example

To further exemplify the OpenId Connect authentication, consider the following example:

:::code language="csharp" source="snippets/AspireApp/AspireApp.Web/Program.cs" highlight="21-22,24-30,32,34-45,47,70":::

The preceding ASP.NET Core Blazor `Program` class:

- Adds the `HttpContextAccessor` to the DI container with the <xref:Microsoft.Extensions.DependencyInjection.HttpServiceCollectionExtensions.AddHttpContextAccessor*> API.
- Adds a custom `AuthorizationHandler` as a transient service to the DI container with the <xref:Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddTransient``1(Microsoft.Extensions.DependencyInjection.IServiceCollection)> API.
- Adds an <xref:System.Net.Http.HttpClient> to the `WeatherApiClient` service with the <xref:Microsoft.Extensions.DependencyInjection.HttpClientFactoryServiceCollectionExtensions.AddHttpClient``1(Microsoft.Extensions.DependencyInjection.IServiceCollection)> API and configuring it's base address with [service discovery](../service-discovery/overview.md) semantics that resolves to the `apiservice`.
  - Chains a call to the <xref:Microsoft.Extensions.DependencyInjection.HttpClientBuilderExtensions.AddHttpMessageHandler*> API to add a `AuthorizationHandler` to the `HttpClient` pipeline.
- Adds authentication services to the DI container with the <xref:Microsoft.Extensions.DependencyInjection.AuthenticationServiceCollectionExtensions.AddAuthentication*> API passing int the OpenId Connect default authentication scheme.
- Calls <xref:Microsoft.Extensions.DependencyInjection.AspireKeycloakExtensions.AddKeycloakOpenIdConnect*> and configures the `serviceName` as `keycloak`, the `realm` as `WeatherShop`, and the `options` object with various settings.
- Adds cascading authentication state to the Blazor app with the <xref:Microsoft.Extensions.DependencyInjection.CascadingAuthenticationStateServiceCollectionExtensions.AddCascadingAuthenticationState*> API.

The final callout is the `MapLoginAndLogout` extension method that adds login and logout routes to the Blazor app. This is defined as follows:

:::code language="csharp" source="snippets/AspireApp/AspireApp.Web/LoginLogoutEndpointRouteBuilderExtensions.cs":::

The preceding code:

- Maps a group for the `authentication` route and maps two endpoints for the `login` and `logout` routes:
  - Maps a `GET` request to the `/login` route that's handler is the `OnLogin` method—this is an anonymous endpoint.
  - Maps a `GET` request to the `/logout` route that's handler is the `OnLogout` method.

The `AuthorizationHandler` is a custom handler that adds the `Bearer` token to the `HttpClient` request. The handler is defined as follows:

:::code language="csharp" source="snippets/AspireApp/AspireApp.Web/AuthorizationHandler.cs":::

The preceding code:

- Is a subclass of the <xref:System.Net.Http.DelegatingHandler> class.
- Injects the `IHttpContextAccessor` service in the primary constructor.
- Overrides the `SendAsync` method to add the `Bearer` token to the `HttpClient` request:
  - The `access_token` is retrieved from the `HttpContext` and added to the `Authorization` header.

To help visualize the auth flow, consider the following sequence diagram:

:::image type="content" source="media/auth-flow-diagram.png" lightbox="media/auth-flow-diagram.png" alt-text="Authentication flow diagram—demonstrating a user request for an access token, Keycloak returning a JWT, and the token being forward to the API.":::

For a complete working sample, see [.NET Aspire playground: Keycloak integration](https://github.com/dotnet/aspire/tree/01ed51919f8df692ececce51048a140615dc759d/playground/keycloak).

## See also

- [Keycloak](https://www.keycloak.org/)
- [.NET Aspire playground: Keycloak integration](https://github.com/dotnet/aspire/tree/01ed51919f8df692ececce51048a140615dc759d/playground/keycloak)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
