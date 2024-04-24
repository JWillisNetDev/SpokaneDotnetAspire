using Microsoft.EntityFrameworkCore;

using SpokaneDotnetAspire.Data.Models;

namespace SpokaneDotnetAspire.Data;

public class AppDbContext : DbContext
{
    public DbSet<Meetup> Meetups => Set<Meetup>();

    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}