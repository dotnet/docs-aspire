using Microsoft.Extensions.DependencyInjection;

var builder = DistributedApplication.CreateBuilder(args);

var fakeResource = builder.AddFakeResource("fake-resource-01")
    .WithCommand("msg-dialog", "Example Message Dialog", ShowMessageBoxExample)
    .WithCommand("msg-bar", "Example Message Bar", ShowNotificationExample)
    .WithCommand("confirm", "Confirmation Example", ShowConfirmationExample)
    .WithCommand("single-input", "Single Input Example", ShowSingleInputExample);

fakeResource.WithCommand(
    "multi-input",
    "Multi Input Example",
    context => ShowMultipleInputExample(context, fakeResource.Resource));


builder.AddFakeResource("fake-resource-02")
    .WithDeployment(async context =>
    {
        var interactionService = context.Services.GetRequiredService<IInteractionService>();
        var logger = context.Logger;

        var inputs = new List<InteractionInput>
        {
            new()
            {
                Label = "Application Name",
                InputType = InputType.Text,
                Required = true,
                Placeholder = "my-app"
            },
            new()
            {
                Label = "Environment",
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
                Label = "Instance Count",
                InputType = InputType.Number,
                Required = true,
                Placeholder = "1"
            },
            new()
            {
                Label = "Enable Monitoring",
                InputType = InputType.Boolean,
                Required = false
            }
        };

        var appConfigurationInput = await interactionService.PromptInputsAsync(
            title: "Application Configuration",
            message: "Configure your application deployment settings:",
            inputs: inputs);
    });

builder.Build().Run();
