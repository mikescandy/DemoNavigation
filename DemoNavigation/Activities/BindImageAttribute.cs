using System;

namespace DemoNavigation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BindImageAttribute : Attribute
    {
        public string Source { get; set; }
    }
}