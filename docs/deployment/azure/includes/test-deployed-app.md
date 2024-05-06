## Test the deployed app

Now that the app has been provisioned and deployed, you can browse to the Azure portal. In the resource group where you deployed the app, you'll see the three container apps and other resources.

:::image type="content" source="../media/azd-azure-portal-deployed-resources.png" lightbox="../media/azd-azure-portal-deployed-resources.png" alt-text="A screenshot of the .NET Aspire app's resource group in the Azure portal.":::

Click on the `web` Container App to open it up in the portal.

:::image type="content" source="../../../media/portal-screens-web-container-app.png" lightbox="../../../media/portal-screens-web-container-app.png" alt-text="A screenshot of the .NET Aspire app's front end in the Azure portal.":::

Click the **Application URL** link to open the front end in the browser.

:::image type="content" source="../../../media/front-end-open.png" lightbox="../../../media/front-end-open.png" alt-text="A screenshot of the .NET Aspire app's front end in the browser.":::

When you click the "Weather" node in the navigation bar, the front end `web` container app makes a call to the `apiservice` container app to get data. The front end's output will be cached using the `redis` container app and the [.NET Aspire Redis Output Caching component](../../../caching/stackexchange-redis-output-caching-component.md). As you refresh the front end a few times, you'll notice that the weather data is cached. It will update after a few seconds.
