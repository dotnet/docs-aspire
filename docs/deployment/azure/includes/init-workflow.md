## Initialize the template

1. Open a new terminal window and `cd` into the root of the .NET Aspire project you created.

1. Execute the `azd init` command to initialize your project with `azd`, which will inspect the local directory structure and determine the type of app.

    ```azdeveloper
    azd init
    ```

1. After scanning, `azd` prompts you to confirm that it found the correct .NET project containing the .NET Aspire app's _AppHost_ code. Select the **Confirm and continue initializing my app** option.

    :::image type="content" source="media/azd-prompt-confirm-path.png" alt-text="Screenshot of `azd` confirming the detected location of the .NET Aspire application.":::

1. `azd` presents each of the projects in the .NET Aspire solution and allows you to identify which project(s) will be deployed with HTTP ingress open publicly to all internet traffic. Select only the `webfrontend`, since you want the API to be private to the Azure Container Apps environment and *not* available publicly.

    :::image type="content" source="media/azd-prompt-select-endpoints.png" alt-text="Screenshot of `azd` prompting which .NET projects should have public endpoints.":::

1. Finally, specify the the environment name, which is used for naming provisioned resources in Azure and managing different environments such as `dev` and `prod`.

    :::image type="content" source="media/azd-prompt-final.png" lightbox="media/azd-prompt-final.png" alt-text="A screenshot of the final `azd` output after initialization.":::

`azd` generates a number of files and places them into the working directory. These files are:

- _azure.yaml_: Describes the services of the app, such as .NET Aspire AppHost project, and maps them to Azure resources.
- _.azure/config.json_: Configuration file that informs `azd` what the current active environment is.
- _.azure/aspireazddev/.env_: Contains environment specific overrides.
- _.azure/aspireazddev/config.json_: Configuration file that informs `azd` which services should have a public endpoint in this environment.
