---
title: Local deployment state
description: Learn how the aspire deploy command manages deployment state through cached configuration files.
ms.date: 10/17/2025
ai-usage: ai-assisted
---

# Local deployment state

The `aspire deploy` command manages deployment state through cached configuration files stored locally on your machine. This caching mechanism streamlines repeated deployments by preserving provisioning settings and parameters, making subsequent deployments faster and more efficient.

## Default behavior

### First deployment

When you run `aspire deploy` for the first time, the command:

1. Prompts for provisioning information (subscription ID, resource group name, location).
1. Prompts for deployment parameters (for example, API keys, connection strings).
1. Initiates the deployment process.
1. Saves all prompted values and deployment state to `~/.aspire/deployments/{AppHostSha}/production.json`.

### Subsequent deployments

On subsequent `aspire deploy` executions, the command:

1. Detects the existing deployment state file at `~/.aspire/deployments/{AppHostSha}/production.json`.
1. Notifies you that settings will be read from the cached file.
1. Prompts for confirmation to load the cached settings.
1. Loads the configuration from the cached file into the configuration provider.
1. Proceeds with deployment using the cached values (no re-prompting).

## Environment-specific deployments

### Specify an environment

Use the `--environment` flag to deploy to different environments:

```Aspire
aspire deploy --environment staging
```

**First deployment to a specific environment:**

- Prompts for all provisioning and parameter information.
- Saves deployment state to `~/.aspire/deployments/{AppHostSha}/{environment}.json` (for example, `staging.json`).

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

### Clear the cache

Use the `--clear-cache` flag to reset deployment state:

```Aspire
aspire deploy --clear-cache
```

**Behavior:**

1. Prompts for confirmation before deleting the cache for the specified environment.
1. Deletes the environment-specific deployment state file (for example, `~/.aspire/deployments/{AppHostSha}/production.json`).
1. Prompts for all provisioning and parameter information as if deploying for the first time.
1. Proceeds with deployment.
1. **Does not save the prompted values** to cache.

### Environment-specific cache clearing

The `--clear-cache` flag respects the environment context:

```Aspire
aspire deploy --environment staging --clear-cache
```

This clears only the `staging.json` cache file while leaving other environment caches (like `production.json`) intact.

## File storage location

- **Path pattern:** `~/.aspire/deployments/{AppHostSha}/{environment}.json`
- **Default environment:** `production`
- **AppHostSha:** A hash value representing the application host configuration, ensuring deployment states are specific to each application configuration.

## Security considerations

The deployment state files are stored locally on your machine in the `~/.aspire/deployments` directory. These files contain provisioning settings and parameter values, including secrets that might be associated with parameter resources. The `aspire deploy` command follows the same security pattern as .NET's user secrets manager:

- Files are stored outside of source code to mitigate against accidental secret leaks in version control.
- Secrets are stored in plain text in the local file system.
- Any process running under your user account can access these files.

Consider these security best practices:

- Ensure your local machine has appropriate security measures in place.
- Be cautious when sharing or backing up files from the `~/.aspire/deployments` directory.
- Use the `--clear-cache` flag when you need to change sensitive parameter values.

## Key points

- Each environment maintains its own isolated deployment state.
- Cached values persist across deployments unless explicitly cleared.
- The `--clear-cache` flag performs a one-time deployment without persisting new values.
- Environment selection can be specified via flag or environment variable.
- You're prompted for confirmation when loading cached settings.
- Cache files are stored per application (via AppHostSha) and per environment.

## See also

- [aspire deploy command reference](../../cli-reference/aspire-deploy.md)
- [Deploy to Azure Container Apps using Aspire CLI](aca-deployment-aspire-cli.md)
