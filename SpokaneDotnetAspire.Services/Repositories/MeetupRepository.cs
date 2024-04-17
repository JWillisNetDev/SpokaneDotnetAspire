using Microsoft.EntityFrameworkCore;
using SpokaneDotnetAspire.Services.Data;
using SpokaneDotnetAspire.Services.Data.Models;

namespace SpokaneDotnetAspire.Services.Repositories;

public class MeetupRepository
{
	private readonly AppDbContext _Db;

	public MeetupRepository(AppDbContext dbContext)
	{
		_Db = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
	}

	public async Task<IList<Meetup>> GetMeetupsAsync()
	{
		return await _Db.Meetups.ToArrayAsync();
	}

	public async Task<Meetup?> GetMeetupById(string id)
	{
		return await _Db.Meetups.FindAsync(id);
	}

	public async Task<Result<string>> CreateMeetupAsync(string title, string content, string? url)
	{
		if (string.IsNullOrWhiteSpace(title))
		{
			return "Title cannot be null or empty";
		}

		if (string.IsNullOrWhiteSpace(content))
		{
			return "Content cannot be null or empty";
		}

		Meetup meetup = new()
		{
			Title = title,
			Content = content,
			MeetupUrl = url,
		};

		await _Db.Meetups.AddAsync(meetup);
		await _Db.SaveChangesAsync();
		return Result.Ok<string>();
	}

	public async Task<Result<Meetup, string>> UpdateMeetupAsync(string id, Option<string> title, Option<string> content, Option<string?> url)
	{
		if (await _Db.Meetups.FindAsync(id) is not { } meetup)
		{
			return $"Could not find meetup of id `{id}`";
		}

		var entry = _Db.Meetups.Entry(meetup);

		if (title.Unwrap(out string? newTitle))
		{
			entry.Property(x => x.Title).IsModified = true;
			meetup.Title = newTitle;
		}

		if (content.Unwrap(out string? newContent))
		{
			entry.Property(x => x.Content).IsModified = true;
			meetup.Content = newContent;
		}

		if (url.Unwrap(out string? newMeetupUrl))
		{
			entry.Property(x => x.MeetupUrl).IsModified = true;
			meetup.MeetupUrl = newMeetupUrl;
		}

		await _Db.SaveChangesAsync();
		return meetup;
	}

	public async Task<Result<Meetup, string>> RemoveMeetupAsync(string id)
	{
		if (await _Db.Meetups.FindAsync(id) is not { } meetup)
		{
			return $"Could not find meetup of id `{id}`";
		}

		_Db.Meetups.Remove(meetup);
		await _Db.SaveChangesAsync();
		return meetup;
	}
}