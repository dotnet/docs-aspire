---
title: GitHub Copilot in the .NET Aspire dashboard
description: Learn how to use GitHub Copilot in the .NET Aspire dashboard.
ms.date: 05/19/2025
ms.topic: reference
---

# GitHub Copilot in the .NET Aspire dashboard

Introducing GitHub Copilot in the .NET Aspire dashboard! GitHub Copilot is your new AI debugging assistant.

GitHub Copilot supercharges the dashboard's OpenTelemetry debugging and diagnostics experience. With AI, you can:

- Review hundreds of log messages with a single click
- Investigate the root cause of errors across multiple apps
- Highlight performance issues in traces
- Explain obscure error codes using AI's huge knowledge repository

You'll have access to Copilot in the dashboard when you launch your app from VS Code or Visual Studio.

## Requirements

GitHub Copilot is only available when a .NET Aspire project is run from an IDE.

> [!div class="checklist"]
>
> - VS Code and C# Dev Kit 1.19.63 or later.
> - Visual Studio 17.14 or later.

You must also be logged into a GitHub account in the IDE that has a GitHub Copilot subscription. Copilot interactions in the .NET Aspire dashboard use the GitHub account associated with your IDE.

If you don't have a Copilot subscription yet, you can use Copilot for free by signing up for the [Copilot Free plan](https://aka.ms/dotnet/aspire/copilot-free-signup), which includes a monthly limit of chat interactions.

## Get started

To get started, launch your .NET Aspire solution as usual in your IDE.

1. Open your .NET Aspire solution in a supported version of Visual Studio or VS Code with the [C# Dev Kit extension](/visualstudio/subscriptions/vs-c-sharp-dev-kit) installed.
2. Ensure you're logged into the IDE with a GitHub account:
   - For Visual Studio, see [Add your GitHub accounts to your Visual Studio keychain](https://aka.ms/dotnet/aspire/copilot-vs-login).
   - For VS Code and C# Dev Kit, see [Set up GitHub Copilot in VS Code](https://aka.ms/dotnet/aspire/copilot-vscode-login).
3. Run the .NET Aspire app host.

A GitHub Copilot button appears in the top-right corner of the dashboard. Selecting the button opens the Copilot UI.

:::image type="content" source="media/copilot/copilot-headerbutton.png" lightbox="media/copilot/copilot-headerbutton.png" alt-text="A screenshot of the dashboard with the GitHub Copilot button highlighted.":::

In the Copilot UI, you can either select a suggested question, such as **Summarize resources**, or enter your own question in the chat input field. Press <kbd>Enter</kbd> to submit. For example: `What health checks are configured in my app?`

:::image type="content" source="media/copilot/copilot-chatquestion.png" lightbox="media/copilot/copilot-chatquestion.png" alt-text="A screenshot of the dashboard with GitHub Copilot open. It contains a chat question and answer.":::

At the top of the Copilot UI are buttons to start a new conversation, expand the chat, or close the UI.

## Analyze resources, console logs, and telemetry

There are several options for analyzing resources, console logs, and telemetry in your app. Two were discussed above:

- Select a suggested question. Copilot's suggested questions are contextual. For example, questions about resources appear on the resources page, and questions about distributed traces appear on the traces page.
- Enter your own question in the chat input field, such as `Why did the product cache fail to start?` or `What caused recent errors in the frontend?`

The dashboard also includes a Copilot option in the context menus for resources, structured logs, traces, and spans. Select **Ask GitHub Copilot** to investigate the selected data.

:::image type="content" source="media/copilot/resources-askcopilot.png" lightbox="media/copilot/resources-askcopilot.png" alt-text="A screenshot of the Ask GitHub Copilot option in the resource context menu page.":::

If your app has structured logs or traces with errors, an **Explain errors** button appears on those pages. Selecting it makes all errors available to Copilot for investigation.

:::image type="content" source="media/copilot/structuredlogs-explainerror.png" lightbox="media/copilot/structuredlogs-explainerror.png" alt-text="A screenshot of the structured logs page with an Explain errors button.":::

On the trace details page, the **Explain trace** button is always visible. It provides a quick way to analyze the currently viewed trace.

:::image type="content" source="media/copilot/tracedetails-explaintrace.png" lightbox="media/copilot/tracedetails-explaintrace.png" alt-text="A screenshot of the trace details page with an Explain trace button.":::

## Troubleshooting

### Copilot hangs on "Getting ready"

A known issue can cause the Copilot UI to hang with a "Getting ready" message when it is first accessed. This occurs when the dashboard is not launched from a [supported version](#requirements) of Visual Studio or VS Code, or when no GitHub account is logged into the IDE. Closing and reopening the Copilot UI fixes the issue. Once fixed, the UI correctly displays a message with requirements for using the dashboard with GitHub Copilot.

### Reached monthly limit of Copilot Free plan

The GitHub Copilot Free plan includes a monthly limit on chat usage. When this limit is exceeded, Copilot will no longer function in the dashboard. [Upgrade your Copilot subscription](https://aka.ms/dotnet/aspire/copilot-pro) or wait for the limit to reset.

## Limitations

### Data size

AI models have limits on how much data they can process at once. The .NET Aspire dashboard may limit the amount of data sent to GitHub Copilot when necessary.

- Large data fields (e.g., long exception stack traces) may be truncated.
- Requests involving large collections of telemetry may be shortened by omitting older items.

## Disable GitHub Copilot in the dashboard

GitHub Copilot only accesses data when you explicitly interact with it. For example, submitting a chat query or selecting **Ask GitHub Copilot** from a context menu.

If you prefer to disable the feature entirely, set the `ASPIRE_DASHBOARD_AI_DISABLED` environment variable to `true` in your app host's _launchSettings.json_ file. This hides all Copilot UI elements in the dashboard.

:::code language="json" source="snippets/DisableAI/AspireApp/AspireApp.AppHost/Properties/launchSettings.json" highlight="14":::

## GitHub Copilot frequently asked questions

For more information about GitHub Copilot, such as questions around plans, privacy and responsible AI usage, see the [GitHub Copilot frequently asked questions](https://github.com/features/copilot#faq).
