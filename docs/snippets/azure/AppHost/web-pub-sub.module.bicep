@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param sku string = 'Free_F1'

param capacity int = 1

param principalType string

param principalId string

resource web_pub_sub 'Microsoft.SignalRService/webPubSub@2024-03-01' = {
  name: take('webpubsub-${uniqueString(resourceGroup().id)}', 63)
  location: location
  sku: {
    name: sku
    capacity: capacity
  }
  tags: {
    'aspire-resource-name': 'web-pub-sub'
  }
}

resource web_pub_sub_WebPubSubServiceOwner 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(web_pub_sub.id, principalId, subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '12cf5a90-567b-43ae-8102-96cf46c7d9b4'))
  properties: {
    principalId: principalId
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '12cf5a90-567b-43ae-8102-96cf46c7d9b4')
    principalType: principalType
  }
  scope: web_pub_sub
}

output endpoint string = 'https://${web_pub_sub.properties.hostName}'