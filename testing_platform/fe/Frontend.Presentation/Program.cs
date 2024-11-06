using Blazored.LocalStorage;
using Contracts.Internal;
using Frontend.Presentation;
using Frontend.Presentation.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using System.Net.Http.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBlazoredLocalStorage();

var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
var configData = await httpClient.GetFromJsonAsync<AppConfiguration>("appsettings.json");
builder.Services.AddSingleton(configData?.ApiSettings);

builder.Services.AddRadzenComponents();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TestPreviewService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();

await builder.Build().RunAsync();
