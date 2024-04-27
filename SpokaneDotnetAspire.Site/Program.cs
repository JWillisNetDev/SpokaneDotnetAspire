using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

using MudBlazor.Services;

using SpokaneDotnetAspire.Site.Components;
using SpokaneDotnetAspire.Site.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.Configure<HubOptions>(options =>
{
    options.MaximumReceiveMessageSize = 1024 * 1024 * 30; // 30MB
});

builder.Services.Configure<SpokaneDotnetAspireServiceOptions>(configure =>
{
    builder.Configuration.GetSection("SpokaneDotnetAspireServiceOptions").Bind(configure);
});

builder.Services.AddHttpClient<IMeetupService, MeetupService>((isp, configure) =>
{
    var options = isp.GetRequiredService<IOptions<SpokaneDotnetAspireServiceOptions>>();
    configure.BaseAddress = options.Value.BaseUrl;
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
