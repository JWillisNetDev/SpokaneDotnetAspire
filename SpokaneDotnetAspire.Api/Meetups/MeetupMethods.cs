using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SpokaneDotnetAspire.Api.Repositories;
using SpokaneDotnetAspire.Api.Storage;
using SpokaneDotnetAspire.Data;
using SpokaneDotnetAspire.Data.Models;

namespace SpokaneDotnetAspire.Api.Meetups;

public sealed class MeetupMethods
{
    public static async Task<Ok<MeetupListDto>> GetMeetupsAsync(
        [AsParameters] GetMeetupsParams @params,
        IMeetupRepository meetupRepository,
        CancellationToken cancellationToken = default)
    {
        var meetups = await meetupRepository.GetMeetupsAsync(@params.Page * @params.PageSize, @params.PageSize, cancellationToken);
        return TypedResults.Ok(new MeetupListDto(meetups));
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
        [AsParameters] CreateMeetupParams @params,
        IMeetupRepository meetupRepository,
        IStorageService storageService,
        ILogger<MeetupMethods> logger,
        CancellationToken cancellationToken = default)
    {
        logger.LogTrace("Create new meetup {meetupTitle} at {at}", @params.Title, DateTimeOffset.UtcNow);

        // Upload the file first.
        Uri? blobUri = null;
        if (@params.ImageFile is { } imageFile)
        {
            if (!imageFile.ContentType.Contains("image"))
            {
                return TypedResults.BadRequest("The file must be an image.");
            }

            var imageBinaryData = await BinaryData.FromStreamAsync(imageFile.OpenReadStream(), cancellationToken);
            blobUri = await storageService.UploadImageAsync(imageFile.FileName, imageBinaryData, cancellationToken);
            if (blobUri is null)
            {
                logger.LogWarning("Failed to upload image: {imageBinaryData} {meetupTitle}, {at}", imageBinaryData, @params.Title, DateTimeOffset.UtcNow);
            }
        }

        var result = await meetupRepository.CreateMeetupAsync(@params.Title, @params.Content, @params.MeetupUrl, blobUri, cancellationToken);

        return result.Match<Results<Created, BadRequest<string>>>(
            created =>
            {
                logger.LogTrace("Created new meetup {meetupId} at {at}", created.Id, DateTimeOffset.UtcNow);
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

public record CreateMeetupParams(
    [property: Required, FromForm] string Title,
    [property: Required, FromForm] string Content,
    [FromForm] string? MeetupUrl,
    [FromForm] IFormFile? ImageFile);

public record UpdateMeetupDto(
    [property: Required] string Title,
    [property: Required] string Content,
    string? MeetupUrl);

public record MeetupListDto(IList<Meetup> Meetups);
