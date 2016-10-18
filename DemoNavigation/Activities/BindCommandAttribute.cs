using System;

namespace DemoNavigation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BindCommandAttribute : Attribute
    {

        public string Target { get; set; }
        public string Source { get; set; }
    }
}