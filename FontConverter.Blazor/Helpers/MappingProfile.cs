using AutoMapper;
using FontConverter.Blazor.ViewModels;
using FontConverter.SharedLibrary.Models;

namespace FontConverter.Blazor.Helpers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Mappings From Model To ViewModel
        CreateMap<LVGLFontSettings, FontSettingsViewModel>();
        CreateMap<LVGLFontAdjusments, FontAdjusmentsViewModel>();
        CreateMap<LVGLFontInformations, FontInformationsViewModel>();
        CreateMap<LVGLGlyphViewItemProperties, GlyphViewItemPropertiesViewModel>();

        CreateMap<SortedList<string, LVGLFontContent>, SortedList<string, FontContentViewModel>>()
            .ConvertUsing((src, dest, context) =>
            {
                var result = new SortedList<string, FontContentViewModel>();
                if (src != null)
                {
                    foreach (var item in src)
                    {
                        if (item.Value != null)
                        {
                            result.Add(item.Key, context.Mapper.Map<FontContentViewModel>(item.Value));
                        }
                    }
                }
                return result;
            });

        CreateMap<LVGLFontContent, FontContentViewModel>()
            .ForMember(dest => dest.Header, opt => opt.MapFrom(src => src.Header ?? string.Empty))
            .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.Count))
            .ForMember(dest => dest.IsSelected, opt => opt.MapFrom(src => src.IsSelected))
            .ForMember(dest => dest.Contents, opt => opt.MapFrom((src, dest, destMember, context) =>
                context.Mapper.Map<SortedList<string, LVGLFontContent>>(src.Contents ?? new SortedList<string, LVGLFontContent>())));

        CreateMap<LVGLFontContents, FontContentsViewModel>()
            .ForMember(dest => dest.Header, opt => opt.MapFrom(src => src.Header ?? string.Empty))
            .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.Count))
            .ForMember(dest => dest.IsSelected, opt => opt.MapFrom(src => src.IsSelected))
            .ForMember(dest => dest.Contents, opt => opt.MapFrom((src, dest, destMember, context) =>
                context.Mapper.Map<SortedList<string, LVGLFontContent>>(src.Contents ?? new SortedList<string, LVGLFontContent>())));

        // Mappings From ViewModel To Model
        CreateMap<FontSettingsViewModel, LVGLFontSettings>();
        CreateMap<FontAdjusmentsViewModel, LVGLFontAdjusments>();
        CreateMap<FontInformationsViewModel, LVGLFontInformations>();
        CreateMap<GlyphViewItemPropertiesViewModel, LVGLGlyphViewItemProperties>();

        CreateMap<SortedList<string, FontContentViewModel>, SortedList<string, LVGLFontContent>>()
            .ConvertUsing((src, dest, context) =>
            {
                var result = new SortedList<string, LVGLFontContent>();
                if (src != null)
                {
                    foreach (var item in src)
                    {
                        if (item.Value != null)
                        {
                            result.Add(item.Key, context.Mapper.Map<LVGLFontContent>(item.Value));
                        }
                    }
                }
                return result;
            });

        CreateMap<FontContentViewModel, LVGLFontContent>()
            .ForMember(dest => dest.Header, opt => opt.MapFrom(src => src.Header))
            .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.Count))
            .ForMember(dest => dest.IsSelected, opt => opt.MapFrom(src => src.IsSelected))
            .ForMember(dest => dest.Contents, opt => opt.MapFrom((src, dest, destMember, context) =>
                context.Mapper.Map<SortedList<string, LVGLFontContent>>(src.Contents ?? new SortedList<string, FontContentViewModel>())));

        CreateMap<FontContentsViewModel, LVGLFontContents>()
            .ForMember(dest => dest.Header, opt => opt.MapFrom(src => src.Header))
            .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.Count))
            .ForMember(dest => dest.IsSelected, opt => opt.MapFrom(src => src.IsSelected))
            .ForMember(dest => dest.Contents, opt => opt.MapFrom((src, dest, destMember, context) =>
                context.Mapper.Map<SortedList<string, LVGLFontContent>>(src.Contents ?? new SortedList<string, FontContentViewModel>())));
    }
}