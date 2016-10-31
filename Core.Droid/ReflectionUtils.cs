using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Android.Graphics;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using Knuj;

namespace Core.Droid
{
    internal static class ReflectionUtils
    {
        private static object GetNestedPropertyValue(this object obj, string name)
        {
            foreach (var part in name.Split('.'))
            {
                if (obj == null) { return null; }

                var type = obj.GetType();
                var info = type.GetProperty(part);
                if (info == null) { return null; }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }

        public static System.Type GetNestedProperty(this System.Type type, string treeProperty, object givenValue)
        {
            var properties = treeProperty.Split('.');
            var value = givenValue;
            var p = type;
            foreach (var property in properties/*.Take(properties.Length - 1)*/)
            {
                p = p.GetProperty(property).PropertyType;
               // value = value.GetType().GetProperty(property).GetValue(value);

               // if (value == null)
              //  {
              //      return null;
              //  }
            }

            return p;//value.GetType().GetProperty(properties[properties.Length - 1]);
        }

        public static object GetDefaultValue(Type type)
        {
            if (type == typeof(string))
            {
                return string.Empty;
            }
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        public static object GetNestedPropertyValue(this System.Type type, string treeProperty, object givenValue)
        {
            var properties = treeProperty.Split('.');
            return properties.Aggregate(givenValue, (current, property) => current.GetType().GetProperty(property).GetValue(current));
        }


        internal static IEnumerable<Binding> Bind(object target, IControllerBase source)
        {
            var bindings = new List<Binding>();
            var bindableProperties = target.GetType().GetProperties().Where(p => p.CustomAttributes.Any(a => a.AttributeType.Name == "BindAttribute"));
            foreach (var bindableProperty in bindableProperties)
            {
                var bindableAttributes = bindableProperty.GetCustomAttributes(typeof(BindAttribute)).Select(m => m as BindAttribute);
                var property = target.GetNestedPropertyValue(bindableProperty.Name);//  target.GetType().GetProperty(target.GetType().GetProperty(bindableProperty.Name).Name).GetValue(target);

                foreach (var bindableAttribute in bindableAttributes)
                {
                    var targetPropertyTree = bindableAttribute.Target.Split('.');
                    var targetProperty = targetPropertyTree.Length == 1
                        ? property
                        : property.GetNestedPropertyValue(string.Join(".", targetPropertyTree.SkipLastN(1)));
                    var targetPropertyName = targetPropertyTree.Last();
                    var targetPropertyType = targetProperty.GetType().GetProperty(targetPropertyName).PropertyType;//  property.GetType().GetProperty(bindableAttribute.Target).PropertyType;


                    var sourcePropertyTree = bindableAttribute.Source.Split('.');
                    var sourceProperty = sourcePropertyTree.Length == 1
                        ? source
                        : source.GetNestedPropertyValue(string.Join(".", sourcePropertyTree.SkipLastN(1)));
                    var sourcePropertyName = sourcePropertyTree.Last();
                    var sourcePropertyType = source.GetType().GetNestedProperty(bindableAttribute.Source, source);//  property.GetType().GetProperty(bindableAttribute.Target).PropertyType;

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


                    var generic = myMethod.MakeGenericMethod(sourcePropertyType, targetPropertyType);
                    
                    var binding = generic.Invoke(targetProperty, new[] { sourceProperty, sourcePropertyName, targetProperty, targetPropertyName, bindableAttribute.BindingMode, null, null });

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
                    bindings.Add((Binding)binding);
                }
            }
            return bindings;
        }

        internal static IEnumerable<Binding> BindImages(object target, IControllerBase source)
        {
            var bindings = new List<Binding>();
            var bindableProperties = target.GetType().GetProperties().Where(p => p.CustomAttributes.Any(a => a.AttributeType.Name == "BindImageAttribute"));
            foreach (var bindableProperty in bindableProperties)
            {
                var bindableAttribute = bindableProperty.GetCustomAttribute(typeof(BindImageAttribute)) as BindImageAttribute;
                if (bindableAttribute == null)
                {
                    continue;
                }

                var binding = source.SetBinding<IControllerBase, ImageView>(bindableAttribute.Source, bindableProperty.GetValue(target)).WhenSourceChanges(() =>

                {
                    var b = (byte[])source.GetType().GetProperty(bindableAttribute.Source).GetValue(source);
                    var iv = ((ImageView)bindableProperty.GetValue(target));
                    if (b != null && b.Any())
                    {
                        iv.SetImageBitmap(BitmapFactory.DecodeByteArray(b, 0, b.Length));
                        iv.Invalidate();
                    }
                });
                bindings.Add(binding);
            }
            return bindings;
        }

        internal static void BindCommands(object target, IControllerBase source)
        {
            var bindableProperties = target.GetType().GetProperties().Where(p => p.CustomAttributes.Any(a => a.AttributeType.Name == "BindCommandAttribute"));
            foreach (var bindableProperty in bindableProperties)
            {
                var bindableAttribute = bindableProperty.GetCustomAttribute(typeof(BindCommandAttribute)) as BindCommandAttribute;
                if (bindableAttribute != null)
                {
                    bindableProperty.GetValue(target).SetCommand(bindableAttribute.Target, (ICommand)source.GetType().GetProperty(bindableAttribute.Source).GetValue(source));
                }
            }
        }
    }
}