---
title: .NET Aspire Azure Key Vault integration
description: Learn about the .NET Aspire Azure Key Vault integration.
ms.date: 07/22/2025
uid: security/azure-security-key-vault-integration
ms.custom: sfi-ropc-nochange
---

# .NET Aspire Azure Key Vault integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[Azure Key Vault](/azure/key-vault/) is a cloud service for securely storing and accessing secrets. The .NET Aspire Azure Key Vault integration enables you to connect to Azure Key Vault instances from your .NET applications.

## Hosting integration

The Azure Key Vault hosting integration models a Key Vault resource as the <xref:Aspire.Hosting.Azure.AzureKeyVaultResource> type. To access this type and APIs for expressing them within your [app host](xref:dotnet/aspire/app-host) project, install the [📦 Aspire.Hosting.Azure.KeyVault](https://www.nuget.org/packages/Aspire.Hosting.Azure.KeyVault) NuGet package:

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

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add Azure Key Vault resource

In your app host project, call <xref:Aspire.Hosting.AzureKeyVaultResourceExtensions.AddAzureKeyVault*> on the builder instance to add an Azure Key Vault resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var keyVault = builder.AddAzureKeyVault("key-vault");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(keyVault);

// After adding all resources, run the app...
```

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` named `"key-vault"`.

> [!IMPORTANT]
> By default, `AddAzureKeyVault` configures a [Key Vault Administrator built-in role](/azure/role-based-access-control/built-in-roles/security#key-vault-administrator).

> [!TIP]
> When you call <xref:Aspire.Hosting.AzureKeyVaultResourceExtensions.AddAzureKeyVault*>, it implicitly calls <xref:Aspire.Hosting.AzureProvisionerExtensions.AddAzureProvisioning*>, which adds support for generating Azure resources dynamically during app startup. The app must configure the appropriate subscription and location. For more information, see [Local provisioning: Configuration](../azure/local-provisioning.md#configuration).

#### Provisioning-generated Bicep

If you're new to [Bicep](/azure/azure-resource-manager/bicep/overview), it's a domain-specific language for defining Azure resources. With .NET Aspire, you don't need to write Bicep by-hand, instead the provisioning APIs generate Bicep for you. When you publish your app, the generated Bicep is output alongside the manifest file. When you add an Azure Key Vault resource, the following Bicep is generated:

:::code language="bicep" source="../snippets/azure/AppHost/key-vault/key-vault.bicep":::

The preceding Bicep is a module that provisions an Azure Key Vault resource. Additionally, role assignments are created for the Azure resource in a separate module:

:::code language="bicep" source="../snippets/azure/AppHost/key-vault-roles/key-vault-roles.bicep":::

The generated Bicep is a starting point and is influenced by changes to the provisioning infrastructure in C#. Customizations to the Bicep file directly will be overwritten, so make changes through the C# provisioning APIs to ensure they are reflected in the generated files.

#### Customize provisioning infrastructure

All .NET Aspire Azure resources are subclasses of the <xref:Aspire.Hosting.Azure.AzureProvisioningResource> type. This type enables the customization of the generated Bicep by providing a fluent API to configure the Azure resources by using the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},System.Action{Aspire.Hosting.Azure.AzureResourceInfrastructure})> API. For example, you can configure the `sku`, `RBAC`, `tags`, and more. The following example demonstrates how to customize the Azure Key Vault resource:

:::code language="csharp" source="../snippets/azure/AppHost/Program.ConfigureKeyVaultInfra.cs" id="configure":::

The preceding code:

- Chains a call to the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure*> API:
  - The `infra` parameter is an instance of the <xref:Aspire.Hosting.Azure.AzureResourceInfrastructure> type.
  - The provisionable resources are retrieved by calling the <xref:Azure.Provisioning.Infrastructure.GetProvisionableResources> method.
  - The single <xref:Azure.Provisioning.KeyVault.KeyVaultService> resource is retrieved.
  - The `Sku` property is set to a new <xref:Azure.Provisioning.KeyVault.KeyVaultSku> instance.
  - The <xref:Azure.Provisioning.KeyVault.KeyVaultProperties.EnableRbacAuthorization?displayProperty=nameWithType> property is set to `true`.
  - A tag is added to the resource with a key of `ExampleKey` and a value of `Example value`.

There are many more configuration options available to customize the Key Vault resource. For more information, see <xref:Azure.Provisioning.KeyVault> and [Azure.Provisioning customization](../azure/customize-azure-resources.md#azureprovisioning-customization).

### Connect to an existing Azure Key Vault instance

You might have an existing Azure AI Key Vault instance that you want to connect to. You can chain a call to annotate that your <xref:Aspire.Hosting.Azure.AzureKeyVaultResource> is an existing resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var existingKeyVaultName = builder.AddParameter("existingKeyVaultName");
var existingKeyVaultResourceGroup = builder.AddParameter("existingKeyVaultResourceGroup");

var keyvault = builder.AddAzureKeyVault("ke-yvault")
                    .AsExisting(existingKeyVaultName, existingKeyVaultResourceGroup);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(keyvault);

// After adding all resources, run the app...
```

[!INCLUDE [azure-configuration](../azure/includes/azure-configuration.md)]

For more information on treating Azure Key Vault resources as existing resources, see [Use existing Azure resources](../azure/integrations-overview.md#use-existing-azure-resources).

> [!NOTE]
> Alternatively, instead of representing an Azure Key Vault resource, you can add a connection string to the app host. This approach is weakly-typed, and doesn't work with role assignments or infrastructure customizations. For more information, see [Add existing Azure resources with connection strings](../azure/integrations-overview.md#add-existing-azure-resources-with-connection-strings).

## Client integration

To get started with the .NET Aspire Azure Key Vault client integration, install the [📦 Aspire.Azure.Security.KeyVault](https://www.nuget.org/packages/Aspire.Azure.Security.KeyVault) NuGet package in the client-consuming project, that is, the project for the application that uses the Azure Key Vault client.

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

The client integration provides two ways to access secrets from Azure Key Vault:

- Add secrets to app configuration, using either the `IConfiguration` or the `IOptions<T>` pattern.
- Use a `SecretClient` to retrieve secrets on demand.

### Add secrets to configuration

In the :::no-loc text="Program.cs"::: file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireKeyVaultExtensions.AddAzureKeyVaultSecrets*> extension method on the <xref:Microsoft.Extensions.Configuration.IConfiguration> to add the secrets as part of your app's configuration. The method takes a connection name parameter.

```csharp
builder.Configuration.AddAzureKeyVaultSecrets(connectionName: "key-vault");
```

> [!NOTE]
> The `AddAzureKeyVaultSecrets` API name has caused a bit of confusion. The method is used to configure the `SecretClient` based on the given connection name, and _it's not used_ to add secrets to the configuration.

> [!TIP]
> The `connectionName` parameter must match the name used when adding the Azure Key Vault resource in the app host project. For more information, see [Add Azure Key Vault resource](#add-azure-key-vault-resource).

You can then retrieve a secret-based configuration value through the normal <xref:Microsoft.Extensions.Configuration.IConfiguration> APIs, or even by binding to strongly-typed classes with the [options pattern](/dotnet/core/extensions/options). To retrieve a secret from an example service class that's been registered with the dependency injection container, consider the following snippets:

#### Retrieve `IConfiguration` instance

```csharp
public class ExampleService(IConfiguration configuration)
{
    // Use configuration...
    private string _secretValue = configuration["SecretKey"];
}
```

The preceding example assumes that you've also registered the `IConfiguration` instance for dependency injection. For more information, see [Dependency injection in .NET](/dotnet/core/extensions/dependency-injection).

#### Retrieve `IOptions<T>` instance

```csharp
public class ExampleService(IOptions<SecretOptions> options)
{
    // Use options...
    private string _secretValue = options.Value.SecretKey;
}
```

The preceding example assumes that you've configured a `SecretOptions` class for use with the options pattern. For more information, see [Options pattern in .NET](/dotnet/core/extensions/options).

Additional `AddAzureKeyVaultSecrets` API parameters are available optionally for the following scenarios:

- `Action<AzureSecurityKeyVaultSettings>? configureSettings`: To set up some or all the options inline.
- `Action<SecretClientOptions>? configureClientOptions`: To set up the <xref:Azure.Security.KeyVault.Secrets.SecretClientOptions> inline.
- `AzureKeyVaultConfigurationOptions? options`: To configure the <xref:Azure.Extensions.AspNetCore.Configuration.Secrets.AzureKeyVaultConfigurationOptions> inline.

### Add an Azure Secret client

Alternatively, you can use the <xref:Azure.Security.KeyVault.Secrets.SecretClient> directly to retrieve the secrets on demand. This requires a slightly different registration API.

In the :::no-loc text="Program.cs"::: file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireKeyVaultExtensions.AddAzureKeyVaultClient*> extension on the <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> instance to register a `SecretClient` for use via the dependency injection container.

```csharp
builder.AddAzureKeyVaultClient(connectionName: "key-vault");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the Azure Key Vault resource in the app host project. For more information, see [Add Azure Key Vault resource](#add-azure-key-vault-resource).

After adding the `SecretClient` to the builder, you can get the <xref:Azure.Security.KeyVault.Secrets.SecretClient> instance using dependency injection. For example, to retrieve the client from an example service define it as a constructor parameter and ensure the `ExampleService` class is registered with the dependency injection container:

```csharp
public class ExampleService(SecretClient client)
{
    // Use client...
}
```

For more information on dependency injection, see [.NET dependency injection](/dotnet/core/extensions/dependency-injection).

### Add keyed Azure Key Vault client

There might be situations where you want to register multiple `SecretClient` instances with different connection names. To register keyed Azure Key Vault clients, call the <xref:Microsoft.Extensions.Hosting.AspireKeyVaultExtensions.AddKeyedAzureKeyVaultClient*> method:

```csharp
builder.AddKeyedAzureKeyVaultClient(name: "feature-toggles");
builder.AddKeyedAzureKeyVaultClient(name: "admin-portal");
```

Then you can retrieve the `SecretClient` instances using dependency injection. For example, to retrieve the client from an example service:

```csharp
public class ExampleService(
    [FromKeyedServices("feature-toggles")] SecretClient featureTogglesClient,
    [FromKeyedServices("admin-portal")] SecretClient adminPortalClient)
{
    // Use clients...
}
```

For more information on keyed services, see [.NET dependency injection: Keyed services](/dotnet/core/extensions/dependency-injection#keyed-services).

### Configuration

The .NET Aspire Azure Key Vault integration provides multiple options to configure the `SecretClient` based on the requirements and conventions of your project.

#### Use configuration providers

The .NET Aspire Azure Key Vault integration supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the <xref:Aspire.Azure.Security.KeyVault.AzureSecurityKeyVaultSettings> from _:::no-loc text="appsettings.json":::_ or other configuration files using `Aspire:Azure:Security:KeyVault` key.

```json
{
  "Aspire": {
    "Azure": {
      "Security": {
        "KeyVault": {
          "DisableHealthChecks": true,
          "DisableTracing": false,
          "ClientOptions": {
            "Diagnostics": {
              "ApplicationId": "myapp"
            }
          }
        }
      }
    }
  }
}
```

For the complete Azure Key Vault client integration JSON schema, see [Aspire.Azure.Security.KeyVault/ConfigurationSchema.json](https://github.com/dotnet/aspire/blob/v9.1.0/src/Components/Aspire.Azure.Security.KeyVault/ConfigurationSchema.json).

#### Use named configuration

The .NET Aspire Azure Key Vault integration supports named configuration, which allows you to configure multiple instances of the same resource type with different settings. The named configuration uses the connection name as a key under the main configuration section.

```json
{
  "Aspire": {
    "Azure": {
      "Security": {
        "KeyVault": {
          "vault1": {
            "VaultUri": "https://myvault1.vault.azure.net/",
            "DisableHealthChecks": true,
            "ClientOptions": {
              "Diagnostics": {
                "ApplicationId": "myapp1"
              }
            }
          },
          "vault2": {
            "VaultUri": "https://myvault2.vault.azure.net/",
            "DisableTracing": true,
            "ClientOptions": {
              "Diagnostics": {
                "ApplicationId": "myapp2"
              }
            }
          }
        }
      }
    }
  }
}
```

In this example, the `vault1` and `vault2` connection names can be used when calling `AddAzureKeyVaultSecrets`:

```csharp
builder.AddAzureKeyVaultSecrets("vault1");
builder.AddAzureKeyVaultSecrets("vault2");
```

Named configuration takes precedence over the top-level configuration. If both are provided, the settings from the named configuration override the top-level settings.

If you have set up your configurations in the `Aspire:Azure:Security:KeyVault` section of your _:::no-loc text="appsettings.json":::_ file you can just call the method `AddAzureKeyVaultSecrets` without passing any parameters.

#### Use inline delegates

You can also pass the `Action<AzureSecurityKeyVaultSettings>` delegate to set up some or all the options inline, for example to set the <xref:Aspire.Azure.Security.KeyVault.AzureSecurityKeyVaultSettings.VaultUri?displayProperty=nameWithType>:

```csharp
builder.AddAzureKeyVaultSecrets(
    connectionName: "key-vault",
    configureSettings: settings => settings.VaultUri = new Uri("KEY_VAULT_URI"));
```

You can also set up the <xref:Azure.Security.KeyVault.Secrets.SecretClientOptions> using `Action<SecretClientOptions>` delegate, which is an optional parameter of the `AddAzureKeyVaultSecrets` method. For example to set the <xref:Azure.Security.KeyVault.Keys.KeyClientOptions.DisableChallengeResourceVerification?displayProperty=nameWithType> ID to identify the client:

```csharp
builder.AddAzureKeyVaultSecrets(
    connectionName: "key-vault",
    configureClientOptions: options => options.DisableChallengeResourceVerification = true))
```

### Configuration options

The following configurable options are exposed through the <xref:Aspire.Azure.Security.KeyVault.AzureSecurityKeyVaultSettings> class:

| Name | Description |
|--|--|
| <xref:Aspire.Azure.Security.KeyVault.AzureSecurityKeyVaultSettings.Credential?displayProperty=nameWithType> | The credential used to authenticate to the Azure Key Vault. |
| <xref:Aspire.Azure.Security.KeyVault.AzureSecurityKeyVaultSettings.DisableHealthChecks?displayProperty=nameWithType> | A boolean value that indicates whether the Key Vault health check is disabled or not. |
| <xref:Aspire.Azure.Security.KeyVault.AzureSecurityKeyVaultSettings.DisableTracing?displayProperty=nameWithType> | A boolean value that indicates whether the OpenTelemetry tracing is disabled or not. |
| <xref:Aspire.Azure.Security.KeyVault.AzureSecurityKeyVaultSettings.VaultUri?displayProperty=nameWithType> | A URI to the vault on which the client operates. Appears as "DNS Name" in the Azure portal. |

[!INCLUDE [client-integration-health-checks](../includes/client-integration-health-checks.md)]

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

- `Azure.Security.KeyVault.Secrets.SecretClient`

### Metrics

The .NET Aspire Azure Key Vault integration currently does not support metrics by default due to limitations with the Azure SDK.

## See also

- [Azure Key Vault docs](/azure/key-vault/general/)
- [Video: Introduction to Azure Key Vault and .NET Aspire](https://www.youtube.com/watch?v=1K5riRctUIg)
- [.NET Aspire Azure integrations overview](../azure/integrations-overview.md)
- [.NET Aspire integrations overview](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
