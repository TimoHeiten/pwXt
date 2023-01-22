using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using LiteDB;

namespace heitech.pwXtCli.Commands;

[Command("create-db", Description = "Create the lite db file at the specified path")]
public sealed  class CreatePasswordStore : ICommand
{
    [CommandParameter(0, IsRequired = true)] 
    public string Path { get; set; } = default!;

    public async ValueTask ExecuteAsync(IConsole console)
    {
        //creates the db
        var db = new LiteDatabase(Path);
        // creates the collection
        db.GetCollection("passwords");

        await TextCopy.ClipboardService.SetTextAsync(Path);
        await console.Output.WriteLineAsync("created db and passwords collection and copied path to clipboard");
    }
}