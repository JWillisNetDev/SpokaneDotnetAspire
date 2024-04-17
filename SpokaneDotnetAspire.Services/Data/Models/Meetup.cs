using System.ComponentModel.DataAnnotations;

namespace SpokaneDotnetAspire.Services.Data.Models;

public class Meetup
{
	[Key]
	public string Id { get; set; } = Guid.NewGuid().ToString();

	[Required, MaxLength(128)]
	public required string Title { get; set; }

	[Required]
	public required string Content { get; set; }

	public string? MeetupUrl { get; set; }
}