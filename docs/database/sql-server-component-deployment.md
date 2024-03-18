---
title: Connect an ASP.NET Core app to SQL Server using .NET Aspire and Entity Framework Core
description: Learn how to connect an ASP.NET Core app to to SQL Server using .NET Aspire and Entity Framework Core.
ms.date: 01/22/2024
ms.topic: how-to
---

# Tutorial: Deploy a ASP.NET Core app that connects to SQL Server to Azure

In this tutorial, you deploy an ASP.NET Core app that uses a .NET Aspire SQL Server component to Azure. .NET Aspire provides multiple configurations that result in different resources provisioned in Azure. You'll learn how to:

> [!div class="checklist"]
>
> - Create a basic ASP.NET Core app that is set up to use .NET Aspire SQL Server components
> - Configure the ASP.NET Core app to create an Azure SQL Database when published
> - Configure and use .NET Aspire Component features to read and write from the database

> [!NOTE]
> This document focuses specifically on .NET Aspire configurations to provision and deploy SQL Server resources in Azure. For in-depth guidance on deploying .NET Aspire apps, visit the [Azure Container Apps deployment]() tutorial.

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

---

## Add the .NET Aspire component to the app

Add the .NET Aspire Sql Server library package to your _AspireSQL.AppHost_ project:

```dotnetcli
dotnet add package Aspire.Microsoft.Data.SqlClient --prerelease
```

## Configure the AppHost for SQL Server deployment

.NET Aspire provides two built-in configuration options to streamline SQL Server provisioning and deployment in Azure:

- Provision SQL Server as a containerized database using Azure Container Apps
- Provision SQL Server using an Azure SQL Database

These configurations are supported by tools such as the Azure Developer CLI (`azd`) to streamline .NET Aspire deployments. The Azure Developer CLI consumes these settings and provisions properly configured resources for you. The configuration examples below assume the use of `azd`.

> [!NOTE]
> You can also use the [Azure CLI]()or [Bicep]() to provision and deploy .NET Aspire apps. These options are more manual but provide more granular control over your deployments. .NET Aspire apps can also connect to databases hosted on other services through manual configurations.

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

---
