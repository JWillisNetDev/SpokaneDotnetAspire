using SpokaneDotnetAspire.Data.Models;

namespace SpokaneDotnetAspire.Site.Services;

public interface IMeetupService
{
    Task<List<Meetup>> GetMeetupsAsync(int page = 0, int pageSize = 10, CancellationToken cancellationToken = default);
}

public class MeetupService : IMeetupService
{
    private readonly HttpClient _http;

    public MeetupService(HttpClient http)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
    }

    public async Task<List<Meetup>> GetMeetupsAsync(int page = 0, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var response = await _http.GetAsync($"/meetups?page={page}&pagesize={pageSize}", cancellationToken);
        response.EnsureSuccessStatusCode();

        var meetups = await response.Content.ReadFromJsonAsync<List<Meetup>>(cancellationToken: cancellationToken);
        return meetups ?? [];
    }

    public async Task<Meetup?> GetMeetupAsync(string meetupId, CancellationToken token)
    {
        var response = await _http.GetAsync($"/meetups/{meetupId}", token);
        response.EnsureSuccessStatusCode();

        var meetup = await response.Content.ReadFromJsonAsync<Meetup>(cancellationToken: token);
        return meetup;
    }

}