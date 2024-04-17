using System.Diagnostics.CodeAnalysis;

namespace SpokaneDotnetAspire.Services;

public static class Option
{
	public static Option<T> Some<T>(T value) => Option<T>.Some(value);
	public static Option<T> None<T>() => Option<T>.None();
}

public sealed class Option<T>
{
	private readonly bool _HasValue;
	private readonly T? _Value;

	public bool IsSome => _HasValue;
	public bool IsNone => !_HasValue;

	private Option(T? value, bool hasValue)
	{
		_Value = value;
		_HasValue = hasValue;
	}

	public bool Unwrap([NotNullWhen(true)] out T? value)
	{
		if (IsSome)
		{
			value = _Value!;
			return true;
		}

		value = default;
		return false;
	}

	public static Option<T> Some(T value) => new(value, true);
	public static Option<T> None() => new(default, false);

	public static implicit operator Option<T>(T value) => Some(value);

	public void Match(Action<T> some, Action none)
	{
		if (IsSome)
		{
			some(_Value!);
			return;
		}

		none();
	}
}