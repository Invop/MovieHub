using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using MovieHub.Client;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddRadzenComponents();
builder.Services.AddRadzenCookieThemeService(options =>
{
    options.Name = "MovieHubTheme";
    options.Duration = TimeSpan.FromDays(365);
});
builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<MovieHub.Client.IdentityDBService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddHttpClient("MovieHub.Server", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("MovieHub.Server"));
builder.Services.AddScoped<MovieHub.Client.SecurityService>();
builder.Services.AddScoped<AuthenticationStateProvider, MovieHub.Client.ApplicationAuthenticationStateProvider>();
var host = builder.Build();
await host.RunAsync();