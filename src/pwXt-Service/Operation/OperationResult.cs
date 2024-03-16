namespace pwXt_Service.Operation;

/// <summary>
/// Represents the result of an operation.
/// </summary>
public sealed class OperationResult
{
    /// <summary>
    /// Did the Operation Succeed?
    /// </summary>
    public bool IsSuccess => Exception is null;
    /// <summary>
    /// Not null if the operation failed.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    /// Can be not null, depends on the Command you used. Typically Mutation Commands DO not return a value but have a side effect.
    /// Read Commands on the other hand (aka queries) DO return a value.
    /// </summary>
    public object? Result { get; private set; }
    private OperationResult(Exception? exception)
        => Exception = exception;

    public static OperationResult Success() => new(null);
    public static OperationResult Success(object result) => new(null) { Result = result };
    public static OperationResult Failure(Exception e) => new(e);
}