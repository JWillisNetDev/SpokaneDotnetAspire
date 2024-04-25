using Microsoft.AspNetCore.Components.Forms;

using SpokaneDotnetAspire.Data.Dtos.Model;
using SpokaneDotnetAspire.Data.Models;

namespace SpokaneDotnetAspire.Site.Services;

public interface IMeetupService
{
    Task<List<Meetup>> GetMeetupsAsync(int page = 0, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<Meetup?> GetMeetupAsync(string meetupId, CancellationToken cancellationToken = default);
    Task<Meetup> CreateMeetupWithImageAsync(CreateMeetupDto dto,
        IBrowserFile file,
        CancellationToken cancellationToken = default);
    Task<Meetup> CreateMeetupAsync(CreateMeetupDto dto, CancellationToken cancellationToken = default);
    Task<Meetup> UpdateMeetupAsync(CancellationToken cancellationToken = default);
    Task DeleteMeetupAsync(CancellationToken cancellationToken = default);
}