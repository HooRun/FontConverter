using FontConverter.Blazor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;
using SkiaSharp;
using SkiaSharp.Views.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress ) });

builder.Services.AddRadzenComponents();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");


await builder.Build().RunAsync();
