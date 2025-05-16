using FontConverter.Blazor;
using FontConverter.Blazor.Layout;
using FontConverter.Blazor.Services;
using FontConverter.SharedLibrary;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddRadzenComponents();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddSingleton<OpenTypeFontAnalyzer>();
builder.Services.AddScoped<OpenTypeFontService>();

var host = builder.Build();

var analyzer = host.Services.GetRequiredService<OpenTypeFontAnalyzer>();
await analyzer.LoadPrimaryDataAsync();

await host.RunAsync();