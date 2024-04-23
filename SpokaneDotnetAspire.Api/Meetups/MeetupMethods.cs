using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SpokaneDotnetAspire.Api.Storage;
using SpokaneDotnetAspire.Services;
using SpokaneDotnetAspire.Services.Data.Models;
using SpokaneDotnetAspire.Services.Repositories;

namespace SpokaneDotnetAspire.Api.Meetups;

public static class MeetupMethods
{
    public static async Task<Ok<MeetupListDto>> GetMeetupsAsync(
        IMeetupRepository meetupRepository,
        CancellationToken cancellationToken = default)
    {
        var meetups = await meetupRepository.GetMeetupsAsync(cancellationToken);
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
        [AsParameters] CreateMeetupParams meetupParams,
        IFormFile imageFormFile,
        IMeetupRepository meetupRepository,
        IStorageService storageService,
        CancellationToken cancellationToken = default)
    {
        if (!imageFormFile.ContentType.Contains("image"))
        {
            return TypedResults.BadRequest("The file must be an image.");
        }

        // Upload the file first.
        var imageBinaryData = await BinaryData.FromStreamAsync(imageFormFile.OpenReadStream(), cancellationToken);
        var blobUrl = await storageService.UploadImageAsync(imageFormFile.FileName, imageBinaryData, cancellationToken);
        if (blobUrl is null)
        {
            return TypedResults.BadRequest("You did something wrong and the blob didn't upload :("); // TODO error message
        }

        var result = await meetupRepository.CreateMeetupAsync(meetupParams.Title, meetupParams.Content, blobUrl, cancellationToken);

        return result.Match<Results<Created, BadRequest<string>>>(
            _ => TypedResults.Created(),
            err => TypedResults.BadRequest(err));
    }

    public static async Task<Results<Ok, NotFound>> UpdateMeetupAsync(
        [FromRoute] string meetupId,
        [FromBody] UpdateMeetupDto meetupDto,
        IMeetupRepository meetupRepository,
        CancellationToken cancellationToken = default)
    {
        Option<string?> urlOption = meetupDto.Url is null ? Option.None<string?>() : Option.Some<string?>(meetupDto.Url);

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

public record CreateMeetupParams(
    [property: Required] string Title,
    [property: Required] string Content);

public record UpdateMeetupDto(
    [property: Required] string Title,
    [property: Required] string Content,
    string? Url);

public record MeetupListDto(IList<Meetup> Meetups);
