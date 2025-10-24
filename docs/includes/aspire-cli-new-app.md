The Aspire CLI can lead you through the choices necessary to create a new Aspire solution, based on one of the Aspire templates. Complete the following steps to create a project for this quickstart:

1. Open a command prompt and navigate to a folder where you want to work. The Aspire CLI will create a new subdirectory and places the entire solution in it.
1. To run the Aspire CLI, execute the following command:

    ```Aspire
    aspire new
    ```

1. The Aspire CLI presents a list of templates for you to choose from. Select `Starter template` and then press <kbd>Enter</kbd>.

    :::image type="content" source="media/aspire-cli-new-select-template.png" lightbox="media/aspire-cli-new-select-template.png" alt-text="A screenshot of the command prompt running the Aspire CLI.":::

1. Enter a name for the new solution, and then press <kbd>Enter</kbd>.
1. Enter a folder for the new solution. The default name is the solution name that you just entered.
1. Select the default version of Aspire, which is the latest version on NuGet. Alternatively you can select the latest daily or stable builds of Aspire.
1. Choose `Yes` to include a Redis Cache integration in the solution.
1. Choose `No` to exclude a test project from the solution. The Aspire CLI creates the new solution.
