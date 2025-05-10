using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LVGLFontConverter.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}