using System;
using System.Dynamic;
using System.Reflection;

namespace DynamicReflectionDemo
{
    public class ExposedObject : DynamicObject
    {
        private readonly Type type;

        private readonly BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;

        public ExposedObject(Type type)
            => this.type = type;

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var property = this.type.GetProperty(binder.Name, flags);

            if (property == null)
            {
                return base.TrySetMember(binder, value);
            }

            property.SetValue(null, value);

            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var property = this.type.GetProperty(binder.Name, flags);

            if (property == null)
            {
                return base.TryGetMember(binder, out result);
            }

            result = property.GetValue(null);

            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var method = this.type.GetMethod(binder.Name, flags);

            if (method == null)
            {
                return base.TryInvokeMember(binder, args, out result);
            }

            result = method.Invoke(null, args);

            return true;
        }
    }
}
