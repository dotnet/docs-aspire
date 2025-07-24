var builder = DistributedApplication.CreateBuilder(args);

// <unresolvedparameters>
#pragma warning disable ASPIREINTERACTION001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
var externalServiceUrl = builder.AddParameter("external-service-url")
    .WithDescription("The URL of the external service.")
    .WithCustomInput(p => new()
    {
        InputType = InputType.Text,
#pragma warning restore ASPIREINTERACTION001
        Value = "https://example.com",
        Label = p.Name,
        Placeholder = $"Enter value for {p.Name}",
        Description = p.Description
    });
var externalService = builder.AddExternalService("external-service", externalServiceUrl);
// </unresolvedparameters>

builder.Build().Run();
