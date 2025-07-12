---
title: aspire command
description: Learn about the aspire command (the generic driver for the Aspire CLI) and its usage.
ms.date: 07/11/2025
ms.topic: include
---

The Aspire CLI uses the following logic, in order, to determine which apphost project to process:

- The `--project` option.

  This option specifies the path to a project to process.

- The `.aspire/settings.json` config file.

  If the config file path exists in the current directory, it's used. If not, the CLI walks up the directory structure looking for the config file. If it finds a config file, it reads the `appHostPath` setting value as the project to process.

- Searches the current directory and subdirectories.

  Starting in the current directory, the CLI gathers all apphost projects from that directory and below. If a single project is discovered, it's automatically selected. If mutliple projects are discovered, they're printed to the terminal for the user to manually select one of the projects.
  
  Once a project selected, either automatically or manually, the path to the project is stored in the `.aspire/settings.json` config file.
