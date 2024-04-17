namespace SpokaneDotnetAspire.Services;

public static class Result
{
	public static Result<T, E> Ok<T, E>(T value) => Result<T, E>.Ok(value);
	public static Result<T, E> Error<T, E>(E error) => Result<T, E>.Error(error);
}

public class Result<T, E>
{
	private readonly bool _success;
	private readonly T? _value;
	private readonly E? _error;

	public bool IsOk => _success;
	public bool IsError => !_success;

	private Result(T? value, E? error, bool success)
	{
		_value = value;
		_error = error;
		_success = success;
	}

	public static Result<T, E> Ok(T value) => new(value, default, true);
	public static Result<T, E> Error(E error) => new(default, error, false);

	public static implicit operator Result<T, E>(T value) => Ok(value);
	public static implicit operator Result<T, E>(E error) => Error(error);

	public R Match<R>(Func<T, R> success, Func<E, R> error)
		=> IsOk ? success(_value!) : error(_error!);
}