using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Java.Lang;
using Java.Util;
using Knuj.Interfaces;
using Knuj.Interfaces.Views;
using Plugin.CurrentActivity;

namespace Core.Droid
{
    public class NavigationService : INavigationService
    {
        private readonly Stack<object> _data;
        private readonly Stack<object> _returnData;
        private readonly Stack<Type> _subNavigationStack;

        private readonly List<Type> _registeredTypes;

        public NavigationService()
        {
            _registeredTypes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                from type in assembly.GetTypes()
                                where
                                Attribute.IsDefined(type, typeof(ActivityAttribute)) ||
                                IsSubclassOfRawGeneric(typeof(FragmentBase<>), type)
                                select type).ToList();
            _data = new Stack<object>();
            _returnData = new Stack<object>();
            _subNavigationStack = new Stack<Type>();
        }

        public bool HasData()
        {
            return _data.Any();
        }

        public bool HasReturnData()
        {
            return _returnData.Any();
        }

        public bool HasSubView()
        {
            return _subNavigationStack.Any();
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

        public Type GetSubView()
        {
            if (HasSubView())
            {
                return _subNavigationStack.Pop();
            }
            else
            {
                return null;
            }
        }

        public void NavigateTo<T>() where T : IControllerBase
        {
            InternalNavigateTo<T>(null, false, false, null, null);
        }

        public void NavigateTo<T>(bool noHistory) where T : IControllerBase
        {
            InternalNavigateTo<T>(null, noHistory, false, null, null);
        }

        public void NavigateTo<T>(object data) where T : IControllerBase
        {
            InternalNavigateTo<T>(data, false, false, null, null);
        }

        public void NavigateTo<T>(object data, bool noHistory) where T : IControllerBase
        {
            InternalNavigateTo<T>(data, noHistory, false, null, null);
        }

        public void NavigateToRoot<T>() where T : IControllerBase
        {
            InternalNavigateTo<T>(null, false, true, null, null);
        }

        public void NavigateToRoot<T>(object data) where T : IControllerBase
        {
            InternalNavigateTo<T>(data, false, true, null, null);
        }

        public void InternalNavigateTo<T>(object data, bool noHistory, bool clearStack, Type containerControllerType, Type subViewController) where T : IControllerBase
        {
            Type targetActivity;
            if (containerControllerType == null)
            {
                targetActivity = _registeredTypes.FirstOrDefault(m => IsSubclassOfRawGeneric(typeof(ActivityBase<>), m) && m.BaseType.GetGenericArguments().Any() && m.BaseType.GetGenericArguments()[0] == typeof(T));
            }
            else
            {
                targetActivity = _registeredTypes.FirstOrDefault(m => IsSubclassOfRawGeneric(typeof(ActivityBase<>), m) && m.BaseType.GetGenericArguments().Any() && m.BaseType.GetGenericArguments()[0] == containerControllerType);
            }
            if (data != null)
            {
                _data.Push(data);
            }

            var intent = new Intent(Application.Context, targetActivity);
            if (noHistory)
            {
                intent.SetFlags(ActivityFlags.NoHistory);
            }

            if (clearStack)
            {
                intent.SetFlags(ActivityFlags.ClearTask);
            }

            if (subViewController != null)
            {
                _subNavigationStack.Push(subViewController);
            }

            var currentActivity = GetActivity();

            if (currentActivity != null && currentActivity.GetType() != targetActivity)
            {
                currentActivity.RunOnUiThread(() => currentActivity.StartActivity(intent));
            }
            else if (currentActivity != null && currentActivity.GetType() == targetActivity && subViewController != null)
            {
                currentActivity.RunOnUiThread(() => ((IContainerViewController)currentActivity).ShowController());
            }
            else
            {

            }
            if (clearStack)
            {
                currentActivity.RunOnUiThread(() => GetActivity().Finish());
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

        public Type GetViewForController<T>() where T : IControllerBase
        {
            return _registeredTypes.FirstOrDefault(m => IsSubclassOfRawGeneric(typeof(FragmentBase<>), m) && m.BaseType.GetGenericArguments().Any() && m.BaseType.GetGenericArguments()[0] == typeof(T));
        }

        public Type GetViewForController(Type type)
        {
            return _registeredTypes.FirstOrDefault(m => IsSubclassOfRawGeneric(typeof(FragmentBase<>), m) && m.BaseType.GetGenericArguments().Any() && m.BaseType.GetGenericArguments()[0] == type);
        }


        static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        private static Activity GetActivity()
        {
            return CrossCurrentActivity.Current.Activity;
            //Activity activity = null;
            //List<Java.Lang.Object> objects = null;

            //var activityThreadClass = Class.ForName("android.app.ActivityThread");
            //var activityThread = activityThreadClass.GetMethod("currentActivityThread").Invoke(null);
            //var activityFields = activityThreadClass.GetDeclaredField("mActivities");
            //activityFields.Accessible = true;

            //var obj = activityFields.Get(activityThread);

            //if (obj is JavaDictionary)
            //{
            //    var activities = (JavaDictionary)obj;
            //    objects = new List<Java.Lang.Object>(activities.Values.Cast<Java.Lang.Object>().ToList());
            //}
            //else if (obj is ArrayMap)
            //{
            //    var activities = (ArrayMap)obj;
            //    objects = new List<Java.Lang.Object>(activities.Values().Cast<Java.Lang.Object>().ToList());
            //}
            //else if (obj is IMap)
            //{
            //    var activities = (IMap)activityFields.Get(activityThread);
            //    objects = new List<Java.Lang.Object>(activities.Values().Cast<Java.Lang.Object>().ToList());
            //}

            //if (objects != null && objects.Any())
            //{
            //    foreach (var activityRecord in objects)
            //    {
            //        var activityRecordClass = activityRecord.Class;
            //        var pausedField = activityRecordClass.GetDeclaredField("paused");
            //        pausedField.Accessible = true;

            //        if (!pausedField.GetBoolean(activityRecord))
            //        {
            //            var activityField = activityRecordClass.GetDeclaredField("activity");
            //            activityField.Accessible = true;
            //            activity = (Activity)activityField.Get(activityRecord);
            //            break;
            //        }
            //    }
            //}

            //return activity;
        }

        public void NavigateToSubView<TContainer, TTarget>()
            where TContainer : IContainerController
            where TTarget : IControllerBase
        {
            InternalNavigateTo<TTarget>(null, false, false, typeof(TContainer), typeof(TTarget));
        }
    }
}