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

builder.Build().Run();
