using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using LiteDB;

namespace heitech.pwXtCli.Commands;

[Command("create-db", Description = "Create the lite db file at the specified path")]
public sealed  class CreatePasswordStore : ICommand
{
    private readonly IClipboardService _clipboard;

    [CommandParameter(0, IsRequired = true)] 
    public string Path { get; set; } = default!;

    public CreatePasswordStore(IClipboardService clipboard)
        => _clipboard = clipboard;

    public async ValueTask ExecuteAsync(IConsole console)
    {
        //creates the db
        using var db = new LiteDatabase(Path);
        // creates the collection
        db.GetCollection("passwords");

        await _clipboard.SetText(Path);
        await console.Output.WriteLineAsync("created db and passwords collection and copied path to clipboard");
    }
}