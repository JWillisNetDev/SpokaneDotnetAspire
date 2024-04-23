using System.ComponentModel.DataAnnotations;

namespace SpokaneDotnetAspire.Api.Storage;

public class StorageOptions
{
    internal const string ConfigurationSource = "StorageOptions";

    [Required]
    public required string ImageContainerName { get; set; }
}