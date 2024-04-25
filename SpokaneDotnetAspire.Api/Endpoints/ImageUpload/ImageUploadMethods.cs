using Microsoft.AspNetCore.Http.HttpResults;

using SpokaneDotnetAspire.Api.Services.Storage;

namespace SpokaneDotnetAspire.Api.Endpoints.ImageUpload;

public sealed class ImageUploadMethods
{
    public static async Task<Results<Ok<string>, BadRequest<string>>> UploadImageAsync(
        IFormFile file,
        IStorageService storageService,
        ILogger<ImageUploadMethods> logger,
        CancellationToken cancellationToken)
    {
        if (file is null)
        {
            return TypedResults.BadRequest("A file is required for upload, found null.");
        }

        if (!file.ContentType.Contains("image"))
        {
            return TypedResults.BadRequest("The file must be an image.");
        }

        BinaryData imageBinaryData = await BinaryData.FromStreamAsync(file.OpenReadStream(), cancellationToken);
        Uri blobUri = await storageService.UploadImageAsync(file.FileName, imageBinaryData, cancellationToken);
        logger.LogInformation("Created image in storage with uri {blobUri} at {at}",
            blobUri, DateTimeOffset.UtcNow);

        return TypedResults.Ok(blobUri.AbsoluteUri);
    }
}