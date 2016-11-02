using System;
using System.Globalization;
using Android.Views;
using BindingEngine.Droid.Converters;

namespace SampleBindingEngine.Core
{
    public class BooleanToStringConverter : IBindingValueConverter<bool, string>
    {
        public Func<bool, string> Convert
        {
            get
            {
                return b => b.ToString();
            }
        }

        public Func<string, bool> ConvertBack
        {
            get
            {
                return s =>
                {
                    bool result;

                    bool.TryParse(s.ToString(), out result);

                    return result;
                };
            }
        }


    }

    public class StringToVisibilityConverter : IBindingValueConverter<string, ViewStates>
    {
        public Func<string, ViewStates> Convert
        {
            get
            {
                return v =>
                {
                    if ((string)v == "asd")
                    {
                        return ViewStates.Invisible;
                    }
                    else
                    {
                        return ViewStates.Visible;
                    }
                };
            }
        }

        public Func<ViewStates, string> ConvertBack
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }



    //public class BoolToVisibleInvisibleConverter : IBindingValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        switch ((bool)value)
    //        {
    //            case false:
    //                return ViewStates.Invisible;
    //            case true:
    //            default:
    //                return ViewStates.Visible;

    //        };
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();

    //    }
    //}

    //public class BoolToVisibleGoneConverter : IBindingValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {

    //        switch ((bool)value)
    //        {
    //            case false:
    //                return ViewStates.Gone;
    //            case true:
    //            default:
    //                return ViewStates.Visible;
    //        }

    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}

