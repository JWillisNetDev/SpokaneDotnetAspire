using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Options;

namespace SpokaneDotnetAspire.Site.Services;

public class MeetupsService
{
    private readonly HttpClient _http;
    private readonly IOptions<SpokaneDotnetAspireServiceOptions> _options;

    public MeetupsService(
        IHttpClientFactory httpClientFactory,
        IOptions<SpokaneDotnetAspireServiceOptions> options)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _http = httpClientFactory.CreateClient();
    }

    //public async Task<List<MeetupDto>>
}

public record MeetupDto(
    string Id,
    string Title,
    string Content,
    string? MeetupUrl,
    Uri? ImageUri);

public record UpdateMeetupDto(
    string Title,
    string Content,
    string? MeetupUrl);

public class SpokaneDotnetAspireServiceOptions
{
    [Required]
    public required string BaseUrl { get; set; }
}