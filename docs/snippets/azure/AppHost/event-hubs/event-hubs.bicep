@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param sku string = 'Standard'

resource event_hubs 'Microsoft.EventHub/namespaces@2024-01-01' = {
  name: take('event_hubs-${uniqueString(resourceGroup().id)}', 256)
  location: location
  properties: {
    disableLocalAuth: true
  }
  sku: {
    name: sku
  }
  tags: {
    'aspire-resource-name': 'event-hubs'
  }
}

resource messages 'Microsoft.EventHub/namespaces/eventhubs@2024-01-01' = {
  name: 'messages'
  parent: event_hubs
}

output eventHubsEndpoint string = event_hubs.properties.serviceBusEndpoint

output name string = event_hubs.name