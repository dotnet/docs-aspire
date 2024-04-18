## Azure provisioning credential store

The .NET Aspire app host uses a credential store for Azure resource authentication and authorization. Depending on your subscription, the correct credential store may be needed for multi-tenant provisioning scenarios.

With the [Aspire.Hosting.Azure](https://nuget.org/packages/Aspire.Hosting.Azure) NuGet package installed, and if your app host depends on Azure resources, the default Azure credential store relies on the <xref:Azure.Identity.DefaultAzureCredential>. To change this behavior, you can set the credential store value in the _appsettings.json_ file, as shown in the following example:

```json
{
  "Azure": {
    "CredentialStore": "AzureCLI"
  }
}
```

As with all [configuration-based settings](/dotnet/core/extensions/configuration), you can configure these with alternative providers, such as, [user secrets](/aspnet/core/security/app-secrets), or [environment variables](/dotnet/core/extensions/configuration-providers#environment-variable-configuration-provider). The `Azure:CredentialStore` value can be set to one of the following values:

- `AzureCLI`: Delegates to the <xref:Azure.Identity.AzureCliCredential>.
- `AzurePowerShell`: Delegates to the <xref:Azure.Identity.AzurePowerShellCredential>.
- `VisualStudio`: Delegates to the <xref:Azure.Identity.VisualStudioCredential>.
- `VisualStudioCode`: Delegates to the <xref:Azure.Identity.VisualStudioCodeCredential>.
- `AzureDeveloperCLI`: Delegates to the <xref:Azure.Identity.AzureDeveloperCliCredential>.
- `InteractiveBrowser`: Delegates to the <xref:Azure.Identity.InteractiveBrowserCredential>.
