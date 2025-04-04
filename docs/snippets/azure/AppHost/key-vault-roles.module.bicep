@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param key_vault_outputs_name string

param principalType string

param principalId string

resource key_vault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: key_vault_outputs_name
}

resource key_vault_KeyVaultSecretsUser 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(key_vault.id, principalId, subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6'))
  properties: {
    principalId: principalId
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6')
    principalType: principalType
  }
  scope: key_vault
}