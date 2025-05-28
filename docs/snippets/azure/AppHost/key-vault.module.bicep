@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

resource key_vault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: take('keyvault-${uniqueString(resourceGroup().id)}', 24)
  location: location
  properties: {
    tenantId: tenant().tenantId
    sku: {
      family: 'A'
      name: 'standard'
    }
    enableRbacAuthorization: true
  }
  tags: {
    'aspire-resource-name': 'key-vault'
  }
}

output vaultUri string = key_vault.properties.vaultUri

output name string = key_vault.name