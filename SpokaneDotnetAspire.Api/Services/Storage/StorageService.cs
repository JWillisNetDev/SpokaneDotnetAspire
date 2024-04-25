using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using Microsoft.Extensions.Options;

namespace SpokaneDotnetAspire.Api.Services.Storage;

public class StorageService : IStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IOptions<StorageOptions> _options;
    private readonly BlobContainerClient _imageContainerClient;

    public StorageService(BlobServiceClient blobServiceClient, IOptions<StorageOptions> options)
    {
        _blobServiceClient = blobServiceClient ?? throw new ArgumentNullException(nameof(blobServiceClient));
        _options = options ?? throw new ArgumentNullException(nameof(options));

        _imageContainerClient = _blobServiceClient.GetBlobContainerClient(_options.Value.ImageContainerName);
    }

    public async Task EnsureContainersCreatedAsync(CancellationToken cancellationToken = default)
        => await _imageContainerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer, cancellationToken: cancellationToken);

    public async Task<Uri> UploadImageAsync(string imageFileName,
        BinaryData imageBinaryData,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(imageFileName);
        ArgumentNullException.ThrowIfNull(imageBinaryData);

        if (Path.GetExtension(imageFileName).Trim() is not { Length: > 0 } extension)
        {
            throw new ArgumentException("The image file name must have an extension.", nameof(imageFileName));
        }

        string blobName = Path.ChangeExtension(
            imageFileName,
            $"{Guid.NewGuid()}{extension}");

        var blob = _imageContainerClient.GetBlobClient(blobName);
        await blob.UploadAsync(imageBinaryData, cancellationToken: cancellationToken);

        return blob.Uri;
    }

    public async Task DeleteImageAsync(Uri blobUri, CancellationToken cancellationToken = default)
    {
        BlobClient blobClient = new(blobUri);
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }
}