---
title: Build an Aspire app with Python and JavaScript
description: Learn how to create a new Aspire application with a Python backend and JavaScript frontend using the Aspire Python template.
ms.date: 10/17/2025
ms.custom: sfi-image-nochange
ai-usage: ai-assisted
---

# Build an Aspire app with Python and JavaScript

In this article, you learn how to create a new Aspire application using the Aspire Python template. The template creates a solution with a Python backend (using FastAPI) and a JavaScript frontend (using React), orchestrated by Aspire. This approach is ideal for Python developers who want to build observable, production-ready applications with modern JavaScript frontends.

> [!NOTE]
> The Aspire Python template is currently under development. This documentation will be updated as the template becomes available. For now, you can follow the guidance in [Orchestrate Python apps in Aspire](build-aspire-apps-with-python.md) to manually add Python applications to an existing Aspire solution.

> [!TIP]
> If you already have an existing Python application and want to add Aspire to it, see [Orchestrate Python apps in Aspire](build-aspire-apps-with-python.md).

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

Additionally, you need to install [Python](https://www.python.org/downloads) on your machine. This article requires Python version 3.12 or later. To verify your Python and pip versions, run the following commands:

```console
python --version
```

```console
pip --version
```

To download Python (including `pip`), see the [Python download page](https://www.python.org/downloads).

You also need to install [Node.js](https://nodejs.org/en/download/package-manager) for the JavaScript frontend. To verify your Node.js and npm versions, run the following commands:

```console
node --version
```

```console
npm --version
```

To download Node.js (including `npm`), see the [Node.js download page](https://nodejs.org/en/download/package-manager).

## Create an Aspire project using the Python template

> [!IMPORTANT]
> The `aspire-python` template is currently being developed and might not be available yet. Once released, you'll be able to create a Python-based Aspire application using the following command. For updates on template availability, see [Aspire Python Templates (issue #11865)](https://github.com/dotnet/aspire/issues/11865).

To create a new Aspire application with a Python backend and JavaScript frontend, use the `aspire-python` template:

```dotnetcli
dotnet new aspire-python -o MyPythonApp
```

This command creates a new Aspire solution with the following structure:

- **MyPythonApp.AppHost**: An orchestrator project designed to connect and configure the different projects and services of your app.
- **MyPythonApp.ServiceDefaults**: A shared project to manage configurations that are reused across the projects in your solution related to resilience, service discovery, and telemetry.
- **MyPythonApp.Backend**: A Python backend using FastAPI framework.
- **MyPythonApp.Frontend**: A JavaScript frontend using React framework.

## Explore the project structure

Navigate to the newly created project directory:

```console
cd MyPythonApp
```

### Python backend

The Python backend is located in the _MyPythonApp.Backend_ directory. It uses the FastAPI framework to create RESTful APIs. The backend includes:

- A virtual environment for Python dependencies
- A _requirements.txt_ file listing required Python packages
- A _main.py_ file containing the FastAPI application code
- OpenTelemetry instrumentation for observability

### JavaScript frontend

The JavaScript frontend is located in the _MyPythonApp.Frontend_ directory. It uses React to create the user interface. The frontend includes:

- A _package.json_ file listing required npm packages
- Source code in the _src_ directory
- Configuration for connecting to the backend API

### AppHost orchestration

The AppHost project (_MyPythonApp.AppHost_) orchestrates both the Python backend and JavaScript frontend. It uses the `AddPythonApp` and `AddNpmApp` APIs to configure and run these applications together.

## Set up the Python environment

Navigate to the Python backend directory:

```console
cd MyPythonApp.Backend
```

Create and activate a Python virtual environment:

### [Unix/macOS](#tab/bash)

```bash
python -m venv .venv
source .venv/bin/activate
```

### [Windows](#tab/powershell)

```powershell
python -m venv .venv
.venv\Scripts\Activate.ps1
```

---

Install the Python dependencies:

```console
python -m pip install --upgrade pip
python -m pip install -r requirements.txt
```

## Set up the JavaScript environment

Navigate to the JavaScript frontend directory:

```console
cd ../MyPythonApp.Frontend
```

Install the npm dependencies:

```console
npm install
```

## Run the application

Navigate back to the solution root directory:

```console
cd ..
```

Run the AppHost project to start the application:

```dotnetcli
dotnet run --project MyPythonApp.AppHost/MyPythonApp.AppHost.csproj
```

The Aspire dashboard opens in your browser. You should see both the Python backend and JavaScript frontend listed as resources.

## Explore the Aspire dashboard

The Aspire dashboard provides a unified view of your application's resources, including:

- **Resources**: View all running services (Python backend and JavaScript frontend)
- **Logs**: See consolidated logs from all services
- **Traces**: Monitor distributed traces across your application
- **Metrics**: View performance metrics and telemetry data

Select the **Endpoints** link for each resource to access:

- The React frontend user interface
- The FastAPI backend API documentation (Swagger UI)

## Understand telemetry and observability

The Python backend and JavaScript frontend are both configured with OpenTelemetry for observability. This means:

- **Logs** from both applications are collected and displayed in the Aspire dashboard
- **Traces** show the flow of requests across services
- **Metrics** provide insights into application performance

The template includes OpenTelemetry packages configured to export telemetry data to the Aspire dashboard automatically.

## Next steps

Now that you have a working Aspire application with Python and JavaScript:

- Explore the FastAPI backend code and add new API endpoints
- Customize the React frontend to create your user interface
- Add additional services or resources to your AppHost
- Deploy your application to a cloud environment

## See also

- [Orchestrate Python apps in Aspire](build-aspire-apps-with-python.md)
- [Orchestrate Node.js apps in Aspire](build-aspire-apps-with-nodejs.md)
- [Aspire templates](../fundamentals/aspire-sdk-templates.md)
- [GitHub: Aspire issue #11865 - Python Templates](https://github.com/dotnet/aspire/issues/11865)
