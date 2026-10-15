namespace MobileBackEnd.Services;

/// <summary>Resultado posible de una operación de negocio; el controlador lo traduce a un código HTTP.</summary>
public enum OperationStatus
{
/// <summary>Todo salió bien (200/204).</summary>
    Ok,
/// <summary>El registro no existe (404).</summary>
    NotFound,
/// <summary>El registro existe pero pertenece a otro usuario (403).</summary>
    Forbidden,
/// <summary>Choca con otro registro, por ejemplo un correo repetido (409).</summary>
    Conflict,
/// <summary>Los datos no son válidos (400).</summary>
    Invalid
}

/// <summary>
/// Resultado de un servicio: estado + valor opcional + mensaje de error. Evita usar excepciones
/// para los casos normales como "no encontrado" o "no es tuyo".
/// </summary>
public record OperationResult<T>(OperationStatus Status, T? Value = default, string? Message = null)
{
    public static OperationResult<T> Ok(T value) => new(OperationStatus.Ok, value);
    public static OperationResult<T> NotFound() => new(OperationStatus.NotFound);
    public static OperationResult<T> Forbidden() => new(OperationStatus.Forbidden);
    public static OperationResult<T> Conflict(string message) => new(OperationStatus.Conflict, default, message);
    public static OperationResult<T> Invalid(string message) => new(OperationStatus.Invalid, default, message);
}
