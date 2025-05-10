using LVGLFontConverter.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace LVGLFontConverter.UserControls;

public sealed partial class FontAdjusmentsUC : UserControl
{
    public FontAdjusmentViewModel ViewModel { get; }

    public FontAdjusmentsUC()
    {
        ViewModel = App.GetService<FontAdjusmentViewModel>();
        this.InitializeComponent();
    }
}
