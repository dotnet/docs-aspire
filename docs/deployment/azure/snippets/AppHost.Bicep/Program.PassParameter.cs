using Aspire.Hosting;
using Aspire.Hosting.Azure;

internal static partial class Program
{
    public static void AddParameterToBicepReference(IDistributedApplicationBuilder builder)
    {
        // <addparameter>
        var region = builder.AddParameter("region");

        builder.AddBicepTemplate("storage", "../infra/storage.bicep")
               .WithParameter("region", region)
               .WithParameter("storageName", "app-storage")
               .WithParameter("tags", ["latest","dev"]);
        // </addparameter>

        // <addwellknownparams>
        var webHookApi = builder.AddProject<Projects.WebHook_Api>("webhook-api");

        var webHookEndpointExpression = ReferenceExpression.Create(
                $"{webHookApi.GetEndpoint("https")}/hook");

        builder.AddBicepTemplate("event-grid-webhook", "../infra/event-grid-webhook.bicep")
               .WithParameter("topicName", "events")
               .WithParameter(AzureBicepResource.KnownParameters.PrincipalId)
               .WithParameter(AzureBicepResource.KnownParameters.PrincipalType)
               .WithParameter("webHookEndpoint", () => webHookEndpointExpression);
        // </addwellknownparams>
    }
}
