---
title: Build your first Aspire solution
description: Learn how to build your first Aspire solution using the Aspire Starter application template.
ms.date: 10/08/2025
ms.topic: quickstart
zone_pivot_groups: dev-tools
ms.custom: sfi-ropc-nochange
---

# Quickstart: Build your first Aspire solution

Cloud-native apps often require connections to various services such as databases, storage and caching solutions, messaging providers, or other web services. Aspire is designed to streamline connections and configurations between these types of services. This quickstart shows how to create an Aspire Starter Application template solution.

In this quickstart, you explore the following tasks:

> [!div class="checklist"]
>
> - Create a basic app that is set up to use Aspire.
> - Test the basic functionality of the app.
> - Use the Aspire dashboard to monitor your app as it runs.
> - Understand the structure of an Aspire solution.

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

## Create the Aspire application

In this quickstart, you can choose whether to use the Aspire CLI or Visual Studio Code to create a new Aspire Starter application.

:::zone pivot="aspire-cli"

[!INCLUDE [aspire-cli-new-app](../includes/aspire-cli-new-app.md)]

:::zone-end
:::zone pivot="vscode"

[!INCLUDE [vscode-file-new](../includes/vscode-file-new.md)]

:::zone-end
:::zone pivot="visual-studio"

[!INCLUDE [vscode-file-new](../includes/visual-studio-file-new.md)]

:::zone-end


> [!NOTE]
> You chose not to add a test project to the new solution. You can always add a test project later but it's not needed for this quickstart.

For more information on the available templates, see [Aspire templates](../fundamentals/aspire-sdk-templates.md).

## Test the app locally

The sample app includes a frontend Blazor app that communicates with a Minimal API project. The API project is used to provide _fake_ weather data to the frontend. The frontend app is configured to use service discovery to connect to the API project. The API project is configured to use output caching with Redis. The sample app is now ready for testing. You want to verify the following conditions:

- Weather data is retrieved from the API project using service discovery and displayed on the weather page.
- Subsequent requests are handled via the output caching configured by the Aspire Redis integration.

:::zone pivot="aspire-cli"

1. In the command prompt, navigate to the folder where you created the solution. It will be a subdirectory of the location where you ran the `aspire new` command.
1. To run the Aspire solution, execute this command:

    ```Aspire
    aspire run
    ```

1. The `aspire run` command compiles all the projects in the solution, and then locates and runs the AppHost project. It displays information about the solution, including the location of the Aspire dashboard:

    :::image type="content" source="media/aspire-cli-run.png" lightbox="media/aspire-cli-run.png" alt-text="A screenshot of the Aspire CLI running an Aspire solution.":::

1. To browse the Aspire dashboard, hold down <kbd>CTRL</kbd> and select the **Dashboard** URL.

:::zone-end
:::zone pivot="vscode"

In Visual Studio Code, press <kbd>F5</kbd> to launch the app. You're prompted to select which language, and C# is suggested. Select **C#** and then select the **AspireSample.AppHost** project with the **Default Configuration**:

:::image type="content" loc-scope="vs-code" source="media/vscode-run.png" lightbox="media/vscode-run.png" alt-text="A screenshot of the Visual Studio Code launch configuration for the AspireSample.AppHost project.":::

If this is the first time you're running Aspire, or it's a new machine with a new .NET installation, you're prompted to install a self-signed localhost certificate—and the project will fail to launch:

:::image type="content" loc-scope="vs-code" source="media/vscode-run-accept-cert.png" lightbox="media/vscode-run-accept-cert.png" alt-text="A screenshot of the Visual Studio Code breaking on an exception and prompting to create a trusted self-signed certificate.":::

Select **Yes**, and you see an informational message indicating that the **Self-signed certificate successfully created**:

:::image type="content" loc-scope="vs-code" source="media/vscode-run-cert-created.png" lightbox="media/vscode-run-cert-created.png" alt-text="A screenshot of the Visual Studio Code success message for creating a self-signed certificate.":::

If you're still having an issue, close all browser windows and try again. For more information, see [Troubleshoot untrusted localhost certificate in Aspire](../troubleshooting/untrusted-localhost-certificate.md).

> [!TIP]
> If you're on MacOS and using Safari, when your browser opens if the page is blank, you might need to manually refresh the page.

:::zone-end
:::zone pivot="visual-studio"

In Visual Studio, set the **AspireSample.AppHost** project as the startup project by right-clicking on the project in the **Solution Explorer** and selecting **Set as Startup Project**. It might already have been automatically set as the startup project. Once set, press <kbd>F5</kbd> or (<kbd>Ctrl</kbd> + <kbd>F5</kbd> to run without debugging) to run the app.

:::zone-end


1. The app displays the Aspire dashboard in the browser. You look at the dashboard in more detail later. For now, find the **webfrontend** project in the list of resources and select the project's **localhost** endpoint.

    :::image type="content" source="media/aspire-dashboard-webfrontend.png" lightbox="media/aspire-dashboard-webfrontend.png" alt-text="A screenshot of the Aspire Dashboard, highlighting the webfrontend project's localhost endpoint.":::

    The home page of the **webfrontend** app displays "Hello, world!"

1. Navigate from the home page to the weather page in the using the left side navigation. The weather page displays weather data. Make a mental note of some of the values represented in the forecast table.
1. Continue occasionally refreshing the page for 10 seconds. Within 10 seconds, the cached data is returned. Eventually, a different set of weather data appears, since the data is randomly generated and the cache is updated.

:::image type="content" source="media/weather-page.png" lightbox="media/weather-page.png" alt-text="The Weather page of the webfrontend app showing the weather data retrieved from the API.":::

🤓 Congratulations! You created and ran your first Aspire solution! Close the browser window.

:::zone pivot="aspire-cli"

To stop the app, switch to the command prompt where the `aspire run` command is still running and then press <kbd>CTRL</kbd> + <kbd>C</kbd>.

:::zone-end
:::zone pivot="vscode"

To stop the app in Visual Studio Code, press <kbd>Shift</kbd> + <kbd>F5</kbd>, or select the **Stop** button at the top center of the window:

:::image type="content" loc-scope="vs-code" source="media/vscode-stop.png" lightbox="media/vscode-stop.png" alt-text="A screenshot of the Visual Studio Code stop button.":::

:::zone-end
:::zone pivot="visual-studio"

To stop the app in Visual Studio, select the **Stop Debugging** from the **Debug** menu.

:::zone-end

Next, investigate the structure and other features of your new Aspire solution.

## Explore the Aspire dashboard

When you run an Aspire project, a [dashboard](../fundamentals/dashboard/overview.md) launches that you use to monitor various parts of your app. The dashboard resembles the following screenshot:

:::image type="content" source="media/aspire-dashboard.png" lightbox="media/aspire-dashboard.png" alt-text="A screenshot of the Aspire Dashboard, depicting the Projects tab.":::

Visit each page using the left navigation to view different information about the Aspire resources:

- **Resources**: Lists basic information for all of the individual .NET projects and other resources in your Aspire project, such as the app state, endpoint addresses, and the environment variables that were loaded in. The **Graph** tab displays dependencies between these components:

    :::image type="content" source="media/aspire-dashboard-graph.png" lightbox="media/aspire-dashboard-graph.png" alt-text="A screenshot showing the Graph page of the Aspire dashboard.":::

- **Console**: Displays the console output from each of the resources in your app.
- **Structured**: Displays structured logs in table format. These logs support basic filtering, free-form search, and log level filtering as well. You should see logs from the `apiservice` and the `webfrontend`. You can expand the details of each log entry by selecting the **View** button on the right end of the row.
- **Traces**: Displays the traces for your application, which can track request paths through your apps. Locate a request for **/weather** and select **View** on the right side of the page. The dashboard should display the request in stages as it travels through the different parts of your app.

    :::image type="content" source="media/aspire-dashboard-trace.png" lightbox="media/aspire-dashboard-trace.png" alt-text="A screenshot showing an Aspire dashboard trace for the webfrontend /weather route.":::

- **Metrics**: Displays various instruments and meters that are exposed and their corresponding dimensions for your app. Metrics conditionally expose filters based on their available dimensions.

    :::image type="content" source="media/aspire-dashboard-metrics.png" lightbox="media/aspire-dashboard-metrics.png" alt-text="A screenshot showing an Aspire dashboard metrics page for the webfrontend.":::

For more information, see [Aspire dashboard overview](../fundamentals/dashboard/overview.md).

## Understand the Aspire solution structure

The solution consists of the following projects:

- **AspireSample.ApiService**: An ASP.NET Core Minimal API project is used to provide data to the front end. This project depends on the shared **AspireSample.ServiceDefaults** project.
- **AspireSample.AppHost**: An orchestrator project designed to connect and configure the different projects and services of your app. The orchestrator should be set as the _Startup project_, and it depends on the **AspireSample.ApiService** and **AspireSample.Web** projects.
- **AspireSample.ServiceDefaults**: A Aspire shared project to manage configurations that are reused across the projects in your solution related to [resilience](/dotnet/core/resilience/http-resilience), [service discovery](../service-discovery/overview.md), and [telemetry](../fundamentals/telemetry.md).
- **AspireSample.Web**: An ASP.NET Core Blazor App project with default Aspire service configurations, this project depends on the **AspireSample.ServiceDefaults** project. For more information, see [Aspire service defaults](../fundamentals/service-defaults.md).

Your _AspireSample_ directory should resemble the following structure:

[!INCLUDE [template-directory-structure](../includes/template-directory-structure.md)]

## See also

- [Aspire integrations overview](../fundamentals/integrations-overview.md)
- [Service discovery in Aspire](../service-discovery/overview.md)
- [Aspire service defaults](../fundamentals/service-defaults.md)
- [Health checks in Aspire](../fundamentals/health-checks.md)
- [Aspire telemetry](../fundamentals/telemetry.md)
- [Troubleshoot untrusted localhost certificate in Aspire](../troubleshooting/untrusted-localhost-certificate.md)

## Next steps

> [!div class="nextstepaction"]
> [Tutorial: Add Aspire to an existing .NET app](add-aspire-existing-app.md)
