---
title: aspire publish command
description: Learn about the aspire publish command and its usage. This command invokes resource publishers declared by the app host to serialize resources to disk.
ms.date: 07/11/2025
---
# aspire publish command

**This article applies to:** ✔️ Aspire CLI 9.4.0 and later versions.

## Name

`aspire publish` - Generates deployment artifacts for an Aspire app host project.

[!INCLUDE [mode-preview](includes/mode-preview.md)]

## Synopsis

```dotnetcli
aspire publish [options] [[--] <additional arguments>...]
```

## Description

The `aspire publish` command publishes resources by serializing them to disk. When this command is run, Aspire invokes registered <xref:Aspire.Hosting.ApplicationModel.PublishingCallbackAnnotation> annotations for resources, in the order they're declared. These annotations serialize a resource so that it can be consumed by deployment tools.

[!INCLUDE [project-search-logic-description](includes/project-search-logic-description.md)]

The command performs the following steps to run an apphost project:

- Writes the apphost project path to the `.aspire/settings.json` config file in the current directory.
- Installs or verifies that Aspires local hosting certificates are installed and trusted.
- Builds the apphost project.
- Starts the apphost in publish mode.
- Invokes all <xref:Aspire.Hosting.ApplicationModel.PublishingCallbackAnnotation> annotations for resources.

## Options

The following options are available:

- **`--`**

  Delimits arguments to aspire publish from arguments for the apphost. All arguments after this delimiter are passed to the apphost.

- [!INCLUDE [option-project](includes/option-project.md)]

- **`-o, --output-path`**

  The output path for the generated artifacts. Defaults the current directory.

- [!INCLUDE [option-help](includes/option-help.md)]

- [!INCLUDE [option-debug](includes/option-debug.md)]

- [!INCLUDE [option-wait](includes/option-wait.md)]

## Examples

- Search the current directory structure for apphost projects to build and publish:

  ```Command
  aspire publish
  ```

- Publish a specific apphost project:

  ```Command
  aspire publish --project './projects/apphost/orchestration.AppHost.csproj'
  ```

- Publish a specific apphost project with arguments:

  ```Command
  aspire publish --project './projects/apphost/orchestration.AppHost.csproj' -- -fast
  ```
