## Deploy the .NET Aspire Dashboard

You can deploy the .NET Aspire dashboard as part of your hosted app. This feature is currently in alpha support, so you must enable the `alpha.aspire.dashboard` [feature flag](/azure/developer/azure-developer-cli/feature-versioning). When enabled, the `azd` output logs print an additional URL to the deployed dashboard.

```azdeveloper
azd config set alpha.aspire.dashboard on
```

You can also run `azd monitor` to automatically launch the dashboard.

```azdeveloper
azd monitor
```
