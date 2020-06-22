using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace NIntercept
{
    public static class IlGeneratorExtensions
    {
        private static Dictionary<Type, OpCode> OpCodesDictionary = new Dictionary<Type, OpCode>
        {
            { typeof(bool), OpCodes.Stind_I1 },
            { typeof(sbyte), OpCodes.Stind_I1 },
            { typeof(byte), OpCodes.Stind_I1 },
            { typeof(char), OpCodes.Stind_I2 },
            { typeof(short), OpCodes.Stind_I2 },
            { typeof(ushort), OpCodes.Stind_I2 },
            { typeof(int), OpCodes.Stind_I4 },
            { typeof(uint), OpCodes.Stind_I4 },
            { typeof(long), OpCodes.Stind_I8 },
            { typeof(ulong), OpCodes.Stind_I8 },
            { typeof(float), OpCodes.Stind_R4 },
            { typeof(double), OpCodes.Stind_R8 }
        };

        public static void EmitLdarg(this ILGenerator il, int i)
        {
            switch (i)
            {
                case 0:
                    il.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    il.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    il.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    il.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    il.Emit(OpCodes.Ldarg_S, i);
                    break;
            }
        }

        public static void EmitLdc_I4(this ILGenerator il, int i)
        {
            switch (i)
            {
                case 0:
                    il.Emit(OpCodes.Ldc_I4_0);
                    break;
                case 1:
                    il.Emit(OpCodes.Ldc_I4_1);
                    break;
                case 2:
                    il.Emit(OpCodes.Ldc_I4_2);
                    break;
                case 3:
                    il.Emit(OpCodes.Ldc_I4_3);
                    break;
                case 4:
                    il.Emit(OpCodes.Ldc_I4_4);
                    break;
                case 5:
                    il.Emit(OpCodes.Ldc_I4_5);
                    break;
                case 6:
                    il.Emit(OpCodes.Ldc_I4_6);
                    break;
                case 7:
                    il.Emit(OpCodes.Ldc_I4_7);
                    break;
                case 8:
                    il.Emit(OpCodes.Ldc_I4_8);
                    break;
                default:
                    il.Emit(OpCodes.Ldc_I4_S, i);
                    break;
            }
        }

        public static void EmitUnboxOrCast(this ILGenerator il, Type targetType)
        {
            if (targetType.IsValueType || targetType.IsGenericParameter)
                il.Emit(OpCodes.Unbox_Any, targetType);
            else
                il.Emit(OpCodes.Castclass, targetType);
        }

        public static void EmitStindOrStobj(this ILGenerator il, Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (type.IsEnum)
            {
                EmitStindOrStobj(il, GetUnderlyingTypeOfEnum(type));
                return;
            }

            if (type.IsByRef)
                throw new NotSupportedException("ByRef values not supported");
            else if (type.IsPrimitive && type != typeof(IntPtr) && type != typeof(UIntPtr))
                il.Emit(OpCodesDictionary[type]);
            else if (type.IsValueType)
                il.Emit(OpCodes.Stobj, type);
            else if (type.IsGenericParameter)
                il.Emit(OpCodes.Stobj, type);
            else
                il.Emit(OpCodes.Stind_Ref);
        }

        private static Type GetUnderlyingTypeOfEnum(Type enumType)
        {
            var baseType = (IConvertible)Activator.CreateInstance(enumType);
            var code = baseType.GetTypeCode();
            switch (code)
            {
                case TypeCode.SByte:
                    return typeof(sbyte);
                case TypeCode.Byte:
                    return typeof(byte);
                case TypeCode.Int16:
                    return typeof(short);
                case TypeCode.Int32:
                    return typeof(int);
                case TypeCode.Int64:
                    return typeof(long);
                case TypeCode.UInt16:
                    return typeof(ushort);
                case TypeCode.UInt32:
                    return typeof(uint);
                case TypeCode.UInt64:
                    return typeof(ulong);
                default:
                    throw new NotSupportedException();
            }
        }
    }

}
