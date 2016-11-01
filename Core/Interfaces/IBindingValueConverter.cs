using System;
using System.Globalization;

namespace BindingEngine.Droid.Converters
{
    public interface IBindingValueConverter<TSource, TTarget> 
    {
        Func<TSource, TTarget> Convert { get; }
        Func<TTarget, TSource> ConvertBack { get; }

        //object Convert(object value, Type targetType, object parameter, CultureInfo culture);
        //object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
    }
}