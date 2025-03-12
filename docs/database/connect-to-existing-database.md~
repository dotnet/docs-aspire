---
title: Connect a .NET Aspire microservice to an external database
description: Learn how to configure a .NET Aspire solution with a connection to an external database that isn't hosted in a .NET Aspire container.
ms.date: 03/13/2025
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

- **AspireExternalDB.ApiService**: A web API project that returns weather forecasts.
- **AspireExternalDB.AppHost**: An orchestrator project designed to connect and configure the different projects and services of your app. The orchestrator should be set as the startup project.
- **AspireExternalDB.ServiceDefaults**: A shared class library to hold configurations that can be reused across the projects in your solution.
- **AspireExternalDB.Web**: A Blazor app that implements a web user interface for the solution.

## Create the database model and context classes

First, install EF Core in the *AspireExternalDB.ApiService* project.

1. In **Solution Explorer**, right-click the **AspireExternalDB.ApiService** project, and then select **Manage NuGet Packages**.
1. Select the **Browse** tab, and then search for **Aspire.Microsoft.EntityFrameworkCore**.
1. Select the **Aspire.Microsoft.EntityFrameworkCore.SqlServer** package, and then select **Install**.
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

> [!NOTE]
> If you see an error on the Forecasts

## Add the Swagger user interface to the API project

We'll use the Swagger UI to test the **AspireExternalDB.ApiService** project. Let's install and configure it:

1. In Visual Studio, in the **Solution Explorer**, right-click the **AspireExternalDB.ApiService** project, and then select **Manage NuGet Packages**.
1. Select the **Browse** tab, and then search for **Swashbuckle**.
1. Select the **Swashbuckle.AspNetCore** package, and then select **Install**.
1. If the **Preview Changes** dialog appears, select **Apply**.
1. In the **License Acceptance** dialog, select **I Accept**.
1. In the **AspireExternalDB.ApiService** project, open the **Program.cs** file.
1. Locate the following line of code:

    ```csharp
    builder.Services.AddOpenApi();
    ```

1. Immediately after that line, add this line of code, which adds the Swagger generator in the dependency injection (DI) container:

    ```csharp
    builder.Services.AddSwaggerGen();
    ```

1. Locate the following lines of code:

    ```csharp
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }
    ```

1. Modify that code to match the following lines:

    ```csharp
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    ```

## Configure a connection string in the App Host project

Usually, when you create a cloud-native solution with .NET Aspire, you call the <xref:Aspire.Hosting.SqlServerBuilderExtensions.AddSqlServer*> method to initiate a container that runs the SQL Server instance. You pass that resource to other projects in your solution that need access to the database.

In this case, however, you want to work with a pre-existing database outside of any container. There are three differences in the App Host project:

- You don't need to install the `Aspire.Hosting.SqlServer` hosting integration.
- You add a connection string in a configuration file, such as **appsetting.json**.
- You call <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> to create a resource that you pass to other projects. Those projects use this resource to connect to the existing database.

Let's implement that configuration:

1. In Visual Studio, in the **AspireExternalDB.AppHost** project, open the **appsetting.json** file.
1. Replace the entire contents of the file with the following code:

    ```json
    {
        "ConnectionStrings": {
            "sql": "Server=localhost;Trusted_Connection=True;TrustServerCertificate=True;Initial Catalog=WeatherForecasts;"
        },
        "Logging": {
            "LogLevel": {
                "Default": "Information",
                "Microsoft.AspNetCore": "Warning",
                "Aspire.Hosting.Dcp": "Warning"
            }
        }
    }
    ```

1. In the **AspireExternalDB.AppHost** project, open the **Program.cs** file.
1. Locate the following line of code:

    ```csharp
    var builder = DistributedApplication.CreateBuilder(args);
    ```

1. Immediately after that line, add this line of code, which obtains the connection string from the configuration file:

    ```csharp
    var connectionString = builder.AddConnectionString("sql");
    ```

1. Locate the following line of code, which creates a resource for the **AspireExternalDB.ApiService** project:

    ```csharp
    var apiService = builder.AddProject<Projects.AspireExternalDB_ApiService>("apiservice");
    ```

1. Modify that line to match the following, which creates the resource and passes the connection string to it:

    ```csharp
    var apiService = builder.AddProject<Projects.AspireExternalDB_ApiService>("apiservice")
                            .WithReference(connectionString);
    ```

1. To save your changes, select **File** > **Save All**.

## Use the database in the API project

Returning to the **AspireExternalDB.ApiService** project, you must obtain the connection string resource from the App Host, and then use it to create the database in the instance of SQL Server:

1. In Visual Studio, in the **AspireExternalDB.ApiService** project, open the **Program.cs** file.
1. Locate the following line of code:

    ```csharp
    var builder = WebApplication.CreateBuilder(args);
    ```

1. Immediately after that line, add this line of code:

    ```csharp
    builder.AddSqlServerDbContext<WeatherDbContext>("sql");
    ```

1. Locate the following line of code:

    ```csharp
    app.MapDefaultEndpoints();
    ```

1. Immediately after that line, add this code, which connects to the database and creates it if it doesn't already exist:

    ```csharp
    if (app.Environment.IsDevelopment())
    {
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<WeatherDbContext>();
            context.Database.EnsureCreated();
        }
    }
    else
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        app.UseHsts();
    }
    ```

The preceding code:

- Checks if the app is running in a development environment.
- If it is, it retrieves the `WeatherDbContext` service from the DI container and calls `Database.EnsureCreated()` to create the database if it doesn't already exist.

> [!NOTE]
> Note that `EnsureCreated()` is not suitable for production environments, and it only creates the database as defined in the context. It doesn't apply any migrations. For more information on Entity Framework Core migrations in .NET Aspire, see [Apply Entity Framework Core migrations in .NET Aspire](ef-core-migrations.md).

## Add code to query weather forecasts from the database

In the .NET Aspire starter solution template, the API creates five random weather forecasts and returns them when another project requests them. Let's replace that with code that queries the database:

1. In Visual Studio, in the **AspireExternalDB.ApiService** project, open the **Program.cs** file.
1. At the top of the file, add the following lines of code:

    ```csharp
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    ```

1. Locate the following code:

    ```csharp
    string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

    app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");
    ```

1. Replace that code with the following lines:

    ```csharp
    app.MapGet("/weatherforecast", async ([FromServices] WeatherDbContext context) =>
    {
        var forecast = await context.Forecasts.ToArrayAsync();
        return forecast;
    })
    .WithName("GetWeatherForecast");
    ```

1. Locate and remove the following code:

    ```csharp
    record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
    ```

## Add code to insert a new forecast into the database

Finally, let's add a POST method to the API, which will add records to the database:

1. In Visual Studio, in the **Program.cs** file for the **AspireExternalDB.ApiService** project, locate the following code:

    ```csharp
    app.MapDefaultEndpoints();
    ```

1. Immediately *before* that line, add the following code, which creates and save a new forecast:

    ```csharp
    app.MapPost("/weatherforecast", async ([FromBody] WeatherForecast forecast, [FromServices] WeatherDbContext context, HttpResponse response) =>
    {
        context.Forecasts.Add(forecast);
        await context.SaveChangesAsync();
        response.StatusCode = 200;
        response.Headers.Location = $"weatherforecast/{forecast.Id}";
    })
    .Accepts<WeatherForecast>("application/json")
    .Produces<WeatherForecast>(StatusCodes.Status201Created)
    .WithName("PostWeatherForecast").WithTags("Setters");
    ```

## Run and test the app locally

The sample app is ready to test. Before you start debugging, make sure that:

- Docker Desktop or Podman is running to host containers for the solution.
- SQL Server is running to host the database.

Let's connect to the SQL Server instance and check the databases that exist.

1. Start **Microsoft SQL Server Management Studio**.
1. Connect to the SQL Server instance on the local machine. Ensure that **Trust server certificate** is selected.

    :::image type="content" source="media/connect-to-external-database/connect-ssms.png" lightbox="media/connect-to-external-database/connect-ssms.png" alt-text="A screenshot showing how to connect to a local instance of SQL Server in SQL Server Management Studio.":::

1. In the **Object Explorer** expand **Databases**. There is no database named **WeatherForecasts**.

Now, let's test the solution:

1. In Visual Studio, select the run button (or press <kbd>F5</kbd>) to launch your .NET Aspire project dashboard in the browser.
1. In the navigation on the left, select **Console**.
1. In the **Resource** drop down list, select **apiservice**. Notice the `CREATE TABLE` SQL command, which has created the **Forecasts** table in the **WeatherForecasts** database.
1. Switch to SQL Server Management Studio. In the **Object Explorer**, right-click **Databases** and then select **Refresh**.
1. Expand the new **WeatherForecasts** and then expand **Tables**. Notice the **dbo.Forecasts** table.
1. Right-click the **dbo.Forecasts** table and then select **Select top 1000 rows**. The query runs but returns no results because the table is empty.
1. In the .NET Aspire dashboard, in the navigation on the left, select **Resources**.
1. Select one of the endpoints for the **apiservice** resource.

    :::image type="content" source="media/connect-to-external-database/dashboard-select-api-endpoint.png" lightbox="media/connect-to-external-database/dashboard-select-api-endpoint.png" alt-text="A screenshot showing how to connect to the API from the .NET Aspire dashboard.":::

1. In the browser window, append **/swagger/index.html** to the web address and then press <kbd>Enter</kbd>. Two methods are displayed: **GET** and **POST**.
1. Expand the **POST** method and then select **Try it out**.
1. In the **Request body** text box, enter your own values for the **temperatureC** and **summary**.
1. Select **Execute** and then check that the response code is 200.
1. Expand the **GET** method and then select **Try it out**.
1. Select **Execute**. The response body should include your new forecast.
1. Switch to SQL Server Management Studio. In the query window for the **Forecasts** table, select **Execute** or press <kbd>F5</kbd>. Your weather forecast is displayed.

## See also

- [.NET Aspire SQL Server Entity Framework Core integration](/dotnet/aspire/database/sql-server-entity-framework-integration)
- [Tutorial: Connect an ASP.NET Core app to SQL Server using .NET Aspire and Entity Framework Core](/dotnet/aspire/database/sql-server-integrations)
- [Use openAPI documents](/aspnet/core/fundamentals/openapi/using-openapi-documents)
