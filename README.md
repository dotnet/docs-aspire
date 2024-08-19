# .NET Aspire docs

[![Discord](https://img.shields.io/discord/732297728826277939?style=flat&logo=discord&logoColor=white&label=join%20our%20discord&labelColor=512bd4&color=cyan)](https://discord.com/invite/h87kDAHQgJ)
[![OpenSSF Best Practices](https://www.bestpractices.dev/projects/9186/badge)](https://www.bestpractices.dev/projects/9186)
[![GitHub contributors](https://img.shields.io/github/contributors/dotnet/docs-aspire.svg)](https://GitHub.com/dotnet/docs-aspire/graphs/contributors/)
[![GitHub repo size](https://img.shields.io/github/repo-size/dotnet/docs-aspire)](https://github.com/dotnet/docs-aspire)
[![GitHub issues-opened](https://img.shields.io/github/issues/dotnet/docs-aspire.svg)](https://GitHub.com/dotnet/docs-aspire/issues?q=is%3Aissue+is%3Aopened)
[![GitHub issues-closed](https://img.shields.io/github/issues-closed/dotnet/docs-aspire.svg)](https://GitHub.com/dotnet/docs-aspire/issues?q=is%3Aissue+is%3Aclosed)
[![GitHub pulls-opened](https://img.shields.io/github/issues-pr/dotnet/docs-aspire.svg)](https://GitHub.com/dotnet/docs-aspire/pulls?q=is%3Aissue+is%3Aopened)
[![GitHub pulls-merged](https://img.shields.io/github/issues-search/dotnet/docs-aspire?label=merged%20pull%20requests&query=is%3Apr%20is%3Aclosed%20is%3Amerged&color=darkviolet)](https://github.com/dotnet/docs-aspire/pulls?q=is%3Apr+is%3Aclosed+is%3Amerged)

This repository contains the conceptual documentation for .NET Aspire. The [.NET Aspire documentation site](https://learn.microsoft.com/dotnet/aspire).

![.NET Aspire](assets/dotnet-aspire.png#gh-light-mode-only)
![.NET Aspire](assets/dotnet-aspire-dark.png#gh-dark-mode-only)

## :purple_heart: Contribute

We welcome contributions to help us improve and complete the .NET docs. This is a very large repo, covering a large area. If this is your first visit, see our [labels and projects roadmap](https://learn.microsoft.com/contribute/content/dotnet/labels-projects) for help navigating the issues and projects in this repository. If your contribution includes third-party dependencies, see our guidance on using [third-party dependencies :link:](https://github.com/dotnet/docs/blob/main/styleguide/3rdPartyDependencies.md).

To contribute, see:

- The [.NET Contributor Guide :ledger:](https://learn.microsoft.com/contribute/dotnet/dotnet-contribute) for instructions on procedures we use.
- Issues labeled [`help wanted` :label:](https://github.com/dotnet/docs-aspire/issues?q=is%3Aopen+is%3Aissue+label%3A%22help+wanted%22+) for ideas.
- [#Hacktoberfest and Microsoft Docs :jack_o_lantern:](https://learn.microsoft.com/contribute/hacktoberfest) for details on our participation in the annual event.

## Microsoft Open Source Code of Conduct

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## :octocat: GitHub Action workflows

- [![Live branch protection](https://github.com/dotnet/docs-aspire/actions/workflows/live-protection.yml/badge.svg)](https://github.com/dotnet/docs-aspire/actions/workflows/live-protection.yml): Adds a comment to PRs that were not automated, but rather manually created that target the `live` branch.
- [![Close stale issues](https://github.com/dotnet/docs-aspire/actions/workflows/stale.yml/badge.svg)](https://github.com/dotnet/docs-aspire/actions/workflows/stale.yml):  Closes stale issues that have not been updated in 180 days.
- [![Markdownlint](https://github.com/dotnet/docs-aspire/actions/workflows/markdownlint.yml/badge.svg)](https://github.com/dotnet/docs-aspire/actions/workflows/markdownlint.yml):  The current status for the entire repositories Markdown linter status.
- [![No response](https://github.com/dotnet/docs-aspire/actions/workflows/no-response.yml/badge.svg)](https://github.com/dotnet/docs-aspire/actions/workflows/no-response.yml):  If an issue is labeled with `needs-more-info` and the op doesn't respond within 14 days, the issue is closed.
- [![OPS status checker](https://github.com/dotnet/docs-aspire/actions/workflows/check-for-build-warnings.yml/badge.svg)](https://github.com/dotnet/docs-aspire/actions/workflows/check-for-build-warnings.yml):  Builds the site for the PR in context, and verifies the build reporting either, `success,` `warnings`, or `error`.
- [![Snippets 5000](https://github.com/dotnet/docs-aspire/actions/workflows/snippets5000.yml/badge.svg)](https://github.com/dotnet/docs-aspire/actions/workflows/snippets5000.yml):  Custom .NET build validation, locates code impacted by a PR, and builds.
- [![Target supported version](https://github.com/dotnet/docs-aspire/actions/workflows/version-sweep.yml/badge.svg)](https://github.com/dotnet/docs-aspire/actions/workflows/version-sweep.yml):  Runs monthly, creating issues on projects that target .NET versions that are out of support.
- [![Update dependabot.yml](https://github.com/dotnet/docs-aspire/actions/workflows/dependabot-bot.yml/badge.svg)](https://github.com/dotnet/docs-aspire/actions/workflows/dependabot-bot.yml):  Automatically updates the `dependabot` configuration weekly, but only if required.
- [![quest import](https://github.com/dotnet/docs-aspire/actions/workflows/quest.yml/badge.svg)](https://github.com/dotnet/docs-aspire/actions/workflows/quest.yml): Automatically synchronizes issues with Quest (Azure DevOps).
- [![bulk quest import](https://github.com/dotnet/docs-aspire/actions/workflows/quest-bulk.yml/badge.svg)](https://github.com/dotnet/docs-aspire/actions/workflows/quest-bulk.yml): Manual bulk import of issues into Quest (Azure DevOps).
