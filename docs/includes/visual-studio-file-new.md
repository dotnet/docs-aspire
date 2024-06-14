Visual Studio provides .NET Aspire project templates that handle some initial setup configurations for you. Complete the following steps to create a project for this quickstart:

1. At the top of Visual Studio, navigate to **File** > **New** > **Project**.
1. In the dialog window, search for *Aspire* and select **.NET Aspire Starter Application**. Select **Next**.

    :::image type="content" loc-scope="visual-studio" source="../media/aspire-templates.png" lightbox="../media/aspire-templates.png" alt-text="A screenshot of the .NET Aspire Starter Application template.":::

1. On the **Configure your new project** screen:
    - Enter a **Project Name** of *AspireSample*.
    - Leave the rest of the values at their defaults and select **Next**.
1. On the **Additional information** screen:
    - Make sure **.NET 8.0 (Long Term Support)** is selected.
    - Ensure that **Use Redis for caching (requires a supported container runtime)** is checked and select **Create**.
    - Optionally, you can select **Create a tests project**. For more information, see [Testing .NET Aspire projects](../fundamentals/testing.md).

Visual Studio creates a new solution that is structured to use .NET Aspire.
