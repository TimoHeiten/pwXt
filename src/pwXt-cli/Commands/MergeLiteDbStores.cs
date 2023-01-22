using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using heitech.pwXtCli.Store;
using LiteDB;

namespace heitech.pwXtCli.Commands;

[Command("merge", Description = "merge multiple litedb files into one")]
public sealed class MergeLiteDbStores : ICommand
{
    [CommandParameter(order: 0, Description = "the db to be merged into")]
    public string MasterDb { get; set; } = default!;

    [CommandParameter(order: 1, Description = "All file paths to the files you want to merge into one")]
    public IReadOnlyList<string> Paths { get; set; } = Array.Empty<string>();

    // todo untested
    public async ValueTask ExecuteAsync(IConsole console)
    {
        var masterDb = new LiteDatabase(MasterDb);
        var masterCol = masterDb.GetCollection<StoredPassword>("passwords");
        var length = masterCol.LongCount();
        foreach (var path in Paths)
        {
            await console.Output.WriteLineAsync("inserting " + path);
            var slaveDb = new LiteDatabase(path);
            var col = slaveDb.GetCollection<StoredPassword>("passwords");
            var all = col.FindAll();

            var current = masterCol.Query().Select(x => x.Key).ToList();
            var noDuplicates = all.Where(x => !current.Contains(x.Key)).ToList();
            masterCol.InsertBulk(noDuplicates);
        }

        await console.Output.WriteLineAsync("finished inserting");
    }
}