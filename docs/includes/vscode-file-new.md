Visual Studio Code provides Aspire project templates that handle some initial setup configurations for you and there is an Aspire official extension that integrates Visual Studio Code more closely with Aspire. Complete the following steps to install the extension:

1. From a new instance of Visual Studio Code (without a folder open), select the **Extensions** tab or press <kbd>CTRL</kbd> + <kbd>SHIFT</kbd> + <kbd>X</kbd>.
1. In the **Search in Extensions in Marketplace** textbox, type **Aspire** and then select the official Aspire extension.
1. Select **Install**. Visual Studio Code installs the extension.

    :::image type="content" loc-scope="vs-code" source="media/vscode-install-aspire-extension.png" lightbox="media/vscode-install-aspire-extension.png" alt-text="A screenshot of the Visual Studio Code installing the Aspire official extension.":::

Now, you can use the extension to create a new Aspire solution. Complete the following steps to create a solution for this quickstart:

1. In Visual Studio Code, select **View** > **Command Palette** or press <kbd>CTRL</kbd> + <kbd>SHIFT</kbd> + <kbd>P</kbd>.
1. Type **Aspire**, and then select **Aspire: New Aspire project**. Visual Studio Code opens a new terminal and starts the Aspire CLI.

    :::image type="content" loc-scope="vs-code" source="media/vscode-create-starter-app.png" lightbox="media/vscode-create-starter-app.png" alt-text="A screenshot of the Aspire Starter App template.":::

1. When prompted for a project template, select **Starter template**.
1. Enter a name for the project, and then press <kbd>ENTER</kbd>.
1. Enter an output path for the new project, and then press <kbd>ENTER</kbd>.
1. Select a template version for the project. The default is the latest version on NuGet. Alternatively you can select the latest daily or stable builds of Aspire.
1. Choose `Yes` to include a Redis Cache integration in the solution.
1. Choose `No` to exclude a test project from the solution. The Aspire CLI creates the new solution.
