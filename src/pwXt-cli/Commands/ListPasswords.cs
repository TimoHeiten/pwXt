using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using heitech.pwXtCli.Store;

namespace heitech.pwXtCli.Commands
{
    [Command("list", Description = "List all password keys in the password store")]
    public sealed class ListPasswords : ICommand
    {
        private readonly IPasswordStore _store;

        public ListPasswords(IPasswordStore store)
            => _store = store;

        public async ValueTask ExecuteAsync(IConsole console)
        {
            var list = await _store.ListKeysAsync();
            foreach (var key in list)
                await console.Output.WriteLineAsync(key);
        }
    }
}
