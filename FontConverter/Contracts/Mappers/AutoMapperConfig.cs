using AutoMapper;
using LVGLFontConverter.Library.Models;
using LVGLFontConverter.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace LVGLFontConverter.Contracts.Mappers;

public class AutoMapperConfig : Profile
{
    public AutoMapperConfig()
    {
        CreateMap<LVGLFontData, FontDataViewModel>().ReverseMap();
        CreateMap<LVGLFontProperties, FontPropertiesViewModel>().ReverseMap();
        CreateMap<LVGLFontAdjusment, FontAdjusmentViewModel>().ReverseMap();
    }
}