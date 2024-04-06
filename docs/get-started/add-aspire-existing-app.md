---
title: Add .NET Aspire to an existing .NET 8 microservices app
description: Learn how to add .NET Aspire components, orchestration, and tooling to a microservices app that already exists.
ms.date: 11/15/2023
ms.topic: how-to
---

# Tutorial: Add .NET Aspire to an existing .NET 8 microservices app

If you have already created a microservices .NET web app, you can use Visual Studio to add .NET Aspire to it and get all the features and benefits available to those who enabled .NET Aspire when they selected **File** > **New** > **Project** and chose **Enlist in Aspire orchestration**. In this article, you'll add .NET Aspire orchestration to a simple, pre-existing .NET 8 project. You'll learn how to:

> [!div class="checklist"]
>
> - Understand the structure of the existing microservices app.
> - Enroll existing projects in .NET Aspire orchestration.
> - Understand the changes enrollment makes in the projects.
> - Start the .NET Aspire app.

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

## Get started

Let's start by obtaining the code for the solution:

1. Open a command prompt and change to a directory where you want to store the code.
1. To clone to .NET 8 example solution, use this command:

    ```bash
    git clone https://github.com/MicrosoftDocs/mslearn-dotnet-cloudnative-devops.git eShopLite
    ```

## Explore the sample app

This article uses a .NET 8 solution with three projects:

- **Data Entities**. This project is an example class library. It defines the `Product` class used in the Web App and Web API.
- **Products**. This example Web API returns a list of products in the catalog and their properties.
- **Store**. This example Blazor Web App displays the product catalog to website visitors.

Open and start debugging the project to examine its default behavior:

1. Start Visual Studio and then select **File** > **Open** > **Project/Solution**.
1. Navigate to the top level folder of the solution you cloned, select **eShopLite.sln**, and then select **Open**.
1. In the **Solution Explorer**, right-click the **eShopLite** solution, and then select **Configure Startup Projects**.
1. Select **Multiple startup projects** and then select the **Create new launch profile** button.
1. In the **Action** column, select **Start** for both the **Products** and **Store** projects.
1. Select **OK**.
1. To start debugging the solution, press <kbd>F5</kbd> or select **Start**.
1. Two pages open in the browser:

    - A page displays products in JSON format from a call to the Products Web API.
    - A page displays the homepage of the website. In the menu on the left, select **Products** to see the catalog obtained from the Web API.

1. To stop debugging, close the browser.

## Add .NET Aspire to the Store web app

Now, let's enroll the **Store** project, which implements the web user interface, in .NET Aspire orchestration:

1. In Visual Studio, in the **Solution Explorer**, right-click the **Store** project, select **Add**, and then select **.NET Aspire Orchestrator Support**.
1. In the **Add .NET Aspire Orchestrator Support** dialog, select **OK**.

    :::image type="content" source="media/add-aspire-orchestrator-support.png" alt-text="Screenshot of the Add .NET Aspire Orchestrator Support dialog.":::

Visual Studio adds two new projects to the solution:

- **eShopLite.AppHost**: An orchestrator project designed to connect and configure the different projects and services of your app. The orchestrator is set as the _Startup project_, and it depends on the **eShopLite.Store** project.
- **eShopLite.ServiceDefaults**: A .NET Aspire shared project to manage configurations that are reused across the projects in your solution related to [resilience](/dotnet/core/resilience/http-resilience), [service discovery](../service-discovery/overview.md), and [telemetry](../fundamentals/telemetry.md).

In the **eShopLite.AppHost** project, open the _Program.cs_ file. Notice this line of code, which registers the **Store** project in the .NET Aspire orchestration:

```csharp
builder.AddProject<Projects.Store>("store");
```

For more information, see <xref:Aspire.Hosting.ProjectResourceBuilderExtensions.AddProject%2A>.

To add the **Products** project to .NET Aspire:

1. In Visual Studio, in the **Solution Explorer**, right-click the **Products** project, select **Add**, and then select **.NET Aspire Orchestrator Support**.
1. In the **Microsoft Visual Studio** dialog, select **OK**.
1. In the **Add .NET Aspire Orchestrator Support** dialog, select **OK**.

In the **eShopLite.AppHost** project, open the **Program.cs** file. Notice this line of code, which registers the **Products** project in the .NET Aspire orchestration:

```csharp
builder.AddProject<Projects.Products>("products");
```

Also notice that the **eShopLite.AppHost** project, now depends on both the **Store** and **Products** projects.

## Service Discovery

At this point, both projects are part of .NET Aspire orchestration, but the _Store_ needs to be able to discover the **Products** backend address through .NET Aspire's service discovery. To enable service discovery, open the _Program.cs_ file in **eShopLite.AppHost** and update the code that the _Store_ adds a reference to the _Products_ project:

```csharp
var products = builder.AddProject<Projects.Products>("products");

builder.AddProject<Projects.Store>("store").WithReference(products);
```

Next, update the _appsettings.json_ in the _Store_ project to of the `ProductEndpoint` and `ProductEndpointHttps`:

```json
"ProductEndpoint": "http://products",
"ProductEndpointHttps": "https://products",
```

The address uses the name of the _Products_ project that was added to the orchestrator in the _AppHost_.

## Explore the enrolled app

Let's start the solution and examine the new behavior that .NET Aspire has added.

> [!NOTE]
> Notice that the **eShopLite.AppHost** project is the new startup project.

1. In Visual Studio, to start debugging, press <kbd>F5</kbd> Visual Studio builds the projects.
1. If the **Start Docker Desktop** dialog appears, select **Yes**. Visual Studio starts the Docker engine and creates the necessary containers. When the deployment is complete, the .NET Aspire dashboard is displayed.
1. In the dashboard, select the endpoint for the **products** project. A new browser tab appears and displays the product catalog in JSON format.
1. In the dashboard, select the endpoint for the **store** project. A new browser tab appears and displays the home page for the web app.
1. In the menu on the left, select **Products**. The product catalog is displayed.
1. Close the browser to stop debugging.

Congratulations! You have added .NET Aspire orchestration to your pre-existing web app. You can now add .NET Aspire components and use the .NET Aspire tooling to streamline your cloud-native web app development.
