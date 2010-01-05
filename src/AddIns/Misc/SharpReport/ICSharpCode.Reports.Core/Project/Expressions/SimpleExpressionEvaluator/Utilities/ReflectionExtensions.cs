using System;
using System.Reflection;

namespace SimpleExpressionEvaluator.Utilities
{
    public static class ReflectionExtensions
    {
        public static PropertyPath ParsePropertyPath(this object target,string propertyPath)
        {
            if (target == null || String.IsNullOrEmpty(propertyPath))
                return null;

            return PropertyPath.Parse(target.GetType(), propertyPath);
        }

        public static object EvaluatePropertyPath(this object target,string propertyPath)
        {
            PropertyPath path = ParsePropertyPath(target, propertyPath);
            if (path != null)
                return path.Evaluate(target);
            return null;
        }

        public static IMemberAccessor FindAccessor(this Type type, string accessorName)
        {
            PropertyInfo prop = type.GetProperty(accessorName,
                                                 BindingFlags.IgnoreCase | BindingFlags.NonPublic |
                                                 BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if (prop != null)
                return new PropertyMemberAccessor(prop);

            FieldInfo field = type.GetField(accessorName,
                                            BindingFlags.IgnoreCase | BindingFlags.NonPublic |
                                            BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if (field != null)
                return new FieldMemberAccessor(field);

            return null;
        }
    }
}
