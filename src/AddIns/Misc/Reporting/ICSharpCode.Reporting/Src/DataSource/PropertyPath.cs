// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Text;

namespace ICSharpCode.Reporting.DataSource
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
            
//            Type s = target.GetType();
           
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
