using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept
{
    public static class EmitHelper
    {          
        public static void StoreFieldInLocal(ILGenerator il, FieldBuilder field, LocalBuilder localBuilder)
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            il.Emit(OpCodes.Stloc, localBuilder);
        }

        public static void StoreProxyMethodToLocal(ILGenerator il, LocalBuilder localBuilder)
        {
            il.Emit(OpCodes.Call, Methods.GetCurrentMethod);
            il.Emit(OpCodes.Castclass, typeof(MethodInfo));
            il.Emit(OpCodes.Stloc, localBuilder);
        }

        public static void StoreMemberToLocal(ILGenerator il, MemberInfo member, LocalBuilder localBuilder)
        {
            switch (member)
            {
                case MethodInfo _:
                    MethodInfo method = member as MethodInfo;
                    il.Emit(OpCodes.Ldtoken, method);
                    il.Emit(OpCodes.Ldtoken, method.DeclaringType);
                    il.Emit(OpCodes.Call, Methods.GetMethodFromHandle);
                    il.Emit(OpCodes.Castclass, typeof(MemberInfo));
                    il.Emit(OpCodes.Stloc, localBuilder);
                    break;
                case PropertyInfo _:
                    il.Emit(OpCodes.Ldtoken, member.DeclaringType);
                    il.Emit(OpCodes.Call, Methods.GetTypeFromHandle);
                    il.Emit(OpCodes.Ldstr, member.Name);
                    il.Emit(OpCodes.Ldc_I4_S, 52);
                    il.Emit(OpCodes.Call, Methods.GetProperty);
                    il.Emit(OpCodes.Stloc, localBuilder);
                    break;
                case EventInfo _:
                    il.Emit(OpCodes.Nop);
                    il.Emit(OpCodes.Ldtoken, member.DeclaringType);
                    il.Emit(OpCodes.Call, Methods.GetTypeFromHandle);
                    il.Emit(OpCodes.Ldstr, member.Name);
                    il.Emit(OpCodes.Ldc_I4_S, 52);
                    il.Emit(OpCodes.Callvirt, Methods.GetEvent);
                    il.Emit(OpCodes.Stloc, localBuilder);
                    break;
                default:
                    throw new NotSupportedException();
            }      
        }

        public static void StoreThisInLocal(ILGenerator il, LocalBuilder localBuilder)
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

        public static void CallProceed(ILGenerator il, Type invocationType, LocalBuilder invocationLocalBuilder)
        {
            il.Emit(OpCodes.Ldloc, invocationLocalBuilder);
            il.Emit(OpCodes.Callvirt, invocationType.GetMethod("Proceed"));
            il.Emit(OpCodes.Nop);
        }

        public static void EmitReturnValue(ILGenerator il, MethodInfo method, LocalBuilder invocationLocalBuilder, LocalBuilder returnValueLocalBuilder)
        {
            if (method.ReturnType != typeof(void))
            {
                Label end = il.DefineLabel();

                // return (string)invocation.ReturnValue;
                il.Emit(OpCodes.Ldloc, invocationLocalBuilder);
                il.Emit(OpCodes.Callvirt, typeof(Invocation).GetMethod("get_ReturnValue"));

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
