using CliFx;
using CliFx.Attributes;
using CliFx.Exceptions;
using CliFx.Infrastructure;
using heitech.pwXtCli.Options;
using heitech.pwXtCli.Store;
using heitech.pwXtCli.ValueObjects;
using Microsoft.Extensions.Options;

namespace heitech.pwXtCli.Commands
{
    [Command(Description = "Mutate - (add, alter, del) -  a password in the password store")]
    public sealed class MutatePassword : ICommand
    {
        private readonly PwXtOptions _options;
        private readonly IPasswordStore _store;

        [CommandParameter(order: 0, Description = "The operation to perform on the password store (add, remove, update)", IsRequired = true)]
        public string Operation { get; set; } = default!;

        [CommandParameter(1, Description = "The key to store the password under", IsRequired = true)]
        public string Key { get; set; } = default!;

        [CommandOption(name: "value", shortName: 'v', Description = "The password to store")]
        public string? Value { get; set; }

        public MutatePassword(IOptions<PwXtOptions> options, IPasswordStore store)
        {
            _store = store;
            _options = options.Value;
        }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            switch (Operation)
            {
                case "add":
                    if (Value == default)
                        throw new CommandException("Value is required for add operation");
                    await Create(console);
                    break;
                case "alter":
                    if (Value == default)
                        throw new CommandException("Value is required for add operation");
                    await Update(console);
                    break;
                case "del":
                    await Delete(console);
                    break;
                default:
                    throw new CommandException($"Operation '{Operation}' is not supported");
            }
        }

        private async Task Create(IConsole console)
        {
            var result = await _store.GetPasswordAsync(Key);
            if (!result.IsEmpty)
                throw new CommandException($"Password with key '{Key}' already exists in store");

            var passwordResult = await _options.EncryptAsync(Key, Value!);
            await _store.AddPasswordAsync(passwordResult);
            await console.Output.WriteLineAsync($"Password '{Key}' added to store");
        }

        private async Task Update(IConsole console)
        {
            var result = await _store.GetPasswordAsync(Key);
            if (result.IsEmpty)
                throw new CommandException($"Password with key '{Key}' does not exist in store");

            var passwordResult = await _options.EncryptAsync(Key, Value!, result.IV);
            await _store.UpdatePasswordAsync(passwordResult);
            await console.Output.WriteLineAsync($"Password '{Key}' updated in store");
        }

        private async Task Delete(IConsole console)
        {
            var result = await _store.GetPasswordAsync(Key);
            if (result.IsEmpty)
                throw new CommandException($"Password with key '{Key}' does not exist in store");

            await _store.DeletePasswordAsync(Key);
            await console.Output.WriteLineAsync($"Password '{Key}' deleted from store");
        }
    }
}