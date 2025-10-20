---
title: Certificate trust customization in Aspire
description: Learn how to customize trusted certificates for Executable and Container resources in Aspire to enable secure communication.
ms.date: 10/20/2025
ai-usage: ai-assisted
---

# Certificate trust customization in Aspire

In Aspire, you can customize which certificates resources consider trusted for TLS/HTTPS traffic. This is particularly useful for resources that don't use the system's root trusted certificates by default, such as containerized applications, Python apps, and Node.js apps. By configuring certificate trust, you enable these resources to communicate securely with services that use certificates they wouldn't otherwise trust, including the Aspire dashboard's OTLP endpoint.

## When to use certificate trust customization

Certificate trust customization is valuable when:

- Resources need to trust the ASP.NET Core Development Certificate for local HTTPS communication.
- Containerized services must communicate with the dashboard over HTTPS.
- Python or Node.js applications need to trust custom certificate authorities.
- You're working with services that have specific certificate trust requirements.
- Resources need to establish secure telemetry connections to the Aspire dashboard.

## Development certificate trust

By default, Aspire attempts to add trust for the ASP.NET Core Development Certificate to resources that wouldn't otherwise trust it. This enables resources to communicate with the dashboard OTEL collector endpoint over HTTPS and any other HTTPS endpoints secured by the development certificate.

You can control this behavior at the per-resource level using the `WithDeveloperCertificateTrust` API or through AppHost configuration settings.

### Configure development certificate trust per resource

To explicitly enable or disable development certificate trust for a specific resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Explicitly enable development certificate trust
var nodeApp = builder.AddNpmApp("frontend", "../frontend")
    .WithDeveloperCertificateTrust(enabled: true);

// Disable development certificate trust
var pythonApp = builder.AddPythonApp("api", "../api", "main.py")
    .WithDeveloperCertificateTrust(enabled: false);

builder.Build().Run();
```

## Certificate authority collections

Certificate authority collections allow you to bundle custom certificates and make them available to resources. You create a collection using the `AddCertificateAuthorityCollection` method and then reference it from resources that need to trust those certificates.

### Create and use a certificate authority collection

```csharp
using System.Security.Cryptography.X509Certificates;

var builder = DistributedApplication.CreateBuilder(args);

// Load your custom certificates
var certificates = new X509Certificate2Collection();
certificates.Import("path/to/certificate.pem");

// Create a certificate authority collection
var certBundle = builder.AddCertificateAuthorityCollection("my-bundle")
    .WithCertificates(certificates);

// Apply the certificate bundle to resources
builder.AddNpmApp("my-project", "../myapp")
    .WithCertificateAuthorityCollection(certBundle);

builder.Build().Run();
```

In the preceding example, the certificate bundle is created with custom certificates and then applied to a Node.js application, enabling it to trust those certificates.

## Certificate trust scopes

Certificate trust scopes control how custom certificates interact with a resource's default trusted certificates. Different scopes provide flexibility in managing certificate trust based on your application's requirements.

The `WithCertificateTrustScope` API accepts a <xref:Aspire.Hosting.ApplicationModel.CertificateTrustScope> value to specify the trust behavior:

### Append mode

Attempts to append the configured certificates to the default trusted certificates for a given resource. This mode is useful when you want to add trust for additional certificates while maintaining trust for the system's default certificates.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddNodeApp("api", "../api")
    .WithCertificateTrustScope(CertificateTrustScope.Append);

builder.Build().Run();
```

> [!NOTE]
> Not all languages and runtimes support Append mode. For example, Python doesn't natively support appending certificates to the default trust store.

### Override mode

Attempts to override a resource to only trust the configured certificates, replacing the default trusted certificates entirely. This mode is useful when you need strict control over which certificates are trusted.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var certBundle = builder.AddCertificateAuthorityCollection("custom-certs")
    .WithCertificates(myCertificates);

builder.AddPythonModule("api", "./api", "uvicorn")
    .WithCertificateAuthorityCollection(certBundle)
    .WithCertificateTrustScope(CertificateTrustScope.Override);

builder.Build().Run();
```

### System mode

Attempts to combine the configured certificates with the default system root certificates and use them to override the default trusted certificates for a resource. This mode is intended to support Python or other languages that don't work well with Append mode.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddPythonApp("worker", "../worker", "main.py")
    .WithCertificateTrustScope(CertificateTrustScope.System);

builder.Build().Run();
```

### None mode

Disables all custom certificate trust for the resource, causing it to rely solely on its default certificate trust behavior.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddContainer("service", "myimage")
    .WithCertificateTrustScope(CertificateTrustScope.None);

builder.Build().Run();
```

## Custom certificate trust callbacks

For advanced scenarios, you can specify custom certificate trust behavior using callback APIs. These callbacks allow you to customize how certificates are configured for different resource types.

### Executable resource certificate trust

Use `WithExecutableCertificateTrustCallback` to customize certificate trust for executable resources:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddExecutable("custom-app", "myapp", ".")
    .WithExecutableCertificateTrustCallback((certificates, args) =>
    {
        // Customize command line arguments
        args.Add("--ca-file");
        args.Add("/path/to/ca-bundle.pem");
        
        // Set environment variables
        // Environment variables can be set through the resource builder
    });

builder.Build().Run();
```

The callback provides access to the certificate collection and allows you to specify command-line arguments required to configure trusted certificates.

### Container resource certificate trust

Use `WithContainerCertificateTrustCallback` to customize certificate trust for container resources:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddContainer("api", "myimage")
    .WithContainerCertificateTrustCallback((certificates, containerConfig) =>
    {
        // Customize container certificate paths
        containerConfig.CertificatePath = "/custom/certs/path";
        
        // Add environment variables for certificate configuration
        containerConfig.EnvironmentVariables["SSL_CERT_FILE"] = "/custom/certs/ca-bundle.pem";
    });

builder.Build().Run();
```

Default implementations are provided for Node.js, Python, and container resources. Container resources rely on standard OpenSSL configuration options, with default values that support the majority of common Linux distributions. You can override these defaults if necessary.

## Common scenarios

### Enable HTTPS telemetry to the dashboard

By default, Aspire enables development certificate trust for resources, allowing them to send telemetry to the dashboard over HTTPS:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Development certificate trust is enabled by default
var nodeApp = builder.AddNpmApp("frontend", "../frontend");
var pythonApp = builder.AddPythonApp("api", "../api", "main.py");

builder.Build().Run();
```

### Trust custom certificates in containers

When working with containerized services that need to trust custom certificates:

```csharp
using System.Security.Cryptography.X509Certificates;

var builder = DistributedApplication.CreateBuilder(args);

// Load custom CA certificates
var customCerts = new X509Certificate2Collection();
customCerts.Import("corporate-ca.pem");

var certBundle = builder.AddCertificateAuthorityCollection("corporate-certs")
    .WithCertificates(customCerts);

// Apply to container
builder.AddContainer("service", "myservice:latest")
    .WithCertificateAuthorityCollection(certBundle);

builder.Build().Run();
```

### Configure Python apps with certificate trust

Python applications require special handling due to their certificate trust model:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Use System mode for Python apps
builder.AddPythonModule("api", "./api", "uvicorn")
    .WithCertificateTrustScope(CertificateTrustScope.System);

builder.Build().Run();
```

## Limitations

Certificate trust customization has the following limitations:

- Currently supported only in run mode, not in publish mode.
- Not all languages and runtimes support all trust scope modes.
- Python applications don't natively support Append mode.
- Custom certificate trust requires appropriate runtime support within the resource.

## See also

- [Host external executables in Aspire](executable-resources.md)
- [Add Dockerfiles to the app model](withdockerfile.md)
- [AppHost configuration](configuration.md)
