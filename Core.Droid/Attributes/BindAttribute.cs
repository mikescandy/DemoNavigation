using System;
using BindingEngine.Droid.Converters;
using GalaSoft.MvvmLight.Helpers;

namespace Core.Droid
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class BindAttribute : Attribute
    {
        public string Converter { get; set; }
        public string Target { get; set; }
        public string Source { get; set; }
        public BindingMode BindingMode { get; set; }
    }
}