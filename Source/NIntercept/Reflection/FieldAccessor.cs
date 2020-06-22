using System;
using System.Reflection;

namespace NIntercept
{
    public class FieldAccessor : IMemberAccessor
    {
        private TypeAccessor typeAccessor;
        private FieldInfo field;

        public FieldAccessor(TypeAccessor typeAccessor, FieldInfo field)
        {
            if (typeAccessor is null)
                throw new ArgumentNullException(nameof(typeAccessor));
            if (field is null)
                throw new ArgumentNullException(nameof(field));

            this.typeAccessor = typeAccessor;
            this.field = field;
        }

        public TypeAccessor TypeAccessor
        {
            get { return typeAccessor; }
        }

        public FieldInfo Field
        {
            get { return field; }
        }

        public string Name
        {
            get { return field.Name; }
        }

        public virtual object GetValue()
        {
            if (field.IsStatic)
                return field.GetValue(null);
            else
            {
                var target = typeAccessor.Target;
                if (target == null)
                    throw new InvalidOperationException("Target cannot be null");
                return field.GetValue(target);
            }
        }

        public TReturnValue GetValue<TReturnValue>()
        {
            return (TReturnValue)GetValue();
        }

        public virtual void SetValue(object value)
        {
            if (field.IsStatic)
                field.SetValue(null, value);
            else
            {
                var target = typeAccessor.Target;
                if (target == null)
                    throw new InvalidOperationException("Target cannot be null");
                field.SetValue(target, value);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
