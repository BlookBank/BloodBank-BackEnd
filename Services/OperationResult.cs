namespace MobileBackEnd.Services;

public enum OperationStatus
{
    Ok,
    NotFound,
    Forbidden,
    Conflict,
    Invalid
}

public record OperationResult<T>(OperationStatus Status, T? Value = default, string? Message = null)
{
    public static OperationResult<T> Ok(T value) => new(OperationStatus.Ok, value);
    public static OperationResult<T> NotFound() => new(OperationStatus.NotFound);
    public static OperationResult<T> Forbidden() => new(OperationStatus.Forbidden);
    public static OperationResult<T> Conflict(string message) => new(OperationStatus.Conflict, default, message);
    public static OperationResult<T> Invalid(string message) => new(OperationStatus.Invalid, default, message);
}
