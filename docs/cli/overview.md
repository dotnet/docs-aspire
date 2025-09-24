---
title: .NET Aspire CLI Overview and Commands
description: Learn about the Aspire CLI commands for creating projects, running an AppHost, and adding integrations. Get started with command-line tools to build and manage distributed applications efficiently.
titleSuffix: ""
ms.date: 06/26/2025
ms.topic: overview
ms.custom:
  - ai-gen-docs-bap
  - ai-gen-title
  - ai-seo-date:06/26/2025
  - ai-gen-description
---

# Aspire CLI Overview

The Aspire CLI (`aspire` command) is a cross-platform tool that provides command-line functionality to create, manage, run, and publish polyglot Aspire projects. Use the Aspire CLI to streamline development workflows and coordinate services for distributed applications.

The Aspire CLI is an interactive-first experience.

- [Install the Aspire CLI.](install.md)
- [`aspire` command reference.](../cli-reference/aspire.md)

## Use templates

[_Command reference: `aspire new`_](../cli-reference/aspire-new.md)

The `aspire new` command is an interactive-first CLI experience, and is used to create one or more Aspire projects. As part of creating a project, Aspire CLI ensures that the latest Aspire project templates are installed into the `dotnet` system.

Use the `aspire new` command to create an Aspire project from a list of templates. Once a template is selected, the name of the project is set, and the output folder is chosen, `aspire` downloads the latest templates and generates one or more projects.

While command line parameters can be used to automate the creation of an Aspire project, the Aspire CLI is an interactive-first experience.

## Start the Aspire AppHost

[_Command reference: `aspire run`_](../cli-reference/aspire-run.md)

The `aspire run` command runs the AppHost project in development mode, which configures the Aspire environment, builds and starts resources defined by the AppHost, launches the web dashboard, and prints a list of endpoints.

When `aspire run` starts, it searches the current directory for an AppHost project. If a project isn't found, the sub directories are searched until one is found. If no AppHost project is found, Aspire stops. Once a project is found, Aspire CLI takes the following steps:

- Installs or verifies that Aspire's local hosting certificates are installed and trusted.
- Builds the AppHost project and its resources.
- Starts the AppHost and its resources.
- Starts the dashboard.

The following snippet is an example of the output displayed by the `aspire run` command:

```Aspire CLI
Dashboard:  https://localhost:17178/login?t=17f974bf68e390b0d4548af8d7e38b65

    Logs:  /home/vscode/.aspire/cli/logs/apphost-1295-2025-07-14-18-16-13.log
```

## Add integrations

[_Command reference: `aspire add`_](../cli-reference/aspire-add.md)

The `aspire add` command is an easy way to add official integration packages to your AppHost project. Use this as an alternative to a NuGet search through your IDE. You can run `aspire add <name|id>` if you know the name or NuGet ID of the integration package. If you omit a name or ID, the tool provides a list of packages to choose from. If you provide a partial name or ID, the tool filters the list of packages with items that match the provided value.

## Publish Aspire applications (preview)

[_Command reference: `aspire publish`_](../cli-reference/aspire-publish.md)

The `aspire publish` command publishes resources by serializing them to disk. When this command is run, Aspire invokes registered <xref:Aspire.Hosting.ApplicationModel.PublishingCallbackAnnotation> resource annotations, in the order they're declared. These annotations serialize a resource so that it can be consumed by deployment tools.

Some integrations automatically register a `PublishingCallbackAnnotation` for you, for example:

- <xref:Aspire.Hosting.Azure.AzureEnvironmentResource> generates Bicep assets.
- <xref:Aspire.Hosting.Docker.DockerComposeEnvironmentResource> generates docker-compose yaml.
- <xref:Aspire.Hosting.Kubernetes.KubernetesEnvironmentResource> generates Kubernetes Helm charts.

## Deploy Aspire applications (preview)

[_Command reference: `aspire deploy`_](../cli-reference/aspire-deploy.md)

The `aspire deploy` command is similar to `aspire publish`. After Aspire has invoked the publishing annotations to serialize resources to disk, it invokes `DeployingCallbackAnnotation` resource annotations, in the order they're declared.

As of Aspire 9.4, Aspire doesn't include any default deployment annotations for its resources, you must use the `DeployingCallbackAnnotation` to build your own.

> [!TIP]
> Consider this a good way to deploy your Aspire solution to a staging or testing environment.

## Manage Aspire configuration

[_Command reference: `aspire config`_](../cli-reference/aspire-config.md)

The `aspire config` command lets you manage Aspire CLI configuration settings. Use it to `list`, `get`, `set`, or `delete` configuration values that control CLI behavior. This command is also used to toggle features on or off.

For more information about Aspire CLI configuration, see [What is .NET Aspire configuration?](config-settings.md)

## Run commands in resource context (preview)

[_Command reference: `aspire exec`_](../cli-reference/aspire-exec.md)

The `aspire exec` command runs a command in the context of a specific Aspire resource, inheriting that resource's configuration, including environment variables, connection strings, and working directory. This is particularly useful for scenarios like running Entity Framework migrations where you need to run commands with the same configuration as your application. For example, you can run `aspire exec --resource api -- dotnet ef migrations add Init` to run Entity Framework commands with the proper database connection strings automatically configured.

[!INCLUDE [aspire exec feature flag note](includes/exec-feature-flag-note.md)]
