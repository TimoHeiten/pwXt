using System.Reflection.Emit;
using heitech.pwXtCli.Options;
using pwXt_Service.Operation;
using pwXt_Service.Store;
using pwXt_Service.ValueObjects;

namespace pwXt_Service.Commands;

/// <summary>
/// Create the CRUD Commands for the PasswordManager
/// </summary>
public sealed class CommandFactory
{
    private readonly PwXtOptions _options;
    private readonly IPasswordStore _passwordStore;

    public CommandFactory(IPasswordStore passwordStore, PwXtOptions options)
    {
        _options = options;
        _passwordStore = passwordStore;
    }

    public IPasswordCommand Mutate(PasswordId id, PasswordValue value, OperationType operation)
    {
        return operation switch
        {
            OperationType.Add => new Mutate(_passwordStore, id, value, OperationType.Add, _options),
            OperationType.Alter => new Mutate(_passwordStore, id, value, OperationType.Alter, _options),
            OperationType.Delete => new Mutate(_passwordStore, id, PasswordValue.Empty, OperationType.Delete, _options),
            _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
        };
    }
    
    /// <summary>
    /// Create an Add Command
    /// </summary>
    /// <param name="id"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private IPasswordCommand Create(PasswordId id, PasswordValue value)
        => new Mutate(_passwordStore, id, value, OperationType.Add, _options);

    /// <summary>
    /// Create an Update Command
    /// </summary>
    /// <param name="id"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private IPasswordCommand Update(PasswordId id, PasswordValue value)
        => new Mutate(_passwordStore, id, value, OperationType.Alter, _options);

    /// <summary>
    /// Create a Delete Command
    /// </summary>
    /// <param name="id"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private IPasswordCommand Delete(PasswordId id, PasswordValue value)
        => new Mutate(_passwordStore, id, value, OperationType.Delete, _options);

    /// <summary>
    /// Create a Get Command
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public IPasswordCommand Get(PasswordId id)
        => new Read(_passwordStore, id, OperationType.Get, _options);

    /// <summary>
    /// Create a List Command
    /// </summary>
    /// <returns></returns>
    public IPasswordCommand List()
        => new Read(_passwordStore, PasswordId.Empty, OperationType.List, _options);
}