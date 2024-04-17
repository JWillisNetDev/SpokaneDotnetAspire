using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SpokaneDotnetAspire.Services.Data;

namespace SpokaneDotnetAspire.Services.Tests;

public sealed class DbFixture : IDisposable, IDbContextFactory<AppDbContext>
{
	public SqliteConnection _Connection;

	public DbFixture()
	{
		string connectionString = $"DataSource=file:{Guid.NewGuid()}?mode=memory";
		_Connection = new SqliteConnection(connectionString);
		_Connection.Open();
	}

	public AppDbContext CreateDbContext()
	{
		var db = new TestAppDbContext(_Connection);
		db.Database.EnsureCreated();

		return db;
	}

	public void Dispose()
	{
		_Connection.Dispose();
	}

	private class TestAppDbContext : AppDbContext
	{
		private readonly SqliteConnection _Connection;

		public TestAppDbContext(SqliteConnection connection)
		{
			_Connection = connection ?? throw new ArgumentNullException(nameof(connection));
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			optionsBuilder.UseSqlite(_Connection);
		}
	}
}