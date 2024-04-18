namespace SpokaneDotnetAspire.Api.Meetups;

public static class MeetupExtensions
{
    public static IEndpointRouteBuilder MapMeetupEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/meetups", MeetupMethods.GetMeetupsAsync)
            .WithName("GetMeetups");

        builder.MapPost("/meetups", MeetupMethods.CreateMeetupAsync)
            .WithName("CreateMeetup");

        return builder;
    }

}