internal static partial class Program
{
    public static void AddBicepInline(IDistributedApplicationBuilder builder)
    {
        // <addinline>
        builder.AddBicepTemplateString(
                name: "ai",
                bicepContent: """
                @description('That name is the name of our application.')
                param cognitiveServiceName string = 'CognitiveService-${uniqueString(resourceGroup().id)}'

                @description('Location for all resources.')
                param location string = resourceGroup().location

                @allowed([
                  'S0'
                ])
                param sku string = 'S0'

                resource cognitiveService 'Microsoft.CognitiveServices/accounts@2021-10-01' = {
                  name: cognitiveServiceName
                  location: location
                  sku: {
                    name: sku
                  }
                  kind: 'CognitiveServices'
                  properties: {
                    apiProperties: {
                      statisticsEnabled: false
                    }
                  }
                }
                """
            );
        // </addinline>
    }
}
