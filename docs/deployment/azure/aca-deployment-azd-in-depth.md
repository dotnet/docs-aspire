# Deploying .NET Aspire apps using AZD (in depth guide)

The Azure Developer CLI (AZD) has been extended to support deploying .NET Aspire
applications. Use this guide to walk through the process of creating and deploying
a .NET Aspire application to Azure Container Apps using the Azure Developer CLI.

## Creating .NET Aspire application from starter template

The first step is to create a new .NET Aspire application. In this example the
`dotnet new` command is being used, although you can create the project Visual
Studio as well.

```dotnetcli
dotnet new aspire-starter --use-redis-cache -o AspireAzdWalkthrough && cd AspireAzdWalkthrough
dotnet run --project AspireAzdWalkthrough.AppHost\AspireAzdWalkthrough.AppHost.csproj
```

The previous commands will create a new .NET Aspire application based on the `aspire-starter`
template which includes a dependency on Redis cache. It then runs the Aspire project which
verifies that everything is working correctly.

## Initialzing AZD

Before deploying a .NET Aspire application with AZD, the repository/path containing the app
needs to be initialized.

```azurecli
azd init
```

AZD will prompt on whether you want to use code in the current directory or select a template,
in this case select the "Use code in the current directory" option.

![Screenshot of AZD prompting whether to scan current directory or use a template](../media/azd-prompt-init-path.png)

After scanning AZD will prompt to confirm that it found the correct .NET project containing
the .NET Aspire applications AppHost code. After checking the path select the "Confirm and
continue initializing my app".

![Screenshot of AZD confirming the detected location of the .NET Aspire application](../media/azd-prompt-confirm-path.png)

Once the path to the AppHost is confirmed AZD will analyze the .NET Aspire app model defined
in the AppHost and prompt which of the projects referenced in the app model should be exposed
via a public endpoint. For the starter application template only the `webfrontend` should be
exposed on a public endpoint.

![Screenshot of AZD prompting which .NET projects should have public endpoints](../media/azd-prompt-select-endpoints.png)

The final step in initializing AZD to work with the .NET Aspire code base is to select an
environment name. The environment forms part of an Azure resource-group name when deploying
the .NET Aspire application. For now select the name **aspireazddev**.

![Screenshot of final AZD output after initialization](../media/azd-prompt-final.png)

After providing the environment name AZD will generate a number of files and place them
into the working directory. These files are:

- `azure.yaml`; this file tell AZD where to find the .NET Aspire AppHost project.
- `.azure\aspireazddev\.env`; this file contains environment specific overrides.

The `azure.yaml` file has the following contents:

```yml
# yaml-language-server: $schema=https://raw.githubusercontent.com/Azure/azure-dev/main/schemas/v1.0/azure.yaml.json

name: AspireAzdWalkthrough
services:  
  app:
    language: dotnet
    project: .\AspireAzdWalkthrough.AppHost\AspireAzdWalkthrough.AppHost.csproj
    host: containerapp
```

Because the `project` field is pointing to a .NET Aspire AppHost project, AZD will
activate its integration with .NET Aspire and derive the required infrastructure
needed to host this appication from the application model specified in the `Program.cs`
file of the .NET Aspire app.

## Initial deployment

In order to deploy the .NET Aspire application to Azure AZD will need to get authorization
to call the Azure resource management APIs.

```azurecli
azd auth login
```

The previous command will launch a browser to authenticate the command-line session. Once
authenticated use the following command to provision and deploy the application.

```dotnetcli
azd up
```

Before deploying the application AZD needs to know which subscription and location the
resources should be deployed. Once these options are selected the .NET Aspire application
will be deployed.



## Deploying application updates

```dotnetcli

```

## Deploying infrastructure updates

## Resource compatability

TODO:

## Working in teams

azd init vs. azd up vs azd env new
