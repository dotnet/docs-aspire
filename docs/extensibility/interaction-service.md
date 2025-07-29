---
title: Interaction Service (Preview)
description: Use the interaction service API to prompt users for input, request confirmation, and display messages in the Aspire dashboard or CLI during publish and deploy.
ms.date: 07/21/2025
---

# Interaction Service (Preview)

The interaction service (`Aspire.Hosting.IInteractionService`) allows you to prompt users for input, request confirmation, and display messages. The interaction service works in two different contexts:

- **Aspire dashboard**: When running `aspire run` or launching the app host directly, interactions appear as dialogs and notifications in the dashboard UI.
- **Aspire CLI**: When running `aspire publish` or `aspire deploy`, interactions are prompted through the command-line interface.

This is useful for scenarios where you need to gather information from the user or provide feedback on the status of operations, regardless of how the application is being launched or deployed.

## The `IInteractionService` API

The `IInteractionService` interface is retrieved from the <xref:Aspire.Hosting.DistributedApplication> dependency injection container. `IInteractionService` can be injected into types created from DI or resolved from <xref:System.IServiceProvider>, which is usually available on a context argument passed to events.

When you request `IInteractionService`, be sure to check if it's available for usage. If you attempt to use the interaction service when it's not available (`IInteractionService.IsAvailable` returns `false`), an exception is thrown.

```csharp
var interactionService = serviceProvider.GetRequiredService<IInteractionService>();
if (interactionService.IsAvailable)
{
    var result = await interactionService.PromptConfirmationAsync(
        title: "Delete confirmation",
        message: "Are you sure you want to delete the data?");
        
    if (result.Data)
    {
        // Run your resource/command logic.
    }
}
```

The interaction service has several methods that you use to interact with users or display messages. The behavior of these methods depends on the execution context:

- **Dashboard context** (`aspire run` or direct app host launch): Interactions appear as modal dialogs, notifications, and form inputs in the [Aspire dashboard web interface](../fundamentals/dashboard/overview.md).
- **CLI context** (`aspire publish` or `aspire deploy`): Interactions are prompted through the command-line interface with text-based prompts and responses.

The following sections describe how to use these APIs effectively in both contexts:

| Method | Description | Contexts supported |
|--|--|--|
| `PromptMessageBoxAsync` | Displays a modal dialog box with a message and buttons for user interaction. | Dashboard only |
| `PromptNotificationAsync` | Displays a non-modal notification in the dashboard as a message bar. | Dashboard only |
| `PromptConfirmationAsync` | Displays a confirmation dialog with options for the user to confirm or cancel an action. | Dashboard only |
| `PromptInputAsync` | Prompts the user for a single input value, such as text or secret. | Dashboard, CLI |
| `PromptInputsAsync` | Prompts the user for multiple input values in a single dialog (dashboard) or sequentially (CLI). | Dashboard, CLI |

> [!IMPORTANT]
> During `aspire publish` and `aspire deploy` operations, only `PromptInputAsync` and `PromptInputsAsync` are available. Other interaction methods (`PromptMessageBoxAsync`, `PromptNotificationAsync`, and `PromptConfirmationAsync`) will throw an exception if called in CLI contexts.

## Where to use the interaction service

Any of the available callback-based extension methods of `IResourceBuilder<T>` can use the interaction service to prompt users for input or confirmation. Use the interaction service in these scenarios:

- **Custom resource types**: Gather input from users or confirm actions when you create custom resource types.

    Resource types are free to define dashboard interactions, such as prompting for user input or displaying messages. The interaction service allows you to create a more interactive experience for users when they manage resources in the Aspire dashboard or CLI. For more information, see [Create custom .NET Aspire hosting integrations](custom-hosting-integration.md).

- **Custom resource commands**: Add commands to resources in the Aspire dashboard or CLI. Use the interaction service to prompt users for input or confirmation when these commands run.

    When you chain a call to <xref:Aspire.Hosting.ResourceBuilderExtensions.WithCommand*> on a target `IResourceBuilder<T>`, for example, your callback can use the interaction service to gather input or confirm actions. For more information, see [Custom resource commands in .NET Aspire](../fundamentals/custom-resource-commands.md).

- **Publish and deploy workflows**: During `aspire publish` or `aspire deploy` operations, use the interaction service to gather deployment-specific configuration and confirm destructive operations through the CLI.

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

There are several ways to display messages to the user:

- **Dialog messages**: Show important information in a dialog box.
- **Notification messages**: Display less critical information in a notification.

> [!NOTE]
> Message display methods (`PromptMessageBoxAsync` and `PromptNotificationAsync`) are only available in dashboard contexts. These methods throw an exception if called during `aspire publish` or `aspire deploy` operations.

### Display a dialog message box

Dialog messages provide important information that requires user attention.

<!-- <xref:Aspire.Hosting.IInteractionService.PromptMessageBoxAsync%2A> -->

The `IInteractionService.PromptMessageBoxAsync` method displays a message with customizable response options.

:::code source="snippets/InteractionService/AppHost.MessageBoxExample.cs" id="example":::

**Dashboard view:**

:::image type="content" source="media/interaction-service-message-dialog.png" lightbox="media/interaction-service-message-dialog.png" alt-text="Aspire dashboard interface showing a message dialog with a title, message, and buttons.":::

**CLI view:**

The `PromptMessageBoxAsync` method only works in the dashboard context. If you call it during `aspire publish` or `aspire deploy`, the method throws an exception and doesn't display a message in the CLI.

### Display a notification message

Notification messages provide non-modal notifications.

> [!TIP]
> In the dashboard, notification messages appear stacked at the top, so you can show several messages at once. You can display notifications one after another by awaiting each dismissal before showing the next. Or, you can display multiple notifications at the same time without waiting for each to be dismissed.

<!-- <xref:Aspire.Hosting.IInteractionService.PromptNotificationAsync%2A> -->

The `IInteractionService.PromptNotificationAsync` method displays informational messages with optional action links in the dashboard context. You don't have to await the result of a notification message if you don't need to. This is especially useful for notifications, since you might want to display a notification and continue without waiting for user to dismiss it.

:::code source="snippets/InteractionService/AppHost.NotificationExample.cs" id="example":::

The previous example demonstrates several ways to use the notification API. Each approach displays different types of notifications, all of which are invoked in parallel and displayed in the dashboard around the same time.

**Dashboard view:**

:::image type="content" source="media/interaction-service-message-bar.png" lightbox="media/interaction-service-message-bar.png" alt-text="Aspire dashboard interface showing multiple notification messages stacked near the top of the page. There are five notifications displayed, each with a different type of notification.":::

**CLI view:**

The `PromptNotificationAsync` method isn't available in CLI contexts. If you call it during `aspire publish` or `aspire deploy`, it throws an exception.

## Prompt for user confirmation

<!-- <xref:Aspire.Hosting.IInteractionService.PromptConfirmationAsync%2A> -->

Use the interaction service when you need the user to confirm an action before proceeding. The `IInteractionService.PromptConfirmationAsync` method displays a confirmation prompt in the dashboard context. Confirmation prompts are essential for destructive operations or actions that have significant consequences. They help prevent accidental actions and give users a chance to reconsider their decisions.

### Prompt for confirmation before destructive operations

For operations that can't be undone, such as deleting resources, always prompt for confirmation:

:::code source="snippets/InteractionService/AppHost.ConfirmationExample.cs" id="example":::

**Dashboard view:**

:::image type="content" source="media/interaction-service-confirmation.png" lightbox="media/interaction-service-confirmation.png" alt-text="Aspire dashboard interface showing a confirmation dialog with a title, message, and buttons for confirming or canceling the operation.":::

**CLI view:**

The `PromptConfirmationAsync` method isn't available in CLI contexts. If you call it during `aspire publish` or `aspire deploy`, the method throws an exception.

## Prompt for user input

The interaction service API allows you to prompt users for input in various ways. You can collect single values or multiple values, with support for different input types including text, secrets, choices, booleans, and numbers. The presentation adapts automatically to the execution context.

| Type         | Dashboard                 | CLI prompt            |
|--------------|---------------------------|-----------------------|
| `Text`       | Textbox                   | Text prompt           |
| `SecretText` | Textbox with masked input | Masked text prompt    |
| `Choice`     | Dropdown box of options   | Choice prompt         |
| `Boolean`    | Checkbox                  | Boolean choice prompt |
| `Number`     | Number textbox            | Text prompt           |

### Prompt the user for input values

<!-- <xref:Aspire.Hosting.IInteractionService.PromptInputsAsync%2A> -->

You can prompt for a single value using the `IInteractionService.PromptInputAsync` method, or collect multiple pieces of information with `IInteractionService.PromptInputsAsync`. In the dashboard, multiple inputs appear together in a single dialog. In the CLI, each input is requested one after another.

> [!IMPORTANT]
> It's possible to create wizard-like flows using the interaction service. By chaining multiple prompts together—handling the results from one prompt before moving to the next—you can guide users through a series of related questions, making it easier to collect all the necessary information.

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

═══════════════════════════════════════════════════════════════════════════════════════════════════════════════

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

<!-- <xref:Aspire.Hosting.InteractionInput> -->

Basic input validation is available by configuring `InteractionInput`. It provides options for requiring a value, or the maximum text length of `Text` or `SecretText` fields.

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
        Label = "Confirm password",
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
        var confirmPasswordInput = context.Inputs.FirstOrDefault(i => i.Label == "Confirm password");

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
    title: "Database configuration",
    message: "Configure your PostgreSQL database connection:",
    inputs: databaseInputs,
    options: validationOptions);

if (!dbResult.Canceled && dbResult.Data != null)
{
    // Use the validated configuration
}
```

Prompting the user for a password and confirming they match is referred to as "dual independent verification" input. This approach is common in scenarios where you want to ensure the user enters the same password twice to avoid typos or mismatches.

### Best practices for user input

When prompting for user input, consider these best practices:

1. **Group related inputs**: Use multiple input prompts to collect related configuration values. In the dashboard, these appear in a single dialog; in the CLI, they're requested sequentially but grouped conceptually.
1. **Provide clear labels and placeholders**: Help users understand what information is expected, regardless of context.
1. **Use appropriate input types**: Choose the right input type for the data you're collecting (secret for passwords, choice for predefined options, etc.). Both contexts support these input types appropriately.
1. **Implement validation**: Validate user input and provide clear error messages when validation fails. Both contexts support validation feedback.
1. **Make required fields clear**: Mark required fields and provide appropriate defaults for optional ones.
1. **Handle cancellation**: Always check if the user canceled the input prompt and handle it gracefully. Users can cancel in both dashboard and CLI contexts.

## Interaction contexts

The interaction service behaves differently depending on how your Aspire solution is launched:

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
- **Limited functionality**: Message boxes, notifications, and confirmation dialogs aren't available and throws exceptions if called.

> [!IMPORTANT]
> The interaction service adapts automatically to dashboard and CLI contexts. In CLI mode, only input-related methods—`PromptInputAsync` and `PromptInputsAsync`—are supported. Calling `PromptMessageBoxAsync`, `PromptNotificationAsync`, or `PromptConfirmationAsync` in CLI operations like `aspire publish` or `aspire deploy` results in an exception.

## See also

<!-- <xref:Aspire.Hosting.IInteractionService> -->

- [Aspire.Hosting.IInteractionService](https://github.com/dotnet/aspire/blob/main/src/Aspire.Hosting/IInteractionService.cs)
- [.NET Aspire extensibility overview](../extensibility/custom-hosting-integration.md)
