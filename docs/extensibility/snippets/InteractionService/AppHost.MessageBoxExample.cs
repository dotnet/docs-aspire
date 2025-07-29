using Microsoft.Extensions.DependencyInjection;

partial class Program
{
    public static async Task<ExecuteCommandResult> ShowMessageBoxExample(ExecuteCommandContext context)
    {
        // <example>
        var interactionService = context.ServiceProvider.GetRequiredService<IInteractionService>();

        var result = await interactionService.PromptMessageBoxAsync(
            "Simple Message Box: Example",
            """
            ##### 🤓 Nice!

            It's worth noting that **Markdown** is _supported_
            (and demonstrated here) in the message body. Simply
            configure the options as:

            ```csharp
            var options = new MessageBoxInteractionOptions
            {
                EnableMessageMarkdown = true,
                // Other options...
            };
            ```

            Cool, [📖 learn more](https://learn.microsoft.com/dotnet/aspire/extensibility/interaction-service)...
            """,
            new MessageBoxInteractionOptions
            {
                EnableMessageMarkdown = true,
                PrimaryButtonText = "Awesome"
            }
        );

        if (result.Canceled)
        {
            return CommandResults.Failure("User cancelled.");
        }

        return result.Data
            ? CommandResults.Success()
            : CommandResults.Failure("The user doesn't like the example");
        // </example>
    }
}
