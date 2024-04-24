namespace SpokaneDotnetAspire.Api.Meetups;

public static class MeetupExtensions
{
    public static IEndpointRouteBuilder MapMeetupEndpoints(this IEndpointRouteBuilder builder)
    {
        builder = builder.MapGroup("/meetups");

        builder.MapGet("/", MeetupMethods.GetMeetupsAsync)
            .WithName("GetMeetups");

        builder.MapGet("/{meetupId}", MeetupMethods.GetMeetupAsync)
            .WithName("GetMeetup");

        builder.MapPost("/create", MeetupMethods.CreateMeetupAsync)
            .WithName("CreateMeetup")
            .DisableAntiforgery();

        builder.MapPut("/{meetupId}", MeetupMethods.UpdateMeetupAsync)
            .WithName("UpdateMeetup");

        builder.MapDelete("/{meetupId}", MeetupMethods.DeleteMeetupAsync)
            .WithName("DeleteMeetup");

        return builder;
    }

}