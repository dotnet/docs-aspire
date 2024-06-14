---
title: Build your first .NET Aspire project
description: Learn how to build your first .NET Aspire project using the .NET Aspire Started Application template.
ms.date: 06/12/2024
ms.topic: quickstart
zone_pivot_groups: dev-environment
---

# Quickstart: Build your first .NET Aspire project

Cloud-native apps often require connections to various services such as databases, storage and caching solutions, messaging providers, or other web services. .NET Aspire is designed to streamline connections and configurations between these types of services. This quickstart shows how to create a .NET Aspire Starter Application template solution.

In this quickstart, you'll explore the following tasks:

> [!div class="checklist"]
>
> - Create a basic .NET app that is set up to use .NET Aspire.
> - Add and configure a .NET Aspire component to implement caching at project creation time.
> - Create an API and use service discovery to connect to it.
> - Orchestrate communication between a front end UI, a back end API, and a local Redis cache.

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

## Create the .NET Aspire template

To create a new .NET Aspire Starter Application, you can use either Visual Studio, Visual Studio Code, or the .NET CLI.

:::zone pivot="visual-studio"

[!INCLUDE [visual-studio-file-new](../includes/visual-studio-file-new.md)]

:::zone-end
:::zone pivot="vscode"

[!INCLUDE [vscode-file-new](../includes/vscode-file-new.md)]

:::zone-end
:::zone pivot="dotnet-cli"

[!INCLUDE [dotnet-cli-file-new](../includes/dotnet-cli-file-new.md)]

:::zone-end

## Test the app locally

The sample app is now ready for testing. You want to verify the following:

- Weather data is retrieved from the API project using service discovery and displayed on the weather page.
- Subsequent requests are handled via the output caching configured by the .NET Aspire Redis component.

:::zone pivot="visual-studio"

In Visual Studio, set the **AspireSample.AppHost** project as the startup project by right-clicking on the project in the **Solution Explorer** and selecting **Set as Startup Project**. Then, press <kbd>F5</kbd> to run the app.

:::zone-end
:::zone pivot="vscode,dotnet-cli"

If you haven't already trusted the ASP.NET Core localhost certificate, you will need to trust the certificate before running the app:

```dotnetcli
dotnet dev-certs https --trust
```

For more information, see [Troubleshoot untrusted localhost certificate in .NET Aspire](../troubleshooting/untrusted-localhost-certificate.md). For in-depth details about troubleshooting localhost certificates on Linux, see [ASP.NET Core: GitHub repository issue #32842](https://github.com/dotnet/aspnetcore/issues/32842).

:::zone-end
:::zone pivot="vscode"

In Visual Studio Code, press <kbd>F5</kbd> to run the app. You'll be prompted to select which language, and C# will be suggested, select C# and then select the **AspireSample.AppHost** project with the **Default Configuration**:

:::image type="content" loc-scope="vs-code" source="media/vscode-run.png" lightbox="media/vscode-run.png" alt-text="A screenshot of the Visual Studio Code run configuration for the AspireSample.AppHost project.":::

:::zone-end
:::zone pivot="dotnet-cli"

```dotnetcli
dotnet run --project AspireSample/AspireSample.AppHost
```

For more information, see [dotnet run](/dotnet/core/tools/dotnet-run).

:::zone-end

1. The app displays the .NET Aspire dashboard in the browser. We'll look at the dashboard in more detail later. For now, find the **webfrontend** project in the list of resources and select the project's **localhost** endpoint.

    :::image type="content" source="media/aspire-dashboard-webfrontend.png" lightbox="media/aspire-dashboard-webfrontend.png" alt-text="A screenshot of the .NET Aspire Dashboard, highlighting the webfrontend project's localhost endpoint.":::

    The home page of the **webfrontend** app displays "Hello, world!"

1. Navigate from the home page to the weather page in the using the left side navigation. The weather page displays weather data. Make a mental note of some of the values represented in the forecast table.
1. Continue occasionally refreshing the page for 10 seconds. Within 10 seconds, the cached data is returned. Eventually, a different set of weather data appears, since the data is randomly generated and the cache is updated.

:::image type="content" source="media/weather-page.png" lightbox="media/weather-page.png" alt-text="The Weather page of the webfrontend app showing the weather data retrieved from the API.":::

🤓 Congratulations! You created and ran your first .NET Aspire project! To stop the app, you can close the browser window.

:::zone pivot="visual-studio"

In Visual Studio, select the **Stop Debugging** from the **Debug** menu to stop the app.

:::zone-end
:::zone pivot="vscode"

In Visual Studio Code, press <kbd>Shift</kbd> + <kbd>F5</kbd> to stop the app, or select the **Stop** button at the top center of the window:

:::image type="content" loc-scope="vs-code" source="media/vscode-stop.png" lightbox="media/vscode-stop.png" alt-text="A screenshot of the Visual Studio Code stop button.":::

:::zone-end
:::zone pivot="dotnet-cli"

Press <kbd>Ctrl</kbd> + <kbd>C</kbd> in the terminal window to stop the app.

:::zone-end

Now let's investigate the structure and other features of your new .NET Aspire project.

## Explore the .NET Aspire dashboard

When you run a .NET Aspire project, a dashboard launches that you use to monitor various parts of your app. The dashboard resembles the following screenshot:

:::image type="content" source="media/aspire-dashboard.png" lightbox="media/aspire-dashboard.png" alt-text="A screenshot of the .NET Aspire Dashboard, depicting the Projects tab.":::

Visit each link on the left navigation to view different information about the .NET Aspire project:

- **Resources**: Lists basic information for all of the individual .NET projects in your .NET Aspire project, such as the app state, endpoint addresses, and the environment variables that were loaded in.
- **Console**: Displays the console output from each of the projects in your app.
- **Structured**: Displays structured logs in table format. These logs support basic filtering, free-form search, and log level filtering as well. You should see logs from the `apiservice` and the `webfrontend`. You can expand the details of each log entry by selecting the **View** button on the right end of the row.
- **Traces**: Displays the traces for your application, which can track request paths through your apps. Locate a request for **/weather** and select **View** on the right side of the page. The dashboard should display the request in stages as it travels through the different parts of your app.

    :::image type="content" source="media/aspire-dashboard-trace.png" lightbox="media/aspire-dashboard-trace.png" alt-text="A screenshot showing an .NET Aspire dashboard trace for the webfrontend /weather route.":::

- **Metrics**: Displays various instruments and meters that are exposed and their corresponding dimensions for your app. Metrics conditionally expose filters based on their available dimensions.

    :::image type="content" source="media/aspire-dashboard-metrics.png" lightbox="media/aspire-dashboard-metrics.png" alt-text="A screenshot showing an Aspire dashboard metrics page for the webfrontend.":::

For more information, see [.NET Aspire dashboard overview](../fundamentals/dashboard/overview.md).

## Understand the .NET Aspire solution structure

The solution consists of the following projects:

- **AspireSample.ApiService**: An ASP.NET Core Minimal API project is used to provide data to the front end. This project depends on the shared **AspireSample.ServiceDefaults** project.
- **AspireSample.AppHost**: An orchestrator project designed to connect and configure the different projects and services of your app. The orchestrator should be set as the _Startup project_, and it depends on the **AspireSample.ApiService** and **AspireSample.Web** projects.
- **AspireSample.ServiceDefaults**: A .NET Aspire shared project to manage configurations that are reused across the projects in your solution related to [resilience](/dotnet/core/resilience/http-resilience), [service discovery](../service-discovery/overview.md), and [telemetry](../fundamentals/telemetry.md).
- **AspireSample.Web**: An ASP.NET Core Blazor App project with default .NET Aspire service configurations, this project depends on the **AspireSample.ServiceDefaults** project. For more information, see [.NET Aspire service defaults](../fundamentals/service-defaults.md).

Your _AspireSample_ directory should resemble the following:

[!INCLUDE [template-directory-structure](../includes/template-directory-structure.md)]

## Explore the starter projects

Each project in an .NET Aspire solution plays a role in the composition of your app. The _*.Web_ project is a standard ASP.NET Core Blazor App that provides a front end UI. For more information, see [New Blazor Web App template](/aspnet/core/release-notes/aspnetcore-8.0?view=aspnetcore-8.0&preserve-view=true#new-blazor-web-app-template). The _*.ApiService_ project is a standard ASP.NET Core Minimal API template project. Both of these projects depend on the _*.ServiceDefaults_ project, which is a shared project that's used to manage configurations that are reused across projects in your solution.

The two projects of interest in this quickstart are the _*.AppHost_ and _*.ServiceDefaults_ projects detailed in the following sections.

### .NET Aspire host project

The _*.AppHost_ project is responsible for acting as the orchestrator, and sets the `IsAspireHost` property of the project file to `true`:

:::code language="xml" source="snippets/quickstart/AspireSample/AspireSample.AppHost/AspireSample.AppHost.csproj" highlight="8":::

Consider the _:::no-loc text="Program.cs":::_ file of the _AspireSample.AppHost_ project:

:::code source="snippets/quickstart/AspireSample/AspireSample.AppHost/Program.cs":::

If you've used either the [.NET Generic Host](/dotnet/core/extensions/generic-host) or the [ASP.NET Core Web Host](/aspnet/core/fundamentals/host/web-host) before, the app host programming model and builder pattern should be familiar to you. The preceding code:

- Creates an <xref:Aspire.Hosting.IDistributedApplicationBuilder> instance from calling <xref:Aspire.Hosting.DistributedApplication.CreateBuilder?displayProperty=nameWithType>.
- Calls <xref:Aspire.Hosting.RedisBuilderExtensions.AddRedis%2A> with the name `"cache"` to add a Redis server to the app, assigning the returned value to a variable named `cache`, which is of type `IResourceBuilder<RedisResource>`.
- Calls <xref:Aspire.Hosting.ProjectResourceBuilderExtensions.AddProject%2A> given the generic-type parameter with the project's details, adding the `AspireSample.ApiService` project to the application model. This is one of the fundamental building blocks of .NET Aspire, and it's used to configure service discovery and communication between the projects in your app. The name argument `"apiservice"` is used to identify the project in the application model, and used later by projects that want to communicate with it.
- Calls `AddProject` again, this time adding the `AspireSample.Web` project to the application model. It also chains multiple calls to <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> passing the `cache` and `apiservice` variables. The `WithReference` API is another fundamental API of .NET Aspire, which injects either service discovery information or connection string configuration into the project being added to the application model.

Finally, the app is built and run. The <xref:Aspire.Hosting.DistributedApplication.Run?displayProperty=nameWithType> method is provided by the .NET Aspire SDK, and is responsible for starting the app and all of its dependencies. For more information, see [.NET Aspire orchestration overview](../fundamentals/app-host-overview.md).

### .NET Aspire service defaults project

The _*.ServiceDefaults_ project is a shared project that's used to manage configurations that are reused across the projects in your solution. This project ensures that all dependent services share the same resilience, service discovery, and OpenTelemetry configuration. A shared .NET Aspire project file contains the `IsAspireSharedProject` property set as `true`:

:::code language="xml" source="snippets/quickstart/AspireSample/AspireSample.ServiceDefaults/AspireSample.ServiceDefaults.csproj" highlight="7":::

The service defaults project exposes an extension method on the <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> type, named `AddServiceDefaults`. The service defaults project from the template is a starting point, and you can customize it to meet your needs. For more information, see [.NET Aspire service defaults](../fundamentals/service-defaults.md).

## Orchestrate service communication

.NET Aspire provides orchestration features to assist with configuring connections and communication between the different parts of your app. The _AspireSample.AppHost_ project added the _AspireSample.ApiService_ and _AspireSample.Web_ projects to the application model. It also declared their names as `"webfrontend"` for Blazor front end, `"apiservice"` for the API project reference. Additionally, a Redis server resource labelled `"cache"` was added. These names are used to configure service discovery and communication between the projects in your app.

The front end app defines a typed <xref:System.Net.Http.HttpClient> that's used to communicate with the API project.

:::code source="snippets/quickstart/AspireSample/AspireSample.Web/WeatherApiClient.cs":::

The `HttpClient` is configured to use service discovery, consider the following code from the _:::no-loc text="Program.cs":::_ file of the _AspireSample.Web_ project:

:::code source="snippets/quickstart/AspireSample/AspireSample.Web/Program.cs" highlight="7-8,14-19":::

The preceding code:

- Calls `AddServiceDefaults`, configuring the shared defaults for the app.
- Calls <xref:Microsoft.Extensions.Hosting.AspireRedisOutputCacheExtensions.AddRedisOutputCache%2A> with the same `connectionName` that was used when adding the Redis container `"cache"` to the application model. This configures the app to use Redis for output caching.
- Calls <xref:Microsoft.Extensions.DependencyInjection.HttpClientFactoryServiceCollectionExtensions.AddHttpClient%2A> and configures the <xref:System.Net.Http.HttpClient.BaseAddress?displayProperty=nameWithType> to be `"https+http://apiservice"`. This is the name that was used when adding the API project to the application model, and with service discovery configured, it will automatically resolve to the correct address to the API project.

For more information, see [Make HTTP requests with the `HttpClient`](/dotnet/fundamentals/networking/http/httpclient) class.

## See also

- [.NET Aspire components overview](../fundamentals/components-overview.md)
- [Service discovery in .NET Aspire](../service-discovery/overview.md)
- [.NET Aspire service defaults](../fundamentals/service-defaults.md)
- [Health checks in .NET Aspire](../fundamentals/health-checks.md)
- [.NET Aspire telemetry](../fundamentals/telemetry.md)
- [Troubleshoot untrusted localhost certificate in .NET Aspire](../troubleshooting/untrusted-localhost-certificate.md)

## Next steps

> [!div class="nextstepaction"]
> [Tutorial: Add .NET Aspire to an existing .NET app](add-aspire-existing-app.md)
