using Microsoft.EntityFrameworkCore;
using SpokaneDotnetAspire.Services.Data.Models;

namespace SpokaneDotnetAspire.Services.Data;

public class AppDbContext : DbContext
{
	public DbSet<Meetup> Meetups => Set<Meetup>();

	public AppDbContext()
	{
	}

	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}
}