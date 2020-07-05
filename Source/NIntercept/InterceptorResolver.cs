using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NIntercept
{
    public class InterceptorResolver
    {
        private static readonly Dictionary<MemberInfo, IInterceptor[]> interceptorsPerMemberCache;
        private static object locker = new object();

        static InterceptorResolver()
        {
            interceptorsPerMemberCache = new Dictionary<MemberInfo, IInterceptor[]>();
        }

        private static IInterceptor[] GetInterceptorsForMember(MemberInfo member, Type interceptorProviderType)
        {
            if (!typeof(IInterceptorProvider).IsAssignableFrom(interceptorProviderType))
                throw new InvalidOperationException($"Invalid interceptor provider type for method '{member.Name}'.");

            var interceptors = new List<IInterceptor>();

            var additionalInterceptorsOnType = ReflectionCache.Default.GetCustomAttributes(member.DeclaringType)
                .Where(p => interceptorProviderType.IsAssignableFrom(p.GetType()));
            CreateAndAddInterceptors(interceptors, additionalInterceptorsOnType);

            var additionalInterceptorsOnMethod = ReflectionCache.Default.GetCustomAttributes(member)
                .Where(p => interceptorProviderType.IsAssignableFrom(p.GetType()));
            CreateAndAddInterceptors(interceptors, additionalInterceptorsOnMethod);

            return interceptors.ToArray();
        }

        private static void CreateAndAddInterceptors(List<IInterceptor> interceptors, IEnumerable<Attribute> additionalInterceptors)
        {
            if (additionalInterceptors.Count() > 0)
            {
                foreach (IInterceptorProvider additionalInterceptor in additionalInterceptors)
                {
                    IInterceptor interceptor = CreateInterceptor(additionalInterceptor);
                    interceptors.Add(interceptor);
                }
            }
        }

        private static IInterceptor CreateInterceptor(IInterceptorProvider additionalInterceptor)
        {
            return ObjectFactory.CreateInstance(additionalInterceptor.InterceptorType) as IInterceptor;
        }

        public static IInterceptor[] GetAllInterceptors(MemberInfo member, IInterceptor[] interceptors, Type interceptorProviderType)
        {
            lock (locker)
            {
                IInterceptor[] interceptorsForMember = null;
                if (!interceptorsPerMemberCache.TryGetValue(member, out interceptorsForMember))
                {
                    interceptorsForMember = GetInterceptorsForMember(member, interceptorProviderType);
                    interceptorsPerMemberCache.Add(member, interceptorsForMember);
                }

                if (interceptors.Length == 0)
                    return interceptorsForMember;
                return interceptors.Concat(interceptorsForMember).ToArray();
            }
        }

        public static void ClearCache()
        {
            interceptorsPerMemberCache.Clear();
        }
    }
}
