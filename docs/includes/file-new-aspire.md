---
zone_pivot_groups: dev-environment
---

## Create the .NET Aspire template

To create a new .NET Aspire Starter Application, you can use either Visual Studio, Visual Studio Code, or the .NET CLI.

:::zone pivot="visual-studio"

Visual Studio provides .NET Aspire project templates that handle some initial setup configurations for you. Complete the following steps to create a project for this quickstart:

1. At the top of Visual Studio, navigate to **File** > **New** > **Project**.
1. In the dialog window, search for *Aspire* and select **.NET Aspire Starter Application**. Select **Next**.

    :::image type="content" source="../media/aspire-templates.png" lightbox="../media/aspire-templates.png" alt-text="A screenshot of the .NET Aspire Starter Application template.":::

1. On the **Configure your new project** screen:
    - Enter a **Project Name** of *AspireSample*.
    - Leave the rest of the values at their defaults and select **Next**.
1. On the **Additional information** screen:
    - Make sure **.NET 8.0 (Long Term Support)** is selected.
    - Ensure that **Use Redis for caching (requires a supported container runtime)** is checked and select **Create**.
    - Optionally, you can select **Create a tests project**. For more information, see [Testing .NET Aspire apps](../fundamentals/testing.md).

Visual Studio creates a new solution that is structured to use .NET Aspire.

:::zone-end
:::zone pivot="vscode"

Visual Studio Code provides .NET Aspire project templates that handle some initial setup configurations for you. Complete the following steps to create a project for this quickstart:

1. From a new instance of Visual Studio Code (without a folder open), select **Create .NET project** button.
1. Select the **.NET Aspire Starter Application** template.

    :::image type="content" source="media/vscode-new-starter-project.png lightbox="media/vscode-new-starter-project.png" alt-text="A screenshot of the .NET Aspire Starter Application template.":::

:::zone-end
:::zone pivot="dotnet-cli"

```dotnetcli
dotnet new aspire-starter --use-redis-cache --output AspireSample
```

For more information, see [dotnet new](/dotnet/core/tools/dotnet-new). The .NET CLI creates a new solution that is structured to use .NET Aspire.

:::zone-end
