---
title: .NET Aspire CLI Overview and Commands
description: Learn .NET Aspire CLI commands for creating projects, running an app host, and adding integrations. Get started with command-line tools to build and manage distributed applications efficiently.
ms.date: 06/26/2025
ms.topic: overview
ms.custom:
  - ai-gen-docs-bap
  - ai-gen-title
  - ai-seo-date:06/26/2025
  - ai-gen-description
---

# .NET Aspire CLI Overview

// TODO: More selling of why you want to use this instead of the .NET CLI you're probably already familiar with.

The .NET Aspire CLI (`aspire` command) is a .NET global tool that provides command-line functionality to create, manage, run, and publish Aspire projects. Use the .NET Aspire CLI to streamline development workflows for distributed applications.

The Aspire CLI is an interactive-first experience.

- [Install .NET Aspire CLI](install.md)

## Create projects

The `aspire new` command is an interactive-first CLI experience, and is used to create one or more Aspire projects. As part of creating a project, Azure CLI ensures that the latest Aspire project templates are installed into the `dotnet` system.

<!-- Add asciinema here -->

Use the `aspire new` command to create an Aspire project from a list of templates. Once a template is selected, the name of the project is set, and the output folder is chosen, `aspire` downloads the latest templates and generates one or more projects.

While command line parameters can be used to automate the creation of an Aspire project, the Aspire CLI is an interactive-first experience.

## Run the app host project

The `aspire run` command runs the app host project in development mode, which configures the Aspire environment, builds the app host, launches the web dashboard, and displays a terminal-based dashboard. This is similar to running the app host project in your IDE of choice, however, a terminal dashboard is also visible.

When `aspire run` starts, it searches the current directory for an app host. If an app host isn't found, the sub directories are searched until an app host is found. If no app host is found, Aspire stops. Once an app host is found, Aspire CLI takes the following stesp:

- Installs or verifies that Aspires local hosting certificates are installed and trusted.
- Builds the app host project.
- Starts the app host.
- Starts the dashboard.
- Displays a terminal-based dashboard.

<!-- Add asciinema here instead of the terminal dashboard -->

The following snippet is an example of the terminal dashboard:

```Aspire CLI
Dashboard:
📈  Direct: https://localhost:17077/login?t=64ebc6d760ab2c48df93607fd431cf0a

╭─────────────┬─────────┬─────────┬────────────────────────╮
│ Resource    │ Type    │ State   │ Endpoint(s)            │
├─────────────┼─────────┼─────────┼────────────────────────┤
│ apiservice  │ Project │ Running │ https://localhost:7422 │
│             │         │         │ http://localhost:5327  │
│ webfrontend │ Project │ Running │ https://localhost:7241 │
│             │         │         │ http://localhost:5185  │
╰─────────────┴─────────┴─────────┴────────────────────────╯
Press CTRL-C to stop the app host and exit.
```

## Add integrations

The `aspire add` command is an easy way to add official integration packages to your app host. Use this as an alternative to a NuGet search through your IDE. You can run `aspire add <name|id>` if you know the name or NuGet ID of the integration package, If you omit a name or ID, the tool provides a list of packages to choose from. If you provide a partial name or ID, the tool filters the list of packages with items that match the provided value.

<!-- Add asciinema here -->

## Publish Aspire applications

// TODO: Stuff about publishing

## Configure Aspire environment


