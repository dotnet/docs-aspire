@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

resource config 'Microsoft.AppConfiguration/configurationStores@2024-06-01' = {
  name: take('config-${uniqueString(resourceGroup().id)}', 50)
  location: location
  properties: {
    disableLocalAuth: true
  }
  sku: {
    name: 'standard'
  }
  tags: {
    'aspire-resource-name': 'config'
  }
}

output appConfigEndpoint string = config.properties.endpoint

output name string = config.name