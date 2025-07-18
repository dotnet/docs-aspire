using Microsoft.Extensions.DependencyInjection;

partial class Program
{
    public static async Task<ExecuteCommandResult> ShowConfirmationExample(ExecuteCommandContext context)
    {
        // <example>
        var interactionService = context.ServiceProvider.GetRequiredService<IInteractionService>();        

        // Prompt for confirmation before resetting database
        var resetConfirmation = await interactionService.PromptConfirmationAsync(
            title: "Confirm Reset",
            message: "Are you sure you want to reset the `development-database`? This action **cannot** be undone.",
            options: new MessageBoxInteractionOptions
            {
                Intent = MessageIntent.Confirmation,
                PrimaryButtonText = "Reset",
                SecondaryButtonText = "Cancel",
                ShowSecondaryButton = true,
                EnableMessageMarkdown = true
            });

        if (resetConfirmation.Data is true)
        {
            // Perform the reset operation...

            return CommandResults.Success();
        }
        else
        {
            return CommandResults.Failure("Database reset canceled by user.");
        }
        // </example>
    }
}
