using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Xml.Linq;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using BindingEngine.Droid.Converters;
using GalaSoft.MvvmLight.Helpers;
using Java.Lang;
using Knuj;

namespace Core.Droid
{
    internal static class BindingUtils
    {
        private static readonly XName BindingOperationXmlNamespace = XNamespace.Get("http://schemas.android.com/apk/res-auto") + "Binding";
        private const string ViewLayoutResourceIdPropertyName = "ResourceId";

        public static IEnumerable<Binding> BindXml<TViewModel>(ActivityBase<TViewModel> bindingActivity, IControllerBase source) where TViewModel : IControllerBase
        {
            var bindings = new List<Binding>();

            List<View> viewElements = null;
            List<XElement> xmlElements = null;

            // Find the value of the ViewLayoutResourceId property
            var viewLayoutResourceIdProperty = bindingActivity.GetType().GetProperty(ViewLayoutResourceIdPropertyName);
            var viewLayoutResourceId = (int)viewLayoutResourceIdProperty.GetValue(bindingActivity);

            if (viewLayoutResourceId > -1)
            {
                // Load the XML elements of the view
                xmlElements = GetViewAsXmlElements(bindingActivity, viewLayoutResourceId);
            }

            // If there is at least one 'Binding' attribute set in the XML file, get the view as objects
            if (xmlElements != null && xmlElements.Any(xe => xe.Attribute(BindingOperationXmlNamespace) != null))
            {
                viewElements = GetViewAsObjects(bindingActivity);
            }

            if (xmlElements != null && xmlElements.Any() && viewElements != null && viewElements.Any())
            {
                // Get all the binding operations inside the XML file.
                var bindingOperations = ExtractBindingOperationsFromLayoutFile(xmlElements, viewElements, source);
                if (bindingOperations != null && bindingOperations.Any())
                {
                    foreach (var bindingOperation in bindingOperations)
                    {
                        bindings.Add(BindInternal(bindingOperation, null));
                    }
                }
            }
            return bindings;
        }

        public static IEnumerable<Binding> BindXml<TViewModel>(FragmentBase<TViewModel> bindingFragment, IControllerBase source) where TViewModel : IControllerBase
        {
            var bindings = new List<Binding>();

            List<View> viewElements = null;
            List<XElement> xmlElements = null;

            // Find the value of the ViewLayoutResourceId property
            var viewLayoutResourceIdProperty = bindingFragment.GetType().GetProperty(ViewLayoutResourceIdPropertyName);
            var viewLayoutResourceId = (int)viewLayoutResourceIdProperty.GetValue(bindingFragment);

            if (viewLayoutResourceId > -1)
            {
                // Load the XML elements of the view
                xmlElements = GetViewAsXmlElements(bindingFragment, viewLayoutResourceId);
            }

            // If there is at least one 'Binding' attribute set in the XML file, get the view as objects
            if (xmlElements != null && xmlElements.Any(xe => xe.Attribute(BindingOperationXmlNamespace) != null))
            {
                viewElements = GetViewAsObjects(bindingFragment);
            }

            if (xmlElements != null && xmlElements.Any() && viewElements != null && viewElements.Any())
            {
                // Get all the binding operations inside the XML file.
                var bindingOperations = ExtractBindingOperationsFromLayoutFile(xmlElements, viewElements, source);
                if (bindingOperations != null && bindingOperations.Any())
                {
                    foreach (var bindingOperation in bindingOperations)
                    {
                        bindings.Add(BindInternal(bindingOperation, null));
                    }
                }
            }
            return bindings;
        }

        /// <summary>
        /// Returns the current view (activity) as a list of XML element.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the ViewModel associated to the activity.</typeparam>
        /// <param name="bindingActivity">The current activity we want to get as a list of XML elements.</param>
        /// <param name="viewLayoutResourceId">The id corresponding to the layout.</param>
        /// <returns>A list of XML elements which represent the XML layout of the view.</returns>
        private static List<XElement> GetViewAsXmlElements<TViewModel>(ActivityBase<TViewModel> bindingActivity, int viewLayoutResourceId) where TViewModel : IControllerBase
        {
            List<XElement> xmlElements;

            using (var viewAsXmlReader = bindingActivity.Resources.GetLayout(viewLayoutResourceId))
            {
                using (var sb = new StringBuilder())
                {
                    while (viewAsXmlReader.Read())
                    {
                        sb.Append(viewAsXmlReader.ReadOuterXml());
                    }

                    var viewAsXDocument = XDocument.Parse(sb.ToString());
                    xmlElements = viewAsXDocument.Descendants().ToList();
                }
            }

            return xmlElements;
        }

        private static List<XElement> GetViewAsXmlElements<TViewModel>(FragmentBase<TViewModel> bindingActivity, int viewLayoutResourceId) where TViewModel : IControllerBase
        {
            List<XElement> xmlElements;

            using (var viewAsXmlReader = bindingActivity.Resources.GetLayout(viewLayoutResourceId))
            {
                using (var sb = new StringBuilder())
                {
                    while (viewAsXmlReader.Read())
                    {
                        sb.Append(viewAsXmlReader.ReadOuterXml());
                    }

                    var viewAsXDocument = XDocument.Parse(sb.ToString());
                    xmlElements = viewAsXDocument.Descendants().ToList();
                }
            }

            return xmlElements;
        }

        /// <summary>
        /// Returns the current view (activity) as a list of .NET objects.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the ViewModel associated to the activity.</typeparam>
        /// <param name="bindingActivity">The current activity we want to get as a list of XML elements.</param>
        /// <returns>A list of .NET objects which composed the view.</returns>
        private static List<View> GetViewAsObjects<TViewModel>(ActivityBase<TViewModel> bindingActivity) where TViewModel : IControllerBase
        {
            // Get the objects on the view
            var rootView = bindingActivity.Window.DecorView.FindViewById(Android.Resource.Id.Content);

            return GetAllChildrenInView(rootView, true);
        }

        private static List<View> GetViewAsObjects<TViewModel>(FragmentBase<TViewModel> bindingActivity) where TViewModel : IControllerBase
        {
            // Get the objects on the view
            var rootView = bindingActivity.Activity.Window.DecorView.FindViewById(Android.Resource.Id.Content);

            return GetAllChildrenInView(rootView, true);
        }

        /// <summary>
        /// Recursive method which returns the list of children contains in a view.
        /// </summary>
        /// <param name="rootView">The root/start view from which the analysis is performed.</param>
        /// <param name="isTopRootView">True is the current root element is, in fact, the top view.</param>
        /// <returns>A list containing all the views with their childrens.</returns>
        private static List<View> GetAllChildrenInView(View rootView, bool isTopRootView = false)
        {
            if (!(rootView is ViewGroup))
            {
                return new List<View> { rootView };
            }

            var childrens = new List<View>();

            var viewGroup = (ViewGroup)rootView;

            for (int i = 0; i < viewGroup.ChildCount; i++)
            {
                var child = viewGroup.GetChildAt(i);

                var childList = new List<View>();
                if (isTopRootView)
                {
                    childList.Add(child);
                }

                childList.AddRange(GetAllChildrenInView(child));

                childrens.AddRange(childList);
            }

            return childrens;
        }

        /// <summary>
        /// Extract the Binding operations (represent by the Binding="" attribute in the XML file).
        /// </summary>
        /// <param name="xmlElements">The list of XML elements from which we want to extract the Binding operations.</param>
        /// <param name="viewElements">The list of .NET objects corresponding to the elements of the view.</param>
        /// <param name="source">The source.</param>
        /// <returns>
        /// A list containing all the binding operations (matching between the Source property, the Target property, the Control bound to the .NET property and the Mode of the binding).
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        private static List<BindingOperation> ExtractBindingOperationsFromLayoutFile(IReadOnlyCollection<XElement> xmlElements, IReadOnlyCollection<View> viewElements, IControllerBase source)
        {
            var bindingOperations = new List<BindingOperation>();

            for (int i = 0; i < xmlElements.Count; i++)
            {
                var currentXmlElement = xmlElements.ElementAt(i);

                if (currentXmlElement.Attributes(BindingOperationXmlNamespace).Any())
                {
                    var xmlBindings = currentXmlElement.Attributes(BindingOperationXmlNamespace);

                    foreach (var xmlBindingAttribute in xmlBindings)
                    {

                        var xmlBindingValue = xmlBindingAttribute.Value;

                        if (!xmlBindingValue.StartsWith("{") || !xmlBindingValue.EndsWith("}"))
                        {
                            throw new InvalidOperationException(
                                $"The following XML binding operation is not well formatted, it should start with '{{' and end with '}}:'{Environment.NewLine}{xmlBindingValue}");
                        }

                        var xmlBindingOperations = xmlBindingValue.Split(';');

                        foreach (var bindingOperation in xmlBindingOperations)
                        {
                            if (!bindingOperation.Contains(","))
                            {
                                //throw new InvalidOperationException(string.Format("The following XML binding operation is not well formatted, it should contains at least one ',' between Source and Target:{0}{1}", Environment.NewLine, xmlBindingValue));
                            }

                            // Source properties can be nested properties: MyObject.MyProperty.SampleProperty
                            var bindingSourceValueRegex = new Regex(@"Source=(\w+(.\w+)+)");
                            var bindingSourceValue = bindingSourceValueRegex.Match(bindingOperation).Groups[1].Value;

                            var bindingTargetValueRegex = new Regex(@"Target=(\w+)");
                            var bindingTargetValue = bindingTargetValueRegex.Match(bindingOperation).Groups[1].Value;

                            var bindingConverterValueRegex = new Regex(@"Converter=(\w+)");
                            var bindingConverterValue = bindingConverterValueRegex.Match(bindingOperation).Groups[1].Value;

                            // Converter parameter support using more than just a word.
                            var bindingConverterParameterValueRegex = new Regex(@"ConverterParameter='(\w+\s(.\w+)+)");
                            var bindingConverterParameterValue = bindingConverterParameterValueRegex.Match(bindingOperation).Groups[1].Value;

                            var bindingModeValue = BindingMode.OneWay;

                            var bindingModeValueRegex = new Regex(@"Mode=(\w+)");
                            var bindingModeValueRegexMatch = bindingModeValueRegex.Match(bindingOperation);

                            if (bindingModeValueRegexMatch.Success)
                            {
                                if (!System.Enum.TryParse(bindingModeValueRegexMatch.Groups[1].Value, true, out bindingModeValue))
                                {
                                    throw new InvalidOperationException(
                                        $"The Mode property of the following XML binding operation is not well formatted, it should be 'OneWay' or 'TwoWay':{Environment.NewLine}{xmlBindingValue}");
                                }
                            }

                            bindingOperations.Add(new BindingOperation { Target = viewElements.ElementAt(i), Source = source, SourceProperty = bindingSourceValue, TargetProperty = bindingTargetValue, Converter = bindingConverterValue, ConverterParameter = bindingConverterParameterValue, Mode = bindingModeValue });
                        }

                    }
                }
            }

            return bindingOperations;
        }

        internal static IEnumerable<Binding> Bind(object target, IControllerBase source)
        {
            var bindings = new List<Binding>();
            var bindableProperties = target.GetType().GetProperties().Where(p => p.CustomAttributes.Any(a => a.AttributeType.Name == "BindAttribute"));
            foreach (var bindableProperty in bindableProperties)
            {
                var bindableAttributes = bindableProperty.GetCustomAttributes(typeof(BindAttribute)).Select(m => m as BindAttribute);
                var property = target.GetNestedPropertyValue(bindableProperty.Name);

                foreach (var bindableAttribute in bindableAttributes)
                {
                    var bindingOperation = new BindingOperation
                    {
                        Source = source,
                        Target = property as View,
                        TargetProperty = bindableAttribute.Target,
                        SourceProperty = bindableAttribute.Source,
                        Mode = bindableAttribute.BindingMode
                    };

                    bindings.Add(BindInternal(bindingOperation, bindableAttribute));
                }
            }
            return bindings;
        }

        internal static Binding BindInternal(BindingOperation bindingOperation, BindAttribute bindableAttribute)
        {
            var source = bindingOperation.Source;
            var property = bindingOperation.Target;
            var targetPropertyAttribute = bindingOperation.TargetProperty;
            var sourcePropertyAttribute = bindingOperation.SourceProperty;
            var bindingMode = bindingOperation.Mode;


            var sourcePropertyTree = sourcePropertyAttribute.Split('.');
            var sourceProperty = sourcePropertyTree.Length == 1
                ? source
                : source.GetNestedPropertyValue(string.Join(".", sourcePropertyTree.SkipLastN(1)));
            var sourcePropertyName = sourcePropertyTree.Last();
            var sourcePropertyType = source.GetType().GetNestedProperty(sourcePropertyAttribute);

            if (typeof(ICommand).IsAssignableFrom(sourcePropertyType))
            {
                property.SetCommand(targetPropertyAttribute, (ICommand)source.GetType().GetProperty(sourcePropertyAttribute).GetValue(source));
                return null;
            }
            else
            {
                var targetPropertyTree = targetPropertyAttribute.Split('.');
                var targetProperty = targetPropertyTree.Length == 1
                    ? property
                    : property.GetNestedPropertyValue(string.Join(".", targetPropertyTree.SkipLastN(1)));

                if (sourcePropertyType == typeof(byte[]))
                {
                    var binding = source.SetBinding<IControllerBase, ImageView>(sourcePropertyName, targetProperty).WhenSourceChanges(() =>

                    {
                        var b = (byte[])source.GetType().GetProperty(sourcePropertyName).GetValue(source);
                        var iv = ((ImageView)targetProperty);
                        if (b != null && b.Any())
                        {
                            iv.SetImageBitmap(BitmapFactory.DecodeByteArray(b, 0, b.Length));
                            iv.Invalidate();
                        }
                    });
                    return binding;
                }
                else
                {
                    var targetPropertyName = targetPropertyTree.Last();
                    var targetPropertyType = targetProperty.GetType().GetProperty(targetPropertyName).PropertyType;

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

                    var binding = generic.Invoke(targetProperty, new[] { sourceProperty, sourcePropertyName, targetProperty, targetPropertyName, bindingMode, null, null });

                    if (!string.IsNullOrWhiteSpace(bindingOperation.Converter))
                    {
                        var converterType = ReflectionUtils.TypesImplementingInterface(typeof(IBindingValueConverter<,>)).FirstOrDefault(m => m.Name == bindingOperation.Converter);

                        if (converterType != null)
                        {
                            var converter = Activator.CreateInstance(converterType);
                            var convertSourceToTargetMethod = binding.GetType().GetMethod("ConvertSourceToTarget");
                            var convertTargetToSourceMethod = binding.GetType().GetMethod("ConvertTargetToSource");
                            try
                            {
                                var sourceToTargetFunc = converterType.GetProperty("Convert").GetValue(converter);
                                convertSourceToTargetMethod.Invoke(binding, new[] { sourceToTargetFunc });
                            }
                            catch (System.Exception)
                            {
                                // ignoring
                            }
                            try
                            {
                                var targetToSourceFunc = converterType.GetProperty("ConvertBack").GetValue(converter);
                                convertTargetToSourceMethod.Invoke(binding, new[] { targetToSourceFunc });
                            }
                            catch (System.Exception)
                            {
                                // ignoring
                            }

                        }
                    }
                    return (Binding)binding;
                }
            }
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