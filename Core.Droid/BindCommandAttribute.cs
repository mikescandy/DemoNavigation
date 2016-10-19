using System;

namespace Core.Droid
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BindCommandAttribute : Attribute
    {
        public string Target { get; set; }
        public string Source { get; set; }
    }
}