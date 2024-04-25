namespace SpokaneDotnetAspire.Api.Meetups;

public static class MeetupExtensions
{
    public static IEndpointRouteBuilder MapMeetupEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/meetups");

        group.MapGet("/", MeetupMethods.GetMeetupsAsync)
            .WithName("GetMeetups");

        group.MapGet("/{meetupId}", MeetupMethods.GetMeetupAsync)
            .WithName("GetMeetup");

        group.MapPost("/create", MeetupMethods.CreateMeetupAsync)
            .WithName("CreateMeetup")
            .DisableAntiforgery();

        group.MapPut("/{meetupId}", MeetupMethods.UpdateMeetupAsync)
            .WithName("UpdateMeetup");

        group.MapDelete("/{meetupId}", MeetupMethods.DeleteMeetupAsync)
            .WithName("DeleteMeetup");

        return builder;
    }

}