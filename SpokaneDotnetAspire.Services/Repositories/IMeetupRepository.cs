using SpokaneDotnetAspire.Services.Data.Models;

namespace SpokaneDotnetAspire.Services.Repositories;

public interface IMeetupRepository
{
	public Task<IList<Meetup>> GetMeetupsAsync(CancellationToken cancellationToken = default);
	public Task<Meetup?> GetMeetupByIdAsync(string id, CancellationToken cancellationToken = default);
	public Task<Result<string>> CreateMeetupAsync(string title, string content, string? url, CancellationToken cancellationToken = default);
	public Task<Result<Meetup, string>> RemoveMeetupAsync(string id, CancellationToken cancellationToken = default);
	public Task<Result<Meetup, string>> UpdateMeetupAsync(
		string id,
		Option<string> title,
		Option<string> content,
		Option<string?> url,
		CancellationToken cancellationToken = default);
}