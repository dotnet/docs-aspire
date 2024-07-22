---
title: Orchestrate Python apps in .NET Aspire
description: Learn how to integrate Python apps into a .NET Aspire App Host project.
ms.date: 07/22/2024
---

# Orchestrate Python apps in .NET Aspire

In this article, you learn how to use Python apps in a .NET Aspire project. The sample app in this article demonstrates launching a Python application. The Python extension for .NET Aspire requires the use of virtual environments.

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

Once the app host starts it should be possible to click on the dashboard link in the console output. At this stage the dashboard will not show any resources.

## Prepare a Python project

Create a new directory to contain Python source code.

```dotnetcli
mkdir hellopython
cd hellopython
```

TODO: Install virtual environment

```dotnetcli
python -m pip install venv
```

TODO: Setup virtual environment

```dotnetcli
python -m venv .venv
```

TODO: Activate virtual environment

```dotnetcli
.\.venv\Scripts\activate.ps1
```

TODO: Setup requirements

```text
Flask=3.0.3
```

```dotnetcli

```

TODO: Add Python code.

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

TODO: Add Python extension to app host.

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

TODO: Add Python project to app model.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddPythonProject("hellopython", "..\\hellopython", "main.py")
       .WithEndpoint(targetPort: 8111, scheme: "http", env: "PORT");

builder.Build().Run();
```

TODO: Launch project / screenshot.

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
