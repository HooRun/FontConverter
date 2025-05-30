using BlazorPro.BlazorSize;
using FontConverter.Blazor;
using FontConverter.Blazor.Helpers;
using FontConverter.Blazor.Services;
using FontConverter.Blazor.ViewModels;
using FontConverter.SharedLibrary.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddRadzenComponents();
builder.Services.AddResizeListener();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddScoped<PredefinedDataService>();
builder.Services.AddScoped<FontNameValidatorHelper>();

builder.Services.AddScoped<MainViewModel>();


builder.Services.AddAutoMapper(typeof(MappingProfile));
var host = builder.Build();

PredefinedDataService predefinedData = host.Services.GetRequiredService<PredefinedDataService>();
await predefinedData.InitializePrimaryDataAsync();

await host.RunAsync();