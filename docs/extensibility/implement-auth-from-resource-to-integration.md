---
title: Implement auth from custom resource to integration
description: Learn how to implement authentication credentials from a custom resource to a custom integration.
ms.date: 08/12/2024
ms.topic: how-to
---

# Implement auth from custom resource to integration

This article is a continuation of two previous articles:

- [Create custom resource types for .NET Aspire](custom-resources.md)
- [Create custom .NET Aspire integration](custom-integration.md)

One of the primary benefits to .NET Aspire is how it simplifies the configurability of resources and consuming clients (or integrations). This article demonstrates how to authentication credentials from from a custom resource to a custom integration. The custom resource is a MailDev container that allows for either incoming or outgoing credentials. The custom integration is a MailKit client that sends emails.

## Prerequisites

Since this article continues from previous content, it's expected that you've already created the resulting solution as a starting point for this article. If you haven't already, complete the following articles:

- [Create custom resource types for .NET Aspire](custom-resources.md)
- [Create custom .NET Aspire integration](custom-integration.md)

The resulting solution from these previous articles contains the following projects:

- _MailDev.Hosting_: Contains the custom resource type for the MailDev container.
- _MailDevResource.AppHost_: The [app host](../fundamentals/app-host-overview.md) that uses the custom resource and defines it as a dependency for a Newsletter service.
- _MailDevResource.NewsletterService_: An ASP.NET Core Web API project that sends emails using the MailDev container.
- _MailDevResource.ServiceDefaults_: Contains the [default service configurations](../fundamentals/service-defaults.md) intended for sharing.
- _MailKit.Client_: Contains the custom integration that exposes the MailKit `SmptClient` through a factory.

## Update the MailDev resource

To flow authentication credentials from the MailDev resource to the MailKit integration, you need to update the MailDev resource to include the username and password parameters.

The MailDev container supports basic authentication for both incoming and outgoing SMTP. To configure the credentials for incoming, you need to set the `MAILDEV_INCOMING_USER` and `MAILDEV_INCOMING_PASS` environment variables. For more information, see [MailDev: Usage](https://maildev.github.io/maildev/#usage). Update the _MailDevResource.cs_ file in the `MailDev.Hosting` project, by replacing its contents with the following C# code:
  
:::code source="snippets/MailDevResourceWithCredentials/MailDev.Hosting/MailDevResource.cs" highlight="9-10":::

These updates add a `UsernameParameter` and `PasswordParameter` property. These properties are used to store the parameters for the MailDev username and password. The `ConnectionStringExpression` property is updated to include the username and password parameters in the connection string. Next, update the _MailDevResourceBuilderExtensions.cs_ file in the `MailDev.Hosting` project with the following C# code:

:::code source="snippets/MailDevResourceWithCredentials/MailDev.Hosting/MailDevResourceBuilderExtensions.cs" highlight="9-10,29-30,32-34,40-41,55-59":::

The preceding code updates the `AddMailDev` extension method to include the `userName` and `password` parameters. The `WithEnvironment` method is updated to include the `UserEnvVarName` and `PasswordEnvVarName` environment variables. These environment variables are used to set the MailDev username and password.

## Update the app host

Now that the resource is updated to include the username and password parameters, you need to update the app host to include these parameters. Update the _:::no-loc text="Program.cs":::_ file in the `MailDevResource.AppHost` project with the following C# code:

:::code source="snippets/MailDevResourceWithCredentials/MailDevResource.AppHost/Program.cs" highlight="3-4,6-9":::

The preceding code adds two parameters for the MailDev username and password. It assigns these parameters to the `MAILDEV_INCOMING_USER` and `MAILDEV_INCOMING_PASS` environment variables. The `AddMailDev` method has two chained calls to `WithEnvironment` which includes these environment variables. For more information on parameters, see [External parameters](../fundamentals/external-parameters.md).

Next, configure the secrets for these paremeters. Right-click on the `MailDevResource.AppHost` project and select `Manage User Secrets`. Add the following JSON to the `secrets.json` file:

```json
{
  "Parameters:maildev-username": "@admin",
  "Parameters:maildev-password": "t3st1ng"
}
```

> [!WARNING]
> These credentials are for demonstration purposes only and MailDev is intended for local development. These crednetials are fictitious and shouldn't be used in a production environment.

## Update the MailKit integration

It's good practice for integrations to expect connection strings to contain varions key/value pairs, and to parse these pairs into the appropriate properties. Update the _MailKitClientSettings.cs_ file in the `MailKit.Client` project with the following C# code:

:::code source="snippets/MailDevResourceWithCredentials/MailKit.Client/MailKitClientSettings.cs" highlight="21-28,95-100":::

The preceding settings class, now includes a `Credentials` property of type `NetworkCredential`. The `ParseConnectionString` method is updated to parse the `Username` and `Password` keys from the connection string. If the `Username` and `Password` keys are present, a `NetworkCredential` is created and assigned to the `Credentials` property.

With the settings class updated to understand and populate the credentials, update the factory to conditionally use the credentials if they're configured. Update the _MailKitClientFactory.cs_ file in the `MailKit.Client` project with the following C# code:

:::code source="snippets/MailDevResourceWithCredentials/MailKit.Client/MailKitClientFactory.cs" highlight="44-48":::

When the factory determines that credentials have been configured, it authenticates with the SMTP server after connecting before returning the `SmtpClient`.

## Run the sample

Now that you've updated both the resource and corresponding integration projects, as well as the app host, you're ready to run the sample app. To run the sample from your IDE, select <kbd>F5</kbd> or use `dotnet run` from the root directory of the solution to start the application—you should see the [.NET Aspire dashboard](../fundamentals/dashboard/overview.md). Navigate to the `maildev` container resource and view the details. You should see the username and password parameters in the resource details, under the **Environment Variables** section:

:::image type="content" source="media/maildev-details.png" lightbox="media/maildev-details.png" alt-text=".NET Aspire Dashboard: MailDev container resource details.":::

Likewise, you should see the connection string in the `newsletterservice` resource details, under the **Environment Variables** section:

:::image type="content" source="media/newsletter-details.png" lightbox="media/newsletter-details.png" alt-text=".NET Aspire Dashboard: Newsletter service resource details.":::

Validate that everything is working as expected.

## Summary

This article demonstrated how to flow authentication credentials from a custom resource to a custom integration. The custom resource is a MailDev container that allows for either incoming or outgoing credentials. The custom integration is a MailKit client that sends emails. By updating the resource to include the username and password parameters, and updating the integration to parse and use these parameters, you can flow authentication credentials from the resource to the integration.
