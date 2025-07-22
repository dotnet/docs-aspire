using Microsoft.Extensions.DependencyInjection;

partial class Program
{
    public static async Task<ExecuteCommandResult> ShowNotificationExample(ExecuteCommandContext context)
    {
        // <example>
        var interactionService = context.ServiceProvider.GetRequiredService<IInteractionService>();

        // Demonstrating various notification types with different intents
        var tasks = new List<Task>
        {
            interactionService.PromptNotificationAsync(
                title: "Confirmation",
                message: "Are you sure you want to proceed?",
                options: new NotificationInteractionOptions
                {
                    Intent = MessageIntent.Confirmation
                }),
            interactionService.PromptNotificationAsync(
                title: "Success",
                message: "Your operation completed successfully.",
                options: new NotificationInteractionOptions
                {
                    Intent = MessageIntent.Success,
                    LinkText = "View Details",
                    LinkUrl = "https://learn.microsoft.com/dotnet/aspire/success"
                }),
            interactionService.PromptNotificationAsync(
                title: "Warning",
                message: "Your SSL certificate will expire soon.",
                options: new NotificationInteractionOptions
                {
                    Intent = MessageIntent.Warning,
                    LinkText = "Renew Certificate",
                    LinkUrl = "https://portal.azure.com/certificates"
                }),
            interactionService.PromptNotificationAsync(
                title: "Information",
                message: "There is an update available for your application.",
                options: new NotificationInteractionOptions
                {
                    Intent = MessageIntent.Information,
                    LinkText = "Update Now",
                    LinkUrl = "https://learn.microsoft.com/dotnet/aspire"
                }),
            interactionService.PromptNotificationAsync(
                title: "Error",
                message: "An error occurred while processing your request.",
                options: new NotificationInteractionOptions
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
