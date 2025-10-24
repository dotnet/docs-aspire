---
title: Certificate trust customization in Aspire
description: Learn how to customize trusted certificates for Executable and Container resources in Aspire to enable secure communication.
ms.date: 10/20/2025
ai-usage: ai-assisted
---

# Certificate trust customization in Aspire

In Aspire, you can customize which certificates resources consider trusted for TLS/HTTPS traffic. This is particularly useful for resources that don't use the system's root trusted certificates by default, such as containerized applications, Python apps, and Node.js apps. By configuring certificate trust, you enable these resources to communicate securely with services that use certificates they wouldn't otherwise trust, including the Aspire dashboard's OTLP endpoint.

> [!IMPORTANT]
> Certificate trust customization only applies at run time. Custom certificates aren't included in publish or deployment artifacts.

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
    .WithDeveloperCertificateTrust(trust: true);

// Disable development certificate trust
var pythonApp = builder.AddPythonApp("api", "../api", "main.py")
    .WithDeveloperCertificateTrust(trust: false);

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
certificates.ImportFromPemFile("path/to/certificate.pem");

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

The `WithCertificateTrustScope` API accepts a <xref:Aspire.Hosting.ApplicationModel.CertificateTrustScope> value to specify the trust behavior.

### Default trust scopes

Different resource types have different default trust scopes:

- **Append**: The default for most resources, appending custom certificates to the default trusted certificates.
- **System**: The default for Python projects, which combines custom certificates with system root certificates because Python doesn't properly support Append mode.
- **None**: The default for .NET projects on Windows, as there's no way to automatically change the default system store source.

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
    .WithExecutableCertificateTrustCallback((ctx) =>
    {
        // Add a command line argument that must be set to enable custom certificates
        ctx.CertificateTrustArguments.Add("--use-custom-ca");
        
        // Add a command line argument that expects the path to a bundle (single file) of the custom CA certificates
        ctx.CertificateBundleArguments.Add("--ca-file");
        
        // Add an environment variable that expects the path to a bundle (single file) of the custom CA certificates
        ctx.CertificateBundleEnvironment.Add("EXTRA_CA_BUNDLE");
    });

builder.Build().Run();
```

The callback provides access to the certificate collection and allows you to specify command-line arguments required to configure trusted certificates.

### Container resource certificate trust

Use `WithContainerCertificateTrustCallback` to customize certificate trust for container resources:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddContainer("api", "myimage")
    .WithContainerCertificateTrustCallback((ctx) =>
    {
        // Override the path to default individual certificates in the container (this is a list of common certificate paths for various Linux distros by default)
        // This should only need to be updated if your container has certificates in non-standard paths
        ctx.DefaultContainerCertificatesDirectoryPaths.Clear();
        ctx.DefaultContainerCertificatesDirectoryPaths.Add("/path/to/custom/certs");
        
        // Same as above, by default this is a collection of the default locations of the system certificate authority bundle file for common Linux distros
        // You should only need to customize this if your image uses non-standard certificate paths
        ctx.DefaultContainerCertificateAuthorityBundlePaths.Clear();
        ctx.DefaultContainerCertificateAuthorityBundlePaths.Add("/path/to/custom/certbundle.pem");
        
        // Add environment variables that should be set with a path to the additional CA certificates as its value
        // By default this includes "SSL_CERT_DIR" for OpenSSL compatibility
        ctx.CertificatesDirectoryEnvironment.Add("EXTRA_CERTS");
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

### Disable certificate trust for Python apps

Python projects use System mode by default. To disable certificate trust customization for a Python app:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Disable certificate trust for Python apps
builder.AddPythonModule("api", "./api", "uvicorn")
    .WithCertificateTrustScope(CertificateTrustScope.None);

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
- [Add Dockerfiles to your .NET app model](withdockerfile.md)
- [AppHost configuration](configuration.md)
