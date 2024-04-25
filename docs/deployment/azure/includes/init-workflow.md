## Initialize the template

1. Open a new terminal window and `cd` into the root of your .NET Aspire project.

1. Execute the `azd init` command to initialize your project with `azd`, which will inspect the local directory structure and determine the type of app.

    ```azdeveloper
    azd init
    ```

    For more information on the `azd init` command, see [azd init](/azure/developer/azure-developer-cli/reference#azd-init).

1. If this is the first time you've initialized the app, `azd` prompts you for the environment name:

    ```azdeveloper
    Initializing an app to run on Azure (azd init)

    ? Enter a new environment name: [? for help]
    ```

    Enter the desired environment name to continue. For more information on managing environments with `azd`, see [azd env](/azure/developer/azure-developer-cli/reference#azd-env).

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

1. `azd` presents each of the projects in the .NET Aspire solution and prompts you to identify which to deploy with HTTP ingress open publicly to all internet traffic. Select only the `webfrontend` (using the <kbd>↓</kbd> and <kbd>Space</kbd> keys), since you want the API to be private to the Azure Container Apps environment and *not* available publicly.

    ```Output
    ? Select an option Confirm and continue initializing my app
    By default, a service can only be reached from inside the Azure Container Apps environment it is running in. Selecting a service here will also allow it to be reached from the Internet.
    ? Select which services to expose to the Internet  [Use arrows to move, space to select, <right> to all, <left> to none, type to filter]
      [ ]  apiservice
    > [x]  webfrontend
    ```

1. Finally, specify the the environment name, which is used to name provisioned resources in Azure and managing different environments such as `dev` and `prod`.

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
- _.azure/aspireazddev/config.json_: Configuration file that informs `azd` which services should have a public endpoint in this environment.
