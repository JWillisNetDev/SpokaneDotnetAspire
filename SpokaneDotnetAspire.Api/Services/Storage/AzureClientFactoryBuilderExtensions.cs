using Azure.Core.Extensions;
using Azure.Storage.Blobs;

using Microsoft.Extensions.Azure;

using SpokaneDotnetAspire.Api.Services.Storage;

namespace SpokaneDotnetAspire.Api.Services.Storage;

internal static class AzureClientFactoryBuilderExtensions
{
    public static IAzureClientBuilder<BlobServiceClient, BlobClientOptions> AddBlobServiceClient(this AzureClientFactoryBuilder builder, string serviceUriOrConnectionString)
    {
        if (Uri.TryCreate(serviceUriOrConnectionString, UriKind.Absolute, out Uri? serviceUri))
        {
            return builder.AddBlobServiceClient(serviceUri);
        }
        return builder.AddBlobServiceClient(connectionString: serviceUriOrConnectionString);
    }
}
