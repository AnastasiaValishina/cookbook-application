using Blazored.SessionStorage;
using Cookbook.Client;
using Cookbook.Client.Handlers;
using Cookbook.Client.Services;
using Cookbook.Client.Services.Contracts;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient<AuthHandler>();

builder.Services.AddHttpClient("ServerApi")
				.ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration["ServerUrl"] ?? ""))
				.AddHttpMessageHandler<AuthHandler>();

builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddScoped<IRecipeService, RecipeService>();

builder.Services.AddBlazoredSessionStorageAsSingleton();

await builder.Build().RunAsync();
