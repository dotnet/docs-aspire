@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param sku string = 'Free_F1'

param capacity int = 1

param messages_url_0 string

resource web_pubsub 'Microsoft.SignalRService/webPubSub@2024-03-01' = {
  name: take('webpubsub-${uniqueString(resourceGroup().id)}', 63)
  location: location
  properties: {
    disableLocalAuth: true
  }
  sku: {
    name: sku
    capacity: capacity
  }
  tags: {
    'aspire-resource-name': 'web-pubsub'
  }
}

resource messages 'Microsoft.SignalRService/webPubSub/hubs@2024-03-01' = {
  name: 'messages'
  properties: {
    eventHandlers: [
      {
        urlTemplate: messages_url_0
        userEventPattern: '*'
        systemEvents: [
          'connected'
        ]
      }
    ]
  }
  parent: web_pubsub
}

output endpoint string = 'https://${web_pubsub.properties.hostName}'

output name string = web_pubsub.name