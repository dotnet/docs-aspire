## Initialize the template

1. Open a new terminal window and `cd` into the  _AppHost_ project directory of your .NET Aspire solution.

1. Execute the `azd init` command to initialize your project with `azd`, which will inspect the local directory structure and determine the type of app.

    ```azdeveloper
    azd init
    ```

    For more information on the `azd init` command, see [azd init](/azure/developer/azure-developer-cli/reference#azd-init).

1. Select **Use code in the current directory** when `azd` prompts you with two app initialization options.

    ```Output
    ? How do you want to initialize your app?  [Use arrows to move, type to filter]
    > Use code in the current directory
      Select a template
    ```

1. After scanning the directory, `azd` prompts you to confirm that it found the correct .NET Aspire _AppHost_ project. Select the **Confirm and continue initializing my app** option.

    ```Output
    Detected services:
    
      .NET (Aspire)
      Detected in: D:\source\repos\AspireSample\AspireSample.AppHost\AspireSample.AppHost.csproj
    
    azd will generate the files necessary to host your app on Azure using Azure Container Apps.
    
    ? Select an option  [Use arrows to move, type to filter]
    > Confirm and continue initializing my app
      Cancel and exit
    ```

1. Enter an environment name, which is used to name provisioned resources in Azure and managing different environments such as `dev` and `prod`.

    ```Output
    Generating files to run your app on Azure:
    
      (✓) Done: Generating ./azure.yaml
      (✓) Done: Generating ./next-steps.md
    
    SUCCESS: Your app is ready for the cloud!
    You can provision and deploy your app to Azure by running the azd up command in this directory. For more information on configuring your app, see ./next-steps.md
    ```

`azd` generates a number of files and places them into the working directory. These files are:

- _azure.yaml_: Describes the services of the app, such as .NET Aspire AppHost project, and maps them to Azure resources.
- _.azure/config.json_: Configuration file that informs `azd` what the current active environment is.
- _.azure/aspireazddev/.env_: Contains environment specific overrides.
