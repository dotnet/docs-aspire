---
title: aspire publish command
description: Learn about the aspire publish command and its usage. This command invokes resource publishers declared by the apphost to serialize resources to disk.
ms.date: 07/11/2025
---
# aspire publish command

**This article applies to:** ✔️ Aspire CLI 9.4.0 and later versions.

## Name

`aspire publish` - Generates deployment artifacts for an Aspire AppHost project.

[!INCLUDE [mode-preview](includes/mode-preview.md)]

## Synopsis

```Command
aspire publish [options] [[--] <additional arguments>...]
```

## Description

The `aspire publish` command publishes resources by serializing them to disk. When this command is run, Aspire invokes registered <xref:Aspire.Hosting.ApplicationModel.PublishingCallbackAnnotation> annotations for resources, in the order they're declared. These annotations serialize a resource so that it can be consumed by deployment tools.

[!INCLUDE [project-search-logic-description](includes/project-search-logic-description.md)]

The command performs the following steps to run an Aspire AppHost:

- Creates or modifies the `.aspire/settings.json` config file in the current directory, and sets the `appHostPath` config value to the path of the AppHost project file.
- Installs or verifies that Aspire's local hosting certificates are installed and trusted.
- Builds the AppHost project and its resources.
- Starts the AppHost and its resources.
- Invokes all <xref:Aspire.Hosting.ApplicationModel.PublishingCallbackAnnotation> annotations for resources.

## Options

The following options are available:

- **`--`**

  Delimits arguments to `aspire publish` from arguments for the AppHost. All arguments after this delimiter are passed to the AppHost.

- [!INCLUDE [option-project](includes/option-project.md)]

- **`-o, --output-path`**

  The output path for the generated artifacts. Defaults the current directory.

- [!INCLUDE [option-help](includes/option-help.md)]

- [!INCLUDE [option-debug](includes/option-debug.md)]

- [!INCLUDE [option-wait](includes/option-wait.md)]

## Examples

- Search the current directory structure for AppHost projects to build and publish:

  ```Command
  aspire publish
  ```

- Publish a specific AppHost project:

  ```Command
  aspire publish --project './projects/apphost/orchestration.AppHost.csproj'
  ```

- Publish a specific AppHost project with arguments:

  ```Command
  aspire publish --project './projects/apphost/orchestration.AppHost.csproj' -- -fast
  ```
