using System;

namespace Core.Droid
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BindImageAttribute : Attribute
    {
        public string Source { get; set; }
    }
}