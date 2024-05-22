1. On the resources page, in the **aspireweb** row, click the link in the **Endpoints** column to open the Swagger UI page of your API.
1. On the .NET Aspire dashboard, navigate to the logs for the **AspireWorkerService** project.
1. Back on the Swagger UI page, expand the **/notify** endpoint and select **Try it out**.
1. Enter a test message in the **message** input box.
1. Select **Execute** to send a test request.
1. Switch back to the **AspireWorkerService** logs. You should see the test message printed in the output logs.