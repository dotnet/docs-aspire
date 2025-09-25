---
title: aspire deploy command
description: Learn about the aspire deploy command and its usage. This command first runs publishing mode, then invokes resource deployments declared by the AppHost.
ms.date: 09/25/2025
---
# aspire deploy command (Preview)

**This article applies to:** ✔️ Aspire CLI 9.4.0 and later versions.

<!--
> [!NOTE]
> This command is disabled by default. To use it, turn on the feature toggle by running:
>
> ```Aspire
> aspire config set features.deployCommandEnabled true
> ```
>
> For more information, see [aspire config command](aspire-config.md).
-->

## Name

`aspire deploy` - Deploy a codebase orchestrated with Aspire to specified targets.

[!INCLUDE [mode-preview](includes/mode-preview.md)]

## Synopsis

```Command
aspire deploy [options] [[--] <additional arguments>...]
```

## Description

The `aspire deploy` command first invokes the [`aspire publish`](./aspire-publish.md) command. After which, Aspire invokes all `DeployingCallbackAnnotation` resource annotations, in the order they're declared.

[!INCLUDE [project-search-logic-description](includes/project-search-logic-description.md)]

The command performs the following steps to deploy an app orchestrated with Aspire:

- Creates or modifies the `.aspire/settings.json` config file in the current directory, and sets the `appHostPath` config value to the path of the AppHost project file.
- Installs or verifies that Aspire's local hosting certificates are installed and trusted.
- Builds the AppHost project and its resources.
- Starts the AppHost and its resources.
- Invokes all <xref:Aspire.Hosting.ApplicationModel.PublishingCallbackAnnotation> resource annotations.
- Invokes all `DeployingCallbackAnnotation` resource annotations.

## Options

The following options are available:

- **`--`**

  Delimits arguments to `aspire publish` from arguments for the AppHost. All arguments after this delimiter are passed to the apphost.

- [!INCLUDE [option-project](includes/option-project.md)]

- **`-o, --output-path`**

  The output path for deployment artifacts. Defaults to a folder named _deploy_ in the current directory.

- [!INCLUDE [option-help](includes/option-help.md)]

- [!INCLUDE [option-debug](includes/option-debug.md)]

- [!INCLUDE [option-wait](includes/option-wait.md)]

## Examples

- Search the current directory structure for AppHost projects to build, publish, and deploy:

  ```Command
  aspire deploy
  ```

- Publish and deploy an Aspire apphost and its dependencies:

  ```Command
  aspire deploy --project './projects/apphost/orchestration.AppHost.csproj'
  ```

- Publish and deploy an Aspire AppHost with arguments:

  ```Command
  aspire deploy --project './projects/apphost/orchestration.AppHost.csproj' -- -fast
  ```
