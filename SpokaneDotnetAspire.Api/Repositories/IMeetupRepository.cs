using SpokaneDotnetAspire.Data;
using SpokaneDotnetAspire.Data.Models;

namespace SpokaneDotnetAspire.Api.Repositories;

public interface IMeetupRepository
{
    public Task<IList<Meetup>> GetMeetupsAsync(int skip, int take, CancellationToken cancellationToken = default);
    public Task<Meetup?> GetMeetupByIdAsync(string id, CancellationToken cancellationToken = default);
    public Task<Result<Meetup, string>> CreateMeetupAsync(string title,
        string content,
        string? url,
        Uri? imageUri,
        CancellationToken cancellationToken = default);
    public Task<Result<Meetup, string>> DeleteMeetupAsync(string id, CancellationToken cancellationToken = default);
    public Task<Result<Meetup, string>> UpdateMeetupAsync(
        string id,
        Option<string> title,
        Option<string> content,
        Option<string?> url,
        CancellationToken cancellationToken = default);
}