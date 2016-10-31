using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Autofac;
using CheeseBind;
using GalaSoft.MvvmLight.Helpers;
using Java.Lang;
using Knuj.Interfaces.Views;

namespace Core.Droid
{
    public abstract class FragmentBase<T> : Fragment, IBackHandlerFragment where T : IControllerBase
    {
        public Stopwatch Stopwatch { get; set; }
        protected List<Binding> Bindings { get; set; }
        private readonly INavigationService _navigationService;
        public virtual T Controller { get; private set; }
        protected IContainerViewController ActivityContainer;
        public bool IsViewVisible { get; set; }

        protected FragmentBase(IntPtr javaReference, JniHandleOwnership jniHandleOwnership)
            : base(javaReference, jniHandleOwnership)
        {
        }

        protected FragmentBase()
        {
            _navigationService = Container.Instance.Resolve<INavigationService>();
            Controller = ResolveController();
            Bindings = new List<Binding>();
        }

        public View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState, int resourceId)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(resourceId, container, false);
            Cheeseknife.Bind(this, view);
            Bindings.AddRange(ReflectionUtils.Bind(this, Controller));
            ReflectionUtils.BindCommands(this, Controller);
            Bindings.AddRange(ReflectionUtils.BindImages(this, Controller));
            return view;
        }
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here

            if (!(Activity is IContainerViewController))
            {
                throw new ClassCastException("Hosting activity must implement IContainerViewController");
            }
            else
            {
                ActivityContainer = (IContainerViewController)Activity;
            }
        }

        public override void OnResume()
        {
            base.OnResume();
            Controller.Refresh();
        }

        public override void OnStart()
        {
            base.OnStart();
            ActivityContainer.SetSelectedFragment(this);
        }
        

        public void HideLoader()
        {
            //    _activityContainer.HideLoader();
        }

        public void ShowLoader(string loadingMessage)
        {
            //    _activityContainer.ShowLoader(loadingMessage);
        }

        public void ShowAlertMessage(string message, bool showNegative, Action positiveAction = null, Action negativeAction = null)
        {
            //   _activityContainer.ShowAlertMessage(message, showNegative, positiveAction, negativeAction);
        }

        public void ShowErrorMessage(string errorMessage, bool showNegativeButton, Action positiveAction = null, Action negativeAction = null)
        {
            //   _activityContainer.ShowErrorMessage(errorMessage, showNegativeButton, positiveAction, negativeAction);
        }

        public void HideKeyboard(object view)
        {
            //  _activityContainer.HideKeyboard(view);
        }

        /// <summary>
        /// Here, should return always false.
        /// Inherited classes that override this method should implement their own custom back behaviour at fragment level and then return true.
        /// </summary>
        /// <returns></returns>
        public virtual bool OnBackPressed()
        {
            return false;
        }

        private T ResolveController()
        {
            T controller;

            if (_navigationService.HasData())
            {
                controller = Container.Instance.Resolve<T>(new NamedParameter("data", _navigationService.GetData()));
            }
            else
            {
                controller = Container.Instance.Resolve<T>();
            }

            if (_navigationService.HasReturnData())
            {
                controller.ReverseInit(_navigationService.GetReturnData());
            }

            return controller;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            //foreach (var binding in Bindings)
            //{
            //    binding.Detach();
            //}

            //var views = GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(p => p.CustomAttributes.Any(m => m.AttributeType.Name == "BindView"));
            //foreach (var view in views)
            //{
            //    ((Java.Lang.Object)view.GetValue(this)).Dispose();
            //}
            //Dispose();
        }
    }
}