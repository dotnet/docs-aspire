using Microsoft.Extensions.DependencyInjection;

partial class Program
{
    public static async Task<ExecuteCommandResult> ShowMessageBarExample(ExecuteCommandContext context)
    {
        // <example>
        var interactionService = context.ServiceProvider.GetRequiredService<IInteractionService>();

        // Demonstrating various message bar types with different intents
        var tasks = new List<Task>
        {
            interactionService.PromptMessageBarAsync(
                title: "Confirmation",
                message: "Are you sure you want to proceed?",
                options: new MessageBarInteractionOptions
                {
                    Intent = MessageIntent.Confirmation
                }),
            interactionService.PromptMessageBarAsync(
                title: "Success",
                message: "Your operation completed successfully.",
                options: new MessageBarInteractionOptions
                {
                    Intent = MessageIntent.Success,
                    LinkText = "View Details",
                    LinkUrl = "https://learn.microsoft.com/dotnet/aspire/success"
                }),
            interactionService.PromptMessageBarAsync(
                title: "Warning",
                message: "Your SSL certificate will expire soon.",
                options: new MessageBarInteractionOptions
                {
                    Intent = MessageIntent.Warning,
                    LinkText = "Renew Certificate",
                    LinkUrl = "https://portal.azure.com/certificates"
                }),
            interactionService.PromptMessageBarAsync(
                title: "Information",
                message: "There is an update available for your application.",
                options: new MessageBarInteractionOptions
                {
                    Intent = MessageIntent.Information,
                    LinkText = "Update Now",
                    LinkUrl = "https://learn.microsoft.com/dotnet/aspire"
                }),
            interactionService.PromptMessageBarAsync(
                title: "Error",
                message: "An error occurred while processing your request.",
                options: new MessageBarInteractionOptions
                {
                    Intent = MessageIntent.Error,
                    LinkText = "Troubleshoot",
                    LinkUrl = "https://learn.microsoft.com/dotnet/aspire/troubleshoot"
                })
        };

        await Task.WhenAll(tasks);

        return CommandResults.Success();
        // </example>
    }
}
