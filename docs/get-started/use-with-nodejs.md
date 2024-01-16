---
title: Build .NET Aspire apps with Node.js
description: Learn how to build .NET Aspire apps with Node.js.
ms.date: 01/16/2024
---

# Build .NET Aspire apps with Node.js

In this article, you learn how to build .NET Aspire apps that use Node.js and Node Package Manager (`npm`). The sample app in this article demonstrates Angular, React, and Vue client experiences. Node support is available through the <xref:Aspire.Hosting.NodeAppHostingExtension.AddNodeApp%2A> API, while `npm` is available with <xref:Aspire.Hosting.NodeAppHostingExtension.AddNpmApp%2A>. The difference between these two APIs is that the former is used to host Node.js apps, while the latter is used to host apps that execute from a _package.json_ file.

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

Additionally, you need to install the [Node.js](https://nodejs.org/en/download/) on your machine.

## Clone sample source code

The sample code for this article is available on [GitHub](https://github.com/dotnet/aspire-samples/tree/main/samples/AspireWithJavaScript). Clone the repository to your local machine:

```bash
git clone https://github.com/dotnet/aspire-samples.git
```

After cloning the repository, navigate to the `samples/AspireWithJavaScript` folder:

```bash
cd samples/AspireWithJavaScript
```

From this directory, there are six child directories described in the following list:

- **AspireJavaScript.Angular**: An Angular app that consumes the weather forecast API and displays the data in a table.
- **AspireJavaScript.AppHost**: A .NET Aspire app that orchestrates the other apps in this sample. For more information, see [.NET Aspire orchestration overview](../fundamentals/app-host-overview.md).
- **AspireJavaScript.MinimalApi**: An HTTP API that returns randomly generated weather forecast data.
- **AspireJavaScript.React**: A React app that consumes the weather forecast API and displays the data in a table.
- **AspireJavaScript.ServiceDefaults**: The default shared project for .NET Aspire apps. For more information, see [.NET Aspire service defaults](../fundamentals/service-defaults.md).
- **AspireJavaScript.Vue**: A Vue app that consumes the weather forecast API and displays the data in a table.

## Install client dependencies

The sample app demonstrates how to use JavaScript client apps that are built atop Node.js. Each client app was initially based on a template created by the `npm` CLI. The following table lists the template commands used to create each client app, along with the default port:

| App type                       | Create template command      | Default port |
|--------------------------------|------------------------------|--------------|
| [Angular](https://angular.dev) | `npm create @angular@latest` | 4200         |
| [React](https://react.dev)     | `npm create reactapp@latest` | 3000         |
| [VueJS](https://vuejs.org)     | `npm create vue@latest`      | 5173         |

> [!TIP]
> You don't need to run any of these commands, since the sample app already includes the clients. Instead, this is a point of reference from which the clients were created.

To run the app, you need to install the dependencies for the client apps first. To do so, navigate to each client folder and run `npm install` (or the install alias `npm i`) commands:

### Angular

```nodejs
npm i ./AspireJavaScript.Angular/
```

For more information, see [Angular client](#explore-the-angular-client).

### React

```nodejs
npm i ./AspireJavaScript.React/
```

For more information, see [React client](#explore-the-react-client).

### Vue

```nodejs
npm i ./AspireJavaScript.Vue/
```

For more information, see [Vue client](#explore-the-vue-client).

## Run the sample app

To run the sample app, call the `dotnet run` command give the _AspireJavaScript.AppHost.csproj_ project:

```dotnetcli
dotnet run --project ./AspireJavaScript.AppHost/AspireJavaScript.AppHost.csproj
```

The .NET Aspire dashboard launches in your default browser, and each client app endpoint displays in the dashboard. The following image shows the dashboard:

:::image type="content" source="media/aspire-dashboard-with-nodejs.png" lightbox="media/aspire-dashboard-with-nodejs.png" alt-text=".NET Aspire dashboard with multiple JavaScript client apps.":::

## Explore the app host

The app host code declares the client app resources using the `AddNpmApp` API.

:::code source="~/../samples/AspireWithJavaScript/AspireJavaScript.AppHost/Program.cs":::

The preceding code:

- Creates a <xref:Aspire.Hosting.DistributedApplicationBuilder>.
- Adds the "weatherapi" service as a project to the app host.
- With a reference to the "weatherapi" service, adds the "angular", "react", and "vue" client apps as npm apps.
  - Each client app is configured to run on a different container port, and uses the `PORT` environment variable to determine the port.
  - All client apps also rely on a _Dockerfile_ to build their container image, and are configured to express themselves in the publishing manifest as a container from the <xref:Aspire.Hosting.ExecutableResourceBuilderExtensions.AsDockerfileInManifest%2A>.

For more information on inner-loop networking, see [.NET Aspire inner-loop networking overview](../fundamentals/networking-overview.md). For more information on deploying apps, see [.NET Aspire manifest format for deployment tool builders](../deployment/manifest-format.md).

## Explore the Angular client

<!-- Update package.json / start: ng server --port %PORT% -->

:::code language="javascript" source="~/../samples/AspireWithJavaScript/AspireJavaScript.Angular/proxy.conf.js":::

:::code language="json" source="~/../samples/AspireWithJavaScript/AspireJavaScript.Angular/package.json":::

:::code language="typescript" source="~/../samples/AspireWithJavaScript/AspireJavaScript.Angular/src/app/app.component.ts":::

### Angular app running

:::image type="content" source="media/angular-app.png" lightbox="media/angular-app.png" alt-text="Angular client app with fake forecast weather data displayed as a table.":::

## Explore the React client

:::code language="ini" source="~/../samples/AspireWithJavaScript/AspireJavaScript.React/.env":::

:::code language="javascript" source="~/../samples/AspireWithJavaScript/AspireJavaScript.React/src/index.js":::

:::code language="javascript" source="~/../samples/AspireWithJavaScript/AspireJavaScript.React/src/App.js":::

### React app running

:::image type="content" source="media/react-app.png" lightbox="media/react-app.png" alt-text="React client app with fake forecast weather data displayed as a table.":::

## Explore the Vue client

:::code language="ini" source="~/../samples/AspireWithJavaScript/AspireJavaScript.Vue/.env":::

Environment type definitions are available in the _env.d.ts_ file. This file is referenced in the _tsconfig.json_ file, and is used to provide type information for the `process.env` object.

:::code language="typescript" source="~/../samples/AspireWithJavaScript/AspireJavaScript.Vue/env.d.ts":::

:::code language="javascript" source="~/../samples/AspireWithJavaScript/AspireJavaScript.Vue/vite.config.js":::

### Vue app running

:::image type="content" source="media/vue-app.png" lightbox="media/vue-app.png" alt-text="Vue client app with fake forecast weather data displayed as a table.":::
