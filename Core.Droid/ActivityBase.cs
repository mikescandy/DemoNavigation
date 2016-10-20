using System;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Android.App;
using Android.Graphics;
using Android.OS;
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
                var bindableAttributes = bindableProperty.GetCustomAttributes(typeof(BindAttribute)).Select(m=>m as BindAttribute);

                foreach (var bindableAttribute in bindableAttributes)
                {
                    if (bindableAttribute != null)
                    {
                        var mo = typeof(GalaSoft.MvvmLight.Helpers.Extensions).GetMethods(BindingFlags.Static | BindingFlags.Public);
                        var mmm = typeof(GalaSoft.MvvmLight.Helpers.Extensions)
                            .GetMethod("SetBinding", BindingFlags.Static | BindingFlags.Public, null,
                            new[] { typeof(string), typeof(object), typeof(string), typeof(BindingMode), typeof(string), typeof(string) }, null);
                        var mm = typeof(GalaSoft.MvvmLight.Helpers.Extensions).GetMethod("SetBinding", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { typeof(string), typeof(string) }, null);

                        var mrmrmr = typeof(GalaSoft.MvvmLight.Helpers.Extensions).GetMethodExt("SetBinding",
                            new[]
                            {
                            typeof(string), typeof(object), typeof(string), typeof(BindingMode), typeof(string), typeof(string)
                            });
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




                        var dsdm = typeof(GalaSoft.MvvmLight.Helpers.Extensions).GetMethod("SetBinding", new[] { bindableProperty.PropertyType, Controller.GetType().GetProperty(bindableAttribute.Source).PropertyType });

                        var generic = myMethod.MakeGenericMethod(bindableProperty.GetValue(this).GetType().GetProperty(bindableAttribute.Target).PropertyType, Controller.GetType().GetProperty(bindableAttribute.Source).PropertyType);

                        var binding = generic.Invoke(bindableProperty.GetValue(this), new object[] { bindableProperty.GetValue(this), bindableAttribute.Target, Controller, bindableAttribute.Source, bindableAttribute.BindingMode, null, null });

                        if (bindableAttribute.SourceToTargetConverter == null) continue;
                           
                        var ps = bindableAttribute.Converters.GetFields(BindingFlags.Static | BindingFlags.Public);



                        var convertSourceToTargetFunc = bindableAttribute.Converters.GetProperty(bindableAttribute.SourceToTargetConverter, BindingFlags.Static | BindingFlags.Public);
                        var convertTargetToSourceFunc = bindableAttribute.Converters.GetProperty(bindableAttribute.TargetToSourceConverter, BindingFlags.Static | BindingFlags.Public);


                        var convertSourceToTargetMethod = typeof(Binding<,>).GetMethod("ConvertSourceToTarget", BindingFlags.Static | BindingFlags.Public);

                        var convertTargetToSourceMethod = typeof(Binding<,>).GetMethod("ConvertTargetToSource", BindingFlags.Static | BindingFlags.Public);

                        var funcType = typeof(Func<,>).MakeGenericType(bindableProperty.GetValue(this).GetType().GetProperty(bindableAttribute.Target).PropertyType, Controller.GetType().GetProperty(bindableAttribute.Source).PropertyType);
                        var funcType2 = typeof(Func<,>).MakeGenericType(Controller.GetType().GetProperty(bindableAttribute.Source).PropertyType, bindableProperty.GetValue(this).GetType().GetProperty(bindableAttribute.Target).PropertyType);

                        var s2t = Delegate.CreateDelegate(funcType2, convertSourceToTargetFunc.GetMethod);
                        var t2s = Delegate.CreateDelegate(funcType, convertTargetToSourceFunc.GetMethod);
                        convertSourceToTargetMethod.Invoke(binding, new[] { s2t });

                        convertTargetToSourceMethod.Invoke(binding, new[] { t2s });


                        var bb = bindableProperty.GetValue(this).SetBinding<string, string>(bindableAttribute.Target, Controller, bindableAttribute.Source, bindableAttribute.BindingMode);




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