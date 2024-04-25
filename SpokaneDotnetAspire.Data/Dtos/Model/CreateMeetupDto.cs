using System.ComponentModel.DataAnnotations;

namespace SpokaneDotnetAspire.Data.Dtos.Model;

public record CreateMeetupDto(
    [Required] string Title,
    [Required] string Content,
    string? MeetupUrl);