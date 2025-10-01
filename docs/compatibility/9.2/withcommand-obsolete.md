---
title: "Breaking change - WithCommand obsolete and new overload with CommandOptions"
description: "Learn about the breaking change in Aspire 9.2 where the WithCommand method overload with optional parameters is marked obsolete."
ms.date: 3/25/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/2888
---

# WithCommand obsolete and new overload with CommandOptions

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithCommand*> method overload that accepted multiple optional parameters is now marked obsolete. A new overload that accepts an instance of `CommandOptions` has been introduced. This change requires updates to existing code to use the new overload.

## Version introduced

Aspire 9.2

## Previous behavior

The `WithCommand` method overload that accepted multiple optional parameters was available and not marked as obsolete.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.AspireApp_ApiService>("apiservice")
       .WithCommand(
           name: "command-name",
           displayName: "Command display name",
           executeCommand: async (ExecuteCommandContext context) =>
           {
                // Command execution logic here
                await Task.CompletedTask;
                return CommandResults.Success();
           },
           updateState: (UpdateCommandStateContext context) =>
           {
                // State update logic here
                return ResourceCommandState.Enabled;
           },
           displayDescription: "Command Description",
           parameter: new[] { "", "" },
           confirmationMessage: "Are you sure?",
           iconName: "Icons",
           iconVariant: "Red",
           isHighlighted: false);
```

## New behavior

The existing <xref:Aspire.Hosting.ResourceBuilderExtensions.WithCommand*> method overload is now marked obsolete. A new overload that accepts an instance of `CommandOptions` should be used instead.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.AspireApp_ApiService>("apiservice")
       .WithCommand(
           name: "command-name",
           displayName: "Command display name",
           executeCommand: async (ExecuteCommandContext context) =>
           {
                // Command execution logic here
                await Task.CompletedTask;
                return CommandResults.Success();
           },
           commandOptions: new CommandOptions
           {
               UpdateState = (UpdateCommandStateContext context) =>
               {
                   // State update logic here
                   return ResourceCommandState.Enabled;
               },
               Description = "Command Description",
               Parameter = new[] { "", "" },
               ConfirmationMessage = "Are you sure?",
               IconName = "Icons",
               IconVariant = "Red",
               IsHighlighted = false
           });
```

The only required parameters are the `name`, `displayName`, and `executeCommand`. The rest of the parameters are now encapsulated within the `CommandOptions` object, which provides a cleaner and more maintainable API.

## Type of breaking change

This is a [source incompatible](../categories.md#source-compatibility) change.

## Reason for change

This change was made following an API review to improve clarity and maintainability by consolidating optional parameters into a single `CommandOptions` object.

## Recommended action

Update your code to use the new `WithCommand` overload that accepts an instance of `CommandOptions`. Replace calls to the obsolete overload with the new overload.

## Affected APIs

- <xref:Aspire.Hosting.ResourceBuilderExtensions.WithCommand*>
