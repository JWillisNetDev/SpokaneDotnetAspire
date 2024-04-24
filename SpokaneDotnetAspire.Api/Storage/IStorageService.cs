namespace SpokaneDotnetAspire.Api.Storage;

public interface IStorageService
{
    public Task EnsureContainersCreatedAsync(CancellationToken cancellationToken = default);
    public Task<Uri?> UploadImageAsync(
        string imageFileName,
        BinaryData imageBinaryData,
        CancellationToken cancellationToken = default);
    public Task DeleteImageAsync(Uri blobUri, CancellationToken cancellationToken = default);
}