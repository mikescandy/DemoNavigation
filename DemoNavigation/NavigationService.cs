using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Autofac;
using Core;
using Java.Lang;
using Java.Util;

namespace DemoNavigation
{
    public class NavigationService : INavigationService
    {
        public void NavigateTo<T>() where T : IControllerBase
        {
            NavigateTo<T>(null);
        }

        public void NavigateTo<T>(object parameters) where T : IControllerBase
        {
            var view = typeof(T).BaseType.GetGenericArguments()[0];
            if (view.GetInterfaces().Contains(typeof(IView)))
            {
                object activity;
                if (parameters == null)
                {
                    activity = Core.Application.Instance.Container.Resolve(view);
                }
                else
                {
                    activity = Core.Application.Instance.Container.Resolve(view, new NamedParameter("parameters", parameters));
                }
                GetActivity().StartActivity(activity.GetType());
            }
        }

        public static Activity GetActivity()
        {

            Activity activity = null;
            List<Java.Lang.Object> objects = null;

            var activityThreadClass = Class.ForName("android.app.ActivityThread");
            var activityThread = activityThreadClass.GetMethod("currentActivityThread").Invoke(null);
            var activityFields = activityThreadClass.GetDeclaredField("mActivities");
            activityFields.Accessible = true;

            var obj = activityFields.Get(activityThread);

            if (obj is JavaDictionary)
            {
                var activities = (JavaDictionary)obj;
                objects = new List<Java.Lang.Object>(activities.Values.Cast<Java.Lang.Object>().ToList());
            }
            else if (obj is ArrayMap)
            {
                var activities = (ArrayMap)obj;
                objects = new List<Java.Lang.Object>(activities.Values().Cast<Java.Lang.Object>().ToList());
            }
            else if (obj is IMap)
            {
                var activities = (IMap)activityFields.Get(activityThread);
                objects = new List<Java.Lang.Object>(activities.Values().Cast<Java.Lang.Object>().ToList());
            }

            if (objects != null && objects.Any())
            {
                foreach (var activityRecord in objects)
                {
                    var activityRecordClass = activityRecord.Class;
                    var pausedField = activityRecordClass.GetDeclaredField("paused");
                    pausedField.Accessible = true;

                    if (!pausedField.GetBoolean(activityRecord))
                    {
                        var activityField = activityRecordClass.GetDeclaredField("activity");
                        activityField.Accessible = true;
                        activity = (Activity)activityField.Get(activityRecord);
                        break;
                    }
                }
            }

            return activity;
        }
    }
}