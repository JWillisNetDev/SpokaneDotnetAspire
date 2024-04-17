using SpokaneDotnetAspire.Services.Data;

namespace SpokaneDotnetAspire.Services.Tests;

public class UnitTestBase : IDisposable
{
	private readonly DbFixture _DbFixture = new();

	protected async Task InDbScopeAsync(Func<AppDbContext, Task> action)
	{
		await using var db = _DbFixture.CreateDbContext();
		await action(db);
	}

	protected async Task InDbScopeAsync(Action<AppDbContext> action)
	{
		await using var db = _DbFixture.CreateDbContext();
		action(db);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			_DbFixture.Dispose();
		}
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}