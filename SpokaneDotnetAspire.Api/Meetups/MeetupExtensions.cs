namespace SpokaneDotnetAspire.Api.Meetups;

public static class MeetupExtensions
{
    public static IEndpointRouteBuilder MapMeetupEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/meetups", MeetupMethods.GetMeetupsAsync)
            .WithName("GetMeetups");

        builder.MapGet("/meetups/{meetupId}", MeetupMethods.GetMeetupAsync)
            .WithName("GetMeetup");

        builder.MapPost("/meetups/create", MeetupMethods.CreateMeetupAsync)
            .WithName("CreateMeetup")
            .DisableAntiforgery();

        builder.MapPut("/meetups/{meetupId}", MeetupMethods.UpdateMeetupAsync)
            .WithName("UpdateMeetup");

        builder.MapDelete("/meetups/{meetupId}", MeetupMethods.DeleteMeetupAsync)
            .WithName("DeleteMeetup");

        return builder;
    }

}