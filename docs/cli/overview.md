---
title: .NET Aspire CLI Overview and Commands
description: Learn .NET Aspire CLI commands for creating projects, running an apphost, and adding integrations. Get started with command-line tools to build and manage distributed applications efficiently.
ms.date: 06/26/2025
ms.topic: overview
ms.custom:
  - ai-gen-docs-bap
  - ai-gen-title
  - ai-seo-date:06/26/2025
  - ai-gen-description
---

# .NET Aspire CLI Overview

The Aspire CLI (`aspire` command) is a cross-platform tool that provides command-line functionality to create, manage, run, and publish polyglot Aspire projects. Use the Aspire CLI to streamline development workflows and coordinate services for distributed applications.

The Aspire CLI is an interactive-first experience.

- [Install .NET Aspire CLI](install.md)

## Commands

The following table describes the basic commands provided by Aspire:

| Command                                                    | Status  | Function                                                                  |
|------------------------------------------------------------|---------|---------------------------------------------------------------------------|
| [`aspire add`](#add-integrations)                          | Stable  | Add an integration to the Aspire project.                                 |
| [`aspire new`](#create-projects)                           | Stable  | Create an Aspire sample project from a template.                          |
| [`aspire run`](#run-the-apphost-project)                   | Stable  | Run an Aspire apphost in development mode.                                |
| [`aspire exec`](#run-commands-in-resource-context-preview) | Preview | Similar to the `aspire run` command, but runs commands in the context of a resource.  |
| [`aspire deploy`](#deploy-aspire-applications-preview)     | Preview | Deploys the artifacts created by `aspire publish`.                        |
| [`aspire publish`](#publish-aspire-applications-preview)   | Preview | Generates deployment artifacts for an Aspire apphost project.             |

<!-- These commands aren't used yet

| [`aspire config`](#configure-aspire-environment)         | Stable  | Configures the Aspire environment.                                        |
| `aspire init`                                            | Future  | ... |

-->

## Create projects

The `aspire new` command is an interactive-first CLI experience, and is used to create one or more Aspire projects. As part of creating a project, Azure CLI ensures that the latest Aspire project templates are installed into the `dotnet` system.

<!-- Add asciinema here -->

Use the `aspire new` command to create an Aspire project from a list of templates. Once a template is selected, the name of the project is set, and the output folder is chosen, `aspire` downloads the latest templates and generates one or more projects.

While command line parameters can be used to automate the creation of an Aspire project, the Aspire CLI is an interactive-first experience.

## Run the apphost project

The `aspire run` command runs the apphost project in development mode, which configures the Aspire environment, builds the apphost, launches the web dashboard, and displays a terminal-based dashboard. This is similar to running the apphost project in your IDE of choice, however, a terminal-based version dashboard is also visible.

When `aspire run` starts, it searches the current directory for an apphost. If an apphost isn't found, the sub directories are searched until an apphost is found. If no apphost is found, Aspire stops. Once an apphost is found, Aspire CLI takes the following steps:

- Installs or verifies that Aspires local hosting certificates are installed and trusted.
- Builds the apphost project.
- Starts the apphost.
- Starts the dashboard.
- Displays a terminal-based dashboard.

<!-- Add asciinema here instead of the terminal dashboard -->

The following snippet is an example of the terminal dashboard:

```Aspire CLI
Dashboard:
📈  Direct: https://localhost:17077/login?t=64ebc6d760ab2c48df93607fd431cf0b

╭─────────────┬─────────┬─────────┬────────────────────────╮
│ Resource    │ Type    │ State   │ Endpoint(s)            │
├─────────────┼─────────┼─────────┼────────────────────────┤
│ apiservice  │ Project │ Running │ https://localhost:7422 │
│             │         │         │ http://localhost:5327  │
│ webfrontend │ Project │ Running │ https://localhost:7241 │
│             │         │         │ http://localhost:5185  │
╰─────────────┴─────────┴─────────┴────────────────────────╯
Press CTRL-C to stop the apphost and exit.
```

## Add integrations

The `aspire add` command is an easy way to add official integration packages to your apphost. Use this as an alternative to a NuGet search through your IDE. You can run `aspire add <name|id>` if you know the name or NuGet ID of the integration package, If you omit a name or ID, the tool provides a list of packages to choose from. If you provide a partial name or ID, the tool filters the list of packages with items that match the provided value.

<!-- Add asciinema here -->

## Publish Aspire applications (preview)

The `aspire publish` command publishes resources, serializing data into JSON format. When this command is run, Aspire invokes registered <xref:Aspire.Hosting.ApplicationModel.PublishingCallbackAnnotation> annotations for resources. These annotations serialize a resource so that it can be consumed by deployment tools.

## Deploy Aspire applications (preview)

The `aspire deploy` command is similar to `aspire publish`, except that in addition to serializing resources, it also deploys resources that have registered a `DeployingCallbackAnnotation` annotation.

> [!TIP]
> Consider this a good way to deploy your Aspire solution to a staging area for testing.

## Run commands in resource context (preview)

The `aspire exec` command runs commands in the context of a specific Aspire resource, inheriting that resource's configuration including environment variables, connection strings, and working directory. This is particularly useful for scenarios like running Entity Framework migrations where you need to run commands with the same configuration as your application. For example, you can run `aspire exec --resource api -- dotnet ef migrations add Init` to run Entity Framework commands with the proper database connection strings automatically configured.
