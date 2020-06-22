using System;
using System.Reflection;

namespace NIntercept.Helpers
{
    public static class MethodInfoHelper
    {
        public static MethodAttributes GetMethodAttributes(MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            var attributes = MethodAttributes.ReuseSlot;

            if (method.IsVirtual)
                attributes |= MethodAttributes.Virtual;

            if (method.IsFinal || method.DeclaringType.IsInterface)
                attributes |= MethodAttributes.NewSlot;

            if (method.IsPublic)
                attributes |= MethodAttributes.Public;

            if (method.IsHideBySig)
                attributes |= MethodAttributes.HideBySig;

            // internal ?
            if (IsInternal(method))
                attributes |= MethodAttributes.Assembly;

            if (method.IsFamilyAndAssembly)
                attributes |= MethodAttributes.FamANDAssem;
            else if (method.IsFamilyOrAssembly)
                attributes |= MethodAttributes.FamORAssem;
            else if (method.IsFamily)
                attributes |= MethodAttributes.Family;

            return attributes;
        }

        public static bool IsInternal(MethodInfo method)
        {
            return method.IsAssembly || (method.IsFamilyAndAssembly && !method.IsFamilyOrAssembly);
        }
    }
}
