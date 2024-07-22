---
title: Orchestrate Python apps in .NET Aspire
description: Learn how to integrate Python apps into a .NET Aspire App Host project.
ms.date: 07/22/2024
---

# Orchestrate Python apps in .NET Aspire

In this article, you learn how to use Python apps in a .NET Aspire project. The sample app in this article demonstrates launching a Python application. The Python extension for .NET Aspire requires the use of virtual environments.

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

Additionally, you need to install [Python](https://www.python.org/downloads) on your machine. The sample app in this article was built with Python version 3.12.4 and pip version 24.1.2. To verify your Python and pip versions, run the following commands:

```python
python --version
```

```python
pip --version
```

To download Python (including `pip`), see the [Python download page](https://www.python.org/downloads).

## Create a .NET Aspire project using the template

To get started launching a Python project in .NET Aspire first use the starter template to create a .NET Aspire application host:

```dotnetcli
dotnet new aspire -o PythonSample
```

In the same terminal session, change directories into the newly created project:

```dotnetcli
cd PythonSample
```

Once the template has been created launch the app host with the following command to ensure that the app host and the dashboard launches successfully:

```dotnetcli
dotnet run --project PythonSample.AppHost\PythonSample.AppHost.csproj
```

Once the app host starts it should be possible to click on the dashboard link in the console output. At this point the dashboard will not show any resources. Stop the app host by pressing <kbd>Ctrl</kbd> + <kbd>C</kbd> in the terminal.

## Prepare a Python project

From your previous terminal session where you created the .NET Aspire solution, create a new directory to contain the Python source code.

```Console
mkdir hellopython
```

Change directories into the newly created _hellopython_ directory:

```Console
cd hellopython
```

To create a virtual environment, run the following command:

```python
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

Verify the location of the Python interpreter by running the following command:

### [Unix/macOS](#tab/bash)

```bash
which python
```

### [Windows](#tab/powershell)

```powershell
where python
```

---

Next, install the Flask package by running the following command:

```python
python -m pip install Flask
```

After Flask is installed, create a new file named _main.py_ in the _hellopython_ directory and add the following code:

```python
import os
from flask import Flask

app = Flask(__name__)

@app.route('/', methods=['GET'])
def hello_world():
    return 'Hello, World!'

if __name__ == '__main__':
    port = int(os.environ.get('PORT', 8111))
    app.run(host='0.0.0.0', port=port)
```

Install the Python hosting package by running the following command:

```dotnetcli
dotnet add ../PythonSample.AppHost/PythonSample.AppHost.csproj package Aspire.Hosting.Python --version 8.1.0
```

After the package is installed, the project XML should have a new package reference similar to the following:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsAspireHost>true</IsAspireHost>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Aspire.Hosting.AppHost" Version="8.1.0" />

    <!-- Add this reference to PythonSample.AppHost.csproj -->
    <PackageReference Include="Aspire.Hosting.Python" Version="8.1.0" />
  </ItemGroup>

</Project>
```

Update the app host _Program.cs_ file to include the Python project, by calling the `AddPythonProject` API and specifying the project name, project path, and the entry point file:

:::code source="snippets/PythonSample/PythonSample.AppHost/Program.cs":::

## Run the app

Now that you've added the Python hosting package, updated the app host _Program.cs_ file, and created a Python project, you can run the app host:

```dotnetcli
dotnet run --project PythonSample.AppHost/PythonSample.AppHost.csproj
```

Launch the .NET Aspire dashboard by clicking the link in the console output. The dashboard should display the Python project as a resource.

:::image source="media/python-dashboard.png" lightbox="media/python-dashboard.png" alt-text=".NET Aspire dashboard: Python sample app.":::

Select the **Endpoints** link to open the `hello-python` endpoint in a new browser tab. The browser should display the message "Hello, World!":

:::image source="media/python-hello-world.png" lightbox="media/python-hello-world.png" alt-text=".NET Aspire dashboard: Python sample app endpoint.":::

## Add telemetry support.

TODO: Add OTLP dependency.

```text
Flask==3.0.3
opentelemetry-distro[otlp]
```

TODO: Install dependencies again.

```dotnetcli
pip install -r requirements.txt
```

TODO: Update source

```python
import os
from flask import Flask
import logging

logging.basicConfig()
logging.getLogger().setLevel(logging.NOTSET)

app = Flask(__name__)

@app.route('/', methods=['GET'])
def hello_world():
    logging.getLogger(__name__).info("request received!")
    return 'Hello, World!'

if __name__ == '__main__':
    port = int(os.environ.get('PORT', 8111))
    app.run(host='0.0.0.0', port=port)
```

TODO: Update app launchSettings to use `ASPIRE_ALLOW_UNSECURED_TRANSPORT`:

```json
"http": {
  "commandName": "Project",
  "dotnetRunMessages": true,
  "launchBrowser": true,
  "applicationUrl": "http://localhost:15044",
  "environmentVariables": {
    "ASPNETCORE_ENVIRONMENT": "Development",
    "DOTNET_ENVIRONMENT": "Development",
    "DOTNET_DASHBOARD_OTLP_ENDPOINT_URL": "http://localhost:19080",
    "DOTNET_RESOURCE_SERVICE_ENDPOINT_URL": "http://localhost:20252",
    "ASPIRE_ALLOW_UNSECURED_TRANSPORT": "true"
  }
}
```
