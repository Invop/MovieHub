using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using MovieHub.Client;
using Microsoft.AspNetCore.Components.Authorization;
using MovieHub.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddRadzenComponents();
builder.Services.AddRadzenCookieThemeService(options =>
{
    options.Name = "MovieHubTheme";
    options.Duration = TimeSpan.FromDays(365);
});
builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IdentityDbService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddHttpClient("MovieHub.Server", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("MovieHub.Server"));
builder.Services.AddScoped<SecurityService>();
builder.Services.AddScoped<AuthenticationStateProvider, ApplicationAuthenticationStateProvider>();
var host = builder.Build();
await host.RunAsync();