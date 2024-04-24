using Azure.Core.Extensions;
using Azure.Storage.Blobs;

using Microsoft.Extensions.Azure;

namespace SpokaneDotnetAspire.Api.Storage;

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
