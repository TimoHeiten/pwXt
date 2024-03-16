using CliFx;
using CliFx.Attributes;
using CliFx.Exceptions;
using CliFx.Infrastructure;
using pwXt_Service.Commands;
using pwXt_Service.Operation;
using pwXt_Service.ValueObjects;

namespace heitech.pwXtCli.Commands
{
    [Command(Description = "Mutate - (add, alter, del) -  a password in the password store")]
    public sealed class MutatePassword : ICommand
    {
        private readonly CommandFactory _factory;

        [CommandParameter(order: 0, Description = "The operation to perform on the password store (add, remove, update)", IsRequired = true)]
        public string Operation { get; set; } = default!;

        [CommandParameter(1, Description = "The key to store the password under", IsRequired = true)]
        public string Id { get; set; } = default!;

        [CommandOption(name: "value", shortName: 'v', Description = "The password to store")]
        public string? Value { get; set; }

        public MutatePassword(CommandFactory factory)
            => _factory = factory;

        private Dictionary<string, (OperationType, string)> _lookUp = new()
        {
            { "add", (OperationType.Add, "Password '{0}' was stored") },
            { "alter", (OperationType.Alter, "Password '{0}' updated in store")},
            { "del", (OperationType.Delete, "Password '{0}' deleted from store") },
        };
        
        public async ValueTask ExecuteAsync(IConsole console)
        {

            if (!_lookUp.TryGetValue(Operation, out var tuple))
                throw new CommandException($"Operation '{Operation}' is not supported");

            var (operationType, message) = tuple;
            if (Value == default && (operationType == OperationType.Add || operationType == OperationType.Alter))
                throw new CommandException($"Operation '{Operation}' requires the -v flag to be set");
            
            var operation = _factory.Mutate(new PasswordId(Id), new PasswordValue(Value!), operationType);
            var opResult = await operation.ExecuteAsync();
            if (!opResult.IsSuccess)
                throw new CommandException(opResult.Exception!.Message, innerException: opResult.Exception);

            var formatted = string.Format(message, Id);
            await console.Output.WriteLineAsync(formatted);
        }
    }
}