---
title: Deployment state caching
description: Learn how the aspire deploy command manages deployment state through cached configuration files.
ms.date: 10/17/2025
ai-usage: ai-assisted
---

# Deployment state caching

The `aspire deploy` command manages deployment state through cached configuration files stored locally on your machine. This caching mechanism streamlines repeated deployments by preserving provisioning settings and parameters, making subsequent deployments faster and more efficient.

## Default behavior

The `aspire deploy` command automatically manages deployment state based on whether cached configuration exists for your application and target environment.

### First deployment

When you run `aspire deploy` for the first time, or for the first time in a target `--environment`, the command:

1. Prompts for provisioning information (subscription ID, resource group name, location).
1. Prompts for deployment parameters (for example, API keys, connection strings).
1. Initiates the deployment process.
1. Saves all prompted values and deployment state to `~/.aspire/deployments/{AppHostSha}/production.json`.

### Subsequent deployments

On subsequent `aspire deploy` executions, the command:

1. Detects the existing deployment state file at _~/.aspire/deployments/{AppHostSha}/production.json_.
1. Notifies you that settings will be read from the cached file.
1. Prompts for confirmation to load the cached settings.
1. Loads the configuration from the cached file into the configuration provider.
1. Proceeds with deployment using the cached values (no re-prompting).

## Environment-specific deployments

Different deployment environments (such as development, staging, and production) typically require different configurations, resource names, and connection strings. The `aspire deploy` command supports environment-specific deployments, ensuring that each environment maintains isolated deployment state.

### Specify an environment

Use the `--environment` flag to deploy to different environments:

```Aspire
aspire deploy --environment staging
```

**First deployment to a specific environment:**

- Prompts for all provisioning and parameter information.
- Saves deployment state to _~/.aspire/deployments/{AppHostSha}/{environment}.json_ (for example, _staging.json_).

**Subsequent deployments:**

- Reads the environment-specific cached file.
- Loads configuration from the cached state.
- Uses cached values without prompting.

### Environment variable support

The deployment environment can also be specified using the `DOTNET_ENVIRONMENT` environment variable:

```bash
export DOTNET_ENVIRONMENT=staging && aspire deploy
```

This behaves identically to using the `--environment` flag, loading the appropriate cached configuration file.

## Cache management

The `aspire deploy` command provides mechanisms to manage cached deployment state, giving you control over when to use cached values and when to start fresh.

### Clear the cache

Use the `--clear-cache` flag to reset deployment state:

```Aspire
aspire deploy --clear-cache
```

**Behavior:**

1. Prompts for confirmation before deleting the cache for the specified environment.
1. Deletes the environment-specific deployment state file (for example, _~/.aspire/deployments/{AppHostSha}/production.json_).
1. Prompts for all provisioning and parameter information as if deploying for the first time.
1. Proceeds with deployment.
1. **Does not save the prompted values** to cache.

### Environment-specific cache clearing

The `--clear-cache` flag respects the environment context:

```Aspire
aspire deploy --environment staging --clear-cache
```

This clears only the _staging.json_ cache file while leaving other environment caches (like _production.json_) intact.

## File storage location

- **Path pattern:** _~/.aspire/deployments/{AppHostSha}/{environment}.json_.
- **Default environment:** `production`.
- **AppHostSha:** A hash value representing the application host configuration, ensuring deployment states are specific to each application configuration.

## Use deployment state in CI/CD pipelines

When using the `aspire deploy` command in continuous integration and deployment (CI/CD) pipelines, you might want to persist deployment state across pipeline runs. This approach can be useful for maintaining consistent deployment configurations without manual intervention.

### GitHub Actions example

The following example demonstrates how to cache deployment state in a GitHub Actions workflow using the `actions/cache` action:

```yaml
name: Deploy to Azure

on:
  push:
    branches: [ main ]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v4
    
    - name: Cache Aspire deployment state
      uses: actions/cache@v4
      with:
        path: ~/.aspire/deployments
        key: aspire-deploy-${{ hashFiles('**/AppHost.csproj') }}-${{ github.ref }}
        restore-keys: |
          aspire-deploy-${{ hashFiles('**/AppHost.csproj') }}-
          aspire-deploy-
    
    - name: Deploy with Aspire CLI
      run: aspire deploy --environment production
      env:
        AZURE_CREDENTIALS: ${{ secrets.AZURE_CREDENTIALS }}
```

This workflow caches the _~/.aspire/deployments_ directory, using the AppHost project file hash and branch reference as cache keys. The `actions/cache` action automatically restores the cache before the deployment step and saves any updates to the cache after the job completes. Subsequent workflow runs restore the cached deployment state, allowing automated deployments without re-prompting for configuration values.

> [!CAUTION]
> When caching deployment state in CI/CD pipelines, ensure that your pipeline has appropriate access controls and secret management practices in place, as the cached state might contain sensitive configuration values.

## Security considerations

The deployment state files are stored locally on your machine in the _~/.aspire/deployments_ directory. These files contain provisioning settings and parameter values, including secrets that might be associated with parameter resources. The `aspire deploy` command follows the same security pattern as .NET's user secrets manager:

- Files are stored outside of source code to mitigate against accidental secret leaks in version control.
- Secrets are stored in plain text in the local file system.
- Any process running under your user account can access these files.

Consider these security best practices:

- Ensure your local machine has appropriate security measures in place.
- Be cautious when sharing or backing up files from the _~/.aspire/deployments_ directory.
- Use the `--clear-cache` flag when you need to change sensitive parameter values.

## Key points

- Each environment maintains its own isolated deployment state.
- Cached values persist across deployments unless explicitly cleared.
- The `--clear-cache` flag performs a one-time deployment without persisting new values.
- Environment selection can be specified via flag or environment variable.
- You're prompted for confirmation when loading cached settings.
- Cache files are stored per application (via AppHostSha) and per environment.

## See also

- [aspire deploy command reference](../cli-reference/aspire-deploy.md)
- [Deploy to Azure Container Apps using Aspire CLI](aspire-deploy/aca-deployment-aspire-cli.md)
