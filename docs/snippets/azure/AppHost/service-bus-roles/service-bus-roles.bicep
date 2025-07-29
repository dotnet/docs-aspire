@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param service_bus_outputs_name string

param principalType string

param principalId string

resource service_bus 'Microsoft.ServiceBus/namespaces@2024-01-01' existing = {
  name: service_bus_outputs_name
}

resource service_bus_AzureServiceBusDataOwner 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(service_bus.id, principalId, subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '090c5cfd-751d-490a-894a-3ce6f1109419'))
  properties: {
    principalId: principalId
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '090c5cfd-751d-490a-894a-3ce6f1109419')
    principalType: principalType
  }
  scope: service_bus
}