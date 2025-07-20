using LVGLFontConverter.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace LVGLFontConverter.Views;

public sealed partial class FontLoader : Page
{
    public FontLoaderViewModel ViewModel { get; }
    public FontLoader()
    {
        ViewModel = App.GetService<FontLoaderViewModel>();
        this.InitializeComponent();
        Loaded += FontLoader_Loaded;
    }

    private void FontLoader_Loaded(object sender, RoutedEventArgs e)
    {
        ViewModel.StartLoading();
    }
}
