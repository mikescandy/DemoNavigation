using System;
using Android.Views;

namespace Core.Droid
{
    public static class BaseConverters
    {

        public static Func<bool, ViewStates> BoolToVisibleInvisibleConverter
        {
            get
            {
                return boolean =>
                {
                    switch (boolean)
                    {
                        case false:
                            return ViewStates.Invisible;
                        case true:
                        default:
                            return ViewStates.Visible;
                    }
                };
            }
        }
        public static Func<bool, ViewStates> BoolToVisibleGoneConverter
        {
            get
            {
                return boolean =>
                {
                    switch (boolean)
                    {
                        case false:
                            return ViewStates.Gone;
                        case true:
                        default:
                            return ViewStates.Visible;
                    }
                };
            }
        }
    }
}