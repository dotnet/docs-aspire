---
title: .NET Aspire Azure Key Vault integration
description: Lean about the .NET Aspire Azure Key Vault integration.
ms.topic: how-to
ms.date: 08/12/2024
---

# .NET Aspire Azure Key Vault integration

In this article, you learn how to use the .NET Aspire Azure Key Vault integration. The `Aspire.Azure.Key.Vault` integration library is used to register a <xref:Azure.Security.KeyVault.Secrets.SecretClient> in the DI container for connecting to Azure Key Vault. It also enables corresponding health checks, logging and telemetry.

## Get started

To get started with the .NET Aspire Azure Key Vault integration, install the [Aspire.Azure.Security.KeyVault](https://www.nuget.org/packages/Aspire.Azure.Security.KeyVault) NuGet package in the client-consuming project, i.e., the project for the application that uses the Azure Key Vault client.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Azure.Security.KeyVault
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Azure.Security.KeyVault"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

THe following sections describe various example usages.

### Add secrets to configuration

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireKeyVaultExtensions.AddAzureKeyVaultSecrets%2A> extension to add the secrets in the Azure Key Vault to the application's Configuration. The method takes a connection name parameter.

```csharp
builder.Configuration.AddAzureKeyVaultSecrets("secrets");
```

You can then retrieve a secret through normal <xref:Microsoft.Extensions.Configuration.IConfiguration> APIs. For example, to retrieve a secret from a service:

```csharp
public class ExampleService(IConfiguration configuration)
{
    string secretValue = configuration["secretKey"];
    // Use secretValue ...
}
```

### Use `SecretClient`

Alternatively, you can use a `SecretClient` to retrieve the secrets on demand. In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireKeyVaultExtensions.AddAzureKeyVaultClient%2A> extension to register a `SecretClient` for use via the dependency injection container.

```csharp
builder.AddAzureKeyVaultClient("secrets");
```

You can then retrieve the <xref:Azure.Security.KeyVault.Secrets.SecretClient> instance using dependency injection. For example, to retrieve the client from a service:

```csharp
public class ExampleService(SecretClient client)
{
    // Use client...
}
```

## App host usage

To add Azure Key Vault hosting support to your <xref:Aspire.Hosting.IDistributedApplicationBuilder>, install the [Aspire.Hosting.Azure.KeyVault](https://www.nuget.org/packages/Aspire.Hosting.Azure.KeyVault) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.KeyVault
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.KeyVault"
                  Version="*" />
```

---

In your app host project, register the Azure Key Vault integration and consume the service using the following methods:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var secrets = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureKeyVault("secrets")
    : builder.AddConnectionString("secrets");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(secrets)
```

The preceding code conditionally adds the Azure Key Vault resource to the project based on the execution context. If the app host is executing in publish mode, the resource is added otherwise the connection string to an existing resource is added.

## Configuration

The .NET Aspire Azure Key Vault integration provides multiple options to configure the `SecretClient` based on the requirements and conventions of your project.

### Use configuration providers

The .NET Aspire Azure Key Vault integration supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the <xref:Aspire.Azure.Security.KeyVault.AzureSecurityKeyVaultSettings> from _:::no-loc text="appsettings.json":::_ or other configuration files using `Aspire:Azure:Security:KeyVault` key.

```json
{
  "Aspire": {
    "Azure": {
      "Security": {
        "KeyVault": {
          "VaultUri": "YOUR_VAULT_URI",
          "DisableHealthChecks": false,
          "DisableTracing": true,
          "ClientOptions": {
            "DisableChallengeResourceVerification": true
          }
        }
      }
    }
  }
}
```

If you have set up your configurations in the `Aspire:Azure:Security:KeyVault` section of your _:::no-loc text="appsettings.json":::_ file you can just call the method `AddAzureKeyVaultSecrets` without passing any parameters.

### Use inline delegates

You can also pass the `Action<AzureSecurityKeyVaultSettings>` delegate to set up some or all the options inline, for example to set the `VaultUri`:

```csharp
builder.AddAzureKeyVaultSecrets(
    "secrets",
    static settings => settings.VaultUri = new Uri("YOUR_VAULTURI"));
```

> [!TIP]
> The `AddAzureKeyVaultSecrets` API name has caused a bit of confusion. The method is used to configure the `SecretClient` and not to add secrets to the configuration.

You can also set up the <xref:Azure.Security.KeyVault.Secrets.SecretClientOptions> using `Action<IAzureClientBuilder<SecretClient, SecretClientOptions>>` delegate, the second parameter of the `AddAzureKeyVaultSecrets` method. For example to set the <xref:Azure.Security.KeyVault.Keys.KeyClientOptions.DisableChallengeResourceVerification?displayProperty=nameWithType> ID to identify the client:

```csharp
builder.AddAzureKeyVaultSecrets(
    "secrets",
    static clientBuilder =>
        clientBuilder.ConfigureOptions(
            static options => options.DisableChallengeResourceVerification = true))
```

### Configuration options

The following configurable options are exposed through the <xref:Aspire.Azure.Security.KeyVault.AzureSecurityKeyVaultSettings> class:

| Name                  | Description                                                                                  |
|-----------------------|----------------------------------------------------------------------------------------------|
| `VaultUri`            | A URI to the vault on which the client operates. Appears as "DNS Name" in the Azure portal.  |
| `Credential`          | The credential used to authenticate to the Azure Key Vault.                                  |
| `DisableHealthChecks` | A boolean value that indicates whether the Key Vault health check is disabled or not.        |
| `DisableTracing`      | A boolean value that indicates whether the OpenTelemetry tracing is disabled or not.         |

[!INCLUDE [integration-health-checks](../includes/integration-health-checks.md)]

The .NET Aspire Azure Key Vault integration includes the following health checks:

- Adds the `AzureKeyVaultSecretsHealthCheck` health check, which attempts to connect to and query the Key Vault
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

### Logging

The .NET Aspire Azure Key Vault integration uses the following log categories:

- `Azure.Core`
- `Azure.Identity`

### Tracing

The .NET Aspire Azure Key Vault integration will emit the following tracing activities using OpenTelemetry:

- "Azure.Security.KeyVault.Secrets.SecretClient"

### Metrics

The .NET Aspire Azure Key Vault integration currently does not support metrics by default due to limitations with the Azure SDK.

## See also

- [Azure Key Vault docs](/azure/key-vault/general/)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
