---
title: .NET Aspire Azure Key Vault component
description: Lean about the .NET Aspire Azure Key Vault component.
ms.topic: how-to
ms.date: 04/18/2024
---

# .NET Aspire Azure Key Vault component

In this article, you learn how to use the .NET Aspire Azure Key Vault component. The `Aspire.Azure.Key.Vault` component library is used to register a <xref:Azure.Security.KeyVault.Secrets.SecretClient> in the DI container for connecting to Azure Key Vault. It also enables corresponding health checks, logging and telemetry.

## Get started

To get started with the .NET Aspire Azure Key Vault component, install the [Aspire.Azure.Security.KeyVault](https://www.nuget.org/packages/Aspire.Azure.Security.KeyVault) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Azure.Security.KeyVault --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Azure.Security.KeyVault"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _Program.cs_ file of your component-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireKeyVaultExtensions.AddAzureKeyVaultSecrets%2A> extension to register a `SecretClient` for use via the dependency injection container.

```csharp
builder.AddAzureKeyVaultSecrets("secrets");
```

You can then retrieve the <xref:Azure.Security.KeyVault.Secrets.SecretClient> instance using dependency injection. For example, to retrieve the client from a service:

```csharp
public class ExampleService(SecretClient client)
{
    // Use client...
}
```

## App host usage

To add Azure Key Vault hosting support to your <xref:Aspire.Hosting.IDistributedApplicationBuilder>, install the [Aspire.Hosting.Azure.KeyVault](https://www.nuget.org/packages/Aspire.Hosting.Azure.KeyVault) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.KeyVault --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.KeyVault"
                  Version="[SelectVersion]" />
```

---

In your app host project, register the Azure Key Vault component and consume the service using the following methods:

```csharp
// Service registration
var secrets = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureKeyVault("secrets")
    : builder.AddConnectionString("secrets");

// Service consumption
builder.AddProject<Projects.ExampleProject>()
       .WithReference(secrets)
```

## Configuration

The .NET Aspire Azure Key Vault component provides multiple options to configure the `SecretClient` based on the requirements and conventions of your project.

### Use configuration providers

The .NET Aspire Azure Key Vault component supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the <xref:Aspire.Azure.Security.KeyVault.AzureSecurityKeyVaultSettings> from _appsettings.json_ or other configuration files using `Aspire:Azure:Security:KeyVault` key.

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

If you have set up your configurations in the `Aspire:Azure:Security:KeyVault` section of your _appsettings.json_ file you can just call the method `AddAzureKeyVaultSecrets` without passing any parameters.

### Use inline delegates

You can also pass the `Action<AzureSecurityKeyVaultSettings>` delegate to set up some or all the options inline, for example to set the `VaultUri`:

```csharp
builder.AddAzureKeyVaultSecrets(
    "secrets",
    static settings => settings.VaultUri = new Uri("YOUR_VAULTURI"));
```

You can also set up the <xref:Azure.Security.KeyVault.Secrets.SecretClientOptions> using `Action<IAzureClientBuilder<SecretClient, SecretClientOptions>>` delegate, the second parameter of the `AddAzureKeyVaultSecrets` method. For example to set the <xref:Azure.Security.KeyVault.Keys.KeyClientOptions.DisableChallengeResourceVerification?displayProperty=nameWithType> ID to identify the client:

```csharp
builder.AddAzureKeyVaultSecrets(
    "secrets",
    static clientBuilder =>
        clientBuilder.ConfigureOptions(
            static options => options.DisableChallengeResourceVerification = true))
```

### Named instances

If you want to add more than one `SecretClient` you can use named instances. Load the named configuration section from the json config by calling the `AddAzureKeyVaultSecrets` method and passing in the `INSTANCE_NAME`.

```csharp
builder.AddAzureKeyVaultSecrets("INSTANCE_NAME");
```

The corresponding configuration JSON is defined as follows:

```json
{
  "Aspire": {
    "Azure": {
      "Security": {
        "KeyVault": {
          "INSTANCE_NAME": {
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
}
```

### Configuration options

The following configurable options are exposed through the <xref:Aspire.Azure.Security.KeyVault.AzureSecurityKeyVaultSettings> class:

| Name                  | Description                                                                                  |
|-----------------------|----------------------------------------------------------------------------------------------|
| `VaultUri`            | A URI to the vault on which the client operates. Appears as "DNS Name" in the Azure portal.  |
| `Credential`          | The credential used to authenticate to the Azure Key Vault.                                  |
| `DisableHealthChecks` | A boolean value that indicates whether the Key Vault health check is disabled or not.        |
| `DisableTracing`      | A boolean value that indicates whether the OpenTelemetry tracing is disabled or not.         |

[!INCLUDE [component-health-checks](../includes/component-health-checks.md)]

The .NET Aspire Azure Key Vault component includes the following health checks:

- Adds the `AzureKeyVaultSecretsHealthCheck` health check, which attempts to connect to and query the Key Vault
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic

[!INCLUDE [component-observability-and-telemetry](../includes/component-observability-and-telemetry.md)]

### Logging

The .NET Aspire Azure Key Vault component uses the following log categories:

- `Azure.Core`
- `Azure.Identity`

### Tracing

The .NET Aspire Azure Key Vault component will emit the following tracing activities using OpenTelemetry:

- "Azure.Security.KeyVault.Secrets.SecretClient"

### Metrics

The .NET Aspire Azure Key Vault component currently does not support metrics by default due to limitations with the Azure SDK.

## See also

- [Azure Key Vault docs](/azure/key-vault/general/)
- [.NET Aspire components](../fundamentals/components-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
