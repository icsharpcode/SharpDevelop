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
using System.Globalization;

namespace ICSharpCode.Reporting.Expressions
{
	/// <summary>
	/// Description of TypeNormalizer.
	/// </summary>
	public  class TypeNormalizer
	{
		/*
		public static void NormalizeTypes(ref object left,ref object right)
		{
			NormalizeTypes(ref left,ref right,0);
		}

		public static void NormalizeTypes(ref object left,ref object right,object nullValue)
		{
			if (left == null)
				left = 0;
			if (right == null)
				right = 0;

			if (left.GetType() == right.GetType())
				return;

			try
			{
				right = Convert.ChangeType(right,left.GetType(),CultureInfo.CurrentCulture);
			}
			catch
			{
				try
				{
					left = Convert.ChangeType(left, right.GetType(),CultureInfo.CurrentCulture);
				}
				catch
				{
					throw new Exception(String.Format("Error converting from {0} type to {1}", left.GetType().FullName, right.GetType().FullName));
				}
			}
		}
		
		public static void EnsureTypes(ref object[] values,Type targetType)
		{
			object nullValue = null;
			if (targetType.IsValueType)
				nullValue = Activator.CreateInstance(targetType);
			EnsureTypes(ref values,targetType,nullValue);
			
		}

		public static void EnsureTypes(ref object[] values,Type targetType,object nullValue)
		{
			for (int i=0;i<values.Length;i++)
			{
				values[i] = EnsureType(values[i],targetType,nullValue);
			}
		}
*/
		public static T EnsureType<T>(object value)
		{
			return EnsureType<T>(value, default(T));
		}

		public static T EnsureType<T>(object value,object nullValue)
		{
			return (T) EnsureType(value, typeof (T), nullValue);
		}

		public static object EnsureType(object value, Type targetType)
		{
			if (value != null && value.GetType() == targetType)
				return value;

			object defaultValue = null;
			if (targetType.IsValueType)
				defaultValue = Activator.CreateInstance(targetType);
			return EnsureType(value, targetType, defaultValue);
		}

		public static object EnsureType(object value,Type targetType,object nullValue)
		{
			if (value == null)
				return nullValue;
			
			if (targetType == typeof(object))
				return value;

			if (value.GetType() == targetType)
				return value;

			try {
				return Convert.ChangeType(value, targetType,CultureInfo.CurrentCulture);
			} catch (Exception e) {
				
				Console.WriteLine("TypeNormalizer {0} - {1}",value.ToString(),e.Message);
				return value.ToString();
				//throw new Exception()String.Format("TypeNormalizer for  <{0}> - {1}",value.ToString(),e.Message));
			}
		}
		
	}
}
