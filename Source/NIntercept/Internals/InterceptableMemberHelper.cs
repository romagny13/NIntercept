using NIntercept.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NIntercept
{
    internal static class InterceptableMemberHelper
    {
        public static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return ReflectionCache.Default.GetProperties(type);
        }

        public static IEnumerable<MethodInfo> GetMethods(Type type)
        {
            return ReflectionCache.Default.GetMethods(type);
        }

        public static IEnumerable<EventInfo> GetEvents(Type type)
        {
            return ReflectionCache.Default.GetEvents(type);
        }

        public static List<PropertyInfo> GetInterceptableProperties(Type type, Type[] interfaces)
        {
            List<PropertyInfo> allProperties = new List<PropertyInfo>(GetProperties(type));
            foreach (var @interface in interfaces)
            {
                var properties = GetProperties(@interface);
                foreach (var property in properties)
                {
                    if (allProperties.FirstOrDefault(p => p.Name == property.Name) == null)
                        allProperties.Add(property);
                }
            }

            return allProperties;
        }

        public static List<MethodInfo> GetInterceptableMethods(Type type, Type[] interfaces)
        {
            // do not get set_..., get_..., add_..., remove_... methods
            List<MethodInfo> allMethods = new List<MethodInfo>(GetMethods(type));

            foreach (var @interface in interfaces)
            {
                var methods = GetMethods(@interface);
                foreach (var method in methods)
                {
                    if (MethodFinder.FindMethod(allMethods, method) == null)
                        allMethods.Add(method);
                }
            }

            return allMethods;
        }

        public static List<EventInfo> GetInterceptableEvents(Type type, Type[] interfaces)
        {
            List<EventInfo> allEvents = new List<EventInfo>(GetEvents(type));

            foreach (var @interface in interfaces)
            {
                var events = GetEvents(@interface);
                foreach (var @event in events)
                {
                    if (allEvents.FirstOrDefault(p => p.Name == @event.Name) == null)
                        allEvents.Add(@event);
                }
            }
            return allEvents;
        }
    }

}
