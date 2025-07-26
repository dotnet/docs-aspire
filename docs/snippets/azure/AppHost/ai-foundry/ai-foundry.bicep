@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

resource ai_foundry 'Microsoft.CognitiveServices/accounts@2024-10-01' = {
  name: take('aifoundry-${uniqueString(resourceGroup().id)}', 64)
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  kind: 'AIServices'
  properties: {
    customSubDomainName: toLower(take(concat('ai-foundry', uniqueString(resourceGroup().id)), 24))
    publicNetworkAccess: 'Enabled'
    disableLocalAuth: true
  }
  sku: {
    name: 'S0'
  }
  tags: {
    'aspire-resource-name': 'ai-foundry'
  }
}

resource chat 'Microsoft.CognitiveServices/accounts/deployments@2024-10-01' = {
  name: 'Phi-4'
  properties: {
    model: {
      format: 'Microsoft'
      name: 'Phi-4'
      version: '1'
    }
  }
  sku: {
    name: 'GlobalStandard'
    capacity: 1
  }
  parent: ai_foundry
}

output aiFoundryApiEndpoint string = ai_foundry.properties.endpoints['AI Foundry API']

output endpoint string = ai_foundry.properties.endpoint

output name string = ai_foundry.name