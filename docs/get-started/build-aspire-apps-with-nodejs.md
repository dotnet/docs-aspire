---
title: Build .NET Aspire apps with Node.js
description: Learn how to build .NET Aspire apps with Node.js.
ms.date: 01/16/2024
---

# Build .NET Aspire apps with Node.js

In this article, you learn how to build .NET Aspire apps that use Node.js and Node Package Manager (`npm`). The sample app in this article demonstrates Angular, React, and Vue client experiences. Node.js support is available through the <xref:Aspire.Hosting.NodeAppHostingExtension.AddNodeApp%2A> API, while [`npm` apps](https://docs.npmjs.com/cli/using-npm/scripts) are available with <xref:Aspire.Hosting.NodeAppHostingExtension.AddNpmApp%2A>. The difference between these two APIs is that the former is used to host Node.js apps, while the latter is used to host apps that execute from a _package.json_ file's `scripts` section—and the corresponding `npm run <script-name>` command.

> [!TIP]
> The sample source code for this article is available on [GitHub](https://github.com/dotnet/aspire-samples/tree/main/samples/AspireWithJavaScript), and there are details available on the [Code Samples: .NET Aspire with Angular, React and Vue](/samples/dotnet/aspire-samples/aspire-angular-react-vue) page.

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

Additionally, you need to install [Node.js](https://nodejs.org/en/download/) on your machine. The sample app in this article was built with Node.js version 20.7.0 and npm version 9.7.2. To verify your Node.js and npm versions, run the following commands:

```nodejs
node --version
```

```nodejs
npm --version
```

## Clone sample source code

To clone the sample source code from [GitHub](https://github.com/dotnet/aspire-samples/tree/main/samples/AspireWithJavaScript), run the following command:

```bash
git clone https://github.com/dotnet/aspire-samples.git
```

After cloning the repository, navigate to the _samples/AspireWithJavaScript_ folder:

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

The sample app demonstrates how to use JavaScript client apps that are built on top of Node.js. Each client app was initially based on a template created by the `npm` CLI. The following table lists the template commands used to create each client app, along with the default port:

| App type                       | Create template command      | Default port |
|--------------------------------|------------------------------|--------------|
| [Angular](https://angular.dev) | `npm create @angular@latest` | 4200         |
| [React](https://react.dev)     | `npm create reactapp@latest` | 3000         |
| [Vue](https://vuejs.org)       | `npm create vue@latest`      | 5173         |

> [!TIP]
> You don't need to run any of these commands, since the sample app already includes the clients. Instead, this is a point of reference from which the clients were created. For more information, see [npm-init](https://docs.npmjs.com/cli/commands/npm-init).

To run the app, you first need to install the dependencies for each client. To do so, navigate to each client folder and run `npm install` (or the install alias `npm i`) commands.

### Install Angular dependencies

```nodejs
npm i ./AspireJavaScript.Angular/
```

For more information on the Angular app, see [Angular client](#explore-the-angular-client).

### Install React dependencies

```nodejs
npm i ./AspireJavaScript.React/
```

For more information on the React app, see [React client](#explore-the-react-client).

### Install Vue dependencies

```nodejs
npm i ./AspireJavaScript.Vue/
```

For more information on the Vue app, see [Vue client](#explore-the-vue-client).

## Run the sample app

To run the sample app, call the [dotnet run](/dotnet/core/tools/dotnet-run) command given the orchestrator app host _AspireJavaScript.AppHost.csproj_ as the `--project` switch:

```dotnetcli
dotnet run --project ./AspireJavaScript.AppHost/AspireJavaScript.AppHost.csproj
```

The .NET Aspire dashboard launches in your default browser, and each client app endpoint displays under the **Endpoints** column of the **Resources** page. The following image depicts the dashboard for this sample app:

:::image type="content" source="media/aspire-dashboard-with-nodejs.png" lightbox="media/aspire-dashboard-with-nodejs.png" alt-text=".NET Aspire dashboard with multiple JavaScript client apps.":::

The `weatherapi` service endpoint resolves to a Swagger UI page that documents the HTTP API. This service is consumed by each client app to display the weather forecast data. You can view each client app by navigating to the corresponding endpoint in the .NET Aspire dashboard. Their screenshots and the modifications made from the template starting point are detailed in the following sections.

In the same terminal session that you used to run the app, press <kbd>Ctrl</kbd> + <kbd>C</kbd> to stop the app.

## Explore the app host

To help understand how each client app resource is orchestrated, look to the app host project. The app host code declares the client app resources using the `AddNpmApp` API.

:::code source="~/aspire-samples/samples/AspireWithJavaScript/AspireJavaScript.AppHost/Program.cs":::

The preceding code:

- Creates a <xref:Aspire.Hosting.DistributedApplicationBuilder>.
- Adds the "weatherapi" service as a project to the app host.
- With a reference to the "weatherapi" service, adds the "angular", "react", and "vue" client apps as npm apps.
  - Each client app is configured to run on a different container port, and uses the `PORT` environment variable to determine the port.
  - All client apps also rely on a _Dockerfile_ to build their container image and are configured to express themselves in the publishing manifest as a container from the <xref:Aspire.Hosting.ExecutableResourceBuilderExtensions.AsDockerfileInManifest%2A>.

For more information on inner-loop networking, see [.NET Aspire inner-loop networking overview](../fundamentals/networking-overview.md). For more information on deploying apps, see [.NET Aspire manifest format for deployment tool builders](../deployment/manifest-format.md).

## Explore the Angular client

There are several key modifications from the original Angular template. The first is the addition of a _proxy.conf.js_ file. This file is used to proxy requests from the Angular client to the "weatherapi" service.

:::code language="javascript" source="~/aspire-samples/samples/AspireWithJavaScript/AspireJavaScript.Angular/proxy.conf.js":::

The preceding configuration proxies HTTP requests that start with `/api` to target the URL within the `services__weatherapi__1` environment variable. This environment variable is set by the .NET Aspire app host and is used to resolve the "weatherapi" service endpoint.

The second update is the to the _package.json_ file. This file is used to configure the Angular client to run on a different port than the default port. This is achieved by using the `PORT` environment variable, and the `run-script-os` npm package to set the port.

:::code language="json" source="~/aspire-samples/samples/AspireWithJavaScript/AspireJavaScript.Angular/package.json":::

The `scripts` section of the _package.json_ file is used to define the `start` script. This script is used by the `npm start` command to start the Angular client app. The `start` script is configured to use the `run-script-os` package to set the port, which delegates to the `ng serve` command passing the appropriate `--port` switch based on the OS-appropriate syntax.

In order to make HTTP calls to the "weatherapi" service, the Angular client app needs to be configured to provide the Angular `HttpClient` for dependency injection. This is achieved by using the `provideHttpClient` helper function while configuring the application in the _app.config.ts_ file.

:::code language="typescript" source="~/aspire-samples/samples/AspireWithJavaScript/AspireJavaScript.Angular/src/app/app.config.ts":::

Finally, the Angular client app needs to call the `/api/WeatherForecast` endpoint to retrieve the weather forecast data. There are several HTML, CSS, and TypeScript updates, all of which are made to the following files:

- _app.component.css_: [Update the CSS to style the table.](https://github.com/dotnet/aspire-samples/blob/ef6868b0999c6eea3d42a10f2b20433c5ea93720/samples/AspireWithJavaScript/AspireJavaScript.Angular/src/app/app.component.css)
- _app.component.html_: [Update the HTML to display the weather forecast data in a table.](https://github.com/dotnet/aspire-samples/blob/ef6868b0999c6eea3d42a10f2b20433c5ea93720/samples/AspireWithJavaScript/AspireJavaScript.Angular/src/app/app.component.html)
- _app.component.ts_: [Update the TypeScript to call the `/api/WeatherForecast` endpoint and display the data in the table.](https://github.com/dotnet/aspire-samples/blob/ef6868b0999c6eea3d42a10f2b20433c5ea93720/samples/AspireWithJavaScript/AspireJavaScript.Angular/src/app/app.component.ts)

:::code language="typescript" source="~/aspire-samples/samples/AspireWithJavaScript/AspireJavaScript.Angular/src/app/app.component.ts":::

### Angular app running

To visualize the Angular client app, navigate to the "angular" endpoint in the .NET Aspire dashboard. The following image depicts the Angular client app:

:::image type="content" source="media/angular-app.png" lightbox="media/angular-app.png" alt-text="Angular client app with fake forecast weather data displayed as a table.":::

## Explore the React client

There are several key modifications from the original React template. The first is the addition of an _.env_ file. This file is used to set two React-specific environment variables:

- `BROWSER=none`: This environment variable is used to prevent the React client app from launching a browser window.
- `REACT_APP_WEATHERAPI_URL`: This environment variable is used to set the URL for the "weatherapi" service.

:::code language="ini" source="~/aspire-samples/samples/AspireWithJavaScript/AspireJavaScript.React/.env":::

The preceding configuration sets the `REACT_APP_WEATHERAPI_URL` environment variable from the `services__weatherapi__1` environment variable. This environment variable is set by the .NET Aspire app host, and is used to resolve the "weatherapi" service endpoint.

> [!IMPORTANT]
> For custom environment variables to be available in the React client app, they must be prefixed with `REACT_APP_`. For more information, see [Adding custom environment variables](https://create-react-app.dev/docs/adding-custom-environment-variables/).

In addition to the aforementioned environment variables, the React app automatically picks up the `PORT` environment variable and uses it to determine the port on which to run. With the environment variables configured, the React client app needs to call the `/api/WeatherForecast` endpoint to retrieve the weather forecast data. The next update is to the _index.js_ file. The file passes the `REACT_APP_WEATHERAPI_URL` environment variable to the `App` component as the `weatherApi` property.

:::code language="javascript" source="~/aspire-samples/samples/AspireWithJavaScript/AspireJavaScript.React/src/index.js":::

The final updates are to the following files:

- _App.css_: [Update the CSS to style the table.](https://github.com/dotnet/aspire-samples/blob/ef6868b0999c6eea3d42a10f2b20433c5ea93720/samples/AspireWithJavaScript/AspireJavaScript.React/src/App.css)
- _App.js_: [Update the JavaScript to call the `/api/WeatherForecast` endpoint and display the data in the table.](https://github.com/dotnet/aspire-samples/blob/ef6868b0999c6eea3d42a10f2b20433c5ea93720/samples/AspireWithJavaScript/AspireJavaScript.React/src/App.js)

### React app running

To visualize the React client app, navigate to the "react" endpoint in the .NET Aspire dashboard. The following image depicts the React client app:

:::image type="content" source="media/react-app.png" lightbox="media/react-app.png" alt-text="React client app with fake forecast weather data displayed as a table.":::

## Explore the Vue client

There are several key modifications from the original Vue template. The first is the addition of an _.env_ file. This file configures a `VITE_WEATHERAPI_URL` environment variable from the `services__weatherapi__1` environment variable. This environment variable is set by the .NET Aspire app host and is used to resolve the "weatherapi" service endpoint.

:::code language="ini" source="~/aspire-samples/samples/AspireWithJavaScript/AspireJavaScript.Vue/.env":::

> [!IMPORTANT]
> For custom environment variables to be available in the Vue client app running on Vite, they must be prefixed with `VITE_`. For more information, see [Environment Variables and Modes](https://vitejs.dev/guide/env-and-mode#env-files).

Environment type definitions are available in the _env.d.ts_ file. This file is referenced in the _tsconfig.app.json_ file, and is used to provide type information for the `process.env` object.

:::code language="typescript" source="~/aspire-samples/samples/AspireWithJavaScript/AspireJavaScript.Vue/env.d.ts":::

To set the server port, the Vue client app uses the `PORT` environment variable. This is achieved by updating the _vite.config.js_ file:

:::code language="typescript" source="~/aspire-samples/samples/AspireWithJavaScript/AspireJavaScript.Vue/vite.config.ts":::

The final update from the template is made to the _TheWelcome.vue_ file. This file calls the `/api/WeatherForecast` endpoint to retrieve the weather forecast data, and displays the data in a table. It includes [CSS, HTML, and TypeScript updates](https://github.com/dotnet/aspire-samples/blob/ef6868b0999c6eea3d42a10f2b20433c5ea93720/samples/AspireWithJavaScript/AspireJavaScript.Vue/src/components/TheWelcome.vue).

### Vue app running

To visualize the Vue client app, navigate to the "vue" endpoint in the .NET Aspire dashboard. The following image depicts the Vue client app:

:::image type="content" source="media/vue-app.png" lightbox="media/vue-app.png" alt-text="Vue client app with fake forecast weather data displayed as a table.":::

## Summary

While there are several considerations that are beyond the scope of this article, you learned how to build .NET Aspire apps that use Node.js and Node Package Manager (`npm`). You also learned how to use the <xref:Aspire.Hosting.NodeAppHostingExtension.AddNpmApp%2A> APIs to host Node.js apps and apps that execute from a _package.json_ file, respectively. Finally, you learned how to use the `npm` CLI to create Angular, React, and Vue client apps, and how to configure them to run on different ports.
