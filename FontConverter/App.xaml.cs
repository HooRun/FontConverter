using LVGLFontConverter.Activation;
using LVGLFontConverter.Contracts.Mappers;
using LVGLFontConverter.Contracts.Services;
using LVGLFontConverter.Services;
using LVGLFontConverter.UserControls;
using LVGLFontConverter.ViewModels;
using LVGLFontConverter.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;

namespace LVGLFontConverter;


public partial class App : Application
{
    public IHost Host { get; }

    public static T GetService<T>() where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }
        return service;
    }


    public App()
    {
        InitializeComponent();
        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
            // Default Activation Handler
            services.AddSingleton<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Mapper
            services.AddAutoMapper(typeof(AutoMapperConfig));

            // Services
            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();

            // Libraries
            services.AddSingleton<LVGLFontConverter.Library.FontConverter>();

            // ViewModels
            services.AddSingleton<FontPropertiesViewModel>();
            services.AddSingleton<FontDataViewModel>();
            services.AddSingleton<FontAdjusmentViewModel>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<FontLoaderViewModel>();

            // User Controls
            services.AddSingleton<AppToolbarUC>();
            services.AddSingleton<FontPropertiesUC>();
            services.AddSingleton<FontAdjusmentsUC>();
            services.AddSingleton<FontContentUC>();
            services.AddSingleton<GlyphToolbarUC>();
            services.AddSingleton<GlyphAdjusmentUC>();

            // Views
            services.AddSingleton<MainWindow>();
            services.AddSingleton<TotalGlyphs>();
            services.AddSingleton<EditGlyph>();
            services.AddSingleton<FontLoader>();

            // Configuration
        }).
        Build();
        UnhandledException += App_UnhandledException;
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);
        await GetService<IActivationService>().ActivateAsync(args);
    }

    public static Window MainWindow { get; } = new MainWindow();
    public static IntPtr _HWND;
    public static WindowId _WindowId;
    public static AppWindow _AppWindow = AppWindow.Create();

}
