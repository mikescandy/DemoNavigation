using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Java.Lang;
using Java.Util;

namespace Core.Droid
{
    public class NavigationService : INavigationService
    {
        private readonly Stack<object> _data;
        private readonly Stack<object> _returnData;

        private readonly List<Type> _registeredTypes;

        public NavigationService()
        {
            _registeredTypes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                from type in assembly.GetTypes()
                                where Attribute.IsDefined(type, typeof(ActivityAttribute))
                                select type).ToList();
            _data = new Stack<object>();
            _returnData = new Stack<object>();
        }

        public bool HasData()
        {
            return _data.Any();
        }

        public bool HasReturnData()
        {
            return _returnData.Any();
        }

        public object GetData()
        {
            if (HasData())
            {
                return _data.Pop();
            }
            else
            {
                return null;
            }
        }

        public object GetReturnData()
        {
            if (HasReturnData())
            {
                return _returnData.Pop();
            }
            else
            {
                return null;
            }
        }

        public void NavigateTo<T>() where T : IControllerBase
        {
            NavigateTo<T>(null, false);
        }

        public void NavigateTo<T>(bool noHistory) where T : IControllerBase
        {
            NavigateTo<T>(null, noHistory);
        }

        public void NavigateTo<T>(object data) where T : IControllerBase
        {
            NavigateTo<T>(data, false);
        }

        public void NavigateTo<T>(object data, bool noHistory) where T : IControllerBase
        {
            var targetActivity = _registeredTypes.FirstOrDefault(m => m.BaseType == typeof(ActivityBase<>) &&  m.BaseType.GetGenericArguments().Any() && m.BaseType.GetGenericArguments()[0] == typeof(T));
            if (data != null)
            {
                _data.Push(data);
            }

            var intent = new Intent(Android.App.Application.Context, targetActivity);
            if (noHistory)
            {
                intent.SetFlags(ActivityFlags.NoHistory);
            }
            var currentActivity = GetActivity();

            if (currentActivity != null)
            {
                GetActivity().StartActivity(intent);
            }
            else
            {

            }
        }

        public void Close()
        {
            GetActivity().Finish();
        }

        public void Back()
        {
            GetActivity().OnBackPressed();
        }

        public void Close(object data)
        {
            _returnData.Push(data);
            GetActivity().Finish();
        }

        public void Back(object data)
        {
            _returnData.Push(data);
            GetActivity().OnBackPressed();
        }

        public void SetResult(object data)
        {
            if (data != null)
            {
                _data.Push(data);
            }
        }

        private static Activity GetActivity()
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