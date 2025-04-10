param topicName string
param webHookEndpoint string
param principalId string
param principalType string
param location string = resourceGroup().location

// The topic name must be unique because it's represented by a DNS entry. 
// must be between 3-50 characters and contain only values a-z, A-Z, 0-9, and "-".

resource topic 'Microsoft.EventGrid/topics@2023-12-15-preview' = {
  name: toLower(take('${topicName}${uniqueString(resourceGroup().id)}', 50))
  location: location

  resource eventSubscription 'eventSubscriptions' = {
    name: 'customSub'
    properties: {
      destination: {
        endpointType: 'WebHook'
        properties: {
          endpointUrl: webHookEndpoint
        }
      }
    }
  }
}

resource EventGridRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(topic.id, principalId, subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'd5a91429-5739-47e2-a06b-3470a27159e7'))
  scope: topic
  properties: {
    principalId: principalId
    principalType: principalType
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'd5a91429-5739-47e2-a06b-3470a27159e7')
  }
}

output endpoint string = topic.properties.endpoint
