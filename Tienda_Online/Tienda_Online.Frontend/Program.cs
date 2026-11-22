using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Tienda_Online.Frontend;
using Tienda_Online.Frontend.AuthenticactionProvider;
using Tienda_Online.Frontend.Repositories;
using Tienda_Online.Frontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<IRepository, Repository>();

// La URL de la API se lee de wwwroot/appsettings.{Entorno}.json (ApiUrl):
// Development -> API local · Production -> App Service de la API en Azure.
var apiUrl = builder.Configuration["ApiUrl"]
    ?? throw new InvalidOperationException("Falta la configuración 'ApiUrl' en wwwroot/appsettings.json.");
builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(apiUrl) });
builder.Services.AddSweetAlert2();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationProviderJWT>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationProviderJWT>(x=>
    x.GetRequiredService<AuthenticationProviderJWT>());
builder.Services.AddScoped<ILoginService, AuthenticationProviderJWT>(x =>
    x.GetRequiredService<AuthenticationProviderJWT>());

await builder.Build().RunAsync();
