---
title: Deploy a Aspire project with a SQL Server Database to Azure
description: Learn how to deploy a Aspire app with a SQL Server Database that connects to Azure
ms.date: 11/08/2024
ms.topic: how-to
---

# Tutorial: Deploy a Aspire project with a SQL Server Database to Azure

In this tutorial, you learn to configure an ASP.NET Core app with a SQL Server Database for deployment to Azure. Aspire provides multiple SQL Server integration configurations that provision different database services in Azure. You'll learn how to:

> [!div class="checklist"]
>
> - Create a basic ASP.NET Core app that is configured to use the Aspire SQL Server integration
> - Configure the app to provision an Azure SQL Database
> - Configure the app to provision a containerized SQL Server database

> [!NOTE]
> This document focuses specifically on Aspire configurations to provision and deploy SQL Server resources in Azure. Visit the [Azure Container Apps deployment](/dotnet/aspire/deployment/azure/aca-deployment?branch=pr-en-us-532&tabs=visual-studio%2Clinux%2Cpowershell&pivots=azure-azd) tutorial to learn more about the full Aspire deployment process.

[!INCLUDE [aspire-prereqs](../../includes/aspire-prereqs.md)]

## Create the sample solution

# [Visual Studio](#tab/visual-studio)

1. At the top of Visual Studio, navigate to **File** > **New** > **Project**.
1. In the dialog window, search for *Aspire* and select **Aspire Starter App**. Choose **Next**.
1. On the **Configure your new project** screen:
    - Enter a  **Solution name** of **AspireSql**.
    - Leave the rest of the values at their defaults and select **Next**.
1. On the **Additional information** screen:
    - In the **Framework** list, verify that **.NET 9.0** is selected.
    - In the **Aspire version** list, verify that **9.1** is selected.
    - Choose **Create**.

Visual Studio creates a new ASP.NET Core solution that is structured to use Aspire. The solution consists of the following projects:

- **AspireSql.ApiService**: An API project that depends on service defaults.
- **AspireSql.AppHost**: An orchestrator project designed to connect and configure the different projects and services of your app. The orchestrator should be set as the startup project.
- **AspireSql.ServiceDefaults**: A shared class library to hold configurations that can be reused across the projects in your solution.
- **AspireSql.Web**: A Blazor project that depends on service defaults.

## [.NET CLI](#tab/cli)

In an empty directory, run the following command to create a new Aspire project:

```dotnetcli
dotnet new aspire-starter --output AspireSql
```

The .NET CLI creates a new ASP.NET Core solution that is structured to use Aspire. The solution consists of the following projects:

- **AspireSql.Web**: A Blazor project that depends on service defaults.
- **AspireSql.ApiService**: An API project that depends on service defaults.
- **AspireSql.AppHost**: An orchestrator project designed to connect and configure the different projects and services of your app. The orchestrator should be set as the startup project.
- **AspireSql.ServiceDefaults**: A shared class library to hold configurations that can be reused across the projects in your solution.

---

## Configure the app for SQL Server deployment

Aspire provides two built-in configuration options to streamline SQL Server deployment on Azure:

- Provision a containerized SQL Server database using Azure Container Apps
- Provision an Azure SQL Database instance

### Add the Aspire integration to the app

Add the appropriate Aspire integration to the _AspireSql.AppHost_ project for your desired hosting service.

# [Azure SQL Database](#tab/azure-sql)

Open a command prompt and add the [📦 Aspire.Hosting.Azure.Sql](https://www.nuget.org/packages/Aspire.Hosting.Azure.Sql) NuGet package to the _AspireSql.AppHost_ project:

```dotnetcli
cd AspireSql.AppHost
dotnet add package Aspire.Hosting.Azure.Sql
```

## [SQL Server Container](#tab/sql-container)

Open a command prompt and add the [📦 Aspire.Hosting.SqlServer](https://www.nuget.org/packages/Aspire.Hosting.SqlServer) NuGet package to the _AspireSql.AppHost_ project:

```dotnetcli
cd AspireSql.AppHost
dotnet add package Aspire.Hosting.SqlServer
```

---

### Configure the AppHost project

Configure the _AspireSql.AppHost_ project for your desired SQL database service.

# [Azure SQL Database](#tab/azure-sql)

Replace the contents of the _:::no-loc text="Program.cs":::_ file in the _AspireSql.AppHost_ project with the following code:

:::code language="csharp" source="../../database/snippets/tutorial/aspiresqldeployazure/AspireSql.AppHost/AppHost.cs":::

The preceding code adds an Azure SQL Server resource to your app and configures a connection to a database called `sqldb`. The <xref:Aspire.Hosting.AzureSqlExtensions.AddAzureSqlServer*> method ensures that tools such as the Azure Developer CLI or Visual Studio create an Azure SQL Database resource during the deployment process.

## [SQL Server Container](#tab/sql-container)

Replace the contents of the _:::no-loc text="Program.cs":::_ file in the _AspireSql.AppHost_ project with the following code:

:::code language="csharp" source="../../database/snippets/tutorial/aspiresqldeploycontainer/AspireSql.AppHost/AppHost.cs":::

The preceding code adds a SQL Server resource to your app and configures a connection to a database called `sqldb`. This configuration also ensures that tools such as the Azure Developer CLI or Visual Studio create a containerized SQL Server instance during the deployment process.

---

## Deploy the app

The `aspire deploy` command supports Aspire SQL Server integration configurations to streamline deployments. The command consumes these settings and provisions properly configured resources for you.

To deploy your app to Azure Container Apps, run the following command from the _AspireSql.AppHost_ directory:

```Aspire
aspire deploy
```

When you run the `aspire deploy` command for the first time, you'll be prompted to:

1. **Sign in to Azure**: Follow the authentication prompts to sign in to your Azure account.
1. **Select a subscription**: Choose the Azure subscription you want to use for deployment.
1. **Select or create a resource group**: Choose an existing resource group or create a new one.
1. **Select a location**: Choose the Azure region where you want to deploy your resources.

The deployment process will provision the necessary Azure resources and deploy your Aspire app.

## [Azure SQL Database](#tab/azure-sql)

The deployment process provisioned an Azure SQL Database resource due to the **.AppHost** configuration you provided.

:::image type="content" loc-scope="azure" source="../../database/media/resources-azure-sql-database.png" alt-text="A screenshot showing the deployed Azure SQL Database.":::

## [SQL Server Container](#tab/sql-container)

The deployment process created a SQL Server app container due to the **.AppHost** configuration you provided.

:::image type="content" loc-scope="azure" source="../../database/media/resources-azure-sql-container.png" alt-text="A screenshot showing the containerized SQL Database.":::

---

[!INCLUDE [clean-up-resources](../../includes/clean-up-resources.md)]

## See also

- [Deploy a Aspire project to Azure Container Apps](../azd/aca-deployment.md)
- [Deploy a Aspire project to Azure Container Apps using the Azure Developer CLI (in-depth guide)](../azd/aca-deployment-azd-in-depth.md)
- [Tutorial: Deploy a Aspire project using the Azure Developer CLI](../azd/aca-deployment-github-actions.md)
