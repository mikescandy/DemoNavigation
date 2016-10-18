using System;
using GalaSoft.MvvmLight.Helpers;

namespace DemoNavigation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BindAttribute : Attribute
    {
        public string Target { get; set; }
        public string Source { get; set; }
        public BindingMode BindingMode { get; set; }    
    }
}