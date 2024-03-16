using pwXt_Service.Operation;

namespace pwXt_Service.Commands;

/// <summary>
/// Represents the actual command
/// </summary>
public interface IPasswordCommand
{
    /// <summary>
    /// Execute the created command
    /// </summary>
    /// <returns></returns>
    Task<OperationResult> ExecuteAsync();
}