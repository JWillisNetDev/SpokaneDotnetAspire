using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SpokaneDotnetAspire.Services.Data.Models;
using SpokaneDotnetAspire.Services.Repositories;

namespace SpokaneDotnetAspire.Api.Meetups;

public static class MeetupMethods
{
    public static async Task<Ok<MeetupList>> GetMeetupsAsync(
        IMeetupRepository meetupRepository,
        CancellationToken cancellationToken = default)
    {
        var meetups = await meetupRepository.GetMeetupsAsync(cancellationToken);

        return TypedResults.Ok(new MeetupList { Meetups = meetups });
    }

    public static async Task<IResult> CreateMeetupAsync(
        [FromBody] CreateMeetup meetup,
        IMeetupRepository meetupRepository,
        CancellationToken cancellationToken = default)
    {
        var result =
            await meetupRepository.CreateMeetupAsync(meetup.Title, meetup.Content, meetup.Url, cancellationToken);

        return result.Match<IResult>(
            TypedResults.Created,
            TypedResults.BadRequest);
    }
}

public class CreateMeetup
{
    public required string Title { get; init; }
    public required string Content { get; init; }
    public string? Url { get; init; } = null;
}

public class MeetupList
{
    public IList<Meetup> Meetups { get; init; } = [];
}