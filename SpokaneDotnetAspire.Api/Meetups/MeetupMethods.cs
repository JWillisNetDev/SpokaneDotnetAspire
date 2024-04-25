using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SpokaneDotnetAspire.Api.Services.Repositories;
using SpokaneDotnetAspire.Data;
using SpokaneDotnetAspire.Data.Dtos.Model;
using SpokaneDotnetAspire.Data.Models;

namespace SpokaneDotnetAspire.Api.Meetups;

public sealed class MeetupMethods
{
    public static async Task<Ok<IList<Meetup>>> GetMeetupsAsync(
        [AsParameters] GetMeetupsParams @params,
        IMeetupRepository meetupRepository,
        CancellationToken cancellationToken = default)
    {
        var meetups = await meetupRepository.GetMeetupsAsync(@params.Page * @params.PageSize, @params.PageSize, cancellationToken);
        return TypedResults.Ok(meetups);
    }

    public static async Task<Results<Ok<Meetup>, NotFound>> GetMeetupAsync(
        [FromRoute] string meetupId,
        IMeetupRepository meetupRepository,
        CancellationToken cancellationToken)
    {
        if (await meetupRepository.GetMeetupByIdAsync(meetupId, cancellationToken) is { } meetup)
        {
            return TypedResults.Ok(meetup);
        }

        return TypedResults.NotFound();
    }

    public static async Task<Results<Created, BadRequest<string>>> CreateMeetupAsync(
        [FromBody] CreateMeetupDto dto,
        IMeetupRepository meetupRepository,
        ILogger<MeetupMethods> logger,
        CancellationToken cancellationToken = default)
    {
        logger.LogTrace("Creating new meetup {meetupTitle} at {at}", dto.Title, DateTimeOffset.UtcNow);

        var result = await meetupRepository.CreateMeetupAsync(dto.Title, dto.Content, dto.MeetupUrl, dto.ImageUri, cancellationToken);

        return result.Match<Results<Created, BadRequest<string>>>(
            created =>
            {
                logger.LogInformation("Created new meetup {meetupId} at {at}", created.Id, DateTimeOffset.UtcNow);
                return TypedResults.Created();
            },
            err => TypedResults.BadRequest(err));
    }

    public static async Task<Results<Ok, NotFound>> UpdateMeetupAsync(
        [FromRoute] string meetupId,
        [FromBody] UpdateMeetupDto meetupDto,
        IMeetupRepository meetupRepository,
        CancellationToken cancellationToken = default)
    {
        Option<string?> urlOption = meetupDto.MeetupUrl is null ? Option.None<string?>() : Option.Some<string?>(meetupDto.MeetupUrl);

        var result = await meetupRepository.UpdateMeetupAsync(meetupId,
            meetupDto.Title,
            meetupDto.Content,
            urlOption,
            cancellationToken);

        return result.Match<Results<Ok, NotFound>>(
            _ => TypedResults.Ok(),
            _ => TypedResults.NotFound());
    }

    public static async Task<Results<NoContent, NotFound>> DeleteMeetupAsync(
        [FromRoute] string meetupId,
        IMeetupRepository meetupRepository,
        CancellationToken cancellationToken = default)
    {
        var result = await meetupRepository.DeleteMeetupAsync(meetupId, cancellationToken);

        return result.Match<Results<NoContent, NotFound>>(
            _ => TypedResults.NoContent(),
            _ => TypedResults.NotFound());
    }
}

public record GetMeetupsParams(
    int Page,
    int PageSize);