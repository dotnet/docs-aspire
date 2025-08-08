---
title: Use the .NET Aspire dashboard with Node.js apps
description: How to use the Aspire Dashboard in a Node.js application.
ms.date: 07/17/2025
ms.topic: tutorial
---

# Tutorial: Use the .NET Aspire dashboard with Node.js apps

The [.NET Aspire dashboard](overview.md) provides a great user experience for viewing telemetry, and is available as a standalone container image that can be used with any OpenTelemetry-enabled app. In this article, you'll learn how to:

> [!div class="checklist"]
>
> - Start the .NET Aspire dashboard in standalone mode.
> - Use the .NET Aspire dashboard with a Node.js app.

## Prerequisites

To complete this tutorial, you need the following:

- [Docker](https://docs.docker.com/get-docker/) or [Podman](https://podman.io/).
  - You can use an alternative container runtime, but the commands in this article are for Docker.
- [Node.js 18 or higher](https://nodejs.org/en/download/package-manager) locally installed.
- A sample application.

## Sample application

This tutorial uses an Express.js application to demonstrate the integration. You can either create a new Express.js app or use an existing Node.js application.

To create a new Express.js application:

1. Create a new directory for your application:

    ```console
    mkdir aspire-nodejs-sample
    cd aspire-nodejs-sample
    ```

1. Initialize a new Node.js project:

    ```console
    npm init -y
    ```

1. Install Express.js:

    ```console
    npm install express
    ```

1. Create a basic Express.js application by creating an *app.js* file:

    ```javascript
    const express = require('express');
    const app = express();
    const port = process.env.PORT || 3000;

    app.get('/', (req, res) => {
        console.log('Received request for home page');
        res.json({
            message: 'Hello from Node.js!',
            timestamp: new Date().toISOString()
        });
    });

    app.get('/api/weather', (req, res) => {
        const weather = [
            { city: 'Seattle', temperature: 72, condition: 'Cloudy' },
            { city: 'Portland', temperature: 68, condition: 'Rainy' },
            { city: 'San Francisco', temperature: 65, condition: 'Foggy' },
            { city: 'Los Angeles', temperature: 78, condition: 'Sunny' }
        ];
        
        console.log('Received request for weather data');
        res.json(weather);
    });

    app.listen(port, () => {
        console.log(`Server running at http://localhost:${port}`);
    });
    ```

1. Replace the entire *package.json* file with the following content:

    :::code language="json" source="snippets/standalone-for-nodejs/package.json":::

1. Test the application by running:

    ```console
    npm start
    ```

1. Browse to the application at <http://localhost:3000> in a web browser to verify it's working.

## Adding OpenTelemetry

To use the .NET Aspire dashboard with your Node.js app, you need to install the OpenTelemetry SDK and exporter. The OpenTelemetry SDK provides the API for instrumenting your application, and the exporter sends telemetry data to the .NET Aspire dashboard.

1. Create a new file called *tracing.js* to configure OpenTelemetry:

    :::code language="javascript" source="snippets/standalone-for-nodejs/tracing.js":::

1. Update your *app.js* to import the tracing configuration at the very beginning:

    ```javascript
    // This must be imported first!
    require('./tracing');

    const express = require('express');
    const app = express();
    const port = process.env.PORT || 3000;

    app.get('/', (req, res) => {
        console.log('Received request for home page');
        res.json({
            message: 'Hello from Node.js!',
            timestamp: new Date().toISOString()
        });
    });

    app.get('/api/weather', (req, res) => {
        const weather = [
            { city: 'Seattle', temperature: 72, condition: 'Cloudy' },
            { city: 'Portland', temperature: 68, condition: 'Rainy' },
            { city: 'San Francisco', temperature: 65, condition: 'Foggy' },
            { city: 'Los Angeles', temperature: 78, condition: 'Sunny' }
        ];
        
        console.log('Received request for weather data');
        res.json(weather);
    });

    app.listen(port, () => {
        console.log(`Server running at http://localhost:${port}`);
    });
    ```

1. Restart your application:

    ```console
    npm start
    ```

## Start the Aspire dashboard

To start the Aspire dashboard in standalone mode, run the following Docker command:

### [Bash](#tab/bash)

```bash
docker run --rm -it -p 18888:18888 -p 4317:18889 --name aspire-dashboard \
    mcr.microsoft.com/dotnet/aspire-dashboard:9.0
```

### [PowerShell](#tab/powershell)

```powershell
docker run --rm -it -p 18888:18888 -p 4317:18889 --name aspire-dashboard `
    mcr.microsoft.com/dotnet/aspire-dashboard:9.0
```

---

In the Docker logs, the endpoint and key for the dashboard are displayed. Copy the key and navigate to `http://localhost:18888` in a web browser. Enter the key to log in to the dashboard.

## View telemetry in the dashboard

After starting both the dashboard and your Node.js application, you can view telemetry data by making requests to your application and observing the results in the dashboard.

1. Make some requests to your Node.js application:

    ```console
    curl http://localhost:3000/
    curl http://localhost:3000/api/weather
    ```

1. Navigate to the .NET Aspire dashboard at <http://localhost:18888> and explore the different sections:

### Traces

The **Traces** page shows distributed traces for HTTP requests. Each request to your Express.js application creates a trace that you can explore to see the request flow and timing information.

### Metrics

The **Metrics** page displays various metrics collected from your Node.js application, including HTTP request metrics, Node.js runtime metrics, and custom metrics if you choose to add them.

## Adding custom telemetry

You can enhance your application with custom spans, logs, and metrics:

1. Update your *app.js* to include custom telemetry:

    :::code language="javascript" source="snippets/standalone-for-nodejs/app.js":::

## Next steps

You have successfully used the .NET Aspire dashboard with a Node.js application. To learn more about the .NET Aspire dashboard, see the [Aspire dashboard overview](overview.md) and how to orchestrate a Node.js application with the [.NET Aspire AppHost](../../get-started/build-aspire-apps-with-nodejs.md).

To learn more about OpenTelemetry instrumentation for Node.js applications, see the [OpenTelemetry JavaScript documentation](https://opentelemetry.io/docs/languages/js/).
