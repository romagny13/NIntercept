using System;

namespace NIntercept
{

    public class ObjectFactory
    {
        private static Func<Type, object> DefaultFactory;

        static ObjectFactory()
        {
            Reset();
        }

        public static void SetDefaultFactory(Func<Type, object> factory)
        {
            if (factory is null)
                throw new ArgumentNullException(nameof(factory));

            DefaultFactory = factory;
        }

        public static void Reset()
        {
            DefaultFactory = new Func<Type, object>(type => Activator.CreateInstance(type));
        }

        public static object CreateInstance(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            try
            {
                return DefaultFactory.Invoke(type);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Unable to create an instance of '{type.Name}. Use an IoC Container with the ObjectFactory can be the solution.'", ex);
            }
        }
    }
}
