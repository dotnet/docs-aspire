@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param sku string = 'Standard'

param principalType string

param principalId string

resource event_hubs 'Microsoft.EventHub/namespaces@2024-01-01' = {
  name: take('event_hubs-${uniqueString(resourceGroup().id)}', 256)
  location: location
  sku: {
    name: sku
  }
  tags: {
    'aspire-resource-name': 'event-hubs'
  }
}

resource event_hubs_AzureEventHubsDataOwner 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(event_hubs.id, principalId, subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'f526a384-b230-433a-b45c-95f59c4a2dec'))
  properties: {
    principalId: principalId
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'f526a384-b230-433a-b45c-95f59c4a2dec')
    principalType: principalType
  }
  scope: event_hubs
}

output eventHubsEndpoint string = event_hubs.properties.serviceBusEndpoint