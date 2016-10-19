using Autofac;

namespace Core
{
    public class Application //: ApplicationBase
    {
        public IContainer Container;

        static Application()
        {
        }

        private Application()
        {
        }
        private static readonly Application instance = new Application();

        // ReSharper disable once ConvertToAutoProperty
        public static Application Instance => instance;

    }
}
