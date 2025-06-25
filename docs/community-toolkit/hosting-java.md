---
title: Java/Spring hosting
author: justinyoo
description: A .NET Aspire hosting integration for hosting Java/Spring applications using either the Java runtime or a container.
ms.date: 06/25/2025
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

## Platform-specific considerations (Linux and macOS certificate trust)

On Linux and macOS platforms, you might need to import the .NET Aspire OpenTelemetry certificate into the Java certificate store for telemetry to work properly. Without this step, your Java application will start successfully, but telemetry collection might fail with certificate errors.

To add the certificate to the Java truststore, you can use the following steps:

1. Export the .NET Aspire dashboard certificate (this varies by your setup)
1. Import it into the Java truststore using the `keytool` command:

```bash
keytool -import -trustcacerts -alias aspire-dashboard \
    -file aspire-dashboard.crt \
    -keystore $JAVA_HOME/lib/security/cacerts \
    -storepass changeit
```

> [!NOTE]
> The exact steps for obtaining and importing the certificate may vary depending on your development environment and .NET Aspire configuration.

## Get started

To get started with the .NET Aspire Java/Spring hosting integration, install the [📦 CommunityToolkit.Aspire.Hosting.Java](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.Java) NuGet package in the AppHost project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.Hosting.Java
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.Hosting.Java"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example Usage

The following sections detail various example usage scenarios, from hosting a containerized Spring app to hosting an executable Spring app.

### [Container hosting](#tab/container-hosting)

In the _:::no-loc text="Program.cs":::_ file of your app host project, call the `AddSpringApp` method to define the containerized Spring app. The `JavaAppContainerResourceOptions` allows you to specify the container image and OpenTelemetry agent configuration.

```csharp
var containerapp = builder.AddSpringApp(
    "containerapp",
    new JavaAppContainerResourceOptions
    {
        ContainerImageName = "your-registry/your-spring-app:latest",
        OtelAgentPath = "./agents"
    });
```

The `ContainerImageName` should point to your Spring Boot application's container image, and `OtelAgentPath` specifies the path within the container where the OpenTelemetry Java agent is located.

### [Executable hosting](#tab/executable-hosting)

In the _:::no-loc text="Program.cs":::_ file of your AppHost project, call the `AddSpringApp` method to define the executable Spring app. The `workingDirectory` parameter specifies the directory containing your Java application, and the `JavaAppExecutableResourceOptions` defines the executable Spring app configuration.

```csharp
var executableapp = builder.AddSpringApp(
    "executableapp",
    workingDirectory: "../path/to/java/app",
    new JavaAppExecutableResourceOptions
    {
        ApplicationName = "target/app.jar",
        OtelAgentPath = "../../../agents"
    });
```

The parameters are defined as follows:

- `workingDirectory`: **(Required)** The directory containing your Java/Spring application. This should be the root directory of your Java project.
- `ApplicationName`: The path to your JAR file relative to the working directory (typically `target/your-app.jar` for Maven projects).
- `OtelAgentPath`: The path to the OpenTelemetry Java agent JAR file relative to the working directory.

---

## See also

- [Java developer resources](/java)
- [.NET Aspire Community Toolkit GitHub repo](https://github.com/CommunityToolkit/Aspire)
