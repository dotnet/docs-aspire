---
title: Add .NET Aspire to an existing .NET app
description: Learn how to add .NET Aspire integrations, orchestration, and tooling to a microservices app that already exists.
ms.date: 04/16/2025
ms.topic: how-to
zone_pivot_groups: dev-environment
---

# Tutorial: Add .NET Aspire to an existing .NET app

If you have existing microservices and .NET web app, you can add .NET Aspire to it and get all the included features and benefits. In this article, you add .NET Aspire orchestration to a simple, preexisting .NET 9 project. You learn how to:

> [!div class="checklist"]
>
> - Understand the structure of the existing microservices app.
> - Enroll existing projects in .NET Aspire orchestration.
> - Understand the changes enrollment makes in the projects.
> - Start the .NET Aspire project.

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

## Get started

Let's start by obtaining the code for the solution:

1. Open a command prompt and change directories to where you want to store the code.
1. To clone to .NET 9 example solution, use the following `git clone` command:

    ```bash
    git clone https://github.com/MicrosoftDocs/mslearn-dotnet-cloudnative-devops.git eShopLite
    ```

## Explore the sample app

This article uses a .NET 9 solution with three projects:

- **Data Entities**: This project is an example class library. It defines the `Product` class used in the Web App and Web API.
- **Products**: This example Web API returns a list of products in the catalog and their properties.
- **Store**: This example Blazor Web App displays the product catalog to website visitors.

### Sample app architecture

To better understand the structure of the sample app, consider the following diagram, which illustrates its simple three-tier architecture:

:::image type="content" source="media/product-store-architecture-thumb.png" alt-text="Simple three-tier architecture diagram showing a product database, product API, and store web app." lightbox="media/product-store-architecture.png":::

This layered design ensures a clear separation of concerns, making the app easier to maintain and scale.

### Run the sample app

Open and start debugging the project to examine its default behavior:

:::zone pivot="visual-studio"

1. Start Visual Studio and then select **File** > **Open** > **Project/Solution**.
1. Navigate to the top level folder of the solution you cloned, select **eShopLite.sln**, and then select **Open**.
1. In the **Solution Explorer**, right-click the **eShopLite** solution, and then select **Configure Startup Projects**.
1. Select **Multiple startup projects**.
1. In the **Action** column, select **Start** for both the **Products** and **Store** projects.
1. Select **OK**.
1. To start debugging the solution, press <kbd>F5</kbd> or select **Start**.
1. Two pages open in the browser:

    - A page displays products in JSON format from a call to the Products Web API.
    - A page displays the homepage of the website. In the menu on the left, select **Products** to see the catalog obtained from the Web API.

1. To stop debugging, close the browser.

:::zone-end
:::zone pivot="vscode"

1. Start Visual Studio Code and open the folder that you cloned. From the terminal where you cloned the repo, run the following command:

    ```bash
    code .
    ```

1. Select the **Run and Debug** menu item, or press <kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>D</kbd>.
1. Select the **create a launch.json file** link.

    :::image type="content" source="media/vscode-launch.json.png" lightbox="media/vscode-launch.json.png" alt-text="Visual Studio Code: Run and Debug create launch.json file.":::

1. Copy and paste the following JSON into this file and **Save**:

    ## [Unix](#tab/unix)

    ```json
    {
        "version": "0.2.0",
        "compounds": [
            {
                "name": "Run all",
                "configurations": [
                    "Run products",
                    "Run store",
                ]
            }
        ],
        "configurations": [
            {
                "name": "Run products",
                "type": "dotnet",
                "request": "launch",
                "projectPath": "${workspaceFolder}/Products/Products.csproj"
            },
            {
                "name": "Run store",
                "type": "dotnet",
                "request": "launch",
                "projectPath": "${workspaceFolder}/Store/Store.csproj"
            }
        ]
    }
    ```

    ## [Windows](#tab/windows)

    ```json
    {
        "version": "0.2.0",
        "compounds": [
            {
                "name": "Run all",
                "configurations": [
                    "Run products",
                    "Run store",
                ]
            }
        ],
        "configurations": [
            {
                "name": "Run products",
                "type": "dotnet",
                "request": "launch",
                "projectPath": "${workspaceFolder}\\Products\\Products.csproj"
            },
            {
                "name": "Run store",
                "type": "dotnet",
                "request": "launch",
                "projectPath": "${workspaceFolder}\\Store\\Store.csproj"
            }
        ]
    }
    ```

    ---

1. To start debugging the solution, press <kbd>F5</kbd> or select **Start**.
1. Two pages open in the browser:

    - A page displays products in JSON format from a call to the Products Web API.
    - A page displays the homepage of the website. In the menu on the left, select **Products** to see the catalog obtained from the Web API.

1. To stop debugging, close the browser, and then select the **Stop** button twice (once for each running debug instance).

:::zone-end
:::zone pivot="dotnet-cli"

1. Open a terminal window and change directories into the newly cloned repository.
1. To start the _Products_ app, run the following command:

    ```dotnetcli
    dotnet run --project ./Products/Products.csproj
    ```

1. A browser page opens, displaying the JSON for the products.
1. In a separate terminal window, again change directories to cloned repository.
1. Start the _Store_ app by running the following command:

    ```dotnetcli
    dotnet run --project ./Store/Store.csproj
    ```

1. The browser opens a page that displays the homepage of the website. In the menu on the left, select **Products** to see the catalog obtained from the Web API.

1. To stop debugging, close the browser, and press <kbd>Ctrl</kbd>+<kbd>C</kbd> in both terminals.

:::zone-end

No matter which tool you use—starting multiple projects manually or configuring connections between them is tedious. Additionally, the **Store** project requires explicit endpoint configuration for the **Products** API, which is both cumbersome and prone to errors. This is where .NET Aspire simplifies and streamlines the process!

## Ensure .NET Aspire templates are installed

If you've worked with .NET Aspire on your current computer before, you likely have the necessary .NET project templates already installed. You can check by using the following command:

```dotnetcli
dotnet new list aspire
```

If the .NET Aspire templates are installed, the output resembles:

```dotnetcli
These templates matched your input: '.NET Aspire'

Template Name       Short Name              Language  Tags
------------------  ----------------------  --------  -------------------------------------------------------------------------------
.NET Aspire App...  aspire-apphost          [C#]      Common/.NET Aspire/Cloud
.NET Aspire Emp...  aspire                  [C#]      Common/.NET Aspire/Cloud/Web/Web API/API/Service
.NET Aspire Ser...  aspire-servicedefaults  [C#]      Common/.NET Aspire/Cloud/Web/Web API/API/Service
.NET Aspire Sta...  aspire-starter          [C#]      Common/.NET Aspire/Blazor/Web/Web API/API/Service/Cloud/Test/MSTest/NUnit/xUnit
.NET Aspire Tes...  aspire-mstest           [C#]      Common/.NET Aspire/Cloud/Web/Web API/API/Service/Test/MSTest
.NET Aspire Tes...  aspire-nunit            [C#]      Common/.NET Aspire/Cloud/Web/Web API/API/Service/Test/NUnit
.NET Aspire Tes...  aspire-xunit            [C#]      Common/.NET Aspire/Cloud/Web/Web API/API/Service/Test/xUnit
```

In this tutorial, you'll add a App Host project and a Service Defaults project.

If the previous command didn't find any templates you must install them. Execute this command:

```dotnetcli
dotnet new install Aspire.ProjectTemplates
```

For more information about the .NET Aspire templates, see [.NET Aspire templates](../fundamentals/setup-tooling.md#net-aspire-templates)

## Add .NET Aspire to the Store web app

Now, let's enroll the **Store** project, which implements the web user interface, in .NET Aspire orchestration:

:::zone pivot="visual-studio"

1. In Visual Studio, in the **Solution Explorer**, right-click the **Store** project, select **Add**, and then select **.NET Aspire Orchestrator Support**.
1. In the **Add .NET Aspire Orchestrator Support** dialog, select **OK**.

    :::image type="content" loc-scope="visual-studio" source="media/add-aspire-orchestrator-support.png" alt-text="Screenshot of the Add .NET Aspire Orchestrator Support dialog.":::

You should now have two new projects, both added to the solution:

- **eShopLite.AppHost**: An orchestrator project designed to connect and configure the different projects and services of your app. The orchestrator is set as the _Startup project_, and it depends on the **eShopLite.Store** project.
- **eShopLite.ServiceDefaults**: A .NET Aspire shared project to manage configurations that are reused across the projects in your solution related to [resilience](/dotnet/core/resilience/http-resilience), [service discovery](../service-discovery/overview.md), and [telemetry](../fundamentals/telemetry.md).

In the **eShopLite.AppHost** project, open the _:::no-loc text="Program.cs":::_ file. Notice this line of code, which registers the **Store** project in the .NET Aspire orchestration:

```csharp
builder.AddProject<Projects.Store>("store");
```

For more information, see <xref:Aspire.Hosting.ProjectResourceBuilderExtensions.AddProject%2A>.

To add the **Products** project to .NET Aspire:

1. In Visual Studio, in the **Solution Explorer**, right-click the **Products** project, select **Add**, and then select **.NET Aspire Orchestrator Support**.
1. A dialog indicating that .NET Aspire Orchestrator project already exists, select **OK**.

    :::image type="content" loc-scope="visual-studio" source="media/orchestrator-already-added.png" alt-text="Screenshot indicating that the.NET Aspire Orchestrator was already added.":::

In the **eShopLite.AppHost** project, open the _:::no-loc text="Program.cs":::_ file. Notice this line of code, which registers the **Products** project in the .NET Aspire orchestration:

```csharp
builder.AddProject<Projects.Products>("products");
```

Also notice that the **eShopLite.AppHost** project, now depends on both the **Store** and **Products** projects.

:::zone-end
:::zone pivot="vscode,dotnet-cli"

### Create an app host project

In order to orchestrate the existing projects, you need to create a new _app host_ project. To create a new [_app host_ project](../fundamentals/app-host-overview.md) from the available .NET Aspire templates, use the following .NET CLI command:

```dotnetcli
dotnet new aspire-apphost -o eShopLite.AppHost
```

Add the _app host_ project to existing solution:

## [Unix](#tab/unix)

```dotnetcli
dotnet sln ./eShopLite.sln add ./eShopLite.AppHost/eShopLite.AppHost.csproj
```

## [Windows](#tab/windows)

```dotnetcli
dotnet sln .\eShopLite.sln add .\eShopLite.AppHost\eShopLite.AppHost.csproj
```

---

Add the **Store** project as a project reference to the _app host_ project using the following .NET CLI command:

## [Unix](#tab/unix)

```dotnetcli
dotnet add ./eShopLite.AppHost/eShopLite.AppHost.csproj reference ./Store/Store.csproj
```

## [Windows](#tab/windows)

```dotnetcli
dotnet add .\eShopLite.AppHost\eShopLite.AppHost.csproj reference .\Store\Store.csproj
```

---

For more information on the available templates, see [.NET Aspire templates](../fundamentals/aspire-sdk-templates.md).

### Create a service defaults project

After the app host project is created, you need to create a new _service defaults_ project. To create a new [_service defaults_ project](../fundamentals/service-defaults.md) from the available .NET Aspire templates, use the following .NET CLI command:

```dotnetcli
dotnet new aspire-servicedefaults -o eShopLite.ServiceDefaults
```

To add the project to the solution, use the following .NET CLI command:

## [Unix](#tab/unix)

```dotnetcli
dotnet sln ./eShopLite.sln add ./eShopLite.ServiceDefaults/eShopLite.ServiceDefaults.csproj
```

## [Windows](#tab/windows)

```dotnetcli
dotnet sln .\eShopLite.sln add .\eShopLite.ServiceDefaults\eShopLite.ServiceDefaults.csproj
```

---

Update the _app host_ project to add a project reference to the **Products** project:

## [Unix](#tab/unix)

```dotnetcli
dotnet add ./eShopLite.AppHost/eShopLite.AppHost.csproj reference ./Products/Products.csproj
```

## [Windows](#tab/windows)

```dotnetcli
dotnet add .\eShopLite.AppHost\eShopLite.AppHost.csproj reference .\Products\Products.csproj
```

---

Both the **Store** and **Products** projects need to reference the _service defaults_ project so that they can easily include [service discovery](../service-discovery/overview.md). To add a reference to the _service defaults_ project in the **Store** project, use the following .NET CLI command:

## [Unix](#tab/unix)

```dotnetcli
dotnet add ./Store/Store.csproj reference ./eShopLite.ServiceDefaults/eShopLite.ServiceDefaults.csproj
```

## [Windows](#tab/windows)

```dotnetcli
dotnet add .\Store\Store.csproj reference .\eShopLite.ServiceDefaults\eShopLite.ServiceDefaults.csproj
```

---

The same command with slightly different paths should be used to add a reference to the _service defaults_ project in the **Products** project:

## [Unix](#tab/unix)

```dotnetcli
dotnet add ./Products/Products.csproj reference ./eShopLite.ServiceDefaults/eShopLite.ServiceDefaults.csproj
```

## [Windows](#tab/windows)

```dotnetcli
dotnet add .\Products\Products.csproj reference .\eShopLite.ServiceDefaults\eShopLite.ServiceDefaults.csproj
```

---

In both the **Store** and **Products** projects, update their _:::no-loc text="Program.cs":::_ files, adding the following line immediately after their `var builder = WebApplication.CreateBuilder(args);` line:

```csharp
builder.AddServiceDefaults();
```

### Update the app host project

Open the _:::no-loc text="Program.cs":::_ file of the _app host_ project, and replace its contents with the following C# code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Store>("store");

builder.AddProject<Projects.Products>("products");

builder.Build().Run();
```

The preceding code:

- Creates a new <xref:Aspire.Hosting.DistributedApplicationBuilder> instance.
- Adds the **Store** project to the orchestrator.
- Adds the **Products** project to the orchestrator.
- Builds and runs the orchestrator.

:::zone-end

## Service Discovery

At this point, both projects are part of .NET Aspire orchestration, but the **Store** project needs to rely on the **Products** backend address through [.NET Aspire's service discovery](../service-discovery/overview.md). To enable service discovery, open the _:::no-loc text="Program.cs":::_ file in **eShopLite.AppHost** project and update the code so that the `builder` adds a reference to the _Products_ project:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var products = builder.AddProject<Projects.Products>("products");

builder.AddProject<Projects.Store>("store")
       .WithExternalHttpEndpoints()
       .WithReference(products)
       .WaitFor(products);

builder.Build().Run();
```

The preceding code expresses that the _Store_ project depends on the _Products_ project. For more information, see [.NET Aspire app host: Reference resources](../fundamentals/app-host-overview.md#reference-resources). This reference is used to discover the address of the _Products_ project at run time. Additionally, the _Store_ project is configured to use external HTTP endpoints. If you later choose to deploy this app, you'd need the call to <xref:Aspire.Hosting.ResourceBuilderExtensions.WithExternalHttpEndpoints%2A> to ensure that it's public to the outside world. Finally, the <xref:Aspire.Hosting.ResourceBuilderExtensions.WaitFor*> API ensures that _Store_ app waits for the _Products_ app to be ready to serve requests.

Next, update the _:::no-loc text="appsettings.json":::_ in the _Store_ project with the following JSON:

```json
{
  "DetailedErrors": true,
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ProductEndpoint": "http://products",
  "ProductEndpointHttps": "https://products"
}
```

The addresses for both the endpoints now uses the "products" name that was added to the orchestrator in the _app host_. These names are used to discover the address of the _Products_ project.

## Explore the enrolled app

Let's start the solution and examine the new behavior that .NET Aspire provides.

:::zone pivot="visual-studio"

> [!NOTE]
> Notice that the **eShopLite.AppHost** project is the new startup project.

1. In Visual Studio, to start debugging, press <kbd>F5</kbd> Visual Studio builds the projects.
1. If the **Start Docker Desktop** dialog appears, select **Yes**. Visual Studio starts the Docker engine and creates the necessary containers. When the deployment is complete, the .NET Aspire dashboard is displayed.
1. In the dashboard, select the endpoint for the **products** project. A new browser tab appears and displays the product catalog in JSON format.
1. In the dashboard, select the endpoint for the **store** project. A new browser tab appears and displays the home page for the web app.
1. In the menu on the left, select **Products**. The product catalog is displayed.
1. To stop debugging, close the browser.

:::zone-end
:::zone pivot="vscode"

Delete the _launch.json_ file that you created earlier, it no longer serves a purpose. Instead, start the _app host_ project, which orchestrates the other projects:

1. Start the _app host_ project by right-clicking the **eShopLite.AppHost** project in the **Solution Explorer** and selecting **Debug** > **Start New Instance**:

    :::image type="content" source="media/vscode-run-app-host.png" lightbox="media/vscode-run-app-host.png" alt-text="Visual Studio Code: Solution Explorer selecting Debug > Start New Instance." :::

    > [!NOTE]
    > If Docker Desktop (or Podman) isn't running, you experience an error. Start the container runtime and try again.

:::zone-end
:::zone pivot="dotnet-cli"

1. Start the _app host_ project by running the following command:

    ```dotnetcli
    dotnet run --project ./eShopLite.AppHost/eShopLite.AppHost.csproj
    ```

    > [!NOTE]
    > If Docker Desktop (or Podman) isn't running, you experience an error. Start the container runtime and try again.

:::zone-end
:::zone pivot="vscode,dotnet-cli"

<!-- markdownlint-disable MD029 -->
<!-- We need to continue from the previous shared step from a different pivot. -->

2. In the dashboard, select the endpoint for the **products** project. A new browser tab appears and displays the product catalog in JSON format.
3. In the dashboard, select the endpoint for the **store** project. A new browser tab appears and displays the home page for the web app.
4. In the menu on the left, select **Products**. The product catalog is displayed.
5. To stop debugging, close the browser.

<!-- markdownlint-enable MD029 -->

:::zone-end

Congratulations, you added .NET Aspire orchestration to your preexisting web app. You can now add .NET Aspire integrations and use the .NET Aspire tooling to streamline your cloud-native web app development.
