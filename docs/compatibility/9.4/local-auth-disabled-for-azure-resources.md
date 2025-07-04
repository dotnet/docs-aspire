---
title: "Breaking change - Local auth is disabled by default on Azure resources"
description: "Learn about the breaking change in Aspire 9.4 where local authentication is disabled by default for certain Azure resources."
ms.date: 7/4/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/3723
---

# Local auth is disabled by default on Azure resources

Starting in .NET Aspire 9.4, local authentication is disabled by default for Azure EventHubs and Azure WebPubSub integrations. This change improves security by aligning with Azure environments that reject resources with local authentication enabled.

## Version introduced

.NET Aspire 9.4

## Previous behavior

Previously, Azure EventHubs and Azure WebPubSub resources were created with local authentication enabled by default (`disableLocalAuth = false`).

## New behavior

Now, Azure EventHubs and Azure WebPubSub resources are created with local authentication disabled by default (`disableLocalAuth = true`).

## Type of breaking change

This is a [behavioral change](../categories.md#behavioral-change).

## Reason for change

Disabling local authentication by default provides a more secure configuration. Some Azure environments reject resources with local authentication enabled, and this change ensures compatibility with those environments.

## Recommended action

If you are using the .NET Aspire client integrations for these services, no changes are required, and your application will continue to function as expected.

If you're using a SAS token or other connection string with an access key, you must either:

1. Re-enable local authentication using the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure*> method.
1. Update your application to use Entra ID authentication.

### Example: Re-enabling local authentication

1. In the corresponding Azure resource, chain a call to `ConfigureInfrastructure`.
1. Get the instance of the provisioning resource type in question, for example:

    - Azure Event Hubs: <xref:Azure.Provisioning.EventHubs.EventHubsNamespace.DisableLocalAuth?displayProperty=fullName>. For more information on configuring infra, see [Customize provisioning infrastructure](../../messaging/azure-event-hubs-integration.md#customize-provisioning-infrastructure).
    - Azure Web PubSub: <xref:Azure.Provisioning.WebPubSub.WebPubSubService.IsLocalAuthDisabled?displayProperty=fullName>. For more information on configuring infra, see [Customize provisioning infrastructure](../../messaging/azure-web-pubsub-integration.md#customize-provisioning-infrastructure).

## Affected APIs

- `AddAzureEventHubs`
- `AddAzureWebPubSub`
