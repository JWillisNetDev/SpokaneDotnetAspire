namespace SpokaneDotnetAspire.Services;

using Unit = ValueTuple;

public static class Result
{
    public static Result<T, E> Ok<T, E>(T value) => Result<T, E>.Ok(value);
    public static Result<E> Ok<E>() => Result<E>.Ok();
    public static Result<T, E> Err<T, E>(E error) => Result<T, E>.Err(error);
    public static Result<E> Err<E>(E error) => Result<E>.Err(error);
}

public class Result<T, E>
{
    private readonly bool _Success;
    private readonly T? _Value;
    private protected E? Error { get; }

    public bool IsOk => _Success;
    public bool IsError => !_Success;

    private protected Result(T? value, E? error, bool success)
    {
        _Value = value;
        Error = error;
        _Success = success;
    }

    public static Result<T, E> Ok(T value) => new(value, default, true);
    public static Result<T, E> Err(E error) => new(default, error, false);

    public static implicit operator Result<T, E>(T value) => Ok(value);
    public static implicit operator Result<T, E>(E error) => Err(error);

    public R Match<R>(Func<T, R> success, Func<E, R> error)
        => IsOk ? success(_Value!) : error(Error!);
}

public sealed class Result<E> : Result<Unit, E>
{
    private Result(E? error, bool success) : base(default, error, success)
    {
    }

    public static Result<E> Ok() => new(default, true);
    public new static Result<E> Err(E error) => new(error, false);

    public static implicit operator Result<E>(E error) => Err(error);

    public R Match<R>(Func<R> success, Func<E, R> error)
        => IsOk ? success() : error(Error!);
}