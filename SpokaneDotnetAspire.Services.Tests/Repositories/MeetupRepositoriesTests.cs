using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SpokaneDotnetAspire.Services.Data.Models;
using SpokaneDotnetAspire.Services.Repositories;

namespace SpokaneDotnetAspire.Services.Tests.Repositories;

public class MeetupRepositoriesTests : UnitTestBase
{
	[Fact]
	public async Task GetMeetupsAsync_ReturnsMeetups()
	{
		// Arrange
		Meetup meetup1 = new()
		{
			Title = "Hello, world!",
			Content = "This is a test",
		};

		Meetup meetup2 = new()
		{
			Title = "This is another post.",
			Content = "This is another test!",
			MeetupUrl = "https://example.com",
		};

		await InDbScopeAsync(async db =>
		{
			db.Meetups.AddRange(meetup1, meetup2);
			await db.SaveChangesAsync();
		});

		IList<Meetup> actualMeetups = null!;
		await InDbScopeAsync(async db =>
		{
			MeetupRepository repository = new(db);

			// Act
			actualMeetups = await repository.GetMeetupsAsync();
		});

		// Assert
		actualMeetups.Should()
			.HaveCount(2)
			.And.ContainEquivalentOf(meetup1)
			.And.ContainEquivalentOf(meetup2);
	}

	[Fact]
	public async Task GetMeetupByIdAsync_ReturnsMeetup()
	{
		// Arrange
		Meetup expectedMeetup = new()
		{
			Title = "Hello, world!",
			Content = "This is a test",
			MeetupUrl = "https://example.com",
		};

		await InDbScopeAsync(async db =>
		{
			Meetup otherMeetup = new()
			{
				Title = "Woah woah woah",
				Content = "If this gets returned, something is wrong",
			};

			db.Meetups.AddRange(expectedMeetup, otherMeetup);
			await db.SaveChangesAsync();
		});

		Meetup? actualMeetup = null;
		await InDbScopeAsync(async db =>
		{
			MeetupRepository repository = new(db);

			// Act
			actualMeetup = await repository.GetMeetupById(expectedMeetup.Id);
		});

		// Assert
		actualMeetup.Should().BeEquivalentTo(expectedMeetup);
	}

	[Fact]
	public async Task CreateMeetup_CreatesNewMeetup()
	{
		// Arrange
		const string expectedTitle = "Hello, world!";
		const string expectedContent = "This is a test";
		const string expectedUrl = "https://example.com";

		await InDbScopeAsync(async db =>
		{
			MeetupRepository repository = new(db);

			// Act
			var result = await repository.CreateMeetupAsync(expectedTitle, expectedContent, expectedUrl);

			// Assert
			result.IsOk.Should().BeTrue();
		});

		Meetup actualMeetup = null!;
		await InDbScopeAsync(async db =>
		{
			actualMeetup = await db.Meetups.SingleAsync();
		});

		actualMeetup.Title.Should().Be(expectedTitle);
		actualMeetup.Content.Should().Be(expectedContent);
		actualMeetup.MeetupUrl.Should().Be(expectedUrl);
	}

	[Fact]
	public async Task CreateMeetup_EmptyTitle_ReturnsErrorResult()
	{
		// Arrange
		await InDbScopeAsync(async db =>
		{
			MeetupRepository repository = new(db);

			// Act
			var result = await repository.CreateMeetupAsync("", "Some content", null);

			// Assert
			result.IsError.Should().BeTrue();
		});
	}

	[Fact]
	public async Task UpdateMeetupAsync_UpdatesMeetupById()
	{
		// Arrange
		const string expectedTitle = "Hello, world!";
		const string expectedContent = "This is a test";
		const string expectedUrl = "https://example.com";

		Meetup meetup = new()
		{
			Title = "Change me!",
			Content = "Change me too!",
			// You can't see it here, but we also want to change the url!
		};

		await InDbScopeAsync(async db =>
		{
			db.Meetups.Add(meetup);
			await db.SaveChangesAsync();
		});

		await InDbScopeAsync(async db =>
		{
			MeetupRepository repository = new(db);

			// Act
			var result = await repository.UpdateMeetupAsync(meetup.Id, expectedTitle, expectedContent, expectedUrl);

			// Assert
			result.IsOk.Should().BeTrue();
		});

		Meetup actualMeetup = null!;
		await InDbScopeAsync(async db =>
		{
			actualMeetup = await db.Meetups.SingleAsync(m => m.Id == meetup.Id);
		});

		actualMeetup.Title.Should().Be(expectedTitle);
		actualMeetup.Content.Should().Be(expectedContent);
		actualMeetup.MeetupUrl.Should().Be(expectedUrl);
	}

	[Fact]
	public async Task UpdateMeetupAsync_OnlyUpdatesValidOptions()
	{
		// Arrange
		const string expectedTitle = "Hello, world!";
		const string expectedContent = "This is a test";
		const string expectedUrl = "https://example.com";

		Meetup meetup = new()
		{
			Title = "Change me!",
			Content = "Change me too!",
			MeetupUrl = expectedUrl, // We aren't changing this url!
		};

		await InDbScopeAsync(async db =>
		{
			db.Meetups.Add(meetup);
			await db.SaveChangesAsync();
		});

		await InDbScopeAsync(async db =>
		{
			MeetupRepository repository = new(db);

			// Act
			var result = await repository.UpdateMeetupAsync(meetup.Id, expectedTitle, expectedContent, Option.None<string?>());

			// Assert
			result.IsOk.Should().BeTrue();
		});

		Meetup actualMeetup = null!;
		await InDbScopeAsync(async db =>
		{
			actualMeetup = await db.Meetups.SingleAsync(m => m.Id == meetup.Id);
		});

		actualMeetup.Title.Should().Be(expectedTitle);
		actualMeetup.Content.Should().Be(expectedContent);
		actualMeetup.MeetupUrl.Should().Be(expectedUrl);
	}

	[Fact]
	public async Task UpdateMeetupAsync_InvalidId_ReturnsErrorResult()
	{
		// Arrange
		Meetup meetup = new()
		{
			Title = "Unimportant",
			Content = "Equally unimportant",
		};

		await InDbScopeAsync(async db =>
		{
			db.Meetups.Add(meetup);
			await db.SaveChangesAsync();
		});

		await InDbScopeAsync(async db =>
		{
			MeetupRepository repository = new(db);

			// Act
			var result = await repository.UpdateMeetupAsync(Guid.Empty.ToString(), Option.None<string>(), Option.None<string>(), Option.None<string?>());

			// Assert
			result.IsError.Should().BeTrue();
		});
	}

	[Fact]
	public async Task RemoveMeetupAsync_RemovesMeetupById()
	{
		// Arrange
		Meetup meetupToDelete = new()
		{
			Title = "I won't make it.",
			Content = "I'm not going to make it either.",
		};

		Meetup meetupToKeep = new()
		{
			Title = "I'm staying.",
			Content = "I'm not going anywhere.",
		};

		await InDbScopeAsync(async db =>
		{
			db.Meetups.AddRange(meetupToDelete, meetupToKeep);
			await db.SaveChangesAsync();
		});

		await InDbScopeAsync(async db =>
		{
			MeetupRepository repository = new(db);

			// Act
			var result = await repository.RemoveMeetupAsync(meetupToDelete.Id);

			// Assert
			result.IsOk.Should().BeTrue();
		});

		await InDbScopeAsync(db =>
		{
			db.Meetups.Should().ContainSingle().Which.Should().BeEquivalentTo(meetupToKeep);
		});
	}

	[Fact]
	public async Task RemoveMeetupAsync_InvalidId_ReturnsErrorResult()
	{
		// Arrange
		Meetup meetup = new()
		{
			Title = "Unimportant",
			Content = "Equally unimportant",
		};

		await InDbScopeAsync(async db =>
		{
			db.Meetups.Add(meetup);
			await db.SaveChangesAsync();
		});

		await InDbScopeAsync(async db =>
		{
			MeetupRepository repository = new(db);

			// Act
			var result = await repository.RemoveMeetupAsync(Guid.Empty.ToString());

			// Assert
			result.IsError.Should().BeTrue();
		});
	}
}
