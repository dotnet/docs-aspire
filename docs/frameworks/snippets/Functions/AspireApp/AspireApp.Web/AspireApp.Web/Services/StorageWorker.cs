﻿namespace AspireApp.Web.Services;

public sealed class StorageWorker(
    QueueClient thumbnailResultsQueueClient,
    [FromKeyedServices("images")] BlobContainerClient imagesContainerClient,
    [FromKeyedServices("thumbnails")] BlobContainerClient thumbsContainerClient,
    QueueMessageHandler handler,
    ILogger<StorageWorker> logger) : BackgroundService
{
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting storage worker.");

        await Task.WhenAll(
            thumbnailResultsQueueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken),
            imagesContainerClient.CreateIfNotExistsAsync(publicAccessType: PublicAccessType.Blob, cancellationToken: cancellationToken),
            thumbsContainerClient.CreateIfNotExistsAsync(publicAccessType: PublicAccessType.Blob, cancellationToken: cancellationToken));

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var message = await thumbnailResultsQueueClient.ReceiveMessageAsync(
                    TimeSpan.FromSeconds(1), stoppingToken);

                if (message is null or { Value: null })
                {
                    logger.LogInformation(
                        "Message received but was either null or without value.");

                    continue;
                }

                var result = JsonSerializer.Deserialize(
                    message.Value.Body.ToString(), SerializationContext.Default.UploadResult);

                if (result is null or { IsSuccessful: false })
                {
                    logger.LogInformation(
                        "Message upload result was either null or unsuccessful for {Name}.",
                        result?.Name);

                    continue;
                }

                logger.LogInformation(
                        "Relaying message of a successful upload...");

                await handler.OnMessageReceivedAsync(result);

                await thumbnailResultsQueueClient.DeleteMessageAsync(
                    message.Value.MessageId, message.Value.PopReceipt, stoppingToken);
            }
            finally
            {
                await Task.Delay(7_500, stoppingToken);
            }
        }
    }
}

public sealed class QueueMessageHandler
{
    public event Func<UploadResult, Task>? MessageReceived;

    public Task OnMessageReceivedAsync(UploadResult? result)
    {
        if (result is null)
        {
            return Task.CompletedTask;
        }

        return MessageReceived?.Invoke(result) ?? Task.CompletedTask;
    }
}
