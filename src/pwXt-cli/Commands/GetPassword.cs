using CliFx;
using CliFx.Attributes;
using CliFx.Exceptions;
using CliFx.Infrastructure;
using pwXt_Service.Commands;
using pwXt_Service.Services;
using pwXt_Service.ValueObjects;

namespace heitech.pwXtCli.Commands
{
    [Command("get", Description = "Get a password from the password store via the specified key")]
    public sealed class GetPassword : ICommand
    {
        private readonly CommandFactory _factory;
        private readonly IClipboardService _clipboardService;

        [CommandParameter(order:0, Description = "The key to store the password under", IsRequired = true)]
        public string Id { get; set; } = default!;

        public GetPassword(CommandFactory factory, IClipboardService clipboardService)
        {
            _factory = factory;
            _clipboardService = clipboardService;
        }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            var opResult = await _factory.Get(new PasswordId(Id)).ExecuteAsync();
            if (opResult.IsSuccess is false)
                throw new CommandException(opResult.Exception!.Message, innerException: opResult.Exception);

            var result = opResult.Result as string;
            await _clipboardService.SetText(result ?? "No password found");
            await console.Output.WriteLineAsync("Password for copied to clipboard");
        }
    }
}