using heitech.pwXtCli.Options;
using pwXt_Service.Operation;
using pwXt_Service.Store;
using pwXt_Service.ValueObjects;

namespace pwXt_Service.Commands;

/// <summary>
/// Mutate a password - (create, update, delete)
/// </summary>
internal sealed class Mutate : IPasswordCommand
{
    private readonly PasswordId _id;
    private readonly PasswordValue _value;
    private readonly IPasswordStore _store;

    private readonly OperationType _operation;
    private readonly PwXtOptions _options;

    internal Mutate(IPasswordStore store, PasswordId id, PasswordValue value, OperationType operation,
        PwXtOptions pwXtOptions)
    {
        _id = id;
        _value = value;
        _store = store;
        _options = pwXtOptions;
        _operation = operation;
    }

    /// <summary>
    /// <inheritdoc cref="IPasswordCommand.ExecuteAsync"></inheritdoc>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<OperationResult> ExecuteAsync()
    {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (_operation)
        {
            case OperationType.Add:
                await Create();
                break;
            case OperationType.Alter:
                await Update();
                break;
            case OperationType.Delete:
                await Delete();
                break;
            default:
                throw new Exception($"Operation '{_operation}' is not supported in a mutate command");
        }
        
        return OperationResult.Success();
    }

    private async Task Create()
    {
        var result = await _store.GetPassword(_id.Value);
        if (!result.IsEmpty)
            throw new Exception($"Password with key '{_id.Value}' already exists in store");

        var passwordResult = _options.Encrypt(_id.Value, _value.Value!);
        await _store.AddPassword(passwordResult);
    }

    private async Task Update()
    {
        var result = await _store.GetPassword(_id.Value);
        if (result.IsEmpty)
            throw new Exception($"Password with key '{_id.Value}' does not exist in store");

        var passwordResult = _options.Encrypt(_id.Value, _value.Value!);
        await _store.UpdatePassword(passwordResult);
    }

    private async Task Delete()
    {
        var result = await _store.GetPassword(_id.Value);
        if (result.IsEmpty)
            throw new Exception($"Password with key '{_id.Value}' does not exist in store");

        await _store.DeletePassword(_id.Value);
    }
}