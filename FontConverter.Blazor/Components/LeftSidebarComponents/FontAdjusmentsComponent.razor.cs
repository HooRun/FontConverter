﻿using FontConverter.Blazor.Services;
using FontConverter.Blazor.ViewModels;
using FontConverter.SharedLibrary;
using FontConverter.SharedLibrary.Helpers;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace FontConverter.Blazor.Components.LeftSidebarComponents;

public partial class FontAdjusmentsComponent : ComponentBase
{
    [Inject]
    public PredefinedDataService PredefinedData { get; set; } = default!;

    [Inject]
    public MainViewModel MainViewModel { get; set; } = default!;

    int colLeft = 4;
    int colRight = 8;

    Variant variant = Variant.Outlined;
    bool floatFieldLabel = true;

}
