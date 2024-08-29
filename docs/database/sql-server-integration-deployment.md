---
title: Deploy a ASP.NET Core app that connects to SQL Server to Azure
description: Learn how to deploy a ASP.NET Core app that connects to SQL Server to Azure
ms.date: 08/12/2024
ms.topic: how-to
---

# Tutorial: Deploy a .NET Aspire project with a SQL Server Database to Azure

In this tutorial, you learn to configure an ASP.NET Core app with a SQL Server Database for deployment to Azure. .NET Aspire provides multiple SQL Server integration configurations that provision different database services in Azure. You'll learn how to:

> [!div class="checklist"]
>
> - Create a basic ASP.NET Core app that is configured to use the .NET Aspire SQL Server integration
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

In an empty directory, run the following command to create a new .NET Aspire project:

```dotnetcli
dotnet new aspire-starter --output AspireSQL
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

### Add the .NET Aspire integration to the app

Add the appropriate .NET Aspire integration to the _AspireSQL.AppHost_ project for your desired hosting service.

# [Azure SQL Database](#tab/azure-sql)

Add the [Aspire.Hosting.Azure.Sql](https://www.nuget.org/packages/Aspire.Hosting.Azure.Sql) package to the _AspireSQL.AppHost_ project:

```dotnetcli
dotnet add package Aspire.Hosting.Azure.Sql
```

## [SQL Server Container](#tab/sql-container)

Add the [Aspire.Hosting.SqlServer](https://www.nuget.org/packages/Aspire.Hosting.SqlServer) package to the _AspireSQL.AppHost_ project:

```dotnetcli
dotnet add package Aspire.Hosting.SqlServer
```

---

### Configure the AppHost project

Configure the _AspireSQL.AppHost_ project for your desired SQL database service.

# [Azure SQL Database](#tab/azure-sql)

Replace the contents of the _:::no-loc text="Program.cs":::_ file in the _AspireSQL.AppHost_ project with the following code:

:::code language="csharp" source="snippets/tutorial/aspiresqldeployazure/AspireSQL.AppHost/Program.cs":::

The preceding code adds a SQL Server Container resource to your app and configures a connection to a database called `sqldata`. The `PublishAsAzureSqlDatabase` method ensures that tools such as the Azure Developer CLI or Visual Studio create an Azure SQL Database resource during the deployment process.

## [SQL Server Container](#tab/sql-container)

Replace the contents of the Program.cs file in the AspireSQL.AppHost project with the following code:

:::code language="csharp" source="snippets/tutorial/aspiresqldeploycontainer/AspireSQL.AppHost/Program.cs":::

The preceding code adds a SQL Server Container resource to your app and configures a connection to a database called `sqldata`. This configuration also ensures that tools such as the Azure Developer CLI or Visual Studio create a containerized SQL Server instance during the deployment process.

---

## Deploy the app

Tools such as the [Azure Developer CLI](/azure/developer/azure-developer-cli/overview) (`azd`) support .NET Aspire SQL Server integration configurations to streamline deployments. `azd` consumes these settings and provisions properly configured resources for you.

[!INCLUDE [init-workflow](../deployment/azure/includes/init-workflow.md)]

[!INCLUDE [azd-up-workflow](../deployment/azure/includes/azd-up-workflow.md)]

## [Azure SQL Database](#tab/azure-sql)

The deployment process provisioned an Azure SQL Database resource due to the **.AppHost** configuration you provided.

:::image type="content" loc-scope="azure" source="media/resources-azure-sql-database.png" alt-text="A screenshot showing the deployed Azure SQL Database.":::

## [SQL Server Container](#tab/sql-container)

The deployment process created a SQL Server app container due to the **.AppHost** configuration you provided.

:::image type="content" loc-scope="azure" source="media/resources-azure-sql-container.png" alt-text="A screenshot showing the containerized SQL Database.":::

---

[!INCLUDE [clean-up-resources](../includes/clean-up-resources.md)]

## See also

- [.NET Aspire deployment via Azure Container Apps](../deployment/azure/aca-deployment.md)
- [.NET Aspire Azure Container Apps deployment deep dive](../deployment/azure/aca-deployment-azd-in-depth.md)
- [Deploy a .NET Aspire project using GitHub Actions](../deployment/azure/aca-deployment-github-actions.md)
