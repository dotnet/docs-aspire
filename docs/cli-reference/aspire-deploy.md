---
title: aspire deploy command
description: Learn about the aspire deploy command and its usage. This command first runs publishing mode, then invokes resource deployments declared by the app host.
ms.date: 07/11/2025
---
# aspire deploy command

**This article applies to:** ✔️ Aspire CLI 9.4.0 and later versions.

## Name

`aspire deploy` - Deploy an Aspire app host project to its supported deployment targets.

[!INCLUDE [mode-preview](includes/mode-preview.md)]

## Synopsis

```dotnetcli
aspire deploy [options] [[--] <additional arguments>...]
```

## Description

The `aspire deploy` command is similar to [`aspire publish`](./aspire-publish.md). After Aspire has invoked the publishing annotations, it invokes `DeployingCallbackAnnotation` resource annotations, in the order they're declared.

[!INCLUDE [project-search-logic-description](includes/project-search-logic-description.md)]

The command performs the following steps to run an apphost project:

- Writes the apphost project path to the `.aspire/settings.json` config file in the current directory.
- Installs or verifies that Aspires local hosting certificates are installed and trusted.
- Builds the apphost project.
- Starts the apphost in publish mode.
- Invokes all <xref:Aspire.Hosting.ApplicationModel.PublishingCallbackAnnotation> resource annotations.
- Invokes all `DeployingCallbackAnnotation` resource annotations.

## Options

The following options are available:

- **`--`**

  Delimits arguments to aspire publish from arguments for the apphost. All arguments after this delimiter are passed to the apphost.

- [!INCLUDE [option-project](includes/option-project.md)]

- **`-o, --output-path`**

  The output path for deployment artifacts. Defaults the a folder named _deploy_ in the current directory.

- [!INCLUDE [option-help](includes/option-help.md)]

- [!INCLUDE [option-debug](includes/option-debug.md)]

- [!INCLUDE [option-wait](includes/option-wait.md)]

## Examples

- Search the current directory structure for apphost projects to build, publish, and deploy:

  ```Command
  aspire deploy
  ```

- Publish and deploy a specific apphost project:

  ```Command
  aspire deploy --project './projects/apphost/orchestration.AppHost.csproj'
  ```

- Publish and deploy a specific apphost project with arguments:

  ```Command
  aspire deploy --project './projects/apphost/orchestration.AppHost.csproj' -- -fast
  ```
