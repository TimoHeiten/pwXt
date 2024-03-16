using System.Diagnostics;

namespace pwXt_ui.Data;

/// <summary>
/// Interact with the PwStore CLI backend Service
/// </summary>
public sealed class PwStore
{
    private readonly string _pathToCliFile;
    private readonly ToastService _toaster;

    public PwStore(string pathToCliFile, ToastService toaster)
    {
        _pathToCliFile = pathToCliFile;
        _toaster = toaster;
    }

    public async Task<IEnumerable<string>> GetListAsync()
    {
        var (success, result) = await RunProcessAsync("list");
        return !success ? Enumerable.Empty<string>() : result.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
    }

    private async Task<(bool, string)> RunProcessAsync(string args)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _pathToCliFile,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };
        try
        {
            process.Start();
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();
            _toaster.ShowToast($"action: {args} lead to: '{output}'", ToastService.ToastLevel.Success);
            return (true, output);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _toaster.ShowToast($"action {args} lead to ERROR: '{e.Message}'", ToastService.ToastLevel.Error);
            return (false, e.Message);
        }
    }

    public async Task<string> GetPasswordAsync(string name)
    {
        var (success, result) = await RunProcessAsync($"get {name}");
        return result;
    }

    public async Task<bool> DeletePassword(string pwIdentifier)
    {
        var (success, _) = await RunProcessAsync($"del {pwIdentifier}");
        return success;
    }

    public async Task<bool> CreatePassword(string passwordId, string passwordValue)
    {
        var (success, _) = await RunProcessAsync($"add {passwordId} -v {passwordValue}");
        return success;
    }
}