namespace AspireApp.Web.Components.Pages;

public sealed partial class Home(
    [FromKeyedServices("images")] BlobContainerClient imagesContainerClient,
    [FromKeyedServices("thumbnails")] BlobContainerClient thumbsContainerClient,
    QueueMessageHandler queueMessageHandler,
    ILogger<Home> logger)
{
    private const long UploadFileSizeLimit = 512_000; // 512 KB

    private readonly HashSet<ImageViewModel> _images = [];

    private bool _isUploading;
    private bool _isDialogOpen;
    private string _dialogMessage = "";
    private string _selectedImage = "";
    private Guid _fileUploadKey = Guid.CreateVersion7();

    private Task OnMessageReceivedAsync(UploadResult uploadResult) => InvokeAsync(LoadBlobsAsync);

    private void PreviewImage(string? imageUrl)
    {
        if (imageUrl is not null)
        {
            _selectedImage = imageUrl;
            _isDialogOpen = true;
            StateHasChanged();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        logger.LogInformation("Subscribing to message handler.");

        await LoadBlobsAsync();

        queueMessageHandler.MessageReceived += OnMessageReceivedAsync;
    }

    private async Task LoadBlobsAsync()
    {
        try
        {
            logger.LogInformation("Loading blobs...");

            await foreach (var blobItem in thumbsContainerClient.GetBlobsAsync())
            {
                var imageBlobClient = imagesContainerClient.GetBlobClient(blobItem.Name);
                var thumbBlobClient = thumbsContainerClient.GetBlobClient(blobItem.Name);

                _images.Add(new ImageViewModel(
                    ImageUrl: imageBlobClient.Uri.AbsoluteUri,
                    ThumbnailUrl: thumbBlobClient.Uri.AbsoluteUri));
            }
        }
        finally
        {
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task OnFilesChangedAsync(InputFileChangeEventArgs e)
    {
        _isUploading = true;

        try
        {
            foreach (var file in e.GetMultipleFiles())
            {
                if (!file.ContentType.StartsWith("image/"))
                {
                    _dialogMessage = $"File {file.Name} is not a valid image.";
                    _isDialogOpen = true;

                    continue;
                }

                if (file is { Size: > UploadFileSizeLimit })
                {
                    _dialogMessage = $"File {file.Name} exceeds the size limit of {UploadFileSizeLimit} bytes.";
                    _isDialogOpen = true;

                    continue;
                }

                logger.LogInformation("Uploading {Name}", file.Name);

                var blobClient = imagesContainerClient.GetBlobClient(file.Name);

                using var stream = file.OpenReadStream();

                await blobClient.UploadAsync(stream, overwrite: true);

                logger.LogInformation("Uploaded {Name}", file.Name);
            }
        }
        finally
        {
            _isUploading = false;
            _fileUploadKey = Guid.CreateVersion7();

            await InvokeAsync(StateHasChanged);
        }
    }

    internal sealed record class ImageViewModel(
        string? ImageUrl,
        string? ThumbnailUrl);
}
