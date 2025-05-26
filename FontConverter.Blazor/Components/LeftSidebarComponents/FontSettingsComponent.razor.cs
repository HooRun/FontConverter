using FontConverter.Blazor.Helpers;
using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.Services;
using FontConverter.Blazor.ViewModels;
using FontConverter.SharedLibrary;
using FontConverter.SharedLibrary.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using Radzen.Blazor;
using System.ComponentModel.DataAnnotations;

namespace FontConverter.Blazor.Components.LeftSidebarComponents;

public partial class FontSettingsComponent : ComponentBase, IRerenderable
{
    [Inject]
    public PredefinedDataService PredefinedData { get; set; } = default!;

    [Inject]
    public FontNameValidatorHelper FontNameValidatorHelper { get; set; } = default!;

    [Inject]
    public MainViewModel MainViewModel { get; set; } = default!;

    Variant variant = Variant.Outlined;
    bool floatFieldLabel = true;

    private EditContext editContext = default!;
    private ValidationMessageStore messageStore = default!;

    private void OnFontNameChanged(ChangeEventArgs e)
    {
        MainViewModel.FontSettingsViewModel.FontName = e.Value?.ToString() ?? string.Empty;
        ValidateForm();
    }

    private void OnFallbackChanged(ChangeEventArgs e)
    {
        MainViewModel.FontSettingsViewModel.Fallback = e.Value?.ToString() ?? string.Empty;
        ValidateForm();
    }

    private void ValidateForm()
    {
        messageStore.Clear();

        var fontNameError = FontNameValidatorHelper.ValidateFontNameMessage(MainViewModel.FontSettingsViewModel.FontName, true);
        if (!string.IsNullOrEmpty(fontNameError))
        {
            messageStore.Add(() => MainViewModel.FontSettingsViewModel.FontName, fontNameError);
        }

        var fallbackError = FontNameValidatorHelper.ValidateFontNameMessage(MainViewModel.FontSettingsViewModel.Fallback);
        if (!string.IsNullOrEmpty(fallbackError))
        {
            messageStore.Add(() => MainViewModel.FontSettingsViewModel.Fallback, fallbackError);
        }

        editContext.NotifyValidationStateChanged();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        editContext = new EditContext(MainViewModel.FontSettingsViewModel);
        messageStore = new ValidationMessageStore(editContext);
        ValidateForm();
        MainViewModel.RegisterComponent(nameof(FontSettingsComponent), this);
    }

    public void ForceRender()
    {
        StateHasChanged();
    }
}
