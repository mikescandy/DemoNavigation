using System;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Autofac;
using CheeseBind;
using GalaSoft.MvvmLight.Helpers;

namespace Core.Droid
{
    public abstract class ActivityBase<T> : Activity where T : IControllerBase
    {
        protected Binding Bindings { get; set; }
        private readonly INavigationService _navigationService;
        protected virtual T Controller { get; private set; }

        protected ActivityBase()
        {
            _navigationService = Core.Application.Instance.Container.Resolve<INavigationService>();
            Controller = ResolveController();
        }

        protected void OnCreate(Bundle savedInstanceState, int resourceId)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(resourceId);
            Cheeseknife.Bind(this);
            BindString();
            BindCommands();
            BindImages();
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (_navigationService.HasReturnData())
            {
                Controller.ReverseInit(_navigationService.GetReturnData());
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            var bindings = GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(p => p.PropertyType.IsAssignableFrom(typeof(Binding)) || p.GetType().IsAssignableFrom(typeof(Binding<,>)));
            foreach (var binding in bindings)
            {
                var o = binding.GetValue(this);
                if (o is Binding)
                {
                    ((Binding)o).Detach();
                }
            }

            var views = GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(p => p.CustomAttributes.Any(m => m.AttributeType.Name == "BindView"));
            foreach (var view in views)
            {
                ((Java.Lang.Object)view.GetValue(this)).Dispose();
            }
        }

        private T ResolveController()
        {
            T controller;

            if (_navigationService.HasData())
            {
                controller = Core.Application.Instance.Container.Resolve<T>(new NamedParameter("data", _navigationService.GetData()));
            }
            else
            {
                controller = Core.Application.Instance.Container.Resolve<T>();
            }

            if (_navigationService.HasReturnData())
            {
                controller.ReverseInit(_navigationService.GetReturnData());
            }

            return controller;
        }

        private void BindString()
        {
            var bindableProperties = GetType().GetProperties().Where(p => p.CustomAttributes.Any(a => a.AttributeType.Name == "BindAttribute"));
            foreach (var bindableProperty in bindableProperties)
            {
                var bindableAttributes = bindableProperty.GetCustomAttributes(typeof(BindAttribute)).Select(m => m as BindAttribute);

                foreach (var bindableAttribute in bindableAttributes)
                {
                    if (bindableAttribute != null)
                    {
                        var myMethod = typeof(GalaSoft.MvvmLight.Helpers.Extensions)
                             .GetMethods()
                             .Where(m => m.Name == "SetBinding")
                             .Select(m => new
                             {
                                 Method = m,
                                 Params = m.GetParameters(),
                                 Args = m.GetGenericArguments()
                             })
                             .Where(x => x.Params.Length == 7
                                         && x.Args.Length == 2
                                         && x.Params[1].ParameterType == typeof(string)
                                          )
                             .Select(x => x.Method)
                             .First();


                        var generic = myMethod.MakeGenericMethod(Controller.GetType().GetProperty(bindableAttribute.Source).PropertyType, bindableProperty.GetValue(this).GetType().GetProperty(bindableAttribute.Target).PropertyType);
                        var binding = generic.Invoke(bindableProperty.GetValue(this), new object[] { Controller, bindableAttribute.Source, bindableProperty.GetValue(this), bindableAttribute.Target, bindableAttribute.BindingMode, null, null });

                        if (!string.IsNullOrEmpty(bindableAttribute.SourceToTargetConverter))
                        {
                            var convertSourceToTargetMethod = binding.GetType().GetMethod("ConvertSourceToTarget");
                            var convertSourceToTargetFunc = bindableAttribute.Converters.GetProperty(bindableAttribute.SourceToTargetConverter, BindingFlags.Static | BindingFlags.Public).GetValue(null);
                            convertSourceToTargetMethod.Invoke(binding, new[] { convertSourceToTargetFunc });
                        }

                        if (!string.IsNullOrEmpty(bindableAttribute.TargetToSourceConverter))
                        {
                            var convertTargetToSourceMethod = binding.GetType().GetMethod("ConvertTargetToSource");
                            var convertTargetToSourceFunc = bindableAttribute.Converters.GetProperty(bindableAttribute.TargetToSourceConverter, BindingFlags.Static | BindingFlags.Public).GetValue(null);
                            convertTargetToSourceMethod.Invoke(binding, new[] { convertTargetToSourceFunc });
                        }
                    }
                }
            }
        }

        private void BindImages()
        {
            var bindableProperties = GetType().GetProperties().Where(p => p.CustomAttributes.Any(a => a.AttributeType.Name == "BindImageAttribute"));
            foreach (var bindableProperty in bindableProperties)
            {
                var bindableAttribute = bindableProperty.GetCustomAttribute(typeof(BindImageAttribute)) as BindImageAttribute;
                if (bindableAttribute != null)
                {
                    Controller.SetBinding<IControllerBase, ImageView>(bindableAttribute.Source, bindableProperty.GetValue(this)).WhenSourceChanges(() =>

                             {
                                 var b = (byte[])Controller.GetType().GetProperty(bindableAttribute.Source).GetValue(Controller);
                                 var iv = ((ImageView)bindableProperty.GetValue(this));
                                 if (b != null && b.Any())
                                 {
                                     iv.SetImageBitmap(BitmapFactory.DecodeByteArray(b, 0, b.Length));
                                     iv.Invalidate();
                                 }
                             });
                }
            }
        }

        private void BindCommands()
        {
            var bindableProperties = GetType().GetProperties().Where(p => p.CustomAttributes.Any(a => a.AttributeType.Name == "BindCommandAttribute"));
            foreach (var bindableProperty in bindableProperties)
            {
                var bindableAttribute = bindableProperty.GetCustomAttribute(typeof(BindCommandAttribute)) as BindCommandAttribute;
                if (bindableAttribute != null)
                {
                    bindableProperty.GetValue(this).SetCommand(bindableAttribute.Target, (ICommand)Controller.GetType().GetProperty(bindableAttribute.Source).GetValue(Controller));
                }
            }
        }
    }
}