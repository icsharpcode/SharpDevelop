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
using System.Reflection;


namespace ICSharpCode.Reporting.DataSource
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
