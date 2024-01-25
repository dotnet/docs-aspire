---
title: Implement caching with .NET Aspire components
description: Learn how to connect to Redis and cache data using .NET Aspire components.
ms.date: 12/09/2023
ms.topic: tutorial
---

# Tutorial: Implement caching with .NET Aspire components

Cloud-native apps often require various types of scalable caching solutions to improve performance. .NET Aspire components simplify the process of connecting to popular caching services such as Redis. In this article, you'll learn how to:

> [!div class="checklist"]
>
> - Create a basic ASP.NET core app that is set up to use .NET Aspire.
> - Add .NET Aspire components to connect to Redis and implement caching.
> - Configure the .NET Aspire components to meet specific requirements.

This article explores how to use two different types of ASP.NET Core caching using .NET Aspire and Redis:

- **[Output caching](/aspnet/core/performance/caching/output)**: A configurable, extensible caching method for storing entire HTTP responses for future requests.
- **[Distributed caching](/aspnet/core/performance/caching/distributed)**: A cache shared by multiple app servers that allows you to cache specific pieces of data. A distributed cache is typically maintained as an external service to the app servers that access it and can improve the performance and scalability of an ASP.NET Core app.

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

## Create the project

1. At the top of Visual Studio, navigate to **File** > **New** > **Project...**.
1. In the dialog window, enter **.NET Aspire** into the project template search box and select **.NET Aspire Starter Application**. Choose **Next**.
1. On the **Configure your new project** screen:
    - Enter a **Project name** of **AspireRedis**.
    - Leave the rest of the values at their defaults and select **Next**.
1. On the **Additional information** screen:
    - Make sure **.NET 8.0** is selected.
    - Uncheck **Use Redis for caching**. You will implement your own caching setup.
    - Select **Create**.

Visual Studio creates a new .NET Aspire solution that consists of the following projects:

- **AspireRedis.Web** - A Blazor UI project with default .NET Aspire configurations.
- **AspireRedis.ApiService** - A Minimal API with default .NET Aspire configurations that provides the frontend with data.
- **AspireRedis.AppHost** - An orchestrator project designed to connect and configure the different projects and services of your app.
- **AspireRedis.ServiceDefaults** - A .NET Aspire shared project to manage configurations that are reused across the projects in your solution related to [resilience](/dotnet/core/resilience/http-resilience), [service discovery](../service-discovery/overview.md), and [telemetry](../telemetry.md).

## Configure the App Host project

Update the _Program.cs_ file of the `AspireRedis.AppHost` project to match the following code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("cache");

var apiservice = builder.AddProject<Projects.AspireRedis_ApiService>("apiservice")
    .WithReference(redis);

builder.AddProject<Projects.AspireRedis_Web>("webfrontend")
    .WithReference(apiservice)
    .WithReference(redis);

builder.Build().Run();
```

The preceding code creates a local Redis container instance and configures the UI and API to use the instance automatically for both output and distributed caching. The code also configures communication between the frontend UI and the backend API using service discovery. With .NET Aspire's implicit service discovery, setting up and managing service connections is streamlined for developer productivity. In the context of this tutorial, the feature simplifies how you connect to Redis.

Traditionally, you'd manually specify the Redis connection string in each project's _appsettings.json_ file:

```json
{
  "ConnectionStrings": {
    "cache": "localhost:6379"
  }
}
```

Configuring connection string with this method, while functional, requires duplicating the connection string across multiple projects, which can be cumbersome and error-prone.

## Configure the UI with output caching

1. Add the [.NET Aspire StackExchange Redis output caching](stackexchange-redis-output-caching-component.md) component packages to your `AspireRedis.Web` app:

```dotnetcli
dotnet add package Aspire.StackExchange.Redis.OutputCaching --prerelease
```

1. In the _Program.cs_ file of the `AspireRedis.Web` Blazor project, immediately after the line `var builder = WebApplication.CreateBuilder(args);`, add a call to the <xref:Microsoft.Extensions.Hosting.AspireRedisOutputCacheExtensions.AddRedisOutputCache%2A> extension method:

    ```csharp
    builder.AddRedisOutputCache("cache");
    ```

    This method accomplishes the following tasks:

    - Configures ASP.NET Core output caching to use a Redis instance with the specified connection name.
    - Automatically enables corresponding health checks, logging, and telemetry.

1. Replace the contents of the _Home.razor_ file of the `AspireRedis.Web` Blazor project with the following:

    ```razor
    @page "/"
    @attribute [OutputCache(Duration = 10)]

    <PageTitle>Home</PageTitle>

    <h1>Hello, world!</h1>

    Welcome to your new app on @DateTime.Now
    ```

    The component include the `[OutputCache]` attribute, which caches the entire rendered response. The page also include a call to `@DateTime.Now` to help verify that the response is cached.

## Configure the API with distributed caching

1. Add the [.NET Aspire StackExchange Redis distributed caching](stackexchange-redis-output-caching-component.md) component packages to your `AspireRedis.ApiService` app:

    ```dotnetcli
    dotnet add package Aspire.StackExchange.Redis.DistributedCaching --prerelease
    ```

1. Towards the top of the _Program.cs_ file, add a call to <xref:Microsoft.Extensions.Hosting.AspireRedisDistributedCacheExtensions.AddRedisDistributedCache%2A>:

    ```csharp
    builder.AddRedisDistributedCache("cache");
    ```

1. In the _Program.cs_ file, replace the existing `/weatherforecast` endpoint code with the following:

    ```csharp
    app.MapGet("/weatherforecast", async (IDistributedCache cache) =>
    {
        var cachedForecast = await cache.GetAsync("forecast");

        if (cachedForecast is null)
        {
            var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
            var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();

            await cache.SetAsync("forecast", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(forecast)), new ()
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(10)
            }); ;

            return forecast;
        }

        return JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(cachedForecast);
    })
    .WithName("GetWeatherForecast");
    ```

## Test the app locally

Test the caching behavior of your app using the following steps:

1. Run the app using Visual Studio by pressing <kbd>F5</kbd>.
1. If the **Start Docker Desktop** dialog appears, select **Yes** to start the service.
1. The .NET Aspire Dashboard loads in the browser and lists the UI and API projects.

Test the output cache:

1. On the projects page, in the **webfrontend** row, click the `localhost` link in the **Endpoints** column to open the UI of your app.
1. The application will display the current time on the home page.
1. Refresh the browser every few seconds to see the same page returned by output caching. After 10 seconds the cache expires and the page reloads with an updated time.

Test the distributed cache:

1. Navigate to the **Weather** page on the Blazor UI to load a table of randomized weather data.
1. Refresh the browser every few seconds to see the same weather data returned by output caching. After 10 seconds the cache expires and the page reloads with updated weather data.

Congratulations! You configured a ASP.NET Core app to use output and distributed caching with .NET Aspire.
