---
ms.topic: include
---

:::zone pivot="visual-studio"

To install the .NET Aspire templates in Visual Studio, you need to manually install them unless you're using Visual Studio 17.12 or later. For Visual Studio 17.9 to 17.11, follow these steps:

1. Open Visual Studio.
1. Go to **Tools** > **NuGet Package Manager** > **Package Manager Console**.
1. Run the following command to install the templates:

  ```dotnetcli
  dotnet new install Aspire.ProjectTemplates
  ```

For Visual Studio 17.12 or later, the .NET Aspire templates are installed automatically.

:::zone-end
:::zone pivot="vscode,dotnet-cli"

To install these templates, use the [dotnet new install](/dotnet/core/tools/dotnet-new-install) command, passing in the `Aspire.ProjectTemplates` NuGet identifier.

```dotnetcli
dotnet new install Aspire.ProjectTemplates
```

To install a specific version, append the version number to the package name:

```dotnetcli
dotnet new install Aspire.ProjectTemplates::9.3.0
```

> [!TIP]
> If you already have the .NET Aspire workload installed, you need to pass the `--force` flag to overwrite the existing templates. Feel free to uninstall the .NET Aspire workload.

:::zone-end
