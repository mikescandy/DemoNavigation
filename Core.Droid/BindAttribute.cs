using System;
using Android.OS;
using Dalvik.Annotation;
using GalaSoft.MvvmLight.Helpers;

namespace Core.Droid
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class BindAttribute : Attribute
    {
        public Type Converters { get; set; }
        public string SourceToTargetConverter { get; set; }
        public string TargetToSourceConverter { get; set; }
        public string Target { get; set; }
        public string Source { get; set; }
        public BindingMode BindingMode { get; set; }
    }

    
}