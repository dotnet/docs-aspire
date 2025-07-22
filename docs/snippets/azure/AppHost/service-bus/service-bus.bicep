@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param sku string = 'Standard'

resource service_bus 'Microsoft.ServiceBus/namespaces@2024-01-01' = {
  name: take('servicebus-${uniqueString(resourceGroup().id)}', 50)
  location: location
  properties: {
    disableLocalAuth: true
  }
  sku: {
    name: sku
  }
  tags: {
    'aspire-resource-name': 'service-bus'
  }
}

output serviceBusEndpoint string = service_bus.properties.serviceBusEndpoint

output name string = service_bus.name