
using AutoMapper;
using FontConverter.Blazor.ViewModels;
using FontConverter.SharedLibrary.Models;
using Radzen.Blazor.Rendering;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FontConverter.Blazor.Helpers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Mappings From Model To ViewModel
        CreateMap<LVGLFontSettings, FontSettingsViewModel>();
        CreateMap<LVGLFontAdjusments, FontAdjusmentsViewModel>();
        CreateMap<LVGLFontInformations, FontInformationsViewModel>();

        CreateMap<LVGLFontContent, FontContentViewModel>()
            .ForMember(dest => dest.Header, opt => opt.MapFrom(src => src.Header ?? string.Empty))
            .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.Count))
            .ForMember(dest => dest.IsSelected, opt => opt.MapFrom(src => src.IsSelected))
            .ForMember(dest => dest.Contents, opt => opt.MapFrom((src, dest, destMember, context) =>
                context.Mapper.Map<SortedList<string, FontContentViewModel>>(src.Contents ?? new SortedList<string, LVGLFontContent>())));

        CreateMap<LVGLFontContents, FontContentsViewModel>()
            .IncludeBase<LVGLFontContent, FontContentViewModel>()
            .ForMember(dest => dest.GlyphsCount, opt => opt.MapFrom(src => src.GlyphsCount))
            .ForMember(dest => dest.EmptyGlyphsCount, opt => opt.MapFrom(src => src.EmptyGlyphsCount))
            .ForMember(dest => dest.UnMappedGlyphsCount, opt => opt.MapFrom(src => src.UnMappedGlyphsCount))
            .ForMember(dest => dest.SingleMappedGlyphsCount, opt => opt.MapFrom(src => src.SingleMappedGlyphsCount))
            .ForMember(dest => dest.MultiMappedGlyphsCount, opt => opt.MapFrom(src => src.MultiMappedGlyphsCount))
            .ForMember(dest => dest.UnicodesCount, opt => opt.MapFrom(src => src.UnicodesCount));

        CreateMap<SortedList<string, LVGLFontContent>, SortedList<string, FontContentViewModel>>()
            .ConvertUsing((src, dest, context) =>
            {
                var result = new SortedList<string, FontContentViewModel>();
                if (src != null)
                {
                    foreach (var item in src)
                    {
                        result.Add(item.Key, context.Mapper.Map<FontContentViewModel>(item.Value));
                    }
                }
                return result;
            });

        // Mappings From ViewModel To Model
        CreateMap<FontSettingsViewModel, LVGLFontSettings>();
        CreateMap<FontAdjusmentsViewModel, LVGLFontAdjusments>();
        CreateMap<FontInformationsViewModel, LVGLFontInformations>();

        CreateMap<FontContentViewModel, LVGLFontContent>()
            .ForMember(dest => dest.Header, opt => opt.MapFrom(src => src.Header))
            .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.Count))
            .ForMember(dest => dest.IsSelected, opt => opt.MapFrom(src => src.IsSelected))
            .ForMember(dest => dest.Contents, opt => opt.MapFrom((src, dest, destMember, context) =>
                context.Mapper.Map<SortedList<string, LVGLFontContent>>(src.Contents ?? new SortedList<string, FontContentViewModel>())));

        CreateMap<FontContentsViewModel, LVGLFontContents>()
            .IncludeBase<FontContentViewModel, LVGLFontContent>()
            .ForMember(dest => dest.GlyphsCount, opt => opt.MapFrom(src => src.GlyphsCount))
            .ForMember(dest => dest.EmptyGlyphsCount, opt => opt.MapFrom(src => src.EmptyGlyphsCount))
            .ForMember(dest => dest.UnMappedGlyphsCount, opt => opt.MapFrom(src => src.UnMappedGlyphsCount))
            .ForMember(dest => dest.SingleMappedGlyphsCount, opt => opt.MapFrom(src => src.SingleMappedGlyphsCount))
            .ForMember(dest => dest.MultiMappedGlyphsCount, opt => opt.MapFrom(src => src.MultiMappedGlyphsCount))
            .ForMember(dest => dest.UnicodesCount, opt => opt.MapFrom(src => src.UnicodesCount));

        CreateMap<SortedList<string, FontContentViewModel>, SortedList<string, LVGLFontContent>>()
            .ConvertUsing((src, dest, context) =>
            {
                var result = new SortedList<string, LVGLFontContent>();
                if (src != null)
                {
                    foreach (var item in src)
                    {
                        result.Add(item.Key, context.Mapper.Map<LVGLFontContent>(item.Value));
                    }
                }
                return result;
            });
    }

    private SortedList<string, FontContentViewModel> MapContents(
        SortedList<string, LVGLFontContent> source, ResolutionContext context)
    {
        var result = new SortedList<string, FontContentViewModel>();
        if (source != null)
        {
            foreach (var item in source)
            {
                result.Add(item.Key, context.Mapper.Map<FontContentViewModel>(item.Value));
            }
        }
        return result;
    }
}