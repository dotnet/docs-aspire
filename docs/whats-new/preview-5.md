---
title: .NET Aspire preview 5
description: .NET Aspire preview 5 is now available and includes many improvements and new capabilities
ms.date: 04/02/2024
---

# .NET Aspire preview 5

Breaking Changes

- The major package split, Aspire.*and Aspire.Hosting.*
- AppHost projects now need to reference Aspire.Hosting.AppHost
- Each resource was split into individual packages
  - Aspire.Hosting.Redis
  - Aspire.Hosting.PostgreSQL
  - Aspire.Hosting.MongoDB
  - Aspire.Hosting.RabbitMQ
  - etc.
- The same applies to the Azure resources
- Aspire.Hosting.Azure.Redis (etc)
- Async all the things, various callbacks in the app model APIs
are now async. (GetConnectionStringAsync etc.)

## Dashboard

In preview 5. our primary focus has been on non-functional requirements around security and performance improvements.

### Security Updates

Communication has been secured across the following endpoints:

- OTLP
- Dashboard
- Resource Server

#### OTLP endpoint security

The OTLP endpoint can be secured with [client certificate](/aspnet/core/security/authentication/certauth) or API key authentication.

- `Otlp:AuthMode` specifies the authentication mode on the OTLP endpoint. Possible values are `Certificate`, `ApiKey`, `Unsecured`. This configuration is required.
- `Otlp:ApiKey` specifies the API key for the OTLP endpoint when API key authentication is enabled. This configuration is required for API key authentication.

#### Dashboard authentication

The dashboard's web application frontend supports OpenID Connect (OIDC) for authentication. These can be applied via configurable settings, once Frontend:AuthMode is set to OpenIdConnect:

- `Authentication:Schemes:OpenIdConnect:Authority` &mdash; URL to the identity provider (IdP)
- `Authentication:Schemes:OpenIdConnect:ClientId` &mdash; Identity of the relying party (RP)
- `Authentication:Schemes:OpenIdConnect:ClientSecret`&mdash; A secret that only the real RP would know
- Other properties of [`OpenIdConnectOptions`](/dotnet/api/microsoft.aspnetcore.builder.openidconnectoptions) specified in configuration container `Authentication:Schemes:OpenIdConnect:*`
  
#### Resource server endpoint security

The resource server client supports client certificates. This can be applied via configurable settings, once ResourceServiceClient:AuthMode to Certificate

- `ResourceServiceClient:ClientCertificate:Source` (required) one of:
  - `File` to load the cert from a file path, configured with:
    - `ResourceServiceClient:ClientCertificate:FilePath` (required, string)
    - `ResourceServiceClient:ClientCertificate:Password` (optional, string)
  - `KeyStore` to load the cert from a key store, configured with:
    - `ResourceServiceClient:ClientCertificate:Subject` (required, string)
    - `ResourceServiceClient:ClientCertificate:KeyStore:Name` (optional, [`StoreName`](/dotnet/api/system.security.cryptography.x509certificates.storename), defaults to `My`)
    - `ResourceServiceClient:ClientCertificate:KeyStore:Location` (optional, [`StoreLocation`](/dotnet/api/system.security.cryptography.x509certificates.storelocation), defaults to `CurrentUser`)
- `ResourceServiceClient:Ssl` (optional, [`SslClientAuthenticationOptions`](/dotnet/api/system.net.security.sslclientauthenticationoptions))

### Performance improvements

- LOTS of performance improvements
  - Console log virtualization
  - Load time improvements
  - Trace ingestion improvements
- Run on a single port (OTLP and UI)
- Standalone container now forces you to choose auth

Templates

- HTTPs by default
- Test project support

AppModel

- Forwarded headers enabled by default for projects with endpoints
- Custom resources support in dashboard
  - Can publish notifications to the dashboard
  - Can log console output to the dashboard
- Built in methods for containers with well known volume mounts
- Add RabbitMQ WithManagementUI method.
- Applied consistent scheme for resources that support passwords
  - They can autogenerate passwords, or take parameters
- Support for composing environment variables using string interpolation
that can capture resource properties. (ReferenceExpression, WithEnvironment overload)

Service Discovery

- Service discovery API changes
- Service discovery auto scheme detection

Tooling

- VS Code support
- Prompting for parameters in Visual Studio

Components

- Azure Events Hubs
- Renamed all of the methods to end with *Client e.g. AddRedisClient
- Nats OpenTelemetry support

Azure

- Azure CDK Support (introducing the new Azure CDK)
  - All Azure resources were refactored to use the CDK
- Azure Provisioning for Development
- Azure OpenAI provisioning

Manifest

- Express container volumes and bindmounts in the manifest
- Support for multiple endpoints in the manifest
- Renamed containerPort to targetPort
- Added port as the "exposed port"

Azure Deployment

- Service selection prompt gone (WithExternalHttpEndpoints in apphost)
- Support for multiple endpoints in ACA
- Support for adding volumes to containers in ACA

- IDE protocol changes
- There's a new IDE protcol
