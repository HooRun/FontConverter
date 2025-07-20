using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LVGLFontConverter.Contracts.Converter;

public class DateTimeToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is DateTime dateTime)
        {
            string? format = parameter as string;
            if (string.IsNullOrEmpty(format))
            {
                format = "yyyy/MM/dd HH:mm:ss";
            }
            return dateTime.ToString(format, CultureInfo.CurrentCulture);
        }
        return string.Empty; 
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is string strValue)
        {
            string? format = parameter as string;
            if (string.IsNullOrEmpty(format))
            {
                format = "yyyy/MM/dd HH:mm:ss"; 
            }

            if (DateTime.TryParseExact(strValue, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
        }
        return DependencyProperty.UnsetValue;
    }
}
