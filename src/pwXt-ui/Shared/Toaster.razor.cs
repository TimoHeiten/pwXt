using Microsoft.AspNetCore.Components;
using pwXt_ui.Data;
namespace pwXt_ui.Shared;

public class ToasterBase : ComponentBase, IDisposable
{
    [Inject] 
    private ToastService ToastService { get; set; } = null!;

    protected string Heading { get; set; } = null!;
    protected string Message { get; set; } = null!;
    protected bool IsVisible { get; set; }
    protected string BackgroundCssClass { get; set; } = null!;
    protected string IconCssClass { get; set; } = null!;

    protected override void OnInitialized()
    {
        ToastService.OnShow += ShowToast;
        ToastService.OnHide += HideToast;
    }

    private void ShowToast(string message, ToastService.ToastLevel level)
    {
        BuildToastSettings(level, message);
        IsVisible = true;
        InvokeAsync(StateHasChanged);
    }

    private void HideToast()
    {
        IsVisible = false;
        InvokeAsync(StateHasChanged);
    }

    private void BuildToastSettings(ToastService.ToastLevel level, string message)
    {
        switch (level)
        {
            case ToastService.ToastLevel.Info:
                BackgroundCssClass = "bg-info";
                IconCssClass = "info";
                Heading = "Info";
                break;
            case ToastService.ToastLevel.Success:
                BackgroundCssClass = "bg-success";
                IconCssClass = "check";
                Heading = "Success";
                break;
            case ToastService.ToastLevel.Warning:
                BackgroundCssClass = "bg-warning";
                IconCssClass = "exclamation";
                Heading = "Warning";
                break;
            case ToastService.ToastLevel.Error:
                BackgroundCssClass = "bg-danger";
                IconCssClass = "times";
                Heading = "Error";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }

        Message = message;
    }

    public void Dispose()
    {
        ToastService.OnShow -= ShowToast;
        ToastService.OnHide -= HideToast;
    }
}