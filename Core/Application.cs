using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Core.Interfaces;

namespace Core
{
    public abstract class Singleton<T> where T : class, new()
    {
        public static readonly T Instance = new T();
    }
}
