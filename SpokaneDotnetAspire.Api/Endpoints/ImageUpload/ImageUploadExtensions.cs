namespace SpokaneDotnetAspire.Api.Endpoints.ImageUpload;

public static class ImageUploadExtensions
{
    public static IEndpointRouteBuilder MapImageUploadEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/image-upload");

        group.MapPost("/", ImageUploadMethods.UploadImageAsync)
            .WithName("UploadImage")
            .DisableAntiforgery();

        return builder;
    }
}