using System;
using System.Reflection;

namespace SimpleExpressionEvaluator.Utilities
{
    public class PropertyMemberAccessor : IMemberAccessor
    {
        private readonly PropertyInfo _prop;

        public PropertyMemberAccessor(PropertyInfo prop)
        {
            _prop = prop;
        }

        public object GetValue(object target)
        {
            return _prop.GetValue(target, null);
        }

        public bool IsStatic
        {
            get { return _prop.GetGetMethod().IsStatic; }
        }

        public Type MemberType
        {
            get { return _prop.PropertyType;}
        }
    }
}
