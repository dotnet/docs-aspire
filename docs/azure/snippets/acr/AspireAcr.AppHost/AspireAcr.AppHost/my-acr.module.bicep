@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

resource my_acr 'Microsoft.ContainerRegistry/registries@2023-07-01' = {
  name: take('myacr${uniqueString(resourceGroup().id)}', 50)
  location: location
  sku: {
    name: 'Basic'
  }
  tags: {
    'aspire-resource-name': 'my-acr'
  }
}

output name string = my_acr.name

output loginServer string = my_acr.properties.loginServer