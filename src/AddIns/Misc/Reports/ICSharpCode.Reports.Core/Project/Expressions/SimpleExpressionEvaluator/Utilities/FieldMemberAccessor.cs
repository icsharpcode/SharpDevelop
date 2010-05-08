using System;
using System.Reflection;

namespace SimpleExpressionEvaluator.Utilities
{
    public class FieldMemberAccessor : IMemberAccessor
    {
        private readonly FieldInfo _field;

        public FieldMemberAccessor(FieldInfo field)
        {
            _field = field;
        }

        public object GetValue(object target)
        {
            return _field.GetValue(target);
        }

        public bool IsStatic
        {
            get { return _field.IsStatic; }
        }

        public Type MemberType
        {
            get { return _field.FieldType;}
        }
    }
}
