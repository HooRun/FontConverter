using System;

namespace LVGLFontConverter.Contracts.Services;

public interface IPageService
{
    Type GetPageType(string key);
}
