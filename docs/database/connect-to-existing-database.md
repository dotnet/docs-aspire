---
title: Connect a .NET Aspire microservice to an existing database
description: Learn how to configure a .NET Aspire solution with a connection to an existing database that isn't hosted in a .NET Aspire container.
ms.date: 03/13/2025
ms.topic: tutorial
uid: database/connect-to-existing-database
zone_pivot_groups: entity-framework-client-integration
---

# Tutorial: Connect a .NET Aspire microservice to an existing database

.NET Aspire is designed to make it easy and quick to develop cloud-native solutions. It uses containers to host the services, such as databases, that underpin each microservice. However, if you want your microservice to query a database that already exists, you must connect your microservice to it instead of creating a database container whenever you run the solution.

In this tutorial, you create a .NET Aspire solution with an API that connects to an existing database. You'll learn how to:

> [!div class="checklist"]
>
> - Create an API microservice that interacts with a database.
> - Configure the .NET Aspire App Host project with a connection string for the existing database.
> - Pass the connection string to the API and use it to connect to the database.

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

:::zone pivot="sql-server-ef"

> [!IMPORTANT]
> This tutorial also assumes you have a Microsoft SQL Server instance running on your local machine. You can connect to a database elsewhere by providing an appropriate connection string instead of the one suggested in this article. To create a new local instance, download and install [SQL Server Developer Edition](https://www.microsoft.com/sql-server/sql-server-downloads).

:::zone-end
:::zone pivot="postgresql-ef"

> [!IMPORTANT]
> This tutorial also assumes you have a PostgreSQL instance running on your local machine. You can connect to a database elsewhere by providing an appropriate connection string instead of the one suggested in this article. To create a new local instance, download and install [PostgreSQL](https://www.postgresql.org/download/).

:::zone-end
:::zone pivot="oracle-ef"

> [!IMPORTANT]
> This tutorial also assumes you have an Oracle Database instance running on your local machine. You can connect to a database elsewhere by providing an appropriate connection string instead of the one suggested in this article. To create a new local instance, download and install [Oracle Database](https://www.oracle.com/database/technologies/oracle-database-software-downloads.html).

:::zone-end
:::zone pivot="mysql-ef"

> [!IMPORTANT]
> This tutorial also assumes you have a MySQL instance running on your local machine. You can connect to a database elsewhere by providing an appropriate connection string instead of the one suggested in this article. To create a new local instance, download and install [MySQL](https://www.mysql.com/downloads/).

:::zone-end

> [!TIP]
> In this tutorial, you use .NET Aspire EF Core integrations to access the database. Other database integrations, which don't use EF Core, can use the same approach to connect to an existing database.

## Create a new .NET Aspire solution

Let's start by creating a new solution with the .NET Aspire Starter template.

1. In Visual Studio, select **File** > **New** > **Project**.
1. In the **Create a new project** dialog window, search for *.NET Aspire*, select **.NET Aspire Starter App**, and then select **Next**.
1. In the **Configure your new project** page:

    - Enter a **Solution name** of **AspireExistingDB**.
    - Leave the other values at their defaults and then select **Next**.

1. In the **Additional information** page:

    - Make sure that **.NET 9.0** is selected.
    - Leave the other values at their defaults and then select **Create**.

Visual Studio creates a new .NET Aspire solution with an API and a web front end. The solution consists of the following projects:

- **AspireExistingDB.ApiService**: A web API project that returns weather forecasts.
- **AspireExistingDB.AppHost**: An orchestrator project designed to connect and configure the different projects and services of your app. The orchestrator should be set as the startup project.
- **AspireExistingDB.ServiceDefaults**: A shared class library to hold configurations that can be reused across the projects in your solution.
- **AspireExistingDB.Web**: A Blazor app that implements a web user interface for the solution.

## Create the database model and context classes

First, install EF Core in the _AspireExistingDB.ApiService_ project.

1. In **Solution Explorer**, right-click the _AspireExistingDB.ApiService_ project, and then select **Manage NuGet Packages**.

:::zone pivot="sql-server-ef"

1. Select the **Browse** tab, and then search for **Aspire.Microsoft.EntityFrameworkCore**.
1. Select the **Aspire.Microsoft.EntityFrameworkCore.SqlServer** package, and then select **Install**.

::zone-end
:::zone pivot="postgresql-ef"

1. Select the **Browse** tab, and then search for **Aspire.Npgsql.EntityFrameworkCore**.
1. Select the **Aspire.Npgsql.EntityFrameworkCore.PostgreSQL** package, and then select **Install**.

::zone-end
:::zone pivot="oracle-ef"

1. Select the **Browse** tab, and then search for **Aspire.Oracle.EntityFrameworkCore**.
1. Select the **Aspire.Oracle.EntityFrameworkCore** package, and then select **Install**.

:::zone-end
:::zone pivot="mysql-ef"

1. Select the **Browse** tab, and then search for **Aspire.Pomelo.EntityFrameworkCore**.
1. Select the **Aspire.Pomelo.EntityFrameworkCore.MySql** package, and then select **Install**.

:::zone-end

1. If the **Preview Changes** dialog appears, select **Apply**.
1. In the **License Acceptance** dialog, select **I Accept**.

To represent a weather report, add the following `WeatherReport` model class at the root of the _AspireExistingDB.ApiService_ project:

```csharp
using System.ComponentModel.DataAnnotations;

namespace AspireExistingDB.ApiService;

public sealed class WeatherReport
{
    public int Id { get; set; }
    [Required]
    public DateTime Date { get; set; } = DateTime.Now;
    [Required]
    public int TemperatureC { get; set; } = 10;
    public string? Summary { get; set; }
}
```

Add the following `WeatherDbContext` data context class at the root of the _AspireExistingDB.ApiService_ project. The class inherits <xref:System.Data.Entity.DbContext?displayProperty=fullName> to work with EF Core and represent your database.

```csharp
using Microsoft.EntityFrameworkCore;

namespace AspireExistingDB.ApiService;

public class WeatherDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<WeatherReport> Forecasts => Set<WeatherReport>();
}
```

## Add the Scalar user interface to the API project

You'll use the Scalar UI to test the _AspireExistingDB.ApiService_ project. Let's install and configure it:

1. In Visual Studio, in the **Solution Explorer**, right-click the _AspireExistingDB.ApiService_ project, and then select **Manage NuGet Packages**.
1. Select the **Browse** tab, and then search for **Scalar**.
1. Select the **Scalar.AspNetCore** package, and then select **Install**.
1. If the **Preview Changes** dialog appears, select **Apply**.
1. In the **License Acceptance** dialog, select **I Accept**.
1. In the _AspireExistingDB.ApiService_ project, open the _Program.cs_ file.
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
        app.MapScalarApiReference(_ => _.Servers = [ ]);
    }
    ```

## Configure a connection string in the App Host project

:::zone pivot="sql-server-ef"

Usually, when you create a cloud-native solution with .NET Aspire, you call the <xref:Aspire.Hosting.SqlServerBuilderExtensions.AddSqlServer*> method to initiate a container that runs the SQL Server instance. You pass that resource to other projects in your solution that need access to the database.

In this case, however, you want to work with an existing database outside of any container. There are three differences in the App Host project:

- You don't need to install the `Aspire.Hosting.SqlServer` hosting integration.
- You add a connection string in a configuration file, such as **appsetting.json**.
- You call <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> to create a resource that you pass to other projects. Those projects use this resource to connect to the existing database.

Let's implement that configuration:

1. In Visual Studio, in the _AspireExistingDB.AppHost_ project, open the _appsetting.json_ file.
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

1. In the _AspireExistingDB.AppHost_ project, open the _Program.cs_ file.
1. Locate the following line of code:

    ```csharp
    var builder = DistributedApplication.CreateBuilder(args);
    ```

1. Immediately after that line, add this line of code, which obtains the connection string from the configuration file:

    ```csharp
    var connectionString = builder.AddConnectionString("sql");
    ```

:::zone-end
:::zone pivot="postgresql-ef"

Usually, when you create a cloud-native solution with .NET Aspire, you call the <xref:Aspire.Hosting.PostgresBuilderExtensions.AddPostgres*> method to initiate a container that runs the PostgreSQL instance. You pass that resource to other projects in your solution that need access to the database.

In this case, however, you want to work with an existing database outside of any container. There are three differences in the App Host project:

- You don't need to install the `Aspire.Hosting.PostgreSQL` hosting integration.
- You add a connection string in a configuration file, such as **appsetting.json**.
- You call <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> to create a resource that you pass to other projects. Those projects use this resource to connect to the existing database.

Let's implement that configuration:

1. In Visual Studio, in the _AspireExistingDB.AppHost_ project, open the _appsetting.json_ file.
1. Replace the entire contents of the file with the following code. In the connection string, replace {Username} and {Password} with the correct credentials for your PostgreSQL server:

    ```json
    {
        "ConnectionStrings": {
            "postgresql": "Server=127.0.0.1;Port=5432;Database=WeatherForecasts;User Id={Username};Password={Password};"
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

1. In the _AspireExistingDB.AppHost_ project, open the _Program.cs_ file.
1. Locate the following line of code:

    ```csharp
    var builder = DistributedApplication.CreateBuilder(args);
    ```

1. Immediately after that line, add this line of code, which obtains the connection string from the configuration file:

    ```csharp
    var connectionString = builder.AddConnectionString("postgresql");
    ```

:::zone-end
:::zone pivot="oracle-ef"

Usually, when you create a cloud-native solution with .NET Aspire, you call the <xref:Aspire.Hosting.OracleDatabaseBuilderExtensions.AddOracle*> method to initiate a container that runs the Oracle Database instance. You pass that resource to other projects in your solution that need access to the database.

In this case, however, you want to work with an existing database outside of any container. There are three differences in the App Host project:

- You don't need to install the `Aspire.Hosting.Oracle` hosting integration.
- You add a connection string in a configuration file, such as **appsetting.json**.
- You call <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> to create a resource that you pass to other projects. Those projects use this resource to connect to the existing database.

Let's implement that configuration:

1. In Visual Studio, in the _AspireExistingDB.AppHost_ project, open the _appsetting.json_ file.
1. Replace the entire contents of the file with the following code:

    ```json
    {
        "ConnectionStrings": {
            "oracle": "Data Source=WeatherForecasts;Integrated Security=yes;"
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

1. In the _AspireExistingDB.AppHost_ project, open the _Program.cs_ file.
1. Locate the following line of code:

    ```csharp
    var builder = DistributedApplication.CreateBuilder(args);
    ```

1. Immediately after that line, add this line of code, which obtains the connection string from the configuration file:

    ```csharp
    var connectionString = builder.AddConnectionString("oracle");
    ```

:::zone-end
:::zone pivot="mysql-ef"

Usually, when you create a cloud-native solution with .NET Aspire, you call the <xref:Aspire.Hosting.MySqlBuilderExtensions.AddMySql*> method to initiate a container that runs the MySQL instance. You pass that resource to other projects in your solution that need access to the database.

In this case, however, you want to work with an existing database outside of any container. There are three differences in the App Host project:

- You don't need to install the `Aspire.Hosting.MySQL` hosting integration.
- You add a connection string in a configuration file, such as **appsetting.json**.
- You call <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> to create a resource that you pass to other projects. Those projects use this resource to connect to the existing database.

Let's implement that configuration:

1. In Visual Studio, in the _AspireExistingDB.AppHost_ project, open the _appsetting.json_ file.
1. Replace the entire contents of the file with the following code. In the connection string, replace {Username} and {Password} with the correct credentials for your MySQL server:

    ```json
    {
        "ConnectionStrings": {
            "mysql": "Server=127.0.0.1;Database=WeatherForecasts;Uid={Username};Pwd={Password};"
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

1. In the _AspireExistingDB.AppHost_ project, open the _Program.cs_ file.
1. Locate the following line of code:

    ```csharp
    var builder = DistributedApplication.CreateBuilder(args);
    ```

1. Immediately after that line, add this line of code, which obtains the connection string from the configuration file:

    ```csharp
    var connectionString = builder.AddConnectionString("mysql");
    ```

:::zone-end

1. Locate the following line of code, which creates a resource for the _AspireExistingDB.ApiService_ project:

    ```csharp
    var apiService = builder.AddProject<Projects.AspireExistingDB_ApiService>("apiservice");
    ```

1. Modify that line to match the following, which creates the resource and passes the connection string to it:

    ```csharp
    var apiService = builder.AddProject<Projects.AspireExistingDB_ApiService>("apiservice")
                            .WithReference(connectionString);
    ```

1. To save your changes, select **File** > **Save All**.

## Use the database in the API project

Returning to the _AspireExistingDB.ApiService_ project, you must obtain the connection string resource from the App Host, and then use it to create the database:

1. In Visual Studio, in the _AspireExistingDB.ApiService_ project, open the _Program.cs_ file.
1. Locate the following line of code:

    ```csharp
    var builder = WebApplication.CreateBuilder(args);
    ```

:::zone pivot="sql-server-ef"

1. Immediately after that line, add this line of code:

    ```csharp
    builder.AddSqlServerDbContext<WeatherDbContext>("sql");
    ```

:::zone-end
:::zone pivot="postgresql-ef"

1. Immediately after that line, add this line of code:

    ```csharp
    builder.AddSqlServerDbContext<WeatherDbContext>("postgresql");
    ```

:::zone-end
:::zone pivot="oracle-ef"

1. Immediately after that line, add this line of code:

    ```csharp
    builder.AddSqlServerDbContext<WeatherDbContext>("oracle");
    ```

:::zone-end
:::zone pivot="mysql-ef"

1. Immediately after that line, add this line of code:

    ```csharp
    builder.AddSqlServerDbContext<WeatherDbContext>("mysql");
    ```

:::zone-end

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

1. In Visual Studio, in the _AspireExistingDB.ApiService_ project, open the _Program.cs_ file.
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

1. In Visual Studio, in the _Program.cs_ file for the _AspireExistingDB.ApiService_ project, locate the following code:

    ```csharp
    app.MapDefaultEndpoints();
    ```

1. Immediately _before_ that line, add the following code, which creates and saves a new forecast:

    ```csharp
    app.MapPost("/weatherforecast", async ([FromBody] WeatherReport forecast, [FromServices] WeatherDbContext context, HttpResponse response) =>
    {
        context.Forecasts.Add(forecast);
        await context.SaveChangesAsync();
        response.StatusCode = 200;
        response.Headers.Location = $"weatherforecast/{forecast.Id}";
    })
    .Accepts<WeatherReport>("application/json")
    .Produces<WeatherReport>(StatusCodes.Status201Created)
    .WithName("PostWeatherForecast").WithTags("Setters");
    ```

## Run and test the app locally

:::zone pivot="sql-server-ef"

The sample app is ready to test. Before you start debugging, make sure that:

- Docker Desktop or Podman is running to host containers for the solution.
- SQL Server is running to host the database.

Let's connect to the SQL Server instance and check the databases that exist.

1. Start **Microsoft SQL Server Management Studio**.
1. Connect to the SQL Server instance on the local machine. Ensure that **Trust server certificate** is selected.

    :::image type="content" source="media/connect-to-existing-database/connect-ssms.png" lightbox="media/connect-to-existing-database/connect-ssms.png" alt-text="A screenshot showing how to connect to a local instance of SQL Server in SQL Server Management Studio.":::

1. In the **Object Explorer** expand **Databases**. There is no database named **WeatherForecasts**.

Now, let's test the solution:

1. In Visual Studio, select the run button (or press <kbd>F5</kbd>) to launch your .NET Aspire project dashboard in the browser.
1. In the navigation on the left, select **Console**.
1. In the **Resource** drop down list, select **apiservice**. Notice the `CREATE TABLE` SQL command, which has created the **Forecasts** table in the **WeatherForecasts** database.

    :::image type="content" source="media/connect-to-existing-database/console-log-create-table.png" lightbox="media/connect-to-existing-database/console-log-create-table.png" alt-text="A screenshot showing the CREATE TABLE query in the API console log.":::

1. Switch to SQL Server Management Studio. In the **Object Explorer**, right-click **Databases** and then select **Refresh**.
1. Expand the new **WeatherForecasts** database and then expand **Tables**. Notice the new **dbo.Forecasts** table.
1. Right-click the **dbo.Forecasts** table and then select **Select Top 1000 Rows**. The query runs but returns no results because the table is empty.
1. In the .NET Aspire dashboard, in the navigation on the left, select **Resources**.
1. Select one of the endpoints for the **apiservice** resource.

    :::image type="content" source="media/connect-to-existing-database/dashboard-select-api-endpoint.png" lightbox="media/connect-to-existing-database/dashboard-select-api-endpoint.png" alt-text="A screenshot showing how to connect to the API from the .NET Aspire dashboard.":::

1. In the browser window, append **/scalar** to the web address and then press <kbd>Enter</kbd>.
1. In the navigation on the left, expand **Setters** and then select **/weatherforecast POST**.
1. Select **Test Request**. Under **Body**, in the **JSON** window, delete the **id** and **date** lines and fill in your own values for **temperatureC** and **summary**.
1. Select **Send**.
1. In the navigation on the left, select **/weatherforecast GET** and then select **Test Request**.

    :::image type="content" source="media/connect-to-existing-database/scalar-text-get.png" lightbox="media/connect-to-existing-database/scalar-text-get.png" alt-text="A screenshot showing how to test the GET method in the API from the Scalar user interface.":::

1. Select **Send**. The call should return JSON with the weather reported you posted.
1. Switch to SQL Server Management Studio. In the query window for the **Forecasts** table, select **Execute** or press <kbd>F5</kbd>. Your weather forecast is displayed.

:::zone-end
:::zone pivot="postgresql-ef"

The sample app is ready to test. Before you start debugging, make sure that:

- Docker Desktop or Podman is running to host containers for the solution.
- PostgreSQL is running to host the database.

Let's connect to PostgreSQL and check the databases that exist.

1. Start the PostgreSQL **psql** shell.
1. Connect to the PostgreSQL instance on the **localhost** using the credentials and port number you specified when you installed PostgreSQL.
1. To list all the databases, enter the command **\l**. There is no database named **WeatherForecasts**.

Now, let's test the solution:

1. In Visual Studio, select the run button (or press <kbd>F5</kbd>) to launch your .NET Aspire project dashboard in the browser.
1. In the navigation on the left, select **Console**.
1. In the **Resource** drop down list, select **apiservice**. Notice the `CREATE TABLE` SQL command, which has created the **Forecasts** table in the **WeatherForecasts** database.

    :::image type="content" source="media/connect-to-existing-database/console-log-create-table.png" lightbox="media/connect-to-existing-database/console-log-create-table.png" alt-text="A screenshot showing the CREATE TABLE query in the API console log.":::

1. Switch to the **psql** shell and enter the **\l** command again. Notice the new **Forecasts** table.
1. To query all rows in the **Forecasts** table, run this command:

    ```sql
    SELECT * FROM Forecasts;
    ```

    The query runs but returns no results because the table is empty.

1. In the .NET Aspire dashboard, in the navigation on the left, select **Resources**.
1. Select one of the endpoints for the **apiservice** resource.

    :::image type="content" source="media/connect-to-existing-database/dashboard-select-api-endpoint.png" lightbox="media/connect-to-existing-database/dashboard-select-api-endpoint.png" alt-text="A screenshot showing how to connect to the API from the .NET Aspire dashboard.":::

1. In the browser window, append **/scalar** to the web address and then press <kbd>Enter</kbd>.
1. In the navigation on the left, expand **Setters** and then select **/weatherforecast POST**.
1. Select **Test Request**. Under **Body**, in the **JSON** window, delete the **id** and **date** lines and fill in your own values for **temperatureC** and **summary**.
1. Select **Send**.
1. In the navigation on the left, select **/weatherforecast GET** and then select **Test Request**.

    :::image type="content" source="media/connect-to-existing-database/scalar-text-get.png" lightbox="media/connect-to-existing-database/scalar-text-get.png" alt-text="A screenshot showing how to test the GET method in the API from the Scalar user interface.":::

1. Select **Send**. The call should return JSON with the weather reported you posted.
1. Switch to the **psql** shell. To query all rows in the **Forecasts** table, run this command:

    ```sql
    SELECT * FROM Forecasts;
    ```

    Your weather forecast is displayed.

:::zone-end
:::zone pivot="oracle-ef"

The sample app is ready to test. Before you start debugging, make sure that:

- Docker Desktop or Podman is running to host containers for the solution.
- Oracle Database is running to host the database.

You will use Oracle **SQL Developer** tool to run queries. You can download the tool from [Oracle](https://www.oracle.com/database/sqldeveloper/technologies/download/).

Let's connect to Oracle Database and check the databases that exist.

1. Start the **Oracle SQL Developer** tool.
1. Right-click the **Connections** node and then select **New Connection**.
1. Connect to the Oracle Database instance on the **localhost** using the credentials and port number you specified when you installed Oracle.
1. Expand the **Tables** node. There is no **Forecasts** table.

Now, let's test the solution:

1. In Visual Studio, select the run button (or press <kbd>F5</kbd>) to launch your .NET Aspire project dashboard in the browser.
1. In the navigation on the left, select **Console**.
1. In the **Resource** drop down list, select **apiservice**. Notice the `CREATE TABLE` SQL command, which has created the **Forecasts** table in the **WeatherForecasts** database.

    :::image type="content" source="media/connect-to-existing-database/console-log-create-table.png" lightbox="media/connect-to-existing-database/console-log-create-table.png" alt-text="A screenshot showing the CREATE TABLE query in the API console log.":::

1. Switch to the **Oracle SQL Developer** tool. Select the **Tables** node and then select the **Refresh** button. Notice the new **Forecasts** table.
1. To query all rows in the **Forecasts** table, run this command in the **SQL Worksheet** window:

    ```sql
    SELECT * FROM Forecasts;
    ```

    The query runs but returns no results because the table is empty.

1. In the .NET Aspire dashboard, in the navigation on the left, select **Resources**.
1. Select one of the endpoints for the **apiservice** resource.

    :::image type="content" source="media/connect-to-existing-database/dashboard-select-api-endpoint.png" lightbox="media/connect-to-existing-database/dashboard-select-api-endpoint.png" alt-text="A screenshot showing how to connect to the API from the .NET Aspire dashboard.":::

1. In the browser window, append **/scalar** to the web address and then press <kbd>Enter</kbd>.
1. In the navigation on the left, expand **Setters** and then select **/weatherforecast POST**.
1. Select **Test Request**. Under **Body**, in the **JSON** window, delete the **id** and **date** lines and fill in your own values for **temperatureC** and **summary**.
1. Select **Send**.
1. In the navigation on the left, select **/weatherforecast GET** and then select **Test Request**.

    :::image type="content" source="media/connect-to-existing-database/scalar-text-get.png" lightbox="media/connect-to-existing-database/scalar-text-get.png" alt-text="A screenshot showing how to test the GET method in the API from the Scalar user interface.":::

1. Select **Send**. The call should return JSON with the weather reported you posted.
1. Switch to the **Oracle SQL Developer** tool. To query all rows in the **Forecasts** table, run this command:

    ```sql
    SELECT * FROM Forecasts;
    ```

    Your weather forecast is displayed.

:::zone-end
:::zone pivot="mysql-ef"

The sample app is ready to test. Before you start debugging, make sure that:

- Docker Desktop or Podman is running to host containers for the solution.
- MySQL is running to host the database.

Let's connect to MySQL and check the databases that exist.

1. At a command prompt start the **mysql** tool. Replace {Username} with the correct username for your installation and then enter the correct password:

    ```cmd
    mysql -u {Username} -p
    ```

1. To list all the databases, enter this command. There is no database named **WeatherForecasts**:

    ```sql
    SHOW DATABASES;
    ```

Now, let's test the solution:

1. In Visual Studio, select the run button (or press <kbd>F5</kbd>) to launch your .NET Aspire project dashboard in the browser.
1. In the navigation on the left, select **Console**.
1. In the **Resource** drop down list, select **apiservice**. Notice the `CREATE TABLE` SQL command, which has created the **Forecasts** table in the **WeatherForecasts** database.

    :::image type="content" source="media/connect-to-existing-database/console-log-create-table.png" lightbox="media/connect-to-existing-database/console-log-create-table.png" alt-text="A screenshot showing the CREATE TABLE query in the API console log.":::

1. Switch to the **mysql** shell and enter this command again. Notice the new **WeatherForecasts** database:

    ```sql
    SHOW DATABASES;
    ```

1. To query all rows in the **Forecasts** table, run these commands:

    ```sql
    USE WeatherForecasts;
    SELECT * FROM Forecasts;
    ```

    The query runs but returns no results because the table is empty.

1. In the .NET Aspire dashboard, in the navigation on the left, select **Resources**.
1. Select one of the endpoints for the **apiservice** resource.

    :::image type="content" source="media/connect-to-existing-database/dashboard-select-api-endpoint.png" lightbox="media/connect-to-existing-database/dashboard-select-api-endpoint.png" alt-text="A screenshot showing how to connect to the API from the .NET Aspire dashboard.":::

1. In the browser window, append **/scalar** to the web address and then press <kbd>Enter</kbd>.
1. In the navigation on the left, expand **Setters** and then select **/weatherforecast POST**.
1. Select **Test Request**. Under **Body**, in the **JSON** window, delete the **id** and **date** lines and fill in your own values for **temperatureC** and **summary**.
1. Select **Send**.
1. In the navigation on the left, select **/weatherforecast GET** and then select **Test Request**.

    :::image type="content" source="media/connect-to-existing-database/scalar-text-get.png" lightbox="media/connect-to-existing-database/scalar-text-get.png" alt-text="A screenshot showing how to test the GET method in the API from the Scalar user interface.":::

1. Select **Send**. The call should return JSON with the weather reported you posted.
1. Switch to the **mysql** shell. To query all rows in the **Forecasts** table, run this command:

    ```sql
    SELECT * FROM Forecasts;
    ```

    Your weather forecast is displayed.

:::zone-end

## See also

- [.NET Aspire SQL Server Entity Framework Core integration](/dotnet/aspire/database/sql-server-entity-framework-integration)
- [Tutorial: Connect an ASP.NET Core app to SQL Server using .NET Aspire and Entity Framework Core](/dotnet/aspire/database/sql-server-integrations)
- [Use openAPI documents](/aspnet/core/fundamentals/openapi/using-openapi-documents)
