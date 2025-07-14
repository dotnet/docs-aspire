---
title: Interaction Service (Preview)
description: Use the interaction service API to prompt users for input, request confirmation, and display messages in the Aspire dashboard or CLI during publish and deploy.
ms.date: 07/14/2025
---

# .NET Aspire Interaction Service (Preview)

The interaction service API (`Aspire.Hosting.IInteractionService`) allows you to prompt users for input, request confirmation, and display messages in the Aspire dashboard or CLI during publish and deploy. This is useful for scenarios where you need to gather information from the user or provide feedback on the status of operations.

## The `IInteractionService` API

The `IInteractionService` interface can be retrieved from the dependency injection container of your <xref:Aspire.Hosting.DistributedApplication>. When you request this service, be sure to check if it's available for usage.

> [!IMPORTANT]
> If you attempt to use the interaction service when it's not available, an exception is thrown.

The interaction service has several methods that you use to interact with the users or display messages. The following sections describe how to use these APIs effectively.

## Where to use the interaction service

Any of the available callback-based extension methods of `IResourceBuilder<T>` can use the interaction service to prompt users for input or confirmation. Use the interaction service in these scenarios:

- **Custom resource types**: Gather input from users or confirm actions when you create custom resource types.

    Resource types are free to define dashboard interactions, such as prompting for user input or displaying messages. The interaction service allows you to create a more interactive experience for users when they manage resources in the Aspire dashboard. For more information, see [Create custom .NET Aspire hosting integrations](custom-hosting-integration.md).

- **Custom resource commands**: Add commands to resources in the Aspire dashboard. Use the interaction service to prompt users for input or confirmation when these commands run.

    When you chain a call to <xref:Aspire.Hosting.ResourceBuilderExtensions.WithCommand*> on a target `IResourceBuilder<T>`, for example, your callback can use the interaction service to gather input or confirm actions. For more information, see [Custom resource commands in .NET Aspire](../fundamentals/custom-resource-commands.md).

These approaches help you create interactive, user-friendly experiences for local development and dashboard interactions.

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

## Display messages

There are a number of ways to display messages to the user:

- **Dialog messages**: Show important information in a dialog box in the Aspire dashboard. The user must interact with the dialog to dismiss it.
- **Message bar messages**: Display less critical information in a message bar in the Aspire dashboard. The message doesn't require immediate action, so the user can continue working.

### Display a dialog message box

Dialog messages are modal windows that require user interaction before they can be dismissed. Use dialog messages for critical information, errors, or important notifications that need immediate attention.

<!-- <xref:Aspire.Hosting.IInteractionService.PromptMessageBoxAsync%2A> -->

The `IInteractionService.PromptMessageBoxAsync` method displays a dialog box with a title, message, and customizable buttons. You can specify the intent of the message (error, warning, information, etc.) and customize the button text.

:::code source="snippets/InteractionService/AppHost.MessageBoxExample.cs" id="example":::

:::image type="content" source="media/interaction-service-message-dialog.png" lightbox="media/interaction-service-message-dialog.png" alt-text="Aspire dashboard interface showing a message dialog with a title, message, and buttons.":::

Message dialogs come with many options that control their appearance and behavior. They optionally support Markdown formatting in the message text, allowing you to include links, lists, and other rich content.

### Display a message bar

Message bars are non-modal notifications that appear as an alert-style banner near the top of the dashboard interface without blocking the user's workflow. They can be dismissed by selecting the **X** button. They're ideal for status updates, informational messages, warnings, or notifications that don't require immediate action.

> [!TIP]
> Message bar notifications stack, so you can display multiple messages at once.

<!-- <xref:Aspire.Hosting.IInteractionService.PromptMessageBarAsync%2A> -->

The `IInteractionService.PromptMessageBarAsync` method displays a message bar with optional action links:

:::code source="snippets/InteractionService/AppHost.MessageBarExample.cs" id="example":::

The previous example demonstrates several ways to use the message bar API. Each approach displays different types of notifications in the Aspire dashboard:

:::image type="content" source="media/interaction-service-message-bar.png" lightbox="media/interaction-service-message-bar.png" alt-text="Aspire dashboard interface showing multiple message bars stacked near the top of the page. There are five message bars displayed, each with a different type of notification.":::

## Prompt for user confirmation

<!-- <xref:Aspire.Hosting.IInteractionService.PromptConfirmationAsync%2A> -->

Use the interaction service when you need the user to confirm an action before proceeding. The `IInteractionService.PromptConfirmationAsync` method displays a confirmation dialog in the Aspire dashboard. The user can choose a _primary_ action (like "Delete" or "Deploy") or _secondary_ action (like "Cancel") to cancel the operation.

Confirmation dialogs are essential for destructive operations or actions that have significant consequences. They help prevent accidental actions and give users a chance to reconsider their decisions.

### Prompt for confirmation before destructive operations

For operations that can't be undone, such as deleting resources, always prompt for confirmation:

:::code source="snippets/InteractionService/AppHost.ConfirmationExample.cs" id="example":::

This example renders on the dashboard as shown in the following image:

:::image type="content" source="media/interaction-service-confirmation.png" lightbox="media/interaction-service-confirmation.png" alt-text="Aspire dashboard interface showing a confirmation dialog with a title, message, and buttons for confirming or canceling the operation.":::

## Prompt for user input

The interaction service API allows you to prompt users for input in a variety of ways. You can collect single values or multiple values at once, with support for different input types including text, secrets, choices, booleans, and numbers.

### Prompt user for a single input

<!-- <xref:Aspire.Hosting.IInteractionService.PromptInputAsync%2A> -->

Use the `IInteractionService.PromptInputAsync` method to collect a single piece of information from the user. This method supports various input types through the `InteractionInput` class. Consider the following example, that prompts the user for an API key as a secret input value:

:::code source="snippets/InteractionService/AppHost.SingleInputExample.cs" id="example":::

When the Aspire dashboard runs and the user selects the corresponding command, the interaction service displays a dialog like the following:

:::image type="content" source="media/interaction-service-single-input.png" lightbox="media/interaction-service-single-input.png" alt-text="Aspire dashboard interface showing a single input dialog with a label, input field, and buttons for confirming or canceling the input.":::

If the user doesn't enter a value, the dialog will show an error message indicating that the input is required:

:::image type="content" source="media/interaction-service-single-input-validation.png" lightbox="media/interaction-service-single-input-validation.png" alt-text="Aspire dashboard interface showing a single input dialog with an error message indicating that the input is required.":::

### Prompt the user for multiple input values

<!-- <xref:Aspire.Hosting.IInteractionService.PromptInputsAsync%2A> -->

Use the `IInteractionService.PromptInputsAsync` method to collect multiple pieces of information in a single dialog. This is more efficient and provides a better user experience when you need several related configuration values.

Consider the following example, which prompts the user for multiple input values:

:::code source="snippets/InteractionService/AppHost.MultipleInputExample.cs" id="example":::

This renders on the dashboard as shown in the following image:

:::image type="content" source="media/interaction-service-multiple-input.png" lightbox="media/interaction-service-multiple-input.png" alt-text="Aspire dashboard interface showing a multiple input dialog with labels, input fields, and buttons for confirming or canceling the input.":::

Imagine you fill out the dialog with the following values:

:::image type="content" source="media/interaction-service-multiple-input-filled.png" lightbox="media/interaction-service-multiple-input-filled.png" alt-text="Aspire dashboard interface showing a multiple input dialog with filled input fields and buttons for confirming or canceling the input.":::

After selecting the **Ok** button, the resource logs display the following output:

:::image type="content" source="media/interaction-service-multiple-input-logs.png" lightbox="media/interaction-service-multiple-input-logs.png" alt-text="Aspire dashboard interface showing logs with the input values entered in the multiple input dialog.":::

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

1. **Group related inputs**: Use multiple input prompts to collect related configuration values in a single dialog.
1. **Provide clear labels and placeholders**: Help users understand what information is expected.
1. **Use appropriate input types**: Choose the right input type for the data you're collecting (secret for passwords, choice for predefined options, etc.).
1. **Implement validation**: Validate user input and provide clear error messages when validation fails.
1. **Make required fields clear**: Mark required fields and provide appropriate defaults for optional ones.
1. **Handle cancellation**: Always check if the user canceled the input dialog and handle it gracefully.

## Real-world usage patterns

The interaction service is particularly useful in scenarios where you need to gather configuration from users during deployment or setup processes. Here are some common patterns:

### Progressive disclosure

For complex configurations, use a progressive approach where you start with basic questions and ask for more details based on user choices:

1. **Initial choice**: Ask users to select a deployment target (local, cloud, specific provider).
1. **Environment-specific questions**: Based on the initial choice, ask for relevant configuration details.
1. **Confirmation with summary**: Show a summary of all choices and ask for final confirmation.
1. **Status updates**: Use message bars to show progress during long-running operations.

### Error handling and recovery

Always provide clear error messages and recovery options:

- Use message boxes with error intent for critical failures.
- Provide links to documentation or troubleshooting guides in message bars.
- Allow users to retry operations after fixing issues.
- Gracefully handle cancellation and provide alternative flows.

## See also

<!-- <xref:Aspire.Hosting.IInteractionService> -->

- [Aspire.Hosting.IInteractionService](https://github.com/dotnet/aspire/blob/main/src/Aspire.Hosting/IInteractionService.cs)
- [.NET Aspire extensibility overview](../extensibility/custom-hosting-integration.md)
