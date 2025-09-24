using Microsoft.Extensions.DependencyInjection;

var builder = DistributedApplication.CreateBuilder(args);

var fakeResource = builder.AddFakeResource("fake-resource-01")
    .WithCommand("msg-dialog", "Example Message Dialog", ShowMessageBoxExample)
    .WithCommand("msg-bar", "Example Message Bar", ShowNotificationExample)
    .WithCommand("confirm", "Confirmation Example", ShowConfirmationExample);

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
    });

builder.Build().Run();
