targetScope = 'subscription'

param resourceGroupName string

param location string

param principalId string

resource rg 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: resourceGroupName
  location: location
}

module acr 'acr/acr.bicep' = {
  name: 'acr'
  scope: rg
  params: {
    location: location
  }
}

module ai_foundry 'ai-foundry/ai-foundry.bicep' = {
  name: 'ai-foundry'
  scope: rg
  params: {
    location: location
  }
}

module config 'config/config.bicep' = {
  name: 'config'
  scope: rg
  params: {
    location: location
  }
}

module app_insights 'app-insights/app-insights.bicep' = {
  name: 'app-insights'
  scope: rg
  params: {
    location: location
  }
}

module cosmos 'cosmos/cosmos.bicep' = {
  name: 'cosmos'
  scope: rg
  params: {
    location: location
  }
}

module event_hubs 'event-hubs/event-hubs.bicep' = {
  name: 'event-hubs'
  scope: rg
  params: {
    location: location
  }
}

module key_vault 'key-vault/key-vault.bicep' = {
  name: 'key-vault'
  scope: rg
  params: {
    location: location
  }
}

module log_analytics_workspace 'log-analytics-workspace/log-analytics-workspace.bicep' = {
  name: 'log-analytics-workspace'
  scope: rg
  params: {
    location: location
  }
}

module openai 'openai/openai.bicep' = {
  name: 'openai'
  scope: rg
  params: {
    location: location
  }
}

module postgres_flexible 'postgres-flexible/postgres-flexible.bicep' = {
  name: 'postgres-flexible'
  scope: rg
  params: {
    location: location
  }
}

module redis 'redis/redis.bicep' = {
  name: 'redis'
  scope: rg
  params: {
    location: location
  }
}

module search 'search/search.bicep' = {
  name: 'search'
  scope: rg
  params: {
    location: location
  }
}

module service_bus 'service-bus/service-bus.bicep' = {
  name: 'service-bus'
  scope: rg
  params: {
    location: location
  }
}

module signalr 'signalr/signalr.bicep' = {
  name: 'signalr'
  scope: rg
  params: {
    location: location
  }
}

module sql 'sql/sql.bicep' = {
  name: 'sql'
  scope: rg
  params: {
    location: location
  }
}

module storage 'storage/storage.bicep' = {
  name: 'storage'
  scope: rg
  params: {
    location: location
  }
}

module web_pubsub 'web-pubsub/web-pubsub.bicep' = {
  name: 'web-pubsub'
  scope: rg
  params: {
    location: location
    messages_url_0: '/eventhandler/'
  }
}

module ai_foundry_roles 'ai-foundry-roles/ai-foundry-roles.bicep' = {
  name: 'ai-foundry-roles'
  scope: rg
  params: {
    location: location
    ai_foundry_outputs_name: ai_foundry.outputs.name
    principalType: ''
    principalId: ''
  }
}

module config_roles 'config-roles/config-roles.bicep' = {
  name: 'config-roles'
  scope: rg
  params: {
    location: location
    config_outputs_name: config.outputs.name
    principalType: ''
    principalId: ''
  }
}

module cosmos_roles 'cosmos-roles/cosmos-roles.bicep' = {
  name: 'cosmos-roles'
  scope: rg
  params: {
    location: location
    cosmos_outputs_name: cosmos.outputs.name
    principalId: ''
  }
}

module event_hubs_roles 'event-hubs-roles/event-hubs-roles.bicep' = {
  name: 'event-hubs-roles'
  scope: rg
  params: {
    location: location
    event_hubs_outputs_name: event_hubs.outputs.name
    principalType: ''
    principalId: ''
  }
}

module key_vault_roles 'key-vault-roles/key-vault-roles.bicep' = {
  name: 'key-vault-roles'
  scope: rg
  params: {
    location: location
    key_vault_outputs_name: key_vault.outputs.name
    principalType: ''
    principalId: ''
  }
}

module openai_roles 'openai-roles/openai-roles.bicep' = {
  name: 'openai-roles'
  scope: rg
  params: {
    location: location
    openai_outputs_name: openai.outputs.name
    principalType: ''
    principalId: ''
  }
}

module postgres_flexible_roles 'postgres-flexible-roles/postgres-flexible-roles.bicep' = {
  name: 'postgres-flexible-roles'
  scope: rg
  params: {
    location: location
    postgres_flexible_outputs_name: postgres_flexible.outputs.name
    principalType: ''
    principalId: ''
    principalName: ''
  }
}

module redis_roles 'redis-roles/redis-roles.bicep' = {
  name: 'redis-roles'
  scope: rg
  params: {
    location: location
    redis_outputs_name: redis.outputs.name
    principalId: ''
    principalName: ''
  }
}

module search_roles 'search-roles/search-roles.bicep' = {
  name: 'search-roles'
  scope: rg
  params: {
    location: location
    search_outputs_name: search.outputs.name
    principalType: ''
    principalId: ''
  }
}

module service_bus_roles 'service-bus-roles/service-bus-roles.bicep' = {
  name: 'service-bus-roles'
  scope: rg
  params: {
    location: location
    service_bus_outputs_name: service_bus.outputs.name
    principalType: ''
    principalId: ''
  }
}

module signalr_roles 'signalr-roles/signalr-roles.bicep' = {
  name: 'signalr-roles'
  scope: rg
  params: {
    location: location
    signalr_outputs_name: signalr.outputs.name
    principalType: ''
    principalId: ''
  }
}

module sql_roles 'sql-roles/sql-roles.bicep' = {
  name: 'sql-roles'
  scope: rg
  params: {
    location: location
    sql_outputs_name: sql.outputs.name
    sql_outputs_sqlserveradminname: sql.outputs.sqlServerAdminName
    principalId: ''
    principalName: ''
  }
}

module storage_roles 'storage-roles/storage-roles.bicep' = {
  name: 'storage-roles'
  scope: rg
  params: {
    location: location
    storage_outputs_name: storage.outputs.name
    principalType: ''
    principalId: ''
  }
}

module web_pubsub_roles 'web-pubsub-roles/web-pubsub-roles.bicep' = {
  name: 'web-pubsub-roles'
  scope: rg
  params: {
    location: location
    web_pubsub_outputs_name: web_pubsub.outputs.name
    principalType: ''
    principalId: ''
  }
}