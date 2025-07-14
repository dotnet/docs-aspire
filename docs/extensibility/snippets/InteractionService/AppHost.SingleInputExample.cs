using Microsoft.Extensions.DependencyInjection;

partial class Program
{
    public static async Task<ExecuteCommandResult> ShowSingleInputExample(ExecuteCommandContext context)
    {
        // <example>
        var interactionService = context.ServiceProvider.GetRequiredService<IInteractionService>();

        var apiKeyInput = await interactionService.PromptInputAsync(
            title: "API Configuration",
            message: "Enter your third-party service API key:",
            input: new InteractionInput
            {
                Label = "API Key",
                InputType = InputType.SecretText,
                Required = true,
                Placeholder = "Enter your API key"
            });

        if (!apiKeyInput.Canceled)
        {
            var apiKey = apiKeyInput.Data?.Value ?? "";

            // Store the API key securely

            return CommandResults.Success();
        }
        else
        {
            return CommandResults.Failure("User canceled API key input.");
        }
        // </example>
    }
}
