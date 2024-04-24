using System.ComponentModel.DataAnnotations;

namespace SpokaneDotnetAspire.Site.Services;

public class SpokaneDotnetAspireServiceOptions
{
    [Required]
    public required Uri BaseUrl { get; set; }
}