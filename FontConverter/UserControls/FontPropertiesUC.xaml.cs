using LVGLFontConverter.Models;
using LVGLFontConverter.ViewModels;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;
using static LVGLFontConverter.Library.Helpers.LVGLFontEnums;


namespace LVGLFontConverter.UserControls;

public sealed partial class FontPropertiesUC : UserControl
{
    public FontPropertiesViewModel ViewModel { get; }

    public FontPropertiesUC()
    {
        ViewModel = App.GetService<FontPropertiesViewModel>();
        InitializeComponent();
    }

}
