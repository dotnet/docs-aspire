---
title: .NET Aspire Keycloak integration
description: This article describes the .NET Aspire Keycloak integration.
ms.topic: how-to
ms.date: 08/12/2024
---

# .NET Aspire Keycloak integration

In this article, you learn how to use the .NET Aspire Keycloak integration. The `Aspire.Keycloak.Authentication` library registers JwtBearer and OpenId Connect authentication handlers in the DI container for connecting to a Keycloak server.

## Prerequisites

- A Keycloak server instance.
- A Keycloak realm.
- For JwtBearer authentication, a configured audience in the Keycloak realm.
- For OpenId Connect authentication, the ID of a client configured in the Keycloak realm.

## Get started

To get started with the .NET Aspire Keycloak integration, install the [Aspire.Keycloak.Authentication](https://www.nuget.org/packages/Aspire.Keycloak.Authentication) NuGet package in the client-consuming project, i.e., the project for the application that uses the Keycloak client.

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

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Jwt bearer authentication usage example

In the _Program.cs_ file of your ASP.NET Core API project, call the `AddKeycloakJwtBearer` extension method to add JwtBearer authentication, using a connection name, realm and any required JWT Bearer options:

```csharp
builder.Services.AddAuthentication()
                .AddKeycloakJwtBearer("keycloak", realm: "WeatherShop", options =>
                {
                    options.Audience = "weather.api";
                });
```

You can set many other options via the `Action<JwtBearerOptions> configureOptions` delegate.

## OpenId Connect authentication usage example

In the _Program.cs_ file of your Blazor project, call the `AddKeycloakOpenIdConnect` extension method to add OpenId Connect authentication, using a connection name, realm and any required OpenId Connect options:

```csharp
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddKeycloakOpenIdConnect(
                    "keycloak", 
                    realm: "WeatherShop", 
                    options =>
                    {
                        options.ClientId = "WeatherWeb";
                        options.ResponseType = OpenIdConnectResponseType.Code;
                        options.Scope.Add("weather:all");
                    });
```

You can set many other options via the `Action<OpenIdConnectOptions>? configureOptions` delegate.

## App host usage

To model the Keycloak resource in the app host, install the [Aspire.Hosting.Keycloak](https://www.nuget.org/packages/Aspire.Hosting.Keycloak) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Keycloak
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Keycloak"
                  Version="*" />
```

---

Then, in the _Program.cs_ file of `AppHost`, register a Keycloak server and consume the connection using the following methods:

```csharp
var keycloak = builder.AddKeycloak("keycloak", 8080);

var apiService = builder.AddProject<Projects.Keycloak_ApiService>("apiservice")
                        .WithReference(keycloak);

builder.AddProject<Projects.Keycloak_Web>("webfrontend")
       .WithExternalHttpEndpoints()
       .WithReference(keycloak)
       .WithReference(apiService);
```

> [!TIP]
> For local development use a stable port for the Keycloak resource (8080 in the example above). It can be any port, but it should be stable to avoid issues with browser cookies that will persist OIDC tokens (which include the authority URL, with port) beyond the lifetime of the _app host_.

The `WithReference` method configures a connection in the `Keycloak.ApiService` and `Keycloak.Web` projects named `keycloak`.

In the _Program.cs_ file of `Keycloak.ApiService`, the Keycloak connection can be consumed using:

```csharp
builder.Services.AddAuthentication()
                .AddKeycloakJwtBearer("keycloak", realm: "WeatherShop");
```

In the _Program.cs_ file of `Keycloak.Web`, the Keycloak connection can be consumed using:

```csharp
var oidcScheme = OpenIdConnectDefaults.AuthenticationScheme;

builder.Services.AddAuthentication(oidcScheme)
                .AddKeycloakOpenIdConnect(
                    "keycloak",
                    realm: "WeatherShop",
                    oidcScheme);
```

## See also

- [Keycloak](https://www.keycloak.org/)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
