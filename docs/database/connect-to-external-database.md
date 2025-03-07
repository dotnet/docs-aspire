---
title: Connect a .NET Aspire microservice to an external database
description: Learn how to configure a .NET Aspire solution with a connection to an external database that isn't hosted in a .NET Aspire container.
ms.date: 03/07/2025
ms.topic: tutorial
uid: database/connect-to-external-database
---

# Tutorial: Connect a .NET Aspire microservice to an external database

.NET Aspire is designed to make it easy and quick to develop cloud-native solutions. It uses containers to host the services, such as databases, that underpin each microservice. However, if you want your microservice to query a database that already exists, you must connect your microservice to it instead of creating a database container whenever you run the solution.

In this tutorial, you create a .NET Aspire solution with an API that connects to an external database. You'll learn how to:

> [!div class="checklist"]
>
> - Create an API microservice that uses Entity Framework Core (EF Core) to interact with a database.
> - Configure the .NET Aspire App Host project with a connection string for the external database.
> - Pass the connection string to the API and use it to connect to the database.

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

> [!IMPORTANT]
> This tutorial also assumes you have a Microsoft SQL Server instance running on your local machine. You can connect to a database elsewhere by providing an appropriate connection string instead of the one suggested in this article. To create a new local instance, download and install [SQL Server Developer Edition](https://www.microsoft.com/sql-server/sql-server-downloads).

> [!TIP]
> In this tutorial, you use EF Core with SQL Server. Other database integrations, including both those that use EF Core and others, can use the same approach to connect to an external database.

## Create a new .NET Aspire solution

Let's start by creating a new solution with the .NET Aspire Starter template.

1. In Visual Studio, select **File** > **New** > **Project**.
1. In the **Create a new project** dialog window, search for *.NET Aspire*, select **.NET Aspire Starter App**, and then select **Next**.
1. In the **Configure your new project** page:

    - Enter a **Solution name** of **AspireExternalDB**.
    - Leave the other values at their defaults and then select **Next**.

1. In the **Additional information** page:

    - Make sure that **.NET 9.0** is selected.
    - Leave the other values at their defaults and then select **Create**.

Visual Studio creates a new .NET Aspire solution with an API and a web front end. The solution consists of the following projects:

- **AspireExternalDB.ApiService**: A web API project that, by default, returns weather forecasts
- **AspireExternalDB.AppHost**: An orchestrator project designed to connect and configure the different projects and services of your app. The orchestrator should be set as the startup project.
- **AspireExternalDB.ServiceDefaults**: A shared class library to hold configurations that can be reused across the projects in your solution.
- **AspireExternalDB.Web**: A Blazor app that implements a web user interface for the solution.

## Create the database model and context classes

First, install EF Core in the *AspireExternalDB.ApiService* project.

1. In **Solution Explorer**, right-click the *AspireExternalDB.ApiService* project, and then select **Manage NuGet Packages**.
1. Select the **Browse** tab, and then search for **EntityFrameworkCore**.
1. Select the **Microsoft.EntityFrameworkCore** package, and then select **Install**.
1. If the **Preview Changes** dialog appears, select **Apply**.
1. In the **License Acceptance** dialog, select **I Accept**.

To represent a weather forecast, add the following `WeatherForecast` model class at the root of the _AspireExternalDB.ApiService_ project:

```csharp
using System.ComponentModel.DataAnnotations;

namespace AspireExternalDB.ApiService
{
    public sealed class WeatherForcast
    {
        public int Id { get; set; }
        [Required]
        public DateTime Date { get; set; } = DateTime.Now;
        [Required]
        public int TemperatureC { get; set; } = 10;
        public string? Summary { get; set; }
    }
}
```

Add the following `WeatherDbContext` data context class at the root of the **AspireExternalDB.ApiService** project. The class inherits <xref:System.Data.Entity.DbContext?displayProperty=fullName> to work with EF Core and represent your database.

```csharp
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace AspireExternalDB.ApiService
{
    public class WeatherDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<WeatherForecast> Forecasts => Set<WeatherForecast>();
    }
}
```

