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
    [Command("get", Description = "Get a password from the password store via the specified key")]
    public sealed class GetPassword : ICommand
    {
        private readonly PwXtOptions _options;
        private readonly IPasswordStore _store;

        [CommandParameter(order:0, Description = "The key to store the password under", IsRequired = true)]
        public string Key { get; set; } = default!;

        public GetPassword(IOptions<PwXtOptions> options, IPasswordStore store)
        {
            _store = store;
            _options = options.Value;
        }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            var password = await _store.GetPasswordAsync(Key);
            if (password.IsEmpty)
                throw new CommandException($"Password with key '{Key}' does not exist in store");

            var result = await _options.DecryptAsync(password);
            await TextCopy.ClipboardService.SetTextAsync(result);

            await console.Output.WriteLineAsync("Password for copied to clipboard");
        }
    }
}