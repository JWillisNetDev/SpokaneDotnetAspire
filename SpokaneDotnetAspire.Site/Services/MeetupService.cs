using System.Net.Http.Headers;

using Microsoft.AspNetCore.Components.Forms;

using SpokaneDotnetAspire.Data.Dtos.Model;
using SpokaneDotnetAspire.Data.Models;

namespace SpokaneDotnetAspire.Site.Services;

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

    public async Task<Meetup?> GetMeetupAsync(string meetupId, CancellationToken cancellationToken = default)
    {
        var response = await _http.GetAsync($"/meetups/{meetupId}", cancellationToken);
        response.EnsureSuccessStatusCode();

        var meetup = await response.Content.ReadFromJsonAsync<Meetup>(cancellationToken: cancellationToken);
        return meetup;
    }

    public async Task<Meetup> CreateMeetupWithImageAsync(CreateMeetupDto dto,
        IBrowserFile file,
        CancellationToken cancellationToken = default)
    {
        // First, we want to upload the file.
        var imageContent = new StreamContent(file.OpenReadStream());
        imageContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        var fileUploadForm = new MultipartFormDataContent
        {
            { imageContent, "file", file.Name }
        };

        var imageUriResponse = await _http.PostAsync("/image-upload", fileUploadForm, cancellationToken);
        imageUriResponse.EnsureSuccessStatusCode();

        var imageUri = await imageUriResponse.Content.ReadFromJsonAsync<Uri>(cancellationToken: cancellationToken);

        // Now, we want to create the meetup.
        dto = dto with { ImageUri = imageUri };

        var createMeetupResponse = await _http.PostAsJsonAsync("/meetups/create", dto, cancellationToken);
        createMeetupResponse.EnsureSuccessStatusCode();

        // Finally, we get the created meetup from the response location.
        var getMeetupResponse = await _http.GetAsync(createMeetupResponse.Headers.Location, cancellationToken);
        getMeetupResponse.EnsureSuccessStatusCode();

        var meetup = await getMeetupResponse.Content.ReadFromJsonAsync<Meetup>(cancellationToken: cancellationToken);
        return meetup!;
    }

    public async Task<Meetup> CreateMeetupAsync(CreateMeetupDto dto, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Meetup> UpdateMeetupAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    public async Task DeleteMeetupAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}