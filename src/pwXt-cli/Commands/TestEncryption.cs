using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using heitech.pwXtCli.Options;
using heitech.pwXtCli.ValueObjects;

namespace heitech.pwXtCli.Commands;

[Command("test", Description = "test the encryption on a a password")]
public class TestEncryption : ICommand
{
    public async ValueTask ExecuteAsync(IConsole console)
    {
        var options = new PwXtOptions
        {
            Passphrase = "abcaffeschnee",
            ConnectionString = "does not matter"
        };

        var pw = await options.EncryptAsync("movie", "swordfish");
        var result = await options.DecryptAsync(pw);

        await console.Output.WriteLineAsync(result);
    }
}