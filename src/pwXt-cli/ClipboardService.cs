namespace heitech.pwXtCli;

/// <summary>
/// abstracts the clipBoard interaction
/// </summary>
public interface IClipboardService
{
    Task SetText(string text);
}

/// <summary>
/// for testing purposes
/// </summary>
public sealed class ClipboardService : IClipboardService
{
    public Task SetText(string text)
        => TextCopy.ClipboardService.SetTextAsync(text);
}