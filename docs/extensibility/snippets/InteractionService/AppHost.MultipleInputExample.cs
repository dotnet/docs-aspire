using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

partial class Program
{
    public static async Task<ExecuteCommandResult> ShowMultipleInputExample(
        ExecuteCommandContext context, FakeResource fakeResource)
    {
        // <example>
        var interactionService = context.ServiceProvider.GetRequiredService<IInteractionService>();
        var loggerService = context.ServiceProvider.GetRequiredService<ResourceLoggerService>();
        var logger = loggerService.GetLogger(fakeResource);

        var inputs = new List<InteractionInput>
        {
            new()
            {
                Name = "Application Name",
                InputType = InputType.Text,
                Required = true,
                Placeholder = "my-app"
            },
            new()
            {
                Name = "Environment",
                InputType = InputType.Choice,
                Required = true,
                Options =
                [
                    new("dev", "Development"),
                    new("staging", "Staging"),
                    new("test", "Testing")
                ]
            },
            new()
            {
                Name = "Instance Count",
                InputType = InputType.Number,
                Required = true,
                Placeholder = "1"
            },
            new()
            {
                Name = "Enable Monitoring",
                InputType = InputType.Boolean,
                Required = false
            }
        };

        var appConfigurationInput = await interactionService.PromptInputsAsync(
            title: "Application Configuration",
            message: "Configure your application deployment settings:",
            inputs: inputs);

        if (!appConfigurationInput.Canceled)
        {
            // Process the collected input values
            var appName = appConfigurationInput.Data[0].Value;
            var environment = appConfigurationInput.Data[1].Value;
            var instanceCount = int.Parse(appConfigurationInput.Data[2].Value ?? "1");
            var enableMonitoring = bool.Parse(appConfigurationInput.Data[3].Value ?? "false");

            logger.LogInformation("""
                Application Name: {AppName}
                Environment: {Environment}
                Instance Count: {InstanceCount}
                Monitoring Enabled: {EnableMonitoring}
                """,
                appName, environment, instanceCount, enableMonitoring);

            // Use the collected values as needed
            return CommandResults.Success();
        }
        else
        {
            return CommandResults.Failure("User canceled application configuration input.");
        }
        // </example>
    }
}
