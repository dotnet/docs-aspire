---
title: Interaction Service (Preview)
description: Use the interaction service API to prompt users for input, request confirmation, and display messages in the Aspire dashboard or CLI during publish and deploy.
ms.date: 07/16/2025
---

# Interaction Service (Preview)

The interaction service (`Aspire.Hosting.IInteractionService`) allows you to prompt users for input, request confirmation, and display messages. The interaction service works in two different contexts:

- **Aspire dashboard**: When running `aspire run` or launching the app host directly, interactions appear as dialogs and notifications in the dashboard UI.
- **Aspire CLI**: When running `aspire publish` or `aspire deploy`, interactions are prompted through the command-line interface.

This is useful for scenarios where you need to gather information from the user or provide feedback on the status of operations, regardless of how the application is being launched or deployed.

## The `IInteractionService` API

The `IInteractionService` interface can be retrieved from the dependency injection container of your <xref:Aspire.Hosting.DistributedApplication>. When you request this service, be sure to check if it's available for usage. If you attempt to use the interaction service when it's not available (`IInteractionService.IsAvailable` returns `false`), an exception is thrown.

The interaction service has several methods that you use to interact with users or display messages. The behavior of these methods depends on the execution context:

- **Dashboard context** (`aspire run` or direct app host launch): Interactions appear as modal dialogs, notifications, and form inputs in the Aspire dashboard web interface.
- **CLI context** (`aspire publish` or `aspire deploy`): Interactions are prompted through the command-line interface with text-based prompts and responses.

The following sections describe how to use these APIs effectively in both contexts:

- `PromptMessageBoxAsync`: Displays a modal dialog box with a message and buttons for user interaction (dashboard only).
- `PromptNotificationAsync`: Displays a nonmodal notification at the top of the dashboard (dashboard only).
- `PromptConfirmationAsync`: Displays a confirmation dialog with options for the user to confirm or cancel an action (dashboard only).
- `PromptInputAsync`: Prompts the user for a single input value, such as a text or secret (both contexts).
- `PromptInputsAsync`: Prompts the user for multiple input values in a single dialog (dashboard) or sequentially (CLI), allowing for more complex configurations (both contexts).

> [!IMPORTANT]
> During `aspire publish` and `aspire deploy` operations, only `PromptInputAsync` and `PromptInputsAsync` are available. Other interaction methods (`PromptMessageBoxAsync`, `PromptNotificationAsync`, and `PromptConfirmationAsync`) will throw an exception if called in CLI contexts.

## Where to use the interaction service

Any of the available callback-based extension methods of `IResourceBuilder<T>` can use the interaction service to prompt users for input or confirmation. Use the interaction service in these scenarios:

- **Custom resource types**: Gather input from users or confirm actions when you create custom resource types.

    Resource types are free to define dashboard interactions, such as prompting for user input or displaying messages. The interaction service allows you to create a more interactive experience for users when they manage resources in the Aspire dashboard or CLI. For more information, see [Create custom .NET Aspire hosting integrations](custom-hosting-integration.md).

- **Custom resource commands**: Add commands to resources in the Aspire dashboard. Use the interaction service to prompt users for input or confirmation when these commands run.

    When you chain a call to <xref:Aspire.Hosting.ResourceBuilderExtensions.WithCommand*> on a target `IResourceBuilder<T>`, for example, your callback can use the interaction service to gather input or confirm actions. For more information, see [Custom resource commands in .NET Aspire](../fundamentals/custom-resource-commands.md).

- **Publish and deploy workflows**: During `aspire publish` or `aspire deploy` operations, use the interaction service to gather deployment-specific configuration, confirm destructive operations, or provide status updates through the CLI.

These approaches help you create interactive, user-friendly experiences for local development, dashboard interactions, and deployment workflows.

> [!IMPORTANT]
> This article demonstrates the interaction service in the context of a `WithCommand` callback with a `FakeResource` type, but the same principles apply to other extension methods that support user interactions.
>
> For example:
>
> ```csharp
> var builder = DistributedApplication.CreateBuilder(args);
> 
> builder.AddFakeResource("fake-resource")
>        .WithCommand("msg-dialog", "Example Message Dialog", async context =>
> {
>     var interactionService = context.GetRequiredService<IInteractionService>();
>     
>     // Use interaction service...
>
>     return CommandResults.Success();
> });
> ```
>
> For CLI specific contexts, the interaction service is retrieved from either the `PublishingContext` or `DeploymentContext` depending on the operation being performed.

## Display messages

There are many ways to display messages to the user. The presentation differs between dashboard and CLI contexts:

- **Dialog messages**: Show important information in a dialog box (dashboard only).
- **Notification messages**: Display less critical information in a notification (dashboard only).

> [!NOTE]
> Message display methods (`PromptMessageBoxAsync` and `PromptNotificationAsync`) are only available in dashboard contexts. These methods throw an exception if called during `aspire publish` or `aspire deploy` operations.

### Display a dialog message box

Dialog messages provide important information that requires user attention. These are only available in the dashboard context and will throw an exception if called during CLI operations like `aspire publish` or `aspire deploy`.

<!-- <xref:Aspire.Hosting.IInteractionService.PromptMessageBoxAsync%2A> -->

The `IInteractionService.PromptMessageBoxAsync` method displays a message with customizable response options in the dashboard context.

:::code source="snippets/InteractionService/AppHost.MessageBoxExample.cs" id="example":::

**Dashboard view:**

:::image type="content" source="media/interaction-service-message-dialog.png" lightbox="media/interaction-service-message-dialog.png" alt-text="Aspire dashboard interface showing a message dialog with a title, message, and buttons.":::

**CLI view:**

This method isn't available in CLI contexts. If you call it during `aspire publish` or `aspire deploy`, it throws an exception.

### Display a notification message

Notification messages provide nonmodal notifications. These are only available in the dashboard context and will throw an exception if called during CLI operations like `aspire publish` or `aspire deploy`.

> [!TIP]
> In the dashboard, notification messages stack, so you can display multiple messages at once.

<!-- <xref:Aspire.Hosting.IInteractionService.PromptNotificationAsync%2A> -->

The `IInteractionService.PromptNotificationAsync` method displays informational messages with optional action links in the dashboard context:

:::code source="snippets/InteractionService/AppHost.NotificationExample.cs" id="example":::

The previous example demonstrates several ways to use the notification API. Each approach displays different types of notifications.

**Dashboard view:**

:::image type="content" source="media/interaction-service-message-bar.png" lightbox="media/interaction-service-message-bar.png" alt-text="Aspire dashboard interface showing multiple notification messages stacked near the top of the page. There are five notifications displayed, each with a different type of notification.":::

**CLI view:**

This method isn't available in CLI contexts. If you call it during `aspire publish` or `aspire deploy`, it throws an exception.

## Prompt for user confirmation

<!-- <xref:Aspire.Hosting.IInteractionService.PromptConfirmationAsync%2A> -->

Use the interaction service when you need the user to confirm an action before proceeding. The `IInteractionService.PromptConfirmationAsync` method displays a confirmation prompt in the dashboard context. This method is only available in dashboard contexts and will throw an exception if called during CLI operations like `aspire publish` or `aspire deploy`.

Confirmation prompts are essential for destructive operations or actions that have significant consequences. They help prevent accidental actions and give users a chance to reconsider their decisions.

### Prompt for confirmation before destructive operations

For operations that can't be undone, such as deleting resources, always prompt for confirmation:

:::code source="snippets/InteractionService/AppHost.ConfirmationExample.cs" id="example":::

**Dashboard view:**

:::image type="content" source="media/interaction-service-confirmation.png" lightbox="media/interaction-service-confirmation.png" alt-text="Aspire dashboard interface showing a confirmation dialog with a title, message, and buttons for confirming or canceling the operation.":::

**CLI view:**

This method isn't available in CLI contexts. If you call it during `aspire publish` or `aspire deploy`, it throws an exception.

## Prompt for user input

The interaction service API allows you to prompt users for input in various ways. You can collect single values or multiple values, with support for different input types including text, secrets, choices, booleans, and numbers. The presentation adapts automatically to the execution context.

### Prompt user for a single input

<!-- <xref:Aspire.Hosting.IInteractionService.PromptInputAsync%2A> -->

Use the `IInteractionService.PromptInputAsync` method to collect a single piece of information from the user. This method supports various input types through the `InteractionInput` class. Consider the following example, that prompts the user for an API key as a secret input value:

:::code source="snippets/InteractionService/AppHost.SingleInputExample.cs" id="example":::

**Dashboard view:**

When the Aspire dashboard runs and the user selects the corresponding command, the interaction service displays a dialog like the following screenshot:

:::image type="content" source="media/interaction-service-single-input.png" lightbox="media/interaction-service-single-input.png" alt-text="Aspire dashboard interface showing a single input dialog with a label, input field, and buttons for confirming or canceling the input.":::

If the user doesn't enter a value, the dialog shows an error message indicating that the input is required:

:::image type="content" source="media/interaction-service-single-input-validation.png" lightbox="media/interaction-service-single-input-validation.png" alt-text="Aspire dashboard interface showing a single input dialog with an error message indicating that the input is required.":::

**CLI view:**

In the CLI context, the same interaction appears as a text prompt:

```Aspire
aspire deploy

Step 1: Analyzing model.

       ✓ DONE: Analyzing the distributed application model for publishing and deployment capabilities. 00:00:00
           Found 1 resources that support deployment. (FakeResource)

✅ COMPLETED: Analyzing model. completed successfully

═══════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════

Enter your third-party service API key: *************
✓ PUBLISHING COMPLETED: Publishing completed successfully
```

### Prompt the user for multiple input values

<!-- <xref:Aspire.Hosting.IInteractionService.PromptInputsAsync%2A> -->

Use the `IInteractionService.PromptInputsAsync` method to collect multiple pieces of information. In the dashboard, this presents all inputs in a single dialog. In the CLI, inputs are requested sequentially.

Consider the following example, which prompts the user for multiple input values:

:::code source="snippets/InteractionService/AppHost.MultipleInputExample.cs" id="example":::

**Dashboard view:**

This renders on the dashboard as shown in the following image:

:::image type="content" source="media/interaction-service-multiple-input.png" lightbox="media/interaction-service-multiple-input.png" alt-text="Aspire dashboard interface showing a multiple input dialog with labels, input fields, and buttons for confirming or canceling the input.":::

Imagine you fill out the dialog with the following values:

:::image type="content" source="media/interaction-service-multiple-input-filled.png" lightbox="media/interaction-service-multiple-input-filled.png" alt-text="Aspire dashboard interface showing a multiple input dialog with filled input fields and buttons for confirming or canceling the input.":::

After you select the **Ok** button, the resource logs display the following output:

:::image type="content" source="media/interaction-service-multiple-input-logs.png" lightbox="media/interaction-service-multiple-input-logs.png" alt-text="Aspire dashboard interface showing logs with the input values entered in the multiple input dialog.":::

**CLI view:**

In the CLI context, the same inputs are requested sequentially:

```Aspire
aspire deploy

Step 1: Analyzing model.

       ✓ DONE: Analyzing the distributed application model for publishing and deployment capabilities. 00:00:00
           Found 1 resources that support deployment. (FakeResource)

✅ COMPLETED: Analyzing model. completed successfully

═══════════════════════════════════════════════════════════════════════════════

Configure your application deployment settings:
Application Name: example-app
Environment:  Staging
Instance Count: 3
Enable Monitoring: [y/n] (n): y
✓ DEPLOY COMPLETED: Deploying completed successfully
```

Depending on the input type, the CLI might display additional prompts. For example, the `Enable Monitoring` input is a boolean choice, so the CLI prompts for a yes/no response. If you enter `y`, it enables monitoring; if you enter `n`, it disables monitoring. For the environment input, the CLI displays a list of available environments for selection:

```Aspire
Configure your application deployment settings:
Application Name: example-app
Environment:

> Development
  Staging
  Testing

(Type to search)
```

#### Input validation

For complex scenarios, you can provide custom validation logic using the `InputsDialogInteractionOptions.ValidationCallback` property:

```csharp
// Multiple inputs with custom validation
var databaseInputs = new List<InteractionInput>
{
    new()
    {
        Label = "Database Name",
        InputType = InputType.Text,
        Required = true,
        Placeholder = "myapp-db"
    },
    new()
    {
        Label = "Username",
        InputType = InputType.Text,
        Required = true,
        Placeholder = "admin"
    },
    new()
    {
        Label = "Password",
        InputType = InputType.SecretText,
        Required = true,
        Placeholder = "Enter a strong password"
    },
    new()
    {
        Label = "Confirm Password",
        InputType = InputType.SecretText,
        Required = true,
        Placeholder = "Confirm your password"
    }
};

var validationOptions = new InputsDialogInteractionOptions
{
    ValidationCallback = async context =>
    {
        var passwordInput = context.Inputs.FirstOrDefault(i => i.Label == "Password");
        var confirmPasswordInput = context.Inputs.FirstOrDefault(i => i.Label == "Confirm Password");

        // Validate password strength
        if (passwordInput?.Value is { Length: < 8 })
        {
            context.AddValidationError(passwordInput, "Password must be at least 8 characters long");
        }

        // Validate password confirmation
        if (passwordInput?.Value != confirmPasswordInput?.Value)
        {
            context.AddValidationError(confirmPasswordInput!, "Passwords do not match");
        }

        await Task.CompletedTask;
    }
};

var dbResult = await interactionService.PromptInputsAsync(
    title: "Database Configuration",
    message: "Configure your PostgreSQL database connection:",
    inputs: databaseInputs,
    options: validationOptions);

if (!dbResult.Canceled && dbResult.Data != null)
{
    // Use the validated configuration
}
```

### Best practices for user input

When prompting for user input, consider these best practices:

1. **Group related inputs**: Use multiple input prompts to collect related configuration values. In the dashboard, these appear in a single dialog; in the CLI, they're requested sequentially but grouped conceptually.
1. **Provide clear labels and placeholders**: Help users understand what information is expected, regardless of context.
1. **Use appropriate input types**: Choose the right input type for the data you're collecting (secret for passwords, choice for predefined options, etc.). Both contexts support these input types appropriately.
1. **Implement validation**: Validate user input and provide clear error messages when validation fails. Both contexts support validation feedback.
1. **Make required fields clear**: Mark required fields and provide appropriate defaults for optional ones.
1. **Handle cancellation**: Always check if the user canceled the input prompt and handle it gracefully. Users can cancel in both dashboard and CLI contexts.

## Interaction contexts

The interaction service behaves differently depending on how your .NET Aspire application is launched:

### Dashboard context

When you run your application using `aspire run` or by directly launching the app host project, interactions appear in the Aspire dashboard web interface:

- **Modal dialogs**: Message boxes and input prompts appear as overlay dialogs that require user interaction.
- **Notification messages**: Informational messages appear as dismissible banners at the top of the dashboard.
- **Rich UI**: Full support for interactive form elements, validation, and visual feedback.
- **All methods available**: All interaction service methods are supported in dashboard contexts.

### CLI context

When you run `aspire publish` or `aspire deploy`, interactions are prompted through the command-line interface:

- **Text prompts**: Only input prompts (`PromptInputAsync` and `PromptInputsAsync`) are available and appear as text-based prompts in the terminal.
- **Sequential input**: Multiple inputs are requested one at a time rather than in a single dialog.
- **Limited functionality**: Message boxes, notifications, and confirmation dialogs aren't available and will throw exceptions if called.

> [!IMPORTANT]
> In CLI contexts, only `PromptInputAsync` and `PromptInputsAsync` methods are supported. Calling `PromptMessageBoxAsync`, `PromptNotificationAsync`, or `PromptConfirmationAsync` results in an exception.

> [!NOTE]
> The same interaction service code works in both contexts, but with limitations in CLI mode. The underlying implementation automatically adapts the user experience based on whether the application is running in dashboard or CLI mode. However, only input-related methods (`PromptInputAsync` and `PromptInputsAsync`) are available during CLI operations like `aspire publish` and `aspire deploy`.

## See also

<!-- <xref:Aspire.Hosting.IInteractionService> -->

- [Aspire.Hosting.IInteractionService](https://github.com/dotnet/aspire/blob/main/src/Aspire.Hosting/IInteractionService.cs)
- [.NET Aspire extensibility overview](../extensibility/custom-hosting-integration.md)
