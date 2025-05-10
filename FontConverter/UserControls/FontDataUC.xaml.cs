using LVGLFontConverter.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace LVGLFontConverter.UserControls;

public sealed partial class FontDataUC : UserControl
{
    public FontDataViewModel ViewModel { get; }

    public FontDataUC()
    {
        ViewModel = App.GetService<FontDataViewModel>();
        this.InitializeComponent();
    }
}
