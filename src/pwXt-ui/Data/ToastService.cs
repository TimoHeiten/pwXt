using System.Timers;
using Timer = System.Timers.Timer;

namespace pwXt_ui.Data;

/// <summary>
/// Reacts to the PwStore Actions and updates the UI accordingly.
/// </summary>
public sealed class ToastService
{
    public event Action<string, ToastLevel>? OnShow;
    public event Action? OnHide;
    private Timer? _countdown;

    public void ShowToast(string message, ToastLevel level)
    {
        OnShow?.Invoke(message, level);
        StartCountdown();
    }

    private void StartCountdown()
    {
        SetCountdown();

        if (_countdown!.Enabled)
        {
            _countdown.Stop();
            _countdown.Start();
        }
        else
        {
            _countdown.Start();
        }
    }

    private void SetCountdown()
    {
        if (_countdown is not null) 
            return;

        _countdown = new Timer(5000);
        _countdown.Elapsed += HideToast;
        _countdown.AutoReset = false;
    }

    private void HideToast(object? source, ElapsedEventArgs args)
    {
        OnHide?.Invoke();
    }

    public void Dispose()
    {
        _countdown?.Dispose();
    }

    public enum ToastLevel
    {
        Info,
        Success,
        Warning,
        Error
    }

}