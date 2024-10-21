---
title: Java/Spring hosting
author: justinyoo
description: A .NET Aspire hosting integration for hosting Java/Spring applications using either the Java runtime or a container.
ms.date: 10/11/2024
---

# .NET Aspire Java/Spring hosting integration

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

[!INCLUDE [banner](includes/banner.md)]

In this article, you learn how to use the .NET Aspire Java/Spring hosting integration to host Java/Spring applications using either the Java runtime or a container.

## Prerequisites

This integration requires the [OpenTelemetry Agent for Java](https://opentelemetry.io/docs/zero-code/java/agent/) to be downloaded and placed in the `agents` directory in the root of the project. Depending on your preferred shell, use either of the following commands to download the agent:

# [Bash](#tab/bash)

```bash
# bash/zsh
mkdir -p ./agents
wget -P ./agents \
    https://github.com/open-telemetry/opentelemetry-java-instrumentation/releases/latest/download/opentelemetry-javaagent.jar
```

# [PowerShell](#tab/powershell)

```powershell
# PowerShell
New-item -type Directory -Path ./agents -Force
Invoke-WebRequest `
    -OutFile "./agents/opentelemetry-javaagent.jar" `
    -Uri "https://github.com/open-telemetry/opentelemetry-java-instrumentation/releases/latest/download/opentelemetry-javaagent.jar"
```

---

## Get started

To get started with the .NET Aspire Azure Static Web Apps emulator integration, install the [📦 Aspire.CommunityToolkit.Hosting.Java](https://dev.azure.com/dotnet/CommunityToolkit/_artifacts/feed/CommunityToolkit-MainLatest/NuGet/Aspire.CommunityToolkit.Hosting.Java) NuGet package in the AppHost project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.CommunityToolkit.Hosting.Java
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.CommunityToolkit.Hosting.Java"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example Usage

The following sections detail various example usage scenarios, from hosting a containerized Spring app to hosting an executable Spring app.

### [Container hosting](#tab/container-hosting)

In the _:::no-loc text="Program.cs":::_file of your app host project, call the `AddSpringApp` method to define the containerized Spring app. Use the `JavaAppContainerResourceOptions` to define the containerized Spring app.

```csharp
var containerapp = builder.AddSpringApp(
    "containerapp",
    new JavaAppContainerResourceOptions
    {
        ContainerImageName = "<repository>/<image>",
        OtelAgentPath = "<agent-path>"
    });
```

### [Executable hosting](#tab/executable-hosting)

In the _:::no-loc text="Program.cs":::_ file of your AppHost project, call the `AddSpringApp` method to define the executable Spring app. Use the `JavaAppExecutableResourceOptions` to define the executable Spring app.

```csharp
var executableapp = builder.AddSpringApp(
    "executableapp",
    new JavaAppExecutableResourceOptions
    {
        ApplicationName = "target/app.jar",
        OtelAgentPath = "../../../agents"
    });
```

---

## See also

- [Java developer resources](/java)
- [.NET Aspire Community Toolkit GitHub repo](https://github.com/CommunityToolkit/Aspire)
