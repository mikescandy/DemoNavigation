using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Autofac;
using CheeseBind;
using GalaSoft.MvvmLight.Helpers;
using Knuj.Interfaces;
using Knuj.Interfaces.Views;
using static Core.Droid.ReflectionUtils;

namespace Core.Droid
{
    public class BindingOperation
    {
        public View Control { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public string Converter { get; set; }
        public string ConverterParameter { get; set; }
        public BindingMode Mode { get; set; }
    }

    public abstract class ActivityBase<T> : AppCompatActivity, IView, IUiStatusNotifier, IContainerViewController where T : IControllerBase
    {
        protected List<Binding> Bindings { get; set; }
        private readonly INavigationService _navigationService;
        public virtual T Controller { get; private set; }
        private List<Fragment> _fragmentCache;

        protected ActivityBase(IntPtr javaReference, JniHandleOwnership jniHandleOwnership)
            : base(javaReference, jniHandleOwnership)
        {
        }

        protected ActivityBase()
        {
            _navigationService = Container.Instance.Resolve<INavigationService>();
            Controller = ResolveController();
            Bindings = new List<Binding>();
            _fragmentCache = new List<Fragment>();
        }

        protected void OnCreate(Bundle savedInstanceState, int resourceId)
        {
            base.OnCreate(savedInstanceState);
            BindingEngine.Droid.BindingEngine.Initialize(this, resourceId);

            SetContentView(resourceId);
            Cheeseknife.Bind(this);
            Bindings.AddRange(Bind(this, Controller));
            BindCommands(this, Controller);
            Bindings.AddRange(BindImages(this, Controller));
        }

        protected override void OnResume()
        {
            base.OnResume();
            OnForeground(this);
            Controller.Refresh();
            if (_navigationService.HasReturnData())
            {
                Controller.ReverseInit(_navigationService.GetReturnData());
            }
            if (_navigationService.HasSubView())
            {
                var subView = _navigationService.GetSubView();
                (this as IContainerViewController).ShowController(subView);
            }
        }

        protected override void OnPause()
        {
            OnBackground(this);
            base.OnPause();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var binding in Bindings)
            {
                binding.Detach();
            }

            var views = GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(p => p.CustomAttributes.Any(m => m.AttributeType.Name == "BindView"));
            foreach (var view in views)
            {
                ((Java.Lang.Object)view.GetValue(this)).Dispose();
            }
            Dispose();
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

        public virtual bool OnForeground(IView view)
        {
            if (!(Controller is IUiStatusNotifier)) return false;
            ((IUiStatusNotifier)Controller).OnForeground(this);

            ((ApplicationBase)Application).CurrentContext = this;
            return true;
        }

        public virtual bool OnBackground(IView view)
        {
            if (!(Controller is IUiStatusNotifier)) return false;
            ((IUiStatusNotifier)Controller).OnBackground(this);
            return true;
        }

        public virtual int GetContainerViewId()
        {
            return -1;
        }

        public virtual void SetTitle(string title)
        {
        }

        public virtual void SetSelectedFragment(IBackHandlerFragment backHandledFragment)
        {

        }

        public virtual void ShowController(Type type)
        {
            var targetType = _navigationService.GetViewForController(type);
            var fragment = (Fragment)Activator.CreateInstance(targetType);

            if (false) // TODO reimplement add to back stack
            {
                SupportFragmentManager.BeginTransaction()
                    .Replace(GetContainerViewId(), fragment)
                    .AddToBackStack("")
                    .Commit();
            }
            else
            {
                if (SupportFragmentManager.Fragments != null && SupportFragmentManager.Fragments.Any())
                {
                    SupportFragmentManager.BeginTransaction()
                        .Replace(GetContainerViewId(), fragment)
                        .Commit();
                }
                else
                {
                    SupportFragmentManager.BeginTransaction()
                        .Add(GetContainerViewId(), fragment)
                        .Commit();
                }
            }

        }

        public virtual void ShowController()
        {
            var targetType = _navigationService.GetSubView();
            ShowController(targetType);
        }
    }
}