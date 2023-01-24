using heitech.pwXtCli.Options;
using pwXt_Service.Operation;
using pwXt_Service.Store;
using pwXt_Service.ValueObjects;

namespace pwXt_Service.Commands;

/// <summary>
/// Read a password from the password store
/// </summary>
public sealed class Read : IPasswordCommand
{
    private readonly PasswordId _id;
    private readonly OperationType _operation;

    private readonly IPasswordStore _store;
    private readonly PwXtOptions _options;

    internal Read(IPasswordStore store,  PasswordId id, OperationType get, PwXtOptions options)
    {
        _id = id;
        _store = store;
        _operation = get;
        _options = options;
    }

    public Task<OperationResult> ExecuteAsync()
    {
        return _operation switch
        {
            OperationType.Get => GetAsync(),
            OperationType.List => ListAsync(),
            _ => throw new ArgumentOutOfRangeException(nameof(_operation), _operation,
                $"Read Command cannot use the Operation {nameof(_operation)}")
        };
    }

    private async Task<OperationResult> GetAsync()
    {
        var password = await _store.GetPassword(_id.Value);
        if (password.IsEmpty)
            return OperationResult.Failure(new Exception($"Password with key '{_id.Value}' does not exist in store"));

        var result = _options.Decrypt(password);
        return OperationResult.Success(result);
    }

    private async Task<OperationResult> ListAsync()
    {
        var list = await _store.ListKeys();
        return OperationResult.Success(list.ToList());
    }
}