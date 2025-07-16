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

- [Install .NET Aspire CLI.](install.md)
- [`aspire` command reference.](../cli-reference/aspire.md)

## Create projects

[_Command reference: `aspire new`_](../cli-reference/aspire-new.md)

The `aspire new` command is an interactive-first CLI experience, and is used to create one or more Aspire projects. As part of creating a project, Azure CLI ensures that the latest Aspire project templates are installed into the `dotnet` system.

<!-- Add asciinema here -->

Use the `aspire new` command to create an Aspire project from a list of templates. Once a template is selected, the name of the project is set, and the output folder is chosen, `aspire` downloads the latest templates and generates one or more projects.

While command line parameters can be used to automate the creation of an Aspire project, the Aspire CLI is an interactive-first experience.

## Run the apphost project

[_Command reference: `aspire run`_](../cli-reference/aspire-run.md)

The `aspire run` command runs the apphost project in development mode, which configures the Aspire environment, builds the apphost, launches the web dashboard, and prints a list of endpoints.

When `aspire run` starts, it searches the current directory for an apphost. If an apphost isn't found, the sub directories are searched until an apphost is found. If no apphost is found, Aspire stops. Once an apphost is found, Aspire CLI takes the following steps:

- Installs or verifies that Aspires local hosting certificates are installed and trusted.
- Builds the apphost project.
- Starts the apphost.
- Starts the dashboard.

The following snippet is an example of the output displayed by the `aspire run` command:

```Aspire CLI
Dashboard:  https://localhost:17178/login?t=17f974bf68e390b0d4548af8d7e38b65                                         
            https://literate-eureka-55x5r74pwxv2vx7p-17178.app.github.dev/login?t=17f974bf68e390b0d4548af8d7e38b65   
                                                                                                                    
    Logs:  /home/vscode/.aspire/cli/logs/apphost-1295-2025-07-14-18-16-13.log                                       
                                                              
Endpoints:  webfrontend has endpoint https://localhost:7294   
            webfrontend has endpoint http://localhost:5131   
            apiservice has endpoint https://localhost:7531   
            apiservice has endpoint http://localhost:5573   
```

## Add integrations

[_Command reference: `aspire add`_](../cli-reference/aspire-add.md)

The `aspire add` command is an easy way to add official integration packages to your apphost. Use this as an alternative to a NuGet search through your IDE. You can run `aspire add <name|id>` if you know the name or NuGet ID of the integration package, If you omit a name or ID, the tool provides a list of packages to choose from. If you provide a partial name or ID, the tool filters the list of packages with items that match the provided value.

<!-- Add asciinema here -->

## Publish Aspire applications (preview)

[_Command reference: `aspire publish`_](../cli-reference/aspire-publish.md)

The `aspire publish` command publishes resources, serializing data into JSON format. When this command is run, Aspire invokes registered <xref:Aspire.Hosting.ApplicationModel.PublishingCallbackAnnotation> annotations for resources. These annotations serialize a resource so that it can be consumed by deployment tools.

## Deploy Aspire applications (preview)

[_Command reference: `aspire deploy`_](../cli-reference/aspire-deploy.md)

The `aspire deploy` command is similar to `aspire publish`, except that in addition to serializing resources, it also deploys resources that have registered a `DeployingCallbackAnnotation` annotation.

> [!TIP]
> Consider this a good way to deploy your Aspire solution to a staging area for testing.

## Run commands in resource context (preview)

[_Command reference: `aspire exec`_](../cli-reference/aspire-exec.md)

The `aspire exec` command runs commands in the context of a specific Aspire resource, inheriting that resource's configuration including environment variables, connection strings, and working directory. This is particularly useful for scenarios like running Entity Framework migrations where you need to run commands with the same configuration as your application. For example, you can run `aspire exec --resource api -- dotnet ef migrations add Init` to run Entity Framework commands with the proper database connection strings automatically configured.
