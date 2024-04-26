---
title: Deploy a ASP.NET Core app that connects to SQL Server to Azure
description: Learn how to deploy a ASP.NET Core app that connects to SQL Server to Azure
ms.date: 03/18/2024
ms.topic: how-to
---

# Tutorial: Deploy a .NET Aspire app with a SQL Server Database to Azure

In this tutorial, you learn to configure an ASP.NET Core app with a SQL Server Database for deployment to Azure. .NET Aspire provides multiple SQL Server component configurations that provision different database services in Azure. You'll learn how to:

> [!div class="checklist"]
>
> - Create a basic ASP.NET Core app that is configured to use the .NET Aspire SQL Server component
> - Configure the app to provision an Azure SQL Database
> - Configure the app to provision a containerized SQL Server database

> [!NOTE]
> This document focuses specifically on .NET Aspire configurations to provision and deploy SQL Server resources in Azure. Visit the [Azure Container Apps deployment](/dotnet/aspire/deployment/azure/aca-deployment?branch=pr-en-us-532&tabs=visual-studio%2Clinux%2Cpowershell&pivots=azure-azd) tutorial to learn more about the full .NET Aspire deployment process.

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

## Create the sample solution

# [Visual Studio](#tab/visual-studio)

1. At the top of Visual Studio, navigate to **File** > **New** > **Project**.
1. In the dialog window, search for *Aspire* and select **.NET Aspire - Starter Application**. Choose **Next**.
1. On the **Configure your new project** screen:
    - Enter a **Project Name** of **AspireSQL**.
    - Leave the rest of the values at their defaults and select **Next**.
1. On the **Additional information** screen:
    - Verify that **.NET 8.0** is selected and choose **Create**.

Visual Studio creates a new ASP.NET Core solution that is structured to use .NET Aspire. The solution consists of the following projects:

- **AspireSQL.Web**: A Blazor project that depends on service defaults.
- **AspireSQL.ApiService**: An API project that depends on service defaults.
- **AspireSQL.AppHost**: An orchestrator project designed to connect and configure the different projects and services of your app. The orchestrator should be set as the startup project.
- **AspireSQL.ServiceDefaults**: A shared class library to hold configurations that can be reused across the projects in your solution.

## [.NET CLI](#tab/cli)

In an empty directory, run the following command to create a new .NET Aspire app:

```dotnetcli
dotnet new aspire-starter --output AspireSql
```

The .NET CLI creates a new ASP.NET Core solution that is structured to use .NET Aspire. The solution consists of the following projects:

- **AspireSQL.Web**: A Blazor project that depends on service defaults.
- **AspireSQL.ApiService**: An API project that depends on service defaults.
- **AspireSQL.AppHost**: An orchestrator project designed to connect and configure the different projects and services of your app. The orchestrator should be set as the startup project.
- **AspireSQL.ServiceDefaults**: A shared class library to hold configurations that can be reused across the projects in your solution.

---

## Configure the app for SQL Server deployment

.NET Aspire provides two built-in configuration options to streamline SQL Server deployment on Azure:

- Provision a containerized SQL Server database using Azure Container Apps
- Provision an Azure SQL Database instance

### Add the .NET Aspire component to the app

Add the appropriate .NET Aspire component to the _AspireSQL.AppHost_ project for your desired hosting service.

# [Azure SQL Database](#tab/azure-sql)

Add the [Aspire.Hosting.Azure.Sql](https://www.nuget.org/packages/Aspire.Hosting.Azure.Sql) package to the _AspireSQL.AppHost_ project:

```dotnetcli
dotnet add package Aspire.Hosting.Azure.Sql --prerelease
```

## [SQL Server Container](#tab/sql-container)

Add the [Aspire.Hosting.SqlServer](https://www.nuget.org/packages/Aspire.Hosting.SqlServer) package to the _AspireSQL.AppHost_ project:

```dotnetcli
dotnet add package Aspire.Hosting.SqlServer --prerelease
```

---

### Configure the AppHost project

Configure the _AspireSQL.AppHost_ project for your desired SQL database service.

# [Azure SQL Database](#tab/azure-sql)

Replace the contents of the _Program.cs_ file in the _AspireSQL.AppHost_ project with the following code:

:::code language="csharp" source="snippets/tutorial/aspiresqldeployazure/AspireSql.AppHost/Program.cs":::

The preceding code adds a SQL Server Container resource to your app and configures a connection to a database called `sqldata`. The `PublishAsAzureSqlDatabase` method ensures that tools such as the Azure Developer CLI or Visual Studio create an Azure SQL Database resource during the deployment process.

## [SQL Server Container](#tab/sql-container)

Replace the contents of the Program.cs file in the AspireSQL.AppHost project with the following code:

:::code language="csharp" source="snippets/tutorial/aspiresqldeploycontainer/AspireSql.AppHost/Program.cs":::

The preceding code adds a SQL Server Container resource to your app and configures a connection to a database called `sqldata`. This configuration also ensures that tools such as the Azure Developer CLI or Visual Studio create a containerized SQL Server instance during the deployment process.

---

## Deploy the app

Tools such as the [Azure Developer CLI](/azure/developer/azure-developer-cli/overview) (`azd`) support .NET Aspire SQL Server component configurations to streamline deployments. `azd` consumes these settings and provisions properly configured resources for you.

> [!NOTE]
> You can also use the [Azure CLI](/dotnet/aspire/deployment/azure/aca-deployment?branch=pr-en-us-532&tabs=visual-studio%2Clinux%2Cpowershell&pivots=azure-cli) or [Bicep](/dotnet/aspire/deployment/azure/aca-deployment?branch=pr-en-us-532&tabs=visual-studio%2Clinux%2Cpowershell&pivots=azure-bicep) to provision and deploy .NET Aspire app resources. These options require more manual steps, but provide more granular control over your deployments. .NET Aspire apps can also connect to databases hosted on other services through manual configurations.

1. Open a terminal window in the root of your .NET Aspire project.

1. Run the `azd init` command to initialize the project with `azd`.

    ```azdeveloper
    azd init
    ```

1. When prompted for an environment name, enter *docs-aspiresql*.

1. Run the `azd up` command to begin the deployment process:

    ```azdeveloper
    azd up
    ```

1. When prompted, choose to expose the **webfrontend** service to the internet.

1. Select the Azure subscription that should host your app resources.

1. Select the Azure location to use.

    The Azure Developer CLI provisions and deploys your app resources. The process may take a few minutes to complete.

    [!INCLUDE [azd-up-output](../deployment/azure/includes/azd-up-output.md)]

1. When the deployment finishes, click the resource group link in the output to view the created resources in the Azure portal.

## [Azure SQL Database](#tab/azure-sql)

The deployment process provisioned an Azure SQL Database resource due to the **.AppHost** configuration you provided.

:::image type="content" source="media/resources-azure-sql-database.png" alt-text="A screenshot showing the deployed Azure SQL Database.":::

## [SQL Server Container](#tab/sql-container)

The deployment process created a SQL Server app container due to the **.AppHost** configuration you provided.

:::image type="content" source="media/resources-azure-sql-container.png" alt-text="A screenshot showing the containerized SQL Database.":::

---

[!INCLUDE [clean-up-resources](../includes/clean-up-resources.md)]

## See also

- [.NET Aspire deployment via Azure Container Apps](../deployment/azure/aca-deployment.md)
- [.NET Aspire Azure Container Apps deployment deep dive](../deployment/azure/aca-deployment-azd-in-depth.md)
- [Deploy a .NET Aspire app using GitHub Actions](../deployment/azure/aca-deployment-github-actions.md)
