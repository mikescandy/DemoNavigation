using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;

namespace Core
{
    public class Container
    {
        private IContainer IoCContainer;
        private ILifetimeScope Scope;

        public EventHandler NotificationMessagesRefresh { get; set; }

        // ReSharper disable once InconsistentNaming
        private static readonly Container instance = new Container();



        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Container()
        {
        }

        private Container()
        {
        }

        // ReSharper disable once ConvertToAutoProperty
        public static Container Instance => instance;

        public T Resolve<T>()
        {
            return Scope.Resolve<T>();
        }

        public Type Resolve(Type type)
        {
            return Scope.Resolve(type).GetType();
        }

        public void SetContainer(ContainerBuilder builder)
        {
            if (IoCContainer == null)
            {
                IoCContainer = builder.Build();
            }
            else
            {
                builder.Update(IoCContainer);
            }
            Scope = IoCContainer.BeginLifetimeScope();
        }

        public T Resolve<T>(params Parameter[] namedParameter)
        {
            return Scope.Resolve<T>(namedParameter);
        }

        public void Load(List<Assembly> assemblies, string name)
        {
            var selectedAsm = assemblies.FirstOrDefault(m => m.FullName.Contains(name));
            var toReplace = selectedAsm.DefinedTypes.Where(p => p.CustomAttributes.Any(a => a.AttributeType.Name == "MockModule") && p.IsClass);

            Scope = IoCContainer.BeginLifetimeScope(innerBuilder =>
            {
                foreach (var typeInfo in toReplace)
                {
                    innerBuilder.RegisterType(typeInfo.AsType()).InstancePerLifetimeScope().AsImplementedInterfaces();
                }
            });
        }
    }
}