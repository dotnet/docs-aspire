---
title: Deploy a ASP.NET Core app that connects to SQL Server to Azure
description: Learn how to deploy a ASP.NET Core app that connects to SQL Server to Azure
ms.date: 03/18/2024
ms.topic: how-to
---

# Tutorial: Deploy an ASP.NET Core Aspire + SQL Database app to Azure

In this tutorial, you learn to configure an .NET app that uses a SQL Server component for deployment to Azure. .NET Aspire provides multiple configurations that result in different resources provisioned in Azure. You'll learn how to:

> [!div class="checklist"]
>
> - Create a basic ASP.NET Core app that is set up to use .NET Aspire SQL Server components
> - Configure a .NET Aspire app to provision and deploy a SQL Server on Azure SQL Database
> - Configure a .NET Aspire app to provision and deploy a containerized SQL Server database

> [!NOTE]
> This document focuses specifically on .NET Aspire configurations to provision and deploy SQL Server resources in Azure. Visit the [Azure Container Apps deployment](/dotnet/aspire/deployment/azure/aca-deployment?branch=pr-en-us-532&tabs=visual-studio%2Clinux%2Cpowershell&pivots=azure-azd) tutorial to learn more about the full .NET  Aspire deployment process.

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

## Create the sample solution

# [Visual Studio](#tab/visual-studio)

1. At the top of Visual Studio, navigate to **File** > **New** > **Project**.
1. In the dialog window, search for *Blazor* and select **Blazor Web App**. Choose **Next**.
1. On the **Configure your new project** screen:
    - Enter a **Project Name** of **AspireSQL**.
    - Leave the rest of the values at their defaults and select **Next**.
1. On the **Additional information** screen:
    - Make sure **.NET 8.0** is selected.
    - Ensure the **Interactive render mode** is set to **None**.
    - Check the **Enlist in .NET Aspire orchestration** option and select **Create**.

Visual Studio creates a new ASP.NET Core solution that is structured to use .NET Aspire. The solution consists of the following projects:

- **AspireSQL**: A Blazor project that depends on service defaults.
- **AspireSQL.AppHost**: An orchestrator project designed to connect and configure the different projects and services of your app. The orchestrator should be set as the startup project.
- **AspireSQL.ServiceDefaults**: A shared class library to hold configurations that can be reused across the projects in your solution.

## [.NET CLI](#tab/cli)

In an empty directory, run the following command to create a new .NET Aspire app:

```dotnetcli
dotnet new aspire-sql --output AspireSql
```

The .NET CLI creates a new ASP.NET Core solution that is structured to use .NET Aspire. The solution consists of the following projects:

**AspireSQL**: A Blazor project that depends on service defaults.
**AspireSQL.AppHost**: An orchestrator project designed to connect and configure the different projects and services of your app. The orchestrator should be set as the startup project.
**AspireSQL.ServiceDefaults**: A shared class library to hold configurations that can be reused across the projects in your solution.

---

## Add the .NET Aspire component to the app

Add the [Aspire.Hosting.Azure](https://www.nuget.org/packages/Aspire.Hosting.Azure/8.0.0-preview.4.24156.9) and [Aspire.Microsoft.Data.Sqlclient](https://www.nuget.org/packages/Aspire.Microsoft.Data.SqlClient/8.0.0-preview.4.24156.9) packages to the _AspireSQL.AppHost_ project:

```dotnetcli
dotnet add package Aspire.Hosting.Azure --prerelease
dotnet add package Aspire.Microsoft.Data.SqlClient --prerelease
```

## Configure the AppHost for SQL Server deployment

.NET Aspire provides two built-in configuration options to streamline SQL Server provisioning and deployment on Azure:

- Provision a containerized SQL Server database using Azure Container Apps
- Provision an Azure SQL Database

Tools such as the Azure Developer CLI (`azd`) support these configurations to streamline .NET Aspire deployments. `azd` consumes these settings and provisions properly configured resources for you. The configuration examples below assume the use of `azd`.

> [!NOTE]
> You can also use the [Azure CLI](/dotnet/aspire/deployment/azure/aca-deployment?branch=pr-en-us-532&tabs=visual-studio%2Clinux%2Cpowershell&pivots=azure-cli) or [Bicep](/dotnet/aspire/deployment/azure/aca-deployment?branch=pr-en-us-532&tabs=visual-studio%2Clinux%2Cpowershell&pivots=azure-bicep) to provision and deploy .NET Aspire apps. These options are more manual but provide more granular control over your deployments. .NET Aspire apps can also connect to databases hosted on other services through manual configurations.

# [Azure SQL Database](#tab/azure-sql)

Replace the contents of the _Program.cs_ file in the _AspireSQL.AppHost_ project with the following code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.AspireSql_ApiService>("apiservice");

// Provisions an Azure SQL Database when published
var sqlServer = builder.AddSqlServer("sqlserver").PublishAsAzureSqlDatabase().AddDatabase("sqldb");

builder.AddProject<Projects.AspireSql_Web>("webfrontend")
    .WithReference(apiService)
    .WithReference(sqlServer);

builder.Build().Run();
```

The preceding code adds a SQL Server Container resource to your app and configures a connection to a database called `sqldata`. The `PublishAsAzureSqlDatabase` method ensures that an Azure SQL Database resources will be created when you deploy the app using supported tools such as the Azure Developer CLI.

:::image type="content" source="media/resources-azure-sql-database.png" alt-text="A screenshot showing the deployed Azure SQL Database.":::

## [SQL Server Container](#tab/sql-container)

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.AspireSql_ApiService>("apiservice");

// Provisions a containerized SQL Server database when published
var sqlServer = builder.AddSqlServer("sqlserver").AddDatabase("sqldb");

builder.AddProject<Projects.AspireSql_Web>("webfrontend")
    .WithReference(apiService)
    .WithReference(sqlServer);

builder.Build().Run();
```

The preceding code adds a SQL Server Container resource to your app and configures a connection to a database called `sqldata`. This configuration also ensures a containerized SQL Server instance will be deployed to Azure Container Apps when you deploy the app using supported tools such as the Azure Developer CLI.

:::image type="content" source="media/resources-azure-sql-container.png" alt-text="A screenshot showing the containerized SQL Database.":::

---
