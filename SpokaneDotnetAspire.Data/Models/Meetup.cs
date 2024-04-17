using System.ComponentModel.DataAnnotations;

namespace SpokaneDotnetAspire.Data.Models;

public class Meetup
{
	public string Id { get; set; } = Guid.NewGuid().ToString();

	[Required]
	[StringLength(128)]
	public required string Title { get; set; }

	[Required]
	public required string Content { get; set; }
}