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
    internal static class ReflectionUtils
    {
        private static readonly XName BindingOperationXmlNamespace = XNamespace.Get("http://schemas.android.com/apk/res-auto") + "Binding";
        private const string ViewLayoutResourceIdPropertyName = "ResourceId";

        public static bool DoesTypeSupportInterface(Type type, Type inter)
        {
            if (inter.IsAssignableFrom(type))
                return true;
            if (type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == inter))
                return true;
            return false;
        }

        public static IEnumerable<Type> TypesImplementingInterface(Type desiredType)
        {
            return AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => DoesTypeSupportInterface(type, desiredType));

        }

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

                    // Find the value of the DataContext property (which is, in fact, our ViewModel)
                    //var viewModel = bindingActivity.DataContext as IControllerBase;
                    //if (viewModel != null)
                    //{
                    //    // Load all the converters if there is a binding using a converter
                    //    if (bindingOperations.Any(bo => !string.IsNullOrWhiteSpace(bo.Converter)))
                    //    {
                    //        var valueConverters = GetAllValueConverters();
                    //        ValueConverters.AddRange(valueConverters.Where(valueConverter => !ValueConverters.Contains(valueConverter)));
                    //    }

                    //    var bindingRelationships = new List<BindingRelationship>();

                    //    // OneWay bindings: all changes to any properties of the ViewModel will need to update the dedicated properties on controls
                    //    viewModel.PropertyChanged += (sender, args) =>
                    //    {
                    //        if (_preventUpdateForTargetProperty)
                    //            return;

                    //        if (bindingRelationships.Any(p => p.SourceProperty.Name == args.PropertyName))
                    //        {
                    //            foreach (var bindingRelationship in bindingRelationships.Where(p => p.SourceProperty.Name == args.PropertyName))
                    //            {
                    //                _preventUpdateForSourceProperty = true;

                    //                // Get the value of the source (ViewModel) property by using the converter if needed
                    //                var sourcePropertyValue = bindingRelationship.Converter == null ? bindingRelationship.SourceProperty.GetValue(viewModel) : bindingRelationship.Converter.Convert(bindingRelationship.SourceProperty.GetValue(viewModel), bindingRelationship.TargetProperty.PropertyType, bindingRelationship.ConverterParameter, CultureInfo.CurrentCulture);

                    //                bindingRelationship.TargetProperty.SetValue(bindingRelationship.Control, sourcePropertyValue);

                    //                _preventUpdateForSourceProperty = false;
                    //            }
                    //        }
                    //    };

                    //    // For each binding operations, bind from the source (ViewModel) to the target (Control) 
                    //    // and from the target (Control) to the source (ViewModel) in case of a TwoWay binding.
                    //    foreach (var bindingOperation in bindingOperations)
                    //    {
                    //        var sourceProperty = typeof(TViewModel).GetProperty(bindingOperation.Source);

                    //        var bindingEvent = bindingOperation.Control.GetType().GetEvent(bindingOperation.Target);
                    //        if (bindingEvent != null)
                    //        {
                    //            // The target is an event of the control

                    //            if (sourceProperty != null)
                    //            {
                    //                // We need to ensure that the bound property implements the interface ICommand so we can call the "Execute" method
                    //                var command = sourceProperty.GetValue(viewModel) as ICommand;
                    //                if (command == null)
                    //                {
                    //                    throw new InvalidOperationException(string.Format("The source property {0}, bound to the event {1}, needs to implement the interface ICommand.", bindingOperation.Source, bindingEvent.Name));
                    //                }

                    //                // Add an event handler to the specified event to execute the command when event is raised
                    //                var executeMethodInfo = typeof(ICommand).GetMethod("Execute", new[] { typeof(object) });

                    //                AddHandler(bindingOperation.Control, bindingOperation.Target, () =>
                    //                {
                    //                    if (!_preventUpdateForSourceProperty)
                    //                    {
                    //                        executeMethodInfo.Invoke(command, new object[] { null });
                    //                    }
                    //                });

                    //                // Add an event handler to manage the CanExecuteChanged event of the command (so we can disable/enable the control attached to the command)
                    //                var currentControl = bindingOperation.Control;

                    //                var enabledProperty = currentControl.GetType().GetProperty("Enabled");
                    //                if (enabledProperty != null)
                    //                {
                    //                    enabledProperty.SetValue(currentControl, command.CanExecute(null));

                    //                    AddHandler(command, "CanExecuteChanged", () => enabledProperty.SetValue(currentControl, command.CanExecute(null)));
                    //                }
                    //            }
                    //            else
                    //            {
                    //                // If the Source property of the ViewModel is not a 'real' property, check if it's a method
                    //                var sourceMethod = typeof(TViewModel).GetMethod(bindingOperation.Source, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                    //                if (sourceMethod != null)
                    //                {
                    //                    if (sourceMethod.GetParameters().Length > 0)
                    //                    {
                    //                        // We only support calls to methods without parameters
                    //                        throw new InvalidOperationException(string.Format("Method {0} should not have any parameters to be called when event {1} is raised.", sourceMethod.Name, bindingEvent.Name));
                    //                    }

                    //                    // If it's a method, add a event handler to the specified event to execute the method when event is raised
                    //                    AddHandler(bindingOperation.Control, bindingOperation.Target, () =>
                    //                    {
                    //                        if (!_preventUpdateForSourceProperty)
                    //                        {
                    //                            sourceMethod.Invoke(viewModel, null);
                    //                        }
                    //                    });
                    //                }
                    //                else
                    //                {
                    //                    throw new InvalidOperationException(string.Format("No property or event named {0} found to bint it to the event {1}.", bindingOperation.Source, bindingEvent.Name));
                    //                }
                    //            }
                    //        }
                    //        else
                    //        {
                    //            if (sourceProperty == null)
                    //            {
                    //                throw new InvalidOperationException(string.Format("Source property {0} does not exist on {1}.", bindingOperation.Source, typeof(TViewModel).Name));
                    //            }

                    //            // The target is a property of the control
                    //            var targetProperty = bindingOperation.Control.GetType().GetProperty(bindingOperation.Target);
                    //            if (targetProperty == null)
                    //            {
                    //                throw new InvalidOperationException(string.Format("Target property {0} of the XML binding operation does not exist on {1}.", bindingOperation.Target, bindingOperation.Control.GetType().Name));
                    //            }

                    //            // If there is a Converter provided, instanciate it and use it to convert the value
                    //            var valueConverterName = bindingOperation.Converter;
                    //            IBindingValueConverter valueConverter = null;

                    //            if (!string.IsNullOrWhiteSpace(valueConverterName))
                    //            {
                    //                var valueConverterType = ValueConverters.FirstOrDefault(t => t.Name == valueConverterName);
                    //                if (valueConverterType != null)
                    //                {
                    //                    var valueConverterCtor = valueConverterType.GetConstructor(Type.EmptyTypes);
                    //                    if (valueConverterCtor != null)
                    //                    {
                    //                        valueConverter = valueConverterCtor.Invoke(null) as IBindingValueConverter;
                    //                    }
                    //                    else
                    //                    {
                    //                        throw new InvalidOperationException(string.Format("Value converter {0} need an empty constructor to be instanciated.", valueConverterName));
                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    throw new InvalidOperationException(string.Format("There is no converter named {0}.", valueConverterName));
                    //                }
                    //            }

                    //            var valueConverterParameter = bindingOperation.ConverterParameter;

                    //            // Get the value of the source (ViewModel) property by using the converter if needed
                    //            var sourcePropertyValue = valueConverter == null ? sourceProperty.GetValue(viewModel) : valueConverter.Convert(sourceProperty.GetValue(viewModel), targetProperty.PropertyType, valueConverterParameter, CultureInfo.CurrentCulture);

                    //            // Set initial binding value
                    //            targetProperty.SetValue(bindingOperation.Control, sourcePropertyValue);

                    //            // Add a relationship between the source (ViewModel) and the target (Control) so we can update the target property when the source changed (OneWay binding)
                    //            bindingRelationships.Add(new BindingRelationship { SourceProperty = sourceProperty, TargetProperty = targetProperty, Converter = valueConverter, ConverterParameter = bindingOperation.ConverterParameter, Control = bindingOperation.Control });

                    //            if (bindingOperation.Mode == BindingMode.TwoWay)
                    //            {
                    //                // TwoWay bindings: Update the ViewModel property when the dedicated event is raised on the bound control
                    //                var controlType = bindingOperation.Control.GetType();

                    //                // Bind controls' events to update the associated ViewModel property

                    //                #region Bind controls' events to update the associated ViewModel property

                    //                // TODO: Need to improve this!
                    //                if (controlType == typeof(CalendarView))
                    //                {
                    //                    ((CalendarView)bindingOperation.Control).DateChange += (sender, args) => UpdateSourceProperty(sourceProperty, viewModel, new DateTime(args.Year, args.Month, args.DayOfMonth), valueConverter, valueConverterParameter);
                    //                }
                    //                else if (controlType == typeof(CheckBox))
                    //                {
                    //                    ((CheckBox)bindingOperation.Control).CheckedChange += (sender, args) => UpdateSourceProperty(sourceProperty, viewModel, args.IsChecked, valueConverter, valueConverterParameter);
                    //                }
                    //                if (controlType == typeof(EditText))
                    //                {
                    //                    ((EditText)bindingOperation.Control).TextChanged += (sender, args) => UpdateSourceProperty(sourceProperty, viewModel, args.Text.ToString(), valueConverter, valueConverterParameter);
                    //                }
                    //                else if (controlType == typeof(RadioButton))
                    //                {
                    //                    ((RadioButton)bindingOperation.Control).CheckedChange += (sender, args) => UpdateSourceProperty(sourceProperty, viewModel, args.IsChecked, valueConverter, valueConverterParameter);
                    //                }
                    //                else if (controlType == typeof(RatingBar))
                    //                {
                    //                    ((RatingBar)bindingOperation.Control).RatingBarChange += (sender, args) => UpdateSourceProperty(sourceProperty, viewModel, args.Rating, valueConverter, valueConverterParameter);
                    //                }
                    //                else if (controlType == typeof(SearchView))
                    //                {
                    //                    ((SearchView)bindingOperation.Control).QueryTextChange += (sender, args) => UpdateSourceProperty(sourceProperty, viewModel, args.NewText, valueConverter, valueConverterParameter);
                    //                }
                    //                else if (controlType == typeof(Switch))
                    //                {
                    //                    ((Switch)bindingOperation.Control).CheckedChange += (sender, args) => UpdateSourceProperty(sourceProperty, viewModel, args.IsChecked, valueConverter, valueConverterParameter);
                    //                }
                    //                else if (controlType == typeof(TimePicker))
                    //                {
                    //                    ((TimePicker)bindingOperation.Control).TimeChanged += (sender, args) => UpdateSourceProperty(sourceProperty, viewModel, new TimeSpan(args.HourOfDay, args.Minute, 0), valueConverter, valueConverterParameter);
                    //                }
                    //                else if (controlType == typeof(ToggleButton))
                    //                {
                    //                    ((ToggleButton)bindingOperation.Control).CheckedChange += (sender, args) => UpdateSourceProperty(sourceProperty, viewModel, args.IsChecked, valueConverter, valueConverterParameter);
                    //                }
                    //                else if (controlType == typeof(SeekBar))
                    //                {
                    //                    ((SeekBar)bindingOperation.Control).ProgressChanged += (sender, args) => UpdateSourceProperty(sourceProperty, viewModel, args.Progress, valueConverter, valueConverterParameter);
                    //                }

                    //                #endregion
                    //            }
                    //        }
                    //    }
                    //}
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
        /// <returns>A list containing all the binding operations (matching between the Source property, the Target property, the Control bound to the .NET property and the Mode of the binding).</returns>
        private static List<BindingOperation> ExtractBindingOperationsFromLayoutFile(List<XElement> xmlElements, List<View> viewElements, IControllerBase source)
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
                            throw new InvalidOperationException(string.Format("The following XML binding operation is not well formatted, it should start with '{{' and end with '}}:'{0}{1}", Environment.NewLine, xmlBindingValue));
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
                                    throw new InvalidOperationException(string.Format("The Mode property of the following XML binding operation is not well formatted, it should be 'OneWay' or 'TwoWay':{0}{1}", Environment.NewLine, xmlBindingValue));
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
                var property = target.GetNestedPropertyValue(bindableProperty.Name);//  target.GetType().GetProperty(target.GetType().GetProperty(bindableProperty.Name).Name).GetValue(target);

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
            var sourcePropertyType = source.GetType().GetNestedProperty(sourcePropertyAttribute, source);//  property.GetType().GetProperty(bindableAttribute.Target).PropertyType;

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
               
                if (sourcePropertyType == typeof(byte[]) /*&& targetProperty.GetType() == typeof(ImageView)*/)
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
                    var targetPropertyType = targetProperty.GetType().GetProperty(targetPropertyName).PropertyType;//  property.GetType().GetProperty(bindableAttribute.Target).PropertyType;

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
                        var converterType = TypesImplementingInterface(typeof(IBindingValueConverter<,>)).FirstOrDefault(m => m.Name == bindingOperation.Converter);

                        if (converterType != null)
                        {
                            var converter = Activator.CreateInstance(converterType);
                            var convertSourceToTargetMethod = binding.GetType().GetMethod("ConvertSourceToTarget");
                            var convertTargetToSourceMethod = binding.GetType().GetMethod("ConvertTargetToSource");
                            try
                            {
                                var s2tFunc = converterType.GetProperty("Convert").GetValue(converter);
                                convertSourceToTargetMethod.Invoke(binding, new[] { s2tFunc });
                            }
                            catch (System.NotImplementedException)
                            {
                                // ignoring
                            }
                            try
                            {
                                var t2sFunc = converterType.GetProperty("ConvertBack").GetValue(converter);
                                convertTargetToSourceMethod.Invoke(binding, new[] { t2sFunc });
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