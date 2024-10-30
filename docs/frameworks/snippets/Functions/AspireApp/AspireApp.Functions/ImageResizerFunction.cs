namespace AspireApp.Functions;

public sealed class ImageResizerFunction(
    ILoggerFactory loggerFactory,
    QueueServiceClient queueServiceClient,
    BlobServiceClient blobServiceClient)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger(
        LogCategories.CreateFunctionUserCategory(typeof(ImageResizerFunction).FullName));

    [Function(nameof(ImageResizerFunction))]
    public async Task Resize(
        [BlobTrigger("images/{name}", Connection = "blobs")] Stream stream, string name)
    {
        try
        {
            using var resizedStream = GetResizedImageStream(name, stream, SKEncodedImageFormat.Jpeg);

            await UploadResizedImageAsync(name, resizedStream);

            await SendQueueMessageAsync(name);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Error generating thumbnail for image: {Name}. Exception: {Message}",
                name, ex.Message);
        }
    }

    private MemoryStream GetResizedImageStream(
        string name, Stream stream, SKEncodedImageFormat format, int targetHeight = 128)
    {
        using var originalBitmap = SKBitmap.Decode(stream);

        var scale = (float)targetHeight / originalBitmap.Height;
        var targetWidth = (int)(originalBitmap.Width * scale);

        using var resizedBitmap = originalBitmap.Resize(
            new SKImageInfo(targetWidth, targetHeight), SKFilterQuality.High);

        using var image = SKImage.FromBitmap(resizedBitmap);

        // Do not put in a using, as this is returned to the caller.
        var resizedStream = new MemoryStream();

        image.Encode(format, 100).SaveTo(resizedStream);

        _logger.LogInformation(
            "Resized image {Name} from {OriginalWidth}x{OriginalHeight} to {Width}x{Height}.",
            name, originalBitmap.Width, originalBitmap.Height, targetWidth, targetHeight);

        return resizedStream;
    }

    private async Task UploadResizedImageAsync(string name, MemoryStream resizedStream)
    {
        resizedStream.Position = 0;

        var containerClient = blobServiceClient.GetBlobContainerClient("thumbnails");

        var blobClient = containerClient.GetBlobClient(name);

        _logger.LogInformation("Uploading {Name}", name);

        _ = await blobClient.UploadAsync(resizedStream, overwrite: true);

        _logger.LogInformation("Uploaded {Name}", name);
    }

    private async Task SendQueueMessageAsync(string name)
    {
        var resultsQueueClient = queueServiceClient.GetQueueClient("thumbnail-queue");

        var jsonMessage = JsonSerializer.Serialize(
            new UploadResult(name, true), SerializationContext.Default.UploadResult);

        _logger.LogInformation("Signaling upload of {Name}", name);

        _ = await resultsQueueClient.SendMessageAsync(jsonMessage);

        _logger.LogInformation("Signaled upload of {Name}", name);
    }
}
