using System;
using System.Reflection;

namespace NIntercept.Reflection
{
    public class TypeAccessor
    {
        private Type type;
        private object target;
        private FieldAccessorCollection fields;
        private ConstructorInfo[] constructors;
        private PropertyAccessorCollection properties;
        private MethodAccessorCollection methodAccessors;
        private EventAccessorCollection events;

        public TypeAccessor(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            this.type = type;
        }

        public Type Type
        {
            get { return type; }
        }

        public object Target
        {
            get { return target; }
        }

        public TypeAccessor(object target)
        {
            if (target is null)
                throw new ArgumentNullException(nameof(target));

            this.target = target;
            this.type = target.GetType();
        }

        public FieldAccessorCollection Fields
        {
            get
            {
                if (fields == null)
                    fields = GetFields();
                return fields;
            }
        }

        public ConstructorInfo[] Constructors
        {
            get
            {
                if (constructors == null)
                    constructors = GetConstructors();
                return constructors;
            }
        }

        public PropertyAccessorCollection Properties
        {
            get
            {
                if (properties == null)
                    properties = GetProperties();
                return properties;
            }
        }

        public MethodAccessorCollection Methods
        {
            get
            {
                if (methodAccessors == null)
                    methodAccessors = GetMethods();
                return methodAccessors;
            }
        }

        public EventAccessorCollection Events
        {
            get
            {
                if (events == null)
                    events = GetEvents();
                return events;
            }
        }

        protected virtual FieldAccessorCollection GetFields()
        {
            var fields = ReflectionCache.Default.GetFields(type);
            int length = fields.Length;
            var accessors = new FieldAccessor[length];
            for (int i = 0; i < length; i++)
                accessors[i] = CreateFieldAccessor(fields[i]);
            return new FieldAccessorCollection(accessors);
        }

        protected virtual FieldAccessor CreateFieldAccessor(FieldInfo field)
        {
            return new FieldAccessor(this, field);
        }

        protected virtual ConstructorInfo[] GetConstructors()
        {
            return ReflectionCache.Default.GetConstructors(type);
        }

        protected virtual PropertyAccessorCollection GetProperties()
        {
            var properties = ReflectionCache.Default.GetProperties(type);
            int length = properties.Length;
            var accessors = new PropertyAccessor[length];
            for (int i = 0; i < length; i++)
                accessors[i] = CreatePropertyAccessor(properties[i]);
            return new PropertyAccessorCollection(accessors);
        }

        protected virtual PropertyAccessor CreatePropertyAccessor(PropertyInfo property)
        {
            return new PropertyAccessor(this, property);
        }

        protected virtual MethodAccessorCollection GetMethods()
        {
            var methods = ReflectionCache.Default.GetMethods(type);
            int length = methods.Length;
            var accessors = new MethodAccessor[length];
            for (int i = 0; i < length; i++)
                accessors[i] = CreateMethodAccessor(methods[i]);
            return new MethodAccessorCollection(accessors);
        }

        protected virtual MethodAccessor CreateMethodAccessor(MethodInfo method)
        {
            return new MethodAccessor(this, method);
        }

        protected virtual EventAccessorCollection GetEvents()
        {
            var events = ReflectionCache.Default.GetEvents(type);
            int length = events.Length;
            var accessors = new EventAccessor[length];
            for (int i = 0; i < length; i++)
                accessors[i] = CreateEventAccessor(events[i]);
            return new EventAccessorCollection(accessors);
        }

        protected virtual EventAccessor CreateEventAccessor(EventInfo @event)
        {
            return new EventAccessor(this, @event);
        }

        public override string ToString()
        {
            return type.Name;
        }
    }
}
