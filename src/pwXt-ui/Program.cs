using ElectronNET.API;
using pwXt_ui.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddElectron();
builder.WebHost.UseElectron(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<ToastService>();
builder.Services.AddSingleton(
    sp => new PwStore(builder.Configuration.GetSection("AppSettings:CliPath").Value,
    sp.GetRequiredService<ToastService>())
);
// maybe we could also utilize the services directly instead of using the CLI 
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseEndpoints(e =>
{
    app.MapBlazorHub();
    app.MapFallbackToPage("/_Host");
});

var t = Task.Run(() => app.Run());
var t1 = Task.Run(async () => await Electron.WindowManager.CreateBrowserViewAsync());
var t2 = Task.Run(async () => await Electron.WindowManager.CreateWindowAsync());
await Task.WhenAll(t, t1, t2);