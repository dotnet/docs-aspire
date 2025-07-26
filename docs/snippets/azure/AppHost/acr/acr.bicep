@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

resource acr 'Microsoft.ContainerRegistry/registries@2025-04-01' = {
  name: take('acr${uniqueString(resourceGroup().id)}', 50)
  location: location
  sku: {
    name: 'Standard'
  }
}

output registryName string = acr.name