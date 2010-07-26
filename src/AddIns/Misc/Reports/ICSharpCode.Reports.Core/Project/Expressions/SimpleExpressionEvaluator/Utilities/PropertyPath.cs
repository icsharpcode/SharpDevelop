using System;
using System.Text;

namespace SimpleExpressionEvaluator.Utilities
{
    public class PropertyPath
    {
        private readonly Type _rootType;
        private readonly IMemberAccessor[] _properties;
        private readonly bool _rootIsStatic;

        private PropertyPath(Type rootType,IMemberAccessor[] props)
        {
            _properties = props;
            _rootType = rootType;
            
            if (_properties[0].IsStatic)
                _rootIsStatic = true;
        }

        public object Evaluate(object target)
        {
            if (target == null && _rootIsStatic == false)
                return null;
            
            Type s = target.GetType();
           
            if (target != null && _rootType.IsAssignableFrom(target.GetType()) == false)
                return null;


            object current = target;
            foreach (IMemberAccessor prop in _properties)
            {
                current = prop.GetValue(current);
                if (current == null)
                    return null;
            }
            return current;
        }

        public static PropertyPath Parse(Type targetType,string propPath)
        {
            if (String.IsNullOrEmpty(propPath))
                return null;

            string[] parts = propPath.Split('.');
            return Compile(targetType, parts);
            
        }

        public static PropertyPath Compile(Type targetType,string[] pathParts)
        {
            var accessors = new IMemberAccessor[pathParts.Length];

            Type currentType = targetType;
            for (int i = 0; i < pathParts.Length; i++)
            {
                string part = pathParts[i];

                IMemberAccessor accessor = currentType.FindAccessor(part);
                if (accessor == null)
                    return null;


                accessors[i] = accessor;
                currentType = accessor.MemberType;
            }

            return new PropertyPath(targetType, accessors);
        }

        public static string GetCacheKey(Type targetType,string[] name)
        {
            var key = new StringBuilder();
            key.Append(targetType.FullName);
            foreach (string namePart in name)
                key.Append(namePart);
            return key.ToString();
        }

        

    }
}
