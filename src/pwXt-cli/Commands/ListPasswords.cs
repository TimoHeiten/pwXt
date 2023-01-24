using CliFx;
using CliFx.Attributes;
using CliFx.Exceptions;
using CliFx.Infrastructure;
using pwXt_Service.Commands;

namespace heitech.pwXtCli.Commands
{
    [Command("list", Description = "List all password keys in the password store")]
    public sealed class ListPasswords : ICommand
    {
        private readonly CommandFactory _factory;

        public ListPasswords(CommandFactory factory)
        {
            _factory = factory;
        }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            var opResult = await _factory.List().ExecuteAsync();
            if (opResult.IsSuccess is false)
                throw new CommandException(opResult.Exception!.Message, innerException:opResult.Exception);

            var list = opResult.Result as List<string>;
            foreach (var key in list!)
                await console.Output.WriteLineAsync(key);
        }
    }
}