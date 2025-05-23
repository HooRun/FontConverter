using FontConverter.Blazor;
using FontConverter.Blazor.Helpers;
using FontConverter.Blazor.Services;
using FontConverter.SharedLibrary.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddRadzenComponents();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddSingleton<PredefinedDataService>();
builder.Services.AddSingleton<OpenTypeFont>();
builder.Services.AddSingleton<LVGLFont>();
builder.Services.AddSingleton<FontNameValidatorHelper>();

builder.Services.AddSingleton<MainViewModel>();


builder.Services.AddAutoMapper(typeof(MappingProfile));
var host = builder.Build();

PredefinedDataService predefinedData = host.Services.GetRequiredService<PredefinedDataService>();
await predefinedData.InitializePrimaryDataAsync();

await host.RunAsync();