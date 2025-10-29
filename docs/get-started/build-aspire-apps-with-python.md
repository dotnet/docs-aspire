---
title: Orchestrate Python apps in Aspire
description: Learn how to integrate existing Python apps into an Aspire solution.
ms.date: 10/17/2025
ms.custom: sfi-image-nochange
---

# Orchestrate Python apps in Aspire

In this article, you learn how to add existing Python applications to an Aspire solution. This approach is ideal when you have an existing Python codebase and want to add Aspire orchestration and observability to it. The sample app in this article demonstrates launching a Python application with [Flask](https://flask.palletsprojects.com/en/stable/). The Python extension for Aspire requires the use of virtual environments.

> [!TIP]
> If you're starting a new project from scratch, consider using the Aspire Python template instead. See [Build an Aspire app with Python and JavaScript](build-aspire-python-app.md) for more information.

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

This tutorial also assumes that you have installed the Aspire CLI. For further instructions, see [Install Aspire CLI](../cli/install.md).

Additionally, you need to install [Python](https://www.python.org/downloads) on your machine. The sample app in this article was built with Python version 3.12.4 and pip version 24.1.2. To verify your Python and pip versions, run the following commands:

```console
python --version
```

```console
pip --version
```

To download Python (including `pip`), see the [Python download page](https://www.python.org/downloads).

## Create an Aspire project using the template

To get started launching a Python project in Aspire, use the starter template to first create an Aspire application host:

1. Use the following Aspire CLI command:

    ```Aspire
    aspire new
    ```

1. To select the **Starter template**, press <kbd>Enter</kbd>.
1. For the **project name** type **PythonSample** and then press <kbd>Enter</kbd>.
1. For the output path, press <kbd>Enter</kbd> to accept the default.
1. To choose the default template version, press <kbd>Enter</kbd>.
1. For **Use Redis Cache**, choose **No**.
1. For **Do you want to create a test project?**, choose **No**.

The Aspire CLI fetches the correct template and uses it to create a new Aspire solution in the **PythonSample** folder.

In the same terminal session, change directories into the newly created project:

```console
cd PythonSample
```

After the template is created, launch the AppHost with the following command to ensure that the AppHost and the [Aspire dashboard](../fundamentals/dashboard/overview.md) run successfully:

```Aspire
aspire run
```

The Aspire CLI runs the solution and displays some information about it. To access the Aspire dashboard, hold down <kbd>CTRL</kbd> and then select the **Dashboard** URL.

:::image source="media/aspire-run-access-dashboard.png" lightbox="media/aspire-run-access-dashboard.png" alt-text="Screenshot showing how to access the Aspire dashboard from the output of the Aspire CLI.":::

At this point the dashboard won't show any resources. Stop the AppHost by pressing <kbd>Ctrl + C</kbd> in the terminal.

## Prepare a Python app

From your previous terminal session where you created the Aspire solution, create a new directory to contain the Python source code.

```console
mkdir hello-python
```

Change directories into the newly created _hello-python_ directory:

```console
cd hello-python
```

### Initialize the Python virtual environment

To work with Python apps, they need to be within a virtual environment. To create a virtual environment, run the following command:

```console
python -m venv .venv
```

For more information on virtual environments, see the [Python: Install packages in a virtual environment using pip and venv](https://packaging.python.org/en/latest/guides/installing-using-pip-and-virtual-environments/).

To activate the virtual environment, enabling installation and usage of packages, run the following command:

### [Unix/macOS](#tab/bash)

```bash
source .venv/bin/activate
```

### [Windows](#tab/powershell)

```powershell
.venv\Scripts\Activate.ps1
```

---

Ensure that pip within the virtual environment is up-to-date by running the following command:

```console
python -m pip install --upgrade pip
```

## Install Python packages

Install the Flask package by creating a _requirements.txt_ file in the _hello-python_ directory and adding the following line:

```text
Flask==3.0.3
```

Then, install the Flask package by running the following command:

```console
python -m pip install -r requirements.txt
```

After Flask is installed, create a new file named _main.py_ in the _hello-python_ directory and add the following code:

```python
import os
import flask

app = flask.Flask(__name__)

@app.route('/', methods=['GET'])
def hello_world():
    return 'Hello, World!'

if __name__ == '__main__':
    port = int(os.environ.get('PORT', 8111))
    app.run(host='0.0.0.0', port=port)
```

The preceding code creates a simple Flask app that listens on port 8111 and returns the message `"Hello, World!"` when the root endpoint is accessed.

## Update the AppHost project

Install the Python hosting package by running the following command:

```Aspire
aspire add
```

Use the arrow keys to select the **python (Aspire.Hosting.Python)** hosting integration and then press <kbd>Enter</kbd>. Then press <kbd>Enter</kbd> again to select the default version. The Aspire CLI adds the Python hosting integration to the Aspire AppHost project.

After the package is installed, the project XML should have a new package reference similar to the following example:

:::code language="xml" source="snippets/PythonSample/PythonSample.AppHost/PythonSample.AppHost.csproj" highlight="15":::

Replace the _AppHost.cs_ code with the following snippet. This code adds the Python project to Aspire by calling the `AddPythonApp` API and specifying the project name, project path, and the entry point file:

:::code source="snippets/PythonSample/PythonSample.AppHost/AppHost.cs" highlight="6":::

> [!IMPORTANT]
> The preceding code suppresses the `ASPIREHOSTINGPYTHON001` diagnostic error. This error is generated because the `AddPythonApp` API is experimental and might change in future release. For more information, see [Compiler Error ASPIREHOSTINGPYTHON001](../diagnostics/aspirehostingpython001.md).

## Run the app

Now that you've added the Python hosting package, updated the _AppHost.cs_ file, and created a Python project, you can run the AppHost:

```Aspire
aspire run
```

Launch the dashboard by clicking the link in the console output. The dashboard should display the Python project as a resource.

:::image source="media/python-dashboard.png" lightbox="media/python-dashboard.png" alt-text="Aspire dashboard: Python sample app.":::

Select the **Endpoints** link to open the `hello-python` endpoint in a new browser tab. The browser should display the message "Hello, World!":

:::image source="media/python-hello-world.png" lightbox="media/python-hello-world.png" alt-text="Aspire dashboard: Python sample app endpoint.":::

Stop the AppHost by pressing <kbd>Ctrl</kbd> + <kbd>C</kbd> in the terminal.

## Add telemetry support

To add a bit of observability, add telemetry to help monitor the dependant Python app. In the Python project, add the following **OpenTelemetry** packages as a dependency in the _requirements.txt_ file:

:::code language="text" source="snippets/PythonSample/hello-python/requirements.txt" highlight="2-5":::

Next, reinstall the Python app requirements into the virtual environment by running the following command:

```console
python -m pip install -r requirements.txt
```

The preceding command installs the **OpenTelemetry** package and the **OTLP** exporter, in the virtual environment. Update the Python app to include the **OpenTelemetry** code, by replacing the existing _main.py_ code with the following:

:::code language="python" source="snippets/PythonSample/hello-python/main.py":::

Update the AppHost project's _launchSettings.json_ file to include the `ASPIRE_ALLOW_UNSECURED_TRANSPORT` environment variable under the `http` profile:

:::code language="json" source="snippets/PythonSample/PythonSample.AppHost/Properties/launchSettings.json" highlight="26":::

The `ASPIRE_ALLOW_UNSECURED_TRANSPORT` variable is required because when running locally the OpenTelemetry client in Python rejects the local development certificate. Launch the _AppHost_ again:

```dotnetcli
dotnet run --project ../PythonSample.AppHost/PythonSample.AppHost.csproj --launch-profile http
```

> [!IMPORTANT]
> The Aspire AppHost must be run using HTTP instead of HTTPS. The **OpenTelemetry** library requires HTTP when running in a local dev environment.

Once the AppHost is running, navigate to the dashboard and select the **Structured** logging tab. Notice that it now contains logging events.

:::image source="media/python-telemetry-in-dashboard.png" lightbox="media/python-telemetry-in-dashboard.png" alt-text="Aspire dashboard: Structured logging from Python process.":::

## Onboard existing Python applications

If you have an existing Python application that you want to add to Aspire, here are some key considerations and best practices:

### Common Python frameworks

Aspire works with various Python web frameworks. The most common ones include:

- **[Flask](https://flask.palletsprojects.com/en/stable/)**: A lightweight WSGI web application framework.
- **[FastAPI](https://fastapi.tiangolo.com/)**: A modern, fast web framework for building APIs with Python.
- **[Django](https://www.djangoproject.com/)**: A high-level web framework that encourages rapid development.

The integration process is similar across frameworks:

1. Ensure your application can read configuration from environment variables.
1. Add OpenTelemetry instrumentation for observability.
1. Use the `AddPythonApp` API in your AppHost to orchestrate the application.

### Migration tips

When migrating an existing Python application to Aspire:

1. **Virtual environments**: Ensure your application uses a virtual environment (`.venv` directory).
1. **Dependencies**: List all dependencies in a `requirements.txt` file.
1. **Configuration**: Use environment variables for configuration instead of hardcoded values.
1. **Port binding**: Make your application read the port from an environment variable (commonly `PORT`).
1. **Logging**: Configure logging to work with OpenTelemetry for better observability.

### Common pitfalls

Be aware of these common issues when onboarding Python apps:

- **Certificate issues**: When running locally, you might need to set `ASPIRE_ALLOW_UNSECURED_TRANSPORT=true` in your AppHost launch settings.
- **Path issues**: Ensure the path to your Python entry point file is correct relative to the AppHost project.
- **Virtual environment activation**: The `AddPythonApp` API handles virtual environment activation automatically—you don't need to activate it manually.
- **Python version compatibility**: Ensure your Python version is compatible with the packages you're using.

### Multiple Python services

If you have multiple Python services, you can add them all to the same AppHost:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var pythonApi = builder.AddPythonApp("python-api", "../python-api", "main.py")
    .WithHttpEndpoint(port: 8111);

var pythonWorker = builder.AddPythonApp("python-worker", "../python-worker", "worker.py");

builder.Build().Run();
```

## Summary

In this article, you learned how to integrate existing Python applications into an Aspire solution. You learned how to:

- Prepare a Python application with a virtual environment.
- Install the Aspire Python hosting package.
- Use the `AddPythonApp` API in the AppHost to orchestrate Python apps.
- Add OpenTelemetry for observability.
- Handle common pitfalls and best practices for migrating existing Python apps.

## See also

- [Build an Aspire app with Python and JavaScript](build-aspire-python-app.md)
- [GitHub: Aspire Samples—Python hosting integration](https://github.com/dotnet/aspire-samples/tree/main/samples/AspireWithPython)
- [GitHub: Aspire issue #11865 - Python Templates](https://github.com/dotnet/aspire/issues/11865)
