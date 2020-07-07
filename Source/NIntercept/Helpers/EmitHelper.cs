using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept.Helpers
{
    public static class EmitHelper
    {
        public static void StoreFieldToLocal(ILGenerator il, FieldBuilder field, LocalBuilder localBuilder)
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            il.Emit(OpCodes.Stloc, localBuilder);
        }

        public static void StoreEventToField(ILGenerator il, EventInfo @event, FieldBuilder memberField)
        {
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldtoken, @event.DeclaringType);
            il.Emit(OpCodes.Call, Methods.GetTypeFromHandle);
            il.Emit(OpCodes.Ldstr, @event.Name);

            if (@event.AddMethod.IsPublic)
            {
                il.Emit(OpCodes.Callvirt, Methods.GetEvent);
            }
            else
            {
                il.Emit(OpCodes.Ldc_I4_S, 52);
                il.Emit(OpCodes.Callvirt, Methods.GetEventWithBindingFlags);
            }
            il.Emit(OpCodes.Stsfld, memberField);
        }

        public static void StorePropertyToField(ILGenerator il, PropertyInfo property, FieldBuilder field)
        {
            bool isPublic = property.CanRead ? property.GetMethod.IsPublic : property.SetMethod.IsPublic;

            il.Emit(OpCodes.Ldtoken, property.DeclaringType);
            il.Emit(OpCodes.Call, Methods.GetTypeFromHandle);
            il.Emit(OpCodes.Ldstr, property.Name);

            if (isPublic)
            {
                il.Emit(OpCodes.Call, Methods.GetProperty);
            }
            else
            {
                il.Emit(OpCodes.Ldc_I4_S, 52);
                il.Emit(OpCodes.Call, Methods.GetPropertyWithBindingFlags);
            }
            il.Emit(OpCodes.Stsfld, field);
        }

        public static void StoreMethodToField(ILGenerator il, MethodInfo method, FieldBuilder field)
        {
            il.Emit(OpCodes.Ldtoken, method);
            il.Emit(OpCodes.Ldtoken, method.DeclaringType);
            il.Emit(OpCodes.Call, Methods.GetMethodFromHandle);
            //il.Emit(OpCodes.Castclass, typeof(MemberInfo));
            il.Emit(OpCodes.Stsfld, field);
        }

        public static void StoreThisToLocal(ILGenerator il, LocalBuilder localBuilder)
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Stloc, localBuilder);
        }

        public static void CreateInvocation(ILGenerator il, Type invocationType, LocalBuilder targetLocalBuilder, LocalBuilder interceptorsLocalBuilder,
           LocalBuilder methodLocalBuilder, LocalBuilder proxyMethodLocalBuilder, LocalBuilder proxyLocalBuilder, LocalBuilder parametersLocalBuilder,
           LocalBuilder invocationLocalBuilder)
        {
            il.Emit(OpCodes.Ldloc, targetLocalBuilder);
            il.Emit(OpCodes.Ldloc, interceptorsLocalBuilder);
            il.Emit(OpCodes.Ldloc, methodLocalBuilder);
            il.Emit(OpCodes.Ldloc, proxyMethodLocalBuilder);
            il.Emit(OpCodes.Ldloc, proxyLocalBuilder);
            il.Emit(OpCodes.Ldloc, parametersLocalBuilder);

            var invocationCtor = invocationType.GetConstructors()[0];
            il.Emit(OpCodes.Newobj, invocationCtor);
            il.Emit(OpCodes.Stloc, invocationLocalBuilder);
        }

        public static void CallProceed(ILGenerator il, LocalBuilder invocationLocalBuilder)
        {
            il.Emit(OpCodes.Ldloc, invocationLocalBuilder);
            il.Emit(OpCodes.Callvirt, InvocationMethods.ProceedMethod);
            il.Emit(OpCodes.Nop);
        }

        public static void EmitReturnValue(ILGenerator il, MethodInfo method, LocalBuilder invocationLocalBuilder, LocalBuilder returnValueLocalBuilder)
        {
            if (method.ReturnType != typeof(void))
            {
                Label end = il.DefineLabel();

                // return (string)invocation.ReturnValue;
                il.Emit(OpCodes.Ldloc, invocationLocalBuilder);
                il.Emit(OpCodes.Callvirt, InvocationMethods.get_ReturnValueMethod);

                il.EmitUnboxOrCast(method.ReturnType);

                il.Emit(OpCodes.Stloc, returnValueLocalBuilder);

                il.Emit(OpCodes.Br_S, end);
                il.MarkLabel(end);

                il.Emit(OpCodes.Ldloc, returnValueLocalBuilder);
            }

            il.Emit(OpCodes.Ret);
        }
    }
}
