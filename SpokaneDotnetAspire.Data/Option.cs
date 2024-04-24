using System.Diagnostics.CodeAnalysis;

namespace SpokaneDotnetAspire.Data;

public static class Option
{
    public static Option<T> Some<T>(T value) => Option<T>.Some(value);
    public static Option<T> None<T>() => Option<T>.None();
}

public sealed class Option<T>
{
    private readonly bool _hasValue;
    private readonly T? _value;

    public bool IsSome => _hasValue;
    public bool IsNone => !_hasValue;

    private Option(T? value, bool hasValue)
    {
        _value = value;
        _hasValue = hasValue;
    }

    public bool Unwrap([NotNullWhen(true)] out T? value)
    {
        if (IsSome)
        {
            value = _value!;
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
            some(_value!);
            return;
        }

        none();
    }
}