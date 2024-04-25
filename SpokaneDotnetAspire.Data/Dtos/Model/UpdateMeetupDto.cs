using System.ComponentModel.DataAnnotations;

namespace SpokaneDotnetAspire.Data.Dtos.Model;

public record UpdateMeetupDto(
    [property: Required] string Title,
    [property: Required] string Content,
    string? MeetupUrl);
